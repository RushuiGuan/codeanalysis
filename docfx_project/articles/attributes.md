# Attribute Inspection

Learn how to analyze and work with attributes on symbols using Albatross.CodeAnalysis.

## Overview

Attributes are a fundamental part of .NET metadata and are essential for many code analysis scenarios. Albatross.CodeAnalysis provides comprehensive utilities for inspecting attributes, extracting their arguments, and understanding attribute inheritance patterns.

## Getting Started

```csharp
using Albatross.CodeAnalysis;
using Microsoft.CodeAnalysis;
```

## Basic Attribute Detection

### Checking for Attributes

```csharp
// Check if a symbol has a specific attribute
bool hasAttr = symbol.HasAttribute(attributeSymbol);

// Example usage:
var obsoleteAttr = compilation.GetTypeByMetadataName("System.ObsoleteAttribute");
if (method.HasAttribute(obsoleteAttr)) {
    Console.WriteLine("Method is obsolete");
}
```

### Getting Attribute Data

```csharp
// Try to get an attribute and its data
if (symbol.TryGetAttribute(attributeSymbol, out AttributeData? attrData)) {
    Console.WriteLine($"Found attribute: {attrData.AttributeClass?.Name}");
    
    // Access constructor arguments
    foreach (var arg in attrData.ConstructorArguments) {
        Console.WriteLine($"Constructor arg: {arg.Value}");
    }
    
    // Access named arguments
    foreach (var namedArg in attrData.NamedArguments) {
        Console.WriteLine($"{namedArg.Key} = {namedArg.Value.Value}");
    }
} else {
    Console.WriteLine("Attribute not found");
}
```

## Working with Attribute Arguments

### Named Arguments

Extract specific named arguments from attributes:

```csharp
// Try to get a named argument from the attribute
if (attrData.TryGetNamedArgument("PropertyName", out TypedConstant value)) {
    Console.WriteLine($"Property value: {value.Value}");
    
    // Handle different value types
    switch (value.Kind) {
        case TypedConstantKind.Primitive:
            Console.WriteLine($"Primitive value: {value.Value}");
            break;
        case TypedConstantKind.Enum:
            Console.WriteLine($"Enum value: {value.Value}");
            break;
        case TypedConstantKind.Type:
            Console.WriteLine($"Type value: {value.Type}");
            break;
        case TypedConstantKind.Array:
            Console.WriteLine("Array value:");
            foreach (var item in value.Values) {
                Console.WriteLine($"  - {item.Value}");
            }
            break;
    }
}
```

### Constructor Arguments

```csharp
public void AnalyzeConstructorArguments(AttributeData attrData) {
    for (int i = 0; i < attrData.ConstructorArguments.Length; i++) {
        var arg = attrData.ConstructorArguments[i];
        Console.WriteLine($"Arg {i}: {arg.Value} (Type: {arg.Type?.Name})");
    }
}
```

### Complex Attribute Example

```csharp
// For an attribute like: [Display(Name = "Full Name", Description = "The user's full name")]
public DisplayInfo ExtractDisplayInfo(ISymbol symbol, Compilation compilation) {
    var displayAttr = compilation.GetTypeByMetadataName("System.ComponentModel.DataAnnotations.DisplayAttribute");
    
    if (!symbol.TryGetAttribute(displayAttr, out var attrData)) {
        return new DisplayInfo();
    }
    
    var info = new DisplayInfo();
    
    if (attrData.TryGetNamedArgument("Name", out var nameValue)) {
        info.Name = nameValue.Value?.ToString();
    }
    
    if (attrData.TryGetNamedArgument("Description", out var descValue)) {
        info.Description = descValue.Value?.ToString();
    }
    
    if (attrData.TryGetNamedArgument("Order", out var orderValue)) {
        info.Order = (int?)orderValue.Value;
    }
    
    return info;
}

public class DisplayInfo {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
}
```

## Attribute Inheritance Analysis

### Base Type Attribute Checking

