using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;


namespace AntiPlagiarism.Vsix.Utilities
{
    public static class VSColors
    {
		private const int NOT_INITIALIZED = 0, INITIALIZED = 1;

		private const byte RedCriteria = 128;
        private const byte GreenCriteria = 128;
        private const byte BlueCriteria = 128;

        private static IVsUIShell5 _vsUIShell5;
		private static int _serviceInitialized = NOT_INITIALIZED;

        public static bool IsDarkTheme()
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			if (Interlocked.Exchange(ref _serviceInitialized, INITIALIZED) == NOT_INITIALIZED)
			{
				_vsUIShell5 = ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell)) as IVsUIShell5;
			}

            if (_vsUIShell5 == null)
                return true;

            Color editorBackgroundColor = _vsUIShell5.GetThemedWPFColor(EnvironmentColors.DarkColorKey);

            if (editorBackgroundColor == null)
                return true;

            return editorBackgroundColor.R < RedCriteria ||
                   editorBackgroundColor.G < GreenCriteria ||
                   editorBackgroundColor.B < BlueCriteria;
        }
    }
}
