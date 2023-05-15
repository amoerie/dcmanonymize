using System.Collections.Generic;
using System.Runtime.InteropServices;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.Render;
using FellowOakDicom.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DcmAnonymize.Imaging;

    public class ImageSharpImage : ImageBase<Image<Bgra32>>
    {

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes an instance of the <see cref="ImageSharpImage"/> object.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public ImageSharpImage(int width, int height)
            : base(width, height, new PinnedIntArray(width * height), null!)
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="ImageSharpImage"/> object.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="pixels">Pixel array.</param>
        /// <param name="image">Bitmap image.</param>
        private ImageSharpImage(int width, int height, PinnedIntArray pixels, Image<Bgra32> image)
            : base(width, height, pixels, image)
        {
        }

        #endregion

        #region Properties

        public Image<Bgra32> RenderedImage => image;

        #endregion

        #region METHODS

        /// <inheritdoc />
        public override void Render(int components, bool flipX, bool flipY, int rotation)
        {
            var data = new byte[pixels.ByteSize];
            Marshal.Copy(pixels.Pointer, data, 0, pixels.ByteSize);
            image = Image.LoadPixelData<Bgra32>(data, width, height);
            var (flipMode, rotationMode) = GetFlipAndRotateMode(flipX, flipY, rotation);
            if (flipMode != FlipMode.None && rotationMode != RotateMode.None)
            {
                image.Mutate(x => x.RotateFlip(rotationMode, flipMode));
            }
        }

        private (FlipMode, RotateMode) GetFlipAndRotateMode(bool flipX, bool flipY, int rotation)
        {
            FlipMode flipMode;
            if (flipX && flipY)
            {
                // flipping both horizontally and vertically is equal to rotating 180 degrees
                rotation += 180;
                flipMode = FlipMode.None;
            }
            else if (flipX)
            {
                flipMode = FlipMode.Horizontal;
            }
            else if (flipY)
            {
                flipMode = FlipMode.Vertical;
            }
            else
            {
                flipMode = FlipMode.None;
            }

            RotateMode rotationMode;
            switch (rotation % 360)
            {
                case 90: rotationMode = RotateMode.Rotate90; break;
                case 180: rotationMode = RotateMode.Rotate180; break;
                case 270: rotationMode = RotateMode.Rotate270; break;
                default: rotationMode = RotateMode.None; break;
            }

            return (flipMode, rotationMode);
        }


        /// <inheritdoc />
        public override void DrawGraphics(IEnumerable<IGraphic> graphics)
        {
            foreach (var graphic in graphics)
            {
                var layer = (graphic.RenderImage(null) as ImageSharpImage)!.image;
                image.Mutate(ctx => ctx
                    .DrawImage(layer, new Point(graphic.ScaledOffsetX, graphic.ScaledOffsetY), 1));
            }
        }
 

        /// <inheritdoc />
        public override FellowOakDicom.Imaging.IImage Clone()
        {
            return new ImageSharpImage(width, height, new PinnedIntArray(pixels.Data), image?.Clone()!);
        }

        #endregion

    }
