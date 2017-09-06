//******************************************************************
// File Name:					ResPaths.cs
// Description:					ResPaths class 
// Author:						wuwei
// Date:						2017.01.04
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FW.ResMgr
{
    static class ResPaths
    {

        //用户存储文件
        public static string UserFile { get { return "userConfig"; } }

        //获取配置文件目录
        public static string GetDatasRoot()
        {
#if UNITY_EDITOR
            string path = System.IO.Directory.GetCurrentDirectory();
            path = path.Replace("\\", "/").Replace("/src/client", "/game/");
            return path;
            //#elif UNITY_STANDALONE_WIN
            //            string path = System.IO.Directory.GetCurrentDirectory();
            //            path = path.Replace("\\", "/").Replace("/game/Windows", "/src/");
            //            return path;
#else
            return Application.persistentDataPath+"/";
#endif
        }

        //获取地图路径
        public static string GetMapPath(string name)
        {
            return "res/map/" + name + "/";
        }

        //获取地图配置文件
        public static string GetMapConfigName(string name)
        {
            return GetMapPath(name) + "config.json";
        }

        //获取普通json配置文件的路径
        public static string GetNormalConfigPath(string name)
        {
            if(name.IndexOf(".")>=0)
                return "config/" + name;
            else
                return "config/" + name+".json";
        }
    }
}