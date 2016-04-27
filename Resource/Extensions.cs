using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 扩展辅助类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// uri字符串转换为URI
        /// </summary>
        /// <param name="uri">uri字符串</param>
        /// <returns>转换后的URI</returns>
        public static Uri ToUri(this string uri)
        {
            return new Uri(uri, UriKind.Relative);
        }

        /// <summary>
        /// 从图片字节数组获取图片数据对象
        /// </summary>
        /// <param name="buffer">图片字节数组</param>
        /// <returns>图片数据对象</returns>
        public static BitmapImage ToBitmapImage(this byte[] buffer)
        {
            BitmapImage bmpImg = new BitmapImage();
            using (MemoryStream mStream = new MemoryStream(buffer))
            {
                bmpImg.SetSource(mStream);
                return bmpImg;
            }
        }

        /// <summary>
        /// 获取以资源方式加载的图片数据
        /// </summary>
        /// <param name="imageUri">图片文件路径</param>
        /// <returns>图片数据</returns>
        public static BitmapImage ToBitmapImage(this Uri imageUri)
        {
            return new BitmapImage(imageUri);
        }

        /// <summary>
        /// 获取以资源方式加载的图片数据
        /// </summary>
        /// <param name="imageUri">图片文件路径</param>
        /// <returns>图片数据</returns>
        public static ImageBrush ToImageBrush(this Uri imageUri)
        {
            return new ImageBrush { ImageSource = imageUri.ToBitmapImage() };
        }

        /// <summary>
        /// 由BitmapImage得到ImageBrush
        /// </summary>
        /// <param name="bitmapImage">源BitmapImage</param>
        /// <returns>ImageBrush</returns>
        public static ImageBrush ToImageBrush(this BitmapImage bitmapImage)
        {
            return new ImageBrush { ImageSource = bitmapImage };
        }

        /// <summary>
        /// 由WriteableBitmap得到ImageBrush
        /// </summary>
        /// <param name="writeableBitmap">源WriteableBitmap</param>
        /// <returns>ImageBrush</returns>
        public static ImageBrush ToImageBrush(this WriteableBitmap writeableBitmap)
        {
            return new ImageBrush { ImageSource = writeableBitmap };
        }
    }
}