//******************************************************************
// File Name:					SceneMgr.cs
// Description:					SceneMgr class 
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

namespace FW.Scene
{
    enum SceneType
    {
        Unknow,
        Login,                  //登陆
        Main,                   //主场景
        Game,                   //游戏
    }
    delegate SceneBase SceneCreator();
    static class SceneMgr
    {
        private static Dictionary<SceneType, SceneCreator> sm_creators;
        private static SceneBase sm_currScene;              //当前场景

        static SceneMgr()
        {
            sm_creators = new Dictionary<SceneType, SceneCreator>();
            sm_creators.Add(SceneType.Login, SceneLogin.Create);
            sm_creators.Add(SceneType.Main, SceneMain.Create);
            sm_creators.Add(SceneType.Game, SceneGame.Create);

            sm_currScene = null;
        }


        //--------------------------------------
        //properties 
        //--------------------------------------
        //当前场景
        public static SceneBase CurrScene { get { return sm_currScene; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private static SceneBase Create(SceneType type)
        {
            SceneCreator creator;
            if(sm_creators.TryGetValue(type, out creator))
            {
                return creator();
            }
            return null;
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public static void Enter(SceneType type)
        {
            if (CurrScene != null)
                CurrScene.Dispose();
            sm_currScene = Create(type);
            if (sm_currScene == null)
                return;
            sm_currScene.Init();
        }

        public static void Update(float deltaTime)
        {
            if (sm_currScene == null) return;
            sm_currScene.Update(deltaTime);
        }
    }
}