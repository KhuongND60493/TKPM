using System.ComponentModel.DataAnnotations;
using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.DTOs.StudentAttempts.Requests;

public class BatchSubmitAnswersRequest
{
    public int UserId { get; set; }

    public SubmitScope Scope { get; set; } = new();

    public List<AnswerSubmission> Answers { get; set; } = new();
}

public class SubmitScope
{
    public SectionPartTypes Type { get; set; } // Section or Part
    public int SectionPartId { get; set; }  // ID of section/part being submitted
    public string Title { get; set; } = string.Empty; // Title for logging/tracking
}

public class AnswerSubmission
{
    [Required]
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }  // For multiple choice
    public string? EssayAnswer { get; set; }    // For writing section
}