using System.ComponentModel.DataAnnotations.Schema;
using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.Entities;

public class StudentAttempt : BaseEntity
{
    [Column("userId")]
    public int UserId { get; set; }
    [Column("examId")]
    public int ExamId { get; set; }
    [Column("startTime")]
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    [Column("endTime")]
    public DateTime? EndTime { get; set; }
    [Column("status")]
    public AttemptStatus? Status { get; set; } = AttemptStatus.InProgress;
    
    [Column("duration")]
    public int Duration { get; set; }
    
    [Column("finalScore", TypeName = "decimal(4,2)")]
    public decimal FinalScore { get; set; } = 0;
    // Navigation properties
    [ForeignKey(nameof(ExamId))]
    public virtual Exam Exam { get; set; } = default!;
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = default!;
    public virtual ICollection<Answer> Answers { get; set; } = default!;
    public virtual ICollection<StudentAttemptDetail> StudentAttemptDetails { get; set; } = default!;
}