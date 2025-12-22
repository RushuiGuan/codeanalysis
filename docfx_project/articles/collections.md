# Collection Types Analysis

Learn how to detect and analyze collection types and extract their element information using Albatross.CodeAnalysis.

## Overview

Working with collection types is a common requirement when building code analyzers and generators. Albatross.CodeAnalysis provides powerful utilities to detect collection types, extract element types, and understand collection hierarchies.

## Getting Started

```csharp
using Albatross.CodeAnalysis;
using Microsoft.CodeAnalysis;
```

## Collection Detection

### Basic Collection Detection

The `IsCollection()` method identifies collection types while excluding strings:

```csharp
// Check if a type is a collection (but not string)
bool isCollection = typeSymbol.IsCollection(compilation);

// Returns true for:
// - List<T>, IList<T>, ICollection<T>
// - Array types (T[])
// - IEnumerable<T> and derived interfaces
// - HashSet<T>, Dictionary<TKey, TValue>
// - Custom collection types

// Returns false for:
// - string (even though it implements IEnumerable<char>)
// - Primitive types (int, bool, etc.)
// - Non-collection reference types
```

### Why Strings Are Excluded

Strings implement `IEnumerable<char>`, but they're typically treated as scalar values rather than collections in most scenarios:

```csharp
// These are all collections:
List<int> numbers;           // IsCollection: true
int[] array;                // IsCollection: true
IEnumerable<string> items;  // IsCollection: true

// But string is not considered a collection:
string text;                // IsCollection: false (even though it's IEnumerable<char>)
```

## Element Type Extraction

### Getting Element Types

Use `TryGetCollectionElementType()` to extract the element type from collections:

```csharp
// Get the element type of a collection
if (typeSymbol.TryGetCollectionElementType(compilation, out ITypeSymbol? elementType)) {
    Console.WriteLine($"Collection element type: {elementType.Name}");
    
    // For List<string>: elementType.Name = "String"
    // For int[]: elementType.Name = "Int32"
    // For Dictionary<string, int>: elementType represents the value type "Int32"
} else {
    Console.WriteLine("Not a collection type");
}
```

### Array Type Handling

Arrays receive special handling:

```csharp
public void AnalyzeArrayType(ITypeSymbol type, Compilation compilation) {
    if (type is IArrayTypeSymbol arrayType) {
        Console.WriteLine($"Array rank: {arrayType.Rank}");
        Console.WriteLine($"Element type: {arrayType.ElementType.Name}");
    }
    
    // Or use the generic method:
    if (type.TryGetCollectionElementType(compilation, out var elementType)) {
        Console.WriteLine($"Element type: {elementType.Name}");
    }
}
```

## Comprehensive Collection Analysis

### Complete Collection Information

```csharp
public class CollectionAnalyzer {
    public CollectionInfo AnalyzeType(ITypeSymbol type, Compilation compilation) {
        var info = new CollectionInfo {
            TypeName = type.Name,
            IsCollection = type.IsCollection(compilation)
        };
        
        if (info.IsCollection) {
            if (type.TryGetCollectionElementType(compilation, out var elementType)) {
                info.ElementType = elementType.Name;
                info.ElementTypeFullName = elementType.ToDisplayString();
                info.IsElementNullable = elementType.IsNullable(compilation);
            }
            
            info.CollectionInterfaces = GetCollectionInterfaces(type, compilation);
            info.IsArray = type.TypeKind == TypeKind.Array;
            info.IsGenericCollection = type is INamedTypeSymbol namedType && namedType.IsGenericType;
        }
        
        return info;
    }
    
    private List<string> GetCollectionInterfaces(ITypeSymbol type, Compilation compilation) {
        var interfaces = new List<string>();
        
        if (type.HasInterface(compilation.IEnumerable())) {
            interfaces.Add("IEnumerable");
        }
        
        if (type.HasInterface(compilation.ICollection())) {
            interfaces.Add("ICollection");
        }
        
        if (type.HasInterface(compilation.IList())) {
            interfaces.Add("IList");
        }
        
        return interfaces;
    }
}

public class CollectionInfo {
    public string TypeName { get; set; } = string.Empty;
    public bool IsCollection { get; set; }
    public string ElementType { get; set; } = string.Empty;
    public string ElementTypeFullName { get; set; } = string.Empty;
    public bool IsElementNullable { get; set; }
    public List<string> CollectionInterfaces { get; set; } = new();
    public bool IsArray { get; set; }
    public bool IsGenericCollection { get; set; }
}
```

