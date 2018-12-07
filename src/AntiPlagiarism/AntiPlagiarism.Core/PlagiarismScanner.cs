using AntiPlagiarism.Core.Method;
using AntiPlagiarism.Core.Solution;
using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;

namespace AntiPlagiarism.Core
{
    public class PlagiarismScanner
    {
        public const double SimilarityThresholdDefault = 0.8;
		public const int DefaultMinMethodSize = 100;


		private readonly List<PlagiarismInfo> _scanResults = new List<PlagiarismInfo>();
        private readonly double _similarityThreshold;
        private readonly int _minMethodSize;
        private readonly string _referenceSolutionPath;
        private readonly string _sourceSolutionPath;
        private readonly Func<MethodIndex, MethodIndex, double> _methodSimilarityStrategy;

        public PlagiarismScanner(string referenceSolutionPath, string sourceSolutionPath,
                                 double? similarityThreshold = null, int? minMethodSize = null,
                                 Func<MethodIndex, MethodIndex, double> methodSimilarityStrategy = null)
        {
            _referenceSolutionPath = referenceSolutionPath;
            _sourceSolutionPath = sourceSolutionPath;
            _similarityThreshold = similarityThreshold ?? SimilarityThresholdDefault;
			_minMethodSize = minMethodSize ?? DefaultMinMethodSize;

            if (methodSimilarityStrategy == null)
            {
                _methodSimilarityStrategy = DefaultSimilarityStrategies.MethodSimilarityStrategy;
            }
        }

        public IEnumerable<PlagiarismInfo> Scan(bool callFromVS)
        {
            if (!callFromVS)
            {
                MSBuildLocator.RegisterDefaults();
            }

            ScanMethods();

            return _scanResults;
        }

        private void ScanMethods()
        {
            SolutionIndex referenceIndex = SolutionIndexBuilder.BuildIndex(_referenceSolutionPath, _minMethodSize);
            SolutionIndex solutionIndex = SolutionIndexBuilder.BuildIndex(_sourceSolutionPath, _minMethodSize);

            foreach (MethodIndex r in referenceIndex.MethodIndices)
            {
                foreach (MethodIndex s in solutionIndex.MethodIndices)
                {
                    double similarity = _methodSimilarityStrategy(r, s);
                    bool isPlagiat = similarity >= _similarityThreshold;

                    if (!isPlagiat)
                        continue;

                    _scanResults.Add(new PlagiarismInfo(PlagiarismType.Method, similarity, r, s));
                }
            }
        }
    }
}
