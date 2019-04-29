using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AntiPlagiarism.Core.Method
{
    public static class MethodReader
    {
        public const string SolutionExtension = ".sln";
        public const string ProjectExtension = ".csproj";
		private const string CSharpExtension = ".cs";
		private const string AnyCSharpFilePattern = "*" + CSharpExtension;

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

		private static IEnumerable<Project> GetProjects(string codePath)
		{
			var workspace = MSBuildWorkspace.Create();
			workspace.WorkspaceFailed += Workspace_WorkspaceFailed;

			if (IsSolution(codePath))
			{
				var solution = workspace.OpenSolutionAsync(codePath).Result;

				return solution.Projects;
			}
			else if (IsProject(codePath))
			{
				var project = workspace.OpenProjectAsync(codePath).Result;

				return new[] { project };
			}
			else
			{
				throw new ArgumentException($"The code path {codePath} is not supported", nameof(codePath));
			}
		}

		private static IEnumerable<SyntaxTree> GetSyntaxTreesFromProjects(string codePath)
		{
			var projects = GetProjects(codePath);

			return projects
				.Select(p => p.GetCompilationAsync().Result)
				.SelectMany(c => c.SyntaxTrees);
		}

		private static IEnumerable<SyntaxTree> GetSyntaxTreesFromFolder(string codePath)
		{
			var cSharpFiles = Directory.EnumerateFiles(codePath, AnyCSharpFilePattern, SearchOption.AllDirectories);

			return cSharpFiles
				.Select(file => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(file), path: file));
		}

		private static IEnumerable<SyntaxTree> GetSyntaxTrees(string codePath)
		{
			if (Directory.Exists(codePath))
			{
				return GetSyntaxTreesFromFolder(codePath);
			}

			return GetSyntaxTreesFromProjects(codePath);
		}


		internal static IEnumerable<MethodDeclarationSyntax> EnumerateMethods(string codePath, int minStatementsCount)
        {
			var syntaxTrees = GetSyntaxTrees(codePath);

			foreach (var tree in syntaxTrees)
			{
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
