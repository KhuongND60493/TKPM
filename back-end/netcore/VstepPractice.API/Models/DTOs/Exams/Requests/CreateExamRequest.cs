using System.ComponentModel.DataAnnotations;
using VstepPractice.API.Models.DTOs.SectionParts.Requests;

namespace VstepPractice.API.Models.DTOs.Exams.Requests;

public class CreateExamRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public List<SectionPartRequest> Sections { get; set; } = new();
}