//******************************************************************
// File Name:					Utility.cs
// Description:					Utility class 
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

using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FW.Utility
{
    static class Utility
    {
        //des加密
        public static string EncryptString(byte[] buff, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = UTF8Encoding.UTF8.GetBytes(key);
            des.IV = UTF8Encoding.UTF8.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(buff, 0, buff.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        public static string EncryptString(string buff, string key)
        {
            byte[] inputBuff = Encoding.UTF8.GetBytes(buff);
            return EncryptString(inputBuff, key);
        }

        //des解密
        public static string DecryptString(byte[] buff, string key)
        {
            string str = Encoding.UTF8.GetString(buff);
            return DecryptString(str, key);
        }

        public static string DecryptString(string buff, string key)
        {
            if (buff == null || buff == string.Empty) return string.Empty;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[buff.Length / 2];
            for (int x = 0; x < buff.Length / 2; x++)
            {
                int i = (Convert.ToInt32(buff.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = UTF8Encoding.UTF8.GetBytes(key);
            des.IV = UTF8Encoding.UTF8.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //StringBuilder ret = new StringBuilder();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        //获取文件MD5
        public static string MD5(FileInfo file)
        {
            if (file != null && file.Exists)
            {
                using (FileStream stream = file.OpenRead())
                {
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
            return string.Empty;
        }

        //取字符串MD5
        public static string MD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        //判断数组中是否有某个元素
        public static bool HasItem<T>(T[] ts, T t)
        {
            return Array.IndexOf<T>(ts, t) != -1;
        }

        //获取GameObject上的UIEventListener组件
        public static UIEventListener GetUIEventListener(Transform t)
        {
            return GetUIEventListener(t.gameObject);
        }

        public static UIEventListener GetUIEventListener(GameObject go)
        {
            if (go.GetComponent<UIEventListener>() == null)
                return go.AddComponent<UIEventListener>();
            return go.GetComponent<UIEventListener>();
        }

        //根据文件路径加载预制体
        public static GameObject GetPrefabGameObject(string path,string returnGoName,Transform parent)
        {
            GameObject go = UnityEngine.Object.Instantiate
                        (ResMgr.ResLoad.Load(path) as GameObject);
            go.name = returnGoName;
            go.transform.parent = parent;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            return go;
        }

        //秒转时间字符串
        public static string Int2DateAndSecond(int second,string format = "yyyy年MM月dd日-HH:mm:ss")
        {
            DateTime s = new DateTime(1970,1,1);
            s = s.AddSeconds(second+8*3600);
            return s.ToString(format);
        }

        public static string Int2DateAndSecondNOtime(int second)
        {
            return Int2DateAndSecond(second, "yyyy年MM月dd日");
        }

        //根据给的秒 返回倒计时的str
        public static string GetTimeString(int Seconds)
        {
            long hour = Seconds / (60 * 60);
            long min = (Seconds - hour * (60 * 60)) / 60;
            long sec = (Seconds - hour * (60 * 60) - min * 60);
            string hStr = hour.ToString();
            if (hour < 10)
                hStr = "0" + hStr;
            string mStr = min.ToString();
            if (min < 10)
                mStr = "0" + mStr;
            string sStr = sec.ToString();
            if (sec < 10)
                sStr = "0" + sStr;
            string timeStr = hStr + ":" + mStr + ":" + sStr;
            return timeStr;
        }

        //给定结束时间返回倒计时的str
        public static string GetNowTimetoTargetTime(int targetTime)
        {
            DateTime nowTimes = DateTime.UtcNow;
            DateTime targetTimes = new DateTime(1970, 1, 1);
            targetTimes = targetTimes.AddSeconds(targetTime);
            TimeSpan diff = targetTimes - nowTimes;
            return GetTimeString((int)diff.TotalSeconds);
        }

        //飘字 提示字 提示时间 
        public static void NotifyStr(string notifyStr, float time = 1.5f)
        {
            Vector3 reginPosition = new Vector3(0, 0, 0);
            Vector3 targetPosition = new Vector3(0, 220, 0);
            //游戏界面的飘字
            if (FW.UI.UISceneMgr.CurrScene.RootObj.name.StartsWith("Game"))
                targetPosition = new Vector3(0, 180, 0);
            GameObject go = GameObject.Find("notify").gameObject;
            NGUITools.SetActive(go, true);
            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (go.transform.GetChild(i).GetComponent<UILabel>().text.Equals(""))
                {
                    go.transform.GetChild(i).GetComponent<UILabel>().text = notifyStr;
                    go = go.transform.GetChild(i).gameObject;
                    break;
                }
            }
            TweenPosition ta = go.GetComponent<TweenPosition>();
            if (ta == null)
            {
                ta = go.AddComponent<TweenPosition>();
            }
            ta.ResetToBeginning();
            ta.method = UITweener.Method.EaseOut;
            ta.from = new Vector3(0, 150, 0);
            ta.to = targetPosition;
            ta.enabled = true;
            ta.duration = time;
            ta.PlayForward();
            //动画执行完 回调
            EventDelegate.Set(ta.onFinished, delegate ()
            {
                go.transform.localPosition = reginPosition;
                go.GetComponent<UILabel>().text = "";
            });
        }

        //正则验证
        public static bool IsMathRex(string patternStr, string inputStr)
        {
            Regex regex = new Regex(patternStr);
            return regex.IsMatch(inputStr);
        }

        //修正scorllView的item顶格
        public static void ModifyItemT0p(GameObject go,Vector3 modifyPosition)
        {
            //修正
            SpringPanel ss = null;
            ss = go.GetComponent<SpringPanel>() == null ? go.AddComponent<SpringPanel>() : go.GetComponent<SpringPanel>();
            ss.target = modifyPosition;
            ss.enabled = true;
        }

        //Colors[EQualityColor_None]="#a4edf4";
        //Colors[EQualityColor_White]="#a4edf4";
        //Colors[EQualityColor_Green]="#70fe98";
        //Colors[EQualityColor_Blue]="#2d9aff";
        //Colors[EQualityColor_Purple]="#c772ee";
        //Colors[EQualityColor_Orange]="#ffb848";
        public static string GetColorStr(string str, int quality)
        {
            string colorStr = "[a4edf4]" + str +"[-]";
            switch (quality)
            {
                case 1: colorStr = "[a4edf4]" + str + "[-]"; break;
                case 2: colorStr = "[70fe98]" + str + "[-]"; break;
                case 3: colorStr = "[2d9aff]" + str + "[-]"; break;
                case 4: colorStr = "[c772ee]" + str + "[-]"; break;
                case 5: colorStr = "[ffb848]" + str + "[-]"; break;
                default:
                    break;
            }
            return colorStr;
        }

        //根据品质返回颜色名称
        public static string GetColorName(int quality)
        {
            string colorStr = "[a4edf4]" + "普通" + "[-]";
            switch (quality)
            {
                case 1: colorStr = "[a4edf4]" + "白色" + "[-]"; break;
                case 2: colorStr = "[70fe98]" + "绿色" + "[-]"; break;
                case 3: colorStr = "[2d9aff]" + "蓝色" + "[-]"; break;
                case 4: colorStr = "[c772ee]" + "紫色" + "[-]"; break;
                case 5: colorStr = "[ffb848]" + "橘色" + "[-]"; break;
                default:
                    break;
            }
            return colorStr;
        }

        //计算补差价
        public static int [] Calculate(int price,int haveGold)
        {
            int[] arrry = new int[2];
            //处理差价的
            int diamond = (price - haveGold) / 10 + 1;
            if ((price - haveGold) % 10 == 0)
            {
                diamond -= 1;
            }
            int gold = price-diamond*10;
            if( gold >= 0)
                arrry[0] = gold;
            arrry[1] = diamond;
            return arrry;
        }

        //移动scollview到指定位置
        public static void MoveScrollViewTOTarget(GameObject go, Vector3 target, float strength = 1000f)
        {
            SpringPanel springPanel = null;
            if (go.GetComponent<SpringPanel>() == null)
            {
                springPanel = go.AddComponent<SpringPanel>();
            }
            else
            {
                springPanel = go.GetComponent<SpringPanel>();
            }
            springPanel.target = target;
            springPanel.strength = strength;
            springPanel.enabled = true;
        }

        public static void MoveScrollViewTOTarget(Transform go, Vector3 target)
        {
            MoveScrollViewTOTarget(go.gameObject, target);
        }

        public static void MoveAnimation(Transform targetTran, Vector3 fromPosition, Vector3 toPosition, float animTime,
           UITweener.Method TWMethod =  UITweener.Method.EaseOut)
        {
            TweenPosition ta = targetTran.GetComponent<TweenPosition>();
            if (ta == null)
            {
                ta = targetTran.gameObject.AddComponent<TweenPosition>();
            }
            ta.ResetToBeginning();
            ta.method = TWMethod;
            ta.from = fromPosition;
            ta.to = toPosition;
            ta.enabled = true;
            ta.duration = animTime;
            ta.PlayForward();
        }
    }
}