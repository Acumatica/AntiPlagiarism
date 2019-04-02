using AntiPlagiarism.Core.Utilities;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AntiPlagiarism.Vsix.ToolWindows
{
    public class Command : ICommand
	{
		private readonly Action<object> _actionToExecute;
		private readonly Predicate<object> _canExecute;

		#region ICommand Members
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

		public void Execute(object parameter) => _actionToExecute(parameter);
		#endregion

		/// <summary>
		/// Creates a new command that can always execute.
		/// </summary>
		/// <param name="actionToExecute">The execution logic.</param>
		public Command(Action<object> actionToExecute) : this(actionToExecute, null)
		{
		}

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="actionToExecute">The execution logic.</param>
		/// <param name="canExecute">The predicate to determine if action can execute.</param>
		public Command(Action<object> actionToExecute, Predicate<object> canExecute)
		{
			actionToExecute.ThrowOnNull(nameof(actionToExecute));
				
			_actionToExecute = actionToExecute;
			_canExecute = canExecute;
		}	
	}
}
