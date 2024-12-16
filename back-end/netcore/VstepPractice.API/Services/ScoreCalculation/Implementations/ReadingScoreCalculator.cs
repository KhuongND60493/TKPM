using System.Text;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Services.ScoreCalculation.Implementations;

public class ReadingScoreCalculator : ISectionScoreCalculator
{
    private readonly ILogger<ReadingScoreCalculator> _logger;
    private const decimal POINTS_PER_QUESTION = 0.25M; // Mỗi câu 0.25 điểm
    private const decimal POINTS_PER_PART = 2.5M; // Mỗi part 2.5 điểm

    public ReadingScoreCalculator(ILogger<ReadingScoreCalculator> logger)
    {
        _logger = logger;
    }

    public async Task<SectionScore> CalculateScoreAsync(
        IEnumerable<Answer> answers,
        IEnumerable<Question> questions,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var parts = questions
                .GroupBy(q => q.Passage)
                .OrderBy(g => g.Key.OrderNum)
                .ToList();

            var partScores = new List<PartScore>();
            decimal totalScore = 0;

            foreach (var part in parts)
            {
                var partQuestions = part.OrderBy(q => q.OrderNum).ToList();
                var partAnswers = answers
                    .Where(a => partQuestions.Select(q => q.Id).Contains(a.QuestionId))
                    .ToList();

                var correctCount = partAnswers.Count(a =>
                    a.SelectedOption != null && a.SelectedOption.IsCorrect);

                var partScore = correctCount * POINTS_PER_QUESTION;

                partScores.Add(new PartScore
                {
                    PartTitle = part.Key.Title,
                    CorrectAnswers = correctCount,
                    TotalQuestions = partQuestions.Count,
                    Score = partScore
                });

                totalScore += partScore;
            }

            // No need to convert scale as reading is already on scale of 10
            return new SectionScore
            {
                Score = ScoreUtils.RoundToNearestHalf(totalScore),
                DetailScores = partScores.ToDictionary(
                    p => p.PartTitle,
                    p => p.Score),
                Feedback = GenerateFeedback(partScores)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating reading score");
            throw;
        }
    }

    private static string GenerateFeedback(List<PartScore> partScores)
    {
        var feedback = new StringBuilder();
        feedback.AppendLine("Reading Score Breakdown:");

        foreach (var part in partScores)
        {
            feedback.AppendLine(
                $"- {part.PartTitle}: {part.CorrectAnswers}/{part.TotalQuestions} correct" +
                $" ({part.Score:F2}/{POINTS_PER_PART} points)");
        }

        var totalScore = partScores.Sum(p => p.Score);
        feedback.AppendLine($"Total Score: {totalScore:F2}/10");

        return feedback.ToString();
    }
}