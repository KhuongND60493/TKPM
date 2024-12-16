using System.ComponentModel.DataAnnotations;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.DTOs.Questions.Requests;

namespace VstepPractice.API.Models.DTOs.SectionParts.Requests;

public class SectionPartRequest
{
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public string? Content { get; set; }
    [Required]
    public int OrderNum { get; set; }
    [Required]
    public SectionTypes SectionType { get; set; }
    [Required]
    public SectionPartTypes Type { get; set; }
    public int? ParentId { get; set; }
    public List<QuestionRequest> Questions { get; set; } = new();
}
