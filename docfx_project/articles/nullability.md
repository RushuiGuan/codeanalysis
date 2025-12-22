# Nullability Detection

Learn how to analyze nullable reference types and nullable value types using Albatross.CodeAnalysis.

## Overview

Nullability analysis is one of the most important features for modern C# development. Albatross.CodeAnalysis provides comprehensive support for detecting and working with both nullable reference types (introduced in C# 8) and nullable value types (`Nullable<T>`).

## Getting Started

```csharp
using Albatross.CodeAnalysis;
using Microsoft.CodeAnalysis;
```

## Nullable Type Detection

### Universal Nullability Check

The `IsNullable()` method works with both reference and value types:

```csharp
// Universal nullability check
bool isNullable = typeSymbol.IsNullable(compilation);

// This works for:
// - string? (nullable reference type) → true
// - string (non-nullable reference type) → false
// - int? (nullable value type) → true  
// - int (non-nullable value type) → false
```

### Nullable Reference Types

For reference types, you can specifically check for nullable annotations:

```csharp
// Check specifically for nullable reference types
bool isNullableRef = typeSymbol.IsNullableReferenceType();

// Examples:
// string? → true
// string → false
// object? → true
// object → false
```

### Nullable Value Types

For value types, you can check for `Nullable<T>` wrapper:

```csharp
// Check specifically for nullable value types (Nullable<T>)
bool isNullableValue = typeSymbol.IsNullableValueType(compilation);

// Examples:
// int? → true
// DateTime? → true
// int → false
// DateTime → false
```

## Working with Nullable Value Types

### Extracting Underlying Types

When working with `Nullable<T>`, you often need the underlying type:

```csharp
// Try to get the underlying value type from Nullable<T>
if (typeSymbol.TryGetNullableValueType(compilation, out ITypeSymbol? underlyingType)) {
    Console.WriteLine($"Underlying type: {underlyingType.Name}");
    // For int?, this would output "Int32"
} else {
    // Not a nullable value type
    Console.WriteLine($"Type is: {typeSymbol.Name}");
}
```

### Practical Example

```csharp
public void AnalyzeProperty(IPropertySymbol property, Compilation compilation) {
    var propertyType = property.Type;
    
    if (propertyType.IsNullable(compilation)) {
        if (propertyType.IsNullableReferenceType()) {
            Console.WriteLine($"{property.Name} is a nullable reference type: {propertyType.Name}");
        } 
        else if (propertyType.TryGetNullableValueType(compilation, out var underlying)) {
            Console.WriteLine($"{property.Name} is nullable {underlying.Name}");
        }
    } else {
        Console.WriteLine($"{property.Name} is non-nullable: {propertyType.Name}");
    }
}
```

## Comprehensive Example

Here's a complete example that demonstrates nullability analysis:

```csharp
public class NullabilityAnalyzer {
    public void AnalyzeClass(INamedTypeSymbol classSymbol, Compilation compilation) {
        Console.WriteLine($"Analyzing class: {classSymbol.Name}");
        
        foreach (var property in classSymbol.GetProperties()) {
            AnalyzeProperty(property, compilation);
        }
    }
    
    private void AnalyzeProperty(IPropertySymbol property, Compilation compilation) {
        var type = property.Type;
        var nullabilityInfo = AnalyzeNullability(type, compilation);
        
        Console.WriteLine($"  {property.Name}: {nullabilityInfo}");
    }
    
    private string AnalyzeNullability(ITypeSymbol type, Compilation compilation) {
        if (!type.IsNullable(compilation)) {
            return $"{type.Name} (non-nullable)";
        }
        
        if (type.IsNullableReferenceType()) {
            return $"{type.Name} (nullable reference)";
        }
        
        if (type.TryGetNullableValueType(compilation, out var underlyingType)) {
            return $"{underlyingType.Name}? (nullable value)";
        }
        
        return $"{type.Name} (unknown nullability)";
    }
}
```

## Usage in Source Generators

Nullability analysis is particularly useful in source generators:

