using AntiPlagiarism.Core.Utilities;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Reflection;

namespace AntiPlagiarism.Core
{
    public static class RoslynSimilarityStrategies
    {
        private const string StatementSyntaxComparerAssemblyName = "Microsoft.CodeAnalysis.CSharp.Features";
        private const string StatementSyntaxComparerTypeName = "Microsoft.CodeAnalysis.CSharp.EditAndContinue.StatementSyntaxComparer";
        private const string StatementSyntaxComparerField = "Default";
        private const string GetDistanceMethodName = "GetDistance";

        private static Func<SyntaxNode, SyntaxNode, double> _getDistance;

        static RoslynSimilarityStrategies()
        {
            InitGetDistanceMethod();
        }

        public static double GetDistance(SyntaxNode referenceNode, SyntaxNode inputNode)
        {
            referenceNode.ThrowOnNull(nameof(referenceNode));
            inputNode.ThrowOnNull(nameof(inputNode));

            if (_getDistance == null)
            {
                throw new InvalidOperationException($"{nameof(_getDistance)} cannot be null");
            }

            return _getDistance(referenceNode, inputNode);
        }

        private static void InitGetDistanceMethod()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith(StatementSyntaxComparerAssemblyName, StringComparison.Ordinal))
                .FirstOrDefault();
            if (assembly == null)
            {
                throw new InvalidOperationException($"{StatementSyntaxComparerAssemblyName} assembly cannot be found in a current domain");
            }

            var type = assembly.GetType(StatementSyntaxComparerTypeName);
            if (type == null)
            {
                throw new InvalidOperationException($"{StatementSyntaxComparerTypeName} type cannot be found in the {assembly} assembly");
            }

            var method = type.GetMethod(GetDistanceMethodName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                throw new InvalidOperationException($"{GetDistanceMethodName} cannot be fount in the {type} type");
            }

            var field = type.GetField(StatementSyntaxComparerField, BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
            {
                throw new InvalidOperationException($"{StatementSyntaxComparerField} field cannot be found in the {type} type");
            }

            var instance = field.GetValue(null);
            if (instance == null)
            {
                throw new InvalidOperationException($"The {field} field is null");
            }

            var typeOfGetDistanceMethod = typeof(Func<SyntaxNode, SyntaxNode, double>);
            _getDistance = (Func<SyntaxNode, SyntaxNode, double>)Delegate.CreateDelegate(typeOfGetDistanceMethod, instance, GetDistanceMethodName);
        }
    }
}
