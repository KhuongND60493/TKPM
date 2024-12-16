using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Repositories.Interfaces;

public interface IAnswerRepository : IRepositoryBase<Answer, int>
{
    Task<Answer?> GetAnswerWithDetailsAsync(
        int answerId,
        CancellationToken cancellationToken = default);
}