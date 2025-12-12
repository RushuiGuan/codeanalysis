using Microsoft.CodeAnalysis;

namespace Albatross.CodeAnalysis.Symbols {
	/// <summary>
	/// Provides extension methods to retrieve commonly used type symbols from a Roslyn compilation.
	/// These helpers simplify accessing framework types during code analysis and generation.
	/// </summary>
	public static class SymbolProvider {
		/// <summary>
		/// Gets the <see cref="System.String"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.String.</returns>
		public static INamedTypeSymbol String (this Compilation compilation) => compilation.GetSpecialType(SpecialType.System_String);
		
		/// <summary>
		/// Gets the <see cref="System.DateTime"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.DateTime.</returns>
		public static INamedTypeSymbol DateTime (this Compilation compilation) => compilation.GetSpecialType(SpecialType.System_DateTime);
		
		/// <summary>
		/// Gets the <see cref="System.DateOnly"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.DateOnly.</returns>
		public static INamedTypeSymbol DateOnly (this Compilation compilation) => compilation.GetRequiredSymbol("System.DateOnly");
		
		/// <summary>
		/// Gets the <see cref="System.TimeOnly"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.TimeOnly.</returns>
		public static INamedTypeSymbol TimeOnly (this Compilation compilation) => compilation.GetRequiredSymbol("System.TimeOnly");
		
		/// <summary>
		/// Gets the <see cref="System.DateTimeOffset"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.DateTimeOffset.</returns>
		public static INamedTypeSymbol DateTimeOffset (this Compilation compilation) => compilation.GetRequiredSymbol("System.DateTimeOffset");
		
		/// <summary>
		/// Gets the <see cref="System.Object"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Object.</returns>
		public static INamedTypeSymbol Object (this Compilation compilation) => compilation.GetSpecialType(SpecialType.System_Object);
		
		/// <summary>
		/// Gets the <see cref="System.Byte"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Byte.</returns>
		public static INamedTypeSymbol Byte(this Compilation compilation) => compilation.GetSpecialType(SpecialType.System_Byte);
		
		/// <summary>
		/// Gets the non-generic <see cref="System.Collections.IEnumerable"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Collections.IEnumerable.</returns>
		public static INamedTypeSymbol IEnumerable (this Compilation compilation) => compilation.GetRequiredSymbol("System.Collections.IEnumerable");
		
		/// <summary>
		/// Gets the generic definition for <see cref="System.Nullable{T}"/>.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Nullable&lt;T&gt;.</returns>
		public static INamedTypeSymbol Nullable (this Compilation compilation) => compilation.GetRequiredSymbol("System.Nullable`1");
		
		/// <summary>
		/// Gets the generic definition for <see cref="System.Collections.Generic.IAsyncEnumerable{T}"/>.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Collections.Generic.IAsyncEnumerable&lt;T&gt;.</returns>
		public static INamedTypeSymbol IAsyncEnumerable (this Compilation compilation) => compilation.GetRequiredSymbol("System.Collections.Generic.IAsyncEnumerable`1");
		
		/// <summary>
		/// Gets the generic definition for <see cref="System.Collections.Generic.IEnumerable{T}"/>.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Collections.Generic.IEnumerable&lt;T&gt;.</returns>
		public static INamedTypeSymbol IEnumerableGenericDefinition (this Compilation compilation) => compilation.GetRequiredSymbol("System.Collections.Generic.IEnumerable`1");

		/// <summary>
		/// Gets the <see cref="System.Text.Json.Serialization.JsonConverter"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Text.Json.Serialization.JsonConverter.</returns>
		public static INamedTypeSymbol JsonConverterClass(this Compilation compilation) => compilation.GetRequiredSymbol("System.Text.Json.Serialization.JsonConverter");
		
		/// <summary>
		/// Gets the <see cref="System.Text.Json.Serialization.JsonConverterAttribute"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Text.Json.Serialization.JsonConverterAttribute.</returns>
		public static INamedTypeSymbol JsonConverterAttribute(this Compilation compilation) => compilation.GetRequiredSymbol("System.Text.Json.Serialization.JsonConverterAttribute");
		
		/// <summary>
		/// Gets the <see cref="System.Text.Json.Serialization.JsonStringEnumConverter"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Text.Json.Serialization.JsonStringEnumConverter.</returns>
		public static INamedTypeSymbol JsonStringEnumConverter(this Compilation compilation) => compilation.GetRequiredSymbol("System.Text.Json.Serialization.JsonStringEnumConverter");
		
		/// <summary>
		/// Gets the <see cref="System.Text.Json.Serialization.JsonIgnoreAttribute"/> type symbol.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Text.Json.Serialization.JsonIgnoreAttribute.</returns>
		public static INamedTypeSymbol JsonIgnoreAttribute(this Compilation compilation) => compilation.GetRequiredSymbol("System.Text.Json.Serialization.JsonIgnoreAttribute");
		
		/// <summary>
		/// Gets the generic definition for <see cref="System.Threading.Tasks.Task{TResult}"/>.
		/// </summary>
		/// <param name="compilation">The compilation instance.</param>
		/// <returns>The type symbol for System.Threading.Tasks.Task&lt;T&gt;.</returns>
		public static INamedTypeSymbol TaskGenericDefinition(this Compilation compilation) => compilation.GetRequiredSymbol("System.Threading.Tasks.Task`1");
	}
}
