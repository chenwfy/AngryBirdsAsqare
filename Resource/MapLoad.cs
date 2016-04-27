using System;
using System.IO;
using System.Windows;
using System.Windows.Resources;
using System.Collections.Generic;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 游戏地图信息类
    /// </summary>
    public class Map
    {
        /// <summary>
        /// 地图可用网格总数
        /// </summary>
        public int GridCount { get; set; }

        /// <summary>
        /// 地图可用的网格序号集合
        /// </summary>
        public List<int> Grids { get; set; }
    }


    /// <summary>
    /// 游戏地图数据加载类
    /// </summary>
    public static class MapLoad
    {
        /// <summary>
        /// 地图数据集合
        /// </summary>
        public static List<Map> MapList { get; private set; }

        /// <summary>
        /// 加载地图数据
        /// </summary>
        public static void Load()
        {
            MapList = ResName.Data_Map.ToMapList();
        }

        /// <summary>
        /// 加载游戏地图数据
        /// </summary>
        /// <param name="resDataFileName">地图数据文件名</param>
        /// <returns>地图数据</returns>
        public static List<Map> ToMapList(this string resDataFileName)
        {
            List<Map> mapList = new List<Map>();
            StreamResourceInfo resInfo = Application.GetResourceStream(resDataFileName.ToDataUri());
            using (Stream stream = resInfo.Stream)
            {
                BinaryReader reader = new BinaryReader(stream);
                int resFlag = reader.ReadInt32();
                short resCount = reader.ReadInt16();
                for (int r = 1; r <= resCount; r++)
                {
                    int gridCount = (int)reader.ReadByte();
                    List<int> gridList = new List<int>();
                    for (int i = 1; i <= gridCount; i++)
                    {
                        gridList.Add((int)reader.ReadByte());
                    }

                    mapList.Add(new Map { GridCount = gridCount, Grids = gridList });
                }
            }
            return mapList;
        }
    }
}
