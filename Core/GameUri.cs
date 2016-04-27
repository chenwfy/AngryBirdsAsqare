using System;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 游戏页面地址辅助类
    /// </summary>
    public static class GameUri
    {
        /// <summary>
        /// 获取游戏页面URI
        /// </summary>
        /// <param name="gamePage">游戏页面</param>
        /// <returns>游戏页面URI</returns>
        public static Uri ToPageUri(this GamePage gamePage)
        {
            return gamePage.ToPageUri(string.Empty);
        }

        /// <summary>
        /// 获取游戏页面URI
        /// </summary>
        /// <param name="gamePage">游戏页面></param>
        /// <param name="args">页面参数</param>
        /// <returns>游戏页面URI</returns>
        public static Uri ToPageUri(this GamePage gamePage, string args)
        {
            string pageName = gamePage == GamePage.None ? "PageStart" : gamePage.ToString();
            string pageUrl = string.Format("/{0}.xaml{1}", pageName, string.IsNullOrEmpty(args) ? string.Empty : "?" + args);
            return new Uri(pageUrl, UriKind.Relative);
        }
    }
}
