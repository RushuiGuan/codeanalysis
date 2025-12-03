namespace System.Runtime.CompilerServices {
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class CallerArgumentExpressionAttribute : Attribute {
		public CallerArgumentExpressionAttribute(string parameterName) { }
	}
}