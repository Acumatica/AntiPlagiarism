using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;

using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;


namespace AntiPlagiarism.Vsix.ToolWindows
{
	public class SettingsViewModel : ViewModelBase
	{
		public AntiPlagiarismWindowViewModel ParentViewModel { get; }

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

		public ExtendedObservableCollection<WorkModeViewModel<ReferenceWorkMode>> ReferenceWorkModes { get; }

		private WorkModeViewModel<ReferenceWorkMode> _selectedReferenceWorkMode;

		public WorkModeViewModel<ReferenceWorkMode> SelectedReferenceWorkMode
		{
			get => _selectedReferenceWorkMode;
			set
			{
				if (_selectedReferenceWorkMode != value)
				{
					_selectedReferenceWorkMode = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ExtendedObservableCollection<WorkModeViewModel<SourceOriginMode>> SourceOriginModes { get; }

		private WorkModeViewModel<SourceOriginMode> _selectedSourceOriginMode;

		public WorkModeViewModel<SourceOriginMode> SelectedSourceOriginMode
		{
			get => _selectedSourceOriginMode;
			set
			{
				if (_selectedSourceOriginMode != value)
				{
					_selectedSourceOriginMode = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ExtendedObservableCollection<ProjectViewModel> Projects { get; } = new ExtendedObservableCollection<ProjectViewModel>();

		private ProjectViewModel _selectedProject;

		public ProjectViewModel SelectedProject
		{
			get => _selectedProject;
			set
			{
				if (_selectedProject != value)
				{
					_selectedProject = value;
					NotifyPropertyChanged();
				}
			}
		}

		private bool _showOnlyItemsExceedingThreshold;

		public bool ShowOnlyItemsExceedingThreshold
		{
			get => _showOnlyItemsExceedingThreshold;
			set
			{
				if (_showOnlyItemsExceedingThreshold != value)
				{
					_showOnlyItemsExceedingThreshold = value;
					NotifyPropertyChanged();
				}
			}
		}

		private string _sourceFolderPath;

		public string SourceFolderPath
		{
			get => _sourceFolderPath;
			private set
			{
				if (_sourceFolderPath != value)
				{
					_sourceFolderPath = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Command SelectSourceFolderCommand { get; }

		public SettingsViewModel(AntiPlagiarismWindowViewModel antiPlagiarismWindowViewModel)
		{
			antiPlagiarismWindowViewModel.ThrowOnNull(nameof(antiPlagiarismWindowViewModel));
			ParentViewModel = antiPlagiarismWindowViewModel;

			var workModes = GetReferenceWorkModes();
			ReferenceWorkModes = new ExtendedObservableCollection<WorkModeViewModel<ReferenceWorkMode>>(workModes);
			_selectedReferenceWorkMode = ReferenceWorkModes.FirstOrDefault(mode => mode.WorkMode == ReferenceWorkMode.ReferenceSolution);

			var sourceOriginModes = GetSourceOriginModes();
			SourceOriginModes = new ExtendedObservableCollection<WorkModeViewModel<SourceOriginMode>>(sourceOriginModes);
			_selectedSourceOriginMode = SourceOriginModes.FirstOrDefault(mode => mode.WorkMode == SourceOriginMode.CurrentSolution);

			SelectSourceFolderCommand = new Command(p => SelectSourceFolder());
		}

		internal void FillColumnsVisibility(IEnumerable<string> columnNames)
		{
			if (columnNames.IsNullOrEmpty())
				return;

			ColumnsVisibilityCollectionViewModel = new ColumnsVisibilityCollectionViewModel(columnNames);
		}

		internal async Task RefillProjectsAsync()
		{
			var workspace = await AntiPlagiarismPackage.Instance.GetVSWorkspaceAsync()
																.ConfigureAwait(continueOnCapturedContext: true);
			var projectVMs = workspace?.CurrentSolution?.Projects?.Select(project => new ProjectViewModel(project)) ??
							 Enumerable.Empty<ProjectViewModel>();
			Projects.Reset(projectVMs);
			SelectedProject = Projects.FirstOrDefault();
		}

		private IEnumerable<WorkModeViewModel<ReferenceWorkMode>> GetReferenceWorkModes()
		{
			yield return WorkModeViewModel.New(ReferenceWorkMode.SelfAnalysis, VSIXResource.SelfAnalysisWorkModeTitle,
											   VSIXResource.SelfAnalysisWorkModeDescription);
			yield return WorkModeViewModel.New(ReferenceWorkMode.ReferenceSolution, VSIXResource.ReferenceSolutionWorkModeTitle,
											   VSIXResource.ReferenceSolutionWorkModeDescription);
			yield return WorkModeViewModel.New(ReferenceWorkMode.AcumaticaSources, VSIXResource.AcumaticaSourcesWorkModeTitle,
											   VSIXResource.AcumaticaSourcesWorkModeDescription);
		}

		private IEnumerable<WorkModeViewModel<SourceOriginMode>> GetSourceOriginModes()
		{
			yield return WorkModeViewModel.New(SourceOriginMode.CurrentSolution, VSIXResource.CurrentSolutionSourceOriginTitle,
											   VSIXResource.CurrentSolutionSourceOriginDescription);
			yield return WorkModeViewModel.New(SourceOriginMode.SelectedProject, VSIXResource.SelectedProjectSourceOriginTitle,
											   VSIXResource.SelectedProjectSourceOriginDescription);
			yield return WorkModeViewModel.New(SourceOriginMode.SelectedFolder, VSIXResource.SelectedFolderSourceOriginTitle,
											   VSIXResource.SelectedFolderSourceOriginDescription);
		}

		private void SelectSourceFolder()
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
	}
}
