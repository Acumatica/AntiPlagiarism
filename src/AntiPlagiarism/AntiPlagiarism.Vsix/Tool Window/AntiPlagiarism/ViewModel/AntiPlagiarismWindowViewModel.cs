using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using AntiPlagiarism.Core.Method;

using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;


namespace AntiPlagiarism.Vsix.ToolWindows
{
	public class AntiPlagiarismWindowViewModel : ToolWindowViewModelBase
	{
		private CancellationTokenSource _cancellationTokenSource;

		public SettingsViewModel Settings { get; }		

		public ExtendedObservableCollection<PlagiarismInfoViewModel> PlagiatedItems { get; }
		 
		private PlagiarismInfoViewModel _selectedItem;

		public PlagiarismInfoViewModel SelectedItem
		{
			get => _selectedItem; 
			set
			{
				if (_selectedItem == value)
					return;

				PlagiarismInfoViewModel oldSelectedItem = _selectedItem;

				if (oldSelectedItem != null)
				{
					oldSelectedItem.AreCodeFragmentsVisible = false;
				}

				_selectedItem = value;
				NotifyPropertyChanged();			
			}
		}

		private bool _isAnalysisRunning;

		public bool IsAnalysisRunning
		{
			get => _isAnalysisRunning;
			private set
			{
				if (_isAnalysisRunning != value)
				{
					_isAnalysisRunning = value;
					NotifyPropertyChanged();
				}
			}
		}

        private string _referenceSolutionPath;

        public string ReferenceSolutionPath
        {
            get => _referenceSolutionPath;
            private set
            {
                if (_referenceSolutionPath != value)
                {
                    _referenceSolutionPath = value;
                    NotifyPropertyChanged();
                }
            }
        }

		private double _threshholdPercent = PlagiarismScanner.SimilarityThresholdDefault * 100;

		public double ThreshholdPercent
		{
			get => _threshholdPercent;
			set
			{
				if (_threshholdPercent == value)
					return;
			
				if (value < 0 || value > 100)
				{
					throw new ArgumentOutOfRangeException(message: "The value should be in range between 0 and 100",
														  innerException: null);
				}

				_threshholdPercent = value;
				NotifyPropertyChanged();

				PlagiatedItems.ForEach(itemVm => itemVm.NotifyPropertyChanged(nameof(itemVm.IsThresholdExceeded)));
			}
		}

		private const int MinCheckedMethodSizeLowerBound = 2;
		private const int MinCheckedMethodSizeUpperBound = 1000;

		private int _minCheckedMethodSize = PlagiarismScanner.DefaultMinMethodSize;

		/// <summary>
		/// The minimum number of statements in a checked method.
		/// </summary>
		public int MinCheckedMethodSize
		{
			get => _minCheckedMethodSize;
			set
			{
				if (_minCheckedMethodSize == value)
					return;

				if (value < MinCheckedMethodSizeLowerBound || value > MinCheckedMethodSizeUpperBound)
				{
					throw new ArgumentOutOfRangeException(message: $"The value should be in range between {MinCheckedMethodSizeLowerBound} and {MinCheckedMethodSizeUpperBound}",
														  innerException: null);
				}

				_minCheckedMethodSize = value;
				NotifyPropertyChanged();
			}
		}

		public Command OpenReferenceSolutionCommand { get; }

        public Command RunAnalysisCommand { get; }

		public Command CancelAnalysisCommand { get; }

		public AntiPlagiarismWindowViewModel()
		{
			PlagiatedItems = new ExtendedObservableCollection<PlagiarismInfoViewModel>();
			Settings = new SettingsViewModel(this);

            OpenReferenceSolutionCommand = new Command(p => OpenReferenceSolution());
			RunAnalysisCommand = new Command(p => RunAntiplagiatorAsync().Forget());
			CancelAnalysisCommand = new Command(p => CancelAntiplagiator());
		}

		public override void FreeResources()
		{
			base.FreeResources();
			_cancellationTokenSource?.Dispose();
		}

		private void OpenReferenceSolution()
		{
			if (Settings.SelectedReferenceWorkMode.WorkMode == ReferenceWorkMode.ReferenceSolution)
			{
				ReferenceSolutionPath = ReferenceSourcePathRetriever.GetReferenceSolutionFilePath() ?? ReferenceSolutionPath;
			}
			else if (Settings.SelectedReferenceWorkMode.WorkMode == ReferenceWorkMode.AcumaticaSources)
			{
				ReferenceSolutionPath = ReferenceSourcePathRetriever.GetAcumaticaSourcesFolderPath() ?? ReferenceSolutionPath;
			}			
		}

