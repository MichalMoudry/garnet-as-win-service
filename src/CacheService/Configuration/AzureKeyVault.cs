using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CacheService.Configuration.Env;

namespace CacheService.Configuration;

/// <summary>
/// A secret vault implementation through Azure's KeyVault service.
/// </summary>
internal sealed class AzureKeyVault : ISecretVault
{
    private readonly SecretClient? _secretClient;
    
    public AzureKeyVault(IConfiguration cfg, IEnvironmentService envService)
    {
        if (envService.IsEnvDevelopment())
        {
            return;
        }
        var keyVaultName =
            cfg["KeyVaultName"]
            ?? Environment.GetEnvironmentVariable("KEY_VAULT_NAME");

        _secretClient = new SecretClient(
            new Uri($"https://{keyVaultName}.vault.azure.net"),
            //new ClientSecretCredential()
            new DefaultAzureCredential()
        );
        IsEnabled = true;
    }

    /// <inheritdoc/>
    public bool IsEnabled { get; }

    /// <inheritdoc/>
    public async Task<string?> GetSecretAsync(
        string name,
        string? version = default,
        CancellationToken cancellationToken = default)
    {
        if (_secretClient == null)
        {
            throw new InvalidOperationException(
                "Secret client wasn't initialized"
            );
        }

        var secret = await _secretClient.GetSecretAsync(
            name,
            version,
            cancellationToken
        );
        return secret.HasValue ? secret.Value.Value : null;
    }
}