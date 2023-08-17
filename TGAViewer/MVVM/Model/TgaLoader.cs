using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FreeImageAPI;

namespace TGAViewer.MVVM.Model;

public class TgaLoader {
    public static BitmapSource? LoadAndDisplayTgaImage(string filePath) {
        if (!File.Exists(filePath)) {
            MessageBox.Show("File does not exist");
            Environment.Exit(0);
            return default;
        }


        // try
        // {
        //     FIBITMAP dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_TARGA,filePath,FREE_IMAGE_LOAD_FLAGS.DEFAULT);
        //
        //     IntPtr bits = FreeImage.GetBits(dib);
        //     var width = FreeImage.GetWidth(dib);
        //     var height = FreeImage.GetHeight(dib);
        //     var dpiX = FreeImage.GetDotsPerMeterX(dib);
        //     var dpiY = FreeImage.GetDotsPerMeterY(dib);
        //     var size = FreeImage.GetDIBSize(dib);
        //     
        //     BitmapSource bitmapSource = BitmapSource.Create(
        //         (int)width,
        //         (int)height,
        //         dpiX,
        //         dpiY,
        //         PixelFormats.Bgr32, 
        //         null,
        //         bits,
        //         (int)size *,
        //         (int)(width * 4)); 
        //     FreeImage.UnloadEx(ref dib);
        //
        //     return bitmapSource;
        // } catch (Exception e)
        // {
        //     MessageBox.Show("Could not load file. " + e + " " + filePath);
        //     Environment.Exit(0);
        //     return default;
        // }


        FIBITMAP image = FreeImage.LoadEx(filePath);

        var map = FreeImage.GetBitmap(image);
        
        BitmapSource bitmapSource =  Convert(map);
        FreeImage.UnloadEx(ref image);
        return bitmapSource;

        // return new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute));
    }
    
    
    public static BitmapSource Convert(System.Drawing.Bitmap bitmap)
    {
        var bitmapData = bitmap.LockBits(
            new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

        var bitmapSource = BitmapSource.Create(
            bitmapData.Width, bitmapData.Height,
            bitmap.HorizontalResolution, bitmap.VerticalResolution,
            PixelFormats.Bgr24, null,
            bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

        bitmap.UnlockBits(bitmapData);

        return bitmapSource;
    }
}