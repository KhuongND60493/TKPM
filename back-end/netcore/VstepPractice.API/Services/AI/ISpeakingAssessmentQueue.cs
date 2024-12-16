using VstepPractice.API.Models.DTOs.AI;

namespace VstepPractice.API.Services.AI;

public interface ISpeakingAssessmentQueue
{
    Task QueueAssessmentTaskAsync(SpeakingAssessmentTask task);
}
