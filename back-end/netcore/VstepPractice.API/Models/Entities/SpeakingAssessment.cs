using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VstepPractice.API.Models.Entities;

public class SpeakingAssessment : BaseEntity
{
    [Column("answerId")]
    public int AnswerId { get; set; }

    // Core pronunciation scores from Azure (0-10)
    [Column("pronunciation", TypeName = "decimal(4,2)")]
    public decimal Pronunciation { get; set; }  // Overall pronunciation score

    [Column("fluency", TypeName = "decimal(4,2)")]
    public decimal Fluency { get; set; }  // Speech fluency/smoothness

    [Column("accuracy", TypeName = "decimal(4,2)")]
    public decimal Accuracy { get; set; }  // Pronunciation accuracy

    [Column("prosody", TypeName = "decimal(4,2)")]
    public decimal Prosody { get; set; }  // Intonation and stress patterns

    // Content assessment scores from OpenAI (0-10)
    [Column("vocabulary", TypeName = "decimal(4,2)")]
    public decimal Vocabulary { get; set; }  // Vocabulary usage score

    [Column("grammar", TypeName = "decimal(4,2)")]
    public decimal Grammar { get; set; }  // Grammar accuracy

    [Column("topicScore", TypeName = "decimal(4,2)")]
    public decimal TopicScore { get; set; }  // Topic relevance score

    // Feedback and text content
    [Column("detailedFeedback")]
    public string DetailedFeedback { get; set; } = string.Empty;  // Generated comprehensive feedback

    [Column("transcribedText")]
    public string TranscribedText { get; set; } = string.Empty;  // Recognized speech text

    // URLs for associated files
    [Column("audioUrl")]
    [StringLength(255)]
    public string AudioUrl { get; set; } = string.Empty;  // URL to audio recording

    [Column("wordDetails", TypeName = "jsonb")]
    public List<WordDetail>? WordDetails { get; set; } = new();  // detailed JSON assessment

    [Column("assessedAt")]
    public DateTime AssessedAt { get; set; } = DateTime.UtcNow;

    // Calculated total score (0-10 scale for VSTEP compatibility)
    [NotMapped]
    public decimal TotalScore => Math.Round(
        (Pronunciation + Accuracy + Fluency + Prosody
        + Grammar + Vocabulary + TopicScore) / 7,
        1);

    // Navigation property
    [ForeignKey(nameof(AnswerId))]
    public virtual Answer Answer { get; set; } = default!;
}

public class WordDetail
{
    public string Word { get; set; } = string.Empty;
    public decimal AccuracyScore { get; set; }
    public string ErrorType { get; set; } = string.Empty;
    public ICollection<PhonemeDetail>? Phonemes { get; set; }
}

public class PhonemeDetail
{
    public string Phoneme { get; set; } = string.Empty;
    public decimal AccuracyScore { get; set; }
    public long Offset { get; set; }
    public int Duration { get; set; }
}