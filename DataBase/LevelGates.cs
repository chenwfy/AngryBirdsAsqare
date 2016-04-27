using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace SLGame.AngryBirdsAsqare.Database
{
    /// <summary>
    /// 游戏关卡信息类
    /// </summary>
    [Table]
    public class LevelGates
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

        /// <summary>
        /// 关卡历史最高得分
        /// </summary>
        [Column(DbType = "INT NOT NULL", CanBeNull = false)]
        public int Score { get; set; }

        /// <summary>
        /// 关卡最高得分星级
        /// </summary>
        [Column(DbType = "INT NOT NULL", CanBeNull = false)]
        public int Star { get; set; }

        /// <summary>
        /// 关卡满分，用于计算星级61%-70%：一星，71%-80%：二星，81%以上，三星
        /// </summary>
        [Column(DbType = "INT NOT NULL", CanBeNull = false)]
        public int HighestScore { get; set; }

        /// <summary>
        /// 关卡地图可用格子数量
        /// </summary>
        [Column(DbType = "INT NOT NULL", CanBeNull = false)]
        public int MapSize { get; set; }

        /// <summary>
        /// 关卡地图数量：第一难度为1，第二难度为2，第三难度为3，第四难度中，1-5为1,6-10为2,11-15为3
        /// </summary>
        [Column(DbType = "INT NOT NULL", CanBeNull = false)]
        public int MapCount { get; set; }

        /// <summary>
        /// 关卡地图布局方块数量
        /// </summary>
        [Column(DbType = "INT NOT NULL", CanBeNull = false)]
        public int ItemCount { get; set; }
    }
}