namespace VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

public class SummaryAttemptResponse
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; } = null;
    public decimal FinalScore { get; set; }
}
