using AntiPlagiarism.Core.Method;
using System.Collections.Generic;

namespace AntiPlagiarism.Core.Solution
{
    internal class SolutionIndex
    {
        public string Path { get; private set; }
        public IEnumerable<MethodIndex> MethodIndices { get; private set; }

        public SolutionIndex(string path, IEnumerable<MethodIndex> methodIndices)
        {
            Path = path;
            MethodIndices = methodIndices;
        }
    }
}
