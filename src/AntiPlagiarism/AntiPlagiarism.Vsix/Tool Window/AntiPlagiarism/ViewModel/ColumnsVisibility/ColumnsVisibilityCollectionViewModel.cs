using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;



namespace AntiPlagiarism.Vsix.ToolWindows
{
    public class ColumnsVisibilityCollectionViewModel : ViewModelBase
	{
		private readonly Dictionary<string, ColumnVisibilityViewModel> _columnVisibilitiesByName;
		private readonly ObservableCollection<ColumnVisibilityViewModel> _columns;

		public AntiPlagiarismWindowViewModel ParentViewModel { get; }

		public ReadOnlyObservableCollection<ColumnVisibilityViewModel> Columns { get; }

		public ColumnsVisibilityCollectionViewModel(AntiPlagiarismWindowViewModel parentViewModel,
													IEnumerable<ColumnVisibilityViewModel> columnVMs)
		{
			parentViewModel.ThrowOnNull(nameof(parentViewModel));
			columnVMs.ThrowOnNull(nameof(columnVMs));

			ParentViewModel = parentViewModel;
			_columnVisibilitiesByName = columnVMs.ToDictionary(colName => colName.ColumnName);
			_columns = new ObservableCollection<ColumnVisibilityViewModel>(_columnVisibilitiesByName.Values);
			Columns = new ReadOnlyObservableCollection<ColumnVisibilityViewModel>(_columns);
		}
	}
}
