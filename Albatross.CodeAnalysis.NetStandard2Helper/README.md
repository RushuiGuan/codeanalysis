This projects exists because Roslyn analyzers only support .NET Standard 2.0.

This project will provide missing Attributes and APIs that are available in .NET Standard 2.1+ but not in .NET Standard 2.0.

Always use conditional references to include this package only when targeting .NET Standard 2.0.
For example in your .csproj:

```xml
<ItemGroup Condition="'$(TargetFramework)' == 'netstandard2.0'">
  <PackageReference Include="Albatross.CodeAnalysis.NetStandard2Helper" Version="1.0.0" />
</ItemGroup>
```