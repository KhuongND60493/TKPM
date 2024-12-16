using VstepPractice.API.Common.Enums;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Services.ScoreCalculation.Implementations;

public class VstepScoreCalculator : IVstepScoreCalculator
{
    private readonly IDictionary<SectionTypes, ISectionScoreCalculator> _calculators;
    private readonly ILogger<VstepScoreCalculator> _logger;

    public VstepScoreCalculator(
        ISectionScoreCalculator listeningCalculator,
        ISectionScoreCalculator readingCalculator,
        ISectionScoreCalculator writingCalculator,
        ISectionScoreCalculator speakingCalculator,
        ILogger<VstepScoreCalculator> logger)
    {
        _logger = logger;
        _calculators = new Dictionary<SectionTypes, ISectionScoreCalculator>
        {
            [SectionTypes.Listening] = listeningCalculator,
            [SectionTypes.Reading] = readingCalculator,
            [SectionTypes.Writing] = writingCalculator,
            [SectionTypes.Speaking] = speakingCalculator
        };
    }

    public async Task<VstepScore> CalculateScoreAsync(
        StudentAttempt attempt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sectionScores = new Dictionary<SectionTypes, SectionScore>();

            var sectionParts = attempt.Exam.SectionParts
                .GroupBy(s => s.SectionType)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var (sectionType, sections) in sectionParts)
            {
                if (!_calculators.TryGetValue(sectionType, out var calculator))
                {
                    _logger.LogWarning(
                        "No calculator found for section type: {SectionType}",
                        sectionType);
                    continue;
                }

                var questions = sections.SelectMany(s => s.Questions).ToList();

                if (!questions.Any())
                    continue;

                var score = await calculator.CalculateScoreAsync(
                    attempt.Answers,
                    questions,
                    cancellationToken);

                sectionScores[sectionType] = score;
            }

            var finalScore = ScoreUtils.CalculateAverageScore(
                sectionScores.Values.Select(s => s.Score).ToArray());

            return new VstepScore
            {
                FinalScore = finalScore,
                SectionScores = sectionScores
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error calculating VSTEP score for attempt {AttemptId}",
                attempt.Id);
            throw;
        }
    }
}