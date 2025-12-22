# Symbol Analysis Guide

This guide provides comprehensive examples for analyzing types and symbols using Albatross.CodeAnalysis.

## Overview

The core strength of Albatross.CodeAnalysis lies in its rich symbol analysis capabilities. This library provides extensive extension methods that make it easier to inspect and analyze Roslyn symbols when building code generators, analyzers, and refactoring tools.

## Getting Started

All symbol analysis functionality is available through extension methods in the root `Albatross.CodeAnalysis` namespace:

```csharp
using Albatross.CodeAnalysis;
using Microsoft.CodeAnalysis;
```

## Type Analysis

### Basic Type Information

```csharp
// Get fully qualified type name
string fullName = typeSymbol.GetFullName(); 
// Result: "System.Collections.Generic.List<string>"

// Get namespace information
string namespaceName = namespaceSymbol.GetFullNamespace();

// Get type name relative to a specific namespace
string relativeName = typeSymbol.GetTypeNameRelativeToNamespace("MyApp.Services");
```

### Numeric Type Detection

```csharp
// Check if a type is numeric
bool isNumeric = typeSymbol.IsNumericType(compilation);

// Works with all numeric types:
// byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal
```

### Type Relationships

```csharp
// Check inheritance relationships
bool isDerived = typeSymbol.IsDerivedFrom(baseTypeSymbol);

// Check interface implementation
bool implementsInterface = typeSymbol.HasInterface(interfaceSymbol);

// Check if type is a concrete class (not abstract, not static, not generic definition)
bool isConcrete = namedTypeSymbol.IsConcreteClass();

// Compare type symbols for equality
bool isSameType = typeSymbol.Is(compilation.String());
```

### Generic Type Analysis

```csharp
// Check if a type is a generic type definition
bool isGenericDef = namedTypeSymbol.IsGenericTypeDefinition();

// Check if a type is constructed from a specific generic definition
bool isConstructed = typeSymbol.IsConstructedFromDefinition(genericDefinition);

// Get generic type arguments
if (typeSymbol.TryGetGenericTypeArguments(compilation.IEnumerableGenericDefinition(), 
    out ITypeSymbol[] arguments)) {
    foreach (var arg in arguments) {
        Console.WriteLine($"Type argument: {arg.Name}");
    }
}
```

## Working with Properties

### Getting Properties

```csharp
// Get all public properties
foreach (var property in namedTypeSymbol.GetProperties()) {
    Console.WriteLine($"Property: {property.Name}, Type: {property.Type.Name}");
}

// Get properties including base class properties
foreach (var property in namedTypeSymbol.GetProperties(useBaseClassProperties: true)) {
    Console.WriteLine($"Property: {property.Name}, Type: {property.Type.Name}");
}

// Get distinct properties (avoiding duplicates from inheritance)
foreach (var property in namedTypeSymbol.GetDistinctProperties(useBaseClassProperties: true)) {
    Console.WriteLine($"Property: {property.Name}");
}
```

### Property Analysis

```csharp
// Analyze property characteristics
foreach (var property in typeSymbol.GetProperties()) {
    var isReadOnly = property.IsReadOnly;
    var isWriteOnly = property.IsWriteOnly;
    var hasPublicGetter = property.GetMethod?.DeclaredAccessibility == Accessibility.Public;
    var hasPublicSetter = property.SetMethod?.DeclaredAccessibility == Accessibility.Public;
    
    Console.WriteLine($"{property.Name}: ReadOnly={isReadOnly}, WriteOnly={isWriteOnly}");
}
```

## Symbol Providers

The library provides convenient access to common framework types through the `SymbolProvider` class and compilation extensions:

### Common Framework Types

