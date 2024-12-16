using System.Text;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Services.ScoreCalculation.Implementations;

public class WritingScoreCalculator : ISectionScoreCalculator
{
    private readonly IWritingAssessmentRepository _assessmentRepo;
    private readonly ILogger<WritingScoreCalculator> _logger;

    public WritingScoreCalculator(
        IWritingAssessmentRepository assessmentRepo,
        ILogger<WritingScoreCalculator> logger)
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
            var writingTasks = questions
                .OrderBy(q => q.OrderNum)
                .ToList();

            if (!writingTasks.Any())
            {
                _logger.LogWarning("No writing tasks found");
                return new SectionScore { Score = 0 };
            }

            var taskScores = new List<(string Title, decimal Score, WritingAssessment? Assessment)>();
            var detailScores = new Dictionary<string, decimal>();

            // Process each task, including unanswered ones
            foreach (var task in writingTasks)
            {
                decimal taskScore = 0;
                WritingAssessment? assessment = null;

                var answer = answers.FirstOrDefault(a => a.QuestionId == task.Id);
                if (answer != null)
                {
                    assessment = await _assessmentRepo.GetByAnswerIdAsync(
                        answer.Id, cancellationToken);

                    if (assessment != null)
                    {
                        taskScore = assessment.TotalScore;
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Answer exists but no assessment found for Task ID: {TaskId} in {TaskTitle}",
                            task.Id, task.Passage.Title);
                    }
                }
                else
                {
                    _logger.LogInformation(
                        "Task ID: {TaskId} ({TaskTitle}) is unanswered (score: 0/10)",
                        task.Id, task.Passage.Title);
                }

                taskScores.Add((task.Passage.Title, taskScore, assessment));

                if (assessment != null)
                {
                    detailScores[$"{task.Passage.Title} - Task Achievement"] = assessment.TaskAchievement;
                    detailScores[$"{task.Passage.Title} - Coherence & Cohesion"] = assessment.CoherenceCohesion;
                    detailScores[$"{task.Passage.Title} - Lexical Resource"] = assessment.LexicalResource;
                    detailScores[$"{task.Passage.Title} - Grammar Accuracy"] = assessment.GrammarAccuracy;
                }
                else
                {
                    // Add zero scores for unanswered tasks
                    detailScores[$"{task.Passage.Title} - Task Achievement"] = 0;
                    detailScores[$"{task.Passage.Title} - Coherence & Cohesion"] = 0;
                    detailScores[$"{task.Passage.Title} - Lexical Resource"] = 0;
                    detailScores[$"{task.Passage.Title} - Grammar Accuracy"] = 0;
                }
                detailScores[$"{task.Passage.Title} - Total"] = taskScore;
            }

            _logger.LogInformation(
                "Writing tasks found: {TaskCount}. Scores: {Scores}",
                writingTasks.Count,
                string.Join(", ", taskScores.Select(t => $"{t.Title}: {t.Score}")));

            // Calculate final score based on number of tasks
            decimal finalScore;
            if (writingTasks.Count == 1)
            {
                finalScore = taskScores[0].Score;
                _logger.LogInformation(
                    "Single writing task. Score: {Score}",
                    finalScore);
            }
            else if (writingTasks.Count == 2)
            {
                // Formula: (Task1 + (Task2 x 2))/3
                finalScore = (taskScores[0].Score + (taskScores[1].Score * 2)) / 3;
                _logger.LogInformation(
                    "Two writing tasks. Task1: {Task1Score}, Task2: {Task2Score}, Final: {FinalScore}",
                    taskScores[0].Score, taskScores[1].Score, finalScore);
            }
            else
            {
                _logger.LogWarning(
                    "Unexpected number of writing tasks: {Count}",
                    writingTasks.Count);
                finalScore = taskScores.Average(t => t.Score);
            }

            detailScores["Writing Section Total"] = finalScore;

            return new SectionScore
            {
                Score = ScoreUtils.RoundToNearestHalf(finalScore),
                DetailScores = detailScores,
                Feedback = GenerateFeedback(taskScores, finalScore)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating writing score");
            throw;
        }
    }

    private static string GenerateFeedback(
        List<(string Title, decimal Score, WritingAssessment? Assessment)> taskScores,
        decimal finalScore)
    {
        var feedback = new StringBuilder();
        feedback.AppendLine("Writing Score Breakdown:");

        var answeredTasks = taskScores.Count(t => t.Assessment != null);
        var totalTasks = taskScores.Count;

        foreach (var (title, score, assessment) in taskScores)
        {
            feedback.AppendLine($"\n{title}:");
            if (assessment != null)
            {
                feedback.AppendLine($"- Task Achievement: {assessment.TaskAchievement:F1}/2.5");
                feedback.AppendLine($"- Coherence & Cohesion: {assessment.CoherenceCohesion:F1}/2.5");
                feedback.AppendLine($"- Lexical Resource: {assessment.LexicalResource:F1}/2.5");
                feedback.AppendLine($"- Grammar Accuracy: {assessment.GrammarAccuracy:F1}/2.5");
                feedback.AppendLine($"- Total: {score:F1}/10");
                feedback.AppendLine("\nDetailed Feedback:");
                feedback.AppendLine(assessment.DetailedFeedback);
            }
            else
            {
                feedback.AppendLine("Task not answered - Score: 0/10");
            }
        }

        feedback.AppendLine($"\nFinal Writing Score: {finalScore:F1}/10");

        if (answeredTasks < totalTasks)
        {
            feedback.AppendLine($"\nWarning: {totalTasks - answeredTasks} task(s) received 0 points");
        }

        if (taskScores.Count == 2)
        {
            feedback.AppendLine("\nNote: Final score calculated using formula (Task1 + (Task2 x 2))/3");
        }

        return feedback.ToString();
    }
}