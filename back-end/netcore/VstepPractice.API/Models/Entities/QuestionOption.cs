using System.ComponentModel.DataAnnotations.Schema;

namespace VstepPractice.API.Models.Entities;

public class QuestionOption : BaseEntity
{
    [Column("questionId")] 
    public int QuestionId { get; set; }
    [Column("content")] 
    public string? Content { get; set; } = "";

    [Column("isCorrect")] 
    public bool IsCorrect { get; set; } = false;
    [Column("orderNum")]
    public int OrderNum { get; set; }
    // Navigation property
    [ForeignKey(nameof(QuestionId))] 
    public virtual Question Question { get; set; } = default!;
}