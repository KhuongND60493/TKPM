using System.Linq.Expressions;
using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Repositories.Interfaces;

public interface IExamRepository : IRepositoryBase<Exam, int>
{
    Task<PagedResult<Exam>> GetPagedAsync(
        Expression<Func<Exam, bool>>? predicate = null,
        int pageIndex = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}
