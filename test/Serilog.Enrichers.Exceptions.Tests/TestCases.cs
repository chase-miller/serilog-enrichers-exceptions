namespace Serilog.Enrichers.Exceptions.Tests;

public record TestCase(string TestName);

public class TestCases
{
    public static IEnumerable<TestCase> GetTestCases()
    {
        yield return new TestCase("First Test");
    }
}