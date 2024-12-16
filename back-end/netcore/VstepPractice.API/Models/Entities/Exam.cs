using System.ComponentModel.DataAnnotations.Schema;

namespace VstepPractice.API.Models.Entities;

public class Exam : BaseEntity
{
    [Column("title")]
    public string? Title { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    
    [Column("duration")]
    public int Duration { get; set; }
    [Column("userId")]
    public int UserId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User CreatedBy { get; set; } = default!;
    public virtual ICollection<SectionPart> SectionParts { get; set; } = default!;
    public virtual ICollection<StudentAttempt> StudentAttempts { get; set; } = default!;
}
