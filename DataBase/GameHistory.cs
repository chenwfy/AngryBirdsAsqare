using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace SLGame.AngryBirdsAsqare.Database
{
    /// <summary>
    /// 游戏历史记录
    /// </summary>
    [Table]
    public class GameHistory
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        [Column(DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID { get; set; }

        /// <summary>
        /// 关卡所属难度级别1-4个级别
        /// </summary>
        [Column(DbType = "INT NOT NULL", CanBeNull = false)]
        public int LevelID { get; set; }

        /// <summary>
        /// 关卡序号
        /// </summary>
        [Column(DbType = "INT NOT NULL", CanBeNull = false)]
        public int GateID { get; set; }
    }
}