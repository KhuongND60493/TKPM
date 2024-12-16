using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

public class QuestionOptionRepository : RepositoryBase<QuestionOption, int>, IQuestionOptionRepository
{
    public QuestionOptionRepository(ApplicationDbContext context) : base(context)
    {
    }
}
