namespace VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

public class WritingScoreDetails
{
    public decimal TaskAchievement { get; set; }
    public decimal CoherenceCohesion { get; set; }
    public decimal LexicalResource { get; set; }
    public decimal GrammarAccuracy { get; set; }
    public decimal TotalScore =>
        Math.Round((TaskAchievement + CoherenceCohesion + LexicalResource + GrammarAccuracy), 1);
}
