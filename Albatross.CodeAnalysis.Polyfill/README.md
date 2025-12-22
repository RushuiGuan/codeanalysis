# Albatross.CodeAnalysis.Polyfill

A polyfill library that provides modern C# language features and attributes for .NET Standard 2.0 projects. This allows you to use newer C# language features while maintaining compatibility with older frameworks.

## Features

- **CallerArgumentExpressionAttribute**: Enables capturing argument expressions as strings (C# 10 feature)
- **RequiredMemberAttribute**: Supports required members syntax (C# 11 feature)
- **IsExternalInit**: Enables init-only setters for properties (C# 9 feature)
- **NotNullAttribute**: Indicates that a method will never return null
- **HashCode**: Provides a type for combining hash codes (available in newer frameworks)
- **Conditional Compilation**: Only included when targeting .NET Standard 2.0

## Example Usage

### Using in a Multi-Targeted Project

Always use conditional references to include this package only when targeting .NET Standard 2.0. This ensures that the polyfills are only used when necessary, and the native implementations are used on newer frameworks.

**In your `.csproj` file:**

```xml
<PropertyGroup>
  <!-- Multi-target to ensure correctness -->
  <TargetFrameworks>netstandard2.0;net8.0</TargetFrameworks>
</PropertyGroup>

<ItemGroup>
  <ProjectReference
    Condition="'$(TargetFramework)' == 'netstandard2.0'"
    Include="..\Albatross.CodeAnalysis.Polyfill\Albatross.CodeAnalysis.Polyfill.csproj" 
    PrivateAssets="All" />
</ItemGroup>
```

### Using CallerArgumentExpression

```csharp
using System.Runtime.CompilerServices;

public static class Guard {
    public static void NotNull<T>(
        T value, 
        [CallerArgumentExpression("value")] string? paramName = null) {
        
        if (value == null) {
            throw new ArgumentNullException(paramName);
        }
    }
}

// Usage
string name = null;
Guard.NotNull(name); // Throws: ArgumentNullException: name
```

### Using Init-Only Properties

```csharp
public class Person {
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

// Usage
var person = new Person { 
    FirstName = "John", 
    LastName = "Doe" 
};
// person.FirstName = "Jane"; // Compile error - init-only
```

### Using Required Members

```csharp
public class Configuration {
    [Required]
    public required string ApiKey { get; set; }
    
    public string? OptionalValue { get; set; }
}

// Usage
var config = new Configuration { 
    ApiKey = "abc123" // Required - compile error if omitted
};
```

## Best Practices

1. **Always Multi-Target**: Include both `netstandard2.0` and a modern target like `net8.0` in your project. The modern compiler will verify that your polyfilled code is correct and compatible.

2. **Use Conditional References**: Only reference this package when targeting .NET Standard 2.0 using the `Condition` attribute on `ProjectReference` or `PackageReference`.

3. **Set PrivateAssets="All"**: This prevents the polyfill types from being exposed to consumers of your library.

4. **Test on Multiple Targets**: Always test your code on both .NET Standard 2.0 and modern frameworks to ensure compatibility.

## Installation

This package is available on NuGet:

```bash
dotnet add package Albatross.CodeAnalysis.Polyfill
```

Or add it conditionally to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference 
    Include="Albatross.CodeAnalysis.Polyfill" 
    Version="8.0.1"
    Condition="'$(TargetFramework)' == 'netstandard2.0'"
    PrivateAssets="All" />
</ItemGroup>
```

## Dependencies

This package has no external dependencies and targets .NET Standard 2.0.

## License

See the [LICENSE](../LICENSE) file in the repository root.