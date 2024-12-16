using Microsoft.Extensions.Logging;
using Moq;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Services.ScoreCalculation.Implementations;
using VstepPractice.API.Services.ScoreCalculation;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Tests.Services.ScoreCalculation;

public class VstepScoreCalculatorTests
{
    private readonly Mock<ISectionScoreCalculator> _mockListeningCalculator;
    private readonly Mock<ISectionScoreCalculator> _mockReadingCalculator;
    private readonly Mock<ISectionScoreCalculator> _mockWritingCalculator;
    private readonly Mock<ILogger<VstepScoreCalculator>> _mockLogger;
    private readonly VstepScoreCalculator _calculator;

    public VstepScoreCalculatorTests()
    {
        // Create mocks
        _mockListeningCalculator = new Mock<ISectionScoreCalculator>();
        _mockReadingCalculator = new Mock<ISectionScoreCalculator>();
        _mockWritingCalculator = new Mock<ISectionScoreCalculator>();
        _mockLogger = new Mock<ILogger<VstepScoreCalculator>>();

        // Create calculator
        _calculator = new VstepScoreCalculator(
            _mockListeningCalculator.Object,
            _mockReadingCalculator.Object,
            _mockWritingCalculator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CalculateScoreAsync_AllSectionsEqualScore_ReturnsSameScore()
    {
        // Arrange
        var attempt = CreateAttempt(
            SectionTypes.Listening,
            SectionTypes.Reading,
            SectionTypes.Writing);
        var expectedScore = 8.5m;

        SetupMockCalculator(_mockListeningCalculator, expectedScore);
        SetupMockCalculator(_mockReadingCalculator, expectedScore);
        SetupMockCalculator(_mockWritingCalculator, expectedScore);

        // Act
        var result = await _calculator.CalculateScoreAsync(attempt);

        // Assert
        Assert.Equal(expectedScore, result.FinalScore);
        Assert.Equal(3, result.SectionScores.Count);
        Assert.All(result.SectionScores.Values, score =>
            Assert.Equal(expectedScore, score.Score));
    }

    [Fact]
    public async Task CalculateScoreAsync_DifferentScores_ReturnsAverageRoundedToHalf()
    {
        // Arrange
        var attempt = CreateAttempt(
            SectionTypes.Listening,
            SectionTypes.Reading,
            SectionTypes.Writing);

        SetupMockCalculator(_mockListeningCalculator, 8.0m);
        SetupMockCalculator(_mockReadingCalculator, 7.0m);
        SetupMockCalculator(_mockWritingCalculator, 6.0m);

        // Act
        var result = await _calculator.CalculateScoreAsync(attempt);

        // Assert
        Assert.Equal(7.0m, result.FinalScore); // (8 + 7 + 6) / 3 = 7
        Assert.Equal(3, result.SectionScores.Count);
    }

    [Fact]
    public async Task CalculateScoreAsync_MissingSection_ExcludesFromCalculation()
    {
        // Arrange
        var attempt = CreateAttempt(
            SectionTypes.Listening,
            SectionTypes.Reading);

        SetupMockCalculator(_mockListeningCalculator, 8.0m);
        SetupMockCalculator(_mockReadingCalculator, 7.0m);

        // Act
        var result = await _calculator.CalculateScoreAsync(attempt);

        // Assert
        Assert.Equal(7.5m, result.FinalScore); // (8 + 7) / 2 = 7.5
        Assert.Equal(2, result.SectionScores.Count);
    }

    private static StudentAttempt CreateAttempt(params SectionTypes[] sectionTypes)
    {
        var exam = new Exam
        {
            Id = 1,
            Title = "Test Exam",
            SectionParts = sectionTypes.Select((type, index) => new SectionPart
            {
                Id = index + 1,
                Title = $"Section {type}",
                SectionType = type,
                Questions = new List<Question>
                {
                    new() { Id = index + 1 }
                }
            }).ToList()
        };

        return new StudentAttempt
        {
            Id = 1,
            ExamId = 1,
            Exam = exam,
            Answers = new List<Answer>()
        };
    }

    private static void SetupMockCalculator(
        Mock<ISectionScoreCalculator> mock,
        decimal score)
    {
        mock.Setup(x => x.CalculateScoreAsync(
                It.IsAny<IEnumerable<Answer>>(),
                It.IsAny<IEnumerable<Question>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SectionScore
            {
                Score = score,
                DetailScores = new Dictionary<string, decimal>
                {
                    ["Part 1"] = score
                }
            });
    }
}