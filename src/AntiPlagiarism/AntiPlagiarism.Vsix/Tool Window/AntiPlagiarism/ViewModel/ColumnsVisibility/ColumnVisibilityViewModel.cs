using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using System.Threading.Tasks;


namespace AntiPlagiarism.Vsix.ToolWindows
{
    public class ColumnVisibilityViewModel : ViewModelBase
	{
		private bool _isVisible;

		public bool IsVisible
		{
			get => _isVisible;
			set
			{
				if (_isVisible != value)
				{
					_isVisible = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string ColumnName { get; }

		public ColumnVisibilityViewModel(string columnName, bool isVisible = true)
		{
			columnName.ThrowOnNullOrWhiteSpace(nameof(columnName));

			ColumnName = columnName;
			_isVisible = isVisible;
		}
	}
}
