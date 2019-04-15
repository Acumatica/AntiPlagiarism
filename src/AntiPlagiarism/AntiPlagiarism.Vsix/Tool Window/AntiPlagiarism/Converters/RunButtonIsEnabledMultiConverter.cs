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
			if (values == null || values.Length != 3 || !(values[0] is WorkMode workMode) ||
				!(values[1] is string referenceSolutionPath) || !(values[2] is bool analysisIsRunning))
			{
				return Binding.DoNothing;
			}

			bool isEnabled = !analysisIsRunning && (workMode == WorkMode.SelfAnalysis || !referenceSolutionPath.IsNullOrWhiteSpace());
			return isEnabled;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
