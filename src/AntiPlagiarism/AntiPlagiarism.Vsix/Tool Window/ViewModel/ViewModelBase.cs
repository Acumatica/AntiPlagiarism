using AntiPlagiarism.Core.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace AntiPlagiarism.Vsix.ToolWindows
{
    public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		public virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
		{
			if (propertyName.IsNullOrWhiteSpace())
				return;

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public virtual void RaiseAllPropertyChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
	}
}
