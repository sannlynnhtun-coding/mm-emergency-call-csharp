using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MMEmergencyCall.ResultPattern.Tests.Helpers;

public static class Logger
{
    public static ILogger<T> For<T>() => NullLogger<T>.Instance;
}
