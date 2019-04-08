using AntiPlagiarism.Core.Utilities;

namespace AntiPlagiarism.Core.Method
{
    public class MethodIndex
    {
        public string Name { get; }

        public string SourceCode { get; }

        public string Path { get; }

        public int Line { get; }

        public int Character { get; }

        public MethodIndex(string name, string path, string sourceCode, int line, int character)
        {
            name.ThrowOnNullOrEmpty(nameof(name));
            path.ThrowOnNullOrEmpty(nameof(path));
			sourceCode.ThrowOnNullOrEmpty(nameof(sourceCode));

            Name = name;
            Path = path;
			SourceCode = sourceCode;
			Line = line;
            Character = character;
        }
    }
}
