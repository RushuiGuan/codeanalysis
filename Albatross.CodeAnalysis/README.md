# Albatross.CodeAnalysis

A powerful code analysis library that provides intuitive syntax builders and utilities to simplify the creation of Roslyn-based code generators. This library abstracts away the complexity of working directly with Roslyn's syntax trees, making it easier to generate C# code programmatically.

## Features

- **Syntax Builders**: Fluent API for building C# syntax trees with methods, classes, properties, and more
- **Code Generation Stack**: `CodeStack` utility for managing nested code structures with automatic scoping
- **Symbol Analysis Extensions**: Helper methods for analyzing symbol information, including nullability detection and type information
- **Compilation Utilities**: Extensions for working with `GeneratorExecutionContext` including diagnostics and debug file generation
- **Type System Helpers**: Predefined constants and utilities for common .NET types and namespaces
- **Multi-targeting Support**: Works with both .NET Standard 2.0 and .NET 8.0

## Example Usage

### Building a Simple Class with Method

```csharp
using Albatross.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

[Generator]
public class MyCodeGen : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(
            context.CompilationProvider,
            (ctx, compilation) => {
                var cs = new CodeStack();
                using (cs.NewScope(new CompilationUnitBuilder())) {
                    using (cs.NewScope(new NamespaceDeclarationBuilder("MyNamespace"))) {
                        using (cs.NewScope(new ClassDeclarationBuilder("MyClass").Public())) {
                            using (cs.NewScope(new MethodDeclarationBuilder("void", "MyMethod").Public())) {
                                cs.Begin(new VariableBuilder("string", "message"))
                                  .With(new LiteralNode("Hello World"))
                                  .End();
                            }
                        }
                    }
                }
                ctx.AddSource("MyClass", SourceText.From(cs.Build(), Encoding.UTF8));
            }
        );
    }
}
```

### Using Symbol Extensions

```csharp
using Albatross.CodeAnalysis.Symbols;

// Check if a type is nullable
bool isNullable = typeSymbol.IsNullable(compilation);

// Get collection element type
var elementType = typeSymbol.GetCollectionType(compilation);

// Get symbol name with namespace
string fullName = symbol.GetSymbolName();
```

### Creating Debug Output

```csharp
using Albatross.CodeAnalysis;

// Generate a debug file during code generation
context.CreateGeneratorDebugFile("debug-output.txt", generatedCode);

// Report custom diagnostics
context.CodeGenDiagnostic(DiagnosticSeverity.Warning, "GEN001", "Custom warning message");
```

## Project Structure

```
Albatross.CodeAnalysis/
├── Extensions.cs              # GeneratorExecutionContext extensions
├── My.cs                      # Constants and utilities for common types/namespaces
├── Symbols/                   # Symbol analysis utilities
│   ├── AttributeDataExtensions.cs
│   ├── Extensions.cs          # Type and symbol analysis extensions
│   └── SymbolProvider.cs
└── Syntax/                    # Syntax builders and nodes
    ├── ClassDeclarationBuilder.cs
    ├── MethodDeclarationBuilder.cs
    ├── PropertyNode.cs
    ├── CodeStack.cs
    └── ... (many more builders)
```

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