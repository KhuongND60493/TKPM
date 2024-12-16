using Microsoft.Extensions.Logging;
using Moq;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Services.ScoreCalculation.Implementations;
using VstepPractice.Tests.Services.ScoreCalculation;

namespace VstepPractice.API.Tests.Services.ScoreCalculation;

public class AlternativeReadingScoreCalculatorTests
{
    private readonly AlternativeReadingScoreCalculator _calculator;
    private readonly Mock<ILogger<AlternativeReadingScoreCalculator>> _mockLogger;

    public AlternativeReadingScoreCalculatorTests()
    {
        _mockLogger = new Mock<ILogger<AlternativeReadingScoreCalculator>>();
        _calculator = new AlternativeReadingScoreCalculator(_mockLogger.Object);
    }

    [Fact]
    public async Task CalculateScoreAsync_PerfectScore_Returns10()
    {
        // Arrange
        var questions = TestData.CreateQuestions(SectionTypes.Reading,
            ("Part 1", 10),
            ("Part 2", 10),
            ("Part 3", 10),
            ("Part 4", 10));

        var answers = TestData.CreateAnswers(questions, 1.0m); // 100% correct

        // Act
        var result = await _calculator.CalculateScoreAsync(answers, questions);

        // Assert
        Assert.Equal(10m, result.Score);
        Assert.Equal(40, result.DetailScores.Values.Sum());
    }

    [Fact]
    public async Task CalculateScoreAsync_NoCorrectAnswers_Returns0()
    {
        // Arrange
        var questions = TestData.CreateQuestions(SectionTypes.Reading,
            ("Part 1", 10),
            ("Part 2", 10),
            ("Part 3", 10),
            ("Part 4", 10));

        var answers = TestData.CreateAnswers(questions, 0m); // 0% correct

        // Act
        var result = await _calculator.CalculateScoreAsync(answers, questions);

        // Assert
        Assert.Equal(0m, result.Score);
        Assert.Equal(0, result.DetailScores.Values.Sum());
    }

    [Fact]
    public async Task CalculateScoreAsync_HalfCorrect_Returns5()
    {
        // Arrange
        var questions = TestData.CreateQuestions(SectionTypes.Reading,
            ("Part 1", 10),
            ("Part 2", 10),
            ("Part 3", 10),
            ("Part 4", 10));

        var answers = TestData.CreateAnswers(questions, 0.5m); // 50% correct

        // Act
        var result = await _calculator.CalculateScoreAsync(answers, questions);

        // Assert
        Assert.Equal(5m, result.Score);
        Assert.Equal(20, result.DetailScores.Values.Sum());
    }
}