```csharp
// Check if a symbol has an attribute, including inherited attributes
bool hasAttrWithBase = symbol.HasAttributeWithBaseType(baseAttributeSymbol);

// This is useful for checking attribute hierarchies like:
// - ValidationAttribute (base)
//   - RequiredAttribute (derived)
//   - RangeAttribute (derived)
//   - StringLengthAttribute (derived)
```

### Custom Attribute Hierarchy Analysis

```csharp
public List<AttributeData> GetAttributeHierarchy(ISymbol symbol, INamedTypeSymbol baseAttributeType) {
    var attributes = new List<AttributeData>();
    
    foreach (var attr in symbol.GetAttributes()) {
        if (attr.AttributeClass?.IsDerivedFrom(baseAttributeType) == true) {
            attributes.Add(attr);
        }
    }
    
    return attributes;
}
```

## Practical Examples

### Validation Attribute Analysis

```csharp
public class ValidationAnalyzer {
    private readonly Compilation _compilation;
    
    public ValidationAnalyzer(Compilation compilation) {
        _compilation = compilation;
    }
    
    public ValidationInfo AnalyzeProperty(IPropertySymbol property) {
        var info = new ValidationInfo();
        
        // Check for Required attribute
        var requiredAttr = _compilation.GetTypeByMetadataName("System.ComponentModel.DataAnnotations.RequiredAttribute");
        if (property.HasAttribute(requiredAttr)) {
            info.IsRequired = true;
        }
        
        // Check for StringLength attribute
        var stringLengthAttr = _compilation.GetTypeByMetadataName("System.ComponentModel.DataAnnotations.StringLengthAttribute");
        if (property.TryGetAttribute(stringLengthAttr, out var stringLengthData)) {
            if (stringLengthData.ConstructorArguments.Length > 0) {
                info.MaxLength = (int)stringLengthData.ConstructorArguments[0].Value!;
            }
            
            if (stringLengthData.TryGetNamedArgument("MinimumLength", out var minLengthValue)) {
                info.MinLength = (int)minLengthValue.Value!;
            }
        }
        
        // Check for Range attribute
        var rangeAttr = _compilation.GetTypeByMetadataName("System.ComponentModel.DataAnnotations.RangeAttribute");
        if (property.TryGetAttribute(rangeAttr, out var rangeData) && rangeData.ConstructorArguments.Length >= 2) {
            info.MinValue = rangeData.ConstructorArguments[0].Value;
            info.MaxValue = rangeData.ConstructorArguments[1].Value;
        }
        
        return info;
    }
}

public class ValidationInfo {
    public bool IsRequired { get; set; }
    public int? MaxLength { get; set; }
    public int? MinLength { get; set; }
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }
}
```

### Serialization Attribute Analysis

```csharp
public class SerializationAnalyzer {
    public SerializationInfo AnalyzeProperty(IPropertySymbol property, Compilation compilation) {
        var info = new SerializationInfo {
            PropertyName = property.Name
        };
        
        // Check for JsonPropertyName attribute
        var jsonPropertyAttr = compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonPropertyNameAttribute");
        if (property.TryGetAttribute(jsonPropertyAttr, out var jsonAttrData) && 
            jsonAttrData.ConstructorArguments.Length > 0) {
            info.SerializedName = jsonAttrData.ConstructorArguments[0].Value?.ToString();
        }
        
        // Check for JsonIgnore attribute
        var jsonIgnoreAttr = compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonIgnoreAttribute");
        if (property.HasAttribute(jsonIgnoreAttr)) {
            info.IsIgnored = true;
        }
        
        // Check for JsonConverter attribute
        var jsonConverterAttr = compilation.GetTypeByMetadataName("System.Text.Json.Serialization.JsonConverterAttribute");
        if (property.TryGetAttribute(jsonConverterAttr, out var converterData) &&
            converterData.ConstructorArguments.Length > 0 &&
            converterData.ConstructorArguments[0].Value is ITypeSymbol converterType) {
            info.CustomConverter = converterType.ToDisplayString();
        }
        
        return info;
    }
}

public class SerializationInfo {
    public string PropertyName { get; set; } = string.Empty;
    public string? SerializedName { get; set; }
    public bool IsIgnored { get; set; }
    public string? CustomConverter { get; set; }
}
```

