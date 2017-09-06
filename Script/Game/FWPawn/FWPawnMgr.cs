//******************************************************************
// File Name:					FWPawnMgr.cs
// Description:					FWPawnMgr class 
// Author:						wuwei
// Date:						2017.01.14
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

namespace FW.Game
{
    delegate FWPawn FWPawnCreator(Int64 id, int resID);
    static class FWPawnMgr
    {
        private static Dictionary<FWPawnType, FWPawnCreator> sm_Creators;
        private static Dictionary<Int64, FWPawn> sm_pawns;
        private static Int64 sm_index;
        private static GameObject sm_pawnPool;
        //暂时只支持回收敌人的模型
        private static List<FWPawn> sm_retrievePawns=new List<FWPawn>();

        static FWPawnMgr()
        {
            sm_Creators = new Dictionary<FWPawnType, FWPawnCreator>();
            sm_Creators.Add(FWPawnType.Hero, FWHero.Create);
            sm_Creators.Add(FWPawnType.Enemy, FWEnemy.Create);

            sm_pawns = new Dictionary<Int64, FWPawn>();
            sm_index = 0;
            sm_pawnPool = new GameObject("PawnPool");
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public static Dictionary<Int64, FWPawn> Pawns { get { return sm_pawns; } }
        public static Int64 GetId()
        {
            return ++sm_index;
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public static FWPawn Create(FWPawnType type,int resID)
        {
            Debug.Log("create pawn,type:"+type.ToString());
            Int64 id = GetId();
            FWPawn pawn = sm_Creators[type](id, resID);
            sm_pawns.Add(id, pawn);
            return pawn;
        }

        public static FWPawn GetPawnFromPool()
        {
            if (sm_retrievePawns.Count>0)
            {
                FWPawn pawn= sm_retrievePawns[0];
                sm_retrievePawns.RemoveAt(0);
                sm_pawns.Add(pawn.ID,pawn);
                pawn.ShowModel();
                pawn.Model.GameObj.transform.SetParent(null);
                return pawn;
            }else
            {
                return null;
            }
        }

        public static void AddPawn(Int64 id,FWPawn pawn)
        {
            sm_pawns.Add(id, pawn);
        }

        //查找某一个pawn
        public static FWPawn FindPawn(Int64 id)
        {
            FWPawn pawn;
            if (sm_pawns.TryGetValue(id, out pawn))
            {
                return pawn;
            }
            return null;
        }

        //移除pawn
        public static void Remove(Int64 id)
        {
            if (sm_pawns.ContainsKey(id) == false) return;
            //FindPawn(id).Dispose();
            FWPawn pawn = FindPawn(id);
            if (pawn.IsSelf)
            {
                pawn.Dispose();
            }else
            {
                pawn.HideModel();
                pawn.Model.GameObj.transform.parent = sm_pawnPool.transform;
                sm_retrievePawns.Add(pawn);
            }
            sm_pawns.Remove(id);
        }
    }
}