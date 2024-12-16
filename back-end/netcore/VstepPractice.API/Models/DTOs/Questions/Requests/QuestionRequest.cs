using System.ComponentModel.DataAnnotations;

namespace VstepPractice.API.Models.DTOs.Questions.Requests;

public class QuestionRequest
{
    [Required]
    [StringLength(1000)]
    public string QuestionText { get; set; } = string.Empty;

    public string? MediaUrl { get; set; }

    [Range(1, 10)]
    public int Points { get; set; } = 1;

    [Required]
    public int OrderNum { get; set; }

    [Required]
    [MinLength(2)]
    public List<QuestionOptionRequest> Options { get; set; } = new();
}
