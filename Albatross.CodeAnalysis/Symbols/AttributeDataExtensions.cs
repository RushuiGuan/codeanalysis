using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Albatross.CodeAnalysis.Symbols {
	public static class AttributeDataExtensions {
		public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol targetAttribute)
			=> symbol.GetAttributes().Any(x => x.AttributeClass?.Is(targetAttribute) == true);

		public static bool TryGetAttribute(this ISymbol symbol, INamedTypeSymbol targetAttribute, [NotNullWhen(true)]out AttributeData? attributeData) {
			foreach (var attribute in symbol.GetAttributes()) {
				if (attribute.AttributeClass?.Is(targetAttribute) == true) {
					attributeData = attribute;
					return true;
				}
			}
			attributeData = null;
			return false;
		}

		public static bool HasAttributeWithBaseType(this ISymbol symbol, INamedTypeSymbol baseType) 
			=> symbol.GetAttributes().Any(x => x.AttributeClass?.BaseType?.Is(baseType) == true);

		public static bool HasAttributeWithConstructorArguments(this ISymbol symbol, INamedTypeSymbol targetAtributes, params INamedTypeSymbol[] parameters) {
			foreach (var attribute in symbol.GetAttributes()) {
				if (attribute.AttributeClass?.Is(targetAtributes) == true) {
					var match = attribute.ConstructorArguments.Select(x => x.Value as INamedTypeSymbol).SequenceEqual(parameters, SymbolEqualityComparer.Default);
					if (match) {
						return true;
					}
				}
			}
			return false;
		}

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