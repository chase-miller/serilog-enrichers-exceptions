using Serilog.Core;
using Serilog.Events;

namespace Serilog.Enrichers.Exceptions;

public sealed class ExceptionEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Exception != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ExceptionDetail", logEvent.Exception, true));
        }
    }
}