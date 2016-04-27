using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 隐藏关卡奖励图片资源数据辅助类
    /// </summary>
    public static class ResImage
    {
        /// <summary>
        /// 资源数据字典
        /// </summary>
        private static List<RewardImage> imageResList = new List<RewardImage>();

        /// <summary>
        /// 读取资源
        /// </summary>
        public static void ReadData()
        {
            imageResList = ResImageToList();
        }
        
        /// <summary>
        /// 随机获取一张奖励图片
        /// </summary>
        /// <returns>一张奖励图片</returns>
        public static ResInfo GetRandRewardImage()
        {
            Random rand = new Random();
            int idx = rand.Next(0, imageResList.Count);
            return GetRandRewardImage(imageResList[idx]);
        }

        #region 私有方法
        /// <summary>
        /// 读取奖励图片文件名称以及数据偏移位置数据集合
        /// </summary>
        /// <returns>奖励图片文件名称以及数据偏移位置数据集合</returns>
        private static List<RewardImage> ResImageToList()
        {
            List<RewardImage> imageList = new List<RewardImage>();
            StreamResourceInfo resInfo = Application.GetResourceStream(ResName.ImageData.ToResUri());
            using (Stream stream = resInfo.Stream)
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int resFlag = reader.ReadInt32();
                    short resCount = reader.ReadInt16();
                    for (int r = 1; r <= resCount; r++)
                    {
                        short resFileNameLen = reader.ReadInt16();
                        byte[] resFileNameBytes = reader.ReadBytes((int)resFileNameLen);
                        string resFileName = new System.Text.UnicodeEncoding().GetString(resFileNameBytes, 0, resFileNameLen);
                        int resPosotion = reader.ReadInt32();
                        imageList.Add(new RewardImage { Name = resFileName, Position = (long)resPosotion });
                    }
                    reader.Close();
                }
            }
            return imageList;
        }

        /// <summary>
        /// 随机读取一张奖励图片数据
        /// </summary>
        /// <param name="rewardImage">数据偏移位置信息</param>
        /// <returns>奖励图片数据</returns>
        private static ResInfo GetRandRewardImage(RewardImage rewardImage)
        {
            ResInfo imgResInfo = new ResInfo{ Name = rewardImage.Name};
            StreamResourceInfo resInfo = Application.GetResourceStream(ResName.ImageData.ToResUri());
            using (Stream stream = resInfo.Stream)
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.ReadBytes((int)rewardImage.Position);
                    int resFileDataLen = reader.ReadInt32();
                    byte[] resFileDataBytes = reader.ReadBytes(resFileDataLen);
                    imgResInfo.Data = resFileDataBytes.ToBitmapImage();
                    reader.Close();
                }
            }
            return imgResInfo;
        }
        #endregion
    }
}

