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

        public MethodIndex(string name, string path, int line, int character)
        {
            name.ThrowOnNullOrEmpty(nameof(name));
            path.ThrowOnNullOrEmpty(nameof(path));

            Name = name;
            Path = path;
            Line = line;
            Character = character;
        }
    }
}
