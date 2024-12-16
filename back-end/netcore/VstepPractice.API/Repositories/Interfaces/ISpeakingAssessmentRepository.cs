using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Repositories.Interfaces;

public interface ISpeakingAssessmentRepository : IRepositoryBase<SpeakingAssessment, int>
{
    Task<SpeakingAssessment?> GetByAnswerIdAsync(
        int answerId,
        CancellationToken cancellationToken = default);
}
