using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 全屏背景图片资源数据辅助类
    /// </summary>
    public static class ResScreen
    {
        #region 从资源文件中读取数据
        /// <summary>
        /// 资源数据字典
        /// </summary>
        private static Dictionary<string, byte[]> screenResDict = new Dictionary<string, byte[]>();

        /// <summary>
        /// 读取资源
        /// </summary>
        public static void ReadData()
        {
            screenResDict = ResName.ScreenBgData.LoadToDictBytes();
        }
        #endregion

        #region 将资源数据加载为可用的图片数据对象
        /// <summary>
        /// 开始游戏界面背景图片
        /// </summary>
        public static BitmapImage StartBg { get; private set; }

        /// <summary>
        /// 难度选择界面背景图片
        /// </summary>
        public static BitmapImage LevelBg { get; private set; }

        /// <summary>
        /// 难度关卡选择界面背景图片
        /// </summary>
        public static List<BitmapImage> GateBgs { get; private set; }

        /// <summary>
        /// 游戏界面背景图片
        /// </summary>
        public static BitmapImage GameBg { get; private set; }

        /// <summary>
        /// 加载资源
        /// </summary>
        public static void Load()
        {
            StartBg = screenResDict["scr_start.jpg"].ToBitmapImage();
            LevelBg = screenResDict["scr_level.jpg"].ToBitmapImage();
            GateBgs = new List<BitmapImage> 
            { 
                screenResDict["scr_gate1.jpg"].ToBitmapImage(),
                screenResDict["scr_gate2.jpg"].ToBitmapImage(),
                screenResDict["scr_gate3.jpg"].ToBitmapImage(),
                screenResDict["scr_gate4.jpg"].ToBitmapImage()
            };
            GameBg = screenResDict["scr_game.jpg"].ToBitmapImage();
        }
        #endregion
    }
}