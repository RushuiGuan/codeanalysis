using Albatross.CodeAnalysis.Syntax;
using Xunit;

namespace Albatross.CodeAnalysis.Test.Syntax {
	public class TestLamdaExpressionBuilder {
		[Fact]
		public void Test() {
			var codeStack = new CodeStack();
			codeStack.Begin(new LambdaExpressionBuilder())
				.With(new ParameterNode("x"))
				.With(new LiteralNode(1))
				.End();
			var code = codeStack.Build().TrimEnd();
			Assert.Equal("(x) => 1", code);
		}
	}
}
