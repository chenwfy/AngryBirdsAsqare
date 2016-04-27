using System;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 游戏页面枚举
    /// </summary>
    public enum GamePage
    {
        /// <summary>
        /// 默认，无
        /// </summary>
        None = 0,

        /// <summary>
        /// 游戏开始界面
        /// </summary>
        PageStart,

        /// <summary>
        /// 游戏难度选择界面
        /// </summary>
        PageLevel,

        /// <summary>
        /// 游戏难度关卡选择界面
        /// </summary>
        PageGate,

        /// <summary>
        /// 游戏中界面
        /// </summary>
        PageGame
    }
}