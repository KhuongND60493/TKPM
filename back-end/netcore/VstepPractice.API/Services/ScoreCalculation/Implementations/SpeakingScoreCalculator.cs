using System.Text;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Services.ScoreCalculation.Implementations;

public class SpeakingScoreCalculator : ISectionScoreCalculator
{
    private readonly ISpeakingAssessmentRepository _assessmentRepo;
    private readonly ILogger<SpeakingScoreCalculator> _logger;

    public SpeakingScoreCalculator(
        ISpeakingAssessmentRepository assessmentRepo,
        ILogger<SpeakingScoreCalculator> logger)
    {
        _assessmentRepo = assessmentRepo;
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
                _logger.LogWarning("No speaking parts found");
                return new SectionScore { Score = 0 };
            }

            var partScores = new List<PartScore>();
            decimal totalScore = 0;
            var detailScores = new Dictionary<string, decimal>();

            foreach (var part in parts)
            {
                var partQuestions = part.OrderBy(q => q.OrderNum).ToList();
                decimal partTotalScore = 0;
                var questionScores = new List<(string QuestionText, decimal Score, string? Feedback)>();

                foreach (var question in partQuestions)
                {
                    var answer = answers.FirstOrDefault(a => a.QuestionId == question.Id);
                    decimal questionScore = 0; // Default to 0 for unanswered questions
                    string? feedback = null;

                    if (answer != null)
                    {
                        var assessment = await _assessmentRepo.GetByAnswerIdAsync(
                            answer.Id, cancellationToken);

                        if (assessment != null)
                        {
                            questionScore = assessment.TotalScore;
                            feedback = assessment.DetailedFeedback;
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Answer exists but no assessment found for Question ID: {QuestionId} in {PartTitle}",
                                question.Id, part.Key.Title);
                        }
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Question ID: {QuestionId} in {PartTitle} is unanswered (score: 0/10)",
                            question.Id, part.Key.Title);
                    }

                    // Add to question scores regardless of whether it was answered
                    questionScores.Add((
                        $"Q{question.OrderNum} ({question.QuestionText ?? "Question"})",
                        questionScore,
                        feedback));

                    partTotalScore += questionScore;

                    // Add individual question score to details
                    detailScores[$"{part.Key.Title} - Q{question.OrderNum}"] = questionScore;
                }

                // Calculate part average including zeros for unanswered questions
                var partAverageScore = ScoreUtils.RoundToNearestHalf(
                    partTotalScore / partQuestions.Count);

                _logger.LogInformation(
                    "{PartTitle} scores - Total: {TotalScore}, Questions: {QuestionCount}, Average: {AverageScore}",
                    part.Key.Title, partTotalScore, partQuestions.Count, partAverageScore);

                partScores.Add(new PartScore
                {
                    PartTitle = part.Key.Title,
                    CorrectAnswers = questionScores.Count(qs => qs.Score > 0),
                    TotalQuestions = partQuestions.Count,
                    Score = partAverageScore
                });

                detailScores[$"{part.Key.Title} - Average"] = partAverageScore;
                totalScore += partAverageScore;
            }

            // Calculate final section score (average of all parts)
            var finalScore = ScoreUtils.RoundToNearestHalf(totalScore / parts.Count);

            return new SectionScore
            {
                Score = finalScore,
                DetailScores = detailScores,
                Feedback = GenerateFeedback(partScores)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating speaking score");
            throw;
        }
    }

    private static string GenerateFeedback(List<PartScore> partScores)
    {
        var feedback = new StringBuilder();
        feedback.AppendLine("Speaking Score Breakdown:");

        foreach (var part in partScores)
        {
            feedback.AppendLine($"\n{part.PartTitle}:");
            feedback.AppendLine($"Score: {part.Score:F1}/10");
            feedback.AppendLine($"Questions answered with score > 0: {part.CorrectAnswers}");
            feedback.AppendLine($"Total questions in part: {part.TotalQuestions}");

            if (part.CorrectAnswers < part.TotalQuestions)
            {
                var unanswered = part.TotalQuestions - part.CorrectAnswers;
                feedback.AppendLine($"Warning: {unanswered} question(s) received 0 points");
            }
        }

        var totalScore = ScoreUtils.RoundToNearestHalf(
            partScores.Sum(p => p.Score) / partScores.Count);
        feedback.AppendLine($"\nFinal Speaking Score: {totalScore:F1}/10");

        return feedback.ToString();
    }
}