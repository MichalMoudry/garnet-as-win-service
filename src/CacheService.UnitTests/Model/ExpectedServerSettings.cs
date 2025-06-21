using System.Net;

namespace CacheService.UnitTests.Model;

internal sealed record ExpectedServerSettings(
    IPAddress HostAddress,
    int Port
);