using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AntiPlagiarism.Core.Method
{
    internal static class MethodIndexBuilder
    {
        public static MethodIndex BuildIndex(MethodDeclarationSyntax method/*, SemanticModel semanticModel*/)
        {
            //var symbol = semanticModel.GetDeclaredSymbol(method);
            var methodName = method.Identifier.ToString();//symbol.Name;
            var path = method.GetLocation().GetMappedLineSpan().Path;
            var line = method.GetLocation().GetMappedLineSpan().StartLinePosition.Line;
            var character = method.GetLocation().GetMappedLineSpan().StartLinePosition.Character;
			string sourceCode = method.ToFullString();

            return new MethodIndex(methodName, path, sourceCode, line, character);
        }
    }
}
