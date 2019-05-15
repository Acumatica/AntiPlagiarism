# Antiplagiarism
Antiplagiarism is a Visual Studio extension that can compare the C# code in order to find similar methods. The tool can be used to compare the source code with itself or with a provided custom code in order to collect data for refactoring. 
Antiplagiarism was designed to be used for the development/certification of third party solutions for Acumatica ERP application. 
However, it can be used with any C# code as an instrument to find code duplication in methods. The functionality is similar to the Code Clone Detection functionality,
which is available in the Visual Studio Enterprise. (For details about this functionality, see https://docs.microsoft.com/en-us/previous-versions/hh205279(v=vs.140)

## Functionality
To start working with the Antiplagiarism, you must open in the Visual Studio the solution you want to analyse. Then you need to open the Antiplagiarism window
from the **Tools** main menu of Visual Studio. 
You can specify the parameters for analysis such as **Threshold** and **Min. Method Size**. The analysis can be started by clicking the **Run"**button. 

### Results Data Grid
In the table with the results, you can sort the results by clicking on the column headers.
You can navigate to the code fragment locations by double clicking on the file names in the **Source Location** column(for the copied code) 
and **Reference Location** column (for the original code). 
The code fragments for the selected row can be shown directly in the data grid if you open the context menu with right click and select **Show Code**. 
You can hide the code by right clicking on the selected row again and selecting **Hide Code** or by changing the selected row.

The table supports copy/paste functionality. You can select a set of rows (the standard **Shift**/**Ctrl** buttons functionality for selection is supported).
You can also press **Ctrl** + **A** to select all rows in the grid. Press **Ctrl** + **C** to copy the selection to the clipboard. 
The copied data can be pasted to Excel or some other place. 

The **Similarity** column shows how similar the "source" and "reference" methods are. The red information sign to the left of the value indicates that the
similarity value exceeds the threshold. If you change the **Threshold** parameter value for the already completed analysis the red information signs will be updated. 
This feature can be used together with the **Show only rows exceeding threshold** mode which is described in the **Settings** section.

### Settings
There is a setting panel which can be expanded/collapsed by clicking on the button with a gear icon. The panel contains two buttons and two comboboxes.
The **Column Visibility** button allows you to specify the columns visible in the data grid. 
The **Show only rows exceeding threshold** button allows you to turn on mode in which all rows with **Similarity** value not exceeding **Threshold** are hidden.

In the **Source Origin** combobox you can choose what will be used as a source code for the comparison. There are 3 possible values:
1. **Current Solution** to use the whole current solution for the comparison
2. **Selected Project** to use selected project from the current solution. The **Select Project** combobox will appear in which you can choose the project for comparison.
3. **Selected Folder** to use selected folder with source code for comparison. You can specify the folder in the appeared **Select Folder** panel.

You can also select the analysis **Work Mode**. There are 3 work modes:
1) **Self-Analysis** to compare the source code (selected with a consideration for specified **Source Origin**) with itself. This mode is useful for refactoring.
2) **Reference Solution** to compare the source code with provided reference solution. The reference solution can be selected in the appeared **Reference Solution** control.
control. You can select solution (.sln) or project (.csproj) file.
3) **Acumatica Sources** to compare the source code with a code stored in a specified folder. This mode was designed for third-party developers which do not have
the whole Acumatica solution but have a set of source code files delivered with Acumatica ERP. However, this mode can be used in a more general way not related to Acumatica 
to compare source code with a code stored in the specified folder. The folder can be selected in the appeared **Acumatica Sources** control.

## The Process of Building the Solution
To build the solution, do the following:

1. Add your strong-name key file as _src/Key.snk_. If you don't have one, run Developer Command Prompt and generate the key by using the following command: _sn.exe -k "src\Key.snk"_.
2. Build _Antiplagiarism.sln_.
