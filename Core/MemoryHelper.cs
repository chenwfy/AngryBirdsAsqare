using System;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Phone.Info;

namespace SLGame.AngryBirdsAsqare.Core
{
    /// <summary>
    /// 应用内存测试辅助类
    /// </summary>
    public class MemoryHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private static DispatcherTimer timer;

        /// <summary>
        /// 显示应用内存使用日志
        /// </summary>
        public static void AppMemoryUsageLog()
        {
            string memLog = string.Format("设备总内存：{0}；\n", GetMB(DeviceStatus.DeviceTotalMemory));
            memLog += string.Format("设备内存使用限制：{0}；\n", GetMB(DeviceStatus.ApplicationMemoryUsageLimit));
            memLog += "=========================================\n";
            System.Diagnostics.Debug.WriteLine(memLog);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler((sender, e) =>
            {
                string log = "=========================================\n";
                log += string.Format("应用占用内存：{0}；\n", GetMB(DeviceStatus.ApplicationCurrentMemoryUsage));
                log += string.Format("应用占用内存峰值：{0}；\n", GetMB(DeviceStatus.ApplicationPeakMemoryUsage));
                System.Diagnostics.Debug.WriteLine(log);
            });
            timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string GetMB(long bytes)
        {
            double toMB = (double)(1024 * 1024);
            return ((double)((double)bytes / toMB)).ToString("0.00") + "MB";
        }
    }
}
