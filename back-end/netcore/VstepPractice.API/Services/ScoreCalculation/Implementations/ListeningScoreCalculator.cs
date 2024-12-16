using System.Text;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Services.ScoreCalculation.Implementations;

public class ListeningScoreCalculator : ISectionScoreCalculator
{
    private readonly ILogger<ListeningScoreCalculator> _logger;

    public ListeningScoreCalculator(ILogger<ListeningScoreCalculator> logger)
    {
        _logger = logger;
    }

    public virtual async Task<SectionScore> CalculateScoreAsync(
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

            if (!parts.Any())
            {
                _logger.LogWarning("No listening parts found");
                return new SectionScore { Score = 0 };
            }

            var partScores = new List<PartScore>();
            var totalCorrect = 0;
            var totalQuestions = 0;
            var detailScores = new Dictionary<string, decimal>();

            foreach (var part in parts)
            {
                var partQuestions = part.OrderBy(q => q.OrderNum).ToList();
                var questionScores = new List<(string QuestionText, bool IsCorrect)>();

                foreach (var question in partQuestions)
                {
                    bool isCorrect = false;
                    var answer = answers.FirstOrDefault(a => a.QuestionId == question.Id);

                    if (answer?.SelectedOption != null)
                    {
                        isCorrect = answer.SelectedOption.IsCorrect;
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Question ID: {QuestionId} in {PartTitle} is unanswered (score: 0)",
                            question.Id, part.Key.Title);
                    }

                    questionScores.Add((
                        $"Q{question.OrderNum} ({question.QuestionText ?? "Question"})",
                        isCorrect));

                    if (isCorrect) totalCorrect++;

                    // Add individual question score to details
                    detailScores[$"{part.Key.Title} - Q{question.OrderNum}"] = isCorrect ? 1 : 0;
                }

                totalQuestions += partQuestions.Count;

                var partScore = new PartScore
                {
                    PartTitle = part.Key.Title,
                    CorrectAnswers = questionScores.Count(q => q.IsCorrect),
                    TotalQuestions = partQuestions.Count,
                    Score = questionScores.Count(q => q.IsCorrect) // Score = number of correct answers
                };

                _logger.LogInformation(
                    "{PartTitle} scores - Correct: {CorrectCount}/{TotalQuestions}",
                    part.Key.Title,
                    partScore.CorrectAnswers,
                    partScore.TotalQuestions);

                partScores.Add(partScore);
                detailScores[$"{part.Key.Title} - Score"] =
                    ScoreUtils.ConvertToScale10(partScore.CorrectAnswers, partScore.TotalQuestions);
            }

            // Convert to scale of 10
            var finalScore = ScoreUtils.ConvertToScale10(totalCorrect, totalQuestions);

            _logger.LogInformation(
                "Total Listening Score: {Score}/10 ({Correct}/{Total} correct)",
                finalScore, totalCorrect, totalQuestions);

            return new SectionScore
            {
                Score = finalScore,
                DetailScores = detailScores,
                Feedback = GenerateFeedback(partScores, finalScore)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating listening score");
            throw;
        }
    }

    private static string GenerateFeedback(List<PartScore> partScores, decimal finalScore)
    {
        var feedback = new StringBuilder();
        feedback.AppendLine("Listening Score Breakdown:");

        foreach (var part in partScores)
        {
            feedback.AppendLine($"\n{part.PartTitle}:");
            feedback.AppendLine($"Correct answers: {part.CorrectAnswers}/{part.TotalQuestions}");

            var partScore = ScoreUtils.ConvertToScale10(part.CorrectAnswers, part.TotalQuestions);
            feedback.AppendLine($"Part score: {partScore:F1}/10");

            var unanswered = part.TotalQuestions -
                (part.CorrectAnswers + part.TotalQuestions - part.CorrectAnswers);
            if (unanswered > 0)
            {
                feedback.AppendLine($"Warning: {unanswered} question(s) were not answered (counted as incorrect)");
            }
        }

        feedback.AppendLine($"\nFinal Listening Score: {finalScore:F1}/10");

        return feedback.ToString();
    }
}
