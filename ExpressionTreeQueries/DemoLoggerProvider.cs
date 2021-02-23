using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ExpressionTreeQueries
{
    public class DemoLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new DemoLogger();
        }

        public void Dispose()
        { }



        private class DemoLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Debug.WriteLine(formatter(state, exception));
            }
            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}
