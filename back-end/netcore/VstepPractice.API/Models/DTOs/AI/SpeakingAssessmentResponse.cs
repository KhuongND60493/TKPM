using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Models.DTOs.AI;
public class SpeakingAssessmentResponse : ISkillAssessmentResponse
{
    // Azure Pronunciation Assessment scores (0-10)
    public decimal PronScore { get; set; }
    public decimal AccuracyScore { get; set; }
    public decimal FluencyScore { get; set; }
    public decimal ProsodyScore { get; set; }

    // OpenAI Assessment scores (0-10)
    public decimal GrammarScore { get; set; }
    public decimal VocabularyScore { get; set; }
    public decimal TopicScore { get; set; }

    // Text content
    public string RecognizedText { get; set; } = string.Empty;
    public string DetailedFeedback { get; set; } = string.Empty;

    // Word-level details from Azure
    public List<WordDetail> Words { get; set; } = new();

    // ISkillAssessmentResponse implementation
    public decimal TotalScore => Math.Round(
        (PronScore + AccuracyScore + FluencyScore + ProsodyScore 
        + GrammarScore + VocabularyScore + TopicScore) / 7,
        1);
}