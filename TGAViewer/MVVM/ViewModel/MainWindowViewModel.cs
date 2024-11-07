using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace TGAViewer.MVVM.ViewModel;

public class MainWindowViewModel
{

    public MainWindowViewModel(string path)
    {
        _path = path;
    }
    private string _path;

    private BitmapSource? _image;

    public BitmapSource Image
    {
        get
        {
           
            return _image;
        }
    }

    public double MinHeight => Image.Height / 10;
    public double MinWidth => Image.Width / 10;
}