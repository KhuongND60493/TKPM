namespace VstepPractice.API.Models.DTOs.Scores;

// Helpers for part scores
public record PartScore
{
    public string PartTitle { get; init; } = string.Empty;
    public int CorrectAnswers { get; init; }
    public int TotalQuestions { get; init; }
    public decimal Score { get; init; }
}