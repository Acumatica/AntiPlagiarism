using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using AntiPlagiarism.Core.Utilities;


namespace AntiPlagiarism.Vsix.ToolWindows.Converters
{
	/// <summary>
	/// Converter which converts <see cref="AntiPlagiarismWindowViewModel.SelectedWorkMode.WorkMode"/>, <see cref="AntiPlagiarismWindowViewModel.ReferenceSolutionPath"/>
	/// and <see cref="AntiPlagiarismWindowViewModel.IsAnalysisRunning"/> into <see cref="bool"/>  value for the Run button IsEnabled property.
	/// </summary>
	public class RunButtonIsEnabledMultiConverter : IMultiValueConverter
	{
	
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length != 6 || 
				!(values[0] is ReferenceWorkMode referenceWorkMode) || 
				!(values[2] is SourceOriginMode sourceOriginMode) ||
				!(values[5] is bool analysisIsRunning))
			{
				return Binding.DoNothing;
			}

			if (analysisIsRunning)
				return false;

			string referenceSolutionPath = values[1] as string;
			string sourceCodeFolderPath = values[3] as string;
			var selectedProject = values[4] as ProjectViewModel;

			if (referenceWorkMode != ReferenceWorkMode.SelfAnalysis && referenceSolutionPath.IsNullOrWhiteSpace())
				return false;
			
			switch (sourceOriginMode)
			{
				case SourceOriginMode.SelectedProject when selectedProject == null:
				case SourceOriginMode.SelectedFolder when sourceCodeFolderPath.IsNullOrWhiteSpace():
					return false;
				default:
					return true;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
