using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Repositories.Interfaces;

public interface IStudentAttemptDetailRepository: IRepositoryBase<StudentAttemptDetail, int>
{
    Task<List<StudentAttemptDetail?>> FindByAttemptIdAsync(int attemptId, CancellationToken cancellationToken = default);
    void Add(StudentAttemptDetail studentAttemptDetail);
}
