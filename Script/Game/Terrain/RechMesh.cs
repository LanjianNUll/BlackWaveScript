//******************************************************************
// File Name:					RectMesh.cs
// Description:					RectMesh class 
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
    class RectMesh
    {
        private Vector3[] m_vertices;
        private Vector2[] m_uv;
        private Vector3[] m_normals;
        private int[] m_indexs;
        private Mesh m_mesh;
        private Bounds m_bounds;

        private RectMesh()
        {
            this.Init();
        }

        private void Init()
        {
            //构建顶点数组
            this.m_vertices = new Vector3[4];
            this.m_normals = new Vector3[4];
            this.m_mesh = new Mesh();
            this.m_bounds = new Bounds();
            //构建uv
            this.CalculateUV();
            //构建索引
            this.CalculateIndexs();
        }

        private void CalculateVertexs(float x, float y, float w, float h)
        {
            this.m_vertices[0] = new Vector3(x, y, 0.0f);
            this.m_vertices[1] = new Vector3(x + w, y, 0.0f);
            this.m_vertices[2] = new Vector3(x, y + h, 0.0f);
            this.m_vertices[3] = new Vector3(x + w, y + h, 0.0f);
            this.m_bounds.Encapsulate(this.m_vertices[0]);
            this.m_bounds.Encapsulate(this.m_vertices[1]);
            this.m_bounds.Encapsulate(this.m_vertices[2]);
            this.m_bounds.Encapsulate(this.m_vertices[3]);
        }

        private void CalculateNormals(Vector3 normal)
        {
            this.m_normals[0] = this.m_normals[1] = this.m_normals[2] = this.m_normals[3] = normal;
        }

        private void CalculateIndexs()
        {
            this.m_indexs = new int[6];
            this.m_indexs[0] = 0; this.m_indexs[1] = 2; this.m_indexs[2] = 1;          //第一个三角形
            this.m_indexs[3] = 1; this.m_indexs[4] = 2; this.m_indexs[5] = 3;          //第二个三角形
        }

        private void CalculateUV()
        {
            this.m_uv = new Vector2[4];
            this.m_uv[0].x = 0.0f; this.m_uv[0].y = 0.0f;
            this.m_uv[1].x = 1.0f; this.m_uv[1].y = 0.0f;
            this.m_uv[2].x = 0.0f; this.m_uv[2].y = 1.0f;
            this.m_uv[3].x = 1.0f; this.m_uv[3].y = 1.0f;
        }

        public Mesh GetMesh()
        {
            this.m_mesh.vertices = this.m_vertices;
            this.m_mesh.uv = this.m_uv;
            this.m_mesh.triangles = this.m_indexs;
            this.m_mesh.normals = this.m_normals;
            this.m_mesh.bounds = this.m_bounds;
            return this.m_mesh;
        }

        public static RectMesh Create(float x, float y, float w, float h, Vector3 normal)
        {
            RectMesh mesh = new RectMesh();
            mesh.CalculateVertexs(x, y, w, h);
            mesh.CalculateNormals(normal);
            return mesh;
        }
    }
}