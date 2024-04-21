using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace CacheService.Configuration;

/// <summary>
/// A secret vault implementation through Azure's KeyVault service.
/// </summary>
internal sealed class AzureKeyVault : ISecretVault
{
    private readonly SecretClient _secretClient;
    
    public AzureKeyVault(IConfiguration cfg)
    {
        var keyVaultName =
            cfg["KeyVaultName"]
            ?? Environment.GetEnvironmentVariable("KEY_VAULT_NAME");

        _secretClient = new SecretClient(
            new Uri($"https://{keyVaultName}.vault.azure.net"),
            //new ClientSecretCredential()
            new DefaultAzureCredential()
        );
    }

    /// <inheritdoc/>
    public bool IsEnabled => false;

    /// <inheritdoc/>
    public async Task<string?> GetSecretAsync(
        string name,
        string? version = default,
        CancellationToken cancellationToken = default)
    {
        var secret = await _secretClient.GetSecretAsync(
            name,
            version,
            cancellationToken
        );
        return secret.HasValue ? secret.Value.Value : null;
    }
}