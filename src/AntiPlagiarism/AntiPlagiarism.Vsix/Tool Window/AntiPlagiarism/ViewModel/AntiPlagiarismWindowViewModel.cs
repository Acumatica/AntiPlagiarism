using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using Microsoft.VisualStudio.Threading;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

namespace AntiPlagiarism.Vsix.ToolWindows
{
	public class AntiPlagiarismWindowViewModel : ToolWindowViewModelBase
	{
		private CancellationTokenSource _cancellationTokenSource;

		private ColumnsVisibilityCollectionViewModel _columnsVisibilityCollectionViewModel;

		public ColumnsVisibilityCollectionViewModel ColumnsVisibilityCollectionViewModel
		{
			get => _columnsVisibilityCollectionViewModel;
			private set
			{
				if (_columnsVisibilityCollectionViewModel != value)
				{
					_columnsVisibilityCollectionViewModel = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ExtendedObservableCollection<WorkModeViewModel> WorkModes { get; }

		private WorkModeViewModel _selectedWorkMode;

		public WorkModeViewModel SelectedWorkMode
		{
			get => _selectedWorkMode;
			set
			{
				if (_selectedWorkMode != value)
				{
					_selectedWorkMode = value;
					NotifyPropertyChanged();
				}
			}
		}

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
					throw new ArgumentOutOfRangeException(nameof(value), "The value should be in range between 0 and 100");
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
					throw new ArgumentOutOfRangeException(nameof(value), 
								$"The value should be in range between {MinCheckedMethodSizeLowerBound} and {MinCheckedMethodSizeUpperBound}");
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
			var workModes = GetWorkModes();
			WorkModes = new ExtendedObservableCollection<WorkModeViewModel>(workModes);
			_selectedWorkMode = WorkModes.FirstOrDefault();

            OpenReferenceSolutionCommand = new Command(p => OpenReferenceSolution());
			RunAnalysisCommand = new Command(p => RunAntiplagiatorAsync().Forget());
			CancelAnalysisCommand = new Command(p => CancelAntiplagiator());
		}

		public override void FreeResources()
		{
			base.FreeResources();
			_cancellationTokenSource?.Dispose();
		}

		internal void FillColumnsVisibility(IEnumerable<string> columnNames)
		{
			if (columnNames.IsNullOrEmpty())
				return;

			ColumnsVisibilityCollectionViewModel = new ColumnsVisibilityCollectionViewModel(this, columnNames);
		}

		private IEnumerable<WorkModeViewModel> GetWorkModes()
		{
			yield return new WorkModeViewModel(WorkMode.SelfAnalysis, VSIXResource.SelfAnalysisWorkModeTitle,
											   VSIXResource.SelfAnalysisWorkModeDescription);
			yield return new WorkModeViewModel(WorkMode.ReferenceSolution, VSIXResource.ReferenceSolutionWorkModeTitle,
											   VSIXResource.ReferenceSolutionWorkModeDescription);
			yield return new WorkModeViewModel(WorkMode.AcumaticaSources, VSIXResource.AcumaticaSourcesWorkModeTitle,
											   VSIXResource.AcumaticaSourcesWorkModeDescription);
		}

		private void OpenReferenceSolution()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Solution files (*.sln)|*.sln|Project files (*.csproj)|*.csproj|All files (*.*)|*.*",
				DefaultExt = "csproj",
				AddExtension = true,
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = false,
				Title = "Select reference solution file"
			};

			if (openFileDialog.ShowDialog() != true || openFileDialog.FileName.IsNullOrWhiteSpace() || !File.Exists(openFileDialog.FileName))
				return;

			string extension = Path.GetExtension(openFileDialog.FileName);

			if (extension != MethodReader.SolutionExtension && extension != MethodReader.ProjectExtension)
				return;

			ReferenceSolutionPath = openFileDialog.FileName;
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
			string solutionPath = await AntiPlagiarismPackage.Instance.GetSolutionPathAsync();
			string referenceSolutionPath = SelectedWorkMode.WorkMode == WorkMode.SelfAnalysis
				? solutionPath
				: ReferenceSolutionPath;

			if (referenceSolutionPath.IsNullOrWhiteSpace() || solutionPath.IsNullOrWhiteSpace() || cancellationToken.IsCancellationRequested)
				return;

			int tabSize = await AntiPlagiarismPackage.Instance.GetTabSizeAsync();

			await TaskScheduler.Default;		 //switch to background thread
			
			double threshholdFraction = ThreshholdPercent / 100.0;
			PlagiarismScanner plagiarismScanner = new PlagiarismScanner(referenceSolutionPath, solutionPath, 
																		threshholdFraction, MinCheckedMethodSize);
			IEnumerable<PlagiarismInfo> plagiatedItems = plagiarismScanner.Scan(callFromVS: true) ?? Enumerable.Empty<PlagiarismInfo>();

			if (SelectedWorkMode.WorkMode == WorkMode.SelfAnalysis)
			{
				plagiatedItems = plagiatedItems.Where(item => !item.Input.Equals(item.Reference));
			}

			plagiatedItems = plagiatedItems.OrderByDescending(item => item.Similarity);

			if (cancellationToken.IsCancellationRequested)
				return;

			string sourceSolutionDir = Path.GetDirectoryName(solutionPath) + Path.DirectorySeparatorChar;
			string referenceSolutionDir = Path.GetDirectoryName(referenceSolutionPath) + Path.DirectorySeparatorChar;
			var plagiatedItemVMs = plagiatedItems.Select(item => new PlagiarismInfoViewModel(this, item, referenceSolutionDir, sourceSolutionDir, tabSize))
												 .ToList();			//Make sure to create View Models on the background thread via buffering operation
			
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			PlagiatedItems.AddRange(plagiatedItemVMs);				//Add view models on UI thread
		}	
	}
}
