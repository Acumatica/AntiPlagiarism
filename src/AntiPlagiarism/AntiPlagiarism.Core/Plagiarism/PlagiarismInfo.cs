using AntiPlagiarism.Core.Method;

namespace AntiPlagiarism.Core.Plagiarism
{
    public class PlagiarismInfo
    {
        public PlagiarismType Type { get; private set; }
        public double Similarity { get; private set; }
        public MethodIndex Reference { get; private set; }
        public MethodIndex Input { get; private set; }

        internal PlagiarismInfo(PlagiarismType type, double similarity, MethodIndex reference, MethodIndex input)
        {
            Type = type;
            Similarity = similarity;
            Reference = reference;
            Input = input;
        }
    }
}
