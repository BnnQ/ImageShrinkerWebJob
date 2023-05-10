namespace ImageShrinkerWebJob.Services.Abstractions;

public interface IImageResizer
{
    public Task<Stream> ResizeImageAsync(string pathToImage, Size newSize);
    public Task ResizeImageAsync(string pathToImage, Size newSize, string pathToResult);
}