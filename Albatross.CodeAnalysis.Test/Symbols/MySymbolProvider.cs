using Albatross.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albatross.CodeAnalysis.Test.Symbols {
	public class MySymbolProvider : SymbolProvider {
		public MySymbolProvider(Compilation compilation) : base(compilation) {
			this.JsonConverterAttribute = compilation.GetRequiredSymbol("System.Text.Json.Serialization.JsonConverterAttribute");
			this.JsonStringEnumConverter = compilation.GetRequiredSymbol("System.Text.Json.Serialization.JsonStringEnumConverter");
		}

		public INamedTypeSymbol JsonConverterAttribute { get; }
		public INamedTypeSymbol JsonStringEnumConverter { get; }
	}
}
