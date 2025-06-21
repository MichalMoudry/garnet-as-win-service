namespace CacheService.Configuration;

/// <summary>
/// A vault containing secrets for the cache.
/// </summary>
internal interface ISecretVault
{
    /// <summary>
    /// An information if secret vault is enabled or not.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Get a specified secret from a given key vault.
    /// </summary>
    Task<string?> GetSecretAsync(
        string name,
        string? version = null,
        CancellationToken cancellationToken = default
    );
}