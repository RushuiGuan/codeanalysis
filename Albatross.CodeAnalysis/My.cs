using System.IO;

namespace Albatross.CodeAnalysis {
	/// <summary>
	/// Provides constant definitions and helper methods for common types, namespaces, and code generation utilities.
	/// </summary>
	public static class My {
		/// <summary>
		/// Contains metadata names for commonly used generic type definitions.
		/// These are typically used with Roslyn's GetTypeByMetadataName method.
		/// </summary>
		public static class GenericDefinition {
			public const string Nullable = "System.Nullable<>";
			public const string IEnumerable = "System.Collections.Generic.IEnumerable<>";
			public const string IAsyncEnumerable = "System.Collections.Generic.IAsyncEnumerable<>";
			public const string List = "System.Collections.Generic.List<>";
			public const string ICollection = "System.Collections.Generic.ICollection<>";
		}
		
		/// <summary>
		/// Contains fully qualified names for commonly used non-generic types.
		/// </summary>
		public static class Class {
			public const string IEnumerable = "System.Collections.IEnumerable";
			public const string CodeGenExtensions = "CodeGenExtensions";
		}
		
		/// <summary>
		/// Contains commonly used namespace names as string constants.
		/// </summary>
		public static class Namespace {
			public const string System = "System";
			public const string System_Text_Json_Serialization = "System.Text.Json.Serialization";
			public const string System_IO = "System.IO";
			public const string System_Threading_Tasks = "System.Threading.Tasks";
			public const string Microsoft_Extensions_DependencyInjection = "Microsoft.Extensions.DependencyInjection";
			public const string System_Collections_Generic = "System.Collections.Generic";
			public const string Microsoft_EntityFrameworkCore = "Microsoft.EntityFrameworkCore";
		}

		/// <summary>
		/// Writes a standard source file header comment to the text writer.
		/// </summary>
		/// <param name="writer">The text writer to write to.</param>
		/// <param name="filename">The name of the file (without extension) to include in the header.</param>
		/// <returns>The text writer for method chaining.</returns>
		public static TextWriter WriteSourceHeader(this TextWriter writer, string filename) {
			writer.WriteLine($"// ********** {filename}.cs ********** //");
			return writer;
		}
	}
}