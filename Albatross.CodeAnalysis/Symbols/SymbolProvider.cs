using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Albatross.CodeAnalysis.Symbols {
	public class SymbolProvider {
		protected readonly Compilation compilation;

		public SymbolProvider(Compilation compilation) {
			this.compilation = compilation;
			this.String = compilation.GetSpecialType(SpecialType.System_String);
			this.IEnumerable = compilation.GetRequiredSymbol("System.Collections.IEnumerable");
			this.Nullable = compilation.GetRequiredSymbol("System.Nullable`1");
			this.IAsyncEnumerable = compilation.GetRequiredSymbol("System.Collections.Generic.IAsyncEnumerable`1");
			this.IEnumerableGenericDefinition = compilation.GetRequiredSymbol("System.Collections.Generic.IEnumerable`1");
		}

		#region symbols
		public INamedTypeSymbol String { get; }
		public INamedTypeSymbol IEnumerable { get; }
		public INamedTypeSymbol Nullable { get; }
		public INamedTypeSymbol GetNullableOf(ITypeSymbol type) => Nullable.Construct(type);
		public INamedTypeSymbol IAsyncEnumerable { get; }
		public INamedTypeSymbol IEnumerableGenericDefinition { get; }
		#endregion

		
	}
}
