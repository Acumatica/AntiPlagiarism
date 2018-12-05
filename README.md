# Antiplagiarism

Antiplagiarism is a code analysis tool for Visual Studio that finds in the solution the fragments of code that have been copied from another solution. 
The tool was designed to be used along with the Acumatica ERP application. However, the tool can be used with any other solutions as an instrument to find code duplication.
The functionality of the tool is similar to the Code Clone Detection functionality, which is available in the Visual Studio Enterprise. (For details about this functionality, see https://docs.microsoft.com/en-us/previous-versions/hh205279(v=vs.140) .)

To work with the copied code fragments that the tool has found, you can open the Antiplagiarism window from the **Tools** main menu of Visual Studio. 
You can sort the results and open the source file by double clicking the file name in the **Source Location** column of the table with the results. 

## The Process of Building the Solution
To build the solution, do the following:

1. Add your strong-name key file as _src/Key.snk_. If you don't have one, run Developer Command Prompt and generate the key by using the following command: _sn.exe -k "src\Key.snk"_.
2. Build _Antiplagiarism.sln_.

