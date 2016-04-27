using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls.Primitives;
using SLGame.AngryBirdsAsqare.Resource;
using SLGame.AngryBirdsAsqare.Core;
using SLGame.AngryBirdsAsqare.Database;

namespace SLGame.AngryBirdsAsqare.View
{
    /// <summary>
    /// 游戏界面类
    /// </summary>
    public partial class PageGame : PhoneApplicationPage
    {
        #region 私有成员
        /// <summary>
        /// 当前游戏难度级别
        /// </summary>
        private int currentLevelId = 1;
        /// <summary>
        /// 当前游戏关卡序号
        /// </summary>
        private int currentGateId = 1;
        /// <summary>
        /// 当前游戏数据
        /// </summary>
        private LevelGates currentGateData;
        /// <summary>
        /// 当前关卡地图布局数据
        /// </summary>
        private List<Dictionary<int, ResInfo>> currentGameItemsList = new List<Dictionary<int, ResInfo>>();
        /// <summary>
        /// 地图（游戏区域）起始（左上角）坐标
        /// </summary>
        private System.Windows.Point mapStartpoint = new System.Windows.Point(16, 16);
        /// <summary>
        /// 地图格子（正方形）大小
        /// </summary>
        private double gameItemSize = 48;
        /// <summary>
        /// 游戏剩余时间进度Image
        /// </summary>
        private Image imgGameTime;
        /// <summary>
        /// 游戏剩余时间进度Image最大宽度
        /// </summary>
        private double imgGameTimeWidth = 0;
        /// <summary>
        /// 游戏剩余时间
        /// </summary>
        private int gameLastTime = 0;
        /// <summary>
        /// 游戏地图格子序号集合
        /// </summary>
        private List<List<int>> gameGridList = new List<List<int>>();
        /// <summary>
        /// 当前正在游戏中的地图网格序号集合
        /// </summary>
        private List<int> currentGameGridList = new List<int>();
        /// <summary>
        /// 当前游戏地图序号（1-3）
        /// </summary>
        private int currentMapIndex = 0;
        /// <summary>
        /// 当前正在游戏中的地图对象
        /// </summary>
        private Canvas currentGameMapBox;
        /// <summary>
        /// 当前游戏得分
        /// </summary>
        private int gameScore = 0;
        /// <summary>
        /// 游戏当前得分显示图片
        /// </summary>
        private Image imgGameScore;
        /// <summary>
        /// 游戏剩余方块数量
        /// </summary>
        private Image imgGameLastItems;
        /// <summary>
        /// 游戏菜单
        /// </summary>
        private Canvas gameMenuBox;
        /// <summary>
        /// 当前游戏关卡序号图片
        /// </summary>
        private Image imgGateIndex;
        /// <summary>
        /// 菜单遮罩层
        /// </summary>
        private Canvas gameShadow;
        /// <summary>
        /// 背景音乐文件uri
        /// </summary>
        private Uri gameBackgroundSoundUri;
        /// <summary>
        /// 背景音乐播放实例
        /// </summary>
        private SoundEffectInstance bgSoundInstance = null;
        /// <summary>
        /// 游戏音效播放实例
        /// </summary>
        private SoundEffectInstance gameSoundInstance = null;
        /// <summary>
        /// 游戏结束音效文件名称集合
        /// </summary>
        private string[] endSoundNames = new string[] { ResName.Sound_EndFail, ResName.Sound_OneStar, ResName.Sound_TwoStar, ResName.Sound_ThreeStar };
        /// <summary>
        /// 游戏开始倒计时秒数
        /// </summary>
        private int gameBeginCountDown = 3;
        /// <summary>
        /// 当前游戏状态
        /// </summary>
        private GameStatus currentStatus = GameStatus.None;
        /// <summary>
        /// 最后一次有效点击的方块对象
        /// </summary>
        private ItemTap lastTapItem = null;
        /// <summary>
        /// 最后一次有效点击的时间
        /// </summary>
        private DateTime lastClickTime;
        /// <summary>
        /// 有效连击次数
        /// </summary>
        private int comboCount = 0;
        /// <summary>
        /// 连击提示文字图片对象
        /// </summary>
        private Image imgComboTxt;
        /// <summary>
        /// 连击数字图片对象
        /// </summary>
        private Image imgComboCount;
        /// <summary>
        /// 当前游戏计时器
        /// </summary>
        private DispatcherTimer currentGameTimer;
        /// <summary>
        /// 游戏暂停开始时间
        /// </summary>
        private DateTime gamePausStartTime;
        /// <summary>
        /// 游戏结束提示对话容器对象
        /// </summary>
        private Canvas gameOverBox = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageGame()
        {
            InitializeComponent();

            //构建XNA模式的模拟环境
            GameTimer gameTimer = new GameTimer();
            gameTimer.UpdateInterval = TimeSpan.FromMilliseconds(33);
            gameTimer.Update += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            gameTimer.Start();
            FrameworkDispatcher.Update();

            //暂停之前界面的背景音乐
            if (App.GameAudioEnable)
            {
                if (PageStart.Instance.BgSoundInstance != null && PageStart.Instance.BgSoundInstance.State == SoundState.Playing)
                    PageStart.Instance.BgSoundInstance.Pause();
            }

            //界面背景
            this.LayoutRoot.Background = ResScreen.GameBg.ToImageBrush();

            //加载完成后执行
            this.Loaded += new RoutedEventHandler((sender, e) =>
            {
                DrawPage();
            });
        }
        #endregion

        #region override
        /// <summary>
        /// 重写导航至页面方法并接收传递的参数
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.currentLevelId = Convert.ToInt32(this.NavigationContext.QueryString["levelId"]);
            this.currentGateId = Convert.ToInt32(this.NavigationContext.QueryString["gateId"]);
        }

