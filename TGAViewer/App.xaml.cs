using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using TGAViewer.MVVM.ViewModel;

namespace TGAViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App:Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            var args = Environment.GetCommandLineArgs();
            string path = "";

            if (args.Length <= 1)
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "TGA Files|*.tga";
                dialog.Title = "Select tga to open";
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    path = dialog.FileName;
                } else
                {
                    MessageBox.Show("Please specify path to image!");
                    Environment.Exit(0);
                }
            }
            path = args[1];
            
            MainWindow main = new MainWindow();
            main.DataContext = new MainWindowViewModel(path);
            main.ShowDialog();
        }

    }
}