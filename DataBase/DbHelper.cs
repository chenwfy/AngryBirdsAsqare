using System;
using System.Data.Linq;

namespace SLGame.AngryBirdsAsqare.Database
{
    /// <summary>
    /// 游戏本地数据库连接辅助类
    /// </summary>
    public class DbHelper : DataContext
    {
        /// <summary>
        /// 数据库连接设置字串
        /// </summary>
        private static readonly string DBConnectionString = "Data Source=isostore:/SLGame.AngryBirdsAsqare.sdf";

        /// <summary>
        /// 游戏设置表
        /// </summary>
        public Table<GameSet> GameSet;

        /// <summary>
        /// 游戏关卡表
        /// </summary>
        public Table<LevelGates> LevelGates;

        /// <summary>
        /// 游戏历史记录
        /// </summary>
        public Table<GameHistory> GameHistory;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbHelper()
            : base(DBConnectionString)
        {
            if (!DatabaseExists())
                CreateDatabase();
        }
    }
}