using System;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 路径节点信息
    /// </summary>
    public struct PathNode
    {
        /// <summary>
        /// 网格序号，1-128
        /// </summary>
        public int GridIndex { get; set; }
        
        /// <summary>
        /// 该网格填充连接图片方向：
        /// 0：横向
        /// 1：竖向
        /// 2：左上
        /// 3：右上
        /// 4：左下
        /// 5：右下
        /// </summary>
        public byte GridDirection { get; set; }
    }
}