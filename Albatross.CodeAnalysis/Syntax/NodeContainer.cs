using Microsoft.CodeAnalysis;
using System;

namespace Albatross.CodeAnalysis.Syntax {
	[Obsolete]
	public class NodeContainer : INodeContainer {
		public NodeContainer(SyntaxNode node) {
			Node = node;
		}
		public SyntaxNode Node { get; protected set; }
		public override string ToString() => Node.NormalizeWhitespace(indentation: "\t").ToFullString();
		//public override string ToString() => Node.ToFullString();
	}
}