namespace VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

public class ExamStudentResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Duration { get; set; } = 30;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}