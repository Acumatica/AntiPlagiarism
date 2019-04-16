using System;
using System.Collections.Generic;
using AntiPlagiarism.Core.Utilities;

namespace AntiPlagiarism.Core.Method
{
    public class MethodIndex : IEquatable<MethodIndex>
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

		public override bool Equals(object obj) => Equals(obj as MethodIndex);

		public bool Equals(MethodIndex other) =>
			Name == other?.Name && Path == other.Path && SourceCode == other.SourceCode && 
			Line == other.Line && Character == other.Character;

		public override int GetHashCode()
		{
			int hash = 17;

			unchecked
			{
				hash = 23 * hash + Name.GetHashCode();
				hash = 23 * hash + SourceCode.GetHashCode();
				hash = 23 * hash + Path.GetHashCode();
				hash = 23 * hash + Line.GetHashCode();
				hash = 23 * hash + Character.GetHashCode();
			}

			return hash;
		}
	}
}
