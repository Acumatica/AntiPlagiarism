using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
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
		/// True if the command can modify document in some way - text or properties.
		/// </summary>
		protected abstract bool CanModifyDocument { get; }

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
				var menuItem = new OleMenuCommand(CommandCallback, menuCommandID)
				{
					// This defers the visibility logic back to the VisibilityConstraints in the .vsct file
					Supported = false
				};
				commandService.AddCommand(menuItem);
			}
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		protected Shell.IAsyncServiceProvider ServiceProvider => Package;

		protected abstract void CommandCallback(object sender, EventArgs e);

		protected static Task<OleMenuCommandService> GetOleCommandServiceAsync(Shell.IAsyncServiceProvider serviceProvider) =>
			serviceProvider.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
	}
}
