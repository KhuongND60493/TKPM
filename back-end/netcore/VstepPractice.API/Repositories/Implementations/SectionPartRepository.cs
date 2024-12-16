using Microsoft.EntityFrameworkCore;
using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

public class SectionPartRepository : RepositoryBase<SectionPart, int>, ISectionPartRepository
{
    public SectionPartRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<SectionPart>> GetHierarchyForExamAsync(
    int examId,
    CancellationToken cancellationToken = default)
    {
        return await _context.SectionParts
            .Where(sp => sp.ExamId == examId)
            .Include(sp => sp.Children.OrderBy(c => c.OrderNum))
                .ThenInclude(sp => sp.Questions.OrderBy(q => q.OrderNum))
                    .ThenInclude(q => q.Options.OrderBy(o => o.OrderNum))
            .Include(sp => sp.Questions.OrderBy(q => q.OrderNum))
                .ThenInclude(q => q.Options.OrderBy(o => o.OrderNum))
            .OrderBy(sp => sp.OrderNum)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasCircularReference(
        int sectionPartId,
        int? parentId,
        CancellationToken cancellationToken = default)
    {
        if (!parentId.HasValue)
            return false;

        var current = parentId.Value;
        var visited = new HashSet<int>();

        while (current != 0)
        {
            if (!visited.Add(current))
                return true;

            if (current == sectionPartId)
                return true;

            var parent = await _context.SectionParts
                .Where(sp => sp.Id == current)
                .Select(sp => sp.ParentId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!parent.HasValue)
                break;

            current = parent.Value;
        }

        return false;
    }
    
    public async Task<SectionPart?> FindFirstSectionPartAsync(int examId, CancellationToken cancellationToken = default)
    {
        return await _context.SectionParts
            .Where(sp => sp.ExamId == examId && sp.ParentId == null)
            .OrderBy(sp => sp.OrderNum)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<SectionPart?> FindNextSectionByOrderNumAsync(int currentSectionId, CancellationToken cancellationToken = default)
    {
        var currentSection = await _context.SectionParts
            .Where(s => s.Id == currentSectionId)
            .SingleOrDefaultAsync(cancellationToken);

        if (currentSection == null)
            return null;

        return await _context.SectionParts
            .Where(s => s.ExamId == currentSection.ExamId && s.OrderNum > currentSection.OrderNum && s.ParentId == null)
            .OrderBy(s => s.OrderNum)
            .FirstOrDefaultAsync(cancellationToken);
    }

}