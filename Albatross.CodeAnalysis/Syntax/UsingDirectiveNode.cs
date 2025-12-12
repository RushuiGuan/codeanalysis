using Microsoft.CodeAnalysis.CSharp;
using System;

namespace Albatross.CodeAnalysis.Syntax {
	[Obsolete]
	public class UsingDirectiveNode : NodeContainer {
		public UsingDirectiveNode(string name)
			: base(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(name))) {
		}
	}
}