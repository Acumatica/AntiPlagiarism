using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Text.Editor;
using AntiPlagiarism.Vsix.Utilities;
using AntiPlagiarism.Core.Utilities.Common;


using FirstChanceExceptionEventArgs = System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs;


namespace AntiPlagiarism.Vsix.Logger
{
	/// <summary>
	/// An AntiPlagiarism logger used to log unhandled exceptions.
	/// </summary>
	internal class AntiPlagiarismLogger : IDisposable
	{
		public const string PackageName = "AntiPlagiarism";

		private readonly string CoreDll = typeof(Core.PlagiarismScanner).Assembly.GetName().Name;
		private readonly string VsixDll = typeof(AntiPlagiarismLogger).Assembly.GetName().Name;

		private readonly AntiPlagiarismPackage _package;

		public AntiPlagiarismLogger(AntiPlagiarismPackage antiPlagiarismPackage)
		{
			antiPlagiarismPackage.ThrowOnNull(nameof(antiPlagiarismPackage));
			_package = antiPlagiarismPackage;
			AppDomain.CurrentDomain.FirstChanceException += AntiPlagiarism_FirstChanceException;
		}

		private void AntiPlagiarism_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
		{			
			LogExceptionAsync(e.Exception)
				.FileAndForget($"vs/{PackageName}/{nameof(AntiPlagiarismLogger)}/{nameof(LogExceptionAsync)}");
		}

		public async System.Threading.Tasks.Task LogExceptionAsync(Exception exception)
		{		
			if (exception == null || (exception.Source != CoreDll && exception.Source != VsixDll))
				return;

			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
			IWpfTextView activeTextView = await _package.GetWpfTextViewAsync();

			if (activeTextView == null)
				return;

			Document currentDocument = activeTextView.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges();

			if (currentDocument == null)
				return;

			string logMessage = CreateLogMessageFromException(exception, currentDocument);
			ActivityLog.TryLogError(PackageName, logMessage);
		}

		private string CreateLogMessageFromException(Exception exception, Document currentDocument)
		{
			StringBuilder messageBuilder = new StringBuilder(capacity: 256);
			messageBuilder.AppendLine($"FILE PATH: {currentDocument.FilePath}")
						  .AppendLine($"MESSAGE: {exception.Message}")
						  .AppendLine($"STACK TRACE: {exception.StackTrace}")
						  .AppendLine($"TARGET SITE: {exception.TargetSite}")
						  .AppendLine($"SOURCE: {exception.Source}");

			return messageBuilder.ToString();
		}

		public void Dispose()
		{
			AppDomain.CurrentDomain.FirstChanceException -= AntiPlagiarism_FirstChanceException;
		}		
	}
}
