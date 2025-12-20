using Albatross.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Xunit;
namespace Albatross.CodeAnalysis.UnitTest.Symbols {
	public class TestSymbolName {
		[Theory]
		[InlineData("class MyClass{ string P1;}", "System.String")]
		[InlineData("class MyClass{ decimal P1;}", "System.Decimal")]
		[InlineData("class MyClass{ MyClass P1;}", "MyClass")]
		[InlineData("class MyClass{ byte[] P1;}", "System.Byte[]")]
		[InlineData("namespace XX { class A{} } class MyClass{ XX.A P1;}", "XX.A")]
		[InlineData("using System.Collections.Generic; class MyClass{ List<string> P1;}", "System.Collections.Generic.List<System.String>")]
		[InlineData("using C = System.Collections; class MyClass{ C.ArrayList P1;}", "System.Collections.ArrayList")]
		public async Task Run(string code, string expected) {
			var compilation = await code.CreateNet8CompilationAsync();
			var symbol = compilation.GetRequiredSymbol("MyClass");
			var p1 = symbol.GetMembers().OfType<IFieldSymbol>().Where(x => x.Name == "P1").First();
			Assert.Equal(expected, p1.Type.GetFullName());
		}

		[Fact]
		public async Task TestGenericDefinitionName() {
			var compilation =await @"
				namespace Test {
					public class MyBase<T> { }
					public class MyBase<T, K> { }
					public class MyClass : MyBase<string> { }
				}
			".CreateNet8CompilationAsync();
			var genericDefinition = compilation.GetRequiredSymbol("Test.MyBase`1");
			Assert.Equal("Test.MyBase<>", genericDefinition.GetFullName());
			genericDefinition = compilation.GetRequiredSymbol("Test.MyBase`2");
			Assert.Equal("Test.MyBase<,>", genericDefinition.GetFullName());
		}
		[Fact]
		public async Task TestGetAttributeName() {
			var compilation = await @"[System.Serializable] class MyClass{}".CreateNet8CompilationAsync();
			var symbol = compilation.GetRequiredSymbol("MyClass");
			var attribute = symbol.GetAttributes().First();
			Assert.Equal("System.SerializableAttribute", attribute.AttributeClass?.GetFullName());
		}
	}
}