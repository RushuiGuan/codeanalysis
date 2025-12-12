# Albatross.CodeAnalysis

A powerful code analysis library that provides symbol analysis utilities and extensions to simplify working with Roslyn-based code generators and analyzers. This library helps you analyze types, symbols, and compilation information when building source generators and code analysis tools.

> **Note**: The `Albatross.CodeAnalysis.Syntax` namespace has been deprecated. All classes in that namespace are marked as obsolete and should not be used in new code.

## Features

- **Symbol Analysis Extensions**: Comprehensive helper methods for analyzing symbol information
  - Nullability detection for reference and value types
  - Collection type analysis and element type extraction
  - Generic type argument inspection
  - Type relationship checks (inheritance, interfaces, derived types)
  - Numeric type detection
- **Attribute Inspection**: Extensions for working with attribute data on symbols
- **Symbol Providers**: Convenient access to common framework types (String, DateTime, IEnumerable, etc.)
- **Compilation Utilities**: Extensions for `GeneratorExecutionContext` including diagnostics and debug file generation
- **Type Name Helpers**: Get fully qualified names, relative names, and namespace information
- **Property Analysis**: Extract public properties from types with optional base class traversal
- **Multi-targeting Support**: Works with both .NET Standard 2.0 and .NET 8.0

## Example Usage

### Analyzing Type Nullability

```csharp
using Albatross.CodeAnalysis.Symbols;
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
using Albatross.CodeAnalysis.Symbols;

// Check if a type is a collection (but not string)
bool isCollection = typeSymbol.IsCollection(compilation);

// Get the element type of a collection
if (typeSymbol.TryGetCollectionElementType(compilation, out ITypeSymbol? elementType)) {
    Console.WriteLine($"Collection element type: {elementType.Name}");
}
```

### Analyzing Generic Types

```csharp
using Albatross.CodeAnalysis.Symbols;

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
using Albatross.CodeAnalysis.Symbols;

// Get fully qualified type name
string fullName = typeSymbol.GetFullName(); // e.g., "System.Collections.Generic.List<string>"

// Get namespace
string namespaceName = namespaceSymbol.GetFullNamespace();

// Get type name relative to a namespace
string relativeName = typeSymbol.GetTypeNameRelativeToNamespace("MyApp.Services");
```

### Checking Type Relationships

```csharp
using Albatross.CodeAnalysis.Symbols;

// Check if a type derives from a base class
bool isDerived = typeSymbol.IsDerivedFrom(baseTypeSymbol);

// Check if a type implements an interface
bool hasInterface = typeSymbol.HasInterface(interfaceSymbol);

// Check if a type is a concrete class (not abstract, not static, not generic definition)
bool isConcrete = namedTypeSymbol.IsConcreteClass();
```

### Working with Attributes

```csharp
using Albatross.CodeAnalysis.Symbols;

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
using Albatross.CodeAnalysis.Symbols;

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
using Albatross.CodeAnalysis.Symbols;

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
├── Extensions.cs              # GeneratorExecutionContext extensions and utilities
├── My.cs                      # Constants for common .NET types and namespaces
└── Symbols/                   # Symbol analysis utilities (primary functionality)
    ├── AttributeDataExtensions.cs   # Attribute inspection helpers
    ├── Extensions.cs                # Type and symbol analysis extensions
    └── SymbolProvider.cs            # Access to common framework types
```

**Note**: The `Syntax/` directory contains deprecated syntax builders marked as obsolete.

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
</ItemGroup>
```

## Dependencies

- Microsoft.CodeAnalysis.CSharp (4.10.0)
- Humanizer.Core (2.14.1)

## License

See the [LICENSE](../LICENSE) file in the repository root.