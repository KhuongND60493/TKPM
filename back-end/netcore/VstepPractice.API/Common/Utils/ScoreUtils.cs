using VstepPractice.API.Models.DTOs.AI;

namespace VstepPractice.API.Common.Utils;

public static class ScoreUtils
{
    public static decimal RoundToNearestHalf(decimal score)
    {
        return Math.Round(score * 2, MidpointRounding.AwayFromZero) / 2;
    }

    public static decimal CalculateAverageScore(params decimal[] scores)
    {
        if (scores == null || scores.Length == 0)
            return 0;

        return RoundToNearestHalf(scores.Average());
    }

    public static decimal ConvertToScale10(int correct, int total)
    {
        if (total == 0) return 0;
        return RoundToNearestHalf((decimal)correct / total * 10);
    }
}
