#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
	/// <summary>
	/// Polyfill attribute that allows capturing the expression passed to a parameter as a string.
	/// This enables better diagnostic messages and argument validation in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class CallerArgumentExpressionAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CallerArgumentExpressionAttribute"/> class.
		/// </summary>
		/// <param name="parameterName">The name of the parameter whose expression should be captured.</param>
		public CallerArgumentExpressionAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		/// <summary>
		/// Gets the name of the parameter whose expression is captured.
		/// </summary>
		public string ParameterName { get; }
	}
}
#endif