using Microsoft.CodeAnalysis;

namespace Albatross.CodeAnalysis.Symbols {
	public static class SymbolProvider {
		public static INamedTypeSymbol String (this Compilation compilation) => compilation.GetSpecialType(SpecialType.System_String);
		public static INamedTypeSymbol IEnumerable (this Compilation compilation) => compilation.GetRequiredSymbol("System.Collections.IEnumerable");
		public static INamedTypeSymbol Nullable (this Compilation compilation) => compilation.GetRequiredSymbol("System.Nullable`1");
		public static INamedTypeSymbol IAsyncEnumerable (this Compilation compilation) => compilation.GetRequiredSymbol("System.Collections.Generic.IAsyncEnumerable`1");
		public static INamedTypeSymbol IEnumerableGenericDefinition (this Compilation compilation) => compilation.GetRequiredSymbol("System.Collections.Generic.IEnumerable`1");

		public static INamedTypeSymbol JsonConverterAttribute(this Compilation compilation) => compilation.GetRequiredSymbol("System.Text.Json.Serialization.JsonConverterAttribute");
		public static INamedTypeSymbol JsonStringEnumConverter(this Compilation compilation) => compilation.GetRequiredSymbol("System.Text.Json.Serialization.JsonStringEnumConverter");
	}
}
