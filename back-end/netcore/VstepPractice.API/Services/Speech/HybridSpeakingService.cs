using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Text;
using System.Text.Json;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.AI;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Services.AI;
using VstepPractice.API.Services.Storage;

namespace VstepPractice.API.Services.Speech;

public class HybridSpeakingService : ISpeakingAssessmentService
{
    private readonly IOpenAIService _openAiService;
    private readonly IAzurePronunciationService _pronService;
    private readonly IFileStorageService _storageService;
    private readonly ILogger<HybridSpeakingService> _logger;
    private readonly OpenAiOptions _options;
    private readonly AsyncRetryPolicy<Result<SpeakingAssessmentResponse>> _retryPolicy;

    public HybridSpeakingService(
        IOpenAIService openAiService,
        IAzurePronunciationService pronService,
        IFileStorageService storageService,
        IOptions<OpenAiOptions> options,
        ILogger<HybridSpeakingService> logger)
    {
        _openAiService = openAiService;
        _pronService = pronService;
        _storageService = storageService;
        _options = options.Value;
        _logger = logger;

        _retryPolicy = Policy<Result<SpeakingAssessmentResponse>>
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                _options.MaxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(
                        exception.Exception,
                        "Error attempting to assess speaking. Retry {RetryCount} of {MaxRetries} after {DelaySeconds} seconds",
                        retryCount,
                        _options.MaxRetries,
                        timeSpan.TotalSeconds);
                });
    }

    public Task<Result<WritingAssessmentResponse>> AssessEssayAsync(
        EssayScoringTask task,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<SpeakingAssessmentResponse>> AssessSpeakingAsync(
        SpeakingAssessmentTask task,
        CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                using var cts = CancellationTokenSource
                    .CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_options.TimeoutSeconds));

                // 1. Get Azure Pronunciation scores
                using var audioStream = await _storageService.DownloadFileAsync(task.AudioUrl);
                var pronResult = await _pronService.AssessAudioAsync(
                    audioStream,
                    task.QuestionText,
                    cts.Token);

                // 2. Get OpenAI assessment for content
                var systemMessage = @$"You are a certified VSTEP B2 speaking examiner. Your task is to evaluate the Transcribed Text of a speaking audio and provide scores (0-10) for each of the criteria below. Then, based on all scores (Vocabulary, Grammar, Topic, Pronunciation, Fluency, Accuracy, Prosody), provide detailed feedback to help the speaker improve their speaking skills.

Scoring Criteria:
1. **Vocabulary**: Evaluate the speaker's effective use of words, variety of vocabulary, and appropriateness of word choice within the context.
2. **Grammar**: Assess the speaker's ability to construct grammatically correct sentences. Consider the frequency and impact of errors on communication.
3. **Topic**: Measure the speaker's understanding, relevance, and engagement with the topic, along with their ability to express thoughts and ideas effectively and the ability to engage with the topic.

4. **Pronunciation**: Judge the clarity and naturalness of the speaker's pronunciation, considering how understandable the speech is.
5. **Fluency**: Assess how smoothly the speaker delivers their speech, focusing on rhythm, speed, and the use of pauses.
6. **Accuracy**: Measure how closely the phonemes match a native speaker's pronunciation.
7. **Prosody**: Evaluate the speaker's use of stress, intonation, rhythm, and speaking speed to make their speech natural and engaging.

**IMPORTANT**:
- Each criterion MUST be scored between 0 and 10 points. DO NOT exceed 10 points per criterion.
- Provide actionable feedback including strengths, weaknesses, and specific suggestions for improvement.

Current pronunciation scores (automatically assessed):
- Pronunciation: {pronResult.PronScore:F1}/10
- Fluency: {pronResult.FluencyScore:F1}/10
- Accuracy: {pronResult.AccuracyScore:F1}/10
- Prosody: {pronResult.ProsodyScore:F1}/10

**Feedback Instructions**:
- Provide strengths and weaknesses for each scoring criterion, including Vocabulary, Grammar, Topic, Pronunciation, Fluency, Accuracy, and Prosody.
- Include actionable suggestions for improvement in each area.

