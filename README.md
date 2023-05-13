# Serilog.Enrichers.Exceptions

A Serilog enricher that adds an `ExceptionDetail` property. This is heavily inspired by the excellent [Serilog.Exceptions](https://github.com/RehanSaeed/Serilog.Exceptions) project. It differs fundamentally in that it relies entirely on Serilog's built-in destructuring hooks to destructure a given exception.

### Getting started

Install _Serilog.Enrichers.Exceptions_ into your .NET project:

```powershell
> dotnet add package {update-me}
```

Register the enricher with Serilog:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetail(loggerConfigFunc: (config, cept) => config
        // --- The following are standard Serilog LoggerConfiguration registrations. You can choose to do this outside this method if you wish.
        .Destructure.AllDictionaryTypesAsStructured() // Ensure Data prop outputs as an object over an array of KVPs
        .Destructure.CommonExceptions(cet)            // Handle common well-known exceptions
    .CreateLogger();
```

By default logs will include an `ExceptionDetail` property with the following structure:

```typescript
interface ExceptionDetail {
    Common: {
      Data: IDictionary;
      HResult: number;
      Message: string;
      Source: string;
      StackTrace: string;
      TargetSite: string;
      Type: string;
      InnerException: Exception;
    };
    TypeSpecific?: any;
}
```

Any types you register with `ByTransformingException` as shown below will populate the `TypeSpecific` property according to the transformation provided.

### Customization

Register your own exception destructuring:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetail(loggerConfigFunc: (config, cept) => config
        .Destructure.AllDictionaryTypesAsStructured()
        .Destructure.CommonExceptions(cet)
        
        // --- Add registrations for whatever additional exception types you like
        // ByTransformingException ensures a consistent format for all exception types
        .Destructure.ByTransformingException<MyCoolException>(cept, x => new
        {
            x.MyCoolProp,
        }))
        // Or destructure exception types as you would any other type. You have complete control.
        .Destructure.ByTransforming<MyOtherException>(x => new
        {
            x.SomeProp
        })
        // Use any Serilog destructuring hook you like, including any IDestructuringPolicy instance
        .Destructure.With(new MyDestructuringPolicy())
    .CreateLogger();
```

Let Serilog destructure using its built-in mechanism (reflection?):

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetail(skipDestructuringSetup: true)
    .CreateLogger();
```