```csharp
[Generator]
public class NullabilityAwareGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // ... setup code ...
        
        context.RegisterSourceOutput(classDeclarations, GenerateCode);
    }
    
    private void GenerateCode(SourceProductionContext context, ClassModel model) {
        var compilation = context.Compilation;
        var stringBuilder = new StringBuilder();
        
        foreach (var property in model.Properties) {
            if (property.Type.IsNullable(compilation)) {
                // Generate null-check code
                stringBuilder.AppendLine($"if ({property.Name} != null)");
                stringBuilder.AppendLine("{");
                stringBuilder.AppendLine($"    // Use {property.Name}");
                stringBuilder.AppendLine("}");
            } else {
                // Generate direct usage code
                stringBuilder.AppendLine($"// {property.Name} is never null");
            }
        }
        
        // Add generated source...
    }
}
```

## Test Examples

Here are some test cases that demonstrate the nullability detection:

```csharp
public class TestClass {
    public int Value { get; set; }              // IsNullable: false
    public int? NullableValue { get; set; }     // IsNullable: true (nullable value)
    public string Text { get; set; } = "";      // IsNullable: false
    public string? NullableText { get; set; }   // IsNullable: true (nullable reference)
    public int[] Array { get; set; } = [];      // IsNullable: false  
    public int[]? NullableArray { get; set; }   // IsNullable: true (nullable reference)
}
```

### Unit Test Example

```csharp
[Theory]
[InlineData("Value", false)]
[InlineData("NullableValue", true)]
[InlineData("Text", false)]
[InlineData("NullableText", true)]
[InlineData("Array", false)]
[InlineData("NullableArray", true)]
public async Task VerifyIsNullable(string propertyName, bool expectedNullable) {
    var compilation = await TestCode.CreateNet8CompilationAsync();
    var classSymbol = compilation.GetRequiredSymbol("TestClass");
    var property = classSymbol.GetMembers()
        .OfType<IPropertySymbol>()
        .First(x => x.Name == propertyName);
        
    var actualNullable = property.Type.IsNullable(compilation);
    actualNullable.Should().Be(expectedNullable);
}
```

## Common Patterns

### Null-Safe Code Generation

```csharp
public string GeneratePropertyAccess(IPropertySymbol property, Compilation compilation) {
    var type = property.Type;
    var propertyName = property.Name;
    
    if (type.IsNullable(compilation)) {
        return $"{propertyName}?.ToString() ?? \"null\"";
    } else {
        return $"{propertyName}.ToString()";
    }
}
```

### Serialization Code Generation

```csharp
public string GenerateSerializationCode(IPropertySymbol property, Compilation compilation) {
    if (property.Type.IsNullable(compilation)) {
        return $@"
if ({property.Name} != null) {{
    writer.WriteProperty(""{property.Name}"", {property.Name});
}}";
    } else {
        return $@"writer.WriteProperty(""{property.Name}"", {property.Name});";
    }
}
```

## Best Practices

### 1. Always Use Compilation Context

Nullability detection requires the compilation context for accurate results:

```csharp
// ✅ Good - uses compilation context
bool isNullable = typeSymbol.IsNullable(compilation);

// ❌ Avoid - less reliable for edge cases
bool isNullable = typeSymbol.CanBeReferencedByName && typeSymbol.IsReferenceType;
```

### 2. Handle Edge Cases

Always consider edge cases in your nullability analysis:

```csharp
public bool IsEffectivelyNullable(ITypeSymbol type, Compilation compilation) {
    // Handle the obvious cases
    if (type.IsNullable(compilation)) {
        return true;
    }
    
    // Handle special cases like generic type parameters
    if (type.TypeKind == TypeKind.TypeParameter) {
        var typeParam = (ITypeParameterSymbol)type;
        // Check constraints to determine nullability
        return !typeParam.HasNotNullConstraint;
    }
    
    return false;
}
```

### 3. Consistent Null Handling

When generating code, be consistent in how you handle nulls:

```csharp
public class CodeGenHelper {
    private readonly Compilation _compilation;
    
    public string GenerateNullCheck(ITypeSymbol type, string variableName) {
        if (type.IsNullable(_compilation)) {
            return $"if ({variableName} != null)";
        }
        
        return string.Empty; // No null check needed
    }
}
```

## Related Topics

- [Symbol Analysis Guide](symbol-analysis.md) - General symbol analysis techniques
- [Collection Types](collections.md) - Working with nullable collections
- [Testing Your Analyzers](testing.md) - Unit testing nullability analysis
- [API Reference](../api/Albatross.CodeAnalysis.yml) - Complete API documentation