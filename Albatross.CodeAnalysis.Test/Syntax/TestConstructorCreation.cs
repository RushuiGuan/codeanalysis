using Albatross.CodeAnalysis.Syntax;
using Albatross.Testing;
using Xunit;

namespace Albatross.CodeAnalysis.Test.Syntax {
	public class TestConstructorCreation {
		const string ClassBuilderWithConstructor_Expected = @"public class Test
{
	public Test()
	{
	}
}
";
		[Fact]
		public void SimpleConstructor() {
			var node = new CodeStack()
				.Begin(new ClassDeclarationBuilder("Test").Public())
					.Begin(new ConstructorDeclarationBuilder("Test").Public()).End()
				.End().Build();
			ClassBuilderWithConstructor_Expected.EqualsIgnoringLineEndings(node);
		}
		const string ConstructorWithParameter_Expected = @"public class Test
{
	public Test(string name)
	{
	}
}
";

		[Fact]
		public void ConstructorWithParameter() {
			var node = new CodeStack()
				.Begin(new ClassDeclarationBuilder("Test").Public())
					.Begin(new ConstructorDeclarationBuilder("Test").Public()).With(new ParameterNode(new TypeNode("string"), "name"))
					.End()
				.End().Build();
			ConstructorWithParameter_Expected.EqualsIgnoringLineEndings(node);
		}
		const string ConstructorWithParameterAndBaseCall_Expected = @"public class Test
{
	public Test(string name) : base(name)
	{
	}
}
";
		[Fact]
		public void ConstructorWithParameterAndBaseCall() {
			var node = new CodeStack()
				.Begin(new ClassDeclarationBuilder("Test").Public())
					.Begin(new ConstructorDeclarationBuilder("Test").Public()).With(new ParameterNode(new TypeNode("string"), "name"))
						.Begin(new ArgumentListBuilder()).With(new IdentifierNode("name")).End()
					.End()
				.End().Build();
			ConstructorWithParameterAndBaseCall_Expected.EqualsIgnoringLineEndings(node);
		}
	}
}