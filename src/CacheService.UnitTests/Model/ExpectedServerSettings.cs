namespace CacheService.UnitTests.Model;

internal sealed record ExpectedServerSettings(
    string HostAddress,
    int Port
);