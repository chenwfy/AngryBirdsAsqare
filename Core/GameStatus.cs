using System;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// 未知状态，默认
        /// </summary>
        None = 0,

        /// <summary>
        /// 游戏中
        /// </summary>
        Playing,

        /// <summary>
        /// 暂停
        /// </summary>
        Pause,

        /// <summary>
        /// 结束
        /// </summary>
        End
    }
}
