using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using VstepPractice.API.Models.DTOs.AI;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;
using VstepPractice.API.Services.AI;
using VstepPractice.API.Services.Speech;

namespace VstepPractice.API.Services.BackgroundServices;

public class HybridSpeakingAssessmentBackgroundService : BackgroundService, ISpeakingAssessmentQueue
{
    private readonly Channel<SpeakingAssessmentTask> _taskChannel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HybridSpeakingAssessmentBackgroundService> _logger;
    private readonly IMapper _mapper;

    public HybridSpeakingAssessmentBackgroundService(
        IServiceScopeFactory scopeFactory,
        IMapper mapper,
        ILogger<HybridSpeakingAssessmentBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _taskChannel = Channel.CreateUnbounded<SpeakingAssessmentTask>();
        _mapper = mapper;
    }

    public async Task QueueAssessmentTaskAsync(SpeakingAssessmentTask task)
    {
        await _taskChannel.Writer.WriteAsync(task);
        _logger.LogInformation(
            "Queued speaking assessment task for answerId {AnswerId}",
            task.AnswerId);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var task = await _taskChannel.Reader.ReadAsync(stoppingToken);
                await ProcessSpeakingAssessmentAsync(task, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing speaking assessment");
            }
        }
    }

    private async Task ProcessSpeakingAssessmentAsync(
    SpeakingAssessmentTask task,
    CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var hybridScoringService = scope.ServiceProvider.GetRequiredService<ISpeakingAssessmentService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            // 1. Get answer with details
            var answer = await unitOfWork.AnswerRepository
                .GetAnswerWithDetailsAsync(task.AnswerId, cancellationToken);

            if (answer == null)
            {
                _logger.LogError("Answer not found for answerId {AnswerId}", task.AnswerId);
                return;
            }

            // 2. Check if assessment already exists
            var existingAssessment = await unitOfWork.SpeakingAssessmentRepository
                .FindSingleAsync(x => x.AnswerId == task.AnswerId, cancellationToken);

            // 3. Get hybrid assessment
            var assessmentResult = await hybridScoringService.AssessSpeakingAsync(
                task,
                cancellationToken);

            if (!assessmentResult.IsSuccess)
            {
                _logger.LogError(
                    "Failed to get assessment for answerId {AnswerId}: {Error}",
                    task.AnswerId,
                    assessmentResult.Error.Message);
                return;
            }

            var assessment = assessmentResult.Value;

            // 4. Update or create speaking assessment
            if (existingAssessment != null)
            {
                // Update existing assessment
                _mapper.Map(assessment, existingAssessment);
                existingAssessment.TranscribedText = assessment.RecognizedText;
                existingAssessment.AudioUrl = task.AudioUrl;
                existingAssessment.UpdateAt = DateTime.UtcNow;

                unitOfWork.SpeakingAssessmentRepository.Update(existingAssessment);
                _logger.LogInformation(
                    "Updated existing speaking assessment for answerId {AnswerId}",
                    task.AnswerId);
            }
            else
            {
                // Create new assessment
                var speakingAssessment = _mapper.Map<SpeakingAssessment>(assessment);
                speakingAssessment.AnswerId = task.AnswerId;
                speakingAssessment.AudioUrl = task.AudioUrl;

                unitOfWork.SpeakingAssessmentRepository.Add(speakingAssessment);
                _logger.LogInformation(
                    "Created new speaking assessment for answerId {AnswerId}",
                    task.AnswerId);
            }

            // 5. Update answer score and feedback
            answer.Score = assessment.TotalScore;
            answer.AiFeedback = assessment.DetailedFeedback;
            unitOfWork.AnswerRepository.Update(answer);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully processed speaking assessment for answerId {AnswerId}. " +
                "Scores - Pronunciation: {PronScore}, Fluency: {FluencyScore}, " +
                "Vocabulary: {VocabScore}, Grammar: {GrammarScore}",
                task.AnswerId,
                assessment.PronScore,
                assessment.FluencyScore,
                assessment.VocabularyScore,
                assessment.GrammarScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing speaking assessment for answerId {AnswerId}",
                task.AnswerId);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}