## Code Generation with Attributes

### Generating Validation Code

```csharp
public string GenerateValidationCode(IPropertySymbol property, Compilation compilation) {
    var validation = new ValidationAnalyzer(compilation);
    var info = validation.AnalyzeProperty(property);
    var code = new StringBuilder();
    var propertyName = property.Name;
    
    if (info.IsRequired) {
        code.AppendLine($"if ({propertyName} == null)");
        code.AppendLine("{");
        code.AppendLine($"    errors.Add(\"{propertyName} is required\");");
        code.AppendLine("}");
    }
    
    if (info.MaxLength.HasValue && property.Type.Is(compilation.String())) {
        code.AppendLine($"if ({propertyName}?.Length > {info.MaxLength.Value})");
        code.AppendLine("{");
        code.AppendLine($"    errors.Add(\"{propertyName} exceeds maximum length of {info.MaxLength.Value}\");");
        code.AppendLine("}");
    }
    
    if (info.MinLength.HasValue && property.Type.Is(compilation.String())) {
        code.AppendLine($"if ({propertyName}?.Length < {info.MinLength.Value})");
        code.AppendLine("{");
        code.AppendLine($"    errors.Add(\"{propertyName} is below minimum length of {info.MinLength.Value}\");");
        code.AppendLine("}");
    }
    
    return code.ToString();
}
```

### Generating Serialization Code

```csharp
public string GenerateSerializationCode(IPropertySymbol property, Compilation compilation) {
    var analyzer = new SerializationAnalyzer();
    var info = analyzer.AnalyzeProperty(property, compilation);
    
    if (info.IsIgnored) {
        return $"// {info.PropertyName} is ignored";
    }
    
    var serializedName = info.SerializedName ?? info.PropertyName;
    var code = new StringBuilder();
    
    if (property.Type.IsNullable(compilation)) {
        code.AppendLine($"if ({info.PropertyName} != null)");
        code.AppendLine("{");
        code.AppendLine($"    writer.WritePropertyName(\"{serializedName}\");");
        
        if (!string.IsNullOrEmpty(info.CustomConverter)) {
            code.AppendLine($"    // Use custom converter: {info.CustomConverter}");
            code.AppendLine($"    WriteWithConverter({info.PropertyName});");
        } else {
            code.AppendLine($"    writer.WriteValue({info.PropertyName});");
        }
        
        code.AppendLine("}");
    } else {
        code.AppendLine($"writer.WritePropertyName(\"{serializedName}\");");
        code.AppendLine($"writer.WriteValue({info.PropertyName});");
    }
    
    return code.ToString();
}
```

## Advanced Attribute Scenarios

### Custom Attribute Analysis

