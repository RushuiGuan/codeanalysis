# Testing Your Analyzers

Learn how to create comprehensive unit tests for your Roslyn-based analyzers and source generators using Albatross.CodeAnalysis.Testing.

## Overview

Testing is crucial for building reliable code analyzers and source generators. The `Albatross.CodeAnalysis.Testing` library provides utilities to simplify the creation of unit tests by making it easy to create compilations with proper framework references.

## Getting Started

### Installation

```bash
dotnet add package Albatross.CodeAnalysis.Testing
```

### Basic Setup

```csharp
using Albatross.CodeAnalysis.Testing;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Xunit;
```

## Creating Test Compilations

### Basic Compilation Creation

The core feature is the `CreateNet8CompilationAsync()` extension method:

```csharp
[Fact]
public async Task TestBasicCompilation() {
    const string code = @"
public class TestClass {
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}";

    var compilation = await code.CreateNet8CompilationAsync();
    
    // Now you can analyze symbols
    var testClass = compilation.GetRequiredSymbol("TestClass");
    testClass.Should().NotBeNull();
    testClass.Name.Should().Be("TestClass");
}
```

### Advanced Compilation Options

```csharp
[Fact]
public async Task TestCompilationWithOptions() {
    const string code = @"
public class TestClass {
    public string? NullableProperty { get; set; }
}";

    var compilation = await code.CreateNet8CompilationAsync(
        assemblyName: "MyTestAssembly",
        cancellationToken: CancellationToken.None
    );
    
    // The compilation includes proper .NET 8.0 framework references
    // and supports C# 12 language features by default
}
```

## Testing Symbol Analysis

### Nullability Testing

```csharp
public class NullabilityTests {
    const string TestCode = @"
public class TestClass {
    public int Value { get; set; }
    public int? NullableValue { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? NullableText { get; set; }
    public int[] Array { get; set; } = Array.Empty<int>();
    public int[]? NullableArray { get; set; }
}";

    [Theory]
    [InlineData("Value", false)]
    [InlineData("NullableValue", true)]
    [InlineData("Text", false)]
    [InlineData("NullableText", true)]
    [InlineData("Array", false)]
    [InlineData("NullableArray", true)]
    public async Task VerifyIsNullable(string propertyName, bool expectedNullable) {
        var compilation = await TestCode.CreateNet8CompilationAsync();
        var symbol = compilation.GetRequiredSymbol("TestClass");
        var property = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .First(x => x.Name == propertyName);
            
        var actualNullable = property.Type.IsNullable(compilation);
        actualNullable.Should().Be(expectedNullable);
    }
}
```

### Collection Type Testing

```csharp
public class CollectionTests {
    [Theory]
    [InlineData("List<string>", true, "String")]
    [InlineData("string[]", true, "String")]
    [InlineData("IEnumerable<int>", true, "Int32")]
    [InlineData("Dictionary<string, int>", true, "Int32")] // Value type for dictionaries
    [InlineData("string", false, null)] // String is not considered a collection
    [InlineData("int", false, null)]
    public async Task TestCollectionDetection(string typeExpression, bool expectedIsCollection, string? expectedElementType) {
        var code = $@"
using System;
using System.Collections.Generic;

public class TestClass {{
    public {typeExpression} Property {{ get; set; }}
}}";

        var compilation = await code.CreateNet8CompilationAsync();
        var testClass = compilation.GetRequiredSymbol("TestClass");
        var property = testClass.GetMembers("Property").OfType<IPropertySymbol>().First();
        var type = property.Type;
        
        var isCollection = type.IsCollection(compilation);
        isCollection.Should().Be(expectedIsCollection);
        
        if (expectedIsCollection && expectedElementType != null) {
            var hasElement = type.TryGetCollectionElementType(compilation, out var elementType);
            hasElement.Should().BeTrue();
            elementType!.Name.Should().Be(expectedElementType);
        }
    }
}
```

### Attribute Testing

```csharp
public class AttributeTests {
    [Fact]
    public async Task TestAttributeDetection() {
        const string code = @"
using System.ComponentModel.DataAnnotations;

public class User {
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Range(18, 120)]
    public int Age { get; set; }
}";

        var compilation = await code.CreateNet8CompilationAsync();
        var userClass = compilation.GetRequiredSymbol("User");
        var nameProperty = userClass.GetMembers("Name").OfType<IPropertySymbol>().First();
        
        // Test Required attribute
        var requiredAttr = compilation.GetRequiredSymbol("System.ComponentModel.DataAnnotations.RequiredAttribute");
        nameProperty.HasAttribute(requiredAttr).Should().BeTrue();
        
        // Test StringLength attribute details
        var stringLengthAttr = compilation.GetRequiredSymbol("System.ComponentModel.DataAnnotations.StringLengthAttribute");
        nameProperty.TryGetAttribute(stringLengthAttr, out var attrData).Should().BeTrue();
        
        // Verify constructor argument
        attrData!.ConstructorArguments[0].Value.Should().Be(50);
        
        // Verify named argument
        attrData.TryGetNamedArgument("MinimumLength", out var minLength).Should().BeTrue();
        minLength.Value.Should().Be(2);
    }
}
```

