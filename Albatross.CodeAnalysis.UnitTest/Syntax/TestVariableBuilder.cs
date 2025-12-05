using Albatross.CodeAnalysis.Syntax;
using Albatross.Testing;
using Xunit;

namespace Albatross.CodeAnalysis.UnitTest {
	public class TestVariableBuilder {
		[Theory]
		[InlineData("var test = 1", null, "test", 1)]
		[InlineData("int test = 1", "int", "test", 1)]
		public void VariableWithInitialization(string expected, string? type, string name, int value) {
			var node = new CodeStack().Begin(new VariableBuilder(type ?? string.Empty, name)).With(new LiteralNode(value)).End().Build();
			expected.EqualsIgnoringLineEndings(node);
		}

		[Theory]
		[InlineData("var test", null, "test")]
		[InlineData("int test", "int", "test")]
		public void VariableDeclarationOnly(string expected, string? type, string name) {
			var node = new CodeStack().Begin(new VariableBuilder(type ?? string.Empty, name)).End().Build();
			expected.EqualsIgnoringLineEndings(node);
		}

		[Fact]
		public void TestArrayVariable() {
			var cs = new CodeStack()
				.Complete(new VariableBuilder(new ArrayTypeNode("string"), "test"))
				.Begin(new AssignmentExpressionBuilder("test"))
					.With(new LiteralNode("a"))
				.End();
			"string[] test\r\ntest = \"a\"".EqualsIgnoringLineEndings(cs.Build());
		}
	}
}