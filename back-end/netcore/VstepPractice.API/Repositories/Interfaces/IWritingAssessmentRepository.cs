using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Repositories.Interfaces;

public interface IWritingAssessmentRepository : IRepositoryBase<WritingAssessment, int>
{
    Task<WritingAssessment?> GetByAnswerIdAsync(
        int answerId,
        CancellationToken cancellationToken = default);
}