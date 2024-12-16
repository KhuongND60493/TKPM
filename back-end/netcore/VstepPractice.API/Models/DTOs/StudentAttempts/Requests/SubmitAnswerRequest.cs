namespace VstepPractice.API.Models.DTOs.StudentAttempts.Requests;

public class SubmitAnswerRequest
{
    public int UserId { get; set; }
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }  // For multiple choice
    public string? EssayAnswer { get; set; }    // For writing section
}
