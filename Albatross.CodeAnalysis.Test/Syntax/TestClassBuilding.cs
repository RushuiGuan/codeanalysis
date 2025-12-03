using Albatross.CodeAnalysis.Syntax;
using Albatross.Testing;
using Xunit;

namespace Albatross.CodeAnalysis.Test.Syntax {
	public class TestClassBuilding {
		[Theory]
		[InlineData("public class Test{}", "Test")]
		public void SimpleClass(string expected, string className) {
			var node = new CodeStack()
				.Begin(new ClassDeclarationBuilder(className).Public())
				.End().Build();
			expected.EqualsIgnoringLineEndings(node);
		}

		const string ClassWithAttribute_Expected = @"[Test]
public class MyClass
{
}
";
		[Fact]
		public void ClassWithAttribute() {
			var node = new CodeStack().Begin(new ClassDeclarationBuilder("MyClass").Public()).Begin(new AttributeBuilder("Test")).End().End().Build();
			ClassWithAttribute_Expected.EqualsIgnoringLineEndings(node);
		}
		const string ClassWithMultipleAttributes_Expected = @"[Test1]
[Test2]
public class MyClass
{
}
";
		[Fact]
		public void ClassWithMultipleAttributes() {
			var node = new CodeStack().Begin(new ClassDeclarationBuilder("MyClass").Public())
				.Begin(new AttributeBuilder("Test1")).End()
				.Begin(new AttributeBuilder("Test2")).End()
			.End().Build();
			ClassWithMultipleAttributes_Expected.EqualsIgnoringLineEndings(node);
		}
		const string ClassWithMultipleAttributesOfTheSameType_Expected = @"[Test(1)]
[Test(2)]
public class MyClass
{
}
";
		[Fact]
		public void ClassWithMultipleAttributesOfTheSameType() {
			var node = new CodeStack().Begin(new ClassDeclarationBuilder("MyClass").Public())
				.Begin(new AttributeBuilder("Test")).Begin(new AttributeArgumentListBuilder()).With(new LiteralNode(1)).End().End()
				.Begin(new AttributeBuilder("Test")).Begin(new AttributeArgumentListBuilder()).With(new LiteralNode(2)).End().End()
			.End().Build();
			ClassWithMultipleAttributesOfTheSameType_Expected.EqualsIgnoringLineEndings(node);
		}

		const string ClassWithAttributeAndConstructorArguments_Expected = @"[Test(1, 2, 3)]
public class MyClass
{
}
";
		[Fact]
		public void ClassWithAttributeAndAttributeArguments() {
			var node = new CodeStack().Begin(new ClassDeclarationBuilder("MyClass").Public())
				.Begin(new AttributeBuilder("Test"))
					.Begin(new AttributeArgumentListBuilder())
						.With(new LiteralNode(1), new LiteralNode(2), new LiteralNode(3))
					.End()
				.End()
			.End().Build();
			ClassWithAttributeAndConstructorArguments_Expected.EqualsIgnoringLineEndings(node);
		}
		const string ClassWithAttributeAndAttributeNamedArguments_Expected = @"[Test(1, 2, 3, Name = ""a"")]
public class MyClass
{
}
";
		[Fact]
		public void ClassWithAttributeAndAttributeNamedArguments() {
			var node = new CodeStack().Begin(new ClassDeclarationBuilder("MyClass").Public())
				.Begin(new AttributeBuilder("Test"))
					.Begin(new AttributeArgumentListBuilder())
						.With(new LiteralNode(1), new LiteralNode(2), new LiteralNode(3))
						.Begin(new AssignmentExpressionBuilder("Name"))
							.With(new LiteralNode("a"))
						.End()
					.End()
				.End()
			.End().Build();
			ClassWithAttributeAndAttributeNamedArguments_Expected.EqualsIgnoringLineEndings(node);
		}

		const string NamespaceWithUsingDirective_Expected = @"#nullable enable
namespace test
{
	using System;

	public class MyClass
	{
	}

	public interface MyInterface
	{
	}
}
#nullable disable";

		[Fact]
		public void NamespaceWithUsingDirective() {
			var node = new CodeStack().Begin(new NamespaceDeclarationBuilder("test"))
					.With(new UsingDirectiveNode("System"))
					.With(new UsingDirectiveNode("System"))
					.Begin(new ClassDeclarationBuilder("MyClass").Public()).End()
					.Begin(new InterfaceDeclarationBuilder("MyInterface").Public()).End()
				.End().Build();
			NamespaceWithUsingDirective_Expected.EqualsIgnoringLineEndings(node);
		}

		const string CompilationUnitWithUsingDirective_Expected = @"using System;

public class MyTest
{
}
";
		[Fact]
		public void CompilationUnitWithUsingDirective() {
			var node = new CodeStack().Begin(new CompilationUnitBuilder())
					.With(new UsingDirectiveNode("System"))
					.Begin(new ClassDeclarationBuilder("MyTest").Public()).End()
				.End().Build();
			CompilationUnitWithUsingDirective_Expected.EqualsIgnoringLineEndings(node);
		}

		const string ClassWithBaseType_Expected = @"public class Test : MyBase, Interface1, Interface2
{
}
";
		[Fact]
		public void ClassWithBaseType() {
			var nod = new CodeStack().Begin(new ClassDeclarationBuilder("Test").Public())
				.With(new BaseTypeNode("MyBase"), new BaseTypeNode("Interface1"), new BaseTypeNode("Interface2"))
				.End().Build();
			ClassWithBaseType_Expected.EqualsIgnoringLineEndings(nod);
		}
	}
}