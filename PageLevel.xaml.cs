using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using SLGame.AngryBirdsAsqare.Resource;
using SLGame.AngryBirdsAsqare.Core;
using SLGame.AngryBirdsAsqare.Database;

namespace SLGame.AngryBirdsAsqare.View
{
    /// <summary>
    /// 游戏难度选择界面
    /// </summary>
    public partial class PageLevel : PhoneApplicationPage
    {
        /// <summary>
        /// 当前页面类示例
        /// </summary>
        public static PageLevel Instance { get; private set; }
        /// <summary>
        /// 页面显示难度级别数量，如果为3则表示隐藏难度不显示。
        /// </summary>
        private int showLevelCount = 3;
        /// <summary>
        /// 游戏关卡数据
        /// </summary>
        private List<LevelGates> levelGates = new List<LevelGates>();
        /// <summary>
        /// 显示所有难度级别图标的Canvas控件
        /// </summary>
        private Canvas canvasLevelBox;
        /// <summary>
        /// canvasLevelBox移动控件
        /// </summary>
        private TranslateTransform translation = new TranslateTransform();
        /// <summary>
        /// canvasLevelBox移动起始X坐标
        /// </summary>
        private double moveStartX = 0;
        /// <summary>
        /// 当前居中显示（即可点击进入下一级界面）的难度级别序号
        /// </summary>
        private int currentLevel = 1;
        /// <summary>
        /// 将要移动至居中显示（即可点击进入下一级界面）的难度级别序号
        /// </summary>
        private int moveToLevel = 1;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageLevel()
        {
            InitializeComponent();
            Instance = this;

            //界面背景
            this.LayoutRoot.Background = ResScreen.LevelBg.ToImageBrush();
            //注册界面多点触控事件
            this.LayoutRoot.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>((sender, e) =>
            {
                this.moveStartX = this.translation.X;
            });
            this.LayoutRoot.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>((sender, e) =>
            {
                this.translation.X += e.DeltaManipulation.Translation.X;
            });
            this.LayoutRoot.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(LevelPage_ManipulationCompleted);
            //加载数据
            LoadData();
        }

        /// <summary>
        /// 重绘界面，当界面需要重绘时，供其他页面调用该方法
        /// </summary>
        public void ReDrawPage()
        {
            List<UIElement> pageElementList = new List<UIElement>();
            foreach (UIElement element in this.LayoutRoot.Children)
                pageElementList.Add(element);
            
            foreach (UIElement element in pageElementList)
                this.LayoutRoot.Children.Remove(element);

            //加载数据
            LoadData();
        }

        /// <summary>
        /// 加载当前游戏关卡数据
        /// </summary>
        private void LoadData()
        {
            this.levelGates = GameData.GetGateList();
            List<LevelGates> topThreeLevelGates = this.levelGates.Where(g => g.LevelID <= 3).ToList();
            int topThreeLevelAllStar = topThreeLevelGates.Count() * 3;
            int topThreeLevelGetStar = topThreeLevelGates.Sum(g => g.Star);
            if (topThreeLevelGetStar >= topThreeLevelAllStar)
                this.showLevelCount = 4;

            DrawPointGrid();
            DrawLevelBox();
            LoadGameHistory();
        }

        /// <summary>
        /// 绘制界面下方的点和数字
        /// </summary>
        private void DrawPointGrid()
        {
            double pointGridHeight = 24;
            double pointGridWidth = 16 * this.showLevelCount;
            Image pointImg = new Image();
            Grid pointGrid = new Grid { Height = pointGridHeight, Width = pointGridWidth };
            pointGrid.SetValue(Canvas.LeftProperty, (double)((Constant.ScreenWidth - pointGridWidth) / 2));
            pointGrid.SetValue(Canvas.TopProperty, (double)(Constant.ScreenHeight - pointGridHeight - 4));
            for (int r = 1; r <= 2; r++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(12);
                pointGrid.RowDefinitions.Add(row);
            }

            for (int c = 1; c <= this.showLevelCount; c++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(12);
                pointGrid.ColumnDefinitions.Add(column);

                WriteableBitmap tmpWB = c.ToString().ToWriteableBitmap();
                pointImg = new Image
                {
                    Source = tmpWB,
                    Stretch = Stretch.None,
                    Name = string.Format("Img_SerialLevel_{0}", c),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Visibility = (c == this.currentLevel) ? Visibility.Visible : Visibility.Collapsed
                };
                Grid.SetRow(pointImg, 0);
                Grid.SetColumn(pointImg, c - 1);
                pointGrid.Children.Add(pointImg);
                tmpWB = null;

                pointImg = new Image
                {
                    Source = (c == this.currentLevel) ? ResIcon.DotLight : ResIcon.DotDark,
                    Stretch = Stretch.None,
                    Name = string.Format("Img_PointLevel_{0}", c),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Visibility = Visibility.Visible
                };
                Grid.SetRow(pointImg, 1);
                Grid.SetColumn(pointImg, c - 1);
                pointGrid.Children.Add(pointImg);
            }
            this.LayoutRoot.Children.Add(pointGrid);
        }

