using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLGame.AngryBirdsAsqare.Resource
{
    /// <summary>
    /// 游戏相关资源数据读取类
    /// </summary>
    public static class ResLoad
    {
        /// <summary>
        /// 读取资源
        /// </summary>
        public static void ReadData()
        {
            ResScreen.ReadData();
            ResIcon.ReadData();
            ResButton.ReadData();
            ResTxt.ReadData();
            ResItem.ReadData();
            ResImage.ReadData();
            MapLoad.Load();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        public static void Load()
        {
            ResScreen.Load();
            ResIcon.Load();
            ResButton.Load();
            ResTxt.Load();
            ResItem.Load();
        }

        /// <summary>
        /// 获取LOADING图片数据
        /// </summary>
        /// <returns></returns>
        public static ImageBrush LoadingImage()
        {
            return ResName.LoadingImg.ToResUri().ToImageBrush();
        }

        /// <summary>
        /// 读取游戏页面相关图片资源数据
        /// </summary>
        /// <param name="resDataFileName">图片资源数据文件名</param>
        /// <returns>图片资源数据字典</returns>
        public static Dictionary<string, byte[]> LoadToDictBytes(this string resDataFileName)
        {
            Dictionary<string, byte[]> resDict = new Dictionary<string, byte[]>();
            StreamResourceInfo resInfo = Application.GetResourceStream(resDataFileName.ToResUri());
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
                        int resFileDataLen = reader.ReadInt32();
                        byte[] resFileDataBytes = reader.ReadBytes(resFileDataLen);

                        resDict.Add(resFileName, resFileDataBytes);
                    }
                    reader.Close();
                }
            }
            return resDict;
        }

        /// <summary>
        /// 获取声音文件数据流
        /// </summary>
        /// <param name="audioFileName">声音文件名</param>
        /// <returns>声音文件数据流</returns>
        public static Stream ToAudioDataStream(this string audioFileName)
        {
            return audioFileName.ToAudioUri().ToAudioDataStream();
        }

        /// <summary>
        /// 获取声音文件数据流
        /// </summary>
        /// <param name="audioFileUri">声音文件URI</param>
        /// <returns>声音文件数据流</returns>
        public static Stream ToAudioDataStream(this Uri audioFileUri)
        {
            StreamResourceInfo resInfo = Application.GetResourceStream(audioFileUri);
            return resInfo.Stream;
        }
    }
}
