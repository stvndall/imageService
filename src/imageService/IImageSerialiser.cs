using System;
using System.Drawing;
using System.IO;

namespace imageService
{
    public interface IImageSerialiser
    {
        IImage GetOriginalImage(string imageB64);

    }

    public class ImageSerialiser : IImageSerialiser
    {
        public IImage GetOriginalImage(string imageB64)
        {
            using (var readStream = new MemoryStream(Convert.FromBase64String(imageB64)))
            {
                using (var writeStream = new MemoryStream())
                {
                    var image = Image.FromStream(readStream);
                    var format = image.RawFormat;
                    image.Save(writeStream, format);
                    return new LocalImage(writeStream.ToArray(), format);
                }
            }
        }
    }
}