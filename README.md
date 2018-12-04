#Antiplagiarism

Antiplagiarism is a code analysis extension tool for Visual Studio that allows to analyze solutions for copy-pasted code from a provided reference solution. 
The tool was designed to be used together with the Acumatica application as a reference solution but can be used with other solutions unrelated to Acumatica as an instrument to find code duplication.
This is similar to the Visual Studio Enterprise functionality "Code Clone Detection":
https://docs.microsoft.com/en-us/previous-versions/hh205279(v=vs.140) 

For the convenient work with the found duplicated code fragments the Antiplagiarism provides a tool window located in the VS "Tools" top level menu. 
The results are displayed in a grid which supports sorting and navigation to source file by double clicking in the location column. 

## The Process of Building the Solution
To build the solution, do the following:
1. Add your strong-name key file as _src/Key.snk_. If you don't have one, run Developer Command Prompt and generate the key by using the following command: _sn.exe -k "src\Key.snk"_.
2. Build _Antiplagiarism.sln_.

