using FluentAssertions;
using FluentAssertions.Equivalency;
using Serilog.Events;

namespace Serilog.Enrichers.Exceptions.Tests.Support;

public static class FluentAssertionExtensions
{
    public static EquivalencyAssertionOptions<T> UsingSerilogTypeComparisons<T>(this EquivalencyAssertionOptions<T> options)
    {
        return options
            .Using<ScalarValue>(ctx => ctx.Subject.Value.Should().BeEquivalentTo(ctx.Expectation.Value))
            .WhenTypeIs<ScalarValue>()
            .Using<StructureValue>(ctx => ctx.Subject.Should().BeEquivalentTo(ctx.Expectation))
            .When(oi => oi.RuntimeType == typeof(StructureValue))
        ;
    }
}