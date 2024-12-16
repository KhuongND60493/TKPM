using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.DTOs.Scores;

public record VstepScore
{
    public decimal FinalScore { get; init; }
    public Dictionary<SectionTypes, SectionScore> SectionScores { get; init; } = new();

    public static VstepScore Empty => new()
    {
        FinalScore = 0,
        SectionScores = new Dictionary<SectionTypes, SectionScore>()
    };
}