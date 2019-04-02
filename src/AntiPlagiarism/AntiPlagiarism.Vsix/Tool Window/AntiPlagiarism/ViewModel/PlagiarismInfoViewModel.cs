using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Plagiarism;
using AntiPlagiarism.Core.Utilities;
using AntiPlagiarism.Vsix.Utilities;
using AntiPlagiarism.Vsix.Utilities.Navigation;
using System.Threading.Tasks;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;


namespace AntiPlagiarism.Vsix.ToolWindows
{
    public class PlagiarismInfoViewModel : ViewModelBase
	{
		private const string LocationPrefix = "SourceFile(";
		private const string LocationSuffix = ")";

		private readonly PlagiarismInfo _plagiarismInfo;

		public AntiPlagiarismWindowViewModel ParentViewModel { get; }

		public string Type => _plagiarismInfo.Type.ToString();

		public double Similarity => _plagiarismInfo.Similarity;

		public bool IsThresholdExceeded
		{
			get
			{
				double threshholdFraction = ParentViewModel.ThreshholdPercent / 100.0;
				return Similarity >= threshholdFraction;
			}
		}

		public string ReferenceName => _plagiarismInfo.Reference.Name;

		public string ReferenceLocation { get; }

		public string SourceName => _plagiarismInfo.Input.Name;

		public string SourceLocation { get; }

		public PlagiarismInfoViewModel(AntiPlagiarismWindowViewModel parentViewModel, PlagiarismInfo plagiarismInfo,
									   string referenceSolutionDir, string sourceSolutionDir)
		{
			parentViewModel.ThrowOnNull(nameof(parentViewModel));
			plagiarismInfo.ThrowOnNull(nameof(plagiarismInfo));
			referenceSolutionDir.ThrowOnNullOrWhiteSpace(nameof(referenceSolutionDir));
			sourceSolutionDir.ThrowOnNullOrWhiteSpace(nameof(sourceSolutionDir));

			_plagiarismInfo = plagiarismInfo;
			ParentViewModel = parentViewModel;
			ReferenceLocation = ExtractShortLocation(_plagiarismInfo.Reference.Path, referenceSolutionDir);
			SourceLocation = ExtractShortLocation(_plagiarismInfo.Input.Path, sourceSolutionDir);
		}

		public async Task OpenLocationAsync(LocationType locationType)
		{
			MethodIndex location;

			switch (locationType)
			{
				case LocationType.Reference:
					location = _plagiarismInfo.Reference;
					break;
				case LocationType.Source:
					location = _plagiarismInfo.Input;
					break;
				default:
					return;
			}

			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(AntiPlagiarismPackage.Instance.DisposalToken);
			var vsWorkspace = await AntiPlagiarismPackage.Instance.GetVSWorkspaceAsync();

			if (vsWorkspace?.CurrentSolution == null)
				return;

			await AntiPlagiarismPackage.Instance.OpenCodeFileAndNavigateByLineAndCharAsync(vsWorkspace.CurrentSolution, location.Path,
																							location.Line, location.Character);
		}

		private string ExtractShortLocation(string location, string solutionDir)
		{
            string preparedLocation = location;

			if (preparedLocation.StartsWith(LocationPrefix))
			{
				preparedLocation = preparedLocation.Substring(LocationPrefix.Length);
			}

			if (preparedLocation.EndsWith(LocationSuffix))
			{
				preparedLocation = preparedLocation.Substring(0, preparedLocation.Length - LocationSuffix.Length);
			}

			if (preparedLocation.StartsWith(solutionDir))
			{
				preparedLocation = preparedLocation.Substring(solutionDir.Length);
			}

			return preparedLocation;
		}
	}
}
