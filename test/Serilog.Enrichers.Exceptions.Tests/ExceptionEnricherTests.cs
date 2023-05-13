using System.Reflection;
using FluentAssertions;
using Serilog.Enrichers.Exceptions.Tests.Support;
using Serilog.Events;

namespace Serilog.Enrichers.Exceptions.Tests;

[TestFixture]
public class ExceptionEnricherTests
{
    [TestCaseSource(typeof(TestCases), nameof(TestCases.GetTestCases))]
    public void TestDriver(TestCase testCase)
    {
        // Setup
        LogEvent? evt = null;

        var log = new LoggerConfiguration()
            .Enrich.WithExceptionDetail(loggerConfigFunc: (config, cept) => config
                .Destructure.ByTransformingException<MyCoolException>(cept, x => new
                {
                    x.MyCoolProp,
                }))
            .WriteTo.Sink(new DelegatingSink(e => evt = e))
            .CreateLogger();

        var objToDestructure = new
        {
            Message = "Hi",
        };

        var exception = GenerateException();

        // Execute
        log.Error(exception, "Here is {@Context}", objToDestructure);

        // Verify
        evt.Properties.Should().BeEquivalentTo(
            new Dictionary<string, LogEventPropertyValue>
            {
                {
                    "Context",
                    new StructureValue(new[]
                    {
                        new LogEventProperty("Message", new ScalarValue("Hi")),
                    })
                },
                {
                    "ExceptionDetail",
                    new StructureValue(new[]
                    {
                        new LogEventProperty("Common", new StructureValue(new[]
                        {
                            new LogEventProperty("Message", new ScalarValue("blah blah blah")),
                            new LogEventProperty("Type", new ScalarValue(typeof(ApplicationException))),
                            new LogEventProperty("InnerException", new StructureValue(Enumerable.Empty<LogEventProperty>())),
                        })),
                        new LogEventProperty("TypeSpecific", new StructureValue(new[]
                        {
                            new LogEventProperty("MyCoolProp", new ScalarValue("What a great prop")),
                        })),
                    })
                },
            },
            options => options
                .UsingSerilogTypeComparisons()
        );
    }

    private Exception GenerateException()
    {
        try
        {
            ThrowMe();
        }
        catch (Exception ex)
        {
            return ex;
        }

        return null;
    }

    private void ThrowMe()
    {
        throw new MyCoolException("blah blah blah") { MyCoolProp = "What a great prop" };
    }
}

public class MyCoolException : Exception
{
    public MyCoolException(string? message, Exception? innerException = null)
        : base(message, innerException)
    { }

    public string MyCoolProp { get; set; }
}