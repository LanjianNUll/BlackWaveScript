//******************************************************************
// File Name:					GameTerrain.cs
// Description:					GameTerrain class 
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

using GJson;

namespace FW.Game
{
    class GameTerrain
    {
        private static Dictionary<TerrainLayerType, float> sm_layerDepths;
        private static Dictionary<TerrainLayerType, string> sm_layerNames;
        private static Dictionary<TerrainLayerType, string> sm_layerPaths;
        private static Dictionary<TerrainLayerType, string> sm_layerFrontNames;

        private Dictionary<TerrainLayerType, TerrainLayer> m_layers;
        private GameObject m_obj;
        private string m_pathName;
        private Vector2 m_startPos;
        private Vector2 m_enemyEndPos;
        private Vector2 m_size;

        static GameTerrain()
        {
            sm_layerDepths = new Dictionary<TerrainLayerType, float>();
            sm_layerDepths.Add(TerrainLayerType.Main, 5.0f);
            sm_layerDepths.Add(TerrainLayerType.Front, -5.0f);

            sm_layerNames = new Dictionary<TerrainLayerType, string>();
            sm_layerNames.Add(TerrainLayerType.Main, "main");
            sm_layerNames.Add(TerrainLayerType.Front, "front");

            sm_layerPaths = new Dictionary<TerrainLayerType, string>();
            sm_layerPaths.Add(TerrainLayerType.Main, "back/");
            sm_layerPaths.Add(TerrainLayerType.Front, "front/");

            sm_layerFrontNames = new Dictionary<TerrainLayerType, string>();
            sm_layerFrontNames.Add(TerrainLayerType.Main, "_b");
            sm_layerFrontNames.Add(TerrainLayerType.Front, "_f");
        }


        public GameTerrain()
        {
            this.m_layers = new Dictionary<TerrainLayerType, TerrainLayer>();
            this.m_obj = new GameObject("Terrain");
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public GameObject GameObj { get { return this.m_obj; } }

        public Vector2 Size { get { return this.m_size; } }

        public Vector2 StartPos { get { return this.m_startPos; } }

        public Vector2 EnemyEndPos { get { return this.m_enemyEndPos; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private string GetLayerFrontName(TerrainLayerType type)
        {
            return ResMgr.ResPaths.GetMapPath(this.m_pathName) + sm_layerPaths[type] + this.m_pathName + sm_layerFrontNames[type];
        }

        //读取layer info
        private TerrainLayerInfo GetLayerInfo(GJsonDict data)
        {
            if (data == null) return new TerrainLayerInfo();
            TerrainLayerInfo info = new TerrainLayerInfo();
            GJsonData subData;
            if (data.TryGetValue("filename", out subData))
            {
                info.FileName = subData.AsString();
            }
            if (data.TryGetValue("size", out subData))
            {
                int[] values = subData.AsInts();
                if(values.Length == 2)
                {
                    info.TileSize = new Vector2(values[0], values[1]) * GameControl.ScreenSize;
                }
            }
            if (data.TryGetValue("count", out subData))
            {
                info.TileCount = subData.AsInt();
            }
            if (data.TryGetValue("level", out subData))
            {
                info.Level = subData.AsFloat();
            }

            return info;
        }

        //读取所有地图层
        private void LoadLayers(GJsonDict data)
        {
            if (data == null) return;
            foreach (TerrainLayerType type in Enum.GetValues(typeof(TerrainLayerType)))
            {
                GJsonData subData = null;
                string name = sm_layerNames[type];
                if (data.TryGetValue(name, out subData))
                {
                    TerrainLayerInfo info = this.GetLayerInfo(subData as GJsonDict);
                    TerrainLayer layer = this.LoadLayer(type, info);
                    this.m_layers.Add(type, layer);
                }
            }
        }

        //读入每一个层
        private TerrainLayer LoadLayer(TerrainLayerType type, TerrainLayerInfo info)
        {
            return new TerrainLayer(this.m_obj, type, info, sm_layerDepths[type], this.GetLayerFrontName(type));
        }

        //--------------------------------------
        //public 加载这个地图的配置文件
        //--------------------------------------
        public void Load(string name)
        {
            this.m_pathName = name;
            //读取配置文件
            string config = ResMgr.ResPaths.GetMapConfigName(name);
            GJsonDict configData = null;
            TextAsset text = ResMgr.ResLoad.Load<TextAsset>(config);
            if (text != null)
                configData = GJson.GJson.Parse(text.text);
            if (configData == null) return;
            //读入layers
            GJsonData layerData;
            if (configData.TryGetValue("layer", out layerData))
            {
                LoadLayers(layerData as GJsonDict);
            }
            //读入地图其他信息
            GJsonData lenData;
            if (configData.TryGetValue("length", out lenData))
            {
                float length = lenData.AsFloat();
                this.m_size = new Vector2(length, GameControl.GameArea.y);
            }
            //读入出生点
            GJsonData startData;
            if (configData.TryGetValue("startPos", out startData))
            {
                float[] values = startData.AsFloats();
                if(values.Length == 2)
                {
                    this.m_startPos = new Vector2(values[0], values[1]);
                }

            }
            //读入敌人最终位置
            GJsonData enemyEndData;
            if (configData.TryGetValue("enemyEndPos", out enemyEndData))
            {
                float[] values = enemyEndData.AsFloats();
                if (values.Length == 2)
                {
                    this.m_enemyEndPos = new Vector2(values[0], values[1]);
                }

            }
        }

        //地图流动
        public void View(Vector2 pos)
        {
            foreach (TerrainLayer layer in this.m_layers.Values)
            {
                layer.View(pos.x, GameControl.WindowSize.x);
            }
        }

        //销毁
        public void Dispose()
        {
            foreach(TerrainLayer layer in this.m_layers.Values)
            {
                layer.Dispose();
            }
            this.m_layers.Clear();
            GameObject.Destroy(m_obj);
        }
    }
}