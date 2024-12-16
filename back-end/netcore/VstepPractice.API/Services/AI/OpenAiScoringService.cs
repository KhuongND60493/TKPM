using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Text;
using System.Text.Json;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.AI;

namespace VstepPractice.API.Services.AI;

public class OpenAiScoringService : IAiScoringService
{
    private readonly IOpenAIService _openAiService;
    private readonly ILogger<OpenAiScoringService> _logger;
    private readonly OpenAiOptions _options;
    private readonly AsyncRetryPolicy<Result<WritingAssessmentResponse>> _retryPolicy;

    public OpenAiScoringService(
        IOpenAIService openAiService,
        IOptions<OpenAiOptions> options,
        ILogger<OpenAiScoringService> logger)
    {
        _openAiService = openAiService;
        _options = options.Value;
        _logger = logger;

        AssessmentScoreValidator.SetLogger(logger);

        _retryPolicy = Policy<Result<WritingAssessmentResponse>>
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                _options.MaxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(
                        exception.Exception,
                        "Error attempting to assess essay. Retry {RetryCount} of {MaxRetries} after {DelaySeconds} seconds",
                        retryCount,
                        _options.MaxRetries,
                        timeSpan.TotalSeconds);
                });
    }

    public async Task<Result<WritingAssessmentResponse>> AssessEssayAsync(
        EssayScoringTask task,
        CancellationToken cancellationToken = default)
    {
        if (task.SectionType != SectionTypes.Writing)
        {
            return Result.Failure<WritingAssessmentResponse>(
                new Error("AiScoring.InvalidSectionType", "Only writing tasks can be assessed."));
        }

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                using var cts = CancellationTokenSource
                    .CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_options.TimeoutSeconds));

                var systemMessage = @"You are a VSTEP B2 writing examiner. Score the essay based on these criteria:
1. Task Achievement (0-2.5 points): How well the response addresses all points in the task
2. Coherence & Cohesion (0-2.5 points): Text organization and use of linking devices
3. Lexical Resource (0-2.5 points): Vocabulary range and accuracy
4. Grammar Accuracy (0-2.5 points): Range and accuracy of grammatical structures

IMPORTANT: Each criterion MUST be scored between 0 and 2.5 points. DO NOT exceed 2.5 points per criterion.
Maximum total score possible is 10 points (2.5 x 4).

Return response as a JSON object with these exact score ranges:
taskAchievement: decimal (0-2.5)
coherenceCohesion: decimal (0-2.5)
lexicalResource: decimal (0-2.5)
grammarAccuracy: decimal (0-2.5)
detailedFeedback: { strengths: [], weaknesses: [], grammarErrors: [], suggestions: [] }

FOR TESTING:
If you see the string 'TestLLM (point)', please provide a score for each criterion such that the sum of all scores equals (point). Additionally, provide feedback based on that score. Thank you!

Example:
Input: TestLLM 7
Output: taskAchievement: 2.0, coherenceCohesion: 2.0, lexicalResource: 1.0, grammarAccuracy: 2.0
Explanation: 2.0 + 2.0 + 1.0 + 2.0 = 7";

                var userMessage = $@"Please assess the following VSTEP B2 writing task.

Task Title: {task.PassageTitle}

Task Description:
{task.PassageContent}

Question:
{task.QuestionText}

