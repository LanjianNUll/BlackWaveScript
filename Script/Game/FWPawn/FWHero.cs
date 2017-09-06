//******************************************************************
// File Name:					FWHero.cs
// Description:					FWHero class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       角色类的子类,英雄自己
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;

namespace FW.Game
{
    public class FWHero : FWPawn
    {
        public delegate bool CheckCanFire();
        /// <summary>
        /// 玩家角色的待机时间
        /// </summary>
        private float m_idleTime = 2;

        public float IdleTime { get { return m_idleTime; } }

        public FWHero(Int64 id, int resID, bool isSelf) : base(id, resID, isSelf)
        {
        }

        //--------------------------------------
        //protected 初始化 
        //--------------------------------------
        protected override void Init(int resID)
        {
            base.Init(resID);
            this.Model.Tranform(new Vector3(0.0f, 90.0f, 0.0f));

            this.SetAttackPower(10);
            this.SetHp(100000);
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public static FWPawn Create(Int64 id, int resID)
        {
            return new FWHero(id, resID, true);
        }

        public override void Move()
        {
            Play(PawnActionType.Walk, true, null);

            //是否在这里检测到达攻击目的地//这里不能检测,这里也不是真正的移动只是播放移动动画
        }


        public override void Died()
        {
            base.Died();
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.GAME_GameOver);
        }
    }
}

