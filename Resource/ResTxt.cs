using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 游戏界面相关文字图片资源数据辅助类
    /// </summary>
    public static class ResTxt
    {
        #region 从资源文件中读取数据
        /// <summary>
        /// 资源数据字典
        /// </summary>
        private static Dictionary<string, byte[]> txtResDict = new Dictionary<string, byte[]>();

        /// <summary>
        /// 读取资源
        /// </summary>
        public static void ReadData()
        {
            txtResDict = ResName.TxtData.LoadToDictBytes();
        }
        #endregion

        #region 将资源数据加载为可用的图片数据对象
        /// <summary>
        /// 12号字体的数字图片
        /// </summary>
        public static Dictionary<string, BitmapImage> Number12 = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// 16号字体的数字图片
        /// </summary>
        public static Dictionary<string, BitmapImage> Number16 = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// 24号字体的数字图片
        /// </summary>
        public static Dictionary<string, BitmapImage> Number24 = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// 48号字体的数字图片
        /// </summary>
        public static Dictionary<string, BitmapImage> Number48 = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// 倒计时提示文字图片
        /// </summary>
        public static BitmapImage Time { get; private set; }

        /// <summary>
        /// 当前得分提示文字图片
        /// </summary>
        public static BitmapImage Score { get; private set; }

        /// <summary>
        /// 剩余方块数量提示文字图片
        /// </summary>
        public static BitmapImage Last { get; private set; }

        /// <summary>
        /// 最高历史记录文字图片
        /// </summary>
        public static BitmapImage Highest { get; private set; }

        /// <summary>
        /// 游戏胜利文字图片
        /// </summary>
        public static BitmapImage Win { get; private set; }

        /// <summary>
        /// 游戏失败文字图片
        /// </summary>
        public static BitmapImage Fail { get; private set; }

        /// <summary>
        /// 本次成绩文字图片
        /// </summary>
        public static BitmapImage Grade { get; private set; }

        /// <summary>
        /// 连击提示文字图片
        /// </summary>
        public static BitmapImage Combo { get; private set; }

        /// <summary>
        /// 成绩加剩余时间提示文字图片
        /// </summary>
        public static BitmapImage AddLastTime { get; private set; }

        /// <summary>
        /// 加载资源 
        /// </summary>
        public static void Load()
        {
            string[] keys = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "-", "/" };
            string[] codes = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "j", "x" };
            for (int i = 0; i < keys.Length; i++)
            {
                Number12.Add(keys[i], txtResDict[string.Format("txt_c{0}.png", codes[i])].ToBitmapImage());
                Number16.Add(keys[i], txtResDict[string.Format("txt_s{0}.png", codes[i])].ToBitmapImage());
                Number24.Add(keys[i], txtResDict[string.Format("txt_m{0}.png", codes[i])].ToBitmapImage());
                Number48.Add(keys[i], txtResDict[string.Format("txt_l{0}.png", codes[i])].ToBitmapImage());
            }
            Time = txtResDict["txt_t1.png"].ToBitmapImage();
            Score = txtResDict["txt_t2.png"].ToBitmapImage();
            Last = txtResDict["txt_t3.png"].ToBitmapImage();
            Highest = txtResDict["txt_t4.png"].ToBitmapImage();
            Win = txtResDict["txt_t5.png"].ToBitmapImage();
            Fail = txtResDict["txt_t6.png"].ToBitmapImage();
            Grade = txtResDict["txt_t7.png"].ToBitmapImage();
            Combo = txtResDict["txt_t8.png"].ToBitmapImage();
            AddLastTime = txtResDict["txt_t9.png"].ToBitmapImage();
        }
        #endregion
    }
}