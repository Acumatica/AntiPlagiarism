using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using System.Threading.Tasks;


namespace AntiPlagiarism.Vsix.ToolWindows
{
    public class ColumnsVisibilityCollectionViewModel : ViewModelBase
	{
		public AntiPlagiarismWindowViewModel ParentViewModel { get; }

		public ColumnsVisibilityCollectionViewModel(AntiPlagiarismWindowViewModel parentViewModel)
		{
			parentViewModel.ThrowOnNull(nameof(parentViewModel));

			ParentViewModel = parentViewModel;
		}
	}
}
