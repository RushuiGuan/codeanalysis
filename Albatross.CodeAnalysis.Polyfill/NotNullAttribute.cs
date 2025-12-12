#if NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis {
	/// <summary>
	/// Polyfill attribute that specifies that null is allowed as an input even if the corresponding type disallows it.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
	public sealed class AllowNullAttribute : Attribute { }

	/// <summary>
	/// Polyfill attribute that specifies that null is disallowed as an input even if the corresponding type allows it.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    public sealed class DisallowNullAttribute : Attribute { }

	/// <summary>
	/// Polyfill attribute that specifies that an output may be null even if the corresponding type disallows it.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class MaybeNullAttribute : Attribute { }

	/// <summary>
	/// Polyfill attribute that specifies that an output is not null even if the corresponding type allows it.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class NotNullAttribute : Attribute { }

	/// <summary>
	/// Polyfill attribute that specifies that when a method returns the specified return value, the parameter may be null even if the corresponding type disallows it.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class MaybeNullWhenAttribute : Attribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="MaybeNullWhenAttribute"/> class.
		/// </summary>
		/// <param name="returnValue">The return value condition. If true, the parameter may be null when the method returns true; otherwise, when the method returns false.</param>
		public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
		
		/// <summary>
		/// Gets the return value condition.
		/// </summary>
		public bool ReturnValue { get; }
	}

	/// <summary>
	/// Polyfill attribute that specifies that when a method returns the specified return value, the parameter is not null even if the corresponding type allows it.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class NotNullWhenAttribute : Attribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="NotNullWhenAttribute"/> class.
		/// </summary>
		/// <param name="returnValue">The return value condition. If true, the parameter is not null when the method returns true; otherwise, when the method returns false.</param>
		public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
		
		/// <summary>
		/// Gets the return value condition.
		/// </summary>
		public bool ReturnValue { get; }
	}

	/// <summary>
	/// Polyfill attribute that specifies that the output is not null if the named parameter is not null.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    public sealed class NotNullIfNotNullAttribute : Attribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="NotNullIfNotNullAttribute"/> class.
		/// </summary>
		/// <param name="parameterName">The name of the parameter that is related to the nullability of the output.</param>
		public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;
		
		/// <summary>
		/// Gets the parameter name that is related to the nullability of the output.
		/// </summary>
		public string ParameterName { get; }
	}

	/// <summary>
	/// Polyfill attribute that indicates a method does not return to the caller. Used for methods that always throw exceptions.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DoesNotReturnAttribute : Attribute { }

	/// <summary>
	/// Polyfill attribute that indicates the method does not return if the associated parameter has the specified value.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class DoesNotReturnIfAttribute : Attribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="DoesNotReturnIfAttribute"/> class.
		/// </summary>
		/// <param name="parameterValue">The parameter value that causes the method not to return.</param>
		public DoesNotReturnIfAttribute(bool parameterValue) => ParameterValue = parameterValue;
		
		/// <summary>
		/// Gets the parameter value that causes the method not to return.
		/// </summary>
		public bool ParameterValue { get; }
	}

	/// <summary>
	/// Polyfill attribute that specifies that the method or property ensures that the listed members are not null.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class MemberNotNullAttribute : Attribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="MemberNotNullAttribute"/> class with a single member.
		/// </summary>
		/// <param name="member">The name of the member that is guaranteed to be non-null.</param>
		public MemberNotNullAttribute(string member) => Members = new[] { member };
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MemberNotNullAttribute"/> class with multiple members.
		/// </summary>
		/// <param name="members">The names of the members that are guaranteed to be non-null.</param>
		public MemberNotNullAttribute(params string[] members) => Members = members;
		
		/// <summary>
		/// Gets the names of the members that are guaranteed to be non-null.
		/// </summary>
		public string[] Members { get; }
	}

	/// <summary>
	/// Polyfill attribute that specifies that the method or property ensures that the listed members are not null when returning the specified value.
	/// Enables nullable reference type annotations in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class MemberNotNullWhenAttribute : Attribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="MemberNotNullWhenAttribute"/> class with a single member.
		/// </summary>
		/// <param name="returnValue">The return value condition.</param>
		/// <param name="member">The name of the member that is guaranteed to be non-null.</param>
		public MemberNotNullWhenAttribute(bool returnValue, string member) {
			ReturnValue = returnValue;
			Members = new[] { member };
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MemberNotNullWhenAttribute"/> class with multiple members.
		/// </summary>
		/// <param name="returnValue">The return value condition.</param>
		/// <param name="members">The names of the members that are guaranteed to be non-null.</param>
		public MemberNotNullWhenAttribute(bool returnValue, params string[] members) {
			ReturnValue = returnValue;
			Members = members;
		}
		
		/// <summary>
		/// Gets the return value condition.
		/// </summary>
		public bool ReturnValue { get; }
		
		/// <summary>
		/// Gets the names of the members that are guaranteed to be non-null.
		/// </summary>
		public string[] Members { get; }
	}
}
#endif