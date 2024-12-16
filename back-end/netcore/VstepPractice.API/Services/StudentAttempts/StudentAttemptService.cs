using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.AI;
using VstepPractice.API.Models.DTOs.StudentAttempts.Requests;
using VstepPractice.API.Models.DTOs.StudentAttempts.Responses;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;
using VstepPractice.API.Services.AI;
using VstepPractice.API.Services.ScoreCalculation;
using VstepPractice.API.Services.Storage;
using Exception = System.Exception;

namespace VstepPractice.API.Services.StudentAttempts;

public class StudentAttemptService : IStudentAttemptService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEssayScoringQueue _essayScoringQueue;
    private readonly ISpeakingAssessmentQueue _speakingAssessmentQueue;
    private readonly IVstepScoreCalculator _scoreCalculator;
    private readonly ILogger<StudentAttemptService> _logger;
    private readonly IFileStorageService _storageService;

    public StudentAttemptService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEssayScoringQueue scoringQueue,
        ISpeakingAssessmentQueue assessmentQueue,
        IVstepScoreCalculator scoreCalculator,
        IFileStorageService storageService,
        ILogger<StudentAttemptService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _essayScoringQueue = scoringQueue;
        _speakingAssessmentQueue = assessmentQueue;
        _logger = logger;
        _scoreCalculator = scoreCalculator;
        _storageService = storageService;
    }

    public async Task<Result<List<ExamStudentResponse>>> GetExamAsync(CancellationToken cancellationToken = default)
    {
        var exam = await _unitOfWork.ExamRepository.FindAll().ToListAsync(cancellationToken);
        return Result.Success(exam.Select(_mapper.Map<ExamStudentResponse>).ToList());
    }

    public async Task<Result<AttemptResponse>> StartAttemptAsync(
        int userId,
        StartAttemptRequest request,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var exam = await _unitOfWork.ExamRepository.FindByIdAsync(
                request.ExamId, cancellationToken);
            if (exam == null)
                return Result.Failure<AttemptResponse>(Error.NotFound);
            // Check if user has any in-progress attempts
            var inProgressAttempt = await _unitOfWork.StudentAttemptRepository
                .FindAttemptInProgress(userId, request.ExamId, cancellationToken);
            
            var studentAttempt = new StudentAttempt
            {
                UserId = userId,
                ExamId = request.ExamId,
                Exam = exam,
            };
            if (inProgressAttempt != null)
            {
                // TODO CHECK DURATION
                studentAttempt.Id = inProgressAttempt.Id;
                studentAttempt.Status = AttemptStatus.Started;
            }
            else
            {
                studentAttempt.StartTime = DateTime.UtcNow;
                studentAttempt.Status = AttemptStatus.InProgress;
                studentAttempt.Duration = exam.Duration;
                _unitOfWork.StudentAttemptRepository.Add(studentAttempt);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            var response = _mapper.Map<AttemptResponse>(studentAttempt);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Success(response);
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Failure<AttemptResponse>(new Error("TransactionFailed", e.Message));
        }
    }

    public async Task<Result<AttemptStudentSummaryResponse>> ListAllAttemptAsync(int userId,
        StartAttemptRequest request, CancellationToken cancellationToken = default)
    {
        var exam = await _unitOfWork.ExamRepository.FindByIdAsync(
            request.ExamId, cancellationToken);

        if (exam == null)
            return Result.Failure<AttemptStudentSummaryResponse>(Error.NotFound);
        var inProgressAttempt = await _unitOfWork.StudentAttemptRepository
            .FindAttemptInProgress(userId, request.ExamId, cancellationToken);
        var completedAttempts = await _unitOfWork.StudentAttemptRepository
            .FindAllAttemptCompleted(userId, request.ExamId, cancellationToken);

        var summaryAttemptResponses = completedAttempts?.Any() == true
            ? completedAttempts.Select(_mapper.Map<SummaryAttemptResponse>).ToList()
            : new List<SummaryAttemptResponse>();

        return Result.Success(new AttemptStudentSummaryResponse()
        {
            ExamId = exam.Id,
            ExamTitle = exam.Title,
            ExamDescription = exam.Description,
            Inprocess = inProgressAttempt != null
                ? _mapper.Map<AttemptResponse>(new StudentAttempt
                {
                    UserId = userId,
                    ExamId = request.ExamId,
                    Exam = exam,
                    Id = inProgressAttempt.Id,
                    StartTime = DateTime.UtcNow,
                    Status = AttemptStatus.Started
                })
                : null,
            Attempts = summaryAttemptResponses
        });
    }

    public async Task<Result<AnswerResponse>> SubmitAnswerAsync(
        int userId,
        int attemptId,
        SubmitAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _unitOfWork.StudentAttemptRepository
            .FindByIdAsync(attemptId, cancellationToken);

        if (attempt == null || attempt.UserId != userId)
            return Result.Failure<AnswerResponse>(Error.NotFound);

        if (attempt.Status != AttemptStatus.InProgress)
            return Result.Failure<AnswerResponse>(
                new Error("Attempt.NotInProgress", "This attempt is not in progress."));

        // Get question to verify it exists and get section type
        var question = await _unitOfWork.QuestionRepository
            .FindByIdAsync(request.QuestionId, cancellationToken, q => q.Passage);

        if (question == null)
            return Result.Failure<AnswerResponse>(
                new Error("Question.NotFound", "Question not found."));

        // Check if answer already exists
        var existingAnswer = await _unitOfWork.AnswerRepository
            .FindSingleAsync(a =>
                    a.AttemptId == attemptId &&
                    a.QuestionId == request.QuestionId,
                cancellationToken);

        Answer answer;
        if (existingAnswer != null)
        {
            answer = existingAnswer;
        }
        else
        {
            answer = new Answer
            {
                AttemptId = attemptId,
                QuestionId = request.QuestionId
            };
            _unitOfWork.AnswerRepository.Add(answer);
        }

        // Handle different section types
        switch (question.Passage.SectionType)
        {
            case SectionTypes.Writing:
                answer.EssayAnswer = request.EssayAnswer;
                answer.QuestionOptionId = null; // Explicitly set to null for writing
                answer.Score = null; // Reset score as it will be set by AI
                answer.AiFeedback = null; // Reset feedback as it will be set by AI
                break;

            case SectionTypes.Reading:
            case SectionTypes.Listening:
                if (!request.SelectedOptionId.HasValue)
                {
                    return Result.Failure<AnswerResponse>(
                        new Error("Answer.OptionRequired", "Multiple choice answer requires a selected option."));
                }

                // Verify the selected option exists and belongs to this question
                var selectedOption = await _unitOfWork.QuestionOptions
                    .FindByIdAsync(request.SelectedOptionId.Value, cancellationToken);

                if (selectedOption == null || selectedOption.QuestionId != question.Id)
                {
                    return Result.Failure<AnswerResponse>(
                        new Error("Answer.InvalidOption", "Selected option is not valid for this question."));
                }

                answer.EssayAnswer = null; // Reset essay answer for multiple choice
                answer.QuestionOptionId = request.SelectedOptionId;
                answer.AiFeedback = null;
                // Calculate score immediately for multiple choice
                answer.Score = selectedOption.IsCorrect ? question.Point : 0;
                break;

            default:
                _logger.LogError(
                    "Unsupported section type {SectionType} for question {QuestionId}",
                    question.Passage.SectionType, question.Id);
                throw new NotSupportedException(
                    $"Section type {question.Passage.SectionType} is not supported.");
        }

        if (existingAnswer != null)
        {
            _unitOfWork.AnswerRepository.Update(answer);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<AnswerResponse>(answer);

        // If it's a writing answer, get the assessment if it exists
        if (question.Passage.SectionType == SectionTypes.Writing)
        {
            var assessment = await _unitOfWork.WritingAssessmentRepository
                .GetByAnswerIdAsync(answer.Id, cancellationToken);

            if (assessment != null)
            {
                response.WritingScore = new WritingScoreDetails
                {
                    TaskAchievement = assessment.TaskAchievement,
                    CoherenceCohesion = assessment.CoherenceCohesion,
                    LexicalResource = assessment.LexicalResource,
                    GrammarAccuracy = assessment.GrammarAccuracy
                };
            }

            // Queue for assessment if has essay answer
            if (!string.IsNullOrEmpty(request.EssayAnswer))
            {
                await _essayScoringQueue.QueueScoringTaskAsync(new EssayScoringTask
                {
                    AnswerId = answer.Id,
                    PassageTitle = question.Passage.Title,
                    PassageContent = question.Passage.Content ?? string.Empty,
                    QuestionText = question.QuestionText ?? string.Empty,
                    Essay = request.EssayAnswer,
                    SectionType = question.Passage.SectionType
                });
            }
        }

        return Result.Success(response);
    }

    public async Task<Result<AnswerResponse>> SubmitSpeakingAnswerAsync(
        int userId,
        int attemptId,
        SubmitSpeakingAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.StudentAttemptRepository
            .FindByIdAsync(attemptId, cancellationToken);

        if (attempt == null || attempt.UserId != userId)
            return Result.Failure<AnswerResponse>(Error.NotFound);

        if (attempt.Status != AttemptStatus.InProgress)
            return Result.Failure<AnswerResponse>(
                new Error("Attempt.NotInProgress", "This attempt is not in progress."));

        // Get question to verify
        var question = await _unitOfWork.QuestionRepository
            .FindByIdAsync(request.QuestionId, cancellationToken, q => q.Passage);

        if (question == null || question.Passage.SectionType != SectionTypes.Speaking)
            return Result.Failure<AnswerResponse>(
                new Error("Question.Invalid", "Invalid speaking question."));

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Get existing answer and its speaking assessment
            var answer = await _unitOfWork.AnswerRepository
                .FindSingleAsync(a =>
                        a.AttemptId == attemptId &&
                        a.QuestionId == request.QuestionId,
                    cancellationToken);

            if (answer != null)
            {
                // Check if there's an existing speaking assessment
                var existingAssessment = await _unitOfWork.SpeakingAssessmentRepository
                    .FindSingleAsync(s => s.AnswerId == answer.Id, cancellationToken);

                // Delete old audio file if exists
                if (existingAssessment != null && !string.IsNullOrEmpty(existingAssessment.AudioUrl))
                {
                    try
                    {
                        await _storageService.DeleteFileAsync(existingAssessment.AudioUrl);
                        _logger.LogInformation(
                            "Deleted previous audio file for answerId {AnswerId}",
                            answer.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Failed to delete previous audio file for answerId {AnswerId}",
                            answer.Id);
                    }
                }
            }

            // Upload new audio file
            using var stream = request.AudioFile.OpenReadStream();
            var audioUrl = await _storageService.UploadFileAsync(
                stream,
                request.AudioFile.FileName,
                request.AudioFile.ContentType);

            if (answer == null)
            {
                answer = new Answer
                {
                    AttemptId = attemptId,
                    QuestionId = request.QuestionId
                };
                _unitOfWork.AnswerRepository.Add(answer);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Queue for assessment
            await _speakingAssessmentQueue.QueueAssessmentTaskAsync(new SpeakingAssessmentTask
            {
                AnswerId = answer.Id,
                AudioUrl = audioUrl,
                QuestionText = question.QuestionText ?? string.Empty
            });

            var response = _mapper.Map<AnswerResponse>(answer);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error submitting speaking answer for attemptId {AttemptId}, questionId {QuestionId}",
                attemptId,
                request.QuestionId);
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result<BatchSubmitResponse>> BatchSubmitSpeakingAsync(
        int userId,
        int attemptId,
        BatchSubmitSpeakingRequest request,
        CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.StudentAttemptRepository
            .FindByIdAsync(attemptId, cancellationToken);

        if (attempt == null || attempt.UserId != userId)
            return Result.Failure<BatchSubmitResponse>(Error.NotFound);

        if (attempt.Status != AttemptStatus.InProgress)
            return Result.Failure<BatchSubmitResponse>(
                new Error("Attempt.NotInProgress", "This attempt is not in progress."));

        try
        {
            var duplicateQuestions = request.Answers
                .GroupBy(a => a.QuestionId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateQuestions.Any())
            {
                return Result.Failure<BatchSubmitResponse>(
                    new Error("Questions.Duplicate",
                        $"Duplicate questions found: {string.Join(", ", duplicateQuestions)}"));
            }

            // 1. Validate scope
            var sectionPart = await _unitOfWork.SectionPartRepository
                .FindByIdAsync(request.Scope.SectionPartId, cancellationToken, sp => sp.Questions);

            if (sectionPart == null)
                return Result.Failure<BatchSubmitResponse>(
                    new Error("SectionPart.NotFound", "Section/Part not found"));

            if (sectionPart.Type != request.Scope.Type)
                return Result.Failure<BatchSubmitResponse>(
                    new Error("SectionPart.TypeMismatch", "Section/Part type mismatch"));

            if (sectionPart.SectionType != SectionTypes.Speaking)
                return Result.Failure<BatchSubmitResponse>(
                    new Error("SectionPart.InvalidType", "Must be a speaking section/part"));

            // 2. Get all valid questions for this scope
            var validQuestionIds = await GetValidQuestionIdsForScope(
                sectionPart, request.Scope.Type, cancellationToken);

            // 3. Validate answers are for valid questions
            var invalidQuestions = request.Answers
                .Where(a => !validQuestionIds.Contains(a.QuestionId))
                .ToList();

            if (invalidQuestions.Any())
            {
                return Result.Failure<BatchSubmitResponse>(
                    new Error("Questions.Invalid",
                        $"Questions {string.Join(",", invalidQuestions.Select(q => q.QuestionId))} " +
                        $"do not belong to {request.Scope.Type} {sectionPart.Title}"));
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var processedAnswers = new List<Answer>();
            var validationErrors = new List<AnswerValidationError>();

            foreach (var submission in request.Answers)
            {
                // Validate audio file
                if (submission.AudioFile == null || submission.AudioFile.Length == 0)
                {
                    validationErrors.Add(new AnswerValidationError
                    {
                        QuestionId = submission.QuestionId,
                        Message = "Audio file is required"
                    });
                    continue;
                }

                if (!IsValidAudioFile(submission.AudioFile))
                {
                    validationErrors.Add(new AnswerValidationError
                    {
                        QuestionId = submission.QuestionId,
                        Message = "Invalid audio file format. Only .mp3, .wav, .m4a are supported"
                    });
                    continue;
                }

                try
                {
                    // Get question details
                    var question = await _unitOfWork.QuestionRepository
                        .FindByIdAsync(submission.QuestionId, cancellationToken);

                    if (question == null)
                    {
                        validationErrors.Add(new AnswerValidationError
                        {
                            QuestionId = submission.QuestionId,
                            Message = "Question not found"
                        });
                        continue;
                    }

                    // Get or create answer
                    var answer = await GetOrCreateAnswer(attemptId, submission.QuestionId, cancellationToken);

                    // Check if there's an existing assessment to clean up
                    var existingAssessment = await _unitOfWork.SpeakingAssessmentRepository
                        .GetByAnswerIdAsync(answer.Id, cancellationToken);

                    // Delete old audio file if exists
                    if (existingAssessment != null && !string.IsNullOrEmpty(existingAssessment.AudioUrl))
                    {
                        try
                        {
                            await _storageService.DeleteFileAsync(existingAssessment.AudioUrl);
                            _logger.LogInformation(
                                "Deleted previous audio file for answerId {AnswerId}",
                                answer.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex,
                                "Failed to delete previous audio file for answerId {AnswerId}",
                                answer.Id);
                        }
                    }

                    // Upload new audio file
                    using var stream = submission.AudioFile.OpenReadStream();
                    var audioUrl = await _storageService.UploadFileAsync(
                        stream,
                        submission.AudioFile.FileName,
                        submission.AudioFile.ContentType);

                    // Queue for assessment
                    await _speakingAssessmentQueue.QueueAssessmentTaskAsync(new SpeakingAssessmentTask
                    {
                        AnswerId = answer.Id,
                        AudioUrl = audioUrl,
                        QuestionText = question.QuestionText ?? string.Empty
                    });

                    answer.Score = null; // Reset score as it will be set by assessment
                    answer.AiFeedback = audioUrl; // Reset feedback as it will be set by assessment
                    _unitOfWork.AnswerRepository.Update(answer);

                    processedAnswers.Add(answer);

                    _logger.LogInformation(
                        "Successfully processed speaking answer for questionId {QuestionId}, attemptId {AttemptId}",
                        submission.QuestionId,
                        attemptId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error processing speaking answer for questionId {QuestionId}, attemptId {AttemptId}",
                        submission.QuestionId,
                        attemptId);

                    validationErrors.Add(new AnswerValidationError
                    {
                        QuestionId = submission.QuestionId,
                        Message = "Failed to process speaking answer"
                    });
                }
            }

            if (validationErrors.Any())
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result.Success(new BatchSubmitResponse
                {
                    AttemptId = attemptId,
                    Scope = request.Scope,
                    ValidationErrors = validationErrors
                });
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success(new BatchSubmitResponse
            {
                AttemptId = attemptId,
                SubmittedCount = processedAnswers.Count,
                SubmittedAt = DateTime.UtcNow,
                Scope = request.Scope,
                Scores = new Dictionary<string, decimal>
                {
                    ["Submitted Recordings"] = processedAnswers.Count,
                    ["Status"] = 0 // Pending Assessment
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error batch submitting speaking answers for attemptId {AttemptId}",
                attemptId);
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }


    public async Task<Result<BatchSubmitResponse>> BatchSubmitAnswersAsync(
        int userId,
        int attemptId,
        BatchSubmitAnswersRequest request,
        CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.StudentAttemptRepository
            .FindByIdAsync(attemptId, cancellationToken);

        if (attempt == null || attempt.UserId != userId)
            return Result.Failure<BatchSubmitResponse>(Error.NotFound);

        if (attempt.Status != AttemptStatus.InProgress)
            return Result.Failure<BatchSubmitResponse>(
                new Error("Attempt.NotInProgress", "This attempt is not in progress."));

        try
        {
            var duplicateQuestions = request.Answers
                .GroupBy(a => a.QuestionId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateQuestions.Any())
            {
                return Result.Failure<BatchSubmitResponse>(
                    new Error("Questions.Duplicate",
                        $"Duplicate questions found: {string.Join(", ", duplicateQuestions)}"));
            }

            // 1. Validate scope
            var sectionPart = await _unitOfWork.SectionPartRepository
                .FindByIdAsync(request.Scope.SectionPartId, cancellationToken, sp => sp.Questions);

            if (sectionPart == null)
                return Result.Failure<BatchSubmitResponse>(
                    new Error("SectionPart.NotFound", "Section/Part not found"));

            if (sectionPart.Type != request.Scope.Type)
                return Result.Failure<BatchSubmitResponse>(
                    new Error("SectionPart.TypeMismatch", "Section/Part type mismatch"));

            // 2. Get all valid questions for this scope
            var validQuestionIds = await GetValidQuestionIdsForScope(
                sectionPart, request.Scope.Type, cancellationToken);

            // 3. Validate answers are for valid questions
            var invalidQuestions = request.Answers
                .Where(a => !validQuestionIds.Contains(a.QuestionId))
                .ToList();

            if (invalidQuestions.Any())
            {
                return Result.Failure<BatchSubmitResponse>(
                    new Error("Questions.Invalid",
                        $"Questions {string.Join(",", invalidQuestions.Select(q => q.QuestionId))} " +
                        $"do not belong to {request.Scope.Type} {sectionPart.Title}"));
            }


            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var validationErrors = new List<AnswerValidationError>();
            var processedAnswers = new List<Answer>();
            var writingAnswers = new List<(Answer Answer, Question Question)>();

            foreach (var submission in request.Answers)
            {
                var result = await ProcessAnswer(
                    attemptId, submission, cancellationToken);

                if (result.ValidationError != null)
                    validationErrors.Add(result.ValidationError);
                else if (result.Answer != null)
                {
                    processedAnswers.Add(result.Answer);

                    // Collect writing answers for later processing
                    var question = await _unitOfWork.QuestionRepository
                        .FindByIdAsync(submission.QuestionId, cancellationToken, q => q.Passage);
                    if (question?.Passage.SectionType == SectionTypes.Writing)
                    {
                        writingAnswers.Add((result.Answer, question));
                    }
                }
            }

            // If there are validation errors, rollback
            if (validationErrors.Any())
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result.Success(new BatchSubmitResponse
                {
                    AttemptId = attemptId,
                    SubmittedCount = 0,
                    SubmittedAt = DateTime.UtcNow,
                    Scope = request.Scope,
                    ValidationErrors = validationErrors
                });
            }

            // First save changes to get IDs
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Queue writing answers for AI scoring AFTER getting IDs
            foreach (var (answer, question) in writingAnswers)
            {
                if (answer.Id > 0) // Make sure we have a valid ID
                {
                    await _essayScoringQueue.QueueScoringTaskAsync(new EssayScoringTask
                    {
                        AnswerId = answer.Id,
                        PassageTitle = question.Passage.Title,
                        PassageContent = question.Passage.Content ?? string.Empty,
                        QuestionText = question.QuestionText ?? string.Empty,
                        Essay = answer.EssayAnswer ?? string.Empty,
                        SectionType = question.Passage.SectionType
                    });
                }
                else
                {
                    _logger.LogWarning(
                        "Skipping AI scoring queue for answer with invalid Id (AttemptId: {AttemptId}, QuestionId: {QuestionId})",
                        attemptId,
                        question.Id);
                }
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            

            return Result.Success(new BatchSubmitResponse
            {
                AttemptId = attemptId,
                SubmittedCount = processedAnswers.Count,
                SubmittedAt = DateTime.UtcNow,
                Scope = request.Scope,
                Scores = await CalculatePartialScores(
                    processedAnswers,
                    sectionPart.SectionType,
                    cancellationToken)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error batch submitting answers for attemptId {AttemptId}, scope: {ScopeType}",
                attemptId,
                request.Scope.Type);
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result<AttemptResultResponse>> FinishAttemptAsync(
        int userId,
        FinishAttemptRequest request,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _unitOfWork.StudentAttemptRepository
            .FindByIdAsync(request.AttemptId, cancellationToken);

        if (attempt == null || attempt.UserId != userId)
            return Result.Failure<AttemptResultResponse>(Error.NotFound);

        if (attempt.Status != AttemptStatus.InProgress)
            return Result.Failure<AttemptResultResponse>(
                new Error("Attempt.NotInProgress", "This attempt is not in progress."));

        attempt.EndTime = DateTime.UtcNow;
        attempt.Status = AttemptStatus.Completed;

        _unitOfWork.StudentAttemptRepository.Update(attempt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetAttemptResultAsync(userId, request.AttemptId, cancellationToken);
    }

    public async Task<Result<AttemptResultResponse>> GetAttemptResultAsync(
        int userId,
        int attemptId,
        CancellationToken cancellationToken = default)
    {
        var attempt = await _unitOfWork.StudentAttemptRepository
            .GetAttemptWithDetailsAsync(attemptId, cancellationToken);

        if (attempt == null || attempt.UserId != userId)
            return Result.Failure<AttemptResultResponse>(Error.NotFound);

        if (attempt.Status != AttemptStatus.Completed)
            return Result.Failure<AttemptResultResponse>(
                new Error("Attempt.NotCompleted", "This attempt is not completed."));

        // Calculate scores using VstepScoreCalculator
        var score = await _scoreCalculator.CalculateScoreAsync(attempt, cancellationToken);

        // Map answers and include assessments (writing and speaking)
        var answers = new List<AnswerResponse>();
        foreach (var answer in attempt.Answers)
        {
            var answerResponse = _mapper.Map<AnswerResponse>(answer);

            // Handle writing assessments
            if (answer.Question.Passage.SectionType == SectionTypes.Writing)
            {
                var writingAssessment = await _unitOfWork.WritingAssessmentRepository
                    .GetByAnswerIdAsync(answer.Id, cancellationToken);

                if (writingAssessment != null)
                {
                    answerResponse.WritingScore = new WritingScoreDetails
                    {
                        TaskAchievement = writingAssessment.TaskAchievement,
                        CoherenceCohesion = writingAssessment.CoherenceCohesion,
                        LexicalResource = writingAssessment.LexicalResource,
                        GrammarAccuracy = writingAssessment.GrammarAccuracy
                    };
                }
            }
            // Handle speaking assessments 
            else if (answer.Question.Passage.SectionType == SectionTypes.Speaking)
            {
                var speakingAssessment = await _unitOfWork.SpeakingAssessmentRepository
                    .GetByAnswerIdAsync(answer.Id, cancellationToken);

                if (speakingAssessment != null)
                {
                    answerResponse.SpeakingScore = _mapper.Map<SpeakingScoreDetails>(speakingAssessment);
                }
            }

            answers.Add(answerResponse);
        }


        var result = new AttemptResultResponse
        {
            Id = attempt.Id,
            ExamTitle = attempt.Exam.Title!,
            StartTime = attempt.StartTime,
            EndTime = attempt.EndTime!.Value,
            Answers = answers,
            SectionScores = score.SectionScores.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Score),
            FinalScore = score.FinalScore
        };

        attempt.FinalScore = score.FinalScore; // Assuming FinalScore is a decimal value
        _unitOfWork.StudentAttemptRepository.Update(attempt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result);
    }

    private async Task<HashSet<int>> GetValidQuestionIdsForScope(
        SectionPart sectionPart,
        SectionPartTypes scopeType,
        CancellationToken cancellationToken)
    {
        var validIds = new HashSet<int>();

        if (scopeType == SectionPartTypes.Part ||
            (sectionPart.SectionType == SectionTypes.Speaking && sectionPart.Questions.Any()))
        {
            // For Part scope, only include direct questions
            validIds.UnionWith(sectionPart.Questions.Select(q => q.Id));
        }
        else // Section scope
        {
            // For Section scope, include questions from all child parts
            var allParts = await _unitOfWork.SectionPartRepository
                .GetHierarchyForExamAsync(sectionPart.ExamId, cancellationToken);

            var childParts = allParts
                .Where(p => p.ParentId == sectionPart.Id)
                .ToList();

            foreach (var part in childParts)
            {
                validIds.UnionWith(part.Questions.Select(q => q.Id));
            }
        }

        return validIds;
    }

    private async Task<(Answer? Answer, AnswerValidationError? ValidationError)> ProcessAnswer(
        int attemptId,
        AnswerSubmission submission,
        CancellationToken cancellationToken)
    {
        // Get question with details
        var question = await _unitOfWork.QuestionRepository
            .FindByIdAsync(submission.QuestionId, cancellationToken, q => q.Passage);

        if (question == null)
        {
            return (null, new AnswerValidationError
            {
                QuestionId = submission.QuestionId,
                Message = "Question not found"
            });
        }

        // Get or create answer
        var answer = await _unitOfWork.AnswerRepository
            .FindSingleAsync(a =>
                    a.AttemptId == attemptId &&
                    a.QuestionId == submission.QuestionId,
                cancellationToken);

        var isNewAnswer = false;
        if (answer == null)
        {
            answer = new Answer
            {
                AttemptId = attemptId,
                QuestionId = submission.QuestionId
            };
            isNewAnswer = true;
        }

        // Process based on section type
        switch (question.Passage.SectionType)
        {
            case SectionTypes.Writing:
                if (string.IsNullOrWhiteSpace(submission.EssayAnswer))
                {
                    return (null, new AnswerValidationError
                    {
                        QuestionId = submission.QuestionId,
                        Message = "Essay answer is required"
                    });
                }

                answer.EssayAnswer = submission.EssayAnswer;
                answer.QuestionOptionId = null;
                answer.Score = null;
                answer.AiFeedback = null;

                break;

            case SectionTypes.Reading:
            case SectionTypes.Listening:
                if (!submission.SelectedOptionId.HasValue)
                {
                    return (null, new AnswerValidationError
                    {
                        QuestionId = submission.QuestionId,
                        Message = "Selected option is required"
                    });
                }

                var option = await _unitOfWork.QuestionOptions
                    .FindByIdAsync(submission.SelectedOptionId.Value, cancellationToken);

                if (option == null || option.QuestionId != question.Id)
                {
                    return (null, new AnswerValidationError
                    {
                        QuestionId = submission.QuestionId,
                        Message = "Invalid option selected"
                    });
                }

                answer.EssayAnswer = null;
                answer.QuestionOptionId = option.Id;
                answer.Score = option.IsCorrect ? question.Point : 0;
                break;

            default:
                return (null, new AnswerValidationError
                {
                    QuestionId = submission.QuestionId,
                    Message = $"Section type {question.Passage.SectionType} not supported"
                });
        }

        // Add or Update based on whether it's a new answer
        if (isNewAnswer)
            _unitOfWork.AnswerRepository.Add(answer);
        else
            _unitOfWork.AnswerRepository.Update(answer);

        return (answer, null);
    }

    private async Task<Dictionary<string, decimal>> CalculatePartialScores(
        List<Answer> answers,
        SectionTypes sectionType,
        CancellationToken cancellationToken)
    {
        var scores = new Dictionary<string, decimal>();

        switch (sectionType)
        {
            case SectionTypes.Reading:
            case SectionTypes.Listening:
                // Có thể tính ngay vì là multiple choice
                var totalPoints = answers.Sum(a => a.Question.Point);
                var earnedPoints = answers.Where(a => a.Score.HasValue).Sum(a => a.Score.Value);

                scores.Add("Total Points", totalPoints);
                scores.Add("Earned Points", earnedPoints);
                scores.Add("Percentage", totalPoints > 0 ? (earnedPoints / totalPoints * 100) : 0);
                break;

            case SectionTypes.Writing:
                // Chỉ return số câu đã submit, điểm sẽ có sau khi OpenAI chấm
                scores.Add("Submitted Essays", answers.Count);
                scores.Add("Status", 0); // 0 = Pending Assessment
                break;

            case SectionTypes.Speaking:
                // Tương tự Writing
                scores.Add("Submitted Recordings", answers.Count);
                scores.Add("Status", 0); // 0 = Pending Assessment 
                break;
        }

        return scores;
    }


    private bool IsValidAudioFile(IFormFile file)
    {
        var validExtensions = new[] { ".mp3", ".wav", ".m4a" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return validExtensions.Contains(fileExtension);
    }

    private async Task<Answer> GetOrCreateAnswer(
        int attemptId,
        int questionId,
        CancellationToken cancellationToken)
    {
        var answer = await _unitOfWork.AnswerRepository
            .FindSingleAsync(a =>
                    a.AttemptId == attemptId &&
                    a.QuestionId == questionId,
                cancellationToken);

        if (answer == null)
        {
            answer = new Answer
            {
                AttemptId = attemptId,
                QuestionId = questionId
            };
            _unitOfWork.AnswerRepository.Add(answer);
            await _unitOfWork.SaveChangesAsync(cancellationToken); // Save to get Id
        }

        return answer;
    }
}