## Working with Generic Collections

### Generic Type Analysis

```csharp
public void AnalyzeGenericCollection(ITypeSymbol type, Compilation compilation) {
    if (type is INamedTypeSymbol namedType && namedType.IsGenericType) {
        Console.WriteLine($"Generic type: {namedType.Name}");
        Console.WriteLine($"Type arguments: {namedType.TypeArguments.Length}");
        
        foreach (var arg in namedType.TypeArguments) {
            Console.WriteLine($"  - {arg.ToDisplayString()}");
        }
        
        // Check if it's constructed from specific generic definitions
        var listDef = compilation.IListGenericDefinition();
        if (namedType.IsConstructedFromDefinition(listDef)) {
            Console.WriteLine("This is a List<T>");
        }
        
        var dictDef = compilation.IDictionaryGenericDefinition();
        if (namedType.IsConstructedFromDefinition(dictDef)) {
            Console.WriteLine("This is a Dictionary<TKey, TValue>");
        }
    }
}
```

### Dictionary Handling

Dictionaries require special consideration:

```csharp
public void AnalyzeDictionary(ITypeSymbol type, Compilation compilation) {
    if (type is INamedTypeSymbol namedType) {
        var dictDef = compilation.IDictionaryGenericDefinition();
        
        if (namedType.IsConstructedFromDefinition(dictDef)) {
            var keyType = namedType.TypeArguments[0];
            var valueType = namedType.TypeArguments[1];
            
            Console.WriteLine($"Dictionary key type: {keyType.ToDisplayString()}");
            Console.WriteLine($"Dictionary value type: {valueType.ToDisplayString()}");
            
            // Note: TryGetCollectionElementType returns the value type for dictionaries
            if (type.TryGetCollectionElementType(compilation, out var elementType)) {
                Console.WriteLine($"Element type (value): {elementType.ToDisplayString()}");
            }
        }
    }
}
```

## Practical Examples

### Code Generation for Collections

```csharp
public string GenerateCollectionProcessingCode(IPropertySymbol property, Compilation compilation) {
    var type = property.Type;
    var propertyName = property.Name;
    
    if (!type.IsCollection(compilation)) {
        return $"// {propertyName} is not a collection";
    }
    
    if (!type.TryGetCollectionElementType(compilation, out var elementType)) {
        return $"// Cannot determine element type for {propertyName}";
    }
    
    var elementTypeName = elementType.ToDisplayString();
    var isNullable = type.IsNullable(compilation);
    var isElementNullable = elementType.IsNullable(compilation);
    
    var code = new StringBuilder();
    
    if (isNullable) {
        code.AppendLine($"if ({propertyName} != null)");
        code.AppendLine("{");
    }
    
    code.AppendLine($"    foreach (var item in {propertyName})");
    code.AppendLine("    {");
    
    if (isElementNullable) {
        code.AppendLine("        if (item != null)");
        code.AppendLine("        {");
        code.AppendLine($"            ProcessItem(item); // {elementTypeName}");
        code.AppendLine("        }");
    } else {
        code.AppendLine($"        ProcessItem(item); // {elementTypeName}");
    }
    
    code.AppendLine("    }");
    
    if (isNullable) {
        code.AppendLine("}");
    }
    
    return code.ToString();
}
```

### Serialization Code Generation

