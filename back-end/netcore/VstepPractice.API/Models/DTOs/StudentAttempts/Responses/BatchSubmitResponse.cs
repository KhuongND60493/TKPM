using VstepPractice.API.Models.DTOs.StudentAttempts.Requests;

namespace VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

public class BatchSubmitResponse
{
    public int AttemptId { get; set; }
    public int SubmittedCount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public SubmitScope Scope { get; set; } = new();
    public Dictionary<string, decimal> Scores { get; set; } = new(); // Partial scores if available
    public List<AnswerValidationError>? ValidationErrors { get; set; }
}

public class AnswerValidationError
{
    public int QuestionId { get; set; }
    public string Message { get; set; } = string.Empty;
}