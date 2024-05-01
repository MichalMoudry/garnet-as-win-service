using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CacheService.Configuration.Env;

namespace CacheService.Configuration.Azure;

/// <summary>
/// A secret vault implementation through Azure's Key Vault service.
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
        var settings = GetKeyVaultSettings(in cfg);
        _secretClient = new SecretClient(
            new Uri($"https://{settings.Uri}"),//.vault.azure.net
            new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret)
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

    /// <summary>
    /// Method for constructing complete Azure Key Vault settings.
    /// </summary>
    private static AzKeyVaultSettings GetKeyVaultSettings(ref readonly IConfiguration cfg)
    {
        return new AzKeyVaultSettings(
            GetCfgOrEnvValue(cfg["KeyVaultUri"], "KEY_VAULT_URI"),
            GetCfgOrEnvValue(cfg["TenantId"], "TENANT_ID"),
            GetCfgOrEnvValue(cfg["ClientId"], "CLIENT_ID"),
            GetCfgOrEnvValue(cfg["ClientSecret"], "CLIENT_SECRET")
        );
    }

    /// <summary>
    /// Method for selecting a configuration value either from a
    /// <see cref="IConfiguration"/> or an environment variable.
    /// Environment variable servers as a backing/default value.
    /// </summary>
    /// <param name="cfgVal">A value from a app's configuration.</param>
    /// <param name="envVarName">
    /// A key/environment variable name that will be searched if config value is null.
    /// </param>
    private static string? GetCfgOrEnvValue(string? cfgVal, string envVarName)
        => cfgVal ?? Environment.GetEnvironmentVariable(envVarName);
}