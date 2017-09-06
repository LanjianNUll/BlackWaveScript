//******************************************************************
// File Name:					GameDefine.cs
// Description:					
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
    //地图层级
    enum TerrainLayerType
    {
        Main,               //主场景
        Front,              //前景象
    }

    //地图层级信息
    struct TerrainLayerInfo
    {
        public string FileName;            //地图级层资源名前缀
        public Vector2 TileSize;           //地图块大小
        public int TileCount;              //地图数量
        public float Level;                //地图高程
    }

    public enum PawnActionType
    {
        Idle,                               //待机
        Walk,                               //行走
        Fight,                              //战斗
        Death,                              //死亡
    }

    public enum EnemyType
    {
        Normal=0,
    }

    public enum FWPawnType
    {
        Unknow,
        Hero,             //自己
        Enemy          //敌人
    }
}