        /// <summary>
        /// 重写导航出页面方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (App.GameAudioEnable)
                { 
                    //停止播放当前界面背景音乐，并释放实例对象 
                    StopBackgroundSound();
                    //停止播放当前播放中的游戏音效，并释放实例对象 
                    StopGameSoundEffect();
                    //重新播放之前界面的背景音乐
                    if (PageStart.Instance.BgSoundInstance != null && PageStart.Instance.BgSoundInstance.State != SoundState.Playing)
                        PageStart.Instance.BgSoundInstance.Resume();
                }
            }
            else 
            {
                if (this.currentStatus == GameStatus.Playing)
                    ShowGameMenu();
            }
        }

        /// <summary>
        /// 重写返回键按键事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (this.currentStatus == GameStatus.Playing)
            {
                e.Cancel = true;
                ShowGameMenu();
            }
            else if (this.currentStatus == GameStatus.Pause)
            {
                e.Cancel = true;
                HideGameMenu();
            }
            else if (this.currentStatus == GameStatus.End)
            {
                e.Cancel = true;
                if (this.gameOverBox != null)
                {
                    this.gameOverBox.Visibility = Visibility.Collapsed;
                    this.gameShadow.Visibility = Visibility.Collapsed;
                    int tIdx = 0;
                    DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(200) };
                    timer.Tick += new EventHandler((sender, ev) =>
                    {
                        tIdx++;
                        if (1 == tIdx)
                        {
                            timer.Stop();
                            Goback();
                        }
                    });
                    timer.Start();
                }
                else
                    Goback();
            }
            else
                Goback();
        }

        /// <summary>
        /// 回退
        /// </summary>
        private void Goback()
        {
            PageGate.Instance.ReDrawPage(this.currentLevelId);
            PageLevel.Instance.ReDrawPage();
            NavigationService.GoBack();
        }
        #endregion

        #region 停止并释放声音对象
        /// <summary>
        /// 背景音乐
        /// </summary>
        private void StopBackgroundSound()
        {
            if (this.bgSoundInstance != null)
            {
                if (this.bgSoundInstance.State == SoundState.Playing)
                    this.bgSoundInstance.Stop();
                this.bgSoundInstance.Dispose();
                this.bgSoundInstance = null;
            }
        }

        /// <summary>
        /// 游戏音效
        /// </summary>
        private void StopGameSoundEffect()
        {
            if (this.gameSoundInstance != null)
            {
                if (this.gameSoundInstance.State == SoundState.Playing)
                    this.gameSoundInstance.Stop();
                this.gameSoundInstance.Dispose();
                this.gameSoundInstance = null;
            }
        }
        #endregion

        #region 界面布局
        /// <summary>
        /// 绘制界面
        /// </summary>
        private void DrawPage()
        {
            #region 游戏界面按钮和文字
            //暂停按钮
            Image imgPause = new Image { Source = ResButton.Pause, Stretch = Stretch.None };
            imgPause.SetValue(Canvas.LeftProperty, 32d);
            imgPause.SetValue(Canvas.TopProperty, 408d);
            imgPause.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { ShowGameMenu(); });
            this.LayoutRoot.Children.Add(imgPause);
            //重排按钮
            Image imgRefresh = new Image { Source = ResButton.Refresh, Stretch = Stretch.None };
            imgRefresh.SetValue(Canvas.LeftProperty, 126d);
            imgRefresh.SetValue(Canvas.TopProperty, 408d);
            imgRefresh.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { RefreshLastItems(); });
            this.LayoutRoot.Children.Add(imgRefresh);
            //倒计时
            Image imgtimeTxt = new Image { Source = ResTxt.Time, Stretch = Stretch.None };
            imgtimeTxt.SetValue(Canvas.LeftProperty, 230d);
            imgtimeTxt.SetValue(Canvas.TopProperty, 408d);
            this.LayoutRoot.Children.Add(imgtimeTxt);

            BitmapImage bmpTimeBg = ResIcon.Time_Bg;
            Canvas timeBox = new Canvas
            {
                Width = bmpTimeBg.PixelWidth,
                Height = bmpTimeBg.PixelHeight,
                Background = bmpTimeBg.ToImageBrush()
            };
            timeBox.SetValue(Canvas.LeftProperty, 322d);
            timeBox.SetValue(Canvas.TopProperty, 412d);

            this.imgGameTimeWidth = bmpTimeBg.PixelWidth - 4;
            this.imgGameTime = new Image
            {
                Source = ResIcon.Time_Fore,
                Stretch = Stretch.Fill,
                Width = this.imgGameTimeWidth,
                Height = bmpTimeBg.PixelHeight
            };
            this.imgGameTime.SetValue(Canvas.LeftProperty, 2d);
            this.imgGameTime.SetValue(Canvas.TopProperty, 0d);

            timeBox.Children.Add(this.imgGameTime);
            this.LayoutRoot.Children.Add(timeBox);
            //当前得分
            Image imgScoreTxt = new Image { Source = ResTxt.Score, Stretch = Stretch.None };
            imgScoreTxt.SetValue(Canvas.LeftProperty, 230d);
            imgScoreTxt.SetValue(Canvas.TopProperty, 440d);
            this.LayoutRoot.Children.Add(imgScoreTxt);

            this.imgGameScore = new Image { Source = "0".ToWriteableBitmap(NumberCharSize.Twentyfour), Stretch = Stretch.None };
            this.imgGameScore.SetValue(Canvas.LeftProperty, (double)322);
            this.imgGameScore.SetValue(Canvas.TopProperty, (double)440);
            this.LayoutRoot.Children.Add(this.imgGameScore);
            //连击提示
            this.imgComboTxt = new Image { Source = ResTxt.Combo, Stretch = Stretch.None };
            this.imgComboTxt.SetValue(Canvas.LeftProperty, 410d);
            this.imgComboTxt.SetValue(Canvas.TopProperty, 440d);
            this.imgComboTxt.Visibility = Visibility.Collapsed;
            this.LayoutRoot.Children.Add(this.imgComboTxt);

            this.imgComboCount = new Image { Source = "0".ToWriteableBitmap(NumberCharSize.Twentyfour), Stretch = Stretch.None };
            this.imgComboCount.SetValue(Canvas.LeftProperty, 492d);
            this.imgComboCount.SetValue(Canvas.TopProperty, 447d);
            this.imgComboCount.Visibility = Visibility.Collapsed;
            this.LayoutRoot.Children.Add(this.imgComboCount);
            //剩余数量
            Image imgLastTxt = new Image { Source = ResTxt.Last, Stretch = Stretch.None };
            imgLastTxt.SetValue(Canvas.LeftProperty, 620d);
            imgLastTxt.SetValue(Canvas.TopProperty, 440d);
            this.LayoutRoot.Children.Add(imgLastTxt);

            this.imgGameLastItems = new Image { Source = "0".ToWriteableBitmap(NumberCharSize.Twentyfour), Stretch = Stretch.None };
            this.imgGameLastItems.SetValue(Canvas.LeftProperty, 716d);
            this.imgGameLastItems.SetValue(Canvas.TopProperty, 440d);
            this.LayoutRoot.Children.Add(this.imgGameLastItems);
            #endregion

            #region 游戏菜单
            //游戏菜单背景
            BitmapImage bmpMenuBg = ResIcon.GameMenu_Bg;
            double menuBoxWidth = bmpMenuBg.PixelWidth;
            this.gameMenuBox = new Canvas
            {
                Width = menuBoxWidth,
                Height = Constant.ScreenHeight,
                Background = bmpMenuBg.ToImageBrush(),
            };
            this.gameMenuBox.SetValue(Canvas.LeftProperty, -menuBoxWidth);
            this.gameMenuBox.SetValue(Canvas.TopProperty, 0d);
            this.gameMenuBox.SetValue(Canvas.ZIndexProperty, 10);
            //关卡序号
            WriteableBitmap wbGateIndex = "0-0".ToWriteableBitmap(NumberCharSize.Fortyeight);
            double wbGateIndexWidth = wbGateIndex.PixelWidth;
            this.imgGateIndex = new Image { Source = wbGateIndex, Stretch = Stretch.None };
            this.imgGateIndex.SetValue(Canvas.LeftProperty, (double)((194 - wbGateIndexWidth) / 2));
            this.imgGateIndex.SetValue(Canvas.TopProperty, (double)21);
            this.gameMenuBox.Children.Add(this.imgGateIndex);
            wbGateIndex = null;
            //重新开始
            Image imgReplay = new Image { Source = ResButton.RePlayNomal, Stretch = Stretch.None };
            imgReplay.SetValue(Canvas.LeftProperty, 47d);
            imgReplay.SetValue(Canvas.TopProperty, 105d);
            imgReplay.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { PlayAgin(); });
            this.gameMenuBox.Children.Add(imgReplay);
            //返回上一级
            Image imgReturn = new Image { Source = ResButton.GobackNomal, Stretch = Stretch.None };
            imgReturn.SetValue(Canvas.LeftProperty, 47d);
            imgReturn.SetValue(Canvas.TopProperty, 235d);
            imgReturn.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { Goback(); });
            this.gameMenuBox.Children.Add(imgReturn);
            //声音开关
            BitmapImage bmpAudio = App.GameAudioEnable ? ResButton.AudioNomal : ResButton.DisenableNomal;
            Image imgAudio = new Image { Source = bmpAudio, Stretch = Stretch.None };
            imgAudio.SetValue(Canvas.LeftProperty, 47d);
            imgAudio.SetValue(Canvas.TopProperty, 365d);
            imgAudio.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
            {
                if (App.GameAudioEnable)
                {
                    imgAudio.Source = ResButton.DisenableNomal;
                    GameData.UpdateGameAudioSet(false);
                    App.GameAudioEnable = false;
                    if (this.bgSoundInstance != null && this.bgSoundInstance.State == SoundState.Playing)
                        this.bgSoundInstance.Pause();
                }
                else
                {
                    imgAudio.Source = ResButton.AudioNomal;
                    GameData.UpdateGameAudioSet(true);
                    App.GameAudioEnable = true;
                    if (this.bgSoundInstance != null && this.bgSoundInstance.State != SoundState.Playing)
                        this.bgSoundInstance.Resume();
                }
                PageStart.Instance.ChanggeAudioImage();
            });
            this.gameMenuBox.Children.Add(imgAudio);
            //继续游戏
            Image imgPlay = new Image { Source = ResButton.Play, Stretch = Stretch.None };
            imgPlay.SetValue(Canvas.LeftProperty, 166d);
            imgPlay.SetValue(Canvas.TopProperty, 208d);
            imgPlay.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { HideGameMenu(); });
            this.gameMenuBox.Children.Add(imgPlay);

            this.LayoutRoot.Children.Add(this.gameMenuBox);
            #endregion

            #region 游戏结果弹出遮罩层
            this.gameShadow = new Canvas
            {
                Width = Constant.ScreenWidth,
                Height = Constant.ScreenHeight,
                Background = new SolidColorBrush(Colors.Black),
                Opacity = 0.6,
                Visibility = Visibility.Collapsed
            };
            this.gameShadow.SetValue(Canvas.LeftProperty, 0d);
            this.gameShadow.SetValue(Canvas.TopProperty, 0d);
            this.gameShadow.SetValue(Canvas.ZIndexProperty, 9);
            this.LayoutRoot.Children.Add(this.gameShadow);
            #endregion

            //加载当前关卡数据
            LoadGameData();
        }

        /// <summary>
        /// 加载游戏数据
        /// </summary>
        private void LoadGameData()
        {
            this.currentGateData = GameData.GetSingleGate(this.currentLevelId, this.currentGateId);
            //关卡地图布局数据
            this.currentGameItemsList = GameData.GetGameMapItemsList(this.currentGateData.MapCount, this.currentGateData.MapSize, this.currentGateData.ItemCount);
            //添加历史记录
            GameHistory history = new GameHistory { LevelID = this.currentLevelId, GateID = this.currentGateId };
            GameData.InsertGameHistory(history);

            //当前关卡序号
            DrawGateIndex();
            //随机一首背景音乐
            RandBackgroundSound();
            //开始游戏
            GameBegin();
        }

        /// <summary>
        /// 绘制当前关卡序号部分
        /// </summary>
        private void DrawGateIndex()
        {
            WriteableBitmap wbGateIndex = string.Format("{0}-{1}", this.currentLevelId, this.currentGateId).ToWriteableBitmap(NumberCharSize.Fortyeight);
            double wbGateIndexWidth = wbGateIndex.PixelWidth;
            this.imgGateIndex.Source = wbGateIndex;
            this.imgGateIndex.SetValue(Canvas.LeftProperty, (double)((194 - wbGateIndexWidth) / 2));
            this.imgGateIndex.SetValue(Canvas.TopProperty, (double)21);
            wbGateIndex = null;
        }

        /// <summary>
        /// 随机一个背景音乐文件
        /// </summary>
        private void RandBackgroundSound()
        {
            List<string> musicName = ResName.Music_GameBg;
            Random rand = new Random();
            int mIdx = rand.Next(0, musicName.Count);
            this.gameBackgroundSoundUri = musicName[mIdx].ToAudioUri();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void GameBegin()
        {
            //初始化部分变量
            this.currentMapIndex = 1;
            this.gameScore = 0;
            this.gameLastTime = Constant.GameTime[this.currentGateData.MapCount - 1];
            this.gameBeginCountDown = 3;
            this.comboCount = 0;
            this.lastTapItem = null;

            //清楚游戏结束提示对话
            if (this.gameOverBox != null)
                this.LayoutRoot.Children.Remove(this.gameOverBox);

            //初始化倒计时
            this.gameShadow.Visibility = Visibility.Visible;
            WriteableBitmap wb = this.gameBeginCountDown.ToString().ToWriteableBitmap(NumberCharSize.Fortyeight);
            Image imgGameBeginCountDown = new Image { Source = wb, Stretch = Stretch.None };
            imgGameBeginCountDown.SetValue(Canvas.TopProperty, Convert.ToDouble((Constant.ScreenHeight - wb.PixelHeight) / 2));
            imgGameBeginCountDown.SetValue(Canvas.LeftProperty, Convert.ToDouble((Constant.ScreenWidth - wb.PixelWidth) / 2));
            imgGameBeginCountDown.SetValue(Canvas.ZIndexProperty, 20);
            this.LayoutRoot.Children.Add(imgGameBeginCountDown);
            wb = null;

            DispatcherTimer countDownTimer = new DispatcherTimer();
            countDownTimer.Interval = TimeSpan.FromSeconds(1);
            countDownTimer.Tick += new EventHandler((sender, e) =>
            {
                this.gameBeginCountDown--;
                if (0 == this.gameBeginCountDown)
                {
                    countDownTimer.Stop();
                    this.LayoutRoot.Children.Remove(imgGameBeginCountDown);
                    this.gameShadow.Visibility = Visibility.Collapsed;

                    //游戏背景声音
                    StopBackgroundSound();
                    this.bgSoundInstance = SoundEffect.FromStream(this.gameBackgroundSoundUri.ToAudioDataStream()).CreateInstance();
                    this.bgSoundInstance.Volume = Constant.PageSoundVolume;
                    this.bgSoundInstance.IsLooped = true;
                    //游戏音效
                    StopGameSoundEffect();
                    this.gameSoundInstance = SoundEffect.FromStream(ResName.Sound_Begin.ToAudioDataStream()).CreateInstance();
                    this.gameSoundInstance.Volume = Constant.GameSoundVolume;
                    this.gameSoundInstance.IsLooped = false;
                    //播放声音
                    if (App.GameAudioEnable)
                    {
                        this.bgSoundInstance.Play();
                        this.gameSoundInstance.Play();
                    }

                    this.currentStatus = GameStatus.Playing;
                    this.lastClickTime = DateTime.Now;

                    this.currentGameTimer = new DispatcherTimer();
                    this.currentGameTimer.Interval = TimeSpan.FromMilliseconds(1000);
                    this.currentGameTimer.Tick += new EventHandler((objectSender, events) =>
                    {
                        if (this.gameLastTime <= 0)
                        {
                            this.currentGameTimer.Stop();
                            //游戏结束
                            //GameOver();
                            int tmpIdx = 0;
                            DispatcherTimer tmpTimer = new DispatcherTimer();
                            tmpTimer.Interval = TimeSpan.FromMilliseconds(1000);
                            tmpTimer.Tick += new EventHandler((s, v) =>
                            {
                                tmpIdx++;
                                if (1 == tmpIdx)
                                {
                                    tmpTimer.Stop();
                                    GameOver();
                                }
                            });
                            tmpTimer.Start();
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            if (now.Subtract(this.lastClickTime).TotalMilliseconds > 3000)
                            {
                                this.gameLastTime--;
                                ReDrawGameTime();
                            }
                        }
                    });
                    this.currentGameTimer.Start();
                }
                else
                    imgGameBeginCountDown.Source = this.gameBeginCountDown.ToString().ToWriteableBitmap(NumberCharSize.Fortyeight);
            });
            countDownTimer.Start();

            //绘制地图
            DrawMap();
            //根据关卡数据重绘界面部分控件
            ReDrawGameLastItems();
            ReDrawGameScore();
            ReDrawGameTime();            
        }

        /// <summary>
        /// 绘制地图
        /// </summary>
        private void DrawMap()
        {
            int mapCount = this.currentGateData.MapCount;
            for (int m = 1; m <= mapCount; m++)
            {
                var mapBox = this.LayoutRoot.FindName(string.Format("Canvas_GameMap_{0}", m));
                if (mapBox != null)
                {
                    Canvas box = mapBox as Canvas;
                    this.LayoutRoot.Children.Remove(box);
                }
            }

            this.gameGridList = new List<List<int>>();

            for (int m = 1; m <= mapCount; m++)
            {
                Canvas mapBox = new Canvas
                {
                    Name = string.Format("Canvas_GameMap_{0}", m),
                    Width = 768d,
                    Height = 384d
                };
                double boxTop = 1 == m ? this.mapStartpoint.Y : (-(m - 1) * 384);
                mapBox.SetValue(Canvas.LeftProperty, this.mapStartpoint.X);
                mapBox.SetValue(Canvas.TopProperty, boxTop);

                Dictionary<int, ResInfo> items = this.currentGameItemsList[m - 1];
                double x = 0;
                double y = 0;
                List<int> curMapGridList = new List<int>();
                for (int i = 1; i <= 128; i++)
                {
                    if (items.Keys.Contains(i))
                    {
                        ResInfo item = items[i];
                        string itemName = string.Format("Img_Map{0}_{1}_{2}", m, i, item.Code);
                        Image imgItem = new Image
                        {
                            Source = item.Data,
                            Stretch = Stretch.None,
                            Name = itemName
                        };
                        ItemTap itemTap = new ItemTap { ItemName = itemName, ItemGrid = i, ItemResCode = item.Code, ItemImage = imgItem };
                        imgItem.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { GameItemImage_Tap(itemTap); });
                        imgItem.SetValue(Canvas.LeftProperty, x * this.gameItemSize);
                        imgItem.SetValue(Canvas.TopProperty, y * this.gameItemSize);
                        mapBox.Children.Add(imgItem);

                        curMapGridList.Add(i);
                    }
                    if (i % 16 == 0)
                    {
                        x = 0;
                        y++;
                    }
                    else
                    {
                        x++;
                    }
                }

                this.LayoutRoot.Children.Add(mapBox);
                this.gameGridList.Add(curMapGridList);

                if (1 == m)
                {
                    this.currentGameMapBox = mapBox;
                    this.currentGameGridList = curMapGridList; 
                }
            }
        }

        /// <summary>
        /// 重绘当前游戏剩余方块数量
        /// </summary>
        private void ReDrawGameLastItems()
        {
            List<int> curMapSize = this.currentGameGridList;
            this.imgGameLastItems.Source = curMapSize.Count.ToString().ToWriteableBitmap(NumberCharSize.Twentyfour);
        }

        /// <summary>
        /// 重绘游戏得分
        /// </summary>
        private void ReDrawGameScore()
        {
            this.imgGameScore.Source = this.gameScore.ToString().ToWriteableBitmap(NumberCharSize.Twentyfour);
        }

        /// <summary>
        /// 重绘游戏剩余时间进度
        /// </summary>
        private void ReDrawGameTime()
        {
            double imgNowLeft = Convert.ToDouble(this.imgGameTime.GetValue(Canvas.LeftProperty));
            double imgNowWidth = Convert.ToDouble(this.imgGameTime.GetValue(FrameworkElement.WidthProperty));
            double imgLastWidth = Convert.ToDouble(this.imgGameTimeWidth * Convert.ToDouble((double)this.gameLastTime / (double)Constant.GameTime[this.currentGateData.MapCount - 1]));
            double imgDiff = imgNowWidth - imgLastWidth;
            this.imgGameTime.Width = imgLastWidth;
            this.imgGameTime.SetValue(Canvas.LeftProperty, imgNowLeft + imgDiff);
        }
        #endregion

        #region 界面按钮事件
        /// <summary>
        /// 展开游戏菜单区域
        /// </summary>
        private void ShowGameMenu()
        {
            //暂停游戏
            this.currentGameTimer.Stop();
            this.gamePausStartTime = DateTime.Now;
            this.currentStatus = GameStatus.Pause;
            
            //显示菜单
            this.gameShadow.Visibility = Visibility.Visible;
            Storyboard storyboard = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.From = Convert.ToDouble(this.gameMenuBox.GetValue(Canvas.LeftProperty));
            da.To = 0d;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, this.gameMenuBox);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(da);
            storyboard.Begin();
        }

        /// <summary>
        /// 隐藏游戏菜单
        /// </summary>
        private void HideGameMenu()
        {
            EventHandler callBack = new EventHandler((sender, e) =>
            {
                //继续游戏
                this.gameShadow.Visibility = Visibility.Collapsed;
                DateTime now = DateTime.Now;
                double ms = now.Subtract(this.gamePausStartTime).TotalMilliseconds;
                this.lastClickTime = this.lastClickTime.AddMilliseconds(ms);
                this.currentGameTimer.Start();
                this.currentStatus = GameStatus.Playing;
            });
            HideGameMenuAnimation(callBack);
        }

        /// <summary>
        /// 隐藏游戏菜单动画
        /// </summary>
        /// <param name="eventhandler">动画完成后处理事件</param>
        private void HideGameMenuAnimation(EventHandler eventhandler)
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0d;
            da.To = -Convert.ToDouble(this.gameMenuBox.GetValue(FrameworkElement.WidthProperty));
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, this.gameMenuBox);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(da);
            storyboard.Completed += eventhandler;
            storyboard.Begin();
        }

        /// <summary>
        /// 重排剩余图标
        /// </summary>
        private void RefreshLastItems()
        {
            this.lastTapItem = null;
            this.currentGameTimer.Stop();
            if (currentGameGridList.Count > 0)
            {
                List<Image> lastItemImages = new List<Image>();
                foreach (UIElement element in this.currentGameMapBox.Children)
                {
                    if (element is Image)
                    {
                        lastItemImages.Add(element as Image);
                    }
                }
                foreach (Image img in lastItemImages)
                {
                    this.currentGameMapBox.Children.Remove(img);
                }

                List<int> randGridList = GameData.GetRandGrids(currentGameGridList.Count, 128);
                this.currentGameGridList = randGridList;
                this.gameGridList[this.currentMapIndex - 1] = this.currentGameGridList;
                int idx = 0;
                foreach (int grid in randGridList)
                {
                    double top = (FindPath.GetGridRow(grid) - 1) * gameItemSize;
                    double left = (FindPath.GetGridColumn(grid) - 1) * gameItemSize;
                    Image tmpImage = lastItemImages[idx];
                    string[] tmpArr = tmpImage.GetValue(FrameworkElement.NameProperty).ToString().Split('_');
                    string newName = string.Format("{0}_{1}_{2}_{3}", tmpArr[0], tmpArr[1], grid, tmpArr[3]);
                    Image newItemImage = new Image { Source = tmpImage.Source, Stretch = Stretch.None, Name = newName };
                    ItemTap itemTap = new ItemTap { ItemName = newName, ItemGrid = grid, ItemResCode = int.Parse(tmpArr[3]), ItemImage = newItemImage };
                    newItemImage.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { GameItemImage_Tap(itemTap); });
                    newItemImage.SetValue(Canvas.LeftProperty, left);
                    newItemImage.SetValue(Canvas.TopProperty, top);
                    this.currentGameMapBox.Children.Add(newItemImage);
                    idx++;
                }
            }
            this.currentGameTimer.Start();
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void PlayAgin()
        {
            if (App.GameAudioEnable)
            {
                StopBackgroundSound();
                StopGameSoundEffect();
            }
            
            if (this.currentStatus == GameStatus.Pause)
                HideGameMenuAnimation(new EventHandler((sender, e) => { GameBegin(); }));
            
            if (this.currentStatus == GameStatus.End)
            {
                this.gameOverBox.Visibility = Visibility.Collapsed;
                GameBegin();
            }
        }
        #endregion

        #region 方块点击事件
        /// <summary>
        /// 方块点击事件
        /// </summary>
        /// <param name="currentTap">当前点击对象</param>
        private void GameItemImage_Tap(ItemTap currentTap)
        {
            #region 无效点击
            if (null == this.lastTapItem)
            {
                this.lastTapItem = currentTap;
                return;
            }

            if (this.lastTapItem.ItemName.Equals(currentTap.ItemName))
            {
                this.comboCount = 0;
                return;
            }

            int lastTapGridIdx = this.lastTapItem.ItemGrid;
            int lastTapItemIdx = this.lastTapItem.ItemResCode;
            int thisTapGridIdx = currentTap.ItemGrid;
            int thisTapItemIdx = currentTap.ItemResCode;
            if (thisTapItemIdx != lastTapItemIdx)
            {
                this.lastTapItem = currentTap;
                this.comboCount = 0;
                return;
            }

            //寻路
            FindPath findPath = new FindPath(this.currentGameGridList);
            List<PathNode> path = findPath.PathFinder(thisTapGridIdx, lastTapGridIdx);
            if (null == path)
            {
                this.lastTapItem = currentTap;
                this.comboCount = 0;
                return;
            }
            #endregion

            #region 有效点击并消除方块
            //有效点击并消除方块
            this.lastClickTime = DateTime.Now;
            Image curTapImg = currentTap.ItemImage;
            Image lastTapImg = this.lastTapItem.ItemImage;
            this.lastTapItem = null;
            this.currentGameGridList.Remove(thisTapGridIdx);
            this.currentGameGridList.Remove(lastTapGridIdx);
            this.gameScore += 2;
            this.comboCount++;

            //绘制连接线
            List<Canvas> linkLineList = new List<Canvas>();
            if (path.Count > 0)
            {
                foreach (PathNode node in path)
                {
                    Canvas cavLinkLine = new Canvas
                    {
                        Background = ResIcon.LinkLines[(int)node.GridDirection].ToImageBrush(),
                        Width = this.gameItemSize,
                        Height = this.gameItemSize
                    };
                    double imgLeft = (FindPath.GetGridColumn(node.GridIndex) - 1) * this.gameItemSize;
                    double imgTop = (FindPath.GetGridRow(node.GridIndex) - 1) * this.gameItemSize;
                    cavLinkLine.SetValue(Canvas.LeftProperty, imgLeft);
                    cavLinkLine.SetValue(Canvas.TopProperty, imgTop);
                    this.currentGameMapBox.Children.Add(cavLinkLine);
                    linkLineList.Add(cavLinkLine);
                }
            }

            //声音
            if (App.GameAudioEnable)
            {
                StopGameSoundEffect();
                this.gameSoundInstance = SoundEffect.FromStream(ResName.Sound_SelSuc.ToAudioDataStream()).CreateInstance();
                this.gameSoundInstance.Volume = Constant.GameSoundVolume;
                this.gameSoundInstance.IsLooped = false;
                this.gameSoundInstance.Play();
            }

            int idx = 0;
            DispatcherTimer dTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            dTimer.Tick += new EventHandler((sender, e) =>
            {
                idx++;
                if (1 == idx)
                {
                    dTimer.Stop();
                    this.currentGameMapBox.Children.Remove(curTapImg);
                    this.currentGameMapBox.Children.Remove(lastTapImg);
                    //消除连接线
                    foreach (Canvas linkLine in linkLineList)
                    {
                        this.currentGameMapBox.Children.Remove(linkLine);
                    }
                    ReDrawGameLastItems();
                    ReDrawGameScore();
                    if (this.comboCount >= 2)
                        ReDrawComboCount();

                    //游戏结束或者换下一张地图
                    if (this.currentGameGridList.Count == 0)
                    {
                        //停止计时
                        this.currentGameTimer.Stop();

                        int tmpIdx = 0;
                        DispatcherTimer tmpTimer = new DispatcherTimer();
                        tmpTimer.Interval = TimeSpan.FromMilliseconds(100);
                        tmpTimer.Tick += new EventHandler((s, v) =>
                        {
                            tmpIdx++;
                            if (1 == tmpIdx)
                            {
                                tmpTimer.Stop();
                                if (this.currentMapIndex >= this.currentGateData.MapCount)
                                    GameOver();
                                else
                                    ChangeMap();
                            }
                        });
                        tmpTimer.Start();
                    }
                }
            });
            dTimer.Start();
            #endregion
        }

        /// <summary>
        /// 重绘连击提示
        /// </summary>
        private void ReDrawComboCount()
        {
            WriteableBitmap wb = (this.comboCount - 1).ToString().ToWriteableBitmap(NumberCharSize.Twentyfour);
            this.imgComboCount.Source = wb;
            wb = null;
            this.imgComboTxt.Visibility = Visibility.Visible;
            this.imgComboCount.Visibility = Visibility.Visible;

            Storyboard storyboard = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.From = 440d;
            da.To = 400d;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime * 5));
            Storyboard.SetTarget(da, this.imgComboTxt);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(da);

            da = new DoubleAnimation();
            da.From = 1d;
            da.To = 0d;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime * 5));
            Storyboard.SetTarget(da, this.imgComboTxt);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Opacity)"));
            storyboard.Children.Add(da);

            da = new DoubleAnimation();
            da.From = 447d;
            da.To = 407d;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime * 5));
            Storyboard.SetTarget(da, this.imgComboCount);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(da);

            da = new DoubleAnimation();
            da.From = 1d;
            da.To = 0d;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime * 5));
            Storyboard.SetTarget(da, this.imgComboCount);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Opacity)"));
            storyboard.Children.Add(da);

            storyboard.Completed += new EventHandler((sender, e) =>
            {
                this.imgComboTxt.SetValue(Canvas.TopProperty, 440d);
                this.imgComboTxt.Visibility = Visibility.Collapsed;
                this.imgComboTxt.SetValue(FrameworkElement.OpacityProperty, 1d);

                this.imgComboCount.SetValue(Canvas.TopProperty, 447d);
                this.imgComboCount.Visibility = Visibility.Collapsed;
                this.imgComboCount.SetValue(FrameworkElement.OpacityProperty, 1d);
            });
            storyboard.Begin();

            this.gameScore++;
            ReDrawGameScore();
        }

        /// <summary>
        /// 更换地图
        /// </summary>
        private void ChangeMap()
        {
            Canvas curMapBox = this.LayoutRoot.FindName(string.Format("Canvas_GameMap_{0}", this.currentMapIndex)) as Canvas;
            this.currentMapIndex++;
            Canvas nextMapBox = this.LayoutRoot.FindName(string.Format("Canvas_GameMap_{0}", this.currentMapIndex)) as Canvas;

            this.currentGameGridList = this.gameGridList[this.currentMapIndex - 1];
            this.currentGameMapBox = nextMapBox;

            Storyboard storyboard = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.From = Convert.ToDouble(curMapBox.GetValue(Canvas.TopProperty));
            da.To = -480d;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, curMapBox);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(da);

            da = new DoubleAnimation();
            da.From = Convert.ToDouble(nextMapBox.GetValue(Canvas.TopProperty));
            da.To = Convert.ToDouble(curMapBox.GetValue(Canvas.TopProperty));
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, nextMapBox);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(da);

            //this.currentGameTimer.Stop();
            storyboard.Completed += new EventHandler((sender, e) =>
            {
                ReDrawGameLastItems();
                this.currentGameTimer.Start();
            });
            storyboard.Begin();
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        private void GameOver()
        {
            this.currentStatus = GameStatus.End;

            //检查是否还有剩余方块没有消除，如果有，则本次成绩无效
            int itemLastCount = this.gameGridList.Sum(list => list.Count);
            //当前成绩+剩余时间
            if (itemLastCount <= 0 && this.gameLastTime > 0)
            {
                this.gameScore += this.gameLastTime;
                //加剩余时间动画
                DispatcherTimer tmpTimer = new DispatcherTimer();
                tmpTimer.Interval = TimeSpan.FromMilliseconds(1);
                tmpTimer.Tick += new EventHandler((s, v) =>
                {
                    tmpTimer.Stop();
                    LastTimeAnimation();
                });
                tmpTimer.Start();
            }
            //更新成绩
            bool isNewHeighest = false;
            int curStar = 0;
            if (itemLastCount <= 0)
            {
                curStar = GameData.GetScoreStar(this.gameScore, this.currentGateData.HighestScore);
                if (this.gameScore > this.currentGateData.Score)
                {
                    isNewHeighest = true;
                    GameData.UpdateGateScore(this.currentLevelId, this.currentGateId, this.gameScore);
                    this.currentGateData = GameData.GetSingleGate(this.currentLevelId, this.currentGateId);
                }
            }

            //显示遮罩
            this.gameShadow.Visibility = Visibility.Visible;
            //游戏结果
            WriteableBitmap wbResult = GetGameResultBg(isNewHeighest, curStar);
            double resultWidth = wbResult.PixelWidth;
            double resultHeight = wbResult.PixelHeight;
            this.gameOverBox = new Canvas
            {
                Width = resultWidth,
                Height = resultHeight,
                Background = wbResult.ToImageBrush()
            };
            wbResult = null;
            this.gameOverBox.SetValue(Canvas.ZIndexProperty, 20);
            this.gameOverBox.SetValue(Canvas.LeftProperty, (double)((Constant.ScreenWidth - resultWidth) / 2));
            this.gameOverBox.SetValue(Canvas.TopProperty, (double)((Constant.ScreenHeight - resultHeight) / 2));
            //按钮
            Image imgPlayAgin = new Image { Source = ResButton.RePlaySmall, Stretch = Stretch.None };
            imgPlayAgin.SetValue(Canvas.LeftProperty, 56d);
            imgPlayAgin.SetValue(Canvas.TopProperty, 265d);
            imgPlayAgin.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { PlayAgin(); });
            this.gameOverBox.Children.Add(imgPlayAgin);

            Image imgGoBack = new Image { Source = ResButton.GobackSmall, Stretch = Stretch.None };
            imgGoBack.SetValue(Canvas.LeftProperty, 176d);
            imgGoBack.SetValue(Canvas.TopProperty, 265d);
            imgGoBack.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) => { /*NavigationService.GoBack();*/Goback(); });
            this.gameOverBox.Children.Add(imgGoBack);

            if (curStar >= 1)
            {
                int[] nextIds = GameData.GetNextLevelAndGateId(this.currentLevelId, this.currentGateId);
                int nextLevelId = nextIds[0], nextGateId = nextIds[1];
                if (nextLevelId > 0 && nextGateId > 0)
                {
                    Image imgNext = new Image { Source = ResButton.Next, Stretch = Stretch.None };
                    imgNext.SetValue(Canvas.LeftProperty, 296d);
                    imgNext.SetValue(Canvas.TopProperty, 265d);
                    imgNext.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
                    {
                        this.currentLevelId = nextLevelId;
                        this.currentGateId = nextGateId;
                        LoadGameData();
                    });
                    this.gameOverBox.Children.Add(imgNext);
                }            
            }
            //停止背景声音，播放游戏结束音效
            if (App.GameAudioEnable)
            {
                StopBackgroundSound();
                StopGameSoundEffect();
                this.gameSoundInstance = SoundEffect.FromStream(this.endSoundNames[curStar].ToAudioDataStream()).CreateInstance();
                this.gameSoundInstance.Volume = Constant.GameSoundVolume;
                this.gameSoundInstance.IsLooped = false;
                this.gameSoundInstance.Play();
            }
            //将游戏结果弹出层添加至上级容器
            this.LayoutRoot.Children.Add(this.gameOverBox);

            //奖励图片
            if (this.currentLevelId >= 4 && curStar >= 3)
            {
                ResInfo rewardImage = ResImage.GetRandRewardImage();
                Canvas cavReward = new Canvas
                {
                    Background = rewardImage.Data.ToImageBrush(),
                    Width = Constant.ScreenWidth,
                    Height = Constant.ScreenHeight
                };
                cavReward.SetValue(Canvas.ZIndexProperty, 30);
                cavReward.SetValue(Canvas.LeftProperty, 0d);
                cavReward.SetValue(Canvas.TopProperty, 0d);
                //保存按钮
                Image imgSave = new Image { Source = ResButton.Save, Stretch = Stretch.None };
                imgSave.SetValue(Canvas.LeftProperty, 672d);
                imgSave.SetValue(Canvas.TopProperty, 416d);
                imgSave.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
                {
                    string imageName = rewardImage.Name;
                    WriteableBitmap wbImg = new WriteableBitmap(rewardImage.Data);
                    var gameStore = IsolatedStorageFile.GetUserStoreForApplication();
                    if (gameStore.FileExists(imageName))
                        gameStore.DeleteFile(imageName);

                    IsolatedStorageFileStream imgFileStream = gameStore.CreateFile(imageName);
                    wbImg.SaveJpeg(imgFileStream, rewardImage.Data.PixelWidth, rewardImage.Data.PixelHeight, 0, 85);
                    imgFileStream.Close();

                    imgFileStream = gameStore.OpenFile(imageName, FileMode.Open, FileAccess.Read);
                    using (MediaLibrary library = new MediaLibrary())
                    {
                        Picture pic = library.SavePicture(imageName, imgFileStream);
                    }
                    imgFileStream.Close();

                    this.LayoutRoot.Children.Remove(cavReward);
                });
                cavReward.Children.Add(imgSave);
                //关闭按钮
                Image imgClose = new Image { Source = ResButton.Close, Stretch = Stretch.None };
                imgClose.SetValue(Canvas.LeftProperty, 736d);
                imgClose.SetValue(Canvas.TopProperty, 416d);
                imgClose.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
                {
                    this.LayoutRoot.Children.Remove(cavReward);
                });
                cavReward.Children.Add(imgClose);

                //添加至上级容器
                this.LayoutRoot.Children.Add(cavReward);
            }
        }

        /// <summary>
        /// 合成游戏结束时的弹出信息背景图片
        /// </summary>
        /// <param name="isNewHeighest">得分是否新记录</param>
        /// <param name="curStar">本次得分星级</param>
        /// <returns>游戏结束时的弹出信息背景图片</returns>
        private WriteableBitmap GetGameResultBg(bool isNewHeighest, int curStar)
        {
            List<CombineSource> resultSourceList = new List<CombineSource>();
            //背景
            BitmapImage bmpBg = ResIcon.GameOver_Bg;
            resultSourceList.Add(new CombineSource { Source = bmpBg, Position = new System.Windows.Point { X = 0d, Y = 0d } });
            //关卡序号
            WriteableBitmap wbGateIndex = string.Format("{0}-{1}", this.currentLevelId, this.currentGateId).ToWriteableBitmap(NumberCharSize.Fortyeight);
            resultSourceList.Add(new CombineSource { Source = wbGateIndex, Position = new System.Windows.Point { X = 80d, Y = 20d } });
            wbGateIndex = null;
            //历史最高纪录文字
            BitmapImage bmpHisTxt = ResTxt.Highest;
            resultSourceList.Add(new CombineSource { Source = bmpHisTxt, Position = new System.Windows.Point { X = 240d, Y = 9d } });
            //历史最高得分
            WriteableBitmap wbHisScore = this.currentGateData.Score.ToString().ToWriteableBitmap(NumberCharSize.Twentyfour);
            resultSourceList.Add(new CombineSource { Source = wbHisScore, Position = new System.Windows.Point { X = 240d, Y = 39d } });
            wbHisScore = null;
            //历史最高星级
            WriteableBitmap wbStar = this.currentGateData.Star.ToStarBitmap();
            resultSourceList.Add(new CombineSource { Source = wbStar, Position = new System.Windows.Point { X = 240d, Y = 65d } });
            wbStar = null;
            //本次闯关成功与失败
            BitmapImage bmpSF = curStar >= 1 ? ResTxt.Win : ResTxt.Fail;
            resultSourceList.Add(new CombineSource { Source = bmpSF, Position = new System.Windows.Point { X = 25d, Y = 110d } });
            //本次成绩
            resultSourceList.Add(new CombineSource { Source = ResTxt.Grade, Position = new System.Windows.Point { X = 28d, Y = 156d } });
            WriteableBitmap wbScore = this.gameScore.ToString().ToWriteableBitmap(NumberCharSize.Twentyfour);
            resultSourceList.Add(new CombineSource { Source = wbScore, Position = new System.Windows.Point { X = 30d, Y = 196d } });
            wbScore = null;
            WriteableBitmap wbCurStar = curStar.ToStarBitmap();
            resultSourceList.Add(new CombineSource { Source = wbCurStar, Position = new System.Windows.Point { X = 32d, Y = 225d } });
            wbCurStar = null;
            //是否新记录
            if (isNewHeighest)
                resultSourceList.Add(new CombineSource { Source = ResIcon.NewHeight, Position = new System.Windows.Point { X = 300d, Y = 120d } });

            return resultSourceList.CombineImages((int)bmpBg.PixelWidth, (int)bmpBg.PixelHeight);
        }

        /// <summary>
        /// 加剩余时间动画
        /// </summary>
        private void LastTimeAnimation()
        {
            Image imgAddLastTime = new Image { Source = ResTxt.AddLastTime, Stretch = Stretch.None };
            imgAddLastTime.SetValue(Canvas.LeftProperty, 590d);
            imgAddLastTime.SetValue(Canvas.TopProperty, 370d);
            this.LayoutRoot.Children.Add(imgAddLastTime);

            WriteableBitmap wbLastTime = this.gameLastTime.ToString().ToWriteableBitmap(NumberCharSize.Twentyfour);
            Image imgLastTime = new Image { Source = wbLastTime, Stretch = Stretch.None };
            imgLastTime.SetValue(Canvas.LeftProperty, 730d);
            imgLastTime.SetValue(Canvas.TopProperty, 377d);
            this.LayoutRoot.Children.Add(imgLastTime);
            wbLastTime = null;

            //重绘成绩
            ReDrawGameScore();

            //加剩余时间提示文字渐隐动画
            Storyboard storyboard = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.From = 1d;
            da.To = 0d;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime * 5));
            Storyboard.SetTarget(da, imgAddLastTime);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Opacity)"));
            storyboard.Children.Add(da);

            da = new DoubleAnimation();
            da.From = 1d;
            da.To = 0d;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime * 5));
            Storyboard.SetTarget(da, imgLastTime);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Opacity)"));
            storyboard.Children.Add(da);

            storyboard.Completed += new EventHandler((sender, e) =>
            {
                this.LayoutRoot.Children.Remove(imgAddLastTime);
                this.LayoutRoot.Children.Remove(imgLastTime);
            });
            storyboard.Begin();
        }
        #endregion
    }
}