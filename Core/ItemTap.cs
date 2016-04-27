using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 方块单点对象信息类
    /// </summary>
    public class ItemTap
    {
        /// <summary>
        /// 方块控件名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 方块所在网格序号
        /// </summary>
        public int ItemGrid { get; set; }

        /// <summary>
        /// 方块图片序号
        /// </summary>
        public int ItemResCode { get; set; }

        /// <summary>
        /// 方块图片对象
        /// </summary>
        public Image ItemImage { get; set; }
    }
}
