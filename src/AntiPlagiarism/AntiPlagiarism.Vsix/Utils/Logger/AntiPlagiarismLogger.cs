﻿using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;
using System;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FirstChanceExceptionEventArgs = System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs;

namespace AntiPlagiarism.Vsix.Logger
{
    /// <summary>
    /// An AntiPlagiarism logger used to log unhandled exceptions.
    /// </summary>
    internal class AntiPlagiarismLogger : IDisposable
	{
		private readonly string CoreDll = typeof(Core.Plagiarism.PlagiarismScanner).Assembly.GetName().Name;
		private readonly string VsixDll = typeof(AntiPlagiarismLogger).Assembly.GetName().Name;

		private readonly AntiPlagiarismPackage _package;
		private readonly bool _swallowUnobservedTaskExceptions;

		public AntiPlagiarismLogger(AntiPlagiarismPackage antiPlagiarismPackage, bool swallowUnobservedTaskExceptions)
		{
			antiPlagiarismPackage.ThrowOnNull(nameof(antiPlagiarismPackage));
			_package = antiPlagiarismPackage;
			_swallowUnobservedTaskExceptions = swallowUnobservedTaskExceptions;

			AppDomain.CurrentDomain.FirstChanceException += AntiPlagiarism_FirstChanceException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
		}

		private void AntiPlagiarism_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
		{
			LogException(e.Exception, logOnlyFromAcuminatorAssemblies: true, LogMode.Warning);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception exception)
			{
				LogMode logMode = e.IsTerminating
					? LogMode.Error
					: LogMode.Warning;

				LogException(exception, logOnlyFromAcuminatorAssemblies: false, logMode);
			}
		}

		private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			LogMode logMode = LogMode.Error;

			if (_swallowUnobservedTaskExceptions && !e.Observed)
			{
				e.SetObserved();
				logMode = LogMode.Warning;
			}

			foreach (Exception exception in e.Exception.Flatten().InnerExceptions)
			{
				LogException(exception, logOnlyFromAcuminatorAssemblies: false, logMode);
			}
		}

		public void LogException(Exception exception, bool logOnlyFromAcuminatorAssemblies, LogMode logMode,
								[CallerMemberName]string reportedFrom = null)
		{
			if (exception == null || logMode == LogMode.None)
				return;
			else if (logOnlyFromAcuminatorAssemblies && exception.Source != CoreDll && exception.Source != VsixDll)
				return;

			string logMessage = CreateLogMessageFromException(exception, logMode, reportedFrom);

			switch (logMode)
			{
				case LogMode.Information:
					ActivityLog.TryLogInformation(AntiPlagiarismPackage.PackageName, logMessage);
					break;
				case LogMode.Warning:
					ActivityLog.TryLogWarning(AntiPlagiarismPackage.PackageName, logMessage);
					break;
				case LogMode.Error:
					ActivityLog.TryLogError(AntiPlagiarismPackage.PackageName, logMessage);
					break;
			}
		}

		private string CreateLogMessageFromException(Exception exception, LogMode logMode, string reportedFrom)
		{

			StringBuilder messageBuilder = new StringBuilder(capacity: 256);

			if (logMode == LogMode.Error)
			{
				messageBuilder.AppendLine($"{AntiPlagiarismPackage.PackageName.ToUpper()} CAUSED CRITICAL ERROR|");
			}

			messageBuilder.AppendLine($"*EXCEPTION TYPE*: {exception.GetType().Name}")
						  .AppendLine($"|*MESSAGE*: {exception.Message}")
						  .AppendLine($"|*STACK TRACE*: {exception.StackTrace}")
						  .AppendLine($"|*TARGET SITE*: {exception.TargetSite}")
						  .AppendLine($"|*SOURCE*: {exception.Source}")
						  .AppendLine($"|*REPORTED FROM*: {reportedFrom}");

			return messageBuilder.ToString();
		}

		public void Dispose()
		{
			AppDomain.CurrentDomain.FirstChanceException -= AntiPlagiarism_FirstChanceException;
			AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
			TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
		}
	}
}
