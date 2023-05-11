namespace ImageShrinkerWebJob.Services.Abstractions;

public interface IImageResizer
{
    public Task<Stream> ResizeImageAsync(string pathToImage, Size newSize);
    public Task<Stream> ResizeImageAsync(Stream imageStream, string imageExtension, Size newSize);
    public Task ResizeImageAsync(string pathToImage, Size newSize, string pathToResult);
    public Task ResizeImageAsync(Stream imageStream, Size newSize, string pathToResult);
}