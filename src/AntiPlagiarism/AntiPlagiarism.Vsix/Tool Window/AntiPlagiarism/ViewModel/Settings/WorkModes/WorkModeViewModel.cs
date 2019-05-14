using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;


namespace AntiPlagiarism.Vsix.ToolWindows
{
	public abstract class WorkModeViewModel
	{
		public string Name { get; }

		public string Description { get; }

		protected WorkModeViewModel(string name, string description)
		{
			name.ThrowOnNullOrWhiteSpace(nameof(name));
			description.ThrowOnNullOrWhiteSpace(nameof(description));

			Name = name;
			Description = description;
		}

		public static WorkModeViewModel<TMode> New<TMode>(TMode mode, string name, string description)
		where TMode : struct, Enum
		{
			return new WorkModeViewModel<TMode>(mode, name, description);
		}
	}



	public class WorkModeViewModel<TWorkMode> : WorkModeViewModel
	where TWorkMode : struct, Enum
	{
		public TWorkMode WorkMode { get; }

		public WorkModeViewModel(TWorkMode workMode, string name, string description) :
							base(name, description)
		{
			WorkMode = workMode;
		}		
	}
}