Student's Essay:
{task.Essay}";

                var completionResult = await _openAiService.ChatCompletion.CreateCompletion(
                    new ChatCompletionCreateRequest
                    {
                        Messages = new List<ChatMessage>
                        {
                            ChatMessage.FromSystem(systemMessage),
                            ChatMessage.FromUser(userMessage)
                        },
                        Model = _options.ModelName,
                        Temperature = 0.7f,
                        MaxTokens = 4096
                    },
                    cancellationToken: cts.Token);

                if (!completionResult.Successful)
                {
                    throw new Exception(completionResult.Error?.Message);
                }

                var responseContent = completionResult.Choices.First().Message.Content;
                responseContent = CleanJsonResponse(responseContent);

                try
                {
                    var assessmentData = JsonSerializer.Deserialize<JsonDocument>(responseContent);
                    var root = assessmentData.RootElement;

                    // Log parsed scores
                    _logger.LogInformation(
                        "Parsed scores for answerId {AnswerId}:\n" +
                        "TaskAchievement: {TaskAchievement}\n" +
                        "CoherenceCohesion: {CoherenceCohesion}\n" +
                        "LexicalResource: {LexicalResource}\n" +
                        "GrammarAccuracy: {GrammarAccuracy}",
                        task.AnswerId,
                        root.GetProperty("taskAchievement").GetDecimal(),
                        root.GetProperty("coherenceCohesion").GetDecimal(),
                        root.GetProperty("lexicalResource").GetDecimal(),
                        root.GetProperty("grammarAccuracy").GetDecimal());

                    // Get scores and validate/normalize them
                    var scores = AssessmentScoreValidator.ValidateAndNormalizeScores<WritingAssessmentResponse>(
    ("taskAchievement", root.GetProperty("taskAchievement").GetDecimal()),
    ("coherenceCohesion", root.GetProperty("coherenceCohesion").GetDecimal()),
    ("lexicalResource", root.GetProperty("lexicalResource").GetDecimal()),
    ("grammarAccuracy", root.GetProperty("grammarAccuracy").GetDecimal())
);

                    // Get feedback
                    var feedbackObj = root.GetProperty("detailedFeedback");
                    var feedback = FormatFeedback(feedbackObj, scores);
                    scores.DetailedFeedback = feedback;

                    // Log if original scores were normalized
                    var originalTotal = root.GetProperty("taskAchievement").GetDecimal() +
                                      root.GetProperty("coherenceCohesion").GetDecimal() +
                                      root.GetProperty("lexicalResource").GetDecimal() +
                                      root.GetProperty("grammarAccuracy").GetDecimal();

                    if (originalTotal != scores.TotalScore)
                    {
                        _logger.LogWarning(
                            "AI scores were normalized. Original: {OriginalTotal}, Normalized: {NormalizedTotal}",
                            originalTotal, scores.TotalScore);
                    }

                    return Result.Success(scores);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse AI response: {Response}", responseContent);
                    return Result.Failure<WritingAssessmentResponse>(
                        new Error("AiScoring.InvalidResponse", "Invalid scoring format received."));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assessing essay for answerId {AnswerId}", task.AnswerId);
                return Result.Failure<WritingAssessmentResponse>(
                    new Error("AiScoring.Failed", "Failed to assess essay. Please try again later."));
            }
        });
    }

    public async Task<Result<SpeakingAssessmentResponse>> AssessSpeakingAsync(
        SpeakingAssessmentTask task,
        CancellationToken cancellationToken = default)
    {
        var systemMessage = @"You are a VSTEP B2 speaking examiner. Score the response based on these criteria:
1. Pronunciation (0-2.5 points): How well the response addresses all points in the task
2. Fluency (0-2.5 points): Text organization and use of linking devices
3. Vocabulary (0-2.5 points): Range and accuracy of vocabulary
4. Grammar (0-2.5 points): Range and accuracy of grammar structures

IMPORTANT: Each criterion MUST be scored between 0 and 2.5 points.
Maximum total score possible is 10 points (2.5 x 4).

Return response as a JSON object with these exact properties:
pronunciation: decimal (0-2.5)
fluency: decimal (0-2.5)
vocabulary: decimal (0-2.5)
grammar: decimal (0-2.5)
detailedFeedback: { strengths: [], weaknesses: [], suggestions: [] }";

        var userMessage = $@"Please assess the following VSTEP B2 speaking task.

Task Title: {task.PassageTitle}

Task Description:
{task.PassageContent}

Question:
{task.QuestionText}

Student's Response (Transcribed):
{task.TranscribedText}";

        try
        {
            var completionResult = await _openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest
                {
                    Messages = new List<ChatMessage>
                    {
                        ChatMessage.FromSystem(systemMessage),
                        ChatMessage.FromUser(userMessage)
                    },
                    Model = _options.ModelName,
                    Temperature = 0.7f
                },
                cancellationToken: cancellationToken);

            if (!completionResult.Successful)
            {
                throw new Exception(completionResult.Error?.Message);
            }

            var responseContent = completionResult.Choices.First().Message.Content;
            responseContent = CleanJsonResponse(responseContent);

            var assessmentData = JsonSerializer.Deserialize<JsonDocument>(responseContent);
            var root = assessmentData.RootElement;

            // Validate and normalize scores
            var scores = AssessmentScoreValidator.ValidateAndNormalizeScores<SpeakingAssessmentResponse>(
    ("pronunciation", root.GetProperty("pronunciation").GetDecimal()),
    ("fluency", root.GetProperty("fluency").GetDecimal()),
    ("vocabulary", root.GetProperty("vocabulary").GetDecimal()),
    ("grammar", root.GetProperty("grammar").GetDecimal())
);

            // Get feedback
            var feedbackObj = root.GetProperty("detailedFeedback");
            var feedback = FormatFeedback(feedbackObj, scores);
            scores.DetailedFeedback = feedback;

            return Result.Success(scores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assessing speaking response");
            return Result.Failure<SpeakingAssessmentResponse>(
                new Error("AiScoring.Failed", "Failed to assess speaking response."));
        }
    }

    private static string CleanJsonResponse(string response)
    {
        // Remove markdown code block if present
        if (response.StartsWith("```json"))
        {
            response = response.Replace("```json", "").Replace("```", "").Trim();
        }
        else if (response.StartsWith("```"))
        {
            response = response.Replace("```", "").Trim();
        }
        return response;
    }

    private static string FormatFeedback<T>(
    JsonElement feedbackObj,
    T scores) where T : ISkillAssessmentResponse
    {
        var strengths = string.Join("\n", feedbackObj.GetProperty("strengths")
            .EnumerateArray()
            .Select(x => $"- {x.GetString()}"));
        var weaknesses = string.Join("\n", feedbackObj.GetProperty("weaknesses")
            .EnumerateArray()
            .Select(x => $"- {x.GetString()}"));
        var suggestions = string.Join("\n", feedbackObj.GetProperty("suggestions")
            .EnumerateArray()
            .Select(x => $"- {x.GetString()}"));

        var scoreDetails = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(decimal) && p.Name != nameof(ISkillAssessmentResponse.TotalScore))
            .Select(p => $"{AddSpacesToPascalCase(p.Name)}: {p.GetValue(scores):F1}/2.5");

        return $@"Assessment Scores:
{string.Join("\n", scoreDetails)}
Total Score: {scores.TotalScore:F1}/10

