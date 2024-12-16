using VstepPractice.API.Models.DTOs.Scores;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Services.ScoreCalculation;

public interface IVstepScoreCalculator
{
    Task<VstepScore> CalculateScoreAsync(
        StudentAttempt attempt,
        CancellationToken cancellationToken = default);
}