```csharp
// Get common framework type symbols
var stringSymbol = compilation.String();
var dateTimeSymbol = compilation.DateTime();
var dateOnlySymbol = compilation.DateOnly();
var timeOnlySymbol = compilation.TimeOnly();
var guidSymbol = compilation.Guid();

// Collection types
var enumerableSymbol = compilation.IEnumerable();
var listSymbol = compilation.IList();
var collectionSymbol = compilation.ICollection();
var dictionarySymbol = compilation.IDictionary();

// Nullable types
var nullableSymbol = compilation.Nullable();

// Generic definitions
var enumerableGeneric = compilation.IEnumerableGenericDefinition();
var listGeneric = compilation.IListGenericDefinition();
```

### Using Symbol Providers

```csharp
// Compare against framework types
bool isString = typeSymbol.Is(compilation.String());
bool isDateTime = typeSymbol.Is(compilation.DateTime());
bool isGuid = typeSymbol.Is(compilation.Guid());

// Check for collection interfaces
bool isEnumerable = typeSymbol.HasInterface(compilation.IEnumerable());
bool isList = typeSymbol.HasInterface(compilation.IList());
```

## Advanced Symbol Analysis

### Symbol Metadata

```csharp
// Access symbol metadata
var accessibility = symbol.DeclaredAccessibility;
var isStatic = symbol.IsStatic;
var isAbstract = symbol.IsAbstract;
var isSealed = symbol.IsSealed;

// For type symbols
if (symbol is ITypeSymbol typeSymbol) {
    var isReferenceType = typeSymbol.IsReferenceType;
    var isValueType = typeSymbol.IsValueType;
    var canBeReferencedByName = typeSymbol.CanBeReferencedByName;
}
```

### Namespace Analysis

```csharp
// Get full namespace path
string fullNamespace = namespaceSymbol.GetFullNamespace();

// Check namespace hierarchy
var currentNamespace = namespaceSymbol;
while (currentNamespace != null && !currentNamespace.IsGlobalNamespace) {
    Console.WriteLine($"Namespace level: {currentNamespace.Name}");
    currentNamespace = currentNamespace.ContainingNamespace;
}
```

## Best Practices

### Performance Considerations

1. **Cache Symbol Comparisons**: When repeatedly checking against framework types, cache the symbol references:

```csharp
// Good: Cache frequently used symbols
var stringType = compilation.String();
var dateTimeType = compilation.DateTime();

foreach (var property in properties) {
    if (property.Type.Is(stringType)) { /* ... */ }
    if (property.Type.Is(dateTimeType)) { /* ... */ }
}
```

2. **Use Specific Methods**: Use the most specific method for your needs:

```csharp
// Instead of general symbol analysis
if (typeSymbol.TypeKind == TypeKind.Class && !typeSymbol.IsAbstract) { /* ... */ }

// Use specific helper
if (typeSymbol.IsConcreteClass()) { /* ... */ }
```

### Error Handling

Always handle cases where symbols might not be found:

```csharp
// Safe symbol access
if (compilation.TryGetTypeByMetadataName("System.String", out var stringType)) {
    // Use stringType safely
} else {
    // Handle missing type
}
```

## Integration with Source Generators

These symbol analysis capabilities are particularly powerful when used in source generators:

```csharp
[Generator]
public class MySourceGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(/* syntax predicate */, /* transform */)
            .Where(x => x != null);
            
        context.RegisterSourceOutput(classDeclarations, GenerateCode);
    }
    
    private void GenerateCode(SourceProductionContext context, /* your model */) {
        var compilation = context.Compilation;
        var stringType = compilation.String();
        
        // Use symbol analysis to generate appropriate code
        foreach (var property in typeSymbol.GetProperties()) {
            if (property.Type.Is(stringType)) {
                // Generate string-specific code
            }
        }
    }
}
```

## Next Steps

- [Nullability Detection](nullability.md) - Learn about nullable type analysis
- [Collection Types](collections.md) - Working with collections and their elements
- [Attribute Inspection](attributes.md) - Analyzing attributes on symbols
- [API Reference](../api/Albatross.CodeAnalysis.yml) - Complete API documentation