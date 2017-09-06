//******************************************************************
// File Name:					FWEnemy.cs
// Description:					FWEnemy class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       角色类的子类,敌人
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System;

namespace FW.Game
{
    public class FWEnemy : FWPawn
    {

        /// <summary>
        /// 敌人类型
        /// </summary>
        private EnemyType m_type = EnemyType.Normal;

        public FWEnemy(Int64 id, int resID, bool isSelf) : base(id, resID, isSelf)
        {

        }

        //--------------------------------------
        //protected 初始化 
        //--------------------------------------
        protected override void Init(int resID)
        {
            base.Init(resID);
            this.Model.Tranform(new Vector3(0.0f, -90.0f, 0.0f));

            this.SetAttackPower(1);
            this.SetHp(100);//这里设置没用
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public static FWPawn Create(Int64 id, int resID)
        {
            return new FWEnemy(id, resID, false);
        }


        public override void Died()
        {
            base.Died();
            FW.Event.FWEvent.Instance.Call(FW.Event.EventID.GAME_EnemyDie, new FW.Event.EventArg(this));
        }
    }

}