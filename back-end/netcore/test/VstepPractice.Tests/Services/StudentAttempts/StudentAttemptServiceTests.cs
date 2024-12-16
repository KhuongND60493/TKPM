using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System.Linq.Expressions;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.AI;
using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.DTOs.StudentAttempts.Requests;
using VstepPractice.API.Models.DTOs.StudentAttempts.Responses;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;
using VstepPractice.API.Services.AI;
using VstepPractice.API.Services.ScoreCalculation;
using VstepPractice.API.Services.Storage;
using VstepPractice.API.Services.StudentAttempts;
using Xunit.Abstractions;

namespace VstepPractice.Tests.Services.StudentAttempts;

public class StudentAttemptServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEssayScoringQueue> _mockScoringQueue;
    private readonly Mock<ISpeakingAssessmentQueue> _speakingAssessmentQueue;
    private readonly Mock<IVstepScoreCalculator> _mockScoreCalculator;
    private readonly Mock<IFileStorageService> _fileStorageService;
    private readonly Mock<ILogger<StudentAttemptService>> _mockLogger;
    private readonly StudentAttemptService _service;
    private readonly ITestOutputHelper _output;

    public StudentAttemptServiceTests(ITestOutputHelper output)
    {
        _output = output;
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockScoringQueue = new Mock<IEssayScoringQueue>();
        _speakingAssessmentQueue = new Mock<ISpeakingAssessmentQueue>();
        _mockScoreCalculator = new Mock<IVstepScoreCalculator>();
        _mockLogger = new Mock<ILogger<StudentAttemptService>>();
        _fileStorageService = new Mock<IFileStorageService>();

        _service = new StudentAttemptService(
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockScoringQueue.Object,
            _speakingAssessmentQueue.Object,
            _mockScoreCalculator.Object,
            _fileStorageService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task StartAttemptAsync_ExamNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var request = new StartAttemptRequest { UserId = 1, ExamId = 1 };
        _mockUnitOfWork.Setup(x => x.ExamRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exam)null!);

        // Act
        var result = await _service.StartAttemptAsync(request.UserId, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.NotFound, result.Error);
    }

    [Fact]
    public async Task StartAttemptAsync_HasInProgressAttempt_ReturnsError()
    {
        // Arrange
        var request = new StartAttemptRequest { UserId = 1, ExamId = 1 };
        var exam = new Exam { Id = 1, Title = "Test Exam" };

        _mockUnitOfWork.Setup(x => x.ExamRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exam);

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.HasInProgressAttempt(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.StartAttemptAsync(request.UserId, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Attempt.InProgress", result.Error.Code);
    }

    [Fact]
    public async Task StartAttemptAsync_Success_CreatesNewAttempt()
    {
        // Arrange
        var request = new StartAttemptRequest { UserId = 1, ExamId = 1 };
        var exam = new Exam { Id = 1, Title = "Test Exam" };
        var expectedResponse = new AttemptResponse { Id = 1, ExamId = 1 };

        _mockUnitOfWork.Setup(x => x.ExamRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exam);

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.HasInProgressAttempt(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockMapper.Setup(x => x.Map<AttemptResponse>(It.IsAny<StudentAttempt>()))
            .Returns(expectedResponse);

        // Act
        var result = await _service.StartAttemptAsync(request.UserId, request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResponse.Id, result.Value.Id);
        _mockUnitOfWork.Verify(x => x.StudentAttemptRepository.Add(It.IsAny<StudentAttempt>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SubmitAnswerAsync_AttemptNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var request = new SubmitAnswerRequest { UserId = 1, QuestionId = 1 };
        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentAttempt)null!);

        // Act
        var result = await _service.SubmitAnswerAsync(1, 1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.NotFound, result.Error);
    }

    [Fact]
    public async Task SubmitAnswerAsync_AttemptNotInProgress_ReturnsError()
    {
        // Arrange
        var request = new SubmitAnswerRequest { UserId = 1, QuestionId = 1 };
        var attempt = new StudentAttempt
        {
            Id = 1,
            UserId = 1,
            Status = AttemptStatus.Completed
        };

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attempt);

        // Act
        var result = await _service.SubmitAnswerAsync(1, 1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Attempt.NotInProgress", result.Error.Code);
    }

    [Fact]
    public async Task SubmitAnswerAsync_WritingQuestion_QueuesAssessmentTask()
    {
        // Arrange
        var section = new SectionPart
        {
            Id = 1,
            Title = "Writing Section",
            SectionType = SectionTypes.Writing,
            OrderNum = 1
        };

        var question = new Question
        {
            Id = 1,
            Passage = section,
            QuestionText = "Test question",
            OrderNum = 1,
            PassageId = section.Id
        };

        section.Questions = new List<Question> { question };

        var attempt = new StudentAttempt
        {
            Id = 1,
            UserId = 1,
            Status = AttemptStatus.InProgress,
            ExamId = 1,
            Exam = new Exam
            {
                Id = 1,
                Title = "Test Exam",
                SectionParts = new List<SectionPart> { section }
            }
        };

        var request = new SubmitAnswerRequest
        {
            UserId = 1,
            QuestionId = 1,
            EssayAnswer = "Test essay"
        };

        // Create mock repositories
        var mockQuestionRepo = new Mock<IQuestionRepository>();
        var mockWritingAssessmentRepo = new Mock<IWritingAssessmentRepository>();

        // Setup QuestionRepository with includes parameter
        mockQuestionRepo.Setup(repo => repo.FindByIdAsync(
            It.Is<int>(id => id == 1),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Question, object>>[]>()))
            .ReturnsAsync(question);

        mockWritingAssessmentRepo.Setup(repo => repo.GetByAnswerIdAsync(
        It.IsAny<int>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync((WritingAssessment)null);

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.FindByIdAsync(
            It.Is<int>(id => id == 1),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(attempt);

        _mockUnitOfWork.Setup(x => x.AnswerRepository.FindSingleAsync(
            It.IsAny<Expression<Func<Answer, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Answer, object>>[]>()))
            .ReturnsAsync((Expression<Func<Answer, bool>> predicate,
                          CancellationToken token,
                          Expression<Func<Answer, object>>[] includes) => null);

        mockWritingAssessmentRepo.Setup(repo => repo.GetByAnswerIdAsync(
        It.IsAny<int>(),
        It.IsAny<CancellationToken>()))
        .ReturnsAsync((WritingAssessment)null);

        // Setup UnitOfWork
        _mockUnitOfWork.Setup(uow => uow.QuestionRepository)
            .Returns(mockQuestionRepo.Object);

        _mockUnitOfWork.Setup(uow => uow.WritingAssessmentRepository)
        .Returns(mockWritingAssessmentRepo.Object);

        var expectedAnswerResponse = new AnswerResponse
        {
            Id = 1,
            QuestionId = question.Id,
            QuestionText = question.QuestionText,
            EssayAnswer = request.EssayAnswer,
            SectionType = SectionTypes.Writing
        };

        _mockMapper.Setup(m => m.Map<AnswerResponse>(It.IsAny<Answer>()))
            .Returns(expectedAnswerResponse);

        // Add debug logs
        _output.WriteLine($"Request QuestionId: {request.QuestionId}");
        _output.WriteLine($"Question Id in mock: {question.Id}");
        _output.WriteLine($"Section Type: {section.SectionType}");

        // Act
        var result = await _service.SubmitAnswerAsync(1, 1, request, CancellationToken.None);

        // Debug output
        if (!result.IsSuccess)
        {
            _output.WriteLine($"Error Code: {result.Error.Code}");
            _output.WriteLine($"Error Message: {result.Error.Message}");

            mockQuestionRepo.Verify(
                repo => repo.FindByIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<Expression<Func<Question, object>>[]>()),
            Times.Once);

            _output.WriteLine("Question Repository was called: " +
                mockQuestionRepo.Invocations.Count + " times");

            foreach (var invocation in mockQuestionRepo.Invocations)
            {
                _output.WriteLine($"Invocation parameters: {string.Join(", ", invocation.Arguments)}");
            }
        }

        // Assert
        Assert.True(result.IsSuccess, $"Failed with error: {result.Error?.Code} - {result.Error?.Message}");
        Assert.NotNull(result.Value);
        Assert.Equal(expectedAnswerResponse.QuestionId, result.Value.QuestionId);
        Assert.Equal(expectedAnswerResponse.EssayAnswer, result.Value.EssayAnswer);
        Assert.Equal(SectionTypes.Writing, result.Value.SectionType);

        // Verify question repository was called with includes
        mockQuestionRepo.Verify(
            repo => repo.FindByIdAsync(
                It.Is<int>(id => id == 1),
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Question, object>>[]>()),
            Times.Once);

        // Verify queue essay assessment task
        _mockScoringQueue.Verify(
            x => x.QueueScoringTaskAsync(It.Is<EssayScoringTask>(t =>
                t.Essay == request.EssayAnswer &&
                t.SectionType == SectionTypes.Writing)),
            Times.Once);

        // Verify answer repository
        _mockUnitOfWork.Verify(x => x.AnswerRepository.Add(
            It.Is<Answer>(a =>
                a.AttemptId == attempt.Id &&
                a.QuestionId == question.Id &&
                a.EssayAnswer == request.EssayAnswer &&
                a.QuestionOptionId == null)),
            Times.Once);

        // Additional verification for WritingAssessmentRepository
        mockWritingAssessmentRepo.Verify(
            repo => repo.GetByAnswerIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAttemptResultAsync_CompletedAttempt_ReturnsCorrectResult()
    {
        // Arrange
        var section = new SectionPart
        {
            Id = 1,
            Title = "Section 1",
            SectionType = SectionTypes.Reading
        };

        var question = new Question
        {
            Id = 1,
            QuestionText = "Test Question",
            Passage = section,
            OrderNum = 1
        };

        var answer = new Answer
        {
            Id = 1,
            QuestionId = 1,
            Question = question,
            Score = 1
        };

        var attempt = new StudentAttempt
        {
            Id = 1,
            UserId = 1,
            Status = AttemptStatus.Completed,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow,
            Exam = new Exam
            {
                Id = 1,
                Title = "Test Exam",
                SectionParts = new List<SectionPart> { section }
            },
            Answers = new List<Answer> { answer }
        };

        var expectedAnswerResponse = new AnswerResponse
        {
            Id = 1,
            QuestionId = 1,
            QuestionText = "Test Question",
            Score = 1,
            SectionType = SectionTypes.Reading
        };

        var mockScore = new VstepScore
        {
            FinalScore = 8.5m,
            SectionScores = new Dictionary<SectionTypes, SectionScore>
            {
                [SectionTypes.Reading] = new SectionScore
                {
                    Score = 8.5m,
                    DetailScores = new Dictionary<string, decimal>
                    {
                        ["Part 1"] = 8.5m
                    }
                }
            }
        };

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.GetAttemptWithDetailsAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attempt);

        _mockScoreCalculator.Setup(x => x.CalculateScoreAsync(
            It.IsAny<StudentAttempt>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockScore);

        // Setup mapper for individual answer
        _mockMapper.Setup(m => m.Map<AnswerResponse>(It.IsAny<Answer>()))
            .Returns(expectedAnswerResponse);

        // Setup mapper for list of answers
        _mockMapper.Setup(m => m.Map<List<AnswerResponse>>(It.IsAny<IEnumerable<Answer>>()))
            .Returns((IEnumerable<Answer> src) =>
                src.Select(a => _mockMapper.Object.Map<AnswerResponse>(a)).ToList());

        // Act
        var result = await _service.GetAttemptResultAsync(1, 1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var response = result.Value;
        Assert.NotNull(response);
        Assert.Equal(attempt.Id, response.Id);
        Assert.Equal(attempt.Exam.Title, response.ExamTitle);
        Assert.Equal(attempt.StartTime, response.StartTime);
        Assert.Equal(attempt.EndTime!.Value, response.EndTime);

        Assert.NotNull(response.Answers);
        Assert.Equal(1, response.Answers.Count);

        var firstAnswer = response.Answers[0];
        Assert.NotNull(firstAnswer);
        Assert.Equal(expectedAnswerResponse.Id, firstAnswer.Id);
        Assert.Equal(expectedAnswerResponse.QuestionText, firstAnswer.QuestionText);
        Assert.Equal(expectedAnswerResponse.SectionType, firstAnswer.SectionType);

        Assert.Equal(mockScore.FinalScore, response.FinalScore);
        Assert.Equal(mockScore.SectionScores[SectionTypes.Reading].Score,
            response.SectionScores[SectionTypes.Reading]);

        _mockScoreCalculator.Verify(x => x.CalculateScoreAsync(
            It.IsAny<StudentAttempt>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FinishAttemptAsync_AttemptNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var request = new FinishAttemptRequest { UserId = 1, AttemptId = 1 };

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentAttempt)null!);

        // Act
        var result = await _service.FinishAttemptAsync(
            request.UserId, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.NotFound, result.Error);
    }

    [Fact]
    public async Task FinishAttemptAsync_WrongUser_ReturnsNotFoundError()
    {
        // Arrange
        var request = new FinishAttemptRequest { UserId = 2, AttemptId = 1 };
        var attempt = new StudentAttempt
        {
            Id = 1,
            UserId = 1, // Different user
            Status = AttemptStatus.InProgress
        };

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attempt);

        // Act
        var result = await _service.FinishAttemptAsync(
            request.UserId, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(Error.NotFound, result.Error);
    }

    [Fact]
    public async Task FinishAttemptAsync_NotInProgress_ReturnsError()
    {
        // Arrange
        var request = new FinishAttemptRequest { UserId = 1, AttemptId = 1 };
        var attempt = new StudentAttempt
        {
            Id = 1,
            UserId = 1,
            Status = AttemptStatus.Completed // Already completed
        };

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attempt);

        // Act
        var result = await _service.FinishAttemptAsync(
            request.UserId, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Attempt.NotInProgress", result.Error.Code);
    }

    [Fact]
    public async Task FinishAttemptAsync_Success_UpdatesAttemptAndReturnsResult()
    {
        // Arrange
        var request = new FinishAttemptRequest { UserId = 1, AttemptId = 1 };

        var section = new SectionPart
        {
            Id = 1,
            Title = "Section 1",
            SectionType = SectionTypes.Reading
        };

        var question = new Question
        {
            Id = 1,
            QuestionText = "Test Question",
            Passage = section,
            OrderNum = 1
        };

        var answer = new Answer
        {
            Id = 1,
            QuestionId = 1,
            Question = question,
            Score = 1
        };

        var attempt = new StudentAttempt
        {
            Id = 1,
            UserId = 1,
            Status = AttemptStatus.InProgress,
            StartTime = DateTime.UtcNow.AddHours(-1),
            Exam = new Exam
            {
                Id = 1,
                Title = "Test Exam",
                SectionParts = new List<SectionPart> { section }
            },
            Answers = new List<Answer> { answer }
        };

        var expectedAnswerResponse = new AnswerResponse
        {
            Id = 1,
            QuestionId = 1,
            QuestionText = "Test Question",
            Score = 1,
            SectionType = SectionTypes.Reading
        };

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.FindByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attempt);

        _mockUnitOfWork.Setup(x => x.StudentAttemptRepository.GetAttemptWithDetailsAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attempt);

        // Set up score calculator
        var mockScore = new VstepScore
        {
            FinalScore = 8.5m,
            SectionScores = new Dictionary<SectionTypes, SectionScore>
            {
                [SectionTypes.Reading] = new SectionScore
                {
                    Score = 8.5m,
                    DetailScores = new Dictionary<string, decimal>
                    {
                        ["Part 1"] = 8.5m
                    }
                }
            }
        };

        _mockScoreCalculator.Setup(x => x.CalculateScoreAsync(
            It.IsAny<StudentAttempt>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockScore);

        // Setup mapper for individual answer
        _mockMapper.Setup(m => m.Map<AnswerResponse>(It.IsAny<Answer>()))
            .Returns(expectedAnswerResponse);

        // Setup mapper for list of answers
        _mockMapper.Setup(m => m.Map<List<AnswerResponse>>(It.IsAny<IEnumerable<Answer>>()))
            .Returns((IEnumerable<Answer> src) =>
                src.Select(a => _mockMapper.Object.Map<AnswerResponse>(a)).ToList());

        // Act
        var result = await _service.FinishAttemptAsync(
            request.UserId, request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var attemptResult = result.Value;
        Assert.NotNull(attemptResult);
        Assert.Equal(attempt.Id, attemptResult.Id);
        Assert.Equal(attempt.Exam.Title, attemptResult.ExamTitle);
        Assert.Equal(attempt.StartTime, attemptResult.StartTime);
        Assert.NotNull(attemptResult.EndTime);

        Assert.NotNull(attemptResult.Answers);
        Assert.Equal(1, attemptResult.Answers.Count);

        var firstAnswer = attemptResult.Answers[0];
        Assert.NotNull(firstAnswer);
        Assert.Equal(expectedAnswerResponse.Id, firstAnswer.Id);
        Assert.Equal(expectedAnswerResponse.QuestionText, firstAnswer.QuestionText);
        Assert.Equal(SectionTypes.Reading, firstAnswer.SectionType);

        Assert.Equal(mockScore.FinalScore, attemptResult.FinalScore);
        Assert.Equal(mockScore.SectionScores[SectionTypes.Reading].Score,
            attemptResult.SectionScores[SectionTypes.Reading]);

        _mockUnitOfWork.Verify(x => x.StudentAttemptRepository.Update(
            It.Is<StudentAttempt>(a =>
                a.Status == AttemptStatus.Completed &&
                a.EndTime.HasValue)),
            Times.Once);

        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}