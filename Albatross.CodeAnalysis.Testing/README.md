# Albatross.CodeAnalysis.Testing

A testing utility library designed to simplify the creation of unit tests for Roslyn-based code analyzers, code generators, and source generators. This library provides convenient extension methods to create compilations with proper framework references for testing purposes.

## Features

- **Easy Compilation Creation**: Create `CSharpCompilation` instances with proper .NET 8.0 framework references
- **Modern C# Support**: Supports C# 12 language features by default
- **Async/Await Support**: Fully async API for compilation creation
- **Proper Reference Assemblies**: Automatically resolves and includes .NET 8.0 reference assemblies
- **Customizable**: Allows custom assembly names and cancellation support
- **Testing-First Design**: Built specifically for testing scenarios with Roslyn

## Example Usage

### Creating a Test Compilation

```csharp
using Albatross.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Xunit;
using FluentAssertions;

public class MyAnalyzerTests {
    [Fact]
    public async Task TestNullableProperty() {
        const string code = @"
            public class TestClass {
                public string? NullableText { get; set; }
                public string NonNullableText { get; set; } = string.Empty;
            }
        ";
        
        // Create a compilation from the source code
        var compilation = await code.CreateNet8CompilationAsync();
        
        // Get symbols and perform assertions
        var symbol = compilation.GetTypeByMetadataName("TestClass");
        symbol.Should().NotBeNull();
        
        var properties = symbol.GetMembers().OfType<IPropertySymbol>();
        var nullableProperty = properties.First(p => p.Name == "NullableText");
        
        // Assert nullability
        nullableProperty.Type.NullableAnnotation
            .Should().Be(NullableAnnotation.Annotated);
    }
}
```

### Testing Symbol Analysis

```csharp
using Albatross.CodeAnalysis.Testing;
using Albatross.CodeAnalysis.Symbols;
using Xunit;

public class SymbolExtensionTests {
    [Theory]
    [InlineData("int?", true)]
    [InlineData("string?", true)]
    [InlineData("int", false)]
    [InlineData("string", false)]
    public async Task TestIsNullable(string typeDeclaration, bool expectedNullable) {
        string code = $@"
            public class TestClass {{
                public {typeDeclaration} TestProperty {{ get; set; }}
            }}
        ";
        
        var compilation = await code.CreateNet8CompilationAsync();
        var classSymbol = compilation.GetTypeByMetadataName("TestClass");
        var property = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .First();
        
        bool isNullable = property.Type.IsNullable(compilation);
        Assert.Equal(expectedNullable, isNullable);
    }
}
```

### Custom Assembly Name and Cancellation

```csharp
using Albatross.CodeAnalysis.Testing;
using System.Threading;

public class CustomCompilationTests {
    [Fact]
    public async Task CreateCustomCompilation() {
        const string code = "public class MyClass { }";
        
        var cts = new CancellationTokenSource();
        
        var compilation = await code.CreateNet8CompilationAsync(
            assemblyName: "MyCustomAssembly",
            cancellationToken: cts.Token
        );
        
        Assert.Equal("MyCustomAssembly", compilation.AssemblyName);
    }
}
```

### Testing Code Generators

```csharp
using Albatross.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public class CodeGeneratorTests {
    [Fact]
    public async Task TestGeneratedCode() {
        const string sourceCode = @"
            [GenerateExtension]
            public partial class MyClass { }
        ";
        
        // Create compilation
        var compilation = await sourceCode.CreateNet8CompilationAsync();
        
        // Run your generator
        var generator = new MySourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(
            compilation, 
            out var outputCompilation, 
            out var diagnostics
        );
        
        // Assert no errors
        diagnostics.Should().BeEmpty();
        
        // Verify generated code
        var generatedTrees = outputCompilation.SyntaxTrees
            .Where(t => !compilation.SyntaxTrees.Contains(t));
        generatedTrees.Should().NotBeEmpty();
    }
}
```

## Project Structure

```
Albatross.CodeAnalysis.Testing/
├── Extensions.cs              # Main extension methods for creating test compilations
└── Albatross.CodeAnalysis.Testing.csproj
```

## How to Run Unit Tests

This library is used by test projects throughout the repository, particularly in `Albatross.CodeAnalysis.UnitTest`.

### Prerequisites
- .NET 8.0 SDK or later

### Running Tests

From the repository root:

```bash
# Run all tests (which use this library)
dotnet test

# Run specific test project that uses this library
dotnet test Albatross.CodeAnalysis.UnitTest/Albatross.CodeAnalysis.UnitTest.csproj

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Example Test Projects Using This Library

- **Albatross.CodeAnalysis.UnitTest**: Tests for the main code analysis library
- **Polyfill.Test**: Tests for the polyfill library

## API Reference

### CreateNet8CompilationAsync Extension Method

```csharp
public static async Task<CSharpCompilation> CreateNet8CompilationAsync(
    this string source,
    string assemblyName = "Net8TestAssembly",
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `source`: The C# source code to compile
- `assemblyName`: The name of the assembly (default: "Net8TestAssembly")
- `cancellationToken`: Optional cancellation token

**Returns:** A `CSharpCompilation` configured with .NET 8.0 references and C# 12 language version.

## Installation

This package is available on NuGet:

```bash
dotnet add package Albatross.CodeAnalysis.Testing
```

Or add it to your test project's `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Albatross.CodeAnalysis.Testing" Version="8.0.1" />
</ItemGroup>
```

## Dependencies

- Microsoft.CodeAnalysis.Analyzer.Testing (1.1.2)
- Microsoft.CodeAnalysis.Common (5.0.0)
- Microsoft.CodeAnalysis.CSharp (5.0.0)
- Microsoft.CodeAnalysis.Workspaces.Common (5.0.0)

## Recommended Testing Packages

This library works great with:
- **xUnit** or **NUnit**: For test framework
- **FluentAssertions**: For expressive assertions
- **Microsoft.CodeAnalysis.CSharp.SourceGenerators.Testing**: For comprehensive source generator testing

## License

See the [LICENSE](../LICENSE) file in the repository root.