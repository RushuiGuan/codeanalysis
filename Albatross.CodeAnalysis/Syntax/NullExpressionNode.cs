using Microsoft.CodeAnalysis.CSharp;
using System;

namespace Albatross.CodeAnalysis.Syntax {
	[Obsolete]
	public class NullExpressionNode : NodeContainer {
		public NullExpressionNode() : base(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)) {
		}
	}
}