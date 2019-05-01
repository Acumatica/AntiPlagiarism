using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using AntiPlagiarism.Vsix.Utilities.Navigation;

using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;


namespace AntiPlagiarism.Vsix.ToolWindows
{
	public class SettingsViewModel : ViewModelBase
	{
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

		public SettingsViewModel()
		{
			var workModes = GetReferenceWorkModes();
			ReferenceWorkModes = new ExtendedObservableCollection<WorkModeViewModel<ReferenceWorkMode>>(workModes);
			_selectedReferenceWorkMode = ReferenceWorkModes.FirstOrDefault(mode => mode.WorkMode == ReferenceWorkMode.ReferenceSolution);

			var sourceOriginModes = GetSourceOriginModes();
			SourceOriginModes = new ExtendedObservableCollection<WorkModeViewModel<SourceOriginMode>>(sourceOriginModes);
			_selectedSourceOriginMode = SourceOriginModes.FirstOrDefault(mode => mode.WorkMode == SourceOriginMode.CurrentSolution);
		}

		internal void FillColumnsVisibility(IEnumerable<string> columnNames)
		{
			if (columnNames.IsNullOrEmpty())
				return;

			ColumnsVisibilityCollectionViewModel = new ColumnsVisibilityCollectionViewModel(columnNames);
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
			yield return WorkModeViewModel.New(SourceOriginMode.CurrentProject, VSIXResource.CurrentProjectSourceOriginTitle,
											   VSIXResource.CurrentProjectSourceOriginDescription);
		}
	}
}
