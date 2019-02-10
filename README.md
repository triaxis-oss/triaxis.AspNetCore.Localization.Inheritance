# triaxis.AspNetCore.Localization.Inheritance

There is a [little-documented][netcore-localization] 
feature of the ASP.NET Core Model Metadata model, that the `DisplayAttribute` actually causes lookups via the `IStringLocalizer` service to happen. The localizer is
scoped to the _static type of the model_ used (i.e. not even the declaring type
of the property in question). The documentation also helpfully provides a way
to concentrate the resources in a single file, but I find this approach too extreme.
Multiple resource files are fine by me, but a copy of all resources for every 
class hierarchy is not.

This package is a simple extension of the standard ASP.NET Core DataAnnotations
localization (`Html.DisplayNameFor`, etc.) that allows resource strings
translations to be inherited within model class hierarchy, finishing with a
special `_Shared` resource file (defined in the assembly of the original
Model).

## Usage

Just replace

```C#
services.AddMvc()
    .AddDataAnnotationsLocalization()
    ...
```

with

```C#
services.AddMvc()
    .AddDataAnnotationsLocalizationWithInheritance()
    ...
```

### Using .restext resources instead of .resx

I find the XML-based `.resx` format to be an overkill for translation files.
There has always been a much simpler alternative in the form of `.restext`
files (simple INI-style key-value pairs), it is just missing in the [default
`EmbeddedResource` `Include` glob][netcore-includes] of the .NET Core `.csproj`.
Fixing this is as simple as adding the following to your `.csproj`:

```XML
  <ItemGroup>
    <EmbeddedResource Include="**/*.restext" />
  </ItemGroup>
```

The naming rules, etc., are the same as for `.resx` files.

[netcore-localization]: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.2#dataannotations-localization
[netcore-includes]: https://docs.microsoft.com/en-us/dotnet/core/tools/csproj#default-compilation-includes-in-net-core-projects]

## License

This package is licensed under the [MIT License](./LICENSE.txt)

Copyright &copy; 2019 triaxis s.r.o.
