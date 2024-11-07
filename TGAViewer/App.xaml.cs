﻿using Lib.Tga;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

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


            
            
            MainWindow main = new MainWindow();
            //main.FilePath= path;
            // main.DataContext = new MainWindowViewModel(path);

            
            main.ShowDialog();
        }

    }
}