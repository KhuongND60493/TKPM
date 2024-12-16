using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

public class AnswerRepository : RepositoryBase<Answer, int>, IAnswerRepository
{
    public AnswerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Answer?> GetAnswerWithDetailsAsync(
    int answerId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Answers
            .Include(a => a.Question)
                .ThenInclude(q => q.Options)
            .Include(a => a.Question)
                .ThenInclude(q => q.Passage) // Add section to get sectionType
            .Include(a => a.SelectedOption)
            .FirstOrDefaultAsync(a => a.Id == answerId, cancellationToken);
    }

    public override async Task<Answer?> FindByIdAsync(
    int id,
    CancellationToken cancellationToken = default,
    params Expression<Func<Answer, object>>[] includeProperties)
    {
        var defaultIncludes = new List<Expression<Func<Answer, object>>>
    {
        a => a.Question,
        a => a.Question.Options,
        a => a.Question.Passage, // Add section to get sectionType
        a => a.SelectedOption
    };

        if (includeProperties != null)
            defaultIncludes.AddRange(includeProperties);

        return await base.FindByIdAsync(id, cancellationToken, defaultIncludes.ToArray());
    }
}
