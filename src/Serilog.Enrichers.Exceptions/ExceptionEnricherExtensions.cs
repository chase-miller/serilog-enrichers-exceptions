using Serilog.Configuration;

namespace Serilog.Enrichers.Exceptions;

public delegate object CommonExceptionPropertiesDestructurer(Exception ex);

public static class ExceptionEnricherExtensions
{
    public static LoggerConfiguration WithExceptionDetail(
        this LoggerEnrichmentConfiguration configuration,
        Func<LoggerConfiguration, CommonExceptionPropertiesDestructurer, LoggerConfiguration>? loggerConfigFunc = null,
        CommonExceptionPropertiesDestructurer? customExceptionDestructuringPolicy = null,
        bool skipDestructuringSetup = false)
    {
        if (skipDestructuringSetup && (customExceptionDestructuringPolicy != null || loggerConfigFunc != null))
        {
            throw new ArgumentException(
                $"{nameof(skipDestructuringSetup)} set to true, but a {nameof(customExceptionDestructuringPolicy)} and/or {nameof(loggerConfigFunc)} value also provided. These values would have been ignored. Either exclude them or don't set {nameof(skipDestructuringSetup)} to true");
        }

        var config = configuration.With(new ExceptionEnricher());

        if (skipDestructuringSetup)
        {
            return config;
        }

        var commonDestructureFunc = customExceptionDestructuringPolicy ?? DestructureCommonExceptionProperties;

        // Make sure we execute consumer-provided destructuring BEFORE our fallback Exception destructurer.
        config = loggerConfigFunc?.Invoke(config, commonDestructureFunc) ?? config;

        config = config
            // A fallback exception destructurer. Any unregistered exception types will be destructured according to this.
            .Destructure.ByTransformingWhereAssignableTo<Exception>(x => new
            {
                Common = commonDestructureFunc(x),
            });

        return config;
    }

    public static object DestructureCommonExceptionProperties(Exception ex)
    {
        return new
        {
            ex.Data,
            ex.HResult,
            ex.Message,
            ex.Source,
            ex.StackTrace,
            ex.TargetSite,
            Type = ex.GetType(),
            ex.InnerException,
        };
    }

    public static LoggerConfiguration ByTransformingExceptionWhere<TValue>(this LoggerDestructuringConfiguration config, Func<Type, bool> destructurePredicate, CommonExceptionPropertiesDestructurer coreExceptionTransformation, Func<TValue, object> transformation)
        where TValue : Exception
    {
        return config.ByTransformingWhere<TValue>(
            destructurePredicate,
            x => x.WithCommon(coreExceptionTransformation, transformation(x)));
    }

    public static LoggerConfiguration ByTransformingException<TValue>(this LoggerDestructuringConfiguration config, CommonExceptionPropertiesDestructurer coreExceptionTransformation, Func<TValue, object> transformation)
        where TValue : Exception
    {
        return config.ByTransformingExceptionWhere(type => type == typeof(TValue), coreExceptionTransformation, transformation);
    }

    public static object WithCommon<TException>(this TException ex, CommonExceptionPropertiesDestructurer coreExceptionTransformation, object typeSpecificTransformedObj)
        where TException : Exception
    {
        return CreateExceptionStructure(coreExceptionTransformation(ex), typeSpecificTransformedObj);
    }

    public static object CreateExceptionStructure(object commonExceptionDestructuredObj, object typeSpecificDestructuredObject)
    {
        return new
        {
            Common = commonExceptionDestructuredObj,
            TypeSpecific = typeSpecificDestructuredObject,
        };
    }
}