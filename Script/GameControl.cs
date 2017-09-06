//******************************************************************
// File Name:					GameControl.cs
// Description:					GameControl class 
// Author:						wuwei
// Date:						2016.12.27
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using FW.Scene;

namespace FW
{
    static class GameControl
    {
        private static bool sm_inited;
        private static float sm_gameAreadTop = 140.0f;
        private static float sm_gameAreadHeight = 640.0f;
        private static float sm_screenScale;
        private static Vector2 sm_defaultSize;                      //缺省屏幕大小
        private static Vector2 sm_screenSize;                       //屏幕大小(像素)
        private static Vector2 sm_windowSize;                       //窗口大小(米)
        private static Vector2 sm_gameArea;                         //游戏区域大小(米)

        static GameControl()
        {
            sm_inited = false;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public static float SceneHeight { get { return 5.0f; } }

        public static float RectTop { get { return sm_gameAreadTop / sm_defaultSize.y; } }
        public static float RectScale { get { return sm_gameAreadHeight / sm_defaultSize.y; } }

        //每个像素的实际尺寸
        public static float PixelScale { get { return sm_defaultSize.y / sm_screenSize.y * sm_screenScale; } }
        public static float ScreenSize { get { return sm_screenScale; } }

        public static Vector2 WindowSize { get { return sm_windowSize; } }
        public static Vector2 GameArea { get { return sm_gameArea; } }

        //--------------------------------------
        //public 
        //--------------------------------------
        static public void Init()
        {
            Debug.Log("系统语言为" + Application.systemLanguage);
            if (Application.systemLanguage.ToString().StartsWith("Chinese"))
            {
                Localization.language = "ChineseSimplified";
            }
            else
            {
                Localization.language = "English";
            }
            if (sm_inited) return;

            sm_defaultSize = new Vector2(1080, 1920);
            sm_screenSize = new Vector2(Screen.width, Screen.height);
            sm_screenScale = SceneHeight / sm_gameAreadHeight;

            sm_windowSize.x = sm_screenSize.x * PixelScale;
            sm_windowSize.y = sm_screenSize.y * PixelScale; 

            sm_gameArea.x = sm_windowSize.x;
            sm_gameArea.y = SceneHeight;

            //加载配置文件
            Login.LoginConfig.LoadFile();

            ResMgr.DatasMgr.Init();

            //初始化网络
            NetMgr.Instance.Init(new NSSerializerProtocol());

            //相机
            Game.GameCamera.Create();
            Game.ShadeMgr.Instance.Init();

            ////进入场景
            SceneMgr.Enter(SceneType.Login);

            sm_inited = true;
            Debug.LogFormat("game inited!!!!");
        }

        static public void Start()
        {
           // Login.LoginHandler.Login("","");
        }

        static public void Update(float deltaTime)
        {
#if UNITY_ANDROID
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
#endif 
            NetDispatcherMgr.Inst.Updata();
            Timer.Update();

            SceneMgr.Update(deltaTime);
        }
    }
}