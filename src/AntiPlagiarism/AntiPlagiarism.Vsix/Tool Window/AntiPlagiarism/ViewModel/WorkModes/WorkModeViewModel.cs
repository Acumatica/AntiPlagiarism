using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using AntiPlagiarism.Vsix.Utilities.Navigation;

using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;


namespace AntiPlagiarism.Vsix.ToolWindows
{
	public class WorkModeViewModel
	{
		public WorkMode WorkMode { get; }

		public string Name { get; }

		public WorkModeViewModel(WorkMode workMode, string name)
		{
			name.ThrowOnNullOrWhiteSpace(nameof(name));

			WorkMode = workMode;
			Name = name;
		}
	}
}
