using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Windows.Media.Imaging;

namespace Stock.Adapter
{
    class ImageAdapter
    {
        public static BitmapImage ImageConvert(Image bmp)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new System.IO.MemoryStream(ms.ToArray());
            bi.EndInit();
            return bi;
        }
        public static Color ColorConvert(System.Windows.Media.Color color)
        {
            return Color.FromArgb(color.R, color.G, color.B);
        }
    }
}