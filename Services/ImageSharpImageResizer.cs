using ImageShrinkerWebJob.Services.Abstractions;

namespace ImageShrinkerWebJob.Services;

public class ImageSharpImageResizer : IImageResizer
{
    private readonly ImageSharpEncoderProvider encoderProvider;

    public ImageSharpImageResizer(ImageSharpEncoderProvider encoderProvider)
    {
        this.encoderProvider = encoderProvider;
    }

    public async Task<Stream> ResizeImageAsync(string pathToImage, Size newSize)
    {
        using var image = await Image.LoadAsync(pathToImage);
        image.Mutate(mutatingBuilder =>
        {
            mutatingBuilder.Resize(GetResizeOptions(newSize));
        });

        var resultStream = new MemoryStream();
        await image.SaveAsync(resultStream, encoderProvider.GetEncoder(Path.GetExtension(pathToImage)));
        return resultStream;
    }

    public async Task ResizeImageAsync(string pathToImage, Size newSize, string pathToResult)
    {
        using var image = await Image.LoadAsync(pathToImage);
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