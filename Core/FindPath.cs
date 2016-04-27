using System;
using System.Linq;
using System.Collections.Generic;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 游戏寻路辅助类。本类的所有算法全部基于地图规格为（16*8），如果地图规格改变，本类方法需要做对应的修改
    /// </summary>
    public class FindPath
    {
        /// <summary>
        /// 当前地图中剩余可用的网格序号集合（即一般寻路算法中的障碍物）
        /// </summary>
        private List<int> mapGridList = new List<int>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mapGrids">结束网格序号</param>
        public FindPath(List<int> mapGrids)
        {
            this.mapGridList = mapGrids;
        }

        /// <summary>
        /// 寻路并返回路径
        /// </summary>
        /// <param name="startGrid">起始网格序号</param>
        /// <param name="endGrid">结束网格序号</param>
        /// <returns>路径网格序号集合，顺序从起点（不包含）至终点（不包含）</returns>
        public List<PathNode> PathFinder(int startGrid, int endGrid)
        {
            //起点、终点相邻
            bool isNear = IsNearGrid(startGrid, endGrid);
            if (isNear)
                return new List<PathNode> { };

            //起点网格的四向（左、上、右、下）可用网格集合
            List<List<int>> startFourDirGridList = GetFourDirGridList(startGrid);
            //终点在起点四向可用网格的顶头或者相邻位置
            List<PathNode> endNearStartFourDirGrids = EndNearStartFourDirGrids(startGrid, endGrid, startFourDirGridList);
            if (null != endNearStartFourDirGrids)
                return endNearStartFourDirGrids;

            //终点可直线连接到起点四向可用网格中的一格时的寻路
            List<PathNode> endStraightToSFourDirGrids = EndStraightToSFourDirGrids(startGrid, endGrid, startFourDirGridList);
            if (null != endStraightToSFourDirGrids)
                return endStraightToSFourDirGrids;

            //循环起点四向可用网格中的每一个网格的四向可用网格到终点寻路
            List<PathNode> loopStartFourDirToEnd = LoopStartFourDirToEnd(startGrid, endGrid, startFourDirGridList);
            if (null != loopStartFourDirToEnd)
                return loopStartFourDirToEnd;

            //找不到路径
            return null;
        }

        /// <summary>
        /// 获取指定网格在地图中的行数
        /// </summary>
        /// <param name="gridIdx">网格序号</param>
        /// <returns>网格在地图中的行数</returns>
        public static int GetGridRow(int gridIdx)
        {
            int row = gridIdx / 16;
            if (gridIdx % 16 > 0)
                row++;
            return row;
        }

        /// <summary>
        /// 获取指定网格在地图中的列数
        /// </summary>
        /// <param name="gridIdx">网格序号</param>
        /// <returns>网格在地图中的列数</returns>
        public static int GetGridColumn(int gridIdx)
        {
            int col = gridIdx % 16;
            if (col == 0)
                col = 16;
            return col;
        }

        #region 私有方法
        /// <summary>
        /// 判断是否是相邻的两个网格
        /// </summary>
        /// <param name="sGrid">起始网格</param>
        /// <param name="eGrid">结束网格</param>
        /// <returns>是否相邻</returns>
        private bool IsNearGrid(int sGrid, int eGrid)
        {
            int diff = eGrid - sGrid;
            int absDiff = Math.Abs(diff);
            //纵向相邻
            if (16 == absDiff)
                return true;

            int startRow = GetGridRow(sGrid);
            int endRow = GetGridRow(eGrid);
            //横向相邻
            if (endRow == startRow && 1 == absDiff)
                return true;

            return false;
        }

        /// <summary>
        /// 终点在起点四向可用网格的顶头或者相邻位置时获取连接路径
        /// </summary>
        /// <param name="sGrid">起始网格序号</param>
        /// <param name="eGrid">结束网格序号</param>
        /// <param name="sDirList">起点网格的四向（左、上、右、下）可用网格集合</param>
        /// <returns>路径集合，顺序从起点（不包含）至终点（不包含）</returns>
        private List<PathNode> EndNearStartFourDirGrids(int sGrid, int eGrid, List<List<int>> sDirList)
        {
            List<List<int>> findPaths = new List<List<int>>();
            foreach (List<int> dirNodes in sDirList)
            {
                List<int> dirPath = new List<int>();
                foreach (int node in dirNodes)
                {
                    dirPath.Add(node);
                    if (IsNearGrid(node, eGrid))
                    {
                        findPaths.Add(dirPath);
                        break;
                    }
                }
            }
            if (findPaths.Count > 0)
            {
                List<int> shortPath = GetShortestPath(findPaths);
                return ConverToPathNode(sGrid, eGrid, shortPath);
            }
            return null;
        }

        /// <summary>
        /// 终点可直线连接到起点四向可用网格中的一格时的寻路
        /// </summary>
        /// <param name="sGrid">起始网格序号</param>
        /// <param name="eGrid">结束网格序号</param>
        /// <param name="sDirList">起点网格的四向（左、上、右、下）可用网格集合</param>
        /// <returns>路径集合，顺序从起点（不包含）至终点（不包含）</returns>
        private List<PathNode> EndStraightToSFourDirGrids(int sGrid, int eGrid, List<List<int>> sDirList)
        {
            List<List<int>> findPaths = new List<List<int>>();
            foreach (List<int> dirNodes in sDirList)
            {
                List<int> dirPath = new List<int>();
                foreach (int node in dirNodes)
                {
                    dirPath.Add(node);
                    List<int> tmpList = StartStraightToEnd(node, eGrid);
                    if (tmpList != null)
                    {
                        dirPath.AddRange(tmpList);
                        findPaths.Add(dirPath);
                        break;
                    }
                }
            }
            if (findPaths.Count > 0)
            {
                List<int> shortPath = GetShortestPath(findPaths);
                return ConverToPathNode(sGrid, eGrid, shortPath);
            }
            return null;
        }

        /// <summary>
        /// 循环起点四向可用网格中的每一个网格的四向可用网格到终点寻路
        /// </summary>
        /// <param name="sGrid">起始网格序号</param>
        /// <param name="eGrid">结束网格序号</param>
        /// <param name="sDirList">起点网格的四向（左、上、右、下）可用网格集合</param>
        /// <returns>路径集合，顺序从起点（不包含）至终点（不包含）</returns>
        private List<PathNode> LoopStartFourDirToEnd(int sGrid, int eGrid, List<List<int>> sDirList)
        {
            List<List<int>> findPaths = new List<List<int>>();
            
            #region 左
            List<int> sLeftDirList = sDirList[0];
            List<int> sLeftFind = new List<int>();
            foreach (int node in sLeftDirList)
            {
                sLeftFind.Add(node);

                bool isFind = false;

                List<List<int>> nodeForDirList = GetFourDirGridList(node);

                List<int> nodeTopList = nodeForDirList[1];
                List<int> nTopFind = new List<int>();
                foreach (int nTop in nodeTopList)
                {
                    nTopFind.Add(nTop);
                    List<int> tmpList = StartStraightToEnd(nTop, eGrid);
                    if (tmpList != null)
                    {
                        nTopFind.AddRange(tmpList);
                        sLeftFind.AddRange(nTopFind);
                        findPaths.Add(sLeftFind);
                        isFind = true;
                        break;
                    }
                }

                List<int> nodeDownList = nodeForDirList[3];
                List<int> nDownFind = new List<int>();
                foreach (int nDown in nodeDownList)
                {
                    nDownFind.Add(nDown);
                    List<int> tmpList = StartStraightToEnd(nDown, eGrid);
                    if (tmpList != null)
                    {
                        nDownFind.AddRange(tmpList);
                        sLeftFind.AddRange(nDownFind);
                        findPaths.Add(sLeftFind);
                        isFind = true;
                        break;
                    }
                }

                if (isFind)
                    break;
            }
            #endregion

            #region 右
            List<int> sRightDirList = sDirList[2];
            List<int> sRightFind = new List<int>();
            foreach (int node in sRightDirList)
            {
                sRightFind.Add(node);

                bool isFind = false;

                List<List<int>> nodeForDirList = GetFourDirGridList(node);

                List<int> nodeTopList = nodeForDirList[1];
                List<int> nTopFind = new List<int>();
                foreach (int nTop in nodeTopList)
                {
                    nTopFind.Add(nTop);
                    List<int> tmpList = StartStraightToEnd(nTop, eGrid);
                    if (tmpList != null)
                    {
                        nTopFind.AddRange(tmpList);
                        sRightFind.AddRange(nTopFind);
                        findPaths.Add(sRightFind);
                        isFind = true;
                        break;
                    }
                }

                List<int> nodeDownList = nodeForDirList[3];
                List<int> nDownFind = new List<int>();
                foreach (int nDown in nodeDownList)
                {
                    nDownFind.Add(nDown);
                    List<int> tmpList = StartStraightToEnd(nDown, eGrid);
                    if (tmpList != null)
                    {
                        nDownFind.AddRange(tmpList);
                        sRightFind.AddRange(nDownFind);
                        findPaths.Add(sRightFind);
                        isFind = true;
                        break;
                    }
                }

                if (isFind)
                    break;
            }
            #endregion

            #region 上
            List<int> sTopDirList = sDirList[1];
            List<int> sTopFind = new List<int>();
            foreach (int node in sTopDirList)
            {
                sTopFind.Add(node);

                bool isFind = false;

                List<List<int>> nodeForDirList = GetFourDirGridList(node);

                List<int> nodeLeftList = nodeForDirList[0];
                List<int> nLeftFind = new List<int>();
                foreach (int nLeft in nodeLeftList)
                {
                    nLeftFind.Add(nLeft);
                    List<int> tmpList = StartStraightToEnd(nLeft, eGrid);
                    if (tmpList != null)
                    {
                        nLeftFind.AddRange(tmpList);
                        sTopFind.AddRange(nLeftFind);
                        findPaths.Add(sTopFind);
                        isFind = true;
                        break;
                    }
                }

                List<int> nodeRightList = nodeForDirList[2];
                List<int> nRightFind = new List<int>();
                foreach (int nRight in nodeRightList)
                {
                    nRightFind.Add(nRight);
                    List<int> tmpList = StartStraightToEnd(nRight, eGrid);
                    if (tmpList != null)
                    {
                        nRightFind.AddRange(tmpList);
                        sTopFind.AddRange(nRightFind);
                        findPaths.Add(sTopFind);
                        isFind = true;
                        break;
                    }
                }

                if (isFind)
                    break;
            }
            #endregion

            #region 下
            List<int> sDownDirList = sDirList[3];
            List<int> sDownFind = new List<int>();
            foreach (int node in sDownDirList)
            {
                sDownFind.Add(node);

                bool isFind = false;

                List<List<int>> nodeForDirList = GetFourDirGridList(node);

                List<int> nodeLeftList = nodeForDirList[0];
                List<int> nLeftFind = new List<int>();
                foreach (int nLeft in nodeLeftList)
                {
                    nLeftFind.Add(nLeft);
                    List<int> tmpList = StartStraightToEnd(nLeft, eGrid);
                    if (tmpList != null)
                    {
                        nLeftFind.AddRange(tmpList);
                        sDownFind.AddRange(nLeftFind);
                        findPaths.Add(sDownFind);
                        isFind = true;
                        break;
                    }
                }

                List<int> nodeRightList = nodeForDirList[2];
                List<int> nRightFind = new List<int>();
                foreach (int nRight in nodeRightList)
                {
                    nRightFind.Add(nRight);
                    List<int> tmpList = StartStraightToEnd(nRight, eGrid);
                    if (tmpList != null)
                    {
                        nRightFind.AddRange(tmpList);
                        sDownFind.AddRange(nRightFind);
                        findPaths.Add(sDownFind);
                        isFind = true;
                        break;
                    }
                }

                if (isFind)
                    break;
            }
            #endregion

            if (findPaths.Count > 0)
            {
                List<int> shortPath = GetShortestPath(findPaths);
                return ConverToPathNode(sGrid, eGrid, shortPath);
            }
            return null;
        }

        /// <summary>
        /// 将网格序号路径转换为PathNode路径
        /// </summary>
        /// <param name="startGrid">起始网格序号</param>
        /// <param name="endGrid">结束网格序号</param>
        /// <param name="path">网格序号路径（从起点到终点，但是不包含起点和终点）</param>
        /// <returns>PathNode路径</returns>
        private List<PathNode> ConverToPathNode(int startGrid, int endGrid, List<int> path)
        {
            List<PathNode> pathNodeList = new List<PathNode>();
            for (int i = 0; i < path.Count; i++)
            {
                int preGrid = startGrid;
                int curGrid = path[i];
                int nextGrid = endGrid;
                if (path.Count > 1)
                {
                    if (0 == i)
                        nextGrid = path[1];
                    else if (i == path.Count - 1)
                        preGrid = path[i - 1];
                    else
                    {
                        preGrid = path[i - 1];
                        nextGrid = path[i + 1];
                    }
                }
                byte dir = GetGridDirection(preGrid, curGrid, nextGrid);
                pathNodeList.Add(new PathNode { GridIndex = curGrid, GridDirection = dir });
            }
            return pathNodeList;
        }

        /// <summary>
        /// 获取指定的三个网格中，中央网格的连接方向
        /// 0：横向
        /// 1：竖向
        /// 2：左上
        /// 3：右上
        /// 4：左下
        /// 5：右下
        /// </summary>
        /// <param name="prevGrid">前一个网格</param>
        /// <param name="currGrid">中央网格</param>
        /// <param name="nextGrid">后一个网格</param>
        /// <returns>中央网格的连接方向</returns>
        private byte GetGridDirection(int prevGrid, int currGrid, int nextGrid)
        {
            int prevRow = GetGridRow(prevGrid), currRow = GetGridRow(currGrid), nextRow = GetGridRow(nextGrid);
            if (prevRow == currRow && prevRow == nextRow)
                return 0;

            int prevCol = GetGridColumn(prevGrid), currCol = GetGridColumn(currGrid), nextCol = GetGridColumn(nextGrid);
            if (prevCol == currCol && prevCol == nextCol)
                return 1;

            if (prevRow == currRow && currCol == nextCol)
            {
                if (prevGrid < currGrid && currGrid > nextGrid)
                    return 2;
                if (prevGrid < currGrid && currGrid < nextGrid)
                    return 4;
                if (prevGrid > currGrid && currGrid > nextGrid)
                    return 3;
                if (prevGrid > currGrid && currGrid < nextGrid)
                    return 5;
            }

            if (prevCol == currCol && currRow == nextRow)
            {
                if (prevGrid < currGrid && currGrid > nextGrid)
                    return 2;
                if (prevGrid < currGrid && currGrid < nextGrid)
                    return 3;
                if (prevGrid > currGrid && currGrid > nextGrid)
                    return 4;
                if (prevGrid > currGrid && currGrid < nextGrid)
                    return 5;
            }

            return 0;
        }

        /// <summary>
        /// 获取两个网格之间直线可用的网格序号集合
        /// </summary>
        /// <param name="sGrid">起点网格序号</param>
        /// <param name="eGrid">终点网格序号</param>
        /// <returns>直线可用的网格序号集合</returns>
        private List<int> StartStraightToEnd(int sGrid, int eGrid)
        {
            List<int> gridList = new List<int>();
            int sRow = GetGridRow(sGrid), eRow = GetGridRow(eGrid);
            if (sRow == eRow)
            {
                if (sGrid < eGrid)
                {
                    for (int i = sGrid + 1; i < eGrid; i++)
                    {
                        if (this.mapGridList.IndexOf(i) >= 0)
                            return null;
                        gridList.Add(i);
                    }
                    return gridList;
                }
                if (sGrid > eGrid)
                {
                    for (int i = sGrid - 1; i > eGrid; i--)
                    {
                        if (this.mapGridList.IndexOf(i) >= 0)
                            return null;
                        gridList.Add(i);
                    }
                    return gridList; 
                }
            }
            int sCol = GetGridColumn(sGrid), eCol = GetGridColumn(eGrid);
            if (sCol == eCol)
            {
                if (sGrid < eGrid)
                {
                    for (int i = sGrid + 16; i < eGrid; i += 16)
                    {
                        if (this.mapGridList.IndexOf(i) >= 0)
                            return null;
                        gridList.Add(i);
                    }
                    return gridList;
                }
                if (sGrid > eGrid)
                {
                    for (int i = sGrid - 16; i > eGrid; i -= 16)
                    {
                        if (this.mapGridList.IndexOf(i) >= 0)
                            return null;
                        gridList.Add(i);
                    }
                    return gridList;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定网格的四向可用网格集合
        /// </summary>
        /// <param name="grid">指定的网格</param>
        /// <returns>四向可用网格集合</returns>
        private List<List<int>> GetFourDirGridList(int grid)
        {
            //左
            List<int> leftList = new List<int>();
            if ((grid - 1) % 16 > 0)
            {
                int leftDot = (GetGridRow(grid) - 1) * 16 + 1;
                for (int i = grid - 1; i >= leftDot; i--)
                {
                    if (this.mapGridList.IndexOf(i) >= 0)
                        break;
                    else
                        leftList.Add(i);
                }
            }
            //上
            List<int> topList = new List<int>();
            if (grid > 16)
            {
                for (int i = grid - 16; i > 0; i -= 16)
                {
                    if (this.mapGridList.IndexOf(i) >= 0)
                        break;
                    else
                        topList.Add(i);
                }
            }
            //右
            List<int> rightList = new List<int>();
            if (grid % 16 > 0)
            {
                int rightDot = GetGridRow(grid) * 16;
                for (int i = grid + 1; i <= rightDot; i++)
                {
                    if (this.mapGridList.IndexOf(i) >= 0)
                        break;
                    else
                        rightList.Add(i);
                }
            }
            //下
            List<int> downList = new List<int>();
            if (grid < 113)
            {
                for (int i = grid + 16; i <= 128; i += 16)
                {
                    if (this.mapGridList.IndexOf(i) >= 0)
                        break;
                    else
                        downList.Add(i);
                }
            }

            return new List<List<int>> { leftList, topList, rightList, downList };
        }
        #endregion

        #region 找出路径集合中最短的一条路径
        /// <summary>
        /// 找出路径集合中最短的一条路径
        /// </summary>
        /// <param name="paths">所有路径集合</param>
        /// <returns>最短的一条路径</returns>
        private List<int> GetShortestPath(List<List<int>> paths)
        {
            paths.Sort(new IntListComparer());
            return paths[0];
        }

        /// <summary>
        /// 
        /// </summary>
        private class IntListComparer : IComparer<List<int>>
        {
            public int Compare(List<int> x, List<int> y)
            {
                return x.Count - y.Count;
            }
        }
        #endregion
    }
}