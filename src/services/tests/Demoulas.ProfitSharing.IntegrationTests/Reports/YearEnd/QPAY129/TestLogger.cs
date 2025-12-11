using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.QPAY129;

/// <summary>
/// Logger factory that outputs to xUnit test output
/// </summary>
internal sealed class TestLoggerFactory : ILoggerFactory
{
    private readonly ITestOutputHelper _output;
    private bool _disposed;

    public TestLoggerFactory(ITestOutputHelper output)
    {
        _output = output;
    }

    public void AddProvider(ILoggerProvider provider) { }

    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger(_output, categoryName);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}

/// <summary>
/// Logger that writes to xUnit test output
/// </summary>
internal sealed class TestLogger : ILogger
{
    private readonly ITestOutputHelper _output;
    private readonly string _categoryName;

    public TestLogger(ITestOutputHelper output, string categoryName)
    {
        _output = output;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        _output.WriteLine($"[{logLevel}] {_categoryName}: {message}");
        if (exception != null)
        {
            _output.WriteLine($"Exception: {exception}");
        }
    }
}
