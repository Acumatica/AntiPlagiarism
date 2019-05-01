using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;



namespace AntiPlagiarism.Vsix.ToolWindows
{
	public class ProjectViewModel : ViewModelBase
	{
		public Project Project { get; }

		public ProjectViewModel(Project project)
		{
			project.ThrowOnNull(nameof(project));
			Project = project;
		}
	}
}
