using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Resources;
using SLGame.AngryBirdsAsqare.Database;
using SLGame.AngryBirdsAsqare.Resource;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 游戏相关数据操作
    /// </summary>
    public class GameData
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static GameData()
        {
        }

        /// <summary>
        /// 进入游戏时加载游戏数据
        /// </summary>
        public static void LoadGameData()
        {
            //声音设置
            if (!HaveGameSet())
                InsertGameAudioSet(true);
            App.GameAudioEnable = GetGameAudioSet();
            //初始关卡数据
            if (!HaveGateData())
                InitGateData();
        }

        #region 游戏声音设置数据
        /// <summary>
        /// 获取是否存在游戏设置项
        /// </summary>
        /// <returns>是否存在游戏设置项</returns>
        public static bool HaveGameSet()
        {
            return DataProvider.HaveGameSet();
        }

        /// <summary>
        /// 获取游戏声音是否开启
        /// </summary>
        /// <returns>游戏声音是否开启</returns>
        public static bool GetGameAudioSet()
        {
            GameSet config = DataProvider.GetGameSet();
            return config.AudioEnabled;
        }

        /// <summary>
        /// 新增游戏声音设置项
        /// </summary>
        /// <param name="audioEnable">声音是否开启</param>
        public static void InsertGameAudioSet(bool audioEnable)
        {
            GameSet config = new GameSet { AudioEnabled = audioEnable };
            DataProvider.InsertGameSet(config);
        }

        /// <summary>
        /// 更新游戏声音设置
        /// </summary>
        /// <param name="audioEnable">声音是否开启</param>
        public static void UpdateGameAudioSet(bool audioEnable)
        {
            GameSet config = DataProvider.GetGameSet();
            config.AudioEnabled = audioEnable;
            DataProvider.UpdateGameSet(config);
        }
        #endregion

        #region 游戏关卡数据
        /// <summary>
        /// 获取是否存在关卡设置数据
        /// </summary>
        /// <returns>是否存在关卡设置数据</returns>
        public static bool HaveGateData()
        {
            return DataProvider.HaveGateData();
        }

        /// <summary>
        /// 新增关卡数据
        /// </summary>
        /// <param name="gate">关卡数据</param>
        public static void InsertGateData(LevelGates gate)
        {
            DataProvider.InsertGateData(gate);
        }

        /// <summary>
        /// 获取所有游戏关卡数据列表
        /// </summary>
        /// <returns>所有游戏关卡数据列表</returns>
        public static List<LevelGates> GetGateList()
        {
            return DataProvider.GetGateDataList();
        }

        /// <summary>
        /// 获取指定难度的关卡数据列表
        /// </summary>
        /// <param name="levelId">难度序号</param>
        /// <returns>指定难度的关卡数据列表</returns>
        public static List<LevelGates> GetLevelGateList(int levelId)
        {
            return GetGateList().Where(g => g.LevelID == levelId).ToList();
        }

        /// <summary>
        /// 获取指定难度和关卡序号的关卡数据
        /// </summary>
        /// <param name="levelId">难度序号</param>
        /// <param name="gateId">关卡序号</param>
        /// <returns>关卡数据</returns>
        public static LevelGates GetSingleGate(int levelId, int gateId)
        {
            return GetLevelGateList(levelId).Single(g => g.GateID == gateId);
        }

        /// <summary>
        /// 更新指定的关卡得分
        /// </summary>
        /// <param name="levelId">难度序号</param>
        /// <param name="gateId">关卡序号</param>
        /// <param name="score">得分</param>
        /// <param name="star">星级</param>
        public static void UpdateGateScore(int levelId, int gateId, int score, int star)
        {
            DataProvider.UpdateGateData(levelId, gateId, score, star);
        }

        /// <summary>
        /// 更新指定的关卡得分
        /// </summary>
        /// <param name="levelId">难度序号</param>
        /// <param name="gateId">关卡序号</param>
        /// <param name="score">得分</param>
        public static void UpdateGateScore(int levelId, int gateId, int score)
        {
            int heighestScore = GetSingleGate(levelId, gateId).HighestScore;
            int star = GetScoreStar(score, heighestScore);
            UpdateGateScore(levelId, gateId, score, star);
            //检查隐藏关卡是否开启
            LevelGates hidden = GetSingleGate(4, 1);
            if (hidden.Score < 0)
            {
                List<LevelGates> threeLevelGates = GetGateList().Where(g => g.LevelID <= 3).ToList();
                if (threeLevelGates.Sum(g => g.Star) >= threeLevelGates.Count * 3)
                    UpdateGateScore(4, 1, 0, 0);
            }
            //过关，开启下一关
            if (star >= 1)
            {
                int[] nextIds = GetNextLevelAndGateId(levelId, gateId);
                int nextLevelId = nextIds[0];
                int nextGateId = nextIds[1];
                if (nextLevelId > 0 && nextGateId > 0)
                {
                    LevelGates next = GetSingleGate(nextLevelId, nextGateId);
                    if (next.Score < 0)
                    {
                        UpdateGateScore(nextLevelId, nextGateId, 0, 0);
                    }
                }
            }
        }

        /// <summary>
        /// 获取下一关的难度级别以及关卡序号
        /// </summary>
        /// <param name="levelId">当前关卡的难度级别序号</param>
        /// <param name="gateId">当前关卡序号</param>
        /// <returns>数组：第一位为下一关卡的难度级别序号，第二位为下一关卡的关卡序号。任意元素为0则表示没有下一关卡</returns>
        public static int[] GetNextLevelAndGateId(int levelId, int gateId)
        {
            int nextLevelId = 0, nextGateId = 0;
            if (levelId < 3)
            {
                nextLevelId = levelId;
                nextGateId = gateId + 1;
                List<LevelGates> curLevelGates = GetLevelGateList(levelId);
                if (gateId >= curLevelGates.Max(g => g.GateID))
                {
                    nextLevelId++;
                    nextGateId = 1;
                }
                return new int[] { nextLevelId, nextGateId };
            }
            if (levelId == 3)
            {
                List<LevelGates> curLevelGates = GetLevelGateList(levelId);
                if (gateId < curLevelGates.Max(g => g.GateID))
                {
                    nextLevelId = 3;
                    nextGateId = gateId + 1;
                    return new int[] { nextLevelId, nextGateId };
                }
                LevelGates hidden = GetSingleGate(4, 1);
                if (hidden.Score < 0)
                {
                    List<LevelGates> threeLevelGates = GetGateList().Where(g => g.LevelID <= 3).ToList();
                    if (threeLevelGates.Sum(g => g.Star) < threeLevelGates.Count * 3)
                        return new int[] { 0, 0 };
                }
                return new int[] { 4, 1 };
            }
            if (levelId == 4)
            {
                List<LevelGates> levelFourGates = GetLevelGateList(levelId);
                if (gateId < levelFourGates.Max(g => g.GateID))
                {
                    nextLevelId = 4;
                    nextGateId = gateId + 1;
                    return new int[] { nextLevelId, nextGateId };
                }
            }
            return new int[] { 0, 0 };
        }

        /// <summary>
        /// 根据当前得分和关卡理论最好分数计算得分星级
        /// </summary>
        /// <param name="curScore">当前得分</param>
        /// <param name="heighestScore">关卡理论最好分数</param>
        /// <returns>得分星级</returns>
        public static int GetScoreStar(int curScore, int heighestScore)
        {
            double percent = (double)((double)curScore / (double)heighestScore);
            int star = 0;
            if (percent >= 0.6)
                star++;
            if (percent >= 0.7)
                star++;
            if (percent >= 0.85)
                star++;

            return star;
        }
        #endregion

        #region 游戏历史记录
        /// <summary>
        /// 获取最近一次游戏记录
        /// </summary>
        /// <returns>最近一次游戏记录</returns>
        public static GameHistory GetLastGameHistory()
        {
            GameHistory history = DataProvider.GetLastGameHistory();
            if (history != null && history != default(GameHistory))
                return history;
            return new GameHistory { LevelID = 1, GateID = 1 };
        }

        /// <summary>
        /// 获取指定难度级别最近一次游戏记录
        /// </summary>
        /// <param name="levelId">游戏难度级别序号1-4</param>
        /// <returns>指定难度级别最近一次游戏记录</returns>
        public static GameHistory GetLastGameHistoryByLevel(int levelId)
        {
            GameHistory history = DataProvider.GetLastGameHistoryByLevel(levelId);
            if (history != null && history != default(GameHistory))
                return history;
            return new GameHistory { LevelID = levelId, GateID = 1 };
        }

        /// <summary>
        /// 新增游戏记录
        /// </summary>
        /// <param name="gameHistory">游戏记录数据</param>
        public static void InsertGameHistory(GameHistory gameHistory)
        {
            DataProvider.InsertGameHistory(gameHistory);
        }
        #endregion

        #region 加载初始游戏关卡数据
        /// <summary>
        /// 加载初始游戏关卡数据
        /// </summary>
        private static void InitGateData()
        {
            List<LevelGates> initGate = LoadLevelGateDataFromXml();
            foreach (LevelGates gate in initGate)
            {
                int heighestScore = gate.MapSize * gate.MapCount;
                heighestScore += (gate.MapSize / 2) * gate.MapCount - 1;
                heighestScore += Constant.GameTime[gate.MapCount - 1];
                gate.HighestScore = heighestScore;
                InsertGateData(gate);
            }
        }

        /// <summary>
        /// 从数据资源文件加载初始关卡信息
        /// </summary>
        /// <returns>初始关卡信息</returns>
        private static List<LevelGates> LoadLevelGateDataFromXml()
        {
            StreamResourceInfo resInfo = Application.GetResourceStream(ResName.Data_Level.ToDataUri());
            using (Stream stream = resInfo.Stream)
            {
                XElement xel = XElement.Load(stream);
                int id = 1;
                List<LevelGates> initGate = new List<LevelGates>();
                foreach (XElement gate in xel.Descendants("Gate"))
                {
                    LevelGates curGate = new LevelGates
                    {
                        ID = id,
                        LevelID = Convert.ToInt32(gate.Attribute("Level").Value),
                        GateID = Convert.ToInt32(gate.Attribute("GateIdx").Value),
                        Score = Convert.ToInt32(gate.Attribute("Score").Value),
                        Star = Convert.ToInt32(gate.Attribute("StarScore").Value),
                        MapSize = Convert.ToInt32(gate.Attribute("MapSize").Value),
                        MapCount = Convert.ToInt32(gate.Attribute("MapCount").Value),
                        ItemCount = Convert.ToInt32(gate.Attribute("ItemCount").Value),
                        HighestScore = 0
                    };
                    initGate.Add(curGate);
                    id++;
                }

                return initGate;
            }
        }
        #endregion

        #region 获取游戏地图布局所需数据
        /// <summary>
        /// 获取游戏地图布局所需数据
        /// </summary>
        /// <param name="mapCount">关卡地图数量</param>
        /// <param name="mapSize">关卡地图可用格子总数</param>
        /// <param name="itemCount">关卡地图可用方块图标总数</param>
        /// <returns>游戏地图布局所需数据</returns>
        public static List<Dictionary<int, ResInfo>> GetGameMapItemsList(int mapCount, int mapSize, int itemCount)
        {
            List<Map> mapList = MapLoad.MapList.Where(m => m.GridCount == mapSize).ToList();
            List<Dictionary<int, ResInfo>> dataList = new List<Dictionary<int, ResInfo>>();
            Random rand = new Random();
            for (int i = 1; i <= mapCount; i++)
            {
                int tmpMapIdx = rand.Next(0, mapList.Count);
                Map tmpMap = mapList[tmpMapIdx];
                List<ResInfo> itemList = GetRandomItemsList(itemCount, mapSize);
                Dictionary<int, ResInfo> tmpDict = new Dictionary<int, ResInfo>();
                for (int n = 0; n < tmpMap.GridCount; n++)
                {
                    tmpDict.Add(tmpMap.Grids[n], itemList[n]);
                }
                dataList.Add(tmpDict);
            }
            return dataList;
        }

        /// <summary>
        /// 随机获取可用方块图标数据
        /// </summary>
        /// <param name="itemCount">不重复的方块总数</param>
        /// <param name="mapSize">地图填充格子总数</param>
        /// <returns>可用方块图标数据</returns>
        private static List<ResInfo> GetRandomItemsList(int itemCount, int mapSize)
        {
            List<ResInfo> items = ResItem.Items;
            List<ResInfo> selItems = new List<ResInfo>();
            Random rand = new Random();
            while (selItems.Count < itemCount)
            {
                int idx = rand.Next(0, items.Count);
                ResInfo tmpRes = items[idx];
                if (selItems.IndexOf(tmpRes) < 0)
                    selItems.Add(tmpRes);
            }

            List<ResInfo> itemList = new List<ResInfo>();
            int halfMapSize = mapSize / 2;
            int size = halfMapSize / itemCount;
            for (int i = 1; i <= size; i++)
            {
                itemList.AddRange(selItems);
            }
            size = halfMapSize % itemCount;
            if (size > 0)
            {
                for (int i = 1; i <= size; i++)
                {
                    int idx = rand.Next(0, selItems.Count);
                    ResInfo tmpRes = selItems[idx];
                    itemList.Add(tmpRes);
                }
            }

            List<ResInfo> resultList = itemList;
            resultList.AddRange(itemList);

            return resultList;
        }
        #endregion

        #region 其他和数据库无关的方法
        /// <summary>
        /// 获取指定范围和数量的随机数集合
        /// </summary>
        /// <param name="count">随机数数量</param>
        /// <param name="max">随机数最大范围</param>
        /// <returns>随机数集合</returns>
        public static List<int> GetRandGrids(int count, int max)
        {
            List<int> randList = new List<int>();
            Random rand = new Random();
            while (randList.Count < count)
            {
                int idx = rand.Next(1, max + 1);
                if (randList.IndexOf(idx) < 0)
                    randList.Add(idx);
            }

            return randList;
        }
        #endregion
    }
}