using Microsoft.EntityFrameworkCore;
using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

public class WritingAssessmentRepository : RepositoryBase<WritingAssessment, int>, IWritingAssessmentRepository
{
    public WritingAssessmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<WritingAssessment?> GetByAnswerIdAsync(
        int answerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.WritingAssessments
            .FirstOrDefaultAsync(w => w.AnswerId == answerId, cancellationToken);
    }
}
