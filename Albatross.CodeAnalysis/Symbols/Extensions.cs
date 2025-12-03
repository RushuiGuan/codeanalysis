using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Albatross.CodeAnalysis.Symbols {
	public static class Extensions {
		#region nullable helpers

		public static bool IsNullable(this ITypeSymbol? symbol, Compilation compilation)
			=> symbol.IsNullableReferenceType() || symbol.IsNullableValueType(compilation);
		
		public static bool IsNullableReferenceType(this ITypeSymbol? symbol) 
			=> symbol is INamedTypeSymbol { IsValueType: false } && symbol.NullableAnnotation == NullableAnnotation.Annotated 
			   || symbol is IArrayTypeSymbol { NullableAnnotation: NullableAnnotation.Annotated };
		
		public static bool IsNullableValueType(this ITypeSymbol? symbol, Compilation compilation) 
			=> symbol is INamedTypeSymbol { IsGenericType: true } named 
			   && named.OriginalDefinition.Is(compilation.Nullable());
		
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
		public static INamedTypeSymbol GetRequiredSymbol(this Compilation compilation, string typeName) {
			var symbol = compilation.GetTypeByMetadataName(typeName);
			if (symbol == null) {
				throw new ArgumentException($"Type {typeName} is not found");
			}
			return symbol;
		}
		public static bool Is(this ISymbol? left, ISymbol? right) => SymbolEqualityComparer.Default.Equals(left, right);
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
		public static string GetFullNamespace(this INamespaceSymbol symbol) {
			if (symbol.IsGlobalNamespace) {
				return string.Empty;
			} else if (symbol.ContainingNamespace.IsGlobalNamespace) {
				return symbol.Name;
			} else {
				return $"{symbol.ContainingNamespace.GetFullNamespace()}.{symbol.Name}";
			}
		}
		public static bool IsPartial(this INamedTypeSymbol? symbol) => 
			symbol != null &&
			symbol.DeclaringSyntaxReferences.Select(x => x.GetSyntax())
				.OfType<InterfaceDeclarationSyntax>()
				.Any(x => x.Modifiers.Any(SyntaxKind.PartialKeyword));
		public static string GetTypeNameRelativeToNamespace(this ITypeSymbol symbol, string currentNamespace) {
			var fullName = symbol.GetFullName();
			if (fullName.StartsWith(currentNamespace + ".")) {
				return fullName.Substring(currentNamespace.Length + 1);
			} else {
				return symbol.GetFullName();
			}
		}
		public static bool IsGenericTypeDefinition(this INamedTypeSymbol? symbol) 
			=> symbol is { IsGenericType: true, IsDefinition: true };
		public static bool IsConcreteClass(this INamedTypeSymbol? symbol) {
			if (symbol is not { TypeKind: TypeKind.Class }) { 
				return false; 
			} else {
				return symbol is { IsAbstract: false, IsStatic: false }
				       && !IsGenericTypeDefinition(symbol);
			}
		}

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
		public static IEnumerable<IPropertySymbol> GetDistinctProperties(this INamedTypeSymbol symbol, bool useBaseClassProperties) {
			var set = new HashSet<string>();
			foreach (var item in GetProperties(symbol, useBaseClassProperties)) {
				if (set.Add(item.Name)) {
					yield return item;
				}
			}
		}
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