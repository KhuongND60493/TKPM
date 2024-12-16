using AutoMapper;
using System.Threading.Channels;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.DTOs.AI;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;
using VstepPractice.API.Services.AI;

namespace VstepPractice.API.Services.BackgroundServices;

public class EssayScoringBackgroundService : BackgroundService, IEssayScoringQueue
{
    private readonly Channel<EssayScoringTask> _taskChannel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EssayScoringBackgroundService> _logger;
    private readonly IMapper _mapper;

    public EssayScoringBackgroundService(
        IServiceScopeFactory scopeFactory,
        IMapper mapper,
        ILogger<EssayScoringBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
        _logger = logger;
        _taskChannel = Channel.CreateUnbounded<EssayScoringTask>(
            new UnboundedChannelOptions { SingleReader = true });
    }

    public async Task QueueScoringTaskAsync(EssayScoringTask task)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));

        if (task.SectionType != SectionTypes.Writing)
        {
            _logger.LogWarning("Attempted to queue non-writing task for scoring. SectionType: {SectionType}", task.SectionType);
            return;
        }

        await _taskChannel.Writer.WriteAsync(task);
        _logger.LogInformation("Queued writing assessment task for answerId {AnswerId}", task.AnswerId);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var task = await _taskChannel.Reader.ReadAsync(stoppingToken);
                await ProcessScoringTaskAsync(task, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing essay scoring task");
            }
        }
    }

    private async Task ProcessScoringTaskAsync(
        EssayScoringTask task,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var aiScoringService = scope.ServiceProvider.GetRequiredService<IAiScoringService>();
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

            // 2. Check if assessment exists
            var existingAssessment = await unitOfWork.WritingAssessmentRepository
                .GetByAnswerIdAsync(task.AnswerId, cancellationToken);

            // 3. Get AI assessment
            var assessmentResult = await aiScoringService.AssessEssayAsync(
                task,
                cancellationToken);

            if (!assessmentResult.IsSuccess)
            {
                _logger.LogError(
                    "Failed to get AI assessment for answerId {AnswerId}: {Error}",
                    task.AnswerId,
                    assessmentResult.Error.Message);
                return;
            }

            var assessment = assessmentResult.Value;

            // 4. Update or create writing assessment using AutoMapper
            if (existingAssessment != null)
            {
                // Update existing assessment
                _mapper.Map(assessment, existingAssessment);
                unitOfWork.WritingAssessmentRepository.Update(existingAssessment);

                _logger.LogInformation(
                    "Updated existing writing assessment for answerId {AnswerId}",
                    task.AnswerId);
            }
            else
            {
                // Create new assessment
                var writingAssessment = _mapper.Map<WritingAssessment>(assessment);
                writingAssessment.AnswerId = task.AnswerId;

                unitOfWork.WritingAssessmentRepository.Add(writingAssessment);

                _logger.LogInformation(
                    "Created new writing assessment for answerId {AnswerId}",
                    task.AnswerId);
            }

            // 5. Update answer score and feedback
            answer.Score = assessment.TotalScore;
            answer.AiFeedback = assessment.DetailedFeedback;
            unitOfWork.AnswerRepository.Update(answer);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully processed writing assessment for answerId {AnswerId}. " +
                "Scores - Task: {TaskScore}, Coherence: {CoherenceScore}, " +
                "Lexical: {LexicalScore}, Grammar: {GrammarScore}",
                task.AnswerId,
                assessment.TaskAchievement,
                assessment.CoherenceCohesion,
                assessment.LexicalResource,
                assessment.GrammarAccuracy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing writing assessment for answerId {AnswerId}",
                task.AnswerId);
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}