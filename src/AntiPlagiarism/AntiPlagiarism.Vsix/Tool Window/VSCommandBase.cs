using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using AntiPlagiarism.Core.Utilities.Common;
using AntiPlagiarism.Vsix.Utilities;

using Shell = Microsoft.VisualStudio.Shell;


namespace AntiPlagiarism.Vsix
{
	/// <summary>
	/// Base command handler
	/// </summary>
	internal abstract class VSCommandBase
	{
		protected const int NOT_INITIALIZED = 0, INITIALIZED = 1;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		protected Guid DefaultCommandSet { get; } = new Guid(AntiPlagiarismPackage.AntiPlagiarismDefaultCommandSetGuidString);

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		protected AsyncPackage Package { get; }

		/// <summary>
		/// Initializes a new instance of the command.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		protected VSCommandBase(AsyncPackage package, OleMenuCommandService commandService, int commandID, Guid? customCommandSet = null)
		{
			package.ThrowOnNull(nameof(package));
			commandService.ThrowOnNull(nameof(commandService));

			Package = package;
			Guid commandSet = customCommandSet ?? DefaultCommandSet;

			if (commandService != null)
			{
				var menuCommandID = new CommandID(commandSet, commandID);
				var menuItem = new OleMenuCommand(CommandCallback, menuCommandID);
				menuItem.BeforeQueryStatus += QueryFormatButtonStatus;
				commandService.AddCommand(menuItem);
			}
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		protected Shell.IAsyncServiceProvider ServiceProvider => Package;
		
		protected virtual void QueryFormatButtonStatus(object sender, EventArgs e)
		{
			if (!(sender is OleMenuCommand menuCommand))
				return;

			ThreadHelper.ThrowIfNotOnUIThread();
			DTE2 dte = ThreadHelper.JoinableTaskFactory.Run(() => ServiceProvider.GetServiceAsync<DTE, DTE2>());
			bool visible = false;
			bool enabled = false;

			if (dte?.ActiveDocument != null)
			{
				string fileExtension = System.IO.Path.GetExtension(dte.ActiveDocument.FullName);
				visible = !fileExtension.IsNullOrEmpty() && fileExtension.Equals(".cs", StringComparison.OrdinalIgnoreCase);
				enabled = !dte.ActiveDocument.ReadOnly;
			}

			menuCommand.Visible = visible;
			menuCommand.Enabled = enabled;
		}

		protected abstract void CommandCallback(object sender, EventArgs e);

		protected static Task<OleMenuCommandService> GetOleCommandServiceAsync(Shell.IAsyncServiceProvider serviceProvider) =>
			serviceProvider.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
	}
}
