using System;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 游戏相关定义
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public const double ScreenWidth = 800;

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public const double ScreenHeight = 480;

        /// <summary>
        /// 游戏界面常规动画时间长度，单位：毫秒
        /// </summary>
        public const double AnimationTime = 200;

        /// <summary>
        /// 每局游戏时间长度，单位：秒
        /// </summary>
        public static readonly int[] GameTime = new int[] { 30, 45, 60 };

        /// <summary>
        /// 游戏背景音乐音量
        /// </summary>
        public const float PageSoundVolume = 0.7f;

        /// <summary>
        /// 游戏中音效声音音量
        /// </summary>
        public const float GameSoundVolume = 0.9f;
    }
}