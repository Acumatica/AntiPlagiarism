using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.LanguageServices;
using AntiPlagiarism.Core.Utilities.Common;

using Shell = Microsoft.VisualStudio.Shell;


namespace AntiPlagiarism.Vsix.Utilities
{
    /// <summary>
    /// The Visual Studio services extensions. Access only from UI thread!
    /// </summary>
    internal static class VSServicesExtensions
	{
		public static async Task<TService> GetServiceAsync<TService>(this Shell.IAsyncServiceProvider serviceProvider)
		where TService : class
		{
			if (serviceProvider == null)
				return null;

			var service = await serviceProvider.GetServiceAsync(typeof(TService));
			return service as TService;
		}

		public static async Task<TActual> GetServiceAsync<TRequested, TActual>(this Shell.IAsyncServiceProvider serviceProvider)
		where TRequested : class
		where TActual : class
		{
			if (serviceProvider == null)
				return null;

			var service = await serviceProvider.GetServiceAsync(typeof(TRequested));
			return service as TActual;
		}

		internal static async Task<VisualStudioWorkspace> GetVSWorkspaceAsync(this Shell.IAsyncServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				return null;

			IComponentModel componentModel = (await serviceProvider.GetServiceAsync(typeof(SComponentModel))) as IComponentModel;
			return componentModel?.GetService<VisualStudioWorkspace>();
		}

		internal static async Task<string> GetSolutionPathAsync(this Shell.IAsyncServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				return null;

			VisualStudioWorkspace workspace = await serviceProvider.GetVSWorkspaceAsync();	
			return workspace?.CurrentSolution?.FilePath ?? string.Empty;
		}

		internal static async Task<IOutliningManager> GetOutliningManagerAsync(this Shell.IAsyncServiceProvider serviceProvider, ITextView textView)
		{
			if (serviceProvider == null || textView == null)
				return null;

			IComponentModel componentModel = await serviceProvider.GetServiceAsync<SComponentModel, IComponentModel>();
			IOutliningManagerService outliningManagerService = componentModel?.GetService<IOutliningManagerService>();

			if (outliningManagerService == null)
				return null;
			
			return outliningManagerService.GetOutliningManager(textView);
		}

		internal static async Task<IWpfTextView> GetWpfTextViewAsync(this Shell.IAsyncServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				return null;

			IVsTextManager textManager = await serviceProvider.GetServiceAsync<SVsTextManager, IVsTextManager>();
			
			if (textManager == null || textManager.GetActiveView(1, null, out IVsTextView textView) != VSConstants.S_OK)
				return null;

			return await serviceProvider.GetWpfTextViewFromTextViewAsync(textView);
		}

		/// <summary>
		/// Returns an IVsTextView for the given file path, if the given file is open in Visual Studio.
		/// </summary>
		/// <param name="packageServiceProvider">The package Service Provider.</param>
		/// <param name="filePath">Full Path of the file you are looking for.</param>
		/// <returns>
		/// The IVsTextView for this file, if it is open, null otherwise.
		/// </returns>
		internal static async Task<IWpfTextView> GetWpfTextViewByFilePathAsync(this Shell.IAsyncServiceProvider packageServiceProvider,
																			   string filePath)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			if (filePath.IsNullOrWhiteSpace() || packageServiceProvider == null)
				return null;

			DTE2 dte2 = await packageServiceProvider.GetServiceAsync<SDTE, DTE2>();
			var oleServiceProvider = dte2 as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

			if (dte2 == null || oleServiceProvider == null)
				return null;

			ServiceProvider shellServiceProvider = new ServiceProvider(oleServiceProvider);


			if (VsShellUtilities.IsDocumentOpen(shellServiceProvider, filePath, Guid.Empty, 
												out IVsUIHierarchy uiHierarchy, out uint itemID, out IVsWindowFrame windowFrame))
			{
				IVsTextView textView = VsShellUtilities.GetTextView(windowFrame);   // Get the IVsTextView from the windowFrame
				return await packageServiceProvider.GetWpfTextViewFromTextViewAsync(textView);
			}

			return null;
		}

		private static async Task<IWpfTextView> GetWpfTextViewFromTextViewAsync(this Shell.IAsyncServiceProvider serviceProvider, 
																				IVsTextView vsTextView)
		{
			if (vsTextView == null)
				return null;

			IComponentModel componentModel = await serviceProvider.GetServiceAsync<SComponentModel, IComponentModel>();
			IVsEditorAdaptersFactoryService vsEditorAdaptersFactoryService = componentModel?.GetService<IVsEditorAdaptersFactoryService>();
			return vsEditorAdaptersFactoryService?.GetWpfTextView(vsTextView);
		}
	}
}
