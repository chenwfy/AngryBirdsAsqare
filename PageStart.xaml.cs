using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SLGame.AngryBirdsAsqare.Resource;
using SLGame.AngryBirdsAsqare.Core;

namespace SLGame.AngryBirdsAsqare.View
{
    /// <summary>
    /// 游戏开始页面类
    /// </summary>
    public partial class PageStart : PhoneApplicationPage
    {
        /// <summary>
        /// 当前页面类实例
        /// </summary>
        public static PageStart Instance { get; private set; }
        /// <summary>
        /// 设置弹出层是否展开
        /// </summary>
        private bool showShadow = false;
        /// <summary>
        /// 设置按钮前景旋转对象
        /// </summary>
        private RotateTransform rotateTransform;
        /// <summary>
        /// 弹出层容器对象
        /// </summary>
        private Canvas canvasShadow;
        /// <summary>
        /// 弹出层半透明背景图
        /// </summary>
        private BitmapImage bmpShadow;
        /// <summary>
        /// 声音开关按钮对象
        /// </summary>
        private Image imgAudioSet;
        /// <summary>
        /// 弹出层距离屏幕底部高度
        /// </summary>
        private double canvasShadowMarginBottom;
        /// <summary>
        /// 当前界面背景声音实例
        /// </summary>
        public SoundEffectInstance BgSoundInstance { get; private set; } 

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageStart()
        {
            InitializeComponent();
            Instance = this;
            this.BgSoundInstance = null;
            //LOADING界面
            Loading();
        }

        /// <summary>
        /// 更改声音开关按钮背景图片
        /// </summary>
        public void ChanggeAudioImage()
        {
            BitmapImage bmpAudioEnable = ResButton.AudioSmall;
            BitmapImage bmpAudioDisenable = ResButton.DisenableSmall;
            BitmapImage imgAudio = App.GameAudioEnable ? bmpAudioEnable : bmpAudioDisenable;
            this.imgAudioSet.Source = imgAudio;
        }

        /// <summary>
        /// 冲洗返回键按键事件，退出前先关闭声音
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (App.GameAudioEnable && this.BgSoundInstance != null)
            {
                this.BgSoundInstance.Stop();
                this.BgSoundInstance.Dispose();
            }
            base.OnBackKeyPress(e);
        }

