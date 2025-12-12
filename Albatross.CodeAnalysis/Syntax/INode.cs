using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Albatross.CodeAnalysis.Syntax {
	[Obsolete]
	public interface INode { }
	[Obsolete]
	public interface INodeBuilder : INode {
		SyntaxNode Build(IEnumerable<SyntaxNode> elements);
	}
	[Obsolete]
	public interface INodeContainer : INode {
		SyntaxNode Node { get; }
	}
}