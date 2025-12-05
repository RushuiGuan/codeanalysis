using Albatross.CodeAnalysis.Symbols;
using Albatross.CodeAnalysis.Testing;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Xunit;
namespace Albatross.CodeAnalysis.UnitTest.Symbols {
	public class TestAttributeSymbols {
		public const string AttributeCode = @"
	using System;
	public class MyAttribute: Attribute {
		public string Name { get; }
		public int Id{ get; }
		public MyAttribute(string name) {
			this.Name = name;
		}
		public MyAttribute(int id) {
			this.Id = id;
		}
	}
";
		[Theory]
		[InlineData(@"[My(""a"")]class MyClass{ string P1;}", "a", "System.String")]
		[InlineData(@"[My(1)]class MyClass{ string P1;}", 1, "System.Int32")]
		public async Task Run(string code, object expected, string type) {
			var compilation =await (AttributeCode + code).CreateNet8CompilationAsync();
			var symbol = compilation.GetRequiredSymbol("MyClass");
			var attributeSymbol = compilation.GetRequiredSymbol("MyAttribute");
			symbol.TryGetAttribute(attributeSymbol, out var data);
			Assert.NotNull(data);
			data.ConstructorArguments.FirstOrDefault().Value.Should().Be(expected);
			Assert.IsType(Type.GetType(type)!, data.ConstructorArguments.First().Value);
		}
	}
}