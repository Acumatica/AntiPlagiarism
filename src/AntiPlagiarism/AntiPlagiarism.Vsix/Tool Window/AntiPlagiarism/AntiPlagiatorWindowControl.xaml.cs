using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;
using System.Windows.Controls.Primitives;



namespace AntiPlagiarism.Vsix.ToolWindows
{
	/// <summary>
	/// Interaction logic for AntiPlagiarismWindowControl.
	/// </summary>
	public partial class AntiPlagiarismWindowControl : UserControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AntiPlagiarismWindowControl"/> class.
		/// </summary>
		public AntiPlagiarismWindowControl()
		{
			this.InitializeComponent();
		}

		private void DataGridCell_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (!(sender is DataGridCell cell) || !(cell.DataContext is PlagiarismInfoViewModel plagiarismInfo) ||
				!(cell.Tag is LocationType locationType) || e.Handled || e.ChangedButton != System.Windows.Input.MouseButton.Left)
			{
				return;
			}

			plagiarismInfo.OpenLocationAsync(locationType)
						  .FileAndForget($"vs/{AntiPlagiarismPackage.PackageName}/{nameof(PlagiarismInfoViewModel.OpenLocationAsync)}");
		}

		private void DataGrid_Initialized(object sender, EventArgs e)
		{
			if (!(sender is DataGrid dataGrid) || !(dataGrid.DataContext is AntiPlagiarismWindowViewModel windowViewModel))
				return;

			var columnNames = dataGrid.Columns.Select(col => col.Header is DataGridColumnHeader header
																? header.Tag.ToString()
																: col.Header.ToString())
											  .OfType<string>();
			windowViewModel.FillColumnsVisibility(columnNames);
		}

		private void ColumnsVisibilityButton_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button columnsVisibilityButton) || columnsVisibilityButton.ContextMenu == null)
				return;

			columnsVisibilityButton.ContextMenu.PlacementTarget = columnsVisibilityButton;
			columnsVisibilityButton.ContextMenu.DataContext = columnsVisibilityButton.DataContext;
			columnsVisibilityButton.ContextMenu.IsOpen = true;
		}
	}
}