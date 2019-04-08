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

		public string ReferenceCodeSnippet { get; }

		public string SourceName => _plagiarismInfo.Input.Name;

		public string SourceLocation { get; }

		public string SourceCodeSnippet { get; }

		private bool _areCodeFragmentsVisible;

		public bool AreCodeFragmentsVisible
		{
			get => _areCodeFragmentsVisible;
			set
			{
				if (_areCodeFragmentsVisible != value)
				{
					_areCodeFragmentsVisible = value;
					NotifyPropertyChanged();
				}
			}
		}

		public Command ShowOrHideCodeCommand { get; }

		public PlagiarismInfoViewModel(AntiPlagiarismWindowViewModel parentViewModel, PlagiarismInfo plagiarismInfo,
									   string referenceSolutionDir, string sourceSolutionDir, int tabSize)
		{
			parentViewModel.ThrowOnNull(nameof(parentViewModel));
			plagiarismInfo.ThrowOnNull(nameof(plagiarismInfo));
			referenceSolutionDir.ThrowOnNullOrWhiteSpace(nameof(referenceSolutionDir));
			sourceSolutionDir.ThrowOnNullOrWhiteSpace(nameof(sourceSolutionDir));

			_plagiarismInfo = plagiarismInfo;
			ParentViewModel = parentViewModel;
			ShowOrHideCodeCommand = new Command(p => ShowOrHideCodeSnippets());

			ReferenceLocation = ExtractShortLocation(_plagiarismInfo.Reference.Path, referenceSolutionDir);
			ReferenceCodeSnippet = IndentCodeSnippet(_plagiarismInfo.Reference.SourceCode, tabSize);

			SourceLocation = ExtractShortLocation(_plagiarismInfo.Input.Path, sourceSolutionDir);
			SourceCodeSnippet = IndentCodeSnippet(_plagiarismInfo.Input.SourceCode, tabSize);		
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

		private void ShowOrHideCodeSnippets() => AreCodeFragmentsVisible = !AreCodeFragmentsVisible;

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

		private string IndentCodeSnippet(string codeSnippet, int tabSize)
		{
			codeSnippet = codeSnippet.Trim('\r', '\n');
			int indentLength = codeSnippet.TakeWhile(c => c == ' ' || c == '\t')
										  .Sum(c => c == '\t' ? tabSize : 1);

			var sb = new StringBuilder(string.Empty, capacity: codeSnippet.Length);
			int counter = 0;

			foreach (char c in codeSnippet)
			{
				switch (c)
				{
					case '\n':
						counter = 0;
						sb.Append(c);
						continue;

					case ' ' when counter < indentLength:
						counter++;
						continue;

					case '\t' when counter < indentLength:
						counter += tabSize;
						continue;

					case ' ' when counter >= indentLength:
					case '\t' when counter >= indentLength:
					default:
						sb.Append(c);
						continue;
				}
			}

			return sb.ToString();
		}	
	}
}
