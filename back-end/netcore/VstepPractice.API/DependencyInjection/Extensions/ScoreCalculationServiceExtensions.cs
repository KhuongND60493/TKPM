using VstepPractice.API.Services.ScoreCalculation.Implementations;
using VstepPractice.API.Services.ScoreCalculation;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.DependencyInjection.Extensions;

public static class ScoreCalculationServiceExtensions
{
    public static IServiceCollection AddScoreCalculationServices(
        this IServiceCollection services)
    {
        // Listening Calculator
        services.AddScoped<ISectionScoreCalculator, ListeningScoreCalculator>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<ListeningScoreCalculator>>();
            return new ListeningScoreCalculator(logger);
        });

        // Reading Calculator
        services.AddScoped<ISectionScoreCalculator, AlternativeReadingScoreCalculator>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<AlternativeReadingScoreCalculator>>();
            return new AlternativeReadingScoreCalculator(logger);
        });

        // Writing Calculator
        services.AddScoped<ISectionScoreCalculator, WritingScoreCalculator>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<WritingScoreCalculator>>();
            var assessmentRepo = sp.GetRequiredService<IWritingAssessmentRepository>();
            return new WritingScoreCalculator(assessmentRepo, logger);
        });

        // Add Speaking Calculator
        services.AddScoped<ISectionScoreCalculator, SpeakingScoreCalculator>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<SpeakingScoreCalculator>>();
            var assessmentRepo = sp.GetRequiredService<ISpeakingAssessmentRepository>();
            return new SpeakingScoreCalculator(assessmentRepo, logger);
        });

        // Main VstepScoreCalculator
        services.AddScoped<IVstepScoreCalculator>(sp =>
        {
            var calculators = sp.GetServices<ISectionScoreCalculator>().ToList();
            var logger = sp.GetRequiredService<ILogger<VstepScoreCalculator>>();

            var listening = calculators.First(c => c is ListeningScoreCalculator);
            var reading = calculators.First(c => c is AlternativeReadingScoreCalculator);
            var writing = calculators.First(c => c is WritingScoreCalculator);
            var speaking = calculators.First(c => c is SpeakingScoreCalculator);

            return new VstepScoreCalculator(listening, reading, writing, speaking, logger);
        });

        return services;
    }
}