        /// <summary>
        /// LOADING界面
        /// </summary>
        private void Loading()
        {
            this.LayoutRoot.Background = ResLoad.LoadingImage();

            //加载资源和初始化数据
            BackgroundWorker backWorker = new BackgroundWorker();
            backWorker.DoWork += new DoWorkEventHandler((sender, e) =>
            {
                ResLoad.ReadData();
                GameData.LoadGameData();
            });
            backWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((sender, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    ResLoad.Load();
                    InitStartPage();
                });
            });
            backWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 游戏开始界面
        /// </summary>
        private void InitStartPage()
        {
            this.LayoutRoot.Background = ResScreen.StartBg.ToImageBrush();
            
            //构建XNA模式的模拟环境
            GameTimer gameTimer = new GameTimer();
            gameTimer.UpdateInterval = TimeSpan.FromMilliseconds(33);
            gameTimer.Update += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            gameTimer.Start();
            FrameworkDispatcher.Update();

            //2012-05-22修改
            PlayBackgroundSound();
            //界面布局
            DrawLogoAndStart();
            DrawGameConfig();
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        private void PlayBackgroundSound()
        {
            this.BgSoundInstance = SoundEffect.FromStream(ResName.Music_PageBg.ToAudioDataStream()).CreateInstance();
            this.BgSoundInstance.Volume = Constant.PageSoundVolume;
            this.BgSoundInstance.IsLooped = true;
            if (App.GameAudioEnable)
                this.BgSoundInstance.Play();
        }

        /// <summary>
        /// 绘制LOGO和开始按钮
        /// </summary>
        private void DrawLogoAndStart()
        {
            BitmapImage bmpLogo = ResIcon.Logo;
            double logoWidth = bmpLogo.PixelWidth;
            double logoHeight = bmpLogo.PixelHeight;
            Image imgLogo = new Image { Source = bmpLogo, Stretch = Stretch.None };
            imgLogo.SetValue(Canvas.TopProperty, 10d);
            imgLogo.SetValue(Canvas.LeftProperty, Convert.ToDouble((Constant.ScreenWidth - logoWidth) / 2));
            this.LayoutRoot.Children.Add(imgLogo);

            BitmapImage bmpStart = ResButton.Start;
            double startWidth = bmpStart.PixelWidth;
            Image imgStart = new Image { Source = bmpStart, Stretch = Stretch.None };
            imgStart.SetValue(Canvas.TopProperty, logoHeight);
            imgStart.SetValue(Canvas.LeftProperty, Convert.ToDouble((Constant.ScreenWidth - startWidth) / 2));
            imgStart.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
            {
                NavigationService.Navigate(GamePage.PageLevel.ToPageUri());
            });
            this.LayoutRoot.Children.Add(imgStart);
        }

        /// <summary>
        /// 绘制游戏设置按钮
        /// </summary>
        private void DrawGameConfig()
        {
            #region 设置按钮
            //设置按钮背景
            BitmapImage bmpSetBack = ResButton.SetBg;
            double setBackWidth = bmpSetBack.PixelWidth;
            double setBackHeight = bmpSetBack.PixelHeight;
            double setLeft = 5;
            double setTop = Constant.ScreenHeight - setBackHeight - 5;

            Canvas setCanvas = new Canvas { Width = setBackWidth, Height = setBackHeight, Background = bmpSetBack.ToImageBrush() };
            setCanvas.SetValue(Canvas.LeftProperty, setLeft);
            setCanvas.SetValue(Canvas.TopProperty, setTop);
            setCanvas.SetValue(Canvas.ZIndexProperty, 2);

            //设置按钮前景
            BitmapImage bmpSetFore = ResButton.SetFore;
            double setForeWidth = bmpSetFore.PixelWidth;
            double setForeHeight = bmpSetFore.PixelHeight;

            this.rotateTransform = new RotateTransform();
            this.rotateTransform.CenterX = setForeWidth / 2;
            this.rotateTransform.CenterY = setForeHeight / 2;
            
            double setForeLeft = (setBackWidth - setForeWidth) / 2;
            double serForeTop =(setBackHeight - setForeHeight) / 2;
            Image imgSetFore = new Image { Source = bmpSetFore, Stretch = Stretch.None, RenderTransform = this.rotateTransform };
            imgSetFore.SetValue(Canvas.TopProperty, serForeTop);
            imgSetFore.SetValue(Canvas.LeftProperty, setForeLeft);
            imgSetFore.MouseLeftButtonDown += new MouseButtonEventHandler(GameConfig_Tap);
            setCanvas.Children.Add(imgSetFore);

            //将控件加入到上级容器中
            this.LayoutRoot.Children.Add(setCanvas);
            #endregion

            #region 弹出设置项
            //设置选项弹出层部分控件对象
            this.canvasShadow = new Canvas();
            this.bmpShadow = ResIcon.Shadow;
            this.canvasShadowMarginBottom = setTop + setBackHeight / 2;
            
            this.canvasShadow.Background = this.bmpShadow.ToImageBrush();
            this.canvasShadow.Width = this.bmpShadow.PixelWidth;
            this.canvasShadow.Height = 0d;
            this.canvasShadow.SetValue(Canvas.LeftProperty, setLeft);
            this.canvasShadow.SetValue(Canvas.TopProperty, this.canvasShadowMarginBottom);

            BitmapImage bmpAudioEnable = ResButton.AudioSmall;
            BitmapImage bmpAudioDisenable = ResButton.DisenableSmall;
            BitmapImage imgAudio = App.GameAudioEnable ? bmpAudioEnable : bmpAudioDisenable;
            this.imgAudioSet = new Image { Source = imgAudio, Stretch = Stretch.None };
            this.imgAudioSet.SetValue(Canvas.TopProperty, 10d);
            this.imgAudioSet.SetValue(Canvas.LeftProperty, (double)((this.bmpShadow.PixelWidth - imgAudio.PixelWidth) / 2));
            this.imgAudioSet.SetValue(FrameworkElement.OpacityProperty, 0d);
            this.imgAudioSet.MouseLeftButtonDown += new MouseButtonEventHandler((sender, e) =>
            {
                if (App.GameAudioEnable)
                {
                    this.imgAudioSet.Source = bmpAudioDisenable;
                    GameData.UpdateGameAudioSet(false);
                    App.GameAudioEnable = false;
                    PauseBackgroundSound();
                }
                else
                {
                    this.imgAudioSet.Source = bmpAudioEnable;
                    GameData.UpdateGameAudioSet(true);
                    App.GameAudioEnable = true;
                    ResumeBackgroundSound();
                }
            });
            this.canvasShadow.Children.Add(this.imgAudioSet);

            //将控件添加到父级控件中
            this.LayoutRoot.Children.Add(this.canvasShadow);
            #endregion
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        private void PauseBackgroundSound()
        {
            if (this.BgSoundInstance != null && this.BgSoundInstance.State == SoundState.Playing)
                this.BgSoundInstance.Pause();
        }

        /// <summary>
        /// 恢复背景音乐
        /// </summary>
        private void ResumeBackgroundSound()
        {
            if (this.BgSoundInstance != null && this.BgSoundInstance.State != SoundState.Playing)
                this.BgSoundInstance.Resume();
        }

        /// <summary>
        /// 设置按钮单点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameConfig_Tap(object sender, MouseButtonEventArgs e)
        {
            double shadowHeight = this.bmpShadow.PixelHeight;
            double shadowHeightFrom = 0, shadowHeightTo = 0, shadowTopFrom = 0, shadowTopTo = 0;
            double audioOpacityFrom = 0, audioOpacityTo = 0, setAngleFrom = 0, setAngleTo = 0;
            bool shadowShow = this.showShadow;
            if (!this.showShadow)
            {
                shadowHeightFrom = 0.0;
                shadowHeightTo = shadowHeight;
                shadowTopFrom = this.canvasShadowMarginBottom;
                shadowTopTo = this.canvasShadowMarginBottom - shadowHeight;
                audioOpacityFrom = 0.0;
                audioOpacityTo = 1.0;
                setAngleFrom = 0.0;
                setAngleTo = 90.0;
                shadowShow = true;
            }
            else
            {
                shadowHeightFrom = shadowHeight;
                shadowHeightTo = 0.0;
                shadowTopFrom = this.canvasShadowMarginBottom - shadowHeight;
                shadowTopTo = this.canvasShadowMarginBottom;
                audioOpacityFrom = 1.0;
                audioOpacityTo = 0.0;
                setAngleFrom = 90.0;
                setAngleTo = 0.0;
                shadowShow = false;
            }

            Storyboard storyboard = new Storyboard();
            //遮罩层高度变化
            DoubleAnimation da = new DoubleAnimation();
            da.From = shadowHeightFrom;
            da.To = shadowHeightTo;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, this.canvasShadow);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Height)"));
            storyboard.Children.Add(da);
            //遮罩层位置变化
            da = new DoubleAnimation();
            da.From = shadowTopFrom;
            da.To = shadowTopTo;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, this.canvasShadow);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(da);
            //声音按钮显示
            da = new DoubleAnimation();
            da.From = audioOpacityFrom;
            da.To = audioOpacityTo;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, this.imgAudioSet);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Opacity)"));
            storyboard.Children.Add(da);
            //设置按钮前景图片旋转
            da = new DoubleAnimation();
            da.From = setAngleFrom;
            da.To = setAngleTo;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(Constant.AnimationTime));
            Storyboard.SetTarget(da, this.rotateTransform);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Angle)"));
            storyboard.Children.Add(da);

            storyboard.Begin();
            this.showShadow = shadowShow;
        }
    }
}