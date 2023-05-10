using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;

namespace ImageShrinkerWebJob.Services;

public class ImageSharpEncoderProvider
{
    private JpegEncoder? jpegEncoder;
    private WebpEncoder? webpEncoder;
    private BmpEncoder? bmpEncoder;
    private GifEncoder? gifEncoder;
    private PbmEncoder? pbmEncoder;
    private PngEncoder? pngEncoder;
    private TgaEncoder? tgaEncoder;
    private TiffEncoder? tiffEncoder;
    
    /// <exception cref="ArgumentException">If image format is not supported (supported formats: jpg, jpeg, webp, bmp, gif, pbm, png, tga, tiff)</exception>
    public IImageEncoder GetEncoder(string imageExtension)
    {
        imageExtension = imageExtension.Replace(".", string.Empty);

        switch (imageExtension)
        {
            case "jpg":
            case "jpeg":
                return jpegEncoder ??= new JpegEncoder();
            case "webp":
                return webpEncoder ??= new WebpEncoder();
            case "bmp":
                return bmpEncoder ??= new BmpEncoder();
            case "gif":
                return gifEncoder ??= new GifEncoder();
            case "pbm":
                return pbmEncoder ??= new PbmEncoder();
            case "png":
                return pngEncoder ??= new PngEncoder();
            case "tga":
                return tgaEncoder ??= new TgaEncoder();
            case "tiff":
                return tiffEncoder ??= new TiffEncoder();
            default:
                throw new ArgumentException(message: "This image format is not supported.",
                    paramName: nameof(imageExtension));
        }
    }
}