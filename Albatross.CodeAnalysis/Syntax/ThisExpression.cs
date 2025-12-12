using Microsoft.CodeAnalysis.CSharp;
using System;

namespace Albatross.CodeAnalysis.Syntax {
	[Obsolete]
	public class ThisExpression : NodeContainer {
		public ThisExpression() : base(SyntaxFactory.ThisExpression()) { }
	}
}