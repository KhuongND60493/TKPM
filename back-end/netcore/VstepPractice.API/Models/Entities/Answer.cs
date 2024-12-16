using System.ComponentModel.DataAnnotations.Schema;

namespace VstepPractice.API.Models.Entities;

public class Answer : BaseEntity
{
    [Column("attemptId")]
    public int AttemptId { get; set; }
    [Column("questionId")]
    public int QuestionId { get; set; }
    [Column("questionOptionId")]
    public int? QuestionOptionId { get; set; }  // For multiple choice questions
    [Column("essayAnswer")]
    public string? EssayAnswer { get; set; }     // For writing questions
    [Column("aiFeedback")]
    public string? AiFeedback { get; set; }
    [Column("score")]
    public decimal? Score { get; set; }         // Điểm số (cho cả trắc nghiệm và tự luận)
    // Navigation properties
    [ForeignKey(nameof(AttemptId))]
    public virtual StudentAttempt Attempt { get; set; } = default!;
    [ForeignKey(nameof(QuestionId))]
    public virtual Question Question { get; set; } = default!;
    [ForeignKey(nameof(QuestionOptionId))]
    public virtual QuestionOption? SelectedOption { get; set; }
}
