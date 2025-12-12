using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Text;

namespace Albatross.CodeAnalysis {
	/// <summary>
	/// Provides extension methods for code generation scenarios using Roslyn.
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// Reports a diagnostic message during code generation with the specified severity.
		/// </summary>
		/// <param name="context">The generator execution context.</param>
		/// <param name="severity">The severity level of the diagnostic.</param>
		/// <param name="code">The diagnostic code identifier.</param>
		/// <param name="message">The diagnostic message to report.</param>
		public static void CodeGenDiagnostic(this GeneratorExecutionContext context, DiagnosticSeverity severity, string code, string message) {
			var descriptor = new DiagnosticDescriptor(code, string.Empty, message, "CodeGenerator", severity, true);
			context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None));
		}
		
		/// <summary>
		/// Builds a comprehensive error message from an exception chain for code generator diagnostics.
		/// Concatenates the exception message with all inner exception messages.
		/// </summary>
		/// <param name="err">The exception to format.</param>
		/// <param name="generatorName">The name of the code generator that threw the exception.</param>
		/// <returns>A formatted error message containing the generator name and all exception messages in the chain.</returns>
		public static string BuildCodeGeneneratorErrorMessage(this Exception err, string generatorName) {
			var sb = new StringBuilder($"An exception has been thrown while running the {generatorName} code generator:");
			while (err != null) {
				sb.Append(err.Message);
				err = err.InnerException;
				if (err != null) {
					sb.Append(" -> ");
				}
			}
			return sb.ToString();
		}
		
		/// <summary>
		/// Creates a debug file in the project directory with the specified content during code generation.
		/// Useful for debugging generated code. The file is only created if the project directory can be determined.
		/// </summary>
		/// <param name="context">The generator execution context.</param>
		/// <param name="fileName">The name of the file to create (without path).</param>
		/// <param name="content">The content to write to the file.</param>
		public static void CreateGeneratorDebugFile(this GeneratorExecutionContext context, string fileName, string content) {
			if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.ProjectDir", out var projectDir)) {
				var path = Path.Combine(projectDir, fileName);
				using (var streamWriter = new StreamWriter(path)) {
					streamWriter.WriteLine(content);
					streamWriter.Flush();
				}
			}
		}
		
		/// <summary>
		/// Adds nullable enable and disable directives as leading and trailing trivia to a namespace declaration.
		/// This enables nullable reference types within the namespace scope.
		/// </summary>
		/// <param name="syntax">The namespace declaration syntax to modify.</param>
		/// <returns>The namespace declaration with nullable directives added.</returns>
		public static NamespaceDeclarationSyntax CreateNamespaceNullableDirective(this NamespaceDeclarationSyntax syntax) {
			var nullableEnableDirective = SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true));
			var nullableDisableDirective = SyntaxFactory.Trivia(SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.DisableKeyword), true));
			return syntax.WithLeadingTrivia(nullableEnableDirective).WithTrailingTrivia(nullableDisableDirective);
		}
	}
}