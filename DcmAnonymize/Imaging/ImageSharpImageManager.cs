using FellowOakDicom.Imaging;

namespace DcmAnonymize.Imaging;

public sealed class ImageSharpImageManager : IImageManager
{
    public IImage CreateImage(int width, int height)
        => new ImageSharpImage(width, height);
}
