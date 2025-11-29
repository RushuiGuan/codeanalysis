using Albatross.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Albatross.CodeAnalysis.Syntax {
	public static class Extensions {
		public static TypeNode AsTypeNode(this ITypeSymbol symbol) {
			if (symbol is IArrayTypeSymbol arrayType) {
				return new ArrayTypeNode(arrayType.ElementType.AsTypeNode());
			} else {
				var node = new TypeNode(symbol.GetFullName());
				if (symbol.IsNullableReferenceType()) {
					return node.NullableReferenceType();
				} else {
					return node;
				}
			}
		}
		public static BlockSyntax BlockSyntax(this IEnumerable<SyntaxNode> nodes)
			=> SyntaxFactory.Block(nodes.Select(x => new StatementNode(x).StatementSyntax));

		public static string GenerateCode(this CodeStack stack) {
			var sb = new StringBuilder();
			foreach (var node in stack.Finalize()) {
				if (node is INodeContainer container) {
					var text = container.Node.NormalizeWhitespace();
					sb.AppendLine(text.ToFullString());
				} else {
					throw new InvalidOperationException($"Stack item of type {node.GetType().Name} is not expected.  Only {typeof(INodeContainer).Name} is expected");
				}
			}
			return sb.ToString();
		}
	}
}