# Albatross.CodeAnalysis

A powerful code analysis library that provides symbol analysis utilities and extensions to simplify working with Roslyn-based code generators and analyzers. This library helps you analyze types, symbols, and compilation information when building source generators and code analysis tools.

> **Note**: The `Albatross.CodeAnalysis.Syntax` namespace has been deprecated. Use [Albatross.CodeGen.CSharp](https://www.nuget.org/packages/Albatross.CodeGen.CSharp) for C# code generation.

## 📚 Documentation

📖 **[Complete Documentation](https://rushuiguan.github.io/codeanalysis/index.html)** | 📚 **[API Reference](https://rushuiguan.github.io/codeanalysis/api/Albatross.CodeAnalysis.html)** | 📝 **[Release Notes](https://rushuiguan.github.io/codeanalysis/articles/release-notes.html)**

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

- **[Symbol Analysis Guide](https://rushuiguan.github.io/codeanalysis/articles/symbol-analysis.html)** - Deep dive into type and symbol inspection
- **[Nullability Detection](https://rushuiguan.github.io/codeanalysis/articles/nullability.html)** - Working with nullable types
- **[Collection Types](https://rushuiguan.github.io/codeanalysis/articles/collections.html)** - Analyzing collections and their elements
- **[Attribute Inspection](https://rushuiguan.github.io/codeanalysis/articles/attributes.html)** - Working with attributes on symbols
- **[Testing Your Analyzers](https://rushuiguan.github.io/codeanalysis/articles/testing.html)** - Unit testing best practices


