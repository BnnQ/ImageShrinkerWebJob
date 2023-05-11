using ImageShrinkerWebJob.Services.Abstractions;

namespace ImageShrinkerWebJob.Services;

public class ImageSharpImageResizer : IImageResizer
{
    private readonly ImageSharpEncoderProvider encoderProvider;

    public ImageSharpImageResizer(ImageSharpEncoderProvider encoderProvider)
    {
        this.encoderProvider = encoderProvider;
    }

    public Task<Stream> ResizeImageAsync(string pathToImage, Size newSize)
    {
        return ResizeImageAsync(File.OpenRead(pathToImage), Path.GetExtension(pathToImage), newSize);
    }
    
    public async Task<Stream> ResizeImageAsync(Stream imageStream, string imageExtension, Size newSize)
    {
        using var image = await Image.LoadAsync(imageStream);
        image.Mutate(mutatingBuilder =>
        {
            mutatingBuilder.Resize(GetResizeOptions(newSize));
        });

        var resultStream = new MemoryStream();
        await image.SaveAsync(resultStream, encoderProvider.GetEncoder(imageExtension));
        resultStream.Position = 0;
        return resultStream;
    }

    public Task ResizeImageAsync(string pathToImage, Size newSize, string pathToResult)
    {
        return ResizeImageAsync(File.OpenRead(pathToImage), newSize, pathToResult);
    }

    public async Task ResizeImageAsync(Stream imageStream, Size newSize, string pathToResult)
    {
        using var image = await Image.LoadAsync(imageStream);
        image.Mutate(mutatingBuilder =>
        {
            mutatingBuilder.Resize(GetResizeOptions(newSize));
        });

        await image.SaveAsync(pathToResult);
    }

    private static ResizeOptions GetResizeOptions(Size size)
    {
        return new ResizeOptions { Size = size, Mode = ResizeMode.Max };
    }

}