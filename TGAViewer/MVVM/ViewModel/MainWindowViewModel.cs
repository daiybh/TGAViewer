using System;
using System.Windows;
using System.Windows.Media.Imaging;
using TGAViewer.MVVM.Model;

namespace TGAViewer.MVVM.ViewModel;

public class MainWindowViewModel {
    private string? _path;

    private string Path {
        get {
            if (_path == null) {
                var args = Environment.GetCommandLineArgs();
                if (args.Length <= 1) {
                    MessageBox.Show("Please specify path to image!");
                    Environment.Exit(0);
                }
                return args[1];
            }

            return _path;
        }
    }

    private BitmapSource? _image;

    public BitmapSource Image {
        get {
            if (_image == null)
                _image = TgaLoader.LoadAndDisplayTgaImage(Path);
            return _image;
        }
    }

    public double MinHeight => Image.Height / 10;
    public double MinWidth => Image.Width / 10;
}