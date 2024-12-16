using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.AI;

namespace VstepPractice.API.Services.Speech;

public interface ISpeakingAssessmentService
{
    Task<Result<SpeakingAssessmentResponse>> AssessSpeakingAsync(
        SpeakingAssessmentTask task,
        CancellationToken cancellationToken = default);
}
