using FellowOakDicom.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DcmAnonymize.Imaging;

/// <summary>
/// Convenience class for non-generic access to <see cref="ImageSharpImage"/> image objects.
/// </summary>
public static class ImageSharpImageExtensions
{

    /// <summary>
    /// Convenience method to access ImageSharpImage <see cref="IImage"/> instance as ImageSharp <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="iimage"><see cref="IImage"/> object.</param>
    /// <returns><see cref="Image"/> contents of <paramref name="image"/>.</returns>
    public static Image<Bgra32> AsSharpImage(this IImage iimage)
    {
        return (iimage as ImageSharpImage)!.RenderedImage;
    }

}
