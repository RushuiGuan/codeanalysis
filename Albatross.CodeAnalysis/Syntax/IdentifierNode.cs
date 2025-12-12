using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Albatross.CodeAnalysis.Syntax {
	[Obsolete]
	public class IdentifierNode : NodeContainer {
		public IdentifierNode(string name) : base(SyntaxFactory.IdentifierName(name)) { }
		public IdentifierNameSyntax Identifier => (IdentifierNameSyntax)Node;
	}
}