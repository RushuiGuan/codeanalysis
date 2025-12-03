namespace System.Diagnostics.CodeAnalysis {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
	public sealed class AllowNullAttribute : Attribute {
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
	public sealed class DisallowNullAttribute : Attribute {
	}

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
	public sealed class MaybeNullAttribute : Attribute {
	}

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
	public sealed class NotNullAttribute : Attribute {
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class NotNullIfNotNullAttribute : Attribute {
		public NotNullIfNotNullAttribute(string parameterName) { }
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class NotNullWhenAttribute : Attribute {
		public NotNullWhenAttribute(bool returnValue) { }
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class MemberNotNullAttribute : Attribute {
		public MemberNotNullAttribute(params string[] members) { }
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class MemberNotNullWhenAttribute : Attribute {
		public MemberNotNullWhenAttribute(bool returnValue, params string[] members) { }
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class DoesNotReturnAttribute : Attribute {
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	
	public sealed class DoesNotReturnIfAttribute : Attribute {
		public DoesNotReturnIfAttribute(bool parameterValue) { }
	}
}