using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Albatross.CodeAnalysis.Symbols {
	/// <summary>
	/// Provides extension methods for working with Roslyn type symbols, including helpers for nullable types,
	/// collections, generic types, and symbol comparison.
	/// </summary>
	public static class Extensions {
		#region nullable helpers

		/// <summary>
		/// Determines whether a type symbol represents a nullable type (either a nullable reference type or nullable value type).
		/// </summary>
		/// <param name="symbol">The type symbol to check.</param>
		/// <param name="compilation">The compilation context.</param>
		/// <returns>True if the type is nullable; otherwise, false.</returns>
		public static bool IsNullable(this ITypeSymbol? symbol, Compilation compilation)
			=> symbol.IsNullableReferenceType() || symbol.IsNullableValueType(compilation);
		
		/// <summary>
		/// Determines whether a type symbol represents a nullable reference type (annotated with nullable annotation).
		/// </summary>
		/// <param name="symbol">The type symbol to check.</param>
		/// <returns>True if the type is a nullable reference type; otherwise, false.</returns>
		public static bool IsNullableReferenceType(this ITypeSymbol? symbol) 
			=> symbol is INamedTypeSymbol { IsValueType: false } && symbol.NullableAnnotation == NullableAnnotation.Annotated 
			   || symbol is IArrayTypeSymbol { NullableAnnotation: NullableAnnotation.Annotated };
		
		/// <summary>
		/// Determines whether a type symbol represents a nullable value type (e.g., int?, DateTime?).
		/// </summary>
		/// <param name="symbol">The type symbol to check.</param>
		/// <param name="compilation">The compilation context.</param>
		/// <returns>True if the type is a nullable value type; otherwise, false.</returns>
		public static bool IsNullableValueType(this ITypeSymbol? symbol, Compilation compilation) 
			=> symbol is INamedTypeSymbol { IsGenericType: true } named 
			   && named.OriginalDefinition.Is(compilation.Nullable());
		
		/// <summary>
		/// Attempts to extract the underlying value type from a nullable value type.
		/// </summary>
		/// <param name="symbol">The type symbol to check.</param>
		/// <param name="compilation">The compilation context.</param>
		/// <param name="valueType">When this method returns true, contains the underlying value type; otherwise, null.</param>
		/// <returns>True if the type is a nullable value type; otherwise, false.</returns>
		public static bool TryGetNullableValueType(this ITypeSymbol? symbol, Compilation compilation, [NotNullWhen(true)] out ITypeSymbol? valueType) {
			if (symbol is INamedTypeSymbol { IsGenericType: true } named && named.OriginalDefinition.Is(compilation.Nullable())) {
				valueType = named.TypeArguments.Single();
				return true;
			} else {
				valueType = null;
				return false;
			}
		}
		#endregion

		#region collection helpers
		/// <summary>
		/// Determines whether a type symbol represents a collection type (implements IEnumerable or is an array).
		/// Excludes System.String even though it implements IEnumerable.
		/// </summary>
		/// <param name="symbol">The type symbol to check.</param>
		/// <param name="compilation">The compilation context.</param>
		/// <returns>True if the type is a collection; otherwise, false.</returns>
		public static bool IsCollection(this ITypeSymbol? symbol, Compilation compilation) {
			if (symbol == null) {
				return false;
			}
			if (symbol.SpecialType == SpecialType.System_String) {
				return false;
			} else if (symbol is IArrayTypeSymbol) {
				return true;
			} else {
				return symbol.Is(compilation.IEnumerable()) || symbol.AllInterfaces.Any(x => x.Is(compilation.IEnumerable()));
			}
		}
		
		/// <summary>
		/// Attempts to determine the element type of a collection type.
		/// Works with arrays and generic IEnumerable implementations. Excludes System.String.
		/// </summary>
		/// <param name="typeSymbol">The type symbol to check.</param>
		/// <param name="compilation">The compilation context.</param>
		/// <param name="elementType">When this method returns true, contains the element type; otherwise, null.</param>
		/// <returns>True if the element type was determined; otherwise, false.</returns>
		public static bool TryGetCollectionElementType(this ITypeSymbol? typeSymbol, Compilation compilation, [NotNullWhen(true)] out ITypeSymbol? elementType) {
			if (typeSymbol == null) {
				elementType = null;
				return false;
			}
			if (typeSymbol.SpecialType == SpecialType.System_String) {
				elementType = null;
				return false;
			} else if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol) {
				elementType = arrayTypeSymbol.ElementType;
				return true;
			} else {
				if (typeSymbol.TryGetGenericTypeArguments( compilation.IEnumerableGenericDefinition(), out var arguments)) {
					elementType = arguments[0];
					return true;
				} else {
					foreach (var @interface in typeSymbol.AllInterfaces) {
						if (@interface.TryGetGenericTypeArguments(compilation.IEnumerableGenericDefinition(), out var args)) {
							elementType = args[0];
							return true;
						}
					}
					elementType = null;
					return false;
				}
			}
		}
		#endregion

		#region generic type helpers
		/// <summary>
		/// Attempts to extract the type arguments from a generic type that matches the specified generic definition.
		/// </summary>
		/// <param name="symbol">The type symbol to check.</param>
		/// <param name="genericDefinition">The generic type definition to match against.</param>
		/// <param name="arguments">When this method returns true, contains the type arguments; otherwise, an empty array.</param>
		/// <returns>True if the type is a constructed generic type matching the definition; otherwise, false.</returns>
		public static bool TryGetGenericTypeArguments(this ITypeSymbol symbol, INamedTypeSymbol genericDefinition, out ITypeSymbol[] arguments) {
			if (symbol is INamedTypeSymbol named && named.IsGenericType && named.OriginalDefinition.Is(genericDefinition)) {
				arguments = named.TypeArguments.ToArray();
				return true;
			} else {
				arguments = Array.Empty<ITypeSymbol>();
				return false;
			}
		}
		#endregion
		
		/// <summary>
		/// Gets a type symbol by its metadata name, throwing an exception if not found.
		/// Useful when a type is known to exist in the compilation.
		/// </summary>
		/// <param name="compilation">The compilation to search.</param>
		/// <param name="typeName">The fully qualified metadata name of the type.</param>
		/// <returns>The named type symbol.</returns>
		/// <exception cref="ArgumentException">Thrown when the type is not found in the compilation.</exception>
		public static INamedTypeSymbol GetRequiredSymbol(this Compilation compilation, string typeName) {
			var symbol = compilation.GetTypeByMetadataName(typeName);
			if (symbol == null) {
				throw new ArgumentException($"Type {typeName} is not found");
			}
			return symbol;
		}
		
		/// <summary>
		/// Compares two symbols for equality using the default symbol equality comparer.
		/// </summary>
		/// <param name="left">The first symbol to compare.</param>
		/// <param name="right">The second symbol to compare.</param>
		/// <returns>True if the symbols are equal; otherwise, false.</returns>
		public static bool Is(this ISymbol? left, ISymbol? right) => SymbolEqualityComparer.Default.Equals(left, right);
		
		/// <summary>
		/// Gets the fully qualified name of a type symbol, including namespace and generic type arguments.
		/// For arrays, appends brackets to the element type name.
		/// </summary>
		/// <param name="symbol">The type symbol.</param>
		/// <returns>The fully qualified type name.</returns>
		public static string GetFullName(this ITypeSymbol symbol) {
			string fullName;
			if (symbol is IArrayTypeSymbol arraySymbol) {
				fullName = $"{arraySymbol.ElementType.GetFullName()}[]";
			} else if (symbol.ContainingNamespace == null) {
				fullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			} else if (symbol.ContainingNamespace.IsGlobalNamespace) {
				fullName = symbol.Name;
			} else {
				fullName = $"{symbol.ContainingNamespace.GetFullNamespace()}.{symbol.Name}";
			}
			if (symbol is INamedTypeSymbol named && named.IsGenericType) {
				var sb = new StringBuilder(fullName);
				sb.Append("<");
				for (int i = 0; i < named.TypeArguments.Length; i++) {
					if (i > 0) {
						sb.Append(",");
					}
					if (!named.IsDefinition) {
						sb.Append(named.TypeArguments[i].GetFullName());
					}
				}
				sb.Append(">");
				fullName = sb.ToString();
			}
			return fullName;
		}
		
		/// <summary>
		/// Gets the fully qualified namespace name for a namespace symbol.
		/// Returns an empty string for the global namespace.
		/// </summary>
		/// <param name="symbol">The namespace symbol.</param>
		/// <returns>The fully qualified namespace name.</returns>
		public static string GetFullNamespace(this INamespaceSymbol symbol) {
			if (symbol.IsGlobalNamespace) {
				return string.Empty;
			} else if (symbol.ContainingNamespace.IsGlobalNamespace) {
				return symbol.Name;
			} else {
				return $"{symbol.ContainingNamespace.GetFullNamespace()}.{symbol.Name}";
			}
		}
		
		/// <summary>
		/// Determines whether a named type symbol is declared as partial.
		/// Checks for the partial modifier in interface declarations.
		/// </summary>
		/// <param name="symbol">The named type symbol to check.</param>
		/// <returns>True if the type is partial; otherwise, false.</returns>
		public static bool IsPartial(this INamedTypeSymbol? symbol) => 
			symbol != null &&
			symbol.DeclaringSyntaxReferences.Select(x => x.GetSyntax())
				.OfType<InterfaceDeclarationSyntax>()
				.Any(x => x.Modifiers.Any(SyntaxKind.PartialKeyword));
		
		/// <summary>
		/// Gets the type name relative to a specified namespace. If the type is in the current namespace,
		/// returns only the type name without the namespace prefix; otherwise, returns the fully qualified name.
		/// </summary>
		/// <param name="symbol">The type symbol.</param>
		/// <param name="currentNamespace">The current namespace for reference.</param>
		/// <returns>The type name, relative to the current namespace if applicable.</returns>
		public static string GetTypeNameRelativeToNamespace(this ITypeSymbol symbol, string currentNamespace) {
			var fullName = symbol.GetFullName();
			if (fullName.StartsWith(currentNamespace + ".")) {
				return fullName.Substring(currentNamespace.Length + 1);
			} else {
				return symbol.GetFullName();
			}
		}
		
		/// <summary>
		/// Determines whether a named type symbol is a generic type definition (unbound generic type).
		/// </summary>
		/// <param name="symbol">The named type symbol to check.</param>
		/// <returns>True if the type is a generic type definition; otherwise, false.</returns>
		public static bool IsGenericTypeDefinition(this INamedTypeSymbol? symbol) 
			=> symbol is { IsGenericType: true, IsDefinition: true };
		
		/// <summary>
		/// Determines whether a named type symbol is a concrete class (non-abstract, non-static, non-generic-definition class).
		/// </summary>
		/// <param name="symbol">The named type symbol to check.</param>
		/// <returns>True if the type is a concrete class; otherwise, false.</returns>
		public static bool IsConcreteClass(this INamedTypeSymbol? symbol) {
			if (symbol is not { TypeKind: TypeKind.Class }) { 
				return false; 
			} else {
				return symbol is { IsAbstract: false, IsStatic: false }
				       && !IsGenericTypeDefinition(symbol);
			}
		}

		/// <summary>
		/// Gets all public properties (with both public getter and setter) from a type symbol.
		/// Optionally includes properties from base classes.
		/// </summary>
		/// <param name="symbol">The named type symbol.</param>
		/// <param name="useBaseClassProperties">If true, includes properties from base classes; otherwise, only direct properties.</param>
		/// <returns>An enumerable of property symbols.</returns>
		public static IEnumerable<IPropertySymbol> GetProperties(this INamedTypeSymbol symbol, bool useBaseClassProperties) {
			foreach (var member in symbol.GetMembers().OfType<IPropertySymbol>()) {
				if (member.SetMethod?.DeclaredAccessibility == Accessibility.Public
					&& member.GetMethod?.DeclaredAccessibility == Accessibility.Public) {
					yield return member;
				}
			}
			if (useBaseClassProperties && symbol.BaseType != null) {
				foreach (var member in GetProperties(symbol.BaseType, useBaseClassProperties)) {
					yield return member;
				}
			}
		}
		
		/// <summary>
		/// Gets distinct public properties from a type symbol, optionally including base class properties.
		/// If a property is overridden, only the most derived version is included.
		/// </summary>
		/// <param name="symbol">The named type symbol.</param>
		/// <param name="useBaseClassProperties">If true, includes properties from base classes; otherwise, only direct properties.</param>
		/// <returns>An enumerable of distinct property symbols.</returns>
		public static IEnumerable<IPropertySymbol> GetDistinctProperties(this INamedTypeSymbol symbol, bool useBaseClassProperties) {
			var set = new HashSet<string>();
			foreach (var item in GetProperties(symbol, useBaseClassProperties)) {
				if (set.Add(item.Name)) {
					yield return item;
				}
			}
		}
		
		/// <summary>
		/// Determines whether a type symbol represents a numeric type (byte, short, int, long, float, double, decimal, etc.).
		/// </summary>
		/// <param name="symbol">The type symbol to check.</param>
		/// <returns>True if the type is numeric; otherwise, false.</returns>
		public static bool IsNumeric(this ITypeSymbol? symbol) {
			if (symbol == null) {
				return false;
			}
			switch (symbol.SpecialType) {
				case SpecialType.System_Byte:
				case SpecialType.System_SByte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
				case SpecialType.System_UInt32:
				case SpecialType.System_Int64:
				case SpecialType.System_UInt64:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
				case SpecialType.System_Decimal:
					return true;
				default:
					return false;
			}
		}
		
		/// <summary>
		/// Determines whether a named type symbol is constructed from a specific generic type definition.
		/// </summary>
		/// <param name="typeSymbol">The type symbol to check.</param>
		/// <param name="genericDefinitionSymbol">The generic type definition to compare against.</param>
		/// <returns>True if the type is constructed from the specified definition; otherwise, false.</returns>
		public static bool IsConstructedFromDefinition(this INamedTypeSymbol? typeSymbol, INamedTypeSymbol genericDefinitionSymbol) {
			if (typeSymbol == null) {
				return false;
			}
			if (typeSymbol.IsGenericType && genericDefinitionSymbol.IsGenericType && genericDefinitionSymbol.IsDefinition) {
				return SymbolEqualityComparer.Default.Equals(typeSymbol.OriginalDefinition, genericDefinitionSymbol);
			} else {
				return false;
			}
		}
		
		/// <summary>
		/// Determines whether a type symbol derives from a specified base type (directly or indirectly).
		/// Only works for class types.
		/// </summary>
		/// <param name="typeSymbol">The type symbol to check.</param>
		/// <param name="baseTypeSymbol">The base type to check for in the inheritance hierarchy.</param>
		/// <returns>True if the type derives from the base type; otherwise, false.</returns>
		public static bool IsDerivedFrom(this ITypeSymbol typeSymbol, INamedTypeSymbol? baseTypeSymbol) {
			if (baseTypeSymbol == null) {
				return false;
			}
			if (typeSymbol.TypeKind == TypeKind.Class) {
				for (var target = typeSymbol.BaseType; target != null; target = target.BaseType) {
					if (target.Is(baseTypeSymbol)) {
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Determines whether a type symbol implements a specified interface (directly or indirectly).
		/// </summary>
		/// <param name="typeSymbol">The type symbol to check.</param>
		/// <param name="interfaceSymbol">The interface to check for.</param>
		/// <returns>True if the type implements the interface; otherwise, false.</returns>
		public static bool HasInterface(this ITypeSymbol typeSymbol, INamedTypeSymbol? interfaceSymbol) {
			if (interfaceSymbol == null) {
				return false;
			}
			foreach (var @interface in typeSymbol.AllInterfaces) {
				if (@interface.Is(interfaceSymbol)) {
					return true;
				}
			}
			return false;
		}
	}
}