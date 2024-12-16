using System.ComponentModel.DataAnnotations;

namespace VstepPractice.API.Models.DTOs.Exams.Requests;

public class UpdateExamRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
}
