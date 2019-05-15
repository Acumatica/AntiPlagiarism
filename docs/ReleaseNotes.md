# AntiPlagiarism Release Notes
This document provides information about fixes, enhancements, and key features that are available in AntiPlagiarism.

## AntiPlagiarism 1.3
AntiPlagiarism 1.3 includes new features, enhancements, and bug fixes described in this section, as well as the features that have been implemented in the previous version.

### Enhanced Code Comparison Algorithm
AntiPlagiarism now uses advanced code analysis algorithm from Roslyn, which performs more complex comparison of syntax trees as compared to the previous version of AntiPlagiarism.
With this new algorithm, AntiPlagiarism can find duplication even for methods that have from 5 to 10 lines.

### Code Fragments in the Data Grid
In the data grid, you can now view the code fragments for the selected row if, in the context menu, you click **Show Code**. 
To hide the code, you can click **Hide Code** in the context menu or change the selected row.

### New Settings
AntiPlagiarism now has the **Settings** panel. You can expand or collapse the panel by clicking on the button with a gear icon. The panel includes the settings, which are described below.

#### Visibility of Columns
You can specify the columns visible in the data grid by using the **Column Visibility** button. 

#### Hiding of Particular Rows
By using the **Show only rows exceeding threshold** button, you can turn on the mode in which all rows with the **Similarity** value that does not exceed **Threshold** are hidden.
You can change the **Threshold** parameter to filter already calculated results.

#### Ability to Select Parts of the Solution for Comparison
In previous version of AntiPlagiarism, you could only use the whole current solution as the source code for the comparison. Now you can choose one of the following options in **Source Origin**:

1. **Current Solution** to use the whole current solution for the comparison.
2. **Selected Project** to use the selected project from the current solution. The **Select Project** box will appear in which you can choose the project for the comparison.
3. **Selected Folder** to use the selected folder with the source code for the comparison. You can specify the folder in the **Select Folder** panel.

#### New Work Modes
In previous version of AntiPlagiarism, you could only specify the solution file as the reference solution for the comparison. Now you can choose one of the following modes:

1) **Self-Analysis** to compare the source code with itself. This mode is useful for refactoring.
2) **Reference Solution** to compare the source code with the provided reference solution. You can select the reference solution in the **Reference Solution** control.
control. You can select a solution (.sln) or project (.csproj) file.
3) **Acumatica Sources** to compare the source code with the code stored in the specified folder. This mode is designed for third-party developers, which do not have
the whole Acumatica ERP solution but have a set of source code files delivered with Acumatica ERP. However, this mode can be used in a general way, which is not related to Acumatica, 
to compare the source code with a code stored in the specified folder. The folder can be selected in the **Acumatica Sources** control.

### Other Enhancements
* The logging of errors is improved.
* The integration with Visual Studio is improved: The background loading of the package has been added.
* If you enter invalid values in the **Threshold** or **Min. Method Size** box, the box's background becomes red and a tooltip
about the possible range of values is shown.
* Visual Studio 2019 is now supported.
* A lot of small UI enhancements, such as new tooltips, messages, enabling and disabling of UI controls depending on the context, have been added.

### Bug Fixes
In this version, the following bugs have been fixed.

| Bug | Fix Description |
| --- | --------------- |
| Sorting didn't work for the **Similarity** column. | Sorting now works for all data grid columns. |
| The data from the **Similarity** column wasn't copied to the clipboard. | The data now is copied to the clipboard for all data grid columns. |


## AntiPlagiarism 1.0
AntiPlagiarism 1.0 is the first release of the tool. The tool allows you to compare the current solution with a specified reference solution.
To start working with the AntiPlagiarism, you must open in the Visual Studio the solution you want to analyze. Then you need to open the AntiPlagiarism window
from the **Tools** main menu of Visual Studio. 

###Functionality
You can specify only the whole current solution as a source code for comparison. The reference solution must be specified as a path to the solution file (.sln).
You can specify the parameters for analysis such as **Threshold** and **Min. Method Size**. You can start the analysis by clicking the **Run** button. 

In the table with the results, you can sort the results by clicking on the column headers.
You can navigate to the code fragment locations by double clicking on the file names in the **Source Location** column (for the copied code) 
and **Reference Location** column (for the original code). 

The table supports copying and pasting. You can select a set of rows (the standard **Shift**/**Ctrl** functionality for selection is supported).
You can also press **Ctrl** + **A** to select all rows in the grid. Press **Ctrl** + **C** to copy the selection to the clipboard. 
The copied data can be pasted to Excel or another place. 

The **Similarity** column shows how similar the source and reference methods are. The red information sign to the left of the value indicates that the
similarity value exceeds the threshold. If you change the **Threshold** parameter value for the already completed analysis, the red information signs is updated. 
