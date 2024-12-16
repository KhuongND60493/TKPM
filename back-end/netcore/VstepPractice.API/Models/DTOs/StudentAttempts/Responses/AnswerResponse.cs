using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

public class AnswerResponse
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string PassageTitle { get; set; } = string.Empty;
    public string? PassageContent { get; set; }
    public int? QuestionOptionId { get; set; }
    public string? EssayAnswer { get; set; }
    public string? AiFeedback { get; set; }
    public decimal? Score { get; set; }
    public bool IsCorrect { get; set; }
    public SectionTypes SectionType { get; set; }
    public WritingScoreDetails? WritingScore { get; set; }
    public SpeakingScoreDetails? SpeakingScore { get; set; }
}
