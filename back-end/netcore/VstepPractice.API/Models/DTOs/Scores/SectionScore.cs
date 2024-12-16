namespace VstepPractice.API.Models.DTOs.Scores;

public record SectionScore
{
    public decimal Score { get; init; }
    public Dictionary<string, decimal> DetailScores { get; init; } = new();
    public string? Feedback { get; init; }
}
