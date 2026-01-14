using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace CacheService.Core.Config.Env;

/// <summary>
/// A service for handling cache's environments.
/// </summary>
public sealed class EnvironmentService : IEnvironmentService
{
    public EnvironmentService(IConfiguration cfg)
    {
        Debug.Assert(cfg != null);
        var varEnv =
            Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var argEnv = cfg["Env"];

        CurrentEnvironment = argEnv == null
            ? CastEnvStrToEnum(varEnv)
            : CastEnvStrToEnum(argEnv);
    }

    public AppEnvironment CurrentEnvironment { get; }

    /// <inheritdoc/>
    public bool IsDevelopment => CurrentEnvironment == AppEnvironment.Dev;

    /// <inheritdoc/>
    public bool IsProduction => CurrentEnvironment == AppEnvironment.Prod;

    /// <inheritdoc/>
    public bool IsStaging => CurrentEnvironment == AppEnvironment.Stg;

    /// <summary>
    /// Method for casting a string representation of an environment to an
    /// enumeration value.
    /// </summary>
    /// <param name="envSymbol">A string representing an environment.</param>
    /// <exception cref="InvalidOperationException">
    /// Env variable contains an invalid/unexpected value.
    /// </exception>
    private static AppEnvironment CastEnvStrToEnum(string? envSymbol)
    {
        return envSymbol?.ToLowerInvariant() switch
        {
            "prod" or "production" => AppEnvironment.Prod,
            "stg" or "staging" => AppEnvironment.Stg,
            "dev" or "development" => AppEnvironment.Dev,
            _ => throw new ArgumentOutOfRangeException(
                nameof(envSymbol),
                envSymbol,
                "Environment variable wasn't set or has unexpected value"
            )
        };
    }
}
