using Albatross.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Albatross.CodeAnalysis.Test.Symbols {
	public class TestAttributeCheck {
		[Fact]
		public async Task TestGetGenericAttributeName() {
			var compilation = await @"
	using System;
	using System.Text.Json.Serialization;
	[JsonConverterAttribute(typeof(JsonStringEnumConverter))]
	public enum MyEnum1 { None }
	
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum MyEnum2 { None }
".CreateNet8CompilationAsync();
			var symbolProvider = new MySymbolProvider(compilation);
			var errors = compilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).ToArray();
			if (errors.Any()) {
				errors.ToList().ForEach(x => Console.WriteLine(x));
			}
			var symbol = compilation.GetRequiredSymbol("MyEnum1");
			Assert.True(symbol.HasAttributeWithConstructorArguments(symbolProvider.JsonConverterAttribute, symbolProvider.JsonStringEnumConverter));
		}
	}
}