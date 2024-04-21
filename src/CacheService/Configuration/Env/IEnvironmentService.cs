namespace CacheService.Configuration.Env;

/// <summary>
/// A service for handling cache's environments.
/// </summary>
internal interface IEnvironmentService
{
    /// <summary>
    /// Cache's current environment, specified by
    /// a cmd arg, env variable or default (based on build configuration).
    /// </summary>
    Environment CurrentEnvironment { get; }
}