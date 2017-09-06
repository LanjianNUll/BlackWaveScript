//******************************************************************
// File Name:					UISceneMgr
// Description:					UISceneMgr class 
// Author:						lanjian
// Date:						1/3/2017 4:17:23 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FW.UI
{
    delegate UISceneBase UISceneCreator();
    static class UISceneMgr
    {
        private static Dictionary<Scene.SceneType, UISceneCreator> sm_creators;
        private static UISceneBase sm_CurrScene;

        static UISceneMgr()
        {
            sm_creators = new Dictionary<Scene.SceneType, UISceneCreator>();
            sm_creators.Add(Scene.SceneType.Login, UISceneLogin.Create);
            sm_creators.Add(Scene.SceneType.Main, UISceneMain.Create);
            sm_creators.Add(Scene.SceneType.Game, UISceneGame.Create);

            sm_CurrScene = null;
        }


        //--------------------------------------
        //public 
        //--------------------------------------
        public static UISceneBase CurrScene { get { return sm_CurrScene; } }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public static void Load(Scene.SceneType type)
        {
            UISceneCreator creator;
            if (sm_creators.TryGetValue(type, out creator))
            {
                sm_CurrScene = creator();
            }
            sm_CurrScene.Init();
        }

        //销毁
        public static void Dispose()
        {
            if(sm_CurrScene != null)
            {
                sm_CurrScene.DisPose();
                sm_CurrScene = null;
            }
        }

    }
}
