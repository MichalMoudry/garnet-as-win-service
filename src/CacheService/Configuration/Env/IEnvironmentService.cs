namespace CacheService.Configuration.Env;

/// <summary>
/// A service for handling cache's environments.
/// </summary>
public interface IEnvironmentService
{
    /// <summary>
    /// Cache's current environment, specified by
    /// a cmd arg, env variable or default (based on build configuration).
    /// </summary>
    AppEnvironment CurrentEnvironment { get; }

    /// <summary>
    /// Method for checking if the current environment set as production environment.
    /// </summary>
    bool IsEnvProduction();

    /// <summary>
    /// Method for checking if the current environment set as staging environment.
    /// </summary>
    bool IsEnvStaging();

    /// <summary>
    /// Method for checking if the current environment set as development environment.
    /// </summary>
    bool IsEnvDevelopment();
}