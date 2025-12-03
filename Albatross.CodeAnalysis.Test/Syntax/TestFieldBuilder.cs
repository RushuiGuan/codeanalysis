using Albatross.CodeAnalysis.Syntax;
using Albatross.Testing;
using FluentAssertions;
using Xunit;

namespace Albatross.CodeAnalysis.Test.Syntax {
	public class TestFieldBuilder {
		[Fact]
		public void FieldWithoutInitializer() {
			var codestack = new CodeStack()
				.Complete(new FieldDeclarationBuilder("int", "test"));
			codestack.Build().EqualsIgnoringLineEndings("private int test;");
		}

		[Fact]
		public void FieldWithInitializer() {
			var codestack = new CodeStack()
				.Begin(new FieldDeclarationBuilder("int", "test"))
					.With(new LiteralNode(1))
				.End();
			codestack.Build().EqualsIgnoringLineEndings("private int test = 1;");
		}
	}
}