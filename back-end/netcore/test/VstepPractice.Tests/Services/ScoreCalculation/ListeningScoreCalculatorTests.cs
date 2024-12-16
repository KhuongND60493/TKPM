using Microsoft.Extensions.Logging;
using Moq;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Services.ScoreCalculation.Implementations;
using VstepPractice.Tests.Services.ScoreCalculation;

namespace VstepPractice.API.Tests.Services.ScoreCalculation;

public class ListeningScoreCalculatorTests
{
    private readonly ListeningScoreCalculator _calculator;
    private readonly Mock<ILogger<ListeningScoreCalculator>> _mockLogger;

    public ListeningScoreCalculatorTests()
    {
        _mockLogger = new Mock<ILogger<ListeningScoreCalculator>>();
        _calculator = new ListeningScoreCalculator(_mockLogger.Object);
    }

    [Fact]
    public async Task CalculateScoreAsync_PerfectScore_Returns10()
    {
        // Arrange
        var questions = TestData.CreateQuestions(SectionTypes.Listening,
            ("Part 1", 8),
            ("Part 2", 12),
            ("Part 3", 15));

        var answers = TestData.CreateAnswers(questions, 1.0m); // 100% correct

        // Act
        var result = await _calculator.CalculateScoreAsync(answers, questions);

        // Assert
        Assert.Equal(10m, result.Score);
        Assert.Equal(35, result.DetailScores.Values.Sum());
    }

    [Fact]
    public async Task CalculateScoreAsync_NoCorrectAnswers_Returns0()
    {
        // Arrange
        var questions = TestData.CreateQuestions(SectionTypes.Listening,
            ("Part 1", 8),
            ("Part 2", 12),
            ("Part 3", 15));

        var answers = TestData.CreateAnswers(questions, 0m); // 0% correct

        // Act
        var result = await _calculator.CalculateScoreAsync(answers, questions);

        // Assert
        Assert.Equal(0m, result.Score);
        Assert.Equal(0, result.DetailScores.Values.Sum());
    }

    [Fact]
    public async Task CalculateScoreAsync_FortyPercentCorrect_Returns4()
    {
        // Arrange
        var questions = TestData.CreateQuestions(SectionTypes.Listening,
            ("Part 1", 8),
            ("Part 2", 12),
            ("Part 3", 15));

        var answers = TestData.CreateAnswers(questions, 0.4m); // 40% correct = 14/35

        // Act
        var result = await _calculator.CalculateScoreAsync(answers, questions);

        // Assert
        Assert.Equal(4m, result.Score);
        Assert.Equal(14, result.DetailScores.Values.Sum());
    }

    [Fact]
    public async Task CalculateScoreAsync_SeventyPercentCorrect_Returns7()
    {
        // Arrange
        var questions = TestData.CreateQuestions(SectionTypes.Listening,
            ("Part 1", 8),
            ("Part 2", 12),
            ("Part 3", 15));

        var answers = TestData.CreateAnswers(questions, 0.7m); // 70% correct ≈ 24/35

        // Act
        var result = await _calculator.CalculateScoreAsync(answers, questions);

        // Assert
        Assert.Equal(7m, result.Score);
        Assert.Equal(24, result.DetailScores.Values.Sum());
    }

    [Fact]
    public async Task CalculateScoreAsync_MissingAnswers_CalculatesBasedOnAvailable()
    {
        // Arrange
        var questions = TestData.CreateQuestions(SectionTypes.Listening,
            ("Part 1", 8),
            ("Part 2", 12),
            ("Part 3", 15));

        var partialQuestions = questions.Take(20).ToList();
        var answers = TestData.CreateAnswers(partialQuestions, 0.8m); // 80% of first 20 questions

        // Act
        var result = await _calculator.CalculateScoreAsync(answers, questions);

        // Assert
        Assert.True(result.Score < 10m);
        Assert.NotEmpty(result.DetailScores);
        Assert.Contains("Missing answers", result.Feedback);
    }
}