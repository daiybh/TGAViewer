using Lib.Tga;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using Lib.Framework;

namespace TGAViewer
{
    public partial class MainWindow : Window
    {
        string FolderPath = "E:\\TGA\\DualAlpha";
        DispatcherTimer dispatcherTimer = null;
        public MainWindow()
        {
            InitializeComponent();
            
            string path = "";

            
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = "TGA Files|*.tga";
                dialog.Title = "Select tga to open";
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    path = dialog.FileName;
                    //get folder Path from Path
                    FolderPath = System.IO.Path.GetDirectoryName(path);
                }
                else
                {
                    MessageBox.Show("Please specify path to image!");
                    Environment.Exit(0);
                }
            }
            
            LoadFileToListBox();
        }
        void LoadFileToListBox()
        {
            var files = Directory.GetFiles(FolderPath, "*.tga", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                int number;
                if (int.TryParse(fileName, out number))
                {
                    // 1000
                    number /= 1000;
                    
                }
                listBox.Items.Add(fileName);
            }
        }
        int g_count = 0;
        public string GenerateNextFilePath(string filePath)
        {
            // 获取文件名和扩展名
            string directory = System.IO.Path.GetDirectoryName(filePath);
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = System.IO.Path.GetExtension(filePath);

            // 尝试解析文件名中的数字部分
            int number;
            if (int.TryParse(fileNameWithoutExtension, out number))
            {
                // 如果文件名是数字，则递增
                number++;
                return System.IO.Path.Combine(directory, number.ToString() + fileExtension);
            }
            else
            {
                // 如果文件名不是数字，则在文件名后添加 _1
                return System.IO.Path.Combine(directory, $"{fileNameWithoutExtension}_1{fileExtension}");
            }
        }
        void LoadTGA(string FilePath)
        {            
            this.Title = FilePath;
            try
            {
                using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new BinaryReader(fs))
                    {
                        xxxxb.Source = new TgaImage(reader).GetBitmap();
                        //main.DataContext = new TgaImage(reader).GetBitmap();
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }
        void loadPreTGA()
        {
            if (listBox.Items.Count > 0)
            {
                int nextIndex = (listBox.SelectedIndex - 1) % listBox.Items.Count;
                if (nextIndex < 0) nextIndex = 0;
                listBox.SelectedIndex = nextIndex;
                listBox.ScrollIntoView(listBox.Items[nextIndex]);
            }
        }
       
        void loadNextTGA()
        {
            if (listBox.Items.Count > 0)
            {
                int nextIndex = (listBox.SelectedIndex + 1) % listBox.Items.Count;
                listBox.SelectedIndex = nextIndex;
                listBox.ScrollIntoView(listBox.Items[nextIndex]);
            }
        }
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Left)
            {
                loadPreTGA();
            }
            else if (e.Key == System.Windows.Input.Key.Right)
            {
                loadNextTGA();
            }
            else if (e.Key== System.Windows.Input.Key.Space)
            {
                if (dispatcherTimer==null)
                {
                    dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Interval = TimeSpan.FromMicroseconds(40);
                    dispatcherTimer.Tick += (s, ea) =>
                    {
                        loadNextTGA();                     
                    };
                    dispatcherTimer.Start();
                }
                else
                {
                    dispatcherTimer.Stop();
                    dispatcherTimer = null;
                }
            }
        }

        private void listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedFileName = listBox.SelectedItem as string;
            if (selectedFileName != null)
            {
                string FilePath = System.IO.Path.Combine(FolderPath, $"{selectedFileName}.tga");
                LoadTGA(FilePath);
            }
        }
        public void Scale(double dx, double dy, double pivotX, double pivotY)
        {
            xxxxb?.Dispatcher?.Invoke(() =>
            {
                TransformGroup group = (TransformGroup)xxxxb.RenderTransform;
                ScaleTransform scale = (ScaleTransform)group.Children[0];
                TranslateTransform translate = (TranslateTransform)group.Children[1];

                double pivotXInObjectSpace = (pivotX - translate.X) / scale.ScaleX;
                double pivotYInObjectSpace = (pivotY - translate.Y) / scale.ScaleY;

                scale.ScaleX = (scale.ScaleX + dx).Clamp(1, 99);
                scale.ScaleY = (scale.ScaleY + ( dy)).Clamp(1,99);

                double newX = pivotX - pivotXInObjectSpace * scale.ScaleX;
                double newY = pivotY - pivotYInObjectSpace * scale.ScaleY;
                var p = new Point(xxxxb.ActualWidth, xxxxb.ActualHeight);
                translate.X = newX.Clamp(p.X - scale.ScaleX * p.X, 0);
                translate.Y = newY.Clamp(p.Y - scale.ScaleY * p.Y, 0);

                //xxxxb.Scale.X = scale.ScaleX;
                //xxxxb.Scale.Y = scale.ScaleY;

                //_player.Translate.X = translate.X / _player.OriginalSize.X;
                //_player.Translate.Y = translate.Y / _player.OriginalSize.Y;

               // xxxxb.Zoom = scale.ScaleX;
            }, DispatcherPriority.Normal);

        }
       
        private void xxxxb_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {

            var center = e.GetPosition(showGrid);

            double temp = e.Delta * 0.001;
            
            temp = temp.Clamp(-99, 99);
         
            Scale(temp, temp, center.X, center.Y);
            // SetTransform(_player.Translate.X, _player.Translate.Y, _player.Scale.X + e.Delta / 1000, _player.Scale.Y)
        }
    }
}