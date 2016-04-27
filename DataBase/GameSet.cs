using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace SLGame.AngryBirdsAsqare.Database
{
    /// <summary>
    /// 游戏声音设置类
    /// </summary>
    [Table]
    public class GameSet
    {
        /// <summary>
        /// 设置项编号
        /// </summary>
        [Column(DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        /// <summary>
        /// 是否开启游戏声音
        /// </summary>
        [Column(DbType = "BIT NOT NULL", CanBeNull = false)]
        public bool AudioEnabled { get; set; }
    }
}