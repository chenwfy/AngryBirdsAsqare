using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 游戏界面相关图片资源数据辅助类
    /// </summary>
    public static class ResIcon
    {
        #region 从资源文件中读取数据
        /// <summary>
        /// 资源数据字典
        /// </summary>
        private static Dictionary<string, byte[]> iconResDict = new Dictionary<string, byte[]>();

        /// <summary>
        /// 读取资源
        /// </summary>
        public static void ReadData()
        {
            iconResDict = ResName.IconData.LoadToDictBytes();
        }
        #endregion

        #region 将资源数据加载为可用的图片数据对象
        /// <summary>
        /// 开始游戏界面LOGO图片
        /// </summary>
        public static BitmapImage Logo { get; private set; }

        /// <summary>
        /// 开始游戏界面设置按钮遮罩图片
        /// </summary>
        public static BitmapImage Shadow { get; private set; }

        /// <summary>
        /// 难度列表图片
        /// </summary>
        public static List<BitmapImage> Levels { get; private set; }

        /// <summary>
        /// 关卡列表图片
        /// </summary>
        public static List<BitmapImage> Gates { get; private set; }

        /// <summary>
        /// 高亮点图片
        /// </summary>
        public static BitmapImage DotLight { get; private set; }

        /// <summary>
        /// 暗色点图片
        /// </summary>
        public static BitmapImage DotDark { get; private set; }

        /// <summary>
        /// 高亮星形图片
        /// </summary>
        public static BitmapImage StarLight { get; private set; }

        /// <summary>
        /// 暗色星形图片
        /// </summary>
        public static BitmapImage StarDark { get; private set; }

        /// <summary>
        /// 游戏结束弹出框背景图片
        /// </summary>
        public static BitmapImage GameOver_Bg { get; private set; }

        /// <summary>
        /// 游戏界面弹出菜单背景图片
        /// </summary>
        public static BitmapImage GameMenu_Bg { get; private set; }

        /// <summary>
        /// 游戏界面创造新记录提示图片
        /// </summary>
        public static BitmapImage NewHeight { get; private set; }

        /// <summary>
        /// 游戏界面倒计时提示背景图片
        /// </summary>
        public static BitmapImage Time_Bg { get; private set; }

        /// <summary>
        /// 游戏界面倒计时进度提示图片
        /// </summary>
        public static BitmapImage Time_Fore { get; private set; }

        /// <summary>
        /// 游戏界面连接线提示图片
        /// </summary>
        public static List<BitmapImage> LinkLines { get; private set; }

        /// <summary>
        /// 加载资源
        /// </summary>
        public static void Load()
        {
            Logo = iconResDict["icon_logo.png"].ToBitmapImage();
            Shadow = iconResDict["icon_shadow.png"].ToBitmapImage();
            Levels = new List<BitmapImage>
            {
                iconResDict["icon_level1.png"].ToBitmapImage(),
                iconResDict["icon_level2.png"].ToBitmapImage(),
                iconResDict["icon_level3.png"].ToBitmapImage(),
                iconResDict["icon_level4.png"].ToBitmapImage()
            };
            Gates = new List<BitmapImage>
            {
                iconResDict["icon_gate_lock.png"].ToBitmapImage(),
                iconResDict["icon_gate_level1.png"].ToBitmapImage(),
                iconResDict["icon_gate_level2.png"].ToBitmapImage(),
                iconResDict["icon_gate_level3.png"].ToBitmapImage(),
                iconResDict["icon_gate_level4.png"].ToBitmapImage()
            };
            DotLight = iconResDict["icon_dotL.png"].ToBitmapImage();
            DotDark = iconResDict["icon_dotD.png"].ToBitmapImage();
            StarLight = iconResDict["icon_star_l.png"].ToBitmapImage();
            StarDark = iconResDict["icon_star_d.png"].ToBitmapImage();
            GameOver_Bg = iconResDict["icon_over.png"].ToBitmapImage();
            GameMenu_Bg = iconResDict["icon_menu_bg.png"].ToBitmapImage();
            NewHeight = iconResDict["icon_newheight.png"].ToBitmapImage();
            Time_Bg = iconResDict["icon_time_bg.png"].ToBitmapImage();
            Time_Fore = iconResDict["icon_time_p.png"].ToBitmapImage();
            LinkLines = new List<BitmapImage>
            {
                iconResDict["icon_path_H.png"].ToBitmapImage(),
                iconResDict["icon_path_V.png"].ToBitmapImage(),
                iconResDict["icon_path_LT.png"].ToBitmapImage(),
                iconResDict["icon_path_RT.png"].ToBitmapImage(),
                iconResDict["icon_path_LD.png"].ToBitmapImage(),
                iconResDict["icon_path_RD.png"].ToBitmapImage()
            };
        }
        #endregion
    }
}