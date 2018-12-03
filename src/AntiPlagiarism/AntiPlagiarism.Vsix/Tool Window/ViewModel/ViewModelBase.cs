using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AntiPlagiarism.Core.Utilities.Common;


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
