//******************************************************************
// File Name:					DebugLog.cs
// Description:					DebugLog class 
// Author:						wuwei
// Date:						2017.02.13
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using GJson;

namespace FW.Utility.DebugEx
{
    static class DebugLog
    {
        private static bool sm_inited;
        private static string sm_outPath;
        
        static DebugLog()
        {
            sm_inited = false;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            sm_outPath = System.IO.Directory.GetCurrentDirectory() + "/debug.log";
#elif UNITY_ANDROID || UNITY_IPHONE
            sm_outPath = Application.persistentDataPath + "/debug.log";
#endif
        }

        //写入文件头
        private static void WriteLogHead()
        {
            try
            {
                StreamWriter writer = new StreamWriter(sm_outPath, false, Encoding.UTF8);
                //运行时间
                writer.WriteLine("start time:{0}", DateTime.Now.ToString());
                //设备名称，及类型
                writer.WriteLine("{0},{1}", SystemInfo.deviceName, SystemInfo.deviceType);
                //操作系统，内存容量
                writer.WriteLine("{0},{1}", SystemInfo.operatingSystem, SystemInfo.systemMemorySize);
                //渲染设备及版本，显存信息
                writer.WriteLine("{0},{1},{2}", SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVersion, SystemInfo.graphicsMemorySize);
                writer.Close();
            }
            catch { }
        }

        private static void WriteLogMsg(string logstr)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sm_outPath, true, Encoding.UTF8);
                writer.WriteLine(logstr);
                writer.Close();
            }
            catch { }
        }

        //处理监听到的Log
        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            WriteLogMsg(logString);
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
            {
                WriteLogMsg(stackTrace);
            }
        }

        public static void Open()
        {
            if (sm_inited) return;
            sm_inited = true;
            WriteLogHead();
            Application.logMessageReceived += HandleLog;
        }

        public static void Close()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}