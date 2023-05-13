using Serilog.Configuration;

namespace Serilog.Enrichers.Exceptions;

public static class TransformingWhereAssignableToExtension
{
    public static LoggerConfiguration ByTransformingWhereAssignableTo<TValue>(this LoggerDestructuringConfiguration config, Func<TValue, object> transformation)
    {
        return config.ByTransformingWhere(
            theType => theType.IsAssignableTo(typeof(TValue)),
            transformation);
    }
}