using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Albatross.CodeAnalysis.Symbols {
	public class SymbolProvider {
		protected readonly Compilation compilation;

		public SymbolProvider(Compilation compilation) {
			this.compilation = compilation;
			this.String = compilation.GetSpecialType(SpecialType.System_String);
			this.IEnumerable = GetRequiredType("System.Collections.IEnumerable");
			this.Nullable = GetRequiredType("System.Nullable`1");
			this.IAsyncEnumerable = GetRequiredType("System.Collections.Generic.IAsyncEnumerable`1");
		}

		#region symbols
		public INamedTypeSymbol String { get; }
		public INamedTypeSymbol IEnumerable { get; }
		public INamedTypeSymbol Nullable { get; }
		public INamedTypeSymbol GetNullableOf(ITypeSymbol type) => Nullable.Construct(type);
		public INamedTypeSymbol IAsyncEnumerable { get; }
		#endregion

		#region nullable
		public bool IsNullable(ITypeSymbol symbol) => symbol is INamedTypeSymbol named
			&& (named.IsGenericType && named.OriginalDefinition.Is(this.Nullable) || symbol.NullableAnnotation == NullableAnnotation.Annotated);
		public bool IsNullableReferenceType(ITypeSymbol symbol) => symbol is INamedTypeSymbol named 
			&& !named.IsValueType && symbol.NullableAnnotation == NullableAnnotation.Annotated;
		public bool IsNullableValueType(ITypeSymbol symbol) => symbol is INamedTypeSymbol named
			&& named.IsGenericType && named.OriginalDefinition.Is(this.Nullable);
		public bool TryGetNullableValueType(ITypeSymbol symbol, [NotNullWhen(true)] out ITypeSymbol? valueType) {
			if (symbol is INamedTypeSymbol named && named.IsGenericType && named.OriginalDefinition.Is(this.Nullable)) {
				valueType = named.TypeArguments.Single();
				return true;
			} else {
				valueType = null;
				return false;
			}
		}
		#endregion

		#region collection
		public bool IsCollection(ITypeSymbol symbol) {
			if (symbol.SpecialType == SpecialType.System_String) {
				return false;
			} else if (symbol is IArrayTypeSymbol) {
				return true;
			} else {
				return symbol.Is(this.IEnumerable) || symbol.AllInterfaces.Any(x => x.Is(this.IEnumerable));
			}
		}
		#endregion

		#region 
		public bool TryGetGenericTypeArguments(ITypeSymbol symbol, INamedTypeSymbol genericDefinition, out ITypeSymbol[] arguments) {
			if (symbol is INamedTypeSymbol named && named.IsGenericType && named.OriginalDefinition.Is(genericDefinition)) {
				arguments = named.TypeArguments.ToArray();
				return true;
			} else {
				arguments = Array.Empty<ITypeSymbol>();
				return false;
			}
		}
		public bool IsDerivedFrom(ITypeSymbol typeSymbol, INamedTypeSymbol baseTypeSymbol) {
			if (typeSymbol.TypeKind == TypeKind.Class) {
				for (var target = typeSymbol.BaseType; target != null; target = target.BaseType) {
					if (target.Is(baseTypeSymbol)) {
						return true;
					}
				}
			}
			return false;
		}
	}
}
