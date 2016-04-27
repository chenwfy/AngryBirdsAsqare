using System;
using System.Collections.Generic;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 游戏相关资源文件辅助类
    /// </summary>
    public static class ResName
    {
        /// <summary>
        /// 资源数据文件存放目录名
        /// </summary>
        public const string ResFileDirectory = "Content";

        #region 图片资源
        /// <summary>
        /// LOADING图片文件名
        /// </summary>
        public const string LoadingImg = "Loading.jpg";

        /// <summary>
        /// 游戏界面背景图片数据文件名
        /// </summary>
        public const string ScreenBgData = "Screen.res";

        /// <summary>
        /// 游戏界面ICON图片数据文件名
        /// </summary>
        public const string IconData = "Icon.res";

        /// <summary>
        /// 游戏界面按钮图片数据文件名
        /// </summary>
        public const string ButtonData = "Button.res";

        /// <summary>
        /// 游戏界面文字图片数据文件名
        /// </summary>
        public const string TxtData = "Txt.res";

        /// <summary>
        /// 游戏界面ITEM图片数据文件名
        /// </summary>
        public const string ItemData = "Item.res";

        /// <summary>
        /// 游戏界面隐藏关卡奖励图片数据文件名
        /// </summary>
        public const string ImageData = "Image.res";
        #endregion

        #region 音乐资源
        /// <summary>
        /// 音乐资源数据文件存放子目录名
        /// </summary>
        public const string AudioResFileDirectory = "Audio";

        /// <summary>
        /// 开始游戏、难度选择、关卡选择等界面背景声音文件
        /// </summary>
        public const string Music_PageBg = "bg.wav";

        /// <summary>
        /// 游戏界面背景声音文件
        /// </summary>
        public static readonly List<string> Music_GameBg = new List<string> { "game_bg01.wav", "game_bg02.wav", "game_bg03.wav", "game_bg04.wav" };

        /// <summary>
        /// 游戏界面 开始游戏音效 声音文件
        /// </summary>
        public const string Sound_Begin = "begin.wav";

        /// <summary>
        /// 游戏界面 游戏失败音效 声音文件
        /// </summary>
        public const string Sound_EndFail = "end.wav";

        /// <summary>
        /// 游戏界面 游戏结束得分一星音效 声音文件
        /// </summary>
        public const string Sound_OneStar = "win_01.wav";

        /// <summary>
        /// 游戏界面 游戏结束得分两星音效 声音文件
        /// </summary>
        public const string Sound_TwoStar = "win_02.wav";

        /// <summary>
        /// 游戏界面 游戏结束得分三星音效 声音文件
        /// </summary>
        public const string Sound_ThreeStar = "win_03.wav";

        /// <summary>
        /// 游戏界面 两个相同的方块连接成功音效 声音文件
        /// </summary>
        public const string Sound_SelSuc = "suc.wav";
        #endregion

        #region 数据资源
        /// <summary>
        /// 初始数据文件存放子目录名
        /// </summary>
        public const string DataResFileDirectory = "Data";

        /// <summary>
        /// 初始关卡数据文件名
        /// </summary>
        public const string Data_Level = "Level.xml";

        /// <summary>
        /// 游戏地图数据文件名
        /// </summary>
        public const string Data_Map = "Map.dat";
        #endregion

        /// <summary>
        /// 获取游戏相关资源数据文件URI
        /// </summary>
        /// <param name="resFileName">资源数据文件名</param>
        /// <returns>资源数据文件URI</returns>
        public static Uri ToResUri(this string resFileName)
        {
            return string.Format("{0}/{1}", ResFileDirectory, resFileName).ToUri();
        }

        /// <summary>
        /// 获取游戏相关声音资源数据文件URI
        /// </summary>
        /// <param name="resFileName">声音资源数据文件名</param>
        /// <returns>资源数据文件URI</returns>
        public static Uri ToAudioUri(this string resFileName)
        {
            return string.Format("{0}/{1}", AudioResFileDirectory, resFileName).ToResUri();
        }

        /// <summary>
        /// 获取游戏相关数据文件URI
        /// </summary>
        /// <param name="resFileName">数据文件名</param>
        /// <returns>资源数据文件URI</returns>
        public static Uri ToDataUri(this string resFileName)
        {
            return string.Format("{0}/{1}", DataResFileDirectory, resFileName).ToResUri();
        }
    }
}