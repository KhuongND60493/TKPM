using System.ComponentModel.DataAnnotations.Schema;

namespace VstepPractice.API.Models.Entities;

public abstract class BaseEntity : IEntity<int>
{
    [Column("id")] 
    public int Id { get; set; }
    
    [Column("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("updatedAt")]
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
}
