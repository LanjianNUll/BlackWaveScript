//******************************************************************
// File Name:					TerrainLayer.cs
// Description:					TerrainLayer class 
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
    class TerrainLayer
    {
        private GameObject m_obj;
        private TerrainLayerType m_type;
        private TerrainLayerInfo m_info;
        private string m_frontName;

        private Dictionary<int, TerrainTile> m_tiles;
        private Dictionary<int, TerrainTile> m_temp;

        public TerrainLayer(GameObject parent, TerrainLayerType type, TerrainLayerInfo info, float depth, string frontName)
        {
            this.m_type = type;
            this.m_info = info;
            this.m_frontName = frontName;

            this.m_tiles = new Dictionary<int, TerrainTile>();
            this.m_temp = new Dictionary<int, TerrainTile>();
            this.m_obj = new GameObject(type.ToString());
            this.m_obj.transform.localPosition = new Vector3(0.0f, 0.0f, depth);
            this.m_obj.transform.parent = parent.transform;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public GameObject GameObj { get { return this.m_obj; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private string GetTileTexName(int index)
        {
            return String.Format("{0}_{1:00}.png", this.m_frontName, index);
        }

        //导入地图小块
        private void LoadTile(int index)
        {
            
            int fileIndex = index % this.m_info.TileCount;
            Vector3 pos = Vector3.zero;
            pos.x = this.m_info.TileSize.x * index;
            pos.y = this.m_info.Level;
            if (this.m_temp.ContainsKey(fileIndex))
            {
                TerrainTile t = this.m_temp[fileIndex];
                this.m_tiles.Add(index, t);
                t.SetPos(pos);
                this.m_temp.Remove(fileIndex);
            }else
            {
                string name = String.Format("tile_{0:00}", index);
                string fileName = this.GetTileTexName(fileIndex + 1);

                TerrainTile tile = new TerrainTile(name, fileName, pos, this.m_info.TileSize, this.m_obj, index);
                this.m_tiles.Add(index, tile);
            }
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        //移动
        public void View(float center, float width)
        {
            List<int> invalids = new List<int>();
            //计算显示的索引
            float left = center - 0.5f * width;
            int leftIndex = (int)(left / this.m_info.TileSize.x);
            float right = center + 0.5f * width;
            int rightIndex = (int)Math.Ceiling(right / this.m_info.TileSize.x);
            List<int> result = new List<int>();
            for (int i = leftIndex; i <= rightIndex; i++)
            {
                result.Add(i);
                //找出新增的
                if(this.m_tiles.ContainsKey(i) == false)
                {
                    LoadTile(i);
                }
            }
            //删除多余的
            foreach(int key in this.m_tiles.Keys)
            {
                if(result.Contains(key) == false)
                {
                    invalids.Add(key);
                }
            }
            foreach(int key in invalids)
            {
                this.m_tiles[key].Dispose();
                this.m_temp.Add(key % this.m_info.TileCount, m_tiles[key]);
                this.m_tiles.Remove(key);
            }
        }

        //销毁
        public void Dispose()
        {
            if(this.m_obj != null)
            {
                GameObject.Destroy(this.m_obj);
            }
            foreach(TerrainTile tile in this.m_tiles.Values)
            {
                tile.Dispose();
            }
            this.m_tiles.Clear();
            foreach (TerrainTile tile in this.m_temp.Values)
            {
                tile.Dispose();
            }
            this.m_temp.Clear();
        }

    }
}