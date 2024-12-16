namespace VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

public class AttemptStudentSummaryResponse
{
    public int ExamId { get; set; }
    public string? ExamTitle { get; set; } = "";
    public string? ExamDescription { get; set; } = "";
    public AttemptResponse? Inprocess { get; set; } = null;
    public List<SummaryAttemptResponse> Attempts { get; set; } = new();
}