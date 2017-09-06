//******************************************************************
// File Name:					TerrainMgr.cs
// Description:					TerrainMgr class 
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
    static class TerrainMgr
    {
        private static GameTerrain sm_terrain;

        static TerrainMgr()
        {
            sm_terrain = null;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        //出生点
        public static Vector2 StartPos { get { return sm_terrain.StartPos; } }

        //敌人的最终位置点
        public static Vector2 EnemyStartPos { get { return sm_terrain.EnemyEndPos; } }

        //地图尺寸
        public static Vector2 Size { get { return sm_terrain.Size; } }

        //--------------------------------------
        //public 
        //--------------------------------------
        public static void Load(string name)
        {
            Dispose();
            sm_terrain = new GameTerrain();
            sm_terrain.Load(name);
        }

        public static void Dispose()
        {
            if (sm_terrain != null)
            {
                sm_terrain.Dispose();
            }
        }

        public static void View(Vector2 pos)
        {
            if (sm_terrain != null)
            {
                sm_terrain.View(pos);
            }
        }
    }
}