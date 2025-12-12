using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;

namespace Albatross.CodeAnalysis.Testing {
	/// <summary>
	/// Provides extension methods for creating Roslyn compilations in test scenarios.
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// Creates a C# compilation targeting .NET 8 with C# 12 language version from the provided source code.
		/// </summary>
		/// <param name="source">The C# source code to compile.</param>
		/// <param name="assemblyName">The name of the assembly. Defaults to "Net8TestAssembly".</param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A <see cref="CSharpCompilation"/> configured for .NET 8 with the parsed source code.</returns>
		public static async Task<CSharpCompilation> CreateNet8CompilationAsync(this string source, string assemblyName = "Net8TestAssembly", CancellationToken cancellationToken = default) {
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
