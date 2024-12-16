using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Repositories.Interfaces;

public interface ISectionPartRepository : IRepositoryBase<SectionPart, int>
{
    Task<List<SectionPart>> GetHierarchyForExamAsync(
        int examId,
        CancellationToken cancellationToken = default);

    Task<SectionPart?> FindNextSectionByOrderNumAsync(int currentSectionId,
        CancellationToken cancellationToken = default);

    Task<bool> HasCircularReference(
        int sectionPartId,
        int? parentId,
        CancellationToken cancellationToken = default);
    Task<SectionPart?> FindFirstSectionPartAsync(int examId, CancellationToken cancellationToken = default);

}