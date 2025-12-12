#if NETSTANDARD2_0
using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
	/// <summary>
	/// Polyfill marker class that enables C# 9 init-only property setters in .NET Standard 2.0.
	/// This allows the use of the init accessor for properties.
	/// </summary>
	public static class IsExternalInit { }
}
#endif