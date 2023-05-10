namespace ImageShrinkerWebJob.Configuration;

public class Azure
{
    public string QueueName { get; set; } = null!;
    public string SourceImagesContainerName { get; set; } = null!;
    public string ProcessedImagesContainerName { get; set; } = null!;
}