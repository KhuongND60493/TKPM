namespace VstepPractice.API.Models.DTOs.AI;

public class SpeakingAssessmentTask
{
    public int AnswerId { get; set; }
    public string PassageTitle { get; set; } = string.Empty;
    public string PassageContent { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string TranscribedText { get; set; } = string.Empty;
    public string AudioUrl { get; set; } = string.Empty;
}
