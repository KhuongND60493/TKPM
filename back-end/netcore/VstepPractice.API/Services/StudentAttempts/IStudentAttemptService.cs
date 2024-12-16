using VstepPractice.API.Common.Utils;
using VstepPractice.API.Models.DTOs.StudentAttempts.Requests;
using VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

namespace VstepPractice.API.Services.StudentAttempts;

public interface IStudentAttemptService
{
    Task<Result<List<ExamStudentResponse>>> GetExamAsync(
        CancellationToken cancellationToken = default);
    Task<Result<AttemptResponse>> StartAttemptAsync(
        int userId,
        StartAttemptRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<AttemptStudentSummaryResponse>> ListAllAttemptAsync(
        int userId,
        StartAttemptRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<AnswerResponse>> SubmitAnswerAsync(
        int userId,
        int attemptId,
        SubmitAnswerRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AnswerResponse>> SubmitSpeakingAnswerAsync(
        int userId,
        int attemptId,
        SubmitSpeakingAnswerRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AttemptResultResponse>> FinishAttemptAsync(
        int userId,
        FinishAttemptRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AttemptResultResponse>> GetAttemptResultAsync(
        int userId,
        int attemptId,
        CancellationToken cancellationToken = default);

    Task<Result<BatchSubmitResponse>> BatchSubmitAnswersAsync(
        int userId,
        int attemptId,
        BatchSubmitAnswersRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<BatchSubmitResponse>> BatchSubmitSpeakingAsync(
    int userId,
    int attemptId,
    BatchSubmitSpeakingRequest request,
    CancellationToken cancellationToken = default);
}