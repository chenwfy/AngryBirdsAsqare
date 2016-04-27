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
    /// 游戏关卡选择界面类
    /// </summary>
    public partial class PageGate : PhoneApplicationPage
    {
        /// <summary>
        /// 当前页面类实例
        /// </summary>
        public static PageGate Instance { get; private set; }
        /// <summary>
        /// 自定义界面所有元素显示的顶级容器
        /// </summary>
        private Canvas pageLayoutRoot;
        /// <summary>
        /// 显示所有关卡图标的Canvas控件
        /// </summary>
        private Canvas canvasGateBox;
        /// <summary>
        /// canvasLevelBox移动控件
        /// </summary>
        private TranslateTransform translation = new TranslateTransform();
        /// <summary>
        /// canvasGateBox移动起始X坐标
        /// </summary>
        private double moveStartX = 0;
        /// <summary>
        /// 当前关卡所属的难度级别序号
        /// </summary>
        private int currentLevelId = 1;
        /// <summary>
        /// 游戏关卡数据
        /// </summary>
        private List<LevelGates> levelGates = new List<LevelGates>();
        /// <summary>
        /// 分页数量
        /// </summary>
        private int pageSize = 15;
        /// <summary>
        /// 当前难度级别所有关卡显示页数
        /// </summary>
        private int pageCount = 1;
        /// <summary>
        /// 当前显示的关卡分页序号
        /// </summary>
        private int pageIndex = 1;
        /// <summary>
        /// 将要移动至显示的关卡分页序号
        /// </summary>
        private int moveToPage = 1;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageGate()
        {
            InitializeComponent();
            Instance = this;
        }

        /// <summary>
        /// 重绘界面，当界面需要重绘时，供其他页面调用该方法
        /// </summary>
        /// <param name="levelId">当前难度级别序号</param>
        public void ReDrawPage(int levelId)
        {
            this.LayoutRoot.Children.Remove(this.pageLayoutRoot);
            //加载数据
            this.currentLevelId = levelId;
            LoadData();
        }

        #region override
        /// <summary>
        /// 重写导航至页面方法并接收传递的参数
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                this.currentLevelId = Convert.ToInt32(this.NavigationContext.QueryString["levelId"]);
                LoadData();
            }
        }
        #endregion

        /// <summary>
        /// 加载页面数据
        /// </summary>
        private void LoadData()
        {
            this.levelGates = GameData.GetLevelGateList(this.currentLevelId);
            int count = levelGates.Count();
            this.pageCount = count / pageSize;
            if (count % pageSize > 0)
                this.pageCount++;

            DrawPage();
        }

        /// <summary>
        /// 绘制界面布局
        /// </summary>
        private void DrawPage()
        {
            this.pageLayoutRoot = new Canvas
            {
                Width = Constant.ScreenWidth,
                Height = Constant.ScreenHeight,
                Background = ResScreen.GateBgs[this.currentLevelId - 1].ToImageBrush()
            };
            this.pageLayoutRoot.SetValue(Canvas.LeftProperty, 0d);
            this.pageLayoutRoot.SetValue(Canvas.TopProperty, 0d);
            this.LayoutRoot.Children.Add(this.pageLayoutRoot);

            if (this.pageCount > 1)
            {
                this.pageLayoutRoot.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>((sender, e) =>
                {
                    this.moveStartX = this.translation.X;
                });
                this.pageLayoutRoot.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>((sender, e) =>
                {
                    this.translation.X += e.DeltaManipulation.Translation.X;
                });
                this.pageLayoutRoot.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(GatePage_ManipulationCompleted);
                DrawPointGrid();
            }
            DrawLevelGatePage();
            LoadGameHistory();
        }

        /// <summary>
        /// 绘制界面下方的点和数字
        /// </summary>
        private void DrawPointGrid()
        {
            double pointGridHeight = 24;
            double pointGridWidth = 16 * this.pageCount;
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

            for (int c = 1; c <= this.pageCount; c++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(12);
                pointGrid.ColumnDefinitions.Add(column);

                WriteableBitmap tmpWB = c.ToString().ToWriteableBitmap();
                pointImg = new Image
                {
                    Source = tmpWB,
                    Stretch = Stretch.None,
                    Name = string.Format("Img_SerialPageIndex_{0}", c),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Visibility = (c == this.pageIndex) ? Visibility.Visible : Visibility.Collapsed
                };
                Grid.SetRow(pointImg, 0);
                Grid.SetColumn(pointImg, c - 1);
                pointGrid.Children.Add(pointImg);
                tmpWB = null;

                pointImg = new Image
                {
                    Source = (c == this.pageIndex) ? ResIcon.DotLight : ResIcon.DotDark,
                    Stretch = Stretch.None,
                    Name = string.Format("Img_PointPageIndex_{0}", c),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Visibility = Visibility.Visible
                };
                Grid.SetRow(pointImg, 1);
                Grid.SetColumn(pointImg, c - 1);
                pointGrid.Children.Add(pointImg);
            }
            this.pageLayoutRoot.Children.Add(pointGrid);
        }

        /// <summary>
        /// 绘制界面
        /// </summary>
        private void DrawLevelGatePage()
        {
            this.canvasGateBox = new Canvas
            {
                Height = Constant.ScreenHeight,
                Width = this.pageCount * Constant.ScreenWidth,
                MaxWidth = this.pageCount * Constant.ScreenWidth,
                MinWidth = this.pageCount * Constant.ScreenWidth,
                RenderTransform = this.translation
            };
            this.canvasGateBox.SetValue(Canvas.TopProperty, 0d);
            this.canvasGateBox.SetValue(Canvas.LeftProperty, 0d);

            int gateIdx = 0;
            double canvasPageLeft = 0;
            double gateBoxLeft = 110;
            double gateBoxTop = 70;
            double gateBoxMargin = 20;
            for (int p = 1; p <= this.pageCount; p++)
            {
                Canvas canvasPage = new Canvas
                {
                    Width = Constant.ScreenWidth,
                    Height = Constant.ScreenHeight
                };
                canvasPage.SetValue(Canvas.TopProperty, 0d);
                canvasPage.SetValue(Canvas.LeftProperty, canvasPageLeft);

                int leftIdx = 0, topIdx = 0;
                for (int i = 1; i <= this.pageSize; i++)
                {
                    LevelGates curGatedata = this.levelGates[gateIdx];
                    BitmapImage bmpGateIcon = ResIcon.Gates[0];
                    if (curGatedata.Score >= 0)
                        bmpGateIcon = ResIcon.Gates[this.currentLevelId];
                    double gateIconWidth = bmpGateIcon.PixelWidth;
                    double gateIconHeight = bmpGateIcon.PixelHeight;
                    double curGateLeft = gateBoxLeft + leftIdx * (gateBoxMargin + gateIconWidth);
                    double curGateTop = gateBoxTop + topIdx * (gateBoxMargin + gateIconHeight);
                    if (i % 5 == 0)
                    {
                        leftIdx = 0;
                        topIdx++;
                    }
                    else
                    {
                        leftIdx++;
                    }

                    Canvas curGateBox = new Canvas
                    {
                        Name = string.Format("Canvas_GateBox_{0}", curGatedata.GateID),
                        Width = gateIconWidth,
                        Height = gateIconHeight,
                        Background = bmpGateIcon.ToImageBrush()
                    };
                    curGateBox.SetValue(Canvas.TopProperty, curGateTop);
                    curGateBox.SetValue(Canvas.LeftProperty, curGateLeft);

                    if (curGatedata.Score >= 0)
                    {
                        WriteableBitmap wbGateIdx = curGatedata.GateID.ToString().ToWriteableBitmap(NumberCharSize.Twentyfour);
                        Image imgGateIdx = new Image { Source = wbGateIdx, Stretch = Stretch.None };
                        imgGateIdx.SetValue(Canvas.TopProperty, (double)((75 - wbGateIdx.PixelHeight) / 2));
                        imgGateIdx.SetValue(Canvas.LeftProperty, (double)((gateIconWidth - wbGateIdx.PixelWidth) / 2));
                        curGateBox.Children.Add(imgGateIdx);
                        wbGateIdx = null;

                        double starImgLeft = 20;
                        if (curGatedata.Star > 0)
                        {
                            for (int s = 1; s <= curGatedata.Star; s++)
                            {
                                BitmapImage bmpStarIcon = ResIcon.StarLight;
                                Image imgStarIcon = new Image { Source = bmpStarIcon, Stretch = Stretch.None };
                                imgStarIcon.SetValue(Canvas.TopProperty, (double)(77 + (25 - bmpStarIcon.PixelHeight) / 2));
                                imgStarIcon.SetValue(Canvas.LeftProperty, starImgLeft);
                                curGateBox.Children.Add(imgStarIcon);

                                starImgLeft += bmpStarIcon.PixelWidth + 5;
                            }
                        }

                        if (this.pageCount == 1)
                        {
                            curGateBox.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
                            {
                                if (e.OriginalSource is Canvas)
                                    GateBoxTap(e.OriginalSource as Canvas);
                            });
                        }
                    }
                    canvasPage.Children.Add(curGateBox);
                    gateIdx++;
                }
                this.canvasGateBox.Children.Add(canvasPage);
                canvasPageLeft += Constant.ScreenWidth;
            }
            this.pageLayoutRoot.Children.Add(this.canvasGateBox);
        }

        /// <summary>
        /// 加载玩家当前难度级别的游戏历史记录
        /// </summary>
        private void LoadGameHistory()
        {
            int lastPlayGateId = GameData.GetLastGameHistoryByLevel(this.currentLevelId).GateID;
            int moveToPageIdx = lastPlayGateId / this.pageSize;
            if (lastPlayGateId % this.pageSize > 0)
                moveToPageIdx++;

            if (moveToPageIdx != this.pageIndex)
                GateBoxMoveToPage(moveToPageIdx);
        }

        /// <summary>
        /// 将当前关卡列表移动至目标分页
        /// </summary>
        /// <param name="toPage">需要移动到的分页页码</param>
        private void GateBoxMoveToPage(int toPage)
        {
            this.moveToPage = toPage;
            double moveLen = (this.pageIndex - toPage) * Constant.ScreenWidth;
            double currentX = this.translation.X;
            double moveToX = currentX + moveLen;
            GateBoxMoving(currentX, moveToX, new EventHandler(GateBoxMoved));
        }

        /// <summary>
        /// 移动界面控件
        /// </summary>
        /// <param name="from">起始位置</param>
        /// <param name="to">目标位置</param>
        /// <param name="movedCall">移动完成后回调</param>
        private void GateBoxMoving(double from, double to, EventHandler movedCall)
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
        private void GateBoxMoved(object sender, EventArgs e)
        {
            Image tmpImage = this.pageLayoutRoot.FindName(string.Format("Img_SerialPageIndex_{0}", this.pageIndex)) as Image;
            tmpImage.Visibility = Visibility.Collapsed;
            tmpImage = this.pageLayoutRoot.FindName(string.Format("Img_SerialPageIndex_{0}", this.moveToPage)) as Image;
            tmpImage.Visibility = Visibility.Visible;

            tmpImage = this.pageLayoutRoot.FindName(string.Format("Img_PointPageIndex_{0}", this.pageIndex)) as Image;
            tmpImage.Source = ResIcon.DotDark;
            tmpImage = this.pageLayoutRoot.FindName(string.Format("Img_PointPageIndex_{0}", this.moveToPage)) as Image;
            tmpImage.Source = ResIcon.DotLight;

            this.pageIndex = this.moveToPage;
        }

        /// <summary>
        /// 界面多点触控结束事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GatePage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            double moveEndX = this.translation.X;
            if (moveEndX > 0)
                GateBoxMoving(this.translation.X, 0, null);

            double moveLeftLimit = -((this.pageCount - 1) * Constant.ScreenWidth);
            if (moveEndX < moveLeftLimit)
                GateBoxMoving(this.translation.X, moveLeftLimit, null);

            double moveLen = moveEndX - this.moveStartX;
            if (moveLen == 0)
            {
                if (e.OriginalSource is Canvas)
                    GateBoxTap(e.OriginalSource as Canvas);
            }
            if (moveLen > 0 && this.pageIndex > 1)
            {
                this.moveToPage = this.pageIndex - 1;
                GateBoxMoving(this.translation.X, this.moveStartX + Constant.ScreenWidth, new EventHandler(GateBoxMoved));
            }
            if (moveLen < 0 && this.pageIndex < this.pageCount)
            {
                this.moveToPage = this.pageIndex + 1;
                GateBoxMoving(this.translation.X, this.moveStartX - Constant.ScreenWidth, new EventHandler(GateBoxMoved));
            }
        }

        /// <summary>
        /// 关卡图标控件单点事件
        /// </summary>
        /// <param name="element">关卡图标控件</param>
        private void GateBoxTap(Canvas element)
        {
            string elementName = element.GetValue(FrameworkElement.NameProperty).ToString();
            if (elementName.Contains("Canvas_GateBox_"))
            {
                int gateIndex = Convert.ToInt32(elementName.Substring(15));
                if (this.levelGates.Single(g => g.LevelID == this.currentLevelId && g.GateID == gateIndex).Score >= 0)
                    NavigationService.Navigate(GamePage.PageGame.ToPageUri(string.Format("levelId={0}&gateId={1}", this.currentLevelId, gateIndex)));
            }
        }
    }
}