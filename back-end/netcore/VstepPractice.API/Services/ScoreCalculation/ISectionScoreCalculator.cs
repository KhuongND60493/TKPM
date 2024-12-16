using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Services.ScoreCalculation;

public interface ISectionScoreCalculator
{
    Task<SectionScore> CalculateScoreAsync(
        IEnumerable<Answer> answers,
        IEnumerable<Question> questions,
        CancellationToken cancellationToken = default);
}
