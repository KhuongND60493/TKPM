using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.Extensions.Options;
using System.Text.Json;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.AI;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Services.Speech;

public class AzurePronunciationService : IAzurePronunciationService
{
    private readonly SpeechConfig _speechConfig;
    private readonly ILogger<AzurePronunciationService> _logger;

    public AzurePronunciationService(
        IOptions<AzureSpeechOptions> options,
        ILogger<AzurePronunciationService> logger)
    {
        _logger = logger;
        _speechConfig = SpeechConfig.FromSubscription(
            options.Value.SubscriptionKey,
            options.Value.Region);
        _speechConfig.SpeechRecognitionLanguage = "en-US";
    }

    public async Task<SpeakingAssessmentResponse> AssessAudioAsync(
    Stream audioStream,
    string topic,
    CancellationToken cancellationToken = default)
    {
        var assessmentResult = new SpeakingAssessmentResponse();
        PushAudioInputStream? pushStream = null;
        SpeechRecognizer? recognizer = null;

        try
        {
            using var wavStream = await AudioUtils.ConvertM4aToWav(audioStream);
            _logger.LogInformation(
                "Converted audio stream - Length: {Length}",
                wavStream.Length);

            // Create resources
            pushStream = AudioInputStream.CreatePushStream(
                AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1));
            var audioInput = AudioConfig.FromStreamInput(pushStream);
            recognizer = new SpeechRecognizer(_speechConfig, audioInput);

            var pronConfig = new PronunciationAssessmentConfig(
                "",
                GradingSystem.HundredMark,
                Granularity.Word,
                enableMiscue: false);

            pronConfig.EnableProsodyAssessment();
            pronConfig.EnableContentAssessmentWithTopic(topic);
            pronConfig.ApplyTo(recognizer);

            var recognizedTexts = new List<string>();
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // Setup handlers...
            recognizer.Recognized += (s, e) => {
                if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrEmpty(e.Result.Text))
                {
                    try
                    {
                        _logger.LogInformation(
                            "Recognition Event - Text: {Text}",
                            e.Result.Text);

                        recognizedTexts.Add(e.Result.Text);

                        var pronResult = PronunciationAssessmentResult.FromResult(e.Result);

                        if (pronResult != null)
                        {
                            assessmentResult.AccuracyScore = (decimal)pronResult.AccuracyScore * 0.1m;
                            assessmentResult.FluencyScore = (decimal)pronResult.FluencyScore * 0.1m;
                            assessmentResult.PronScore = (decimal)pronResult.PronunciationScore * 0.1m;
                            assessmentResult.ProsodyScore = (decimal)pronResult.ProsodyScore * 0.1m;

                            // Add word-level assessment
                            foreach (var word in pronResult.Words)
                            {
                                var wordDetail = new WordDetail
                                {
                                    Word = word.Word,
                                    AccuracyScore = (decimal)word.AccuracyScore * 0.1m, // Convert to 0-10 scale
                                    ErrorType = word.ErrorType,
                                };

                                if (word.Phonemes != null)
                                {
                                    wordDetail.Phonemes = word.Phonemes.Select(p => new PhonemeDetail
                                    {
                                        Phoneme = p.Phoneme,
                                        AccuracyScore = (decimal)p.AccuracyScore,
                                        Offset = p.Offset,
                                        Duration = p.Duration
                                    }).ToList();
                                }

                                assessmentResult.Words.Add(wordDetail);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing recognition result");
                    }
                }
            };


            recognizer.Recognizing += (s, e) => {
                _logger.LogDebug(
                    "Recognizing speech... Partial text: {Text}",
                    e.Result.Text);
            };

            recognizer.Canceled += (s, e) =>
            {
                if (e.Reason == CancellationReason.Error)
                {
                    _logger.LogError(
                        "Assessment error: {ErrorCode} - {ErrorDetails}",
                        e.ErrorCode,
                        e.ErrorDetails);
                    taskCompletionSource.TrySetException(
                        new Exception($"Assessment canceled: {e.ErrorDetails}"));
                }
                else
                {
                    taskCompletionSource.TrySetResult(true);
                }
            };

            recognizer.SessionStopped += (s, e) =>
            {
                _logger.LogInformation("Assessment session stopped");
                taskCompletionSource.TrySetResult(true);
            };

            // Start recognition
            await recognizer.StartContinuousRecognitionAsync()
                .ConfigureAwait(false);

            // Write audio data with proper cancellation handling
            byte[] buffer = new byte[8192];
            int bytesRead;
            var totalBytesWritten = 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested &&
                       (bytesRead = await wavStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    pushStream.Write(buffer[..bytesRead]);
                    totalBytesWritten += bytesRead;
                    _logger.LogDebug("Wrote {BytesRead} bytes. Total: {Total}", bytesRead, totalBytesWritten);
                }

                _logger.LogInformation("Finished writing audio data. Total: {TotalBytes}", totalBytesWritten);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Audio processing was cancelled");
                throw;
            }
            finally
            {
                // Always close the push stream when done
                pushStream.Close();
                _logger.LogInformation("Push stream closed");
            }

            // Wait for recognition to complete with timeout
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(300));
            using var linkedCts = CancellationTokenSource
                .CreateLinkedTokenSource(timeoutCts.Token, cancellationToken);

            try
            {
                await taskCompletionSource.Task
                    .WaitAsync(linkedCts.Token)
                    .ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Recognition timed out after 300 seconds");
                throw;
            }

            assessmentResult.RecognizedText = string.Join(" ", recognizedTexts);
            return assessmentResult;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error during pronunciation assessment");
            throw;
        }
        finally
        {
            // Cleanup resources
            if (recognizer != null)
            {
                try
                {
                    await recognizer.StopContinuousRecognitionAsync()
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping recognition");
                }
                recognizer.Dispose();
            }

            pushStream?.Dispose();
        }
    }
}