		private async Task RunAntiplagiatorAsync()
		{
			try
			{
				IsAnalysisRunning = true;
				
				using (_cancellationTokenSource = new CancellationTokenSource())
				{
					CancellationToken cancellationToken = _cancellationTokenSource.Token;
					await FillItemsAsync(cancellationToken).ConfigureAwait(false);
					await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
				}
			}
			finally
			{
				IsAnalysisRunning = false;
				_cancellationTokenSource = null;
			}
		}

		private void CancelAntiplagiator()
		{
			if (!IsAnalysisRunning)
				return;

			_cancellationTokenSource?.Cancel();
		}

		private async Task FillItemsAsync(CancellationToken cancellationToken)
		{
			PlagiatedItems.Clear();

			var workspace = await AntiPlagiarismPackage.Instance.GetVSWorkspaceAsync();
			string sourceSolutionPath = await GetSourceSolutionPathAsync();
			string referenceSolutionPath = Settings.SelectedReferenceWorkMode.WorkMode == ReferenceWorkMode.SelfAnalysis
				? sourceSolutionPath
				: ReferenceSolutionPath;

			if (referenceSolutionPath.IsNullOrWhiteSpace() || sourceSolutionPath.IsNullOrWhiteSpace() || cancellationToken.IsCancellationRequested)
				return;

			int tabSize = await AntiPlagiarismPackage.Instance.GetTabSizeAsync();

			await TaskScheduler.Default;		 //switch to background thread
			
			double threshholdFraction = ThreshholdPercent / 100.0;
			PlagiarismScanner plagiarismScanner = new PlagiarismScanner(referenceSolutionPath, sourceSolutionPath, 
																		threshholdFraction, MinCheckedMethodSize);
			IEnumerable<PlagiarismInfo> plagiatedItems = plagiarismScanner.Scan(callFromVS: true) ?? Enumerable.Empty<PlagiarismInfo>();

			if (Settings.SelectedReferenceWorkMode.WorkMode == ReferenceWorkMode.SelfAnalysis)
			{
				plagiatedItems = FilterPlagiarismSimmetricResultsOnSelfAnalysis(plagiatedItems);
			}

			plagiatedItems = plagiatedItems.OrderByDescending(item => item.Similarity);

			if (cancellationToken.IsCancellationRequested)
				return;

			string sourceSolutionDir = Path.GetDirectoryName(sourceSolutionPath) + Path.DirectorySeparatorChar;
			string referenceSolutionDir = Path.GetDirectoryName(referenceSolutionPath) + Path.DirectorySeparatorChar;
			var plagiatedItemVMs = plagiatedItems.Select(item => new PlagiarismInfoViewModel(this, item, referenceSolutionDir, 
																							 sourceSolutionDir, tabSize))
												 .ToList();			//Make sure to create View Models on the background thread via buffering operation
			
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			PlagiatedItems.AddRange(plagiatedItemVMs);				//Add view models on UI thread
		}	

		private async Task<string> GetSourceSolutionPathAsync()
		{
			switch (Settings.SelectedSourceOriginMode.WorkMode)
			{
				case SourceOriginMode.CurrentSolution:
					return await AntiPlagiarismPackage.Instance.GetSolutionPathAsync();
				case SourceOriginMode.SelectedProject:
					var workspace = await AntiPlagiarismPackage.Instance.GetVSWorkspaceAsync();
					return string.Empty;
					break;
				default:
					return string.Empty;
			}
		}

		private IEnumerable<PlagiarismInfo> FilterPlagiarismSimmetricResultsOnSelfAnalysis(IEnumerable<PlagiarismInfo> plagiatedItems)
		{
			plagiatedItems = plagiatedItems.Where(item => !item.Input.Equals(item.Reference));
			HashSet<MethodIndex> addedInputs = new HashSet<MethodIndex>();

			foreach (PlagiarismInfo item in plagiatedItems)
			{
				if (addedInputs.Contains(item.Reference))
					continue;

				addedInputs.Add(item.Input);
				yield return item;
			}
		}
	}
}
