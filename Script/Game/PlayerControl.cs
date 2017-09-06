//******************************************************************
// File Name:					PlayerControl.cs
// Description:					PlayerControl class 
// Author:						wuwei
// Date:						2017.01.16
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FW.Player;
using Network;
using Network.Serializer;

using GJson;

namespace FW.Game
{
    static class PlayerControl
    {
        //玩家对象
        private static FWPawn sm_pawn;
        //玩家的武器
        private static FWWeapon sm_weapon;

        static PlayerControl()
        {
            sm_pawn = null;
        }

        //--------------------------------------
        //private 
        //--------------------------------------
        private static void Move()
        {
            Vector2 pos = TerrainMgr.StartPos;
            pos.x += GameCamera.Pos.x;
            sm_pawn.SetPos(pos);
        }

        public static void PlayAnim(PawnActionType type, bool loop = false)
        {
            sm_pawn.Play(type, loop,null);
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public static void Create()
        {
            //生成主角
            sm_pawn = FWPawnMgr.Create(FWPawnType.Hero,1);//1指玩家角色对应模型的资源id
            //生成武器
            sm_weapon = new FWWeapon(1);
            //绑定武器
            if (sm_pawn != null)
                sm_pawn.Attach(sm_weapon);

            //设置出生点
            Move();
        }

        public static void Dispose()
        {
            //销毁武器
            if (sm_weapon != null)
                sm_weapon.Dispose();
            //销毁主角
            if (sm_pawn != null)
                FWPawnMgr.Remove(sm_pawn.ID);
        }

        public static void Update(float deltaTime)
        {
            if (sm_pawn == null) return;
            Move();
        }
    }
}