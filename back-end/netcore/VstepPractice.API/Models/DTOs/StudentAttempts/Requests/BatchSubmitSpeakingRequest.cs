namespace VstepPractice.API.Models.DTOs.StudentAttempts.Requests;

public class BatchSubmitSpeakingRequest
{
    public int UserId { get; set; }
    public SubmitScope Scope { get; set; } = new();
    public List<SpeakingSubmission> Answers { get; set; } = new();
}

public class SpeakingSubmission
{
    public int QuestionId { get; set; }
    public IFormFile AudioFile { get; set; } = null!;
}