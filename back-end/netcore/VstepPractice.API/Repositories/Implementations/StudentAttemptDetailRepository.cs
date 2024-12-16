using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

using Microsoft.EntityFrameworkCore;

public class StudentAttemptDetailRepository : RepositoryBase<StudentAttemptDetail, int>, IStudentAttemptDetailRepository
{
    public StudentAttemptDetailRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<List<StudentAttemptDetail>> FindByAttemptIdAsync(
        int attemptId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.StudentAttemptDetails
            .Where(detail => detail.StudentAttemptId == attemptId)
            .ToListAsync(cancellationToken);
    }


    public void Add(StudentAttemptDetail studentAttemptDetail)
    {
        _context.StudentAttemptDetails.Add(studentAttemptDetail);
    }
}