## Testing Source Generators

### Generator Testing Setup

```csharp
public class SourceGeneratorTests {
    [Fact]
    public async Task TestSourceGenerator() {
        const string inputCode = @"
using System;

[GenerateToString]
public partial class TestClass {
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}";

        var compilation = await inputCode.CreateNet8CompilationAsync();
        
        // Create and run your generator
        var generator = new MySourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        
        var runResult = driver.RunGenerators(compilation).GetRunResult();
        
        // Assert no diagnostics
        runResult.Diagnostics.Should().BeEmpty();
        
        // Assert generated sources
        runResult.Results.Should().HaveCount(1);
        var generatorResult = runResult.Results[0];
        generatorResult.GeneratedSources.Should().HaveCount(1);
        
        var generatedSource = generatorResult.GeneratedSources[0];
        generatedSource.SourceText.ToString().Should().Contain("public override string ToString()");
    }
}
```

### Testing Generator with Multiple Files

```csharp
[Fact]
public async Task TestGeneratorWithMultipleFiles() {
    var sources = new Dictionary<string, string> {
        ["Class1.cs"] = @"
[GenerateSerializer]
public partial class Class1 {
    public string Name { get; set; } = string.Empty;
}",
        ["Class2.cs"] = @"
[GenerateSerializer]  
public partial class Class2 {
    public int Value { get; set; }
}"
    };
    
    var compilation = await sources.CreateNet8CompilationAsync();
    
    var generator = new SerializationGenerator();
    var driver = CSharpGeneratorDriver.Create(generator);
    var runResult = driver.RunGenerators(compilation).GetRunResult();
    
    // Should generate one file per input class
    runResult.Results[0].GeneratedSources.Should().HaveCount(2);
}
```

## Advanced Testing Scenarios

### Testing with Custom References

```csharp
[Fact]
public async Task TestWithCustomReferences() {
    const string code = @"
using Newtonsoft.Json;

public class TestClass {
    [JsonProperty(""custom_name"")]
    public string Name { get; set; } = string.Empty;
}";

    // Add custom references beyond the default .NET 8.0 references
    var references = new[] {
        MetadataReference.CreateFromFile(typeof(Newtonsoft.Json.JsonPropertyAttribute).Assembly.Location)
    };
    
    var compilation = await code.CreateNet8CompilationAsync();
    compilation = compilation.AddReferences(references);
    
    var testClass = compilation.GetRequiredSymbol("TestClass");
    var nameProperty = testClass.GetMembers("Name").OfType<IPropertySymbol>().First();
    
    var jsonPropertyAttr = compilation.GetTypeByMetadataName("Newtonsoft.Json.JsonPropertyAttribute");
    nameProperty.HasAttribute(jsonPropertyAttr).Should().BeTrue();
}
```

### Testing Compilation Errors

```csharp
[Fact]
public async Task TestCompilationWithErrors() {
    const string invalidCode = @"
public class TestClass {
    // This will cause a compilation error
    public UndefinedType Property { get; set; }
}";

    var compilation = await invalidCode.CreateNet8CompilationAsync();
    var diagnostics = compilation.GetDiagnostics();
    
    diagnostics.Should().NotBeEmpty();
    diagnostics.Should().Contain(d => d.Severity == DiagnosticSeverity.Error);
}
```

### Testing with Different Target Frameworks

While the testing library defaults to .NET 8.0, you can test compatibility:

```csharp
[Fact]
public async Task TestNetStandard20Compatibility() {
    const string code = @"
using System;
using System.Collections.Generic;

public class TestClass {
    public List<string> Items { get; set; } = new List<string>();
}";

    var compilation = await code.CreateNet8CompilationAsync();
    
    // Test that your analyzer works with the compilation
    var analyzer = new MyAnalyzer();
    // ... run analyzer logic
    
    // Verify compatibility with .NET Standard 2.0 concepts
    var testClass = compilation.GetRequiredSymbol("TestClass");
    testClass.Should().NotBeNull();
}
```

