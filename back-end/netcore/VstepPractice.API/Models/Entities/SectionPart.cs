using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.Entities;

public class SectionPart : BaseEntity
{
    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Column("instructions")]
    public string? Instructions { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("orderNum")]
    public int OrderNum { get; set; }
    
    [Column("sectionType")]
    public SectionTypes SectionType { get; set; }
    [Column("type")]
    public SectionPartTypes Type { get; set; }

    [Column("examId")]
    public int ExamId { get; set; }

    [Column("parentId")]
    public int? ParentId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ExamId))]
    public virtual Exam Exam { get; set; } = default!;

    [ForeignKey(nameof(ParentId))]
    public virtual SectionPart? Parent { get; set; }

    public virtual ICollection<SectionPart> Children { get; set; } = new List<SectionPart>();
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}