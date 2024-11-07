using System.Collections.Concurrent;
using System.IO;
using System.Windows.Media.Imaging;

namespace Lib.Tga
{
    public sealed class TgaEngine
    {
        private static readonly ConcurrentDictionary<string, TgaProvider> Tgas = new ConcurrentDictionary<string, TgaProvider>();

        public static BitmapSource GetBitmap(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) return null;
            var data = GoTgaImage(fileName, 1);
            return data?.GetBitmap();
        }

       

        public static TgaImage GoTgaImage(string filePath, int indexkey)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            Tgas.TryGetValue(filePath, out var tga);
            return tga?.GoTgaImage(indexkey);
        }

        public static bool IsReady(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            Tgas.TryGetValue(filePath, out var tga);
            return tga != null && tga.IsReady(filePath);
        }

        public static int GetTgaImageCount(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return 0;
            Tgas.TryGetValue(filePath, out var tga);
            if (tga != null)
            {
                return tga.GetTgaImageCount();
            }
            return 0;
        }
    }

    public sealed class TgaProvider
    {
        private readonly ConcurrentDictionary<int, TgaImage> Tgas = new ConcurrentDictionary<int, TgaImage>();

        private bool _isReady;

        private bool AddTga(string fileName, int i)
        {
            bool temp = false;
            if (!Tgas.ContainsKey(i))
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new BinaryReader(fs))
                    {
                        temp = Tgas.TryAdd(i, new TgaImage(reader));
                    }
                }
            }
            return temp;
        }

        static bool IsDirectory(string filename)
        {
            FileAttributes fa = new FileInfo(filename).Attributes;
            return fa != (FileAttributes)(-1) && (fa & FileAttributes.Directory) != 0;
        }

        

        public TgaImage GoTgaImage(int indexkey)
        {
            if (Tgas.TryGetValue(indexkey, out var temp))
                return temp;
            return null;
        }

        public int GetTgaImageCount()
        {
            return Tgas.Count;
        }

        public bool IsReady(string filePath)
        {
            return _isReady;
        }
    }
}
