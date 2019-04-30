using System;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Utilities;


namespace AntiPlagiarism.Vsix.ToolWindows
{
	public static class ReferenceSourcePathRetriever
	{
		public static string GetReferenceSolutionFilePath()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Solution files (*.sln)|*.sln|Project files (*.csproj)|*.csproj|All files (*.*)|*.*",
				DefaultExt = "csproj",
				AddExtension = true,
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = false,
				Title = "Select reference solution file"
			};

			if (openFileDialog.ShowDialog() != true || openFileDialog.FileName.IsNullOrWhiteSpace() || !File.Exists(openFileDialog.FileName))
				return null;

			string extension = Path.GetExtension(openFileDialog.FileName);

			if (extension != MethodReader.SolutionExtension && extension != MethodReader.ProjectExtension)
				return null;

			return openFileDialog.FileName;
		}

		public static string GetAcumaticaSourcesFolderPath()
		{
			if (CommonFileDialog.IsPlatformSupported)
			{
				var folderSelectDialog = new CommonOpenFileDialog
				{
					IsFolderPicker = true,
					AllowNonFileSystemItems = false,
					Multiselect = false,
					Title = "Select folder with Acumatica source code files",				
					EnsurePathExists = true,
					AddToMostRecentlyUsedList = true,
					DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					ShowPlacesList = true				
				};

				var dialogRes = folderSelectDialog.ShowDialog();

				if (dialogRes != CommonFileDialogResult.Ok)
					return null;

				string selectedDirectory = folderSelectDialog.FileName;
				return !selectedDirectory.IsNullOrWhiteSpace() && Directory.Exists(selectedDirectory)
					? selectedDirectory
					: null;
			}
			else
			{
				return GetAcumaticaSourcesFolderPathWithOldStyleDialog();
			}
		}
		
		public static string GetAcumaticaSourcesFolderPathWithOldStyleDialog()
		{
			using (var openFolderDialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				openFolderDialog.Description = "Select folder with Acumatica source code files";
				openFolderDialog.ShowNewFolderButton = false;
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
