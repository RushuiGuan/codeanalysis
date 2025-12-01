using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Albatross.CodeAnalysis.Syntax {
	public class ParameterNode : NodeContainer {
		public ParameterNode(TypeNode? typeNode, string name) : base(Build(typeNode, name)) { }
		public ParameterNode(string name) : this(null, name) { }
		//public ParameterNode(string type, string name) : this(new TypeNode(type), name) { }

		public ParameterNode WithThis() {
			this.Node = this.Parameter.AddModifiers(SyntaxFactory.Token(SyntaxKind.ThisKeyword));
			return this;
		}

		static SyntaxNode Build(TypeNode? typeNode, string name) {
			var node = SyntaxFactory.Parameter(SyntaxFactory.Identifier(name));
			if (typeNode != null) {
				node = node.WithType(typeNode.Type);
			}
			return node;
		}
		public ParameterSyntax Parameter => (ParameterSyntax)Node;
	}
}