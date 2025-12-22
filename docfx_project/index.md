# Albatross CodeAnalysis

A comprehensive suite of libraries for building Roslyn-based code analyzers, source generators, and analysis tools. These libraries provide powerful utilities to simplify working with the .NET Compiler Platform.

## Overview

The Albatross CodeAnalysis suite consists of three complementary libraries designed to make code analysis and generation tasks easier and more robust:

- **Symbol Analysis**: Rich extensions for analyzing types, symbols, and compilation information
- **Multi-targeting Support**: Full compatibility with both .NET Standard 2.0 and .NET 8.0
- **Testing Utilities**: Simplified testing framework for your analyzers and generators
- **Polyfill Support**: Modern C# language features for older target frameworks

## 📖 Learn More

For detailed examples, advanced usage, and comprehensive API documentation:

- **[Symbol Analysis Guide](articles/symbol-analysis.md)** - Deep dive into type and symbol inspection
- **[Nullability Detection](articles/nullability.md)** - Working with nullable types
- **[Collection Types](articles/collections.md)** - Analyzing collections and their elements
- **[Attribute Inspection](articles/attributes.md)** - Working with attributes on symbols
- **[Testing Your Analyzers](articles/testing.md)** - Unit testing best practices

## 🚀 Quick Start

### Installation

Install the core package via NuGet:

```powershell
Install-Package Albatross.CodeAnalysis
```

For additional functionality:

```powershell
# For .NET Standard 2.0 polyfills
Install-Package Albatross.CodeAnalysis.Polyfill

# For testing utilities
Install-Package Albatross.CodeAnalysis.Testing
```

### Basic Usage

```csharp
using Albatross.CodeAnalysis;
using Microsoft.CodeAnalysis;

// Analyze type nullability
bool isNullable = typeSymbol.IsNullable(compilation);

// Check for collection types
bool isCollection = typeSymbol.IsCollection(compilation);

// Extract element types from collections
if (typeSymbol.TryGetCollectionElementType(compilation, out var elementType)) {
    Console.WriteLine($"Element type: {elementType.Name}");
}
```
## Key Features

### Symbol Analysis Extensions
- **Nullability Detection**: Identify nullable reference and value types
- **Collection Analysis**: Detect collection types and extract element information
- **Type Relationships**: Analyze inheritance, interfaces, and generic constraints
- **Attribute Inspection**: Work with attribute data on symbols

### Developer Experience
- **Comprehensive Documentation**: Full API documentation with examples
- **Testing Support**: Simplified unit testing for your analyzers
- **Multi-targeting**: Support for both legacy and modern .NET frameworks
- **Performance Optimized**: Efficient symbol analysis with minimal allocations

## Use Cases

- **Source Generators**: Build powerful code generators with rich type analysis
- **Code Analyzers**: Create custom analyzers with comprehensive symbol inspection
- **Refactoring Tools**: Develop tools that understand type relationships and nullability
- **Code Quality Tools**: Build tools that enforce coding standards and patterns

## Getting Help

- **[API Documentation](./api/Albatross.CodeAnalysis.yml)**: Complete API reference
- **[Release Notes](./articles/release-notes.md)**: What's new in each version
- **[GitHub Repository](https://github.com/RushuiGuan/CodeAnalysis)**: Source code and issues
- **[Nuget Packages](https://www.nuget.org/packages?q=albatross.codeanalysis&includeComputedFrameworks=true&prerel=true)**: Nuget packages
---

*Built for developers who work with Roslyn and want to focus on their analysis logic rather than the underlying complexity.*

