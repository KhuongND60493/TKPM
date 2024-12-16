using System.ComponentModel.DataAnnotations.Schema;

namespace VstepPractice.API.Models.Entities;

public class Question : BaseEntity
{
    // for meaningfull
    [Column("passageId")]
    public int PassageId { get; set; }
    [Column("questionText")]
    public string? QuestionText { get; set; } = "";
    [Column("point")]
    public decimal Point { get; set; } = 1;
    [Column("orderNum")]
    public int OrderNum { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(PassageId))]
    public virtual SectionPart Passage { get; set; } = default!;
    public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}