using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;



namespace AntiPlagiarism.Vsix.ToolWindows
{
	/// <summary>
	/// Open AntiPlagiarism window command.
	/// </summary>
	internal sealed class OpenAntiPlagiarismWindowCommand : OpenToolWindowCommandBase<AntiPlagiarismWindow>
	{
		private static int _isCommandInitialized = NOT_INITIALIZED;

		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0101;

		private OpenAntiPlagiarismWindowCommand(AsyncPackage package, OleMenuCommandService commandService) : 
										   base(package, commandService, CommandId)
		{		
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static OpenAntiPlagiarismWindowCommand Instance
		{
			get;
			private set;
		}

		protected override bool CanModifyDocument => false;

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
			var oleCommandService = await GetOleCommandServiceAsync(package);

			if (Interlocked.CompareExchange(ref _isCommandInitialized, value: INITIALIZED, comparand: NOT_INITIALIZED) == NOT_INITIALIZED)
			{		
				Instance = new OpenAntiPlagiarismWindowCommand(package, oleCommandService);
			}
		}
	}
}
