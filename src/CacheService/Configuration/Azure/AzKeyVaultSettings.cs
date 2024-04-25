namespace CacheService.Configuration.Azure;

/// <summary>
/// A container for all the Azure Key Vault settings.
/// </summary>
internal sealed record AzKeyVaultSettings(
    string? Uri,
    string? TenantId,
    string? ClientId,
    string? ClientSecret
);