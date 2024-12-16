using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

public class ExamRepository : RepositoryBase<Exam, int>, IExamRepository
{
    public ExamRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Exam>> GetPagedAsync(
    Expression<Func<Exam, bool>>? predicate = null,
    int pageIndex = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Exam>()
            .AsNoTracking()
            .Include(e => e.SectionParts.OrderBy(s => s.OrderNum))
                .ThenInclude(s => s.Questions.OrderBy(q => q.OrderNum))
                    .ThenInclude(q => q.Options.OrderBy(o => o.OrderNum))
            .AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        return await PagedResult<Exam>.CreateAsync(
            query,
            pageIndex,
            pageSize,
            cancellationToken);
    }

    public override async Task<Exam?> FindByIdAsync(
    int id,
    CancellationToken cancellationToken = default,
    params Expression<Func<Exam, object>>[] includeProperties)
    {
        var query = _context.Set<Exam>().AsQueryable();

        // Include default relations
        query = query
            .Include(e => e.SectionParts.OrderBy(s => s.OrderNum))
                .ThenInclude(s => s.Questions.OrderBy(q => q.OrderNum))
                    .ThenInclude(q => q.Options.OrderBy(o => o.OrderNum));

        // Add any additional includes
        foreach (var property in includeProperties)
        {
            query = query.Include(property);
        }

        return await query
            .AsTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}