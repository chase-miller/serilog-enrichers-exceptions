# Serilog.Enrichers.Exceptions

A Serilog enricher that adds an `ExceptionDetail` property. This is heavily inspired by the excellent [Serilog.Exceptions](https://github.com/RehanSaeed/Serilog.Exceptions) project. It differs fundamentally in that it relies entirely on Serilog's built-in destructuring hooks to destructure a given exception.

### Getting started

Install _Serilog.Enrichers.Exceptions_ into your .NET project:

```powershell
> dotnet add package {update-me}
```

Point the logger to Seq:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetail(loggerConfigFunc: (config, cept) => config
        // The following are standard LoggerConfiguration calls
        .Destructure.AllDictionaryTypesAsStructured() // Ensure Data prop outputs as an object over an array of KVPs
        .Destructure.CommonExceptions(cet)            // Handle common well-known exceptions
        // Add registrations for whatever additional exception types you like
        .Destructure.ByTransformingException<MyCoolException>(cept, x => new
        {
            x.MyCoolProp,
        }))
    .CreateLogger();
```

And use the Serilog logging methods to associate named properties with log events:

```csharp
Log.Error("Failed to log on user {ContactId}", contactId);
```