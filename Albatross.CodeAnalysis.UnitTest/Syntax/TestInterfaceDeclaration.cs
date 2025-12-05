using Albatross.CodeAnalysis.Syntax;
using Albatross.Testing;
using Xunit;

namespace Albatross.CodeAnalysis.UnitTest.Syntax {
	public class TestInterfaceDeclaration {
		const string InterfaceDeclaration_Expected = @"public interface ITest
{
	void test();
}
";
		[Fact]
		public void MethodInvocation() {
			var result = new CodeStack().Begin(new InterfaceDeclarationBuilder("ITest").Public())
				.Begin(new MethodDeclarationBuilder("void", "test").SignatureOnly()).End()
				.End().Build();
			InterfaceDeclaration_Expected.EqualsIgnoringLineEndings(result);
		}
	}
}
