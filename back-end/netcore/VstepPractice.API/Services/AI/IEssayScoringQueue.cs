using VstepPractice.API.Models.DTOs.AI;
namespace VstepPractice.API.Services.AI;

public interface IEssayScoringQueue
{
    Task QueueScoringTaskAsync(EssayScoringTask task);
}
