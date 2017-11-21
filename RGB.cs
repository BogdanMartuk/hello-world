using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Rozpiznavannya_obraziv
{
    class RGB
    {
        /// <summary>
        /// Метод загружает растровые изображения без блокирования файла
        /// (как это делает конструктор Bitmap(fileName)).
        /// </summary>
        /// <param name="fileName">Имя файла для загрузки.</param>
        /// <returns>Экземпляр Bitmap.</returns>
        public static Bitmap LoadBitmap(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return new Bitmap(fs);
        }

        public static byte[,,] BitmapToByteRgbNaive(Bitmap bmp)
        {
            int width = bmp.Width,
                height = bmp.Height;
            byte[,,] res = new byte[3, height, width];
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color color = bmp.GetPixel(x, y);
                    res[0, y, x] = Limit(color.R);
                    res[1, y, x] = Limit(color.G);
                    res[2, y, x] = Limit(color.B);
                }
            }
            return res;
        }

        public static byte Limit(double x)
        {
            if (x < 0)
                return 0;
            if (x > 255)
                return 255;
            return (byte)x;
        }

        public static Bitmap RgbToBitmapNaive(byte[,,] rgb)
        {
            if ((rgb.GetLength(0) != 3))
            {
                throw new ArrayTypeMismatchException("Size of first dimension for passed array must be 3 (RGB components)");
            }

            int width = rgb.GetLength(2),
                height = rgb.GetLength(1);

            Bitmap result = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    result.SetPixel(x, y, Color.FromArgb(rgb[0, y, x], rgb[1, y, x], rgb[2, y, x]));
                }
            }

            return result;
        }
    }
}
