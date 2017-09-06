//******************************************************************
// File Name:					SceneGame.cs
// Description:					SceneGame class 
// Author:						wuwei
// Date:						2016.12.30
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FW.Game;
using FW.Event;

namespace FW.Scene
{
    class SceneGame : SceneBase
    {
        protected SceneGame()
        {
            this.m_type = SceneType.Game;
        }

        public static SceneBase Create()
        {
            return new SceneGame();
        }

        

        //--------------------------------------
        //public 
        //--------------------------------------
        //初始化
        public override void Init()
        {
            base.Init();

            GJGameControl.Instance.Init();

        }

        


        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            GJGameControl.Instance.Update(deltaTime);
        }

        //销毁
        public override void Dispose()
        {
            
            GJGameControl.Instance.Dispose();
            base.Dispose();
        }
    }
}