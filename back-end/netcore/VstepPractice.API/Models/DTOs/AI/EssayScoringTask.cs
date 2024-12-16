using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.DTOs.AI;

public class EssayScoringTask
{
    public int AnswerId { get; set; }
    public string PassageTitle { get; set; } = string.Empty;
    public string PassageContent { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string Essay { get; set; } = string.Empty;
    public SectionTypes SectionType { get; set; }
}
