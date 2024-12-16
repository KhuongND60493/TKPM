namespace VstepPractice.API.Models.Entities;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
    DateTime CreatedAt { get; set; }
}