```csharp
public string GenerateSerializationCode(IPropertySymbol property, Compilation compilation) {
    var type = property.Type;
    var name = property.Name;
    
    if (!type.IsCollection(compilation)) {
        return GenerateScalarSerialization(name, type);
    }
    
    if (!type.TryGetCollectionElementType(compilation, out var elementType)) {
        return $"// Cannot serialize {name} - unknown element type";
    }
    
    var code = new StringBuilder();
    var isNullable = type.IsNullable(compilation);
    
    if (isNullable) {
        code.AppendLine($"if ({name} != null)");
        code.AppendLine("{");
        code.AppendLine($"    writer.WriteStartArray(\"{name}\");");
        code.AppendLine($"    foreach (var item in {name})");
    } else {
        code.AppendLine($"writer.WriteStartArray(\"{name}\");");
        code.AppendLine($"foreach (var item in {name})");
    }
    
    code.AppendLine("    {");
    
    if (elementType.IsNullable(compilation)) {
        code.AppendLine("        if (item != null)");
        code.AppendLine("        {");
        code.AppendLine("            writer.WriteValue(item);");
        code.AppendLine("        }");
        code.AppendLine("        else");
        code.AppendLine("        {");
        code.AppendLine("            writer.WriteNull();");
        code.AppendLine("        }");
    } else {
        code.AppendLine("        writer.WriteValue(item);");
    }
    
    code.AppendLine("    }");
    code.AppendLine("    writer.WriteEndArray();");
    
    if (isNullable) {
        code.AppendLine("}");
        code.AppendLine("else");
        code.AppendLine("{");
        code.AppendLine($"    writer.WriteNull(\"{name}\");");
        code.AppendLine("}");
    }
    
    return code.ToString();
}
```

## Testing Collection Analysis

### Unit Test Examples

```csharp
[Theory]
[InlineData("List<string>", true, "String")]
[InlineData("string[]", true, "String")]
[InlineData("IEnumerable<int>", true, "Int32")]
[InlineData("Dictionary<string, int>", true, "Int32")] // Value type for dictionaries
[InlineData("string", false, null)] // String is not considered a collection
[InlineData("int", false, null)]
public async Task TestCollectionDetection(string typeName, bool expectedIsCollection, string? expectedElementType) {
    var compilation = await CreateTestCompilation();
    var type = compilation.GetRequiredTypeByName(typeName);
    
    var isCollection = type.IsCollection(compilation);
    isCollection.Should().Be(expectedIsCollection);
    
    if (expectedIsCollection && expectedElementType != null) {
        var hasElement = type.TryGetCollectionElementType(compilation, out var elementType);
        hasElement.Should().BeTrue();
        elementType!.Name.Should().Be(expectedElementType);
    }
}
```

## Advanced Scenarios

### Custom Collection Types

```csharp
// For custom collections that implement IEnumerable<T>
public class CustomList<T> : IEnumerable<T> {
    // Implementation...
}

// The analysis will correctly identify:
// - CustomList<string>.IsCollection(compilation) → true
// - CustomList<string>.TryGetCollectionElementType(...) → elementType = "String"
```

### Nested Collections

```csharp
public void AnalyzeNestedCollections(ITypeSymbol type, Compilation compilation) {
    if (type.TryGetCollectionElementType(compilation, out var elementType)) {
        Console.WriteLine($"First level element: {elementType.Name}");
        
        // Check if the element itself is a collection
        if (elementType.IsCollection(compilation) && 
            elementType.TryGetCollectionElementType(compilation, out var nestedElement)) {
            Console.WriteLine($"Nested element: {nestedElement.Name}");
            // For List<List<int>>, this would show "Int32"
        }
    }
}
```

## Performance Considerations

### Caching Collection Interface Symbols

```csharp
public class OptimizedCollectionAnalyzer {
    private readonly INamedTypeSymbol _ienumerable;
    private readonly INamedTypeSymbol _icollection;
    private readonly INamedTypeSymbol _ilist;
    
    public OptimizedCollectionAnalyzer(Compilation compilation) {
        _ienumerable = compilation.IEnumerable();
        _icollection = compilation.ICollection();
        _ilist = compilation.IList();
    }
    
    public bool IsCollectionInterface(ITypeSymbol type) {
        return type.HasInterface(_ienumerable) ||
               type.HasInterface(_icollection) ||
               type.HasInterface(_ilist);
    }
}
```

## Related Topics

- [Symbol Analysis Guide](symbol-analysis.md) - General symbol analysis techniques
- [Nullability Detection](nullability.md) - Working with nullable collections
- [Attribute Inspection](attributes.md) - Analyzing collection-related attributes
- [API Reference](../api/Albatross.CodeAnalysis.yml) - Complete API documentation