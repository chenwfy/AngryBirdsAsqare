using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 位图合成辅助类
    /// </summary>
    public static class ImageCombine
    {
        /// <summary>
        /// 将数字字符转换为位图对象
        /// </summary>
        /// <param name="numberChars">待转换的数字字符，只能包含（0-9和-和/），此处不做异常处理，调用时注意</param>
        /// <returns>位图对象</returns>
        public static WriteableBitmap ToWriteableBitmap(this string numberChars)
        {
            return numberChars.ToWriteableBitmap(NumberCharSize.None);
        }

        /// <summary>
        /// 将数字字符转换为位图对象
        /// </summary>
        /// <param name="numberChars">待转换的数字字符，只能包含（0-9和-和/），此处不做异常处理，调用时注意</param>
        /// <param name="size">数字字符字体大小</param>
        /// <returns>位图对象</returns>
        public static WriteableBitmap ToWriteableBitmap(this string numberChars, NumberCharSize size)
        {
            List<BitmapImage> bmpList = GetBmpList(numberChars, size);
            int wBmpHeight = bmpList[0].PixelHeight;
            int wBmpWidth = bmpList.Select(b => b.PixelWidth).Sum();

            WriteableBitmap writeableBitmap = new WriteableBitmap(wBmpWidth, wBmpHeight);
            TranslateTransform translateTransform = new TranslateTransform { X = 0, Y = 0 };
            Image image = new Image();

            foreach (BitmapImage bmp in bmpList)
            {
                image.Source = bmp;
                writeableBitmap.Render(image, translateTransform);
                translateTransform.X += bmp.PixelWidth;
            }

            writeableBitmap.Invalidate();

            return writeableBitmap; 
        }

        /// <summary>
        /// 将多张图片对象合成一张图片
        /// </summary>
        /// <param name="sources">将多张图片对象</param>
        /// <param name="width">合成后图片对象的宽度</param>
        /// <param name="height">合成后图片对象的高度</param>
        /// <returns>合成后的图片对象</returns>
        public static WriteableBitmap CombineImages(this List<CombineSource> sources, int width, int height)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height);
            Image image = new Image();
            foreach (CombineSource item in sources)
            {
                image.Source = item.Source;
                writeableBitmap.Render(image, new TranslateTransform { X = item.Position.X, Y = item.Position.Y });
            }
            writeableBitmap.Invalidate();
            return writeableBitmap;
        }

        /// <summary>
        /// 根据得分星级得到显示的图片对象
        /// </summary>
        /// <param name="star">得分星级</param>
        /// <returns>得分星级的显示图片对象</returns>
        public static WriteableBitmap ToStarBitmap(this int star)
        {
            BitmapImage bmpLS = ResIcon.StarLight;
            WriteableBitmap writeableBitmap = new WriteableBitmap(bmpLS.PixelWidth * 3 + 15, bmpLS.PixelHeight);
            TranslateTransform translateTransform = new TranslateTransform { X = 5, Y = 0 };
            Image image = new Image();
            for (int s = 1; s <= 3; s++)
            {
                if (s <= star)
                {
                    image.Source = bmpLS;
                    writeableBitmap.Render(image, translateTransform);
                    translateTransform.X += bmpLS.PixelWidth + 5;
                }
            }
            writeableBitmap.Invalidate();
            return writeableBitmap;
        }

        /// <summary>
        /// 获取数字字符串对应的原始位图数据集合
        /// </summary>
        /// <param name="numberChars">数字字符串</param>
        /// <param name="size">数字字符字体大小</param>
        /// <returns>原始位图数据集合</returns>
        private static List<BitmapImage> GetBmpList(string numberChars, NumberCharSize size)
        {
            Dictionary<string, BitmapImage> bmpDict = GetBmpDict(size);
            List<BitmapImage> bmpList = new List<BitmapImage>();
            foreach (char num in numberChars.ToCharArray())
            {
                bmpList.Add(bmpDict[num.ToString()]);
            }
            return bmpList;
        }

        /// <summary>
        /// 根据字体大小获取对应位图数据字典
        /// </summary>
        /// <param name="size">数字字符字体大小</param>
        /// <returns>位图数据字典</returns>
        private static Dictionary<string, BitmapImage> GetBmpDict(NumberCharSize size)
        {
            Dictionary<string, BitmapImage> bmpDict = new Dictionary<string, BitmapImage>();
            switch (size)
            {

                case NumberCharSize.Sixteen:
                    bmpDict = ResTxt.Number16;
                    break;
                case NumberCharSize.Twentyfour:
                    bmpDict = ResTxt.Number24;
                    break;
                case NumberCharSize.Fortyeight:
                    bmpDict = ResTxt.Number48;
                    break;
                case NumberCharSize.None:
                case NumberCharSize.Twelve:
                default:
                    bmpDict = ResTxt.Number12;
                    break;
            }
            return bmpDict;
        }
    }
}