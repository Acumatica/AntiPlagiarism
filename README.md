# AntiPlagiarism
AntiPlagiarism is a Visual Studio extension that can compare C# code from different sources and find similar methods in the compared code. You can use the tool to collect the data for refactoring by comparing the source code with itself or with the provided custom code. 

AntiPlagiarism is intended to be used for the development and certification of third-party solutions for the Acumatica ERP application. 
However, it can be used with any C# code as an instrument to find code duplication in methods. The functionality is similar to the Code Clone Detection functionality,
which is available in Visual Studio Enterprise. (For details about this functionality, see https://docs.microsoft.com/en-us/previous-versions/hh205279(v=vs.140)
.)

## Functionality
To start working with AntiPlagiarism, you open in Visual Studio the solution you want to analyse and then open the AntiPlagiarism window
from the **Tools** main menu of Visual Studio. 

You can specify the parameters for analysis such as **Threshold** and **Min. Method Size**. You start the analysis by clicking the **Run** button. 

### Data Grid with Analysis Results
You can work with the data grid as follows:

 - In the table with the results, you can sort the results by clicking on the column headers.
 - You can navigate to the code fragment locations by double clicking on the file names in the **Source Location** column (for the copied code) 
and **Reference Location** column (for the original code). 
 - You can view the code fragments for the selected row if, in the context menu, you click **Show Code**. To hide the code, you can click **Hide Code** in the context menu or change the selected row.
 - You can copy the table rows. You can select a set of rows (the standard **Shift**/**Ctrl** functionality for selection is supported).
You can also press **Ctrl** + **A** to select all rows in the grid. Press **Ctrl** + **C** to copy the selection to the clipboard. 
The copied data can be pasted to Excel or another place.
 - You can filter the data in the table by using the **Threshold** parameter. The **Similarity** column shows how similar the source and reference methods are. The red information sign to the left of the value indicates that the similarity value exceeds the threshold. If you change the **Threshold** parameter value for the already completed analysis, the red information signs is updated. You can also use the **Show only rows exceeding threshold** settign for filtering, which is described in the **Settings** section.

### Settings
AntiPlagiarism has the **Settings** panel. You can expand or collapse the panel by clicking on the button with a gear icon. 

The panel contains the following elements.

| Element | Description |
|---------|-------------|
| **Column Visibility** (button) | You use this button to specify the columns visible in the data grid. |
| **Show only rows exceeding threshold** (button) | You use this button to turn on the mode in which all rows with the value of **Similarity** that does not exceed **Threshold** are hidden. |
|  **Source Origin** | In this box, you can choose what will be used as the source code for the comparison. You can use one of the following values:<ul><li>**Current Solution** to use the whole current solution for the comparison.</li><li>**Selected Project** to use the selected project from the current solution. The **Select Project** box will appear in which you can choose the project for the comparison.</li><li>**Selected Folder** to use the selected folder with the source code for the comparison. You can specify the folder in the **Select Folder** panel.</li></ul> |
| **Work Mode** | In this box, you can select the analysis work mode. You can use one of the following values: <ul><li>**Self-Analysis** to compare the source code (selected with a consideration for specified **Source Origin**) with itself. This mode is useful for refactoring.</li><li>**Reference Solution** to compare the source code with provided reference solution. The reference solution can be selected in the appeared **Reference Solution** control. You can select a solution (.sln) or project (.csproj) file.</li><li>**Acumatica Sources** to compare the source code with a code stored in a specified folder. This mode was designed for third-party developers which do not have the whole Acumatica solution but have a set of source code files delivered with Acumatica ERP. However, this mode can be used in a more general way not related to Acumatica to compare source code with a code stored in the specified folder. The folder can be selected in the appeared **Acumatica Sources** control.</li></ul> |

## The Building of the Solution
To build the solution, do the following:

1. Add your strong-name key file as _src/Key.snk_. If you don't have one, run Developer Command Prompt and generate the key by using the following command: _sn.exe -k "src\Key.snk"_.
2. Build _AntiPlagiarism.sln_.
