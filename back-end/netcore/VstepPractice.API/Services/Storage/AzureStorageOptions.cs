namespace VstepPractice.API.Services.Storage;

public class AzureStorageOptions
{
    public const string SectionName = "Azure:StorageAccount";

    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}