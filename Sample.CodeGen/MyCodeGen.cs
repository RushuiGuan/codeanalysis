using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Sample.CodeGen {
	[Generator]
	public class MyCodeGen : IIncrementalGenerator {
		public void Initialize(IncrementalGeneratorInitializationContext context) {
			context.RegisterSourceOutput(
				context.CompilationProvider,
				(ctx, compilation) => {
					ctx.AddSource("MyTest", SourceText.From("public class Test { }", Encoding.UTF8));
				}
			);
		}
	}
}
