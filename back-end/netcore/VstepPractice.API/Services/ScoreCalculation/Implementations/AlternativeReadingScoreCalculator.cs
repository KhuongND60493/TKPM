using System.Text;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Services.ScoreCalculation.Implementations;

public class AlternativeReadingScoreCalculator : ISectionScoreCalculator
{
    private readonly ILogger<AlternativeReadingScoreCalculator> _logger;

    public AlternativeReadingScoreCalculator(ILogger<AlternativeReadingScoreCalculator> logger)
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
                _logger.LogWarning("No reading parts found");
                return new SectionScore { Score = 0 };
            }

            var partScores = new List<PartScore>();
            var totalCorrect = 0;
            var totalQuestions = 0;
            var detailScores = new Dictionary<string, decimal>();

            foreach (var part in parts)
            {
                var partQuestions = part.OrderBy(q => q.OrderNum).ToList();
                var questionScores = new List<(string QuestionText, bool IsCorrect, bool IsAnswered)>();

                foreach (var question in partQuestions)
                {
                    bool isCorrect = false;
                    bool isAnswered = false;
                    var answer = answers.FirstOrDefault(a => a.QuestionId == question.Id);

                    if (answer?.SelectedOption != null)
                    {
                        isCorrect = answer.SelectedOption.IsCorrect;
                        isAnswered = true;
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Question ID: {QuestionId} in {PartTitle} is unanswered (score: 0)",
                            question.Id, part.Key.Title);
                    }

                    questionScores.Add((
                        $"Q{question.OrderNum} ({question.QuestionText ?? "Question"})",
                        isCorrect,
                        isAnswered));

                    if (isCorrect) totalCorrect++;

                    // Add individual question result to details
                    detailScores[$"{part.Key.Title} - Q{question.OrderNum}"] = new decimal(isCorrect ? 1 : 0);
                }

                var partCorrect = questionScores.Count(q => q.IsCorrect);
                var partTotal = partQuestions.Count;
                totalQuestions += partTotal;

                var partScore = new PartScore
                {
                    PartTitle = part.Key.Title,
                    CorrectAnswers = partCorrect,
                    TotalQuestions = partTotal,
                    Score = partCorrect // Raw score = number of correct answers
                };

                _logger.LogInformation(
                    "{PartTitle} scores - Correct: {CorrectCount}/{TotalQuestions} " +
                    "(Unanswered: {UnansweredCount})",
                    part.Key.Title,
                    partCorrect,
                    partTotal,
                    partQuestions.Count - questionScores.Count(q => q.IsAnswered));

                partScores.Add(partScore);

                // Add part score to details
                var partScoreOn10 = ScoreUtils.ConvertToScale10(partCorrect, partTotal);
                detailScores[$"{part.Key.Title} - Score"] = partScoreOn10;

                // Add part details
                detailScores[$"{part.Key.Title} - Correct"] = partCorrect;
                detailScores[$"{part.Key.Title} - Total"] = partTotal;
                detailScores[$"{part.Key.Title} - Unanswered"] =
                    partTotal - questionScores.Count(q => q.IsAnswered);
            }

            // Convert to scale of 10
            var finalScore = ScoreUtils.ConvertToScale10(totalCorrect, totalQuestions);

            _logger.LogInformation(
                "Total Reading Score: {FinalScore}/10 ({TotalCorrect}/{TotalQuestions} correct)",
                finalScore,
                totalCorrect,
                totalQuestions);

            return new SectionScore
            {
                Score = finalScore,
                DetailScores = detailScores,
                Feedback = GenerateFeedback(partScores, totalCorrect, totalQuestions)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating reading score");
            throw;
        }
    }

    private static string GenerateFeedback(
        List<PartScore> partScores,
        int totalCorrect,
        int totalQuestions)
    {
        var feedback = new StringBuilder();
        feedback.AppendLine("Reading Score Breakdown:");

        foreach (var part in partScores)
        {
            feedback.AppendLine($"\n{part.PartTitle}:");
            feedback.AppendLine($"Correct answers: {part.CorrectAnswers}/{part.TotalQuestions}");

            var partScore = ScoreUtils.ConvertToScale10(part.CorrectAnswers, part.TotalQuestions);
            feedback.AppendLine($"Part score: {partScore:F1}/10");

            var unanswered = part.TotalQuestions -
                (part.CorrectAnswers + (part.TotalQuestions - part.CorrectAnswers));
            if (unanswered > 0)
            {
                feedback.AppendLine($"Warning: {unanswered} question(s) were not answered (counted as incorrect)");
            }
        }

        feedback.AppendLine($"\nTotal correct answers: {totalCorrect}/{totalQuestions}");
        var finalScore = ScoreUtils.ConvertToScale10(totalCorrect, totalQuestions);
        feedback.AppendLine($"Final Reading Score: {finalScore:F1}/10");

        var totalUnanswered = partScores.Sum(p => p.TotalQuestions -
            (p.CorrectAnswers + (p.TotalQuestions - p.CorrectAnswers)));
        if (totalUnanswered > 0)
        {
            feedback.AppendLine($"\nTotal unanswered questions: {totalUnanswered}");
        }

        return feedback.ToString();
    }
}