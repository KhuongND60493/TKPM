using System.ComponentModel.DataAnnotations;

namespace VstepPractice.API.Models.DTOs.StudentAttempts.Requests;

public class StartAttemptRequest
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public int ExamId { get; set; }
}
