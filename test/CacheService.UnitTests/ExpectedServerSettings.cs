using System.Net;

namespace CacheService.UnitTests;

internal sealed record ExpectedServerSettings(
    IPAddress HostAddress,
    int Port
);
