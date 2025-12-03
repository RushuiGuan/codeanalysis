* Always use conditional references to include this package only when targeting .NET Standard 2.0.
For example in your .csproj:

```xml
<ItemGroup>
    <ProjectReference
            Condition="'$(TargetFramework)' == 'netstandard2.0'"
		    Include="..\Albatross.CodeAnalysis.Polyfill\Albatross.CodeAnalysis.Polyfill.csproj" 
            PrivateAssets="All" />
</ItemGroup>
```

* Always multi-target your projects to include a later version of .net such as .net8.0.  The .net8.0 compiler will ensure that the polyfilled
code are correct.