Strengths:
{strengths}

Areas for Improvement:
{weaknesses}

Suggestions:
{suggestions}";
    }

    private static string AddSpacesToPascalCase(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        var newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);

        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                newText.Append(' ');
            newText.Append(text[i]);
        }

        return newText.ToString();
    }

    private static class AssessmentScoreValidator
    {
        private const decimal MAX_CRITERION_SCORE = 2.5m;
        private const decimal MAX_TOTAL_SCORE = 10m;

        private static ILogger? _logger;

        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static T ValidateAndNormalizeScores<T>(params (string name, decimal score)[] scores)
            where T : ISkillAssessmentResponse, new()
        {
            var totalScore = scores.Sum(s => s.score);
            _logger?.LogInformation("Original total score: {TotalScore}", totalScore);

            // Only normalize if total score exceeds MAX_TOTAL_SCORE
            if (totalScore > MAX_TOTAL_SCORE)
            {
                var ratio = MAX_TOTAL_SCORE / totalScore;
                scores = scores.Select(s => (s.name, Math.Round(s.score * ratio, 1))).ToArray();
                _logger?.LogInformation(
                    "Scores normalized with ratio {Ratio}. New scores: {Scores}",
                    ratio,
                    string.Join(", ", scores.Select(s => $"{s.name}: {s.score}")));
            }

            // Ensure each score is between 0 and MAX_CRITERION_SCORE
            var normalizedScores = scores.ToDictionary(
                s => s.name.ToLower(),  // Convert to lowercase for case-insensitive matching
                s => Math.Min(Math.Max(s.score, 0), MAX_CRITERION_SCORE)
            );

            var response = new T();
            var properties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(decimal) &&
                           p.Name != nameof(ISkillAssessmentResponse.TotalScore) &&
                           p.CanWrite);

            foreach (var prop in properties)
            {
                if (normalizedScores.TryGetValue(prop.Name.ToLower(), out var score))
                {
                    prop.SetValue(response, score);
                }
            }

            // Log final scores for debugging
            if (_logger?.IsEnabled(LogLevel.Debug) == true)
            {
                var finalScores = properties
                    .Select(p => $"{p.Name}: {p.GetValue(response)}")
                    .ToList();
                _logger.LogDebug("Final normalized scores: {Scores}", string.Join(", ", finalScores));
            }

            return response;
        }
    }
}