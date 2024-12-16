using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("StudentAttemptDetails")]
public class StudentAttemptDetail : BaseEntity
{
    [Required]
    [Column("startTime")]
    public DateTime StartTime { get; set; } 

    [Column("endTime")]
    public DateTime? EndTime { get; set; } 
    
    [Required]
    [Column("sectionId")]
    public int SectionId { get; set; } 
    
    [Required]
    [Column("sectionType")]
    public SectionTypes SectionType { get; set; } 

    [Required]
    [Range(1, int.MaxValue)]
    [Column("duration")]
    public int Duration { get; set; } = 30; 
    [Required]
    [Column("studentAttemptId")]
    public int StudentAttemptId { get; set; }
    [ForeignKey(nameof(StudentAttemptId))]
    public virtual StudentAttempt StudentAttempt { get; set; } = default!;
}