using AntiPlagiarism.Core.Method;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace AntiPlagiarism.Core.Plagiarism
{
    public class PlagiarismScanner
    {
        public const double SimilarityThresholdDefault = 0.67;
		public const int DefaultMinMethodSize = 10;

		private readonly List<PlagiarismInfo> _scanResults = new List<PlagiarismInfo>();
        private readonly double _similarityThreshold;
        private readonly int _minStatementsCount;
        private readonly string _referenceSolutionPath;
        private readonly string _sourceSolutionPath;
        private readonly Func<SyntaxNode, SyntaxNode, double> _getDistance;

        public PlagiarismScanner(string referenceSolutionPath, string sourceSolutionPath,
            double? similarityThreshold = null, int? minMethodSize = null)
        {
            _referenceSolutionPath = referenceSolutionPath;
            _sourceSolutionPath = sourceSolutionPath;
            _similarityThreshold = similarityThreshold ?? SimilarityThresholdDefault;
            _minStatementsCount = minMethodSize ?? DefaultMinMethodSize;
            _getDistance = RoslynSimilarityStrategies.GetDistance;
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
            var referenceMethods = MethodReader.GetMethods(_referenceSolutionPath, _minStatementsCount);
            var inputMethods = MethodReader.EnumerateMethods(_sourceSolutionPath, _minStatementsCount);

            foreach (var i in inputMethods)
            {
                foreach (var r in referenceMethods)
                {
                    var distance = _getDistance(i, r);
                    var similarity = 1 - distance;
                    var isPlagiat = similarity >= _similarityThreshold;

                    if (!isPlagiat)
                    {
                        continue;
                    }

                    var rIndex = MethodIndexBuilder.BuildIndex(r);
                    var iIndex = MethodIndexBuilder.BuildIndex(i);
                    _scanResults.Add(new PlagiarismInfo(PlagiarismType.Method, similarity, rIndex, iIndex));
                }
            }
        }
    }
}
