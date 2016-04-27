using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace SLGame.AngryBirdsAsqare.Database
{
    /// <summary>
    /// 游戏相关数据库操作类
    /// </summary>
    public class DataProvider
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DataProvider()
        {
        }

        #region 游戏设置
        /// <summary>
        /// 获取是否存在游戏设置项
        /// </summary>
        /// <returns>是否存在游戏设置项</returns>
        public static bool HaveGameSet()
        {
            using (DbHelper db = new DbHelper())
            {
                return db.GameSet.Count() > 0;
            }
        }

        /// <summary>
        /// 获取游戏设置项内容
        /// </summary>
        /// <returns>游戏设置项内容</returns>
        public static GameSet GetGameSet()
        {
            using (DbHelper db = new DbHelper())
            {
                return db.GameSet.FirstOrDefault();
            }
        }

        /// <summary>
        /// 新增游戏设置项
        /// </summary>
        /// <param name="gameSet">游戏设置项</param>
        public static void InsertGameSet(GameSet gameSet)
        {
            using (DbHelper db = new DbHelper())
            {
                db.GameSet.InsertOnSubmit(gameSet);
                db.SubmitChanges();
            }
        }

        /// <summary>
        /// 修改游戏设置项
        /// </summary>
        /// <param name="gameSet">游戏设置项</param>
        public static void UpdateGameSet(GameSet gameSet)
        {
            using (DbHelper db = new DbHelper())
            {
                GameSet set = db.GameSet.Single(s => s.Id == gameSet.Id);
                set.AudioEnabled = gameSet.AudioEnabled;
                db.SubmitChanges();
            }
        }
        #endregion

        #region 游戏关卡
        /// <summary>
        /// 获取是否已经存在关卡数据
        /// </summary>
        /// <returns>是否已经存在关卡数据</returns>
        public static bool HaveGateData()
        {
            using (DbHelper db = new DbHelper())
            {
                return db.LevelGates.Count() > 0;
            }
        }

        /// <summary>
        /// 新增关卡记录
        /// </summary>
        /// <param name="gate">关卡数据</param>
        public static void InsertGateData(LevelGates gate)
        {
            using (DbHelper db = new DbHelper())
            {
                db.LevelGates.InsertOnSubmit(gate);
                db.SubmitChanges();
            }
        }

        /// <summary>
        /// 获取所有关卡数据列表
        /// </summary>
        /// <returns>所有关卡数据列表</returns>
        public static List<LevelGates> GetGateDataList()
        {
            using (DbHelper db = new DbHelper())
            {
                return db.LevelGates.ToList();
            }
        }

        /// <summary>
        /// 获取单一关卡数据
        /// </summary>
        /// <param name="levelId">级别序号</param>
        /// <param name="gateId">关卡序号</param>
        /// <returns>关卡数据</returns>
        public static LevelGates GetSingleGateData(int levelId, int gateId)
        {
            using (DbHelper db = new DbHelper())
            {
                LevelGates gate = db.LevelGates.Single(g => g.LevelID == levelId && g.GateID == gateId);
                return gate;
            }
        }

        /// <summary>
        /// 更新关卡成绩数据
        /// </summary>
        /// <param name="levelId">级别序号</param>
        /// <param name="gateId">关卡序号</param>
        /// <param name="score">关卡得分</param>
        /// <param name="star">关卡得分星级</param>
        public static void UpdateGateData(int levelId, int gateId, int score, int star)
        {
            using (DbHelper db = new DbHelper())
            {
                LevelGates gate = db.LevelGates.Single(g => g.LevelID == levelId && g.GateID == gateId);
                gate.Score = score;
                gate.Star = star;
                db.SubmitChanges();
            }
        }
        #endregion

        #region 游戏历史
        /// <summary>
        /// 获取最近一次游戏记录
        /// </summary>
        /// <returns>最近一次游戏记录</returns>
        public static GameHistory GetLastGameHistory()
        {
            using (DbHelper db = new DbHelper())
            {
                return db.GameHistory.OrderByDescending(h => h.ID).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取指定难度级别最近一次游戏记录
        /// </summary>
        /// <param name="levelId">游戏难度级别序号1-4</param>
        /// <returns>指定难度级别最近一次游戏记录</returns>
        public static GameHistory GetLastGameHistoryByLevel(int levelId)
        {
            using (DbHelper db = new DbHelper())
            {
                return db.GameHistory.Where(h => h.LevelID == levelId).OrderByDescending(h => h.ID).FirstOrDefault();
            }
        }

        /// <summary>
        /// 新增游戏记录
        /// </summary>
        /// <param name="gameHistory">游戏记录数据</param>
        public static void InsertGameHistory(GameHistory gameHistory)
        {
            using (DbHelper db = new DbHelper())
            {
                db.GameHistory.InsertOnSubmit(gameHistory);
                db.SubmitChanges();
            }
        }
        #endregion
    }
}