```csharp
// For custom attributes like:
// [TableName("Users")]
// [Column("user_id", Type = DbType.Int32, IsPrimaryKey = true)]

public class DatabaseMappingAnalyzer {
    public TableInfo AnalyzeClass(INamedTypeSymbol classSymbol, Compilation compilation) {
        var tableInfo = new TableInfo {
            ClassName = classSymbol.Name
        };
        
        // Get table name from TableName attribute
        var tableNameAttr = compilation.GetTypeByMetadataName("MyApp.Attributes.TableNameAttribute");
        if (classSymbol.TryGetAttribute(tableNameAttr, out var tableAttrData) &&
            tableAttrData.ConstructorArguments.Length > 0) {
            tableInfo.TableName = tableAttrData.ConstructorArguments[0].Value?.ToString();
        }
        
        // Analyze properties for column mapping
        foreach (var property in classSymbol.GetProperties()) {
            var columnInfo = AnalyzeColumn(property, compilation);
            if (columnInfo != null) {
                tableInfo.Columns.Add(columnInfo);
            }
        }
        
        return tableInfo;
    }
    
    private ColumnInfo? AnalyzeColumn(IPropertySymbol property, Compilation compilation) {
        var columnAttr = compilation.GetTypeByMetadataName("MyApp.Attributes.ColumnAttribute");
        
        if (!property.TryGetAttribute(columnAttr, out var columnData)) {
            return null;
        }
        
        var columnInfo = new ColumnInfo {
            PropertyName = property.Name,
            PropertyType = property.Type.ToDisplayString()
        };
        
        // Get column name (first constructor argument)
        if (columnData.ConstructorArguments.Length > 0) {
            columnInfo.ColumnName = columnData.ConstructorArguments[0].Value?.ToString();
        }
        
        // Get database type
        if (columnData.TryGetNamedArgument("Type", out var dbTypeValue) &&
            dbTypeValue.Value is int dbTypeInt) {
            columnInfo.DbType = (DbType)dbTypeInt;
        }
        
        // Get primary key flag
        if (columnData.TryGetNamedArgument("IsPrimaryKey", out var pkValue)) {
            columnInfo.IsPrimaryKey = (bool)pkValue.Value!;
        }
        
        return columnInfo;
    }
}

public class TableInfo {
    public string ClassName { get; set; } = string.Empty;
    public string? TableName { get; set; }
    public List<ColumnInfo> Columns { get; set; } = new();
}

public class ColumnInfo {
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string? ColumnName { get; set; }
    public DbType? DbType { get; set; }
    public bool IsPrimaryKey { get; set; }
}
```

## Testing Attribute Analysis

### Unit Test Example

```csharp
[Fact]
public async Task TestAttributeDetection() {
    var code = @"
using System.ComponentModel.DataAnnotations;

public class User {
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Range(18, 120)]
    public int Age { get; set; }
}";

    var compilation = await code.CreateNet8CompilationAsync();
    var userClass = compilation.GetRequiredSymbol("User");
    var nameProperty = userClass.GetMembers("Name").OfType<IPropertySymbol>().First();
    
    // Test Required attribute
    var requiredAttr = compilation.GetRequiredSymbol("System.ComponentModel.DataAnnotations.RequiredAttribute");
    nameProperty.HasAttribute(requiredAttr).Should().BeTrue();
    
    // Test StringLength attribute
    var stringLengthAttr = compilation.GetRequiredSymbol("System.ComponentModel.DataAnnotations.StringLengthAttribute");
    nameProperty.TryGetAttribute(stringLengthAttr, out var attrData).Should().BeTrue();
    
    // Verify constructor argument
    attrData!.ConstructorArguments[0].Value.Should().Be(50);
    
    // Verify named argument
    attrData.TryGetNamedArgument("MinimumLength", out var minLength).Should().BeTrue();
    minLength.Value.Should().Be(2);
}
```

## Performance Tips

### Caching Attribute Symbols

```csharp
public class CachedAttributeAnalyzer {
    private readonly Dictionary<string, INamedTypeSymbol?> _attributeCache = new();
    private readonly Compilation _compilation;
    
    public CachedAttributeAnalyzer(Compilation compilation) {
        _compilation = compilation;
    }
    
    private INamedTypeSymbol? GetAttributeType(string metadataName) {
        if (!_attributeCache.TryGetValue(metadataName, out var attrType)) {
            attrType = _compilation.GetTypeByMetadataName(metadataName);
            _attributeCache[metadataName] = attrType;
        }
        return attrType;
    }
    
    public bool HasRequiredAttribute(ISymbol symbol) {
        var requiredAttr = GetAttributeType("System.ComponentModel.DataAnnotations.RequiredAttribute");
        return requiredAttr != null && symbol.HasAttribute(requiredAttr);
    }
}
```

## Related Topics

- [Symbol Analysis Guide](symbol-analysis.md) - General symbol analysis techniques
- [Nullability Detection](nullability.md) - Working with nullable attributes
- [Collection Types](collections.md) - Collection-specific attributes
- [Testing Your Analyzers](testing.md) - Unit testing attribute analysis
- [API Reference](../api/Albatross.CodeAnalysis.yml) - Complete API documentation