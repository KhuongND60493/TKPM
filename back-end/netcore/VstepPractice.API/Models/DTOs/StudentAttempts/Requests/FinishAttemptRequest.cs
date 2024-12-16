namespace VstepPractice.API.Models.DTOs.StudentAttempts.Requests;

public class FinishAttemptRequest
{
    public int UserId { get; set; }
    public int AttemptId { get; set; }
}
