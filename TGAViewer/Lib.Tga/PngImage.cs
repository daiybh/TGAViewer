using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TGAViewer.Lib.Tga
{
    internal class PngImage
    {
        public PngImage() { }
        public BitmapSource PngStreamToBitmapSource(Stream stream)
        {
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            // Optional: Reset stream position if needed
            stream.Seek(0, SeekOrigin.Begin);

            return bitmapImage;
        }
    }
}
