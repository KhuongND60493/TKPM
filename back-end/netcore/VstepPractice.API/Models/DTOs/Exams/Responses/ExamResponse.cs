
using VstepPractice.API.Models.DTOs.SectionParts.Responses;

namespace VstepPractice.API.Models.DTOs.Exams.Responses;

public class ExamResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SectionPartResponse> Sections { get; set; } = new();
}
