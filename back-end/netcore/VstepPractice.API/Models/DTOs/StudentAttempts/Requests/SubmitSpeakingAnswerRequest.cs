namespace VstepPractice.API.Models.DTOs.StudentAttempts.Requests;

public class SubmitSpeakingAnswerRequest
{
    public int UserId { get; set; }
    public int QuestionId { get; set; }
    public IFormFile AudioFile { get; set; } = null!;
}
