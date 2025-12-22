# Albatross.CodeAnalysis

A powerful code analysis library that provides symbol analysis utilities and extensions to simplify working with Roslyn-based code generators and analyzers. This library helps you analyze types, symbols, and compilation information when building source generators and code analysis tools.

> **Note**: The `Albatross.CodeAnalysis.Syntax` namespace has been deprecated. Use [Albatross.CodeGen.CSharp](https://www.nuget.org/packages/Albatross.CodeGen.CSharp) for C# code generation.

## 📚 Documentation

📖 **[Complete Documentation](../docfx_project/_site/index.html)** | 📚 **[API Reference](../docfx_project/_site/api/index.html)** | 📝 **[Release Notes](../docfx_project/docs/release-notes.md)**

## ✨ Key Features

- **Symbol Analysis**: Comprehensive type and symbol inspection utilities
- **Nullability Detection**: Smart nullable reference and value type analysis
- **Collection Analysis**: Element type extraction and collection detection
- **Attribute Inspection**: Powerful attribute data analysis tools
- **Type Relationships**: Inheritance, interface, and generic type checking
- **Multi-targeting**: Full .NET Standard 2.0 and .NET 8.0 support

## 🚀 Quick Start

### Installation

```bash
dotnet add package Albatross.CodeAnalysis
```

### Basic Usage

```csharp
using Albatross.CodeAnalysis;
using Microsoft.CodeAnalysis;

// Analyze type nullability
bool isNullable = typeSymbol.IsNullable(compilation);

// Check collection types and get element type
if (typeSymbol.TryGetCollectionElementType(compilation, out var elementType)) {
    Console.WriteLine($"Collection of: {elementType.Name}");
}

// Work with attributes
if (symbol.TryGetAttribute(attributeSymbol, out var attrData)) {
    // Process attribute data
}

// Get common framework types
var stringType = compilation.String();
var dateTimeType = compilation.DateTime();
```

## 📦 Related Packages

- **[Albatross.CodeAnalysis.Polyfill](../Albatross.CodeAnalysis.Polyfill/README.md)** - Modern C# features for .NET Standard 2.0
- **[Albatross.CodeAnalysis.Testing](../Albatross.CodeAnalysis.Testing/README.md)** - Testing utilities for analyzers/generators

## 📖 Learn More

For detailed examples, advanced usage, and comprehensive API documentation:

- **[Symbol Analysis Guide](../docfx_project/_site/articles/symbol-analysis.html)** - Deep dive into type and symbol inspection
- **[Nullability Detection](../docfx_project/_site/articles/nullability.html)** - Working with nullable types
- **[Collection Types](../docfx_project/_site/articles/collections.html)** - Analyzing collections and their elements
- **[Attribute Inspection](../docfx_project/_site/articles/attributes.html)** - Working with attributes on symbols
- **[Testing Your Analyzers](../docfx_project/_site/articles/testing.html)** - Unit testing best practices

## Installation Options

### Basic Installation
```bash
dotnet add package Albatross.CodeAnalysis
```

### Multi-targeting Project Setup
```xml
<ItemGroup>
  <PackageReference Include="Albatross.CodeAnalysis" Version="8.0.1" />
  
  <!-- For .NET Standard 2.0 projects -->
  <PackageReference Include="Albatross.CodeAnalysis.Polyfill" Version="8.0.1" 
                    Condition="'$(TargetFramework)' == 'netstandard2.0'" 
                    PrivateAssets="All" />
                    
  <!-- For testing -->
  <PackageReference Include="Albatross.CodeAnalysis.Testing" Version="8.0.1" 
                    PrivateAssets="All" />
</ItemGroup>
```

## Requirements

- Microsoft.CodeAnalysis.CSharp 4.14.0+
- .NET Standard 2.0 or .NET 8.0+

## License

See the [LICENSE](../LICENSE) file in the repository root.

## Example Usage

### Analyzing Type Nullability

```csharp
using Albatross.CodeAnalysis;
using Microsoft.CodeAnalysis;

// Check if a type is nullable (reference or value type)
bool isNullable = typeSymbol.IsNullable(compilation);

// Check specifically for nullable reference types
bool isNullableRef = typeSymbol.IsNullableReferenceType();

// Check specifically for nullable value types (Nullable<T>)
bool isNullableValue = typeSymbol.IsNullableValueType(compilation);

// Try to get the underlying value type from Nullable<T>
if (typeSymbol.TryGetNullableValueType(compilation, out ITypeSymbol? underlyingType)) {
    Console.WriteLine($"Underlying type: {underlyingType.Name}");
}
```

### Working with Collection Types

```csharp
using Albatross.CodeAnalysis;

// Check if a type is a collection (but not string)
bool isCollection = typeSymbol.IsCollection(compilation);

// Get the element type of a collection
if (typeSymbol.TryGetCollectionElementType(compilation, out ITypeSymbol? elementType)) {
    Console.WriteLine($"Collection element type: {elementType.Name}");
}
```

### Analyzing Generic Types

```csharp
using Albatross.CodeAnalysis;

// Get generic type arguments
if (typeSymbol.TryGetGenericTypeArguments(compilation.IEnumerableGenericDefinition(), 
    out ITypeSymbol[] arguments)) {
    foreach (var arg in arguments) {
        Console.WriteLine($"Type argument: {arg.Name}");
    }
}

// Check if a type is a generic type definition
bool isGenericDef = namedTypeSymbol.IsGenericTypeDefinition();

// Check if a type is constructed from a generic definition
bool isConstructed = typeSymbol.IsConstructedFromDefinition(genericDefinition);
```

### Getting Type Names and Namespaces

```csharp
using Albatross.CodeAnalysis;

// Get fully qualified type name
string fullName = typeSymbol.GetFullName(); // e.g., "System.Collections.Generic.List<string>"

// Get namespace
string namespaceName = namespaceSymbol.GetFullNamespace();

// Get type name relative to a namespace
string relativeName = typeSymbol.GetTypeNameRelativeToNamespace("MyApp.Services");
```

### Checking Type Relationships

```csharp
using Albatross.CodeAnalysis;

// Check if a type derives from a base class
bool isDerived = typeSymbol.IsDerivedFrom(baseTypeSymbol);

// Check if a type implements an interface
bool hasInterface = typeSymbol.HasInterface(interfaceSymbol);

// Check if a type is a concrete class (not abstract, not static, not generic definition)
bool isConcrete = namedTypeSymbol.IsConcreteClass();
```

### Working with Attributes

```csharp
using Albatross.CodeAnalysis;

// Check if a symbol has an attribute
bool hasAttr = symbol.HasAttribute(attributeSymbol);

// Try to get an attribute
if (symbol.TryGetAttribute(attributeSymbol, out AttributeData? attrData)) {
    // Try to get a named argument from the attribute
    if (attrData.TryGetNamedArgument("PropertyName", out TypedConstant value)) {
        Console.WriteLine($"Attribute property value: {value.Value}");
    }
}

// Check if has attribute with specific base type
bool hasAttrWithBase = symbol.HasAttributeWithBaseType(baseAttributeSymbol);
```

### Using Symbol Providers

```csharp
using Albatross.CodeAnalysis;

// Get common framework type symbols
var stringSymbol = compilation.String();
var dateTimeSymbol = compilation.DateTime();
var dateOnlySymbol = compilation.DateOnly();
var enumerableSymbol = compilation.IEnumerable();
var nullableSymbol = compilation.Nullable();

// Compare symbols
bool isString = typeSymbol.Is(compilation.String());
```

### Generator Context Extensions

```csharp
using Albatross.CodeAnalysis;

// Report custom diagnostics during code generation
context.CodeGenDiagnostic(DiagnosticSeverity.Warning, "GEN001", "Custom warning message");

// Create a debug file during code generation
context.CreateGeneratorDebugFile("debug-output.txt", generatedCode);

// Build error message from exception
string errorMsg = exception.BuildCodeGeneneratorErrorMessage("MyGenerator");
```

### Analyzing Properties

```csharp
using Albatross.CodeAnalysis;

// Get public properties (with optional base class properties)
foreach (var property in namedTypeSymbol.GetProperties(useBaseClassProperties: true)) {
    Console.WriteLine($"Property: {property.Name}, Type: {property.Type.Name}");
}

// Get distinct properties (avoiding duplicates from inheritance)
foreach (var property in namedTypeSymbol.GetDistinctProperties(useBaseClassProperties: true)) {
    Console.WriteLine($"Property: {property.Name}");
}
```

## Project Structure

```
Albatross.CodeAnalysis/
├── Extensions.cs                    # GeneratorExecutionContext extensions and utilities
├── AttributeDataExtensions.cs       # Attribute inspection helpers (moved from Symbols/)
├── SymbolProvider.cs               # Access to common framework types (moved from Symbols/)
└── Syntax/                         # ⚠️  DEPRECATED - Syntax builders (marked obsolete)
    └── [Various syntax builders]    # Use Albatross.CodeGen.CSharp instead
```

> **Important**: Symbol extensions have been consolidated into the root namespace. The deprecated `Symbols/` subdirectory structure has been flattened for better API discoverability.

## How to Run Unit Tests

This library includes comprehensive unit tests in the `Albatross.CodeAnalysis.UnitTest` project.

### Prerequisites
- .NET 8.0 SDK or later

### Running Tests

From the repository root:

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test Albatross.CodeAnalysis.UnitTest/Albatross.CodeAnalysis.UnitTest.csproj

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Test Structure

Tests are organized by feature area:
- `Symbols/` - Tests for symbol analysis extensions
- `Syntax/` - Tests for syntax builders and code generation

## Installation

This package is available on NuGet:

```bash
dotnet add package Albatross.CodeAnalysis
```

Or add it directly to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Albatross.CodeAnalysis" Version="8.0.1" />
  
  <!-- Optional: For .NET Standard 2.0 projects -->
  <PackageReference Include="Albatross.CodeAnalysis.Polyfill" Version="8.0.1" 
                    Condition="'$(TargetFramework)' == 'netstandard2.0'" 
                    PrivateAssets="All" />
                    
  <!-- Optional: For testing -->
  <PackageReference Include="Albatross.CodeAnalysis.Testing" Version="8.0.1" 
                    PrivateAssets="All" />
</ItemGroup>
```

## Dependencies

- Microsoft.CodeAnalysis.CSharp (4.14.0+)
- System.Text.Json (for framework compatibility)

## Related Packages

- **[Albatross.CodeAnalysis.Polyfill](../Albatross.CodeAnalysis.Polyfill/README.md)** - Modern C# language features for .NET Standard 2.0
- **[Albatross.CodeAnalysis.Testing](../Albatross.CodeAnalysis.Testing/README.md)** - Testing utilities for code analyzers and generators

For complete package suite documentation, see the **[main documentation site](../docfx_project/_site/index.html)**.

## License

See the [LICENSE](../LICENSE) file in the repository root.