**Output Format**: Return the evaluation result in JSON format as follows:
{{
    ""grammarScore"": decimal,
    ""vocabularyScore"": decimal,
    ""topicScore"": decimal,
    ""feedback"": {{
        ""strengths"": [""...""], // Specific areas where the speaker performed well.
        ""weaknesses"": [""...""], // Areas needing improvement across all criteria.
        ""suggestions"": [""...""] // Actionable recommendations to improve speaking skills.
    }}
}}
";




                var prompt = @$"Please assess the following VSTEP B2 speaking task:
Topic Title: {task.PassageTitle}

Topic Description: {task.PassageContent}

Topic Question: {task.QuestionText}

Student's Response: {pronResult.RecognizedText}
";

                var completionResult = await _openAiService.ChatCompletion.CreateCompletion(
                    new ChatCompletionCreateRequest
                    {
                        Messages = new List<ChatMessage>
                        {
                            ChatMessage.FromSystem(systemMessage),
                            ChatMessage.FromUser(prompt)
                        },
                        Model = _options.ModelName,
                        Temperature = 0.7f,
                    },
                    cancellationToken: cts.Token);

                if (!completionResult.Successful)
                {
                    throw new Exception(completionResult.Error?.Message);
                }

                var responseContent = completionResult.Choices.First().Message.Content;
                responseContent = CleanJsonResponse(responseContent);
                var aiResult = JsonSerializer.Deserialize<JsonDocument>(responseContent)?.RootElement;

                if (aiResult == null)
                {
                    throw new Exception("Failed to parse AI response");
                }

                // 4. Combine results

                // OpenAI scores
                pronResult.GrammarScore = decimal.Parse(aiResult.Value.GetProperty("grammarScore").ToString());
                pronResult.VocabularyScore = decimal.Parse(aiResult.Value.GetProperty("vocabularyScore").ToString());
                pronResult.TopicScore = decimal.Parse(aiResult.Value.GetProperty("topicScore").ToString());

                pronResult.DetailedFeedback = FormatFeedback(pronResult, aiResult.Value.GetProperty("feedback"));


                return Result.Success(pronResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assessing speaking response");
                return Result.Failure<SpeakingAssessmentResponse>(
                    new Error("SpeakingAssessment.Failed", "Failed to assess speaking response"));
            }
        });
    }

    private string FormatFeedback(
        SpeakingAssessmentResponse pronResult,
        JsonElement feedbackElement)
    {
        var feedback = new StringBuilder();

        // Overall scores
        feedback.AppendLine("Speaking Assessment Results:\n");
        feedback.AppendLine("Pronunciation & Fluency Scores:");
        feedback.AppendLine($"- Pronunciation: {pronResult.PronScore:F1}/100");
        feedback.AppendLine($"- Fluency: {pronResult.FluencyScore:F1}/100");
        feedback.AppendLine($"- Accuracy: {pronResult.AccuracyScore:F1}/100");
        feedback.AppendLine($"- Prosody: {pronResult.ProsodyScore:F1}/100");
        feedback.AppendLine($"- Grammar: {pronResult.GrammarScore:F1}/100");
        feedback.AppendLine($"- Vocabulary: {pronResult.VocabularyScore:F1}/100");
        feedback.AppendLine($"- Topic: {pronResult.TopicScore:F1}/100");

        feedback.AppendLine("\nDetailed Feedback:");

        // Add strengths
        feedback.AppendLine("\nStrengths:");
        foreach (var strength in feedbackElement.GetProperty("strengths").EnumerateArray())
        {
            feedback.AppendLine($"✓ {strength.GetString()}");
        }

        // Add areas for improvement
        feedback.AppendLine("\nAreas for Improvement:");
        foreach (var weakness in feedbackElement.GetProperty("weaknesses").EnumerateArray())
        {
            feedback.AppendLine($"• {weakness.GetString()}");
        }

        // Add suggestions
        feedback.AppendLine("\nSuggestions for Practice:");
        foreach (var suggestion in feedbackElement.GetProperty("suggestions").EnumerateArray())
        {
            feedback.AppendLine($"- {suggestion.GetString()}");
        }

        return feedback.ToString();
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
}