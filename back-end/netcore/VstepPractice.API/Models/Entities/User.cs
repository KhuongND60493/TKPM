using System.ComponentModel.DataAnnotations.Schema;
using VstepPractice.API.Common.Enums;

namespace VstepPractice.API.Models.Entities;

public class User : BaseEntity
{
    [Column("email")]
    public string? Email { get; set; }
    [Column("password")]
    public string? Password { get; set; }
    [Column("firstName")]
    public string? FirstName { get; set; }
    [Column("lastName")]
    public string? LastName { get; set; }
    [Column("role")]
    public UserRoles Role { get; set; }
    public virtual ICollection<Exam> CreatedExams { get; set; } = default!;
    public virtual ICollection<StudentAttempt> StudentAttempts { get; set; } = default!;
}
