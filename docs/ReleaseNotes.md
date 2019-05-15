# Antiplagiarism Release Notes
This document provides information about fixes, enhancements, and key features that are available in Antiplagiarism.

## Antiplagiarism 1.3
Antiplagiarism 1.3 includes new features, enhancements, and bug fixes described in this section, as well as the features that have been implemented in the previous version.

### Code Comparison Algorithm Enhancement
Antiplagiarism now uses advanced code analysis algorithm from Roslyn which performs mich more complex comparison of syntax trees.
This greatly improved the quality of analysis and allowed to find duplication even for small methods (about 5-10 lines).

### Code Fragments in Data Grid
The code fragments for the selected row can be shown directly in the data grid if you open the context menu with right click and select **Show Code**. 
You can hide the code by right clicking on the selected row again and selecting **Hide Code** or by changing the selected row.

### New Settings
Antiplagiarism now has a new **Settings** panel. It can be expanded/collapsed by clicking on the button with a gear icon.

#### Columns Visibility
The **Column Visibility** button in the settings panel allows you to specify the columns visible in the data grid. 

#### Show only rows exceeding threshold
The **Show only rows exceeding threshold** button allows you to turn on mode in which all rows with **Similarity** value not exceeding **Threshold** are hidden.
This feature can be combined with the filtering calculated results by the **Threshold** parameter.

#### New Source Origin Modes
In previous version of Antiplagiarism, you could only use the whole current solution as a source code for the comparison. Now you can choose one of the three modes:

1. **Current Solution** to use the whole current solution for the comparison.
2. **Selected Project** to use selected project from the current solution. The **Select Project** combobox will appear in which you can choose the project for comparison.
3. **Selected Folder** to use selected folder with source code for comparison. You can specify the folder in the appeared **Select Folder** panel.

#### New Supported Work Modes
In previous version of Antiplagiarism, you could only specify the solution file as a reference solution for comparison. Now you can choose one of the three modes:

1) **Self-Analysis** to compare the source code (selected with a consideration for specified **Source Origin**) with itself. This mode is useful for refactoring.
2) **Reference Solution** to compare the source code with provided reference solution. The reference solution can be selected in the appeared **Reference Solution** control.
control. You can select solution (.sln) or project (.csproj) file.
3) **Acumatica Sources** to compare the source code with a code stored in a specified folder. This mode was designed for third-party developers which do not have
the whole Acumatica solution but have a set of source code files delivered with Acumatica ERP. However, this mode can be used in a more general way not related to Acumatica 
to compare source code with a code stored in the specified folder. The folder can be selected in the appeared **Acumatica Sources** control.

### Other Enhancements
* The logging for errors is greatly improved.
* The integraion with Visual Studio is improved. Added the background loading of the package.
* If you enter invalid values in the **Threshold** or **Min. Method Size** textbox the textbox's background will become red and the useful tooltip
about allowed range of values will be shown.
* Added support for Visual Studio 2019.
* A lot of small UI enhancements - tooltips, messages, enable/disable UI controls depending on the context, etc.

### Bug Fixes
In this version, the following bugs have been fixed.

| Bug | Fix Description |
| --- | --------------- |
| The sort didn't work for the **Similarity** column. | The sort now works for all data grid columns. |
| The data from the **Similarity** column wasn't copied to the clipboard. | The data now is copied to the clipboard for all data grid columns. |


## Antiplagiarism 1.0
Antiplagiarism 1.0 is the first release of the tool. It allows you to compare the current solution with a specified reference solution.
To start working with the Antiplagiarism, you must open in the Visual Studio the solution you want to analyse. Then you need to open the Antiplagiarism window
from the **Tools** main menu of Visual Studio. 

###Functionality
You can only the whole current solution as a source code for comparison. The reference solution must be specified as a path to solution file (.sln).
You can specify the parameters for analysis such as **Threshold** and **Min. Method Size**. The analysis can be started by clicking the **Run"**button. 

In the table with the results, you can sort the results by clicking on the column headers.
You can navigate to the code fragment locations by double clicking on the file names in the **Source Location** column(for the copied code) 
and **Reference Location** column (for the original code). 

The table supports copy/paste functionality. You can select a set of rows (the standard **Shift**/**Ctrl** buttons functionality for selection is supported).
You can also press **Ctrl** + **A** to select all rows in the grid. Press **Ctrl** + **C** to copy the selection to the clipboard. 
The copied data can be pasted to Excel or some other place. 

The **Similarity** column shows how similar the "source" and "reference" methods are. The red information sign to the left of the value indicates that the
similarity value exceeds the threshold. If you change the **Threshold** parameter value for the already completed analysis the red information signs will be updated. 