using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Albatross.CodeAnalysis.Syntax {
	[Obsolete]
	public class LambdaExpressionBuilder : INodeBuilder {
		public SyntaxNode Build(IEnumerable<SyntaxNode> elements) {
			var parameters = new List<ParameterSyntax>();
			foreach (var element in elements) {
				if (element is ParameterSyntax parameterSyntax) {
					parameters.Add(parameterSyntax);
				}else if (element == elements.Last()) {
					// The last element is the body
					if (element is ExpressionSyntax expressionSyntax) {
						return SyntaxFactory.ParenthesizedLambdaExpression(
							SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)),
							expressionSyntax
						);
					} else if (element is BlockSyntax blockSyntax) {
						return SyntaxFactory.ParenthesizedLambdaExpression(
							SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)),
							blockSyntax
						);
					} else {
						throw new ArgumentException($"Invalid body type {element.GetType().Name} for lambda expression");
					}
				} else {
					throw new ArgumentException($"Invalid element type {element.GetType().Name} in lambda expression");
				}
			}
			throw new ArgumentException("Lambda expression requires a body element");
		}
	}
}
