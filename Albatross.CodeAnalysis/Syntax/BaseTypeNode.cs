using Microsoft.CodeAnalysis.CSharp;
using System;

namespace Albatross.CodeAnalysis.Syntax {
	[Obsolete]
	public class BaseTypeNode : NodeContainer {
		public BaseTypeNode(string name)
			: base(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(name))) {
		}
	}
}