        /// <summary>
        /// 绘制难度界别ICON显示部分
        /// </summary>
        private void DrawLevelBox()
        {
            this.canvasLevelBox = new Canvas
            {
                Height = Constant.ScreenHeight,
                Width = (this.showLevelCount + 1) * 400,
                RenderTransform = this.translation
            };
            this.canvasLevelBox.SetValue(Canvas.TopProperty, 0d);
            this.canvasLevelBox.SetValue(Canvas.LeftProperty, 0d);
            for (int i = 1; i <= this.showLevelCount; i++)
            {
                BitmapImage bmpLevelIcon = ResIcon.Levels[i - 1];
                double levelIconWidth = bmpLevelIcon.PixelWidth;
                double levelIconHegiht = bmpLevelIcon.PixelHeight;
                double levelIconTop = (Constant.ScreenHeight - levelIconHegiht) / 2;
                double levelIconLeft = 280 + 400 * (i - 1);
                //背景图片容器
                Canvas cvLevelIcon = new Canvas
                {
                    Name = string.Format("Canvas_LevelIcon_{0}", i),
                    Width = levelIconWidth,
                    Height = levelIconHegiht,
                    Background = bmpLevelIcon.ToImageBrush()
                };
                cvLevelIcon.SetValue(Canvas.TopProperty, levelIconTop);
                cvLevelIcon.SetValue(Canvas.LeftProperty, levelIconLeft);
                //分数和星级
                Grid scoreGrid = new Grid { Height = 30, Width = 200 };
                scoreGrid.SetValue(Canvas.LeftProperty, 20d);
                scoreGrid.SetValue(Canvas.TopProperty, 190d);
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(30);
                scoreGrid.RowDefinitions.Add(row);
                for (int c = 1; c <= 2; c++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(100);
                    scoreGrid.ColumnDefinitions.Add(column);
                }

                var curLevelGates = this.levelGates.Where(g => g.LevelID == i);
                int sum_LevelScore = curLevelGates.Sum(g => g.Score);
                if (sum_LevelScore < 0) sum_LevelScore = 0;
                int sum_LevelAllStar = curLevelGates.Count() * 3;
                int sum_LevelGetStar = curLevelGates.Sum(g => g.Star);

                WriteableBitmap tmpWB = sum_LevelScore.ToString().ToWriteableBitmap(NumberCharSize.Sixteen);
                Image scoreImg = new Image
                {
                    Name = string.Format("Img_LevelScore_{0}", i),
                    Source = tmpWB,
                    Stretch = Stretch.None,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                };
                Grid.SetRow(scoreImg, 0);
                Grid.SetColumn(scoreImg, 0);
                scoreGrid.Children.Add(scoreImg);

                tmpWB = string.Format("{0}/{1}", sum_LevelGetStar, sum_LevelAllStar).ToWriteableBitmap(NumberCharSize.Sixteen);
                Image starImg = new Image
                {
                    Name = string.Format("Img_LevelStar_{0}", i),
                    Source = tmpWB,
                    Stretch = Stretch.None,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                };
                Grid.SetRow(starImg, 0);
                Grid.SetColumn(starImg, 1);
                scoreGrid.Children.Add(starImg);

                cvLevelIcon.Children.Add(scoreGrid);
                this.canvasLevelBox.Children.Add(cvLevelIcon);
                tmpWB = null;
            }
            this.LayoutRoot.Children.Add(this.canvasLevelBox);
        }

