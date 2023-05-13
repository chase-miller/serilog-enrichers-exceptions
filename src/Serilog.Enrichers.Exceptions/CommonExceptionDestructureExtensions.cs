using Serilog.Configuration;

namespace Serilog.Enrichers.Exceptions;

public static class CommonExceptionDestructureExtensions
{
    public static LoggerConfiguration CommonExceptions(this LoggerDestructuringConfiguration config, CommonExceptionPropertiesDestructurer coreExceptionTransformation)
    {
        var ct = coreExceptionTransformation;

        return config
            .ByTransformingException<AggregateException>(
                ct,
                ex => new
                {
                    ex.InnerExceptions,
                });
    }
}