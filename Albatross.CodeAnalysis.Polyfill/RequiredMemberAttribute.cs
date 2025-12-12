#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices {
	/// <summary>
	/// Polyfill attribute that indicates a feature required by the compiler. 
	/// Enables C# 11 required members feature in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	public sealed class CompilerFeatureRequiredAttribute : Attribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="CompilerFeatureRequiredAttribute"/> class.
		/// </summary>
		/// <param name="featureName">The name of the required compiler feature.</param>
		public CompilerFeatureRequiredAttribute(string featureName) {
			FeatureName = featureName;
		}

		/// <summary>
		/// Gets the name of the required compiler feature.
		/// </summary>
		public string FeatureName { get; }
		
		/// <summary>
		/// Gets or sets a value indicating whether the feature is optional.
		/// </summary>
		public bool IsOptional { get; init; }
	}

	/// <summary>
	/// Polyfill attribute that indicates a type has required members or that a member is required.
	/// Enables C# 11 required members feature in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class RequiredMemberAttribute : Attribute {
	}

	/// <summary>
	/// Polyfill attribute that indicates a constructor sets all required members for the containing type.
	/// Enables C# 11 required members feature in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
	public sealed class SetsRequiredMembersAttribute : Attribute {
	}
}
namespace System.Diagnostics.CodeAnalysis {
	/// <summary>
	/// Polyfill attribute that indicates a constructor sets all required members for the containing type.
	/// Enables C# 11 required members feature in .NET Standard 2.0.
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor, Inherited = false)]
	public sealed class SetsRequiredMembersAttribute : Attribute {
	}
}
#endif