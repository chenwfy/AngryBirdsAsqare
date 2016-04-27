using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 游戏界面按钮图片资源数据辅助类
    /// </summary>
    public static class ResButton
    {
        #region 从资源文件中读取数据
        /// <summary>
        /// 资源数据字典
        /// </summary>
        private static Dictionary<string, byte[]> buttonResDict = new Dictionary<string, byte[]>();

        /// <summary>
        /// 读取资源
        /// </summary>
        public static void ReadData()
        {
            buttonResDict = ResName.ButtonData.LoadToDictBytes();
        }
        #endregion

        #region 将资源数据加载为可用的图片数据对象
        /// <summary>
        /// 开始游戏界面“开始按钮”图片
        /// </summary>
        public static BitmapImage Start { get; private set; }

        /// <summary>
        /// 开始游戏界面设置按钮背景图片
        /// </summary>
        public static BitmapImage SetBg { get; private set; }

        /// <summary>
        /// 开始游戏界面设置按钮前景图片
        /// </summary>
        public static BitmapImage SetFore { get; private set; }

        /// <summary>
        /// 开启声音小按钮图片
        /// </summary>
        public static BitmapImage AudioSmall { get; private set; }

        /// <summary>
        /// 开启声音按钮图片
        /// </summary>
        public static BitmapImage AudioNomal { get; private set; }

        /// <summary>
        /// 不可用小按钮图片
        /// </summary>
        public static BitmapImage DisenableSmall { get; private set; }

        /// <summary>
        /// 不可用按钮图片
        /// </summary>
        public static BitmapImage DisenableNomal { get; private set; }

        /// <summary>
        /// 暂停按钮图片
        /// </summary>
        public static BitmapImage Pause { get; private set; }

        /// <summary>
        /// 继续游戏按钮图片
        /// </summary>
        public static BitmapImage Play { get; private set; }

        /// <summary>
        /// 重新排列按钮图片
        /// </summary>
        public static BitmapImage Refresh { get; private set; }

        /// <summary>
        /// 下一关按钮图片
        /// </summary>
        public static BitmapImage Next { get; private set; }

        /// <summary>
        /// 重新开始小按钮图片
        /// </summary>
        public static BitmapImage RePlaySmall { get; private set; }

        /// <summary>
        /// 重新开始按钮图片
        /// </summary>
        public static BitmapImage RePlayNomal { get; private set; }

        /// <summary>
        /// 返回关卡界面小按钮图片
        /// </summary>
        public static BitmapImage GobackSmall { get; private set; }

        /// <summary>
        /// 返回关卡界面按钮图片
        /// </summary>
        public static BitmapImage GobackNomal { get; private set; }

        /// <summary>
        /// 保存按钮图片
        /// </summary>
        public static BitmapImage Save { get; private set; }

        /// <summary>
        /// 关闭按钮图片
        /// </summary>
        public static BitmapImage Close { get; private set; }

        /// <summary>
        /// 加载资源 buttonResDict
        /// </summary>
        public static void Load()
        {
            Start = buttonResDict["btn_start.png"].ToBitmapImage();
            SetBg = buttonResDict["btn_setbg.png"].ToBitmapImage();
            SetFore = buttonResDict["btn_set.png"].ToBitmapImage();
            AudioSmall = buttonResDict["btn_audio.png"].ToBitmapImage();
            AudioNomal = buttonResDict["btn_audio_big.png"].ToBitmapImage();
            DisenableSmall = buttonResDict["btn_disaudio.png"].ToBitmapImage();
            DisenableNomal = buttonResDict["btn_disaudio_big.png"].ToBitmapImage();
            Pause = buttonResDict["btn_pause.png"].ToBitmapImage();
            Play = buttonResDict["btn_play.png"].ToBitmapImage();
            Refresh = buttonResDict["btn_refresh.png"].ToBitmapImage();
            Next = buttonResDict["btn_next.png"].ToBitmapImage();
            RePlaySmall = buttonResDict["btn_agin.png"].ToBitmapImage();
            RePlayNomal = buttonResDict["btn_agin_big.png"].ToBitmapImage();
            GobackSmall = buttonResDict["btn_golevel.png"].ToBitmapImage();
            GobackNomal = buttonResDict["btn_golevel_big.png"].ToBitmapImage();
            Save = buttonResDict["btn_save.png"].ToBitmapImage();
            Close = buttonResDict["btn_close.png"].ToBitmapImage();
        }
        #endregion
    }
}