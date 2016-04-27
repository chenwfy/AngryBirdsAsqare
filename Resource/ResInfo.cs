using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 资源信息类
    /// </summary>
    public class ResInfo
    {
        /// <summary>
        /// 自定义资源序号
        /// </summary>
        public int Code { get; set; }
        
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件数据
        /// </summary>
        public BitmapImage Data { get; set; }
    }

    /// <summary>
    /// 奖励图片信息类
    /// </summary>
    public class RewardImage
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 文件在数据文件中的偏移
        /// </summary>
        public long Position { get; set; }
    }

    /// <summary>
    /// 待合成的图片信息类
    /// </summary>
    public class CombineSource
    {
        /// <summary>
        /// 图片数据
        /// </summary>
        public ImageSource Source { get; set; }
        
        /// <summary>
        /// 坐标
        /// </summary>
        public Point Position { get; set; }
    }
}
