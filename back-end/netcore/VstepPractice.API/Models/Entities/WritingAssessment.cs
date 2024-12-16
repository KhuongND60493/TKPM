using System.ComponentModel.DataAnnotations.Schema;

namespace VstepPractice.API.Models.Entities;

public class WritingAssessment : BaseEntity
{
    [Column("answerId")]
    public int AnswerId { get; set; }

    [Column("taskAchievement", TypeName = "decimal(4,2)")]
    public decimal TaskAchievement { get; set; }

    [Column("coherenceCohesion", TypeName = "decimal(4,2)")]
    public decimal CoherenceCohesion { get; set; }

    [Column("lexicalResource", TypeName = "decimal(4,2)")]
    public decimal LexicalResource { get; set; }

    [Column("grammarAccuracy", TypeName = "decimal(4,2)")]
    public decimal GrammarAccuracy { get; set; }

    [Column("detailedFeedback")]
    public string DetailedFeedback { get; set; } = string.Empty;

    [Column("assessedAt")]
    public DateTime AssessedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public decimal TotalScore => Math.Round((TaskAchievement + CoherenceCohesion + LexicalResource + GrammarAccuracy), 1);

    [ForeignKey(nameof(AnswerId))]
    public virtual Answer Answer { get; set; } = default!;
}