        /// <summary>
        /// 获取最近一次游戏记录
        /// </summary>
        private void LoadGameHistory()
        {
            int lastPlayLevelId = GameData.GetLastGameHistory().LevelID;
            if (lastPlayLevelId != this.currentLevel)
                LevelBoxMoveTo(lastPlayLevelId);
        }

        /// <summary>
        /// 界面控件移动
        /// </summary>
        /// <param name="moveToId">将设置为当前难度的难度序号</param>
        private void LevelBoxMoveTo(int moveToId)
        {
            this.moveToLevel = moveToId;
            double moveLen = (this.currentLevel - moveToId) * 400;
            double currentX = this.translation.X;
            double moveToX = currentX + moveLen;
            LevelBoxMoving(currentX, moveToX, new EventHandler(LevelBoxMoved));
        }

        /// <summary>
        /// 移动界面控件
        /// </summary>
        /// <param name="from">起始位置</param>
        /// <param name="to">目标位置</param>
        /// <param name="movedCall">移动完成后回调</param>
        private void LevelBoxMoving(double from, double to, EventHandler movedCall)
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, this.translation);
            Storyboard.SetTargetProperty(da, new PropertyPath("(X)"));
            storyboard.Children.Add(da);
            storyboard.Begin();
            if (movedCall != null)
                storyboard.Completed += movedCall;
        }

        /// <summary>
        /// 移动完成后处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelBoxMoved(object sender, EventArgs e)
        {
            Image tmpImage = this.LayoutRoot.FindName(string.Format("Img_SerialLevel_{0}", this.currentLevel)) as Image;
            tmpImage.Visibility = Visibility.Collapsed;
            tmpImage = this.LayoutRoot.FindName(string.Format("Img_SerialLevel_{0}", this.moveToLevel)) as Image;
            tmpImage.Visibility = Visibility.Visible;

            tmpImage = this.LayoutRoot.FindName(string.Format("Img_PointLevel_{0}", this.currentLevel)) as Image;
            tmpImage.Source = ResIcon.DotDark;
            tmpImage = this.LayoutRoot.FindName(string.Format("Img_PointLevel_{0}", this.moveToLevel)) as Image;
            tmpImage.Source = ResIcon.DotLight;

            this.currentLevel = this.moveToLevel;
        }

        /// <summary>
        /// 控件多点触控完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LevelPage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            double moveEndX = this.translation.X;
            if (moveEndX > 0)
                LevelBoxMoving(this.translation.X, 0, null);

            double moveLeftLimit = -(this.canvasLevelBox.Width - Constant.ScreenWidth);
            if (moveEndX < moveLeftLimit)
                LevelBoxMoving(this.translation.X, moveLeftLimit, null);

            double moveLen = moveEndX - this.moveStartX;
            if (moveLen == 0)
            {
                if (e.OriginalSource is Canvas)
                {
                    Canvas element = e.OriginalSource as Canvas;
                    string elementName = element.GetValue(FrameworkElement.NameProperty).ToString();
                    if (elementName.Contains("Canvas_LevelIcon_"))
                    {
                        int levelIndex = Convert.ToInt32(elementName.Substring(17));
                        if (levelIndex == this.currentLevel)
                        {
                            NavigationService.Navigate(GamePage.PageGate.ToPageUri(string.Format("levelId={0}", levelIndex)));
                        }
                        else
                        {
                            this.moveToLevel = levelIndex;
                            double to = this.moveStartX + (this.currentLevel - levelIndex) * 400;
                            LevelBoxMoving(this.translation.X, to, new EventHandler(LevelBoxMoved));
                        }
                    }
                }
            }
            if (moveLen > 0 && this.currentLevel > 1)
            {
                this.moveToLevel = this.currentLevel - 1;
                LevelBoxMoving(this.translation.X, this.moveStartX + 400, new EventHandler(LevelBoxMoved));
            }
            if (moveLen < 0 && this.currentLevel < this.showLevelCount)
            {
                this.moveToLevel = this.currentLevel + 1;
                LevelBoxMoving(this.translation.X, this.moveStartX - 400, new EventHandler(LevelBoxMoved));
            }
        }
    }
}