namespace VstepPractice.API.DependencyInjection.Extensions;

public static class EnvLoader
{
    public static void LoadEnv(this IConfigurationBuilder builder, string filePath = null)
    {
        // Load .env file
        DotNetEnv.Env.Load(filePath);
        // Add environment variables to configuration
        builder.AddEnvironmentVariables();
    }
}
