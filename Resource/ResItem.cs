using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 游戏界面方块图标资源数据辅助类
    /// </summary>
    public static class ResItem
    {
        #region 从资源文件中读取数据
        /// <summary>
        /// 资源数据字典
        /// </summary>
        private static Dictionary<string, byte[]> itemResDict = new Dictionary<string, byte[]>();

        /// <summary>
        /// 读取资源
        /// </summary>
        public static void ReadData()
        {
            itemResDict = ResName.ItemData.LoadToDictBytes();
        }
        #endregion

        #region 将资源数据加载为可用的图片数据对象
        /// <summary>
        /// 图标数据集合
        /// </summary>
        public static List<ResInfo> Items = new List<ResInfo>();
        
        /// <summary>
        /// 加载资源 
        /// </summary>
        public static void Load()
        {
            int idx = 1;
            foreach (string key in itemResDict.Keys)
            {
                Items.Add(new ResInfo
                {
                    Code = idx,
                    Name = key,
                    Data = itemResDict[key].ToBitmapImage()
                });
                idx++;
            }
        }
        #endregion
    }
}
