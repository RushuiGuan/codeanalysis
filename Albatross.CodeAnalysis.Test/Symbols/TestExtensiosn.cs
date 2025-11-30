using Albatross.CodeAnalysis.Symbols;
using Albatross.CodeAnalysis.Syntax;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Albatross.CodeAnalysis.Test.Symbols {
	public class TestExtensiosn {
		[Fact]
		public async Task TestIsDerivedFrom() {
			var compilation =await @"
				namespace Test {
					public class MyBase { }
					public class MyClass : MyBase { }
					public class YourClass : MyClass{ }
				}
			".CreateNet8CompilationAsync();

			var yourClass = compilation.GetRequiredSymbol("Test.YourClass");
			var myBase = compilation.GetRequiredSymbol("Test.MyBase");
			Assert.True(yourClass.IsDerivedFrom(compilation.GetRequiredSymbol("Test.MyBase")));
			Assert.False(myBase.IsDerivedFrom(compilation.GetRequiredSymbol("Test.MyBase")));
		}
		[Fact]
		public async Task TestIsConstructedFrom() {
			var compilation = await @"
				namespace Test {
					public class MyBase<T> { }
					public class MyClass : MyBase<string> { }
				}
			".CreateNet8CompilationAsync();
			var genericDefinition = compilation.GetRequiredSymbol("Test.MyBase`1");
			var type = genericDefinition.Construct(compilation.GetRequiredSymbol("System.String"));
			Assert.True(type.IsConstructedFromDefinition(genericDefinition));

			type = compilation.GetRequiredSymbol("Test.MyClass");
			Assert.False(type.IsConstructedFromDefinition(genericDefinition));
		}

		[Fact]
		public async Task TestIsNullable() {
			var compilation = await @"
using System;
public class MyClass {
	public string? Text{ get; set; }
	publis string Text2{get; set; }
	public int? Number{ get; set; }
	public Nullable<int> Number2{ get; set; }
	public int Number3{ get; set; }
}
			".CreateNet8CompilationAsync();
			var type = compilation.GetRequiredSymbol("MyClass");
			var textProperty = (IPropertySymbol)type.GetMembers("Text").First();
			var text2Property = (IPropertySymbol)type.GetMembers("Text2").First();

			var numberProperty = (IPropertySymbol)type.GetMembers("Number").First();
			var number2Property = (IPropertySymbol)type.GetMembers("Number2").First();
			var number3Property = (IPropertySymbol)type.GetMembers("Number3").First();

			textProperty.Type.IsNullableReferenceType().Should().BeTrue();
			textProperty.Type.IsNullableValueType(compilation).Should().BeFalse();
			text2Property.Type.IsNullableValueType(compilation).Should().BeFalse();
			text2Property.Type.IsNullableValueType(compilation).Should().BeFalse();

			numberProperty.Type.IsNullableReferenceType().Should().BeFalse();
			numberProperty.Type.IsNullableValueType(compilation).Should().BeTrue();

			number2Property.Type.IsNullableReferenceType().Should().BeFalse();
			number2Property.Type.IsNullableValueType(compilation).Should().BeTrue();

			number3Property.Type.IsNullableReferenceType().Should().BeFalse();
			number3Property.Type.IsNullableValueType(compilation).Should().BeFalse();
		}

		[Fact]
		public async Task TestGetNullableValueType() {
			var compilation = await  @"
using System;
public class MyClass {
public string? Text{ get; set; }
	public int? Number{ get; set; }
	public Nullable<int> Number2{ get; set; }
	public int Number3{ get; set; }
}
			".CreateNet8CompilationAsync();
			var type = compilation.GetRequiredSymbol("MyClass");
			var textProperty = (IPropertySymbol)type.GetMembers("Text").First();
			var numberProperty = (IPropertySymbol)type.GetMembers("Number").First();
			var number2Property = (IPropertySymbol)type.GetMembers("Number2").First();
			var number3Property = (IPropertySymbol)type.GetMembers("Number3").First();

			textProperty.Type.TryGetNullableValueType(compilation, out var valueType).Should().BeFalse();
			numberProperty.Type.TryGetNullableValueType(compilation, out valueType).Should().BeTrue();
			valueType!.GetFullName().Should().Be("System.Int32");

			number2Property.Type.TryGetNullableValueType(compilation, out valueType).Should().BeTrue();
			valueType!.GetFullName().Should().Be("System.Int32");

			number3Property.Type.TryGetNullableValueType(compilation, out valueType).Should().BeFalse();
		}

		[Theory]
		[InlineData("int", "System.Int32")]
		[InlineData("int?", "System.Nullable<System.Int32>")]
		[InlineData("string?", "System.String?")]
		[InlineData("string", "System.String")]
		[InlineData("int[]", "System.Int32[]")]
		[InlineData("int?[]", "System.Nullable<System.Int32>[]")]
		[InlineData("string[]", "System.String[]")]
		[InlineData("string?[]", "System.String? []")]
		public async Task TypeSymbol2TypeNodeConversion(string typeName, string expectedResult) {
			var code = @"class A { [Type] Field; }".Replace("[Type]", typeName);
			var compilation = await code.CreateNet8CompilationAsync();
			var classType = compilation.GetRequiredSymbol("A");
			var type = classType.GetMembers("Field").First().As<IFieldSymbol>().Type;
			var result = type.AsTypeNode();
			result.Node.NormalizeWhitespace().ToFullString().Should().Be(expectedResult);
		}

		[Fact]
		public async Task TestIsOpenGenericType() {
			const string code = @"public class MyClass<T>{}";
			var compilation = await code.CreateNet8CompilationAsync();
			var classType = compilation.GetRequiredSymbol("MyClass`1");
			classType.IsGenericTypeDefinition().Should().BeTrue();
		}

		[Theory]
		[InlineData("MyNamespace", "CommandHandler.MyClass")]
		[InlineData("MyNamespace.CommandHandler", "MyClass")]
		[InlineData("XXX", "MyNamespace.CommandHandler.MyClass")]
		public async Task TestGetTypeNameRelativeToNamespace(string currentNamespace, string expected) {
			const string code = @"namespace MyNamespace.CommandHandler { public class MyClass{} }";
			var compilation = await code.CreateNet8CompilationAsync();
			var classType = compilation.GetRequiredSymbol("MyNamespace.CommandHandler.MyClass");
			classType.GetTypeNameRelativeToNamespace(currentNamespace).Should().Be(expected);
		}
	}
}