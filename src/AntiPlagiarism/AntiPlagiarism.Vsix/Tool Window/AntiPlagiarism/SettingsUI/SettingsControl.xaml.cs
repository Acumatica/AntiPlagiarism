using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AntiPlagiarism.Vsix.ToolWindows
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl
	{
		public SettingsControl()
		{
			InitializeComponent();
		}

		private void ColumnsVisibilityButton_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button columnsVisibilityButton) || columnsVisibilityButton.ContextMenu == null)
				return;

			columnsVisibilityButton.ContextMenu.PlacementTarget = columnsVisibilityButton;
			columnsVisibilityButton.ContextMenu.DataContext = columnsVisibilityButton.DataContext;
			columnsVisibilityButton.ContextMenu.IsOpen = true;
		}

		private async void SettingsControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (!(DataContext is SettingsViewModel settingsViewModel))
				return;

			await settingsViewModel.RefillProjectsAsync()
								   .ConfigureAwait(continueOnCapturedContext: true); ;
		}
	}
}
