using Albatross.CodeAnalysis.Symbols;
using FluentAssertions;
using Xunit;

namespace Albatross.CodeAnalysis.Test.Symbols {
	public class TestIsNullable {
		const string code = @"
	public class TestClass {
		public int Value { get; set; }
		public int? NullableValue { get; set; }
		public string Text { get; set; } = string.Empty;
		public string? NullableText { get; set; }
		public int[] Array { get; set; } = Array.Empty<int>();
		public int[]? NullableArray {get; set; }
	}
";
		[Theory]
		[InlineData("Value", false)]
		[InlineData("NullableValue", true)]
		[InlineData("Text", false)]
		[InlineData("NullableText", true)]
		[InlineData("Array", false)]
		[InlineData("NullableArray", true)]
		public async Task VerifyIsNullable(string property, bool nullable) {
			var compilation = await code.CreateNet8CompilationAsync();
			var symbol = compilation.GetRequiredSymbol("TestClass");
			var propertySymbol  = symbol.GetMembers().OfType<Microsoft.CodeAnalysis.IPropertySymbol>().Where(x => x.Name == property).First();
			propertySymbol.Type.IsNullable(new SymbolProvider(compilation)).Should().Be(nullable);
		}

		[Theory]
		[InlineData("Value", false)]
		[InlineData("NullableValue", true)]
		[InlineData("Text", false)]
		[InlineData("NullableText", false)]
		[InlineData("Array", false)]
		[InlineData("NullableArray", false)]
		public async Task VerifyIsNullableValueType(string property, bool nullable) {
			var compilation = await code.CreateNet8CompilationAsync();
			var symbol = compilation.GetRequiredSymbol("TestClass");
			var propertySymbol = symbol.GetMembers().OfType<Microsoft.CodeAnalysis.IPropertySymbol>().Where(x => x.Name == property).First();
			propertySymbol.Type.IsNullableValueType(new SymbolProvider(compilation)).Should().Be(nullable);
		}

		[Theory]
		[InlineData("Value", false)]
		[InlineData("NullableValue", false)]
		[InlineData("Text", false)]
		[InlineData("NullableText", true)]
		[InlineData("Array", false)]
		[InlineData("NullableArray", true)]
		public async Task VerifyIsNullableReferenceType(string property, bool nullable) {
			var compilation = await code.CreateNet8CompilationAsync();
			var symbol = compilation.GetRequiredSymbol("TestClass");
			var propertySymbol = symbol.GetMembers().OfType<Microsoft.CodeAnalysis.IPropertySymbol>().Where(x => x.Name == property).First();
			propertySymbol.Type.IsNullableReferenceType().Should().Be(nullable);
		}

		[Theory]
		[InlineData("Value", false, null)]
		[InlineData("NullableValue", true, "System.Int32")]
		[InlineData("Text", false, null)]
		[InlineData("NullableText", false, null)]
		public async Task VerifyTryGetNullableValueType(string property, bool nullable, string? genericType) {
			var compilation = await code.CreateNet8CompilationAsync();
			var symbol = compilation.GetRequiredSymbol("TestClass");
			var propertySymbol = symbol.GetMembers().OfType<Microsoft.CodeAnalysis.IPropertySymbol>().Where(x => x.Name == property).First();
			var result = propertySymbol.Type.TryGetNullableValueType(new SymbolProvider(compilation), out var underlyingType);
			Assert.Equal(nullable, result);
			Assert.Equal(genericType, underlyingType?.GetFullName());

		}
	}
}
