using System;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Utilities;


namespace AntiPlagiarism.Vsix.ToolWindows
{
	public static class SelectFileOrFolder
	{
		public static string SelectSolutionOrProjectFile(string title)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Solution files (*.sln)|*.sln|Project files (*.csproj)|*.csproj|All files (*.*)|*.*",
				DefaultExt = "csproj",
				AddExtension = true,
				CheckFileExists = true,
				CheckPathExists = true,			
				Multiselect = false,
				Title = title
			};

			if (openFileDialog.ShowDialog() != true || openFileDialog.FileName.IsNullOrWhiteSpace() || !File.Exists(openFileDialog.FileName))
				return null;

			string extension = Path.GetExtension(openFileDialog.FileName);

			if (extension != MethodReader.SolutionExtension && extension != MethodReader.ProjectExtension)
				return null;

			return openFileDialog.FileName;
		}

		public static string SelectFolder(string title, string defaultDirectory = null)
		{
			if (CommonFileDialog.IsPlatformSupported)
			{
				if (defaultDirectory.IsNullOrWhiteSpace() || !Directory.Exists(defaultDirectory))
				{
					defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				}

				using (var folderSelectDialog = CreateDialog())
				{
					var dialogRes = folderSelectDialog.ShowDialog();

					if (dialogRes != CommonFileDialogResult.Ok)
						return null;

					string selectedDirectory = folderSelectDialog.FileName;
					return !selectedDirectory.IsNullOrWhiteSpace() && Directory.Exists(selectedDirectory)
						? selectedDirectory
						: null;
				}
			}
			else
			{
				return SelectFolderWithOldStyleDialog(title);
			}

			//-------------------------------------Local Function---------------------------------------------
			CommonOpenFileDialog CreateDialog() =>
				new CommonOpenFileDialog()
				{
					IsFolderPicker = true,
					AllowNonFileSystemItems = false,
					Multiselect = false,
					Title = title,
					EnsurePathExists = true,
					EnsureValidNames = true,
					AddToMostRecentlyUsedList = true,
					DefaultDirectory =  defaultDirectory,
					ShowPlacesList = true
				};
		}
		
		public static string SelectFolderWithOldStyleDialog(string title)
		{
			using (var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				openFolderDialog.Description = title;
				openFolderDialog.ShowNewFolderButton = false;
				openFolderDialog.RootFolder = Environment.SpecialFolder.MyDocuments;

				var dialogRes = openFolderDialog.ShowDialog();

				if (dialogRes != System.Windows.Forms.DialogResult.OK && dialogRes != System.Windows.Forms.DialogResult.Yes)
					return null;

				string selectedDirectory = openFolderDialog.SelectedPath;
				return !selectedDirectory.IsNullOrWhiteSpace() && Directory.Exists(selectedDirectory)
					? selectedDirectory
					: null;
			}
		}
	}
}
