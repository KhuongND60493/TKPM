namespace VstepPractice.API.Models.DTOs.Questions.Responses;

public class QuestionOptionResponse
{
    public int Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}