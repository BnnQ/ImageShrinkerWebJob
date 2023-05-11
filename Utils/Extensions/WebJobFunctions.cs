using ImageShrinkerWebJob.Services.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageShrinkerWebJob.Utils.Extensions;

public static class WebJobFunctions
{
    private static IServiceProvider? services;
    private static IImageResizer imageResizer = null!;
    private static IOptions<Configuration.ImageProcessing> imageProcessingOptions = null!;
    private static ILogger logger = null!;

    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        services = serviceProvider;

        imageResizer = services.GetRequiredService<IImageResizer>();
        imageProcessingOptions = services.GetRequiredService<IOptions<Configuration.ImageProcessing>>();    
        logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(WebJobFunctions));
        logger.LogInformation("Sucessfully setupped WebJobFunctions");
    }

    public static async Task ProcessUploadedImageAsync([QueueTrigger(queueName: "images-for-processing")] string pathToUploadedFile,
        [Blob("images/{queueTrigger}", FileAccess.Read)] Stream uploadedImage,
        [Blob("images-min/{queueTrigger}", FileAccess.Write)] Stream processedImage)
    {
        if (services is null)
        {
            throw new InvalidOperationException("Call WebJobFunctions.SetServiceProvider before using this class");
        }

        var imageExtension = Path.GetExtension(pathToUploadedFile);
        var newSize = new Size(imageProcessingOptions.Value.ResultWidth, imageProcessingOptions.Value.ResultHeight);
        
        var processedImageStream = await imageResizer.ResizeImageAsync(uploadedImage, imageExtension, newSize);
        logger.LogInformation("Resized {UploadedFile} image", pathToUploadedFile);
        await processedImageStream.CopyToAsync(processedImage);
        logger.LogInformation("Saved resized {UploadedFile} image", pathToUploadedFile);
    }
    
}