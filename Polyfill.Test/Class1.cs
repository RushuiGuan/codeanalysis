using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Polyfill.Test {

	public class Class1 {
		public required int A { get; set; }
		public int B { get; init; }

		public bool TryGetData([NotNullWhen(true)] out string? data) {
			data = "Hello";
			return true;
		}

		public void Invoke(string text, [CallerArgumentExpression("text")] string? expression = null) {
			Console.WriteLine($"Text: {text}, Expression: {expression}");
		}
	}

	public record class Record1 {
		public required int X { get; init; }
		public int Y { get; init; }
	}
}