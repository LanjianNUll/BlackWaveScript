//******************************************************************
// File Name:					TerrainTile.cs
// Description:					TerrainTile class 
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
    class TerrainTile
    {
        private GameObject m_gameObj;
        private GameObject m_parent;
        private Material m_material;
        private string m_name;
        private Vector3 m_pos;
        private Vector2 m_size;
        private string m_texFile;
        private int m_index;

        public TerrainTile(string name, string tex, Vector3 pos, Vector2 size, GameObject parent, int index)
        {
            this.m_parent = parent;
            this.m_name = name;
            this.m_pos = pos;
            this.m_size = size;
            this.m_texFile = tex;
            this.m_index = index;
            this.Init();
        }
        //--------------------------------------
        //properties 
        //--------------------------------------
        public GameObject GameObj { get { return this.m_gameObj; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init()
        {
            if (this.GameObj == null)
            {
                this.m_gameObj = new GameObject(this.m_name);
                this.m_gameObj.AddComponent<MeshFilter>();
                this.m_gameObj.AddComponent<MeshRenderer>();
                this.m_gameObj.GetComponent<MeshFilter>().mesh = RectMesh.Create(0.0f, 0.0f, this.m_size.x, this.m_size.y, new Vector3(0.0f, 0.0f, -1.0f)).GetMesh();

                m_material= this.GetMat();
                this.m_gameObj.GetComponent<MeshRenderer>().material = m_material;
                if (this.m_parent != null)
                    this.m_gameObj.transform.parent = this.m_parent.transform;
                this.m_gameObj.transform.localPosition = this.m_pos;
            }
        }

        private Material GetMat()
        {
            Shader shader = Shader.Find("Unlit/Transparent Colored");
            Material mat = new Material(shader);
            Texture2D tex = ResMgr.ResLoad.Load<Texture2D>(this.m_texFile);
            if (tex != null)
            {
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Trilinear;
                mat.mainTexture = tex;
            }
            return mat;
        }

        public void SetPos(Vector3 pos)
        {
            this.m_pos = pos;
            this.m_gameObj.transform.localPosition = this.m_pos;
        }

        public void SetAlpha(float alpha)
        {
            Color c = m_material.GetColor("_TintColor");
            c.a = alpha;
            m_material.SetColor("_TintColor", c);
        }

        //------------------------------------------------
        //public
        //------------------------------------------------
        public void Dispose()
        {
            if (this.GameObj == null) return;
            GameObject.Destroy(m_material);
            GameObject.Destroy(this.GameObj);
            this.m_gameObj = null;
        }
    }
}