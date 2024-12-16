namespace VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

public class SpeakingScoreDetails
{
    public decimal Pronunciation { get; set; }
    public decimal Fluency { get; set; }
    public decimal Accuracy { get; set; }
    public decimal Prosody { get; set; }

    public decimal Vocabulary { get; set; }
    public decimal Grammar { get; set; }
    public decimal TopicScore { get; set; }

    public string AudioUrl { get; set; } = string.Empty;
    public string TranscribedText { get; set; } = string.Empty;
    public decimal TotalScore => Math.Round(
        (Pronunciation + Accuracy + Fluency + Prosody
        + Grammar + Vocabulary + TopicScore) / 7,
        1);
}