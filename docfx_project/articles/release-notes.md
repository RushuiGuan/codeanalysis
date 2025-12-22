# Release Notes

## Version 8.0.1 - December 2025

### 🚨 Breaking Changes

- **Deprecated Roslyn Syntax Helpers**: All classes in the `Albatross.CodeAnalysis.Syntax` namespace have been marked as obsolete. Use [Albatross.CodeGen.CSharp](https://www.nuget.org/packages/Albatross.CodeGen.CSharp) instead for C# code generation functionality.
- **Removed MSBuild Integration**: The `Albatross.CodeAnalysis.MSBuild` project has been completely removed from the solution.

### 🆕 New Features

#### New Testing Library
- **`Albatross.CodeAnalysis.Testing`**: Brand new testing utility library for Roslyn-based analyzers and source generators
  - Easy creation of `CSharpCompilation` instances with proper .NET 8.0 framework references
  - Modern C# 12 language support by default
  - Async API with cancellation token support
  - Simplified testing workflows for code analysis scenarios

#### Enhanced Polyfill Library
- **`HashCode` struct**: Added for .NET Standard 2.0 projects to provide modern hash code combining functionality
- **`CallerArgumentExpressionAttribute`**: Enhanced support for C# 10 caller argument expressions
- **`RequiredMemberAttribute`**: Full implementation for C# 11 required members syntax
- **`NotNullAttribute`**: Comprehensive nullable reference type annotations
- **Conditional Compilation**: All polyfills are now properly conditionally compiled for .NET Standard 2.0 only

### 🔧 Core Library Improvements

#### Enhanced Symbol Analysis
- **Improved Nullability Detection**: Enhanced `IsNullable()` methods with better support for both reference and value types
- **Expanded Symbol Extensions**: New extension methods for comprehensive type analysis
- **Better Collection Type Handling**: Improved detection and analysis of collection types and their element types
- **Enhanced Attribute Analysis**: Better `AttributeDataExtensions` for working with symbol attributes

#### Code Organization
- **Consolidated Extensions**: Moved symbol extensions from `Symbols` subfolder to root namespace for better API discoverability
- **Improved Symbol Provider**: Enhanced `SymbolProvider` class with more comprehensive framework type access
- **Better Multi-targeting**: Improved support for both .NET Standard 2.0 and .NET 8.0 scenarios

### 🐛 Bug Fixes

- **Fixed `IsExternalInit`**: Corrected implementation for better init-only property support
- **Improved Framework References**: Better handling of framework references in multi-targeted scenarios
- **Enhanced Compilation Factory**: More robust compilation creation for testing scenarios

### 📦 Package & Build Improvements

- **Cleaned Up Solution**: Removed deprecated projects and improved solution structure
- **Better NuGet Packaging**: Improved package metadata and dependencies
- **Simplified Build Process**: Streamlined build and packaging scripts

---

*For detailed API documentation, see the [API Reference](../api/Albatross.CodeAnalysis.yml).*