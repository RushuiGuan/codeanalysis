#if NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis {
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class NotNullWhenAttribute : Attribute {
        public NotNullWhenAttribute(bool returnValue) { }
    }
}
#endif