using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Albatross.CodeAnalysis {
	/// <summary>
	/// Provides extension methods for working with attributes on Roslyn symbols.
	/// </summary>
	public static class AttributeDataExtensions {
		/// <summary>
		/// Determines whether a symbol has the specified attribute applied to it.
		/// </summary>
		/// <param name="symbol">The symbol to check.</param>
		/// <param name="targetAttribute">The attribute type to look for.</param>
		/// <returns>True if the symbol has the attribute; otherwise, false.</returns>
		public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol? targetAttribute)
			=> targetAttribute != null && symbol.GetAttributes().Any(x => x.AttributeClass?.Is(targetAttribute) == true);

		/// <summary>
		/// Attempts to retrieve the specified attribute from a symbol.
		/// </summary>
		/// <param name="symbol">The symbol to check.</param>
		/// <param name="targetAttribute">The attribute type to look for.</param>
		/// <param name="attributeData">When this method returns true, contains the attribute data; otherwise, null.</param>
		/// <returns>True if the attribute was found; otherwise, false.</returns>
		public static bool TryGetAttribute(this ISymbol symbol, INamedTypeSymbol? targetAttribute, [NotNullWhen(true)]out AttributeData? attributeData) {
			if(targetAttribute == null){
				attributeData = null;
				return false;
			}
			foreach (var attribute in symbol.GetAttributes()) {
				if (attribute.AttributeClass?.Is(targetAttribute) == true) {
					attributeData = attribute;
					return true;
				}
			}
			attributeData = null;
			return false;
		}

		/// <summary>
		/// Determines whether a symbol has any attribute whose class derives from the specified base type.
		/// </summary>
		/// <param name="symbol">The symbol to check.</param>
		/// <param name="baseType">The base type to check for in attribute class hierarchies.</param>
		/// <returns>True if the symbol has an attribute with the specified base type; otherwise, false.</returns>
		public static bool HasAttributeWithBaseType(this ISymbol symbol, INamedTypeSymbol? baseType) 
			=> baseType != null && symbol.GetAttributes().Any(x => x.AttributeClass?.BaseType?.Is(baseType) == true);

		/// <summary>
		/// Determines whether a symbol has the specified attribute with constructor arguments matching the given parameter types.
		/// </summary>
		/// <param name="symbol">The symbol to check.</param>
		/// <param name="targetAtribute">The attribute type to look for.</param>
		/// <param name="parameters">The expected constructor parameter types.</param>
		/// <returns>True if the symbol has the attribute with matching constructor arguments; otherwise, false.</returns>
		public static bool HasAttributeWithConstructorArguments(this ISymbol symbol, INamedTypeSymbol? targetAtribute, params INamedTypeSymbol[] parameters) {
			if(targetAtribute == null){
				return false;
			}
			foreach (var attribute in symbol.GetAttributes()) {
				if (attribute.AttributeClass?.Is(targetAtribute) == true) {
					var match = attribute.ConstructorArguments.Select(x => x.Value as INamedTypeSymbol).SequenceEqual(parameters, SymbolEqualityComparer.Default);
					if (match) {
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Attempts to retrieve a named argument value from an attribute.
		/// </summary>
		/// <param name="attributeData">The attribute data to query.</param>
		/// <param name="name">The name of the argument to retrieve.</param>
		/// <param name="result">When this method returns true, contains the typed constant value; otherwise, an empty TypedConstant.</param>
		/// <returns>True if the named argument was found; otherwise, false.</returns>
		public static bool TryGetNamedArgument(this AttributeData attributeData, string name, out TypedConstant result) {
			var argument = attributeData.NamedArguments.Where(x => x.Key == name).Select<KeyValuePair<string, TypedConstant>, TypedConstant?>(x => x.Value).FirstOrDefault();
			if (argument != null) {
				result = argument.Value;
				return true;
			} else {
				result = new TypedConstant();
				return false;
			}
		}
	}
}