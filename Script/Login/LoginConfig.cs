//******************************************************************
// File Name:					LoginConfig.cs
// Description:					LoginConfig class 
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

using System.IO;

namespace FW.Login
{
    static class LoginConfig
    {
        private static string sm_userKey;
        private static string sm_username;              //用户名
        private static string sm_userpwd;               //用户密码
        private static bool sm_autoLogin;               //是否自动登陆
        private static string sm_ipAdress;
        private static string sm_luckyStr;              //中奖记录的字符串

        static LoginConfig()
        {
            sm_username = "";                           //用户名
            sm_userpwd = "";                            //用户密码
            sm_autoLogin = false;                       //是否自动登陆
            sm_userKey = "abcdefgh";
            sm_ipAdress = "192.168.8.231";
            sm_luckyStr = "";
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public static string UserName { get { return sm_username; } }

        public static string UserPwd { get { return sm_userpwd; } }

        public static bool IsAutoLogin { get { return sm_autoLogin; } }

        public static string IpAdr { get { return sm_ipAdress; } }

        //中奖记录放在这里
        public static string LuckyStr { get { return sm_luckyStr; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private static string GetConfigName()
        {
            return ResMgr.ResPaths.GetDatasRoot() + Utility.Utility.MD5(ResMgr.ResPaths.UserFile);
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        //导放配置文件
        public static void LoadFile()
        {
            string fileName = GetConfigName();
            FileInfo info = new FileInfo(fileName);
            if(info.Exists == false)
            {
                Debug.LogFormat(fileName+" doesn't exist");
                return;
            }
            StreamReader sr = info.OpenText();
            string buf = sr.ReadLine();
            if(buf != string.Empty)
                sm_username = Utility.Utility.DecryptString(buf, sm_userKey);
            buf = sr.ReadLine();
            if (buf != string.Empty)
                sm_userpwd = Utility.Utility.DecryptString(buf, sm_userKey);
            buf = sr.ReadLine();
            if (buf != string.Empty)
                sm_autoLogin = Convert.ToBoolean(Utility.Utility.DecryptString(buf, sm_userKey));
            buf = sr.ReadLine();
            if (buf != string.Empty)
                sm_ipAdress = Utility.Utility.DecryptString(buf, sm_userKey);
            buf = sr.ReadLine();
            if (buf != string.Empty)
                sm_luckyStr = Utility.Utility.DecryptString(buf, sm_userKey);
            sr.Close();
            Debug.LogFormat("LoadFile " + fileName + " " + sm_username + " " + sm_userpwd + " " + sm_autoLogin);
        }

        //写入配置文件
        public static void SaveFile()
        {
            string fileName = GetConfigName();
            FileInfo info = new FileInfo(fileName);
            StreamWriter sw = new StreamWriter(info.Create());
            sw.WriteLine(Utility.Utility.EncryptString(sm_username, sm_userKey));
            sw.WriteLine(Utility.Utility.EncryptString(sm_userpwd, sm_userKey));
            sw.WriteLine(Utility.Utility.EncryptString(sm_autoLogin.ToString(), sm_userKey));
            sw.WriteLine(Utility.Utility.EncryptString(sm_ipAdress, sm_userKey));
            sw.WriteLine(Utility.Utility.EncryptString(sm_luckyStr, sm_userKey));
            sw.Close();
            Debug.LogFormat("SaveFile " + fileName + " " + sm_username + " " + sm_userpwd + " " + sm_autoLogin+"  "+sm_ipAdress);
        }

        //设置用户名密码
        public static void SetUserNameAndPwd(string name, string pwd)
        {
            sm_username = name;
            sm_userpwd = pwd;
        }
        
        //设置是否自动登陆
        public static void SetUserAutoLogin(bool autoLogin)
        {
            sm_autoLogin = autoLogin;
        }

        //设置登陆ip
        public static void SetLoginIPAdr(string ipAdr)
        {
            sm_ipAdress = ipAdr;
        }

        //设置中奖记录的字符串
        public static void SetLuckyStt(string luckyStr)
        {
            sm_luckyStr = luckyStr;
        }
    }
}