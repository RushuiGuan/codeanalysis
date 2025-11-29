using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;

namespace Albatross.CodeAnalysis.Test.Symbols {
	public static class Extensions {
		public static async Task<CSharpCompilation> CreateNet8CompilationAsync(this string source, string assemblyName = "TestAssembly", CancellationToken cancellationToken = default) {
			// 1. Get the proper .NET 8 reference assemblies
			ImmutableArray<MetadataReference> frameworkRefs =
				await ReferenceAssemblies.Net.Net80.ResolveAsync(
					LanguageNames.CSharp, cancellationToken);

			// 2. Parse your source as C# 12 (default for .NET 8)
			var parseOptions = new CSharpParseOptions(languageVersion: LanguageVersion.CSharp12);
			var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);

			// 3. Create the compilation targeting net8.0
			var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			var compilation = CSharpCompilation.Create(
				assemblyName,
				syntaxTrees: new[] { syntaxTree },
				references: frameworkRefs,
				options: compilationOptions);

			return compilation;
		}
	}
}
