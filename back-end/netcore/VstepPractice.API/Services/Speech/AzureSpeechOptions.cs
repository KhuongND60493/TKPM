namespace VstepPractice.API.Services.Speech;

public class AzureSpeechOptions
{
    public const string SectionName = "Azure:Speech";

    public string SubscriptionKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}