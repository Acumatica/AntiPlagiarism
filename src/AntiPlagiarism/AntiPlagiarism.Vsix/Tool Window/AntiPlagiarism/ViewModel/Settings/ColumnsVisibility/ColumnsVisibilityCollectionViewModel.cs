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

		public ReadOnlyObservableCollection<ColumnVisibilityViewModel> Columns { get; }

		public ColumnVisibilityViewModel this[string name] => 
			name.IsNullOrWhiteSpace()
				? null
				: _columnVisibilitiesByName.TryGetValue(name, out var columnVM)
					? columnVM
					: null;

		public ColumnsVisibilityCollectionViewModel(IEnumerable<string> columnNames)
		{
			columnNames.ThrowOnNull(nameof(columnNames));

			_columnVisibilitiesByName = columnNames.Distinct()
												   .Select(name => new ColumnVisibilityViewModel(name))
												   .ToDictionary(colName => colName.ColumnName);
			_columns = new ObservableCollection<ColumnVisibilityViewModel>(_columnVisibilitiesByName.Values);
			Columns = new ReadOnlyObservableCollection<ColumnVisibilityViewModel>(_columns);
		}
	}
}
