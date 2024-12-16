namespace VstepPractice.API.Models.DTOs.AI;

public class WritingAssessmentResponse : ISkillAssessmentResponse
{
    public decimal TaskAchievement { get; set; }
    public decimal CoherenceCohesion { get; set; }
    public decimal LexicalResource { get; set; }
    public decimal GrammarAccuracy { get; set; }
    public string DetailedFeedback { get; set; } = string.Empty;

    public decimal TotalScore => Math.Round((TaskAchievement + CoherenceCohesion + LexicalResource + GrammarAccuracy), 1);
}