## Test Organization

### Base Test Class Pattern

```csharp
public abstract class AnalyzerTestBase {
    protected async Task<Compilation> CreateTestCompilationAsync(string code) {
        return await code.CreateNet8CompilationAsync();
    }
    
    protected INamedTypeSymbol GetRequiredType(Compilation compilation, string typeName) {
        return compilation.GetRequiredSymbol(typeName);
    }
    
    protected IPropertySymbol GetRequiredProperty(INamedTypeSymbol type, string propertyName) {
        return type.GetMembers(propertyName).OfType<IPropertySymbol>().First();
    }
}

public class MyAnalyzerTests : AnalyzerTestBase {
    [Fact]
    public async Task TestMyAnalyzer() {
        const string code = "public class Test { public string Name { get; set; } }";
        var compilation = await CreateTestCompilationAsync(code);
        
        var testClass = GetRequiredType(compilation, "Test");
        var nameProperty = GetRequiredProperty(testClass, "Name");
        
        // Test your analyzer logic...
    }
}
```

### Parameterized Test Patterns

```csharp
public static class TestCases {
    public static IEnumerable<object[]> NullabilityTestCases() {
        yield return new object[] { "string", false };
        yield return new object[] { "string?", true };
        yield return new object[] { "int", false };
        yield return new object[] { "int?", true };
        yield return new object[] { "List<string>", false };
        yield return new object[] { "List<string>?", true };
    }
}

public class ParameterizedTests {
    [Theory]
    [MemberData(nameof(TestCases.NullabilityTestCases), MemberType = typeof(TestCases))]
    public async Task TestNullabilityDetection(string typeExpression, bool expectedNullable) {
        var code = $@"
using System.Collections.Generic;
public class TestClass {{ public {typeExpression} Property {{ get; set; }} }}";

        var compilation = await code.CreateNet8CompilationAsync();
        var testClass = compilation.GetRequiredSymbol("TestClass");
        var property = testClass.GetMembers("Property").OfType<IPropertySymbol>().First();
        
        property.Type.IsNullable(compilation).Should().Be(expectedNullable);
    }
}
```

## Best Practices

### 1. Test Organization

```csharp
// ✅ Good: Organize tests by feature
public class NullabilityAnalysisTests { }
public class CollectionAnalysisTests { }
public class AttributeInspectionTests { }

// ✅ Good: Use descriptive test names
[Fact]
public async Task IsNullable_WithNullableReferenceType_ReturnsTrue() { }

[Fact]  
public async Task TryGetCollectionElementType_WithListOfString_ReturnsStringType() { }
```

### 2. Test Data Management

```csharp
// ✅ Good: Use constants for reusable test code
public static class TestCode {
    public const string SimpleClass = @"
public class TestClass {
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}";

    public const string GenericClass = @"
public class GenericClass<T> {
    public T Value { get; set; }
    public List<T> Items { get; set; } = new();
}";
}
```

### 3. Assertion Patterns

```csharp
// ✅ Good: Use FluentAssertions for readable tests
compilation.GetDiagnostics()
    .Where(d => d.Severity == DiagnosticSeverity.Error)
    .Should()
    .BeEmpty("compilation should not have errors");

property.Type.IsNullable(compilation)
    .Should()
    .BeTrue($"{property.Name} should be nullable");
```

### 4. Error Handling in Tests

```csharp
// ✅ Good: Test error conditions explicitly
[Fact]
public async Task GetRequiredSymbol_WithMissingType_ThrowsException() {
    var compilation = await "public class Test { }".CreateNet8CompilationAsync();
    
    var act = () => compilation.GetRequiredSymbol("NonExistentType");
    act.Should().Throw<InvalidOperationException>();
}
```

## Integration with CI/CD

### Test Configuration

```xml
<!-- In your test project -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Albatross.CodeAnalysis.Testing" Version="8.0.1" />
  </ItemGroup>
</Project>
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "ClassName=NullabilityTests"

# Generate code coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Prerequisites

- .NET 8.0 SDK or later
- Understanding of Roslyn APIs and symbol analysis
- Familiarity with unit testing frameworks (xUnit, MSTest, NUnit)

## Related Topics

- [Symbol Analysis Guide](symbol-analysis.md) - What to test in symbol analysis
- [Nullability Detection](nullability.md) - Testing nullability scenarios  
- [Collection Types](collections.md) - Testing collection analysis
- [Attribute Inspection](attributes.md) - Testing attribute analysis
- [API Reference](../api/Albatross.CodeAnalysis.Testing.yml) - Testing library API documentation