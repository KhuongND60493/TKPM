namespace VstepPractice.API.Models.DTOs.AI;

public interface ISkillAssessmentResponse
{
    decimal TotalScore { get; }
    string DetailedFeedback { get; set; }
}
