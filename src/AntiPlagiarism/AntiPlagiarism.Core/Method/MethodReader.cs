using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntiPlagiarism.Core.Method
{
    public static class MethodReader
    {
        public const string SolutionExtension = ".sln";
        public const string ProjectExtension = ".csproj";

        private static bool IsSolution(string path)
        {
            return HasExtension(path, SolutionExtension);
        }

        private static bool IsProject(string path)
        {
            return HasExtension(path, ProjectExtension);
        }

        private static bool HasExtension(string path, string extension)
        {
            var pathExtension = Path.GetExtension(path);

            return extension.Equals(pathExtension, StringComparison.Ordinal);
        }

        internal static IEnumerable<MethodDeclarationSyntax> EnumerateMethods(string codePath, int minStatementsCount)
        {
            var workspace = MSBuildWorkspace.Create();
            workspace.WorkspaceFailed += Workspace_WorkspaceFailed;

            var projects = Enumerable.Empty<Project>();

            if (IsSolution(codePath))
            {
                var solution = workspace.OpenSolutionAsync(codePath).Result;
                projects = solution.Projects;
            }
            else if (IsProject(codePath))
            {
                var project = workspace.OpenProjectAsync(codePath).Result;
                projects = new[] { project };
            }
            else
            {
                throw new ArgumentException($"The code path {codePath} is not supported", nameof(codePath));
            }

            foreach (var project in projects)
            {
                var compilation = project.GetCompilationAsync().Result;

                foreach (var tree in compilation.SyntaxTrees)
                {
                    var semanticModel = compilation.GetSemanticModel(tree);
                    var methodDeclarations = tree.GetRoot()
                        .DescendantNodesAndSelf()
                        .OfType<MethodDeclarationSyntax>()
                        .Where(m => m.Body?.Statements.Count >= minStatementsCount);

                    foreach (var method in methodDeclarations)
                    {
                        yield return method;
                    }
                }
            }
        }

        internal static MethodDeclarationSyntax[] GetMethods(string codePath, int minStatementsCount)
        {
            var methodsArray = EnumerateMethods(codePath, minStatementsCount).ToArray();

            return methodsArray;
        }

        //This is needed for debug
        private static void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
        }
    }
}
