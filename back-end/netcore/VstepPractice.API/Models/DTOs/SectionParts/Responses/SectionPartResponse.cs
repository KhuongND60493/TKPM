using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.DTOs.Questions.Responses;

namespace VstepPractice.API.Models.DTOs.SectionParts.Responses;

public class SectionPartResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public string? Content { get; set; }
    public int OrderNum { get; set; }
    public SectionTypes SectionType { get; set; }
    public SectionPartTypes Type { get; set; }
    public int? ParentId { get; set; }
    public List<SectionPartResponse> Children { get; set; } = new();
    public List<QuestionResponse> Questions { get; set; } = new();
}