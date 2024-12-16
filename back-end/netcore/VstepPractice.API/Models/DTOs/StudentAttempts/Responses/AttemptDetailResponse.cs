using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

public class AttemptDetailResponse
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int SectionType { get; set; }
    
    public int SectionId { get; set; }
    public int Duration { get; set; }
    public AttemptStatus Status => EndTime.HasValue ? AttemptStatus.Completed : AttemptStatus.InProgress;

}
