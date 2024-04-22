namespace CacheService.Configuration.Env;

/// <summary>
/// A service for handling cache's environments.
/// </summary>
internal sealed class EnvironmentService : IEnvironmentService
{
    public EnvironmentService(IConfiguration cfg)
    {
        var dotnetEnv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        var aspEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var argEnv = cfg["Env"];

        CurrentEnvironment = argEnv == null
            ? CastEnvStrToEnum(dotnetEnv ?? aspEnv)
            : CastEnvStrToEnum(argEnv);
    }

    public AppEnvironment CurrentEnvironment { get; }

    /// <inheritdoc/>
    public bool IsEnvDevelopment() => CurrentEnvironment == AppEnvironment.Dev;

    /// <inheritdoc/>
    public bool IsEnvProduction() => CurrentEnvironment == AppEnvironment.Prod;

    /// <inheritdoc/>
    public bool IsEnvStaging() => CurrentEnvironment == AppEnvironment.Stg;

    /// <summary>
    /// Method for casting a string representation of an environment to an enumeration value.
    /// </summary>
    /// <param name="envSymbol">A string representing an environment.</param>
    /// <exception cref="InvalidOperationException">Env variable contains an invalid/unexpected value.</exception>
    private static AppEnvironment CastEnvStrToEnum(string? envSymbol)
    {
        return envSymbol?.ToLowerInvariant() switch
        {
            "prod" or "production" => AppEnvironment.Prod,
            "stg" or "staging" => AppEnvironment.Stg,
            "dev" or "development" => AppEnvironment.Dev,
            _ => throw new InvalidOperationException(
                "Environment variable wasn't set or has unexpected value"
            )
        };
    }
}