using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using ImageShrinkerWebJob.Configuration;
using ImageShrinkerWebJob.Services;
using ImageShrinkerWebJob.Services.Abstractions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageShrinkerWebJob.Utils.Extensions;

public static class StartupExtensions
{
    public static IHostBuilder ConfigureServices(this IHostBuilder builder)
    {
        #region Configuration

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        });

        #endregion

        #region Services

        builder.ConfigureServices((context, services) =>
        {
            #region Options
            
            services.Configure<ImageShrinkerWebJob.Configuration.Azure>(context.Configuration.GetRequiredSection(nameof(ImageShrinkerWebJob.Configuration.Azure)));
            services.Configure<ImageProcessing>(context.Configuration.GetRequiredSection(nameof(ImageProcessing)));
            
            #endregion
            
            services.AddSingleton<ImageSharpEncoderProvider>();
            services.AddSingleton<IImageResizer, ImageSharpImageResizer>();
            
            services.AddAzureClients(clients =>
            {
                var storageConnectionString = context.Configuration.GetConnectionString("AzureStorage");
                clients.AddBlobServiceClient(storageConnectionString);
                clients.AddQueueServiceClient(storageConnectionString);
            });
            
        });

        #endregion

        #region Logging

        builder.ConfigureLogging((context, loggingBuilder) =>
        {
            loggingBuilder.AddConsole();
            loggingBuilder.AddApplicationInsights(configureTelemetryConfiguration: (config) =>
            {
                config.ConnectionString = context.Configuration.GetConnectionString("AzureApplicationInsights");
            }, configureApplicationInsightsLoggerOptions: _ => { });
        });

        #endregion
        
        #region Other

        builder.ConfigureWebJobs(options =>
        {
            options.AddAzureStorageBlobs();
            options.AddAzureStorageQueues();
        });

        #endregion
        
        return builder;
    }

    public static void Configure(this IHost app)
    {
        #region Ensure Azure service clients existing

        var azureOptions = app.Services.GetRequiredService<IOptions<Configuration.Azure>>();
        var blobServiceClient = app.Services.GetRequiredService<BlobServiceClient>();
        var queueServiceClient = app.Services.GetRequiredService<QueueServiceClient>();

        blobServiceClient.GetBlobContainerClient(azureOptions.Value.SourceImagesContainerName)
            .CreateIfNotExists(PublicAccessType.BlobContainer);
        
        blobServiceClient.GetBlobContainerClient(azureOptions.Value.ProcessedImagesContainerName)
            .CreateIfNotExists(PublicAccessType.BlobContainer);

        queueServiceClient.GetQueueClient(azureOptions.Value.QueueName)
            .CreateIfNotExists();

        #endregion
        
        WebJobFunctions.SetServiceProvider(app.Services);
        
    }
    
}