using Microsoft.EntityFrameworkCore;
using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

public class SpeakingAssessmentRepository : RepositoryBase<SpeakingAssessment, int>, ISpeakingAssessmentRepository
{
    public SpeakingAssessmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SpeakingAssessment?> GetByAnswerIdAsync(
        int answerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SpeakingAssessments
            .FirstOrDefaultAsync(w => w.AnswerId == answerId, cancellationToken);
    }
}
