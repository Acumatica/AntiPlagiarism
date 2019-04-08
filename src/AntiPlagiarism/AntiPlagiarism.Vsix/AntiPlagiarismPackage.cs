using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using AntiPlagiarism.Vsix.ToolWindows;
using AntiPlagiarism.Vsix.Logger;
using EnvDTE;

using Task = System.Threading.Tasks.Task;



namespace AntiPlagiarism.Vsix
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, PackageAutoLoadFlags.BackgroundLoad)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, PackageAutoLoadFlags.BackgroundLoad)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.Debugging_string, PackageAutoLoadFlags.BackgroundLoad)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(AntiPlagiarismPackage.PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	[ProvideToolWindow(toolType: typeof(AntiPlagiarismWindow))]
	public sealed class AntiPlagiarismPackage : AsyncPackage
	{
		public const string PackageName = "AntiPlagiarism";

		/// <summary>
		/// AntiPlagiarism Package GUID string.
		/// </summary>
		public const string PackageGuidString = "f571df7b-c776-4783-859e-9946ff1ce156";

		/// <summary>
		/// The AntiPlagiarism default command set GUID string.
		/// </summary>
		public const string AntiPlagiarismDefaultCommandSetGuidString = "ADE9FE1C-58B0-4CB7-A6DC-177C794BF72B";

		private const int INSTANCE_UNINITIALIZED = 0;
		private const int INSTANCE_INITIALIZED = 1;
		private static int instanceInitialized;

		internal AntiPlagiarismLogger AntiPlagiarismLogger
		{
			get;
			private set;
		}

		public static AntiPlagiarismPackage Instance { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AntiPlagiarismPackage"/> class.
		/// </summary>
		public AntiPlagiarismPackage()
		{
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.

			SetupSingleton(this);
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread
			await base.InitializeAsync(cancellationToken, progress);

			if (Zombied)
				return;

			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			await OpenAntiPlagiarismWindowCommand.InitializeAsync(this);

			SubscribeOnSolutionEvents();
			InitializeLogger();
		}

		private static void SetupSingleton(AntiPlagiarismPackage package)
		{
			if (package == null)
				return;

			if (Interlocked.CompareExchange(ref instanceInitialized, INSTANCE_INITIALIZED, INSTANCE_UNINITIALIZED) == INSTANCE_UNINITIALIZED)
			{
				Instance = package;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			AntiPlagiarismLogger?.Dispose();

			if (ThreadHelper.CheckAccess())
			{
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
				if (GetService(typeof(DTE)) is DTE dte)
				{
					dte.Events.SolutionEvents.AfterClosing -= SolutionEvents_AfterClosing;
				}
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
			}
		}

		protected override int QueryClose(out bool canClose)
		{
			CloseOpenToolWindows();
			return base.QueryClose(out canClose);
		}

		private void InitializeLogger()
		{
			try
			{
				AntiPlagiarismLogger = new AntiPlagiarismLogger(this);
			}
			catch (Exception ex)
			{
				ActivityLog.TryLogError(PackageName,
					$"An error occurred during the logger initialization: ({ex.GetType().Name}, message: \"{ex.Message}\")");
			}
		}

		private void SubscribeOnSolutionEvents()
		{
			if (!ThreadHelper.CheckAccess())
				return;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
			if (GetService(typeof(DTE)) is DTE dte)
			{
				dte.Events.SolutionEvents.AfterClosing += SolutionEvents_AfterClosing;
			}
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
		}

		private void SolutionEvents_AfterClosing()
		{
			CloseOpenToolWindows();
		}

		private void CloseOpenToolWindows()
		{
			if (!ThreadHelper.CheckAccess())
				return;

			try
			{
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread

				DTE dte = GetService(typeof(DTE)) as DTE;
				dte?.Windows.Item($"{{{AntiPlagiarismWindow.AntiPlagiarismWindowGuidString}}}")?.Close();	
				
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
			}
			catch
			{
			}
		}
	}
}