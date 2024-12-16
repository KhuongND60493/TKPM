using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.DTOs.Questions.Responses;

public class QuestionResponse
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public int Points { get; set; }
    public SectionTypes SectionType { get; set; }
    public List<QuestionOptionResponse> Options { get; set; } = new();
}
