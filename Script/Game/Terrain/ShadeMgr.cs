//******************************************************************
// File Name:					ShadeMgr.cs
// Description:					ShadeMgr class 
// Author:						yangyongfang
// Date:						2017.03.27
// Reference:
// Using:                       场景切换时,加个遮罩层,平滑过渡
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System;

namespace FW.Game
{
    class ShadeMgr
    {
        //变换时间
        public const float CHANGE_TIME = 1f;
        //变换频率
        public const int CHANGE_COUNT = 20;
        public static ShadeMgr Instance = new ShadeMgr();

        private GameObject m_obj;//显示容器对象
        private MeshRenderer m_render;//容器渲染对象
        private Material m_mat;//容器渲染对象的材质
        
        private RenderTexture m_texture;
        private Int64 m_timer;
        private float m_color;//当前颜色
        private bool m_changeTransparent;//不透明-->透明 or 透明--->不透明
        private ShadeMgr()
        {
            
        }

        //--------------------------------------
        //private 
        //--------------------------------------
        public void Init()
        {
            m_obj = new GameObject("plane");
            m_obj.layer = 9;
            m_obj.transform.position = new Vector3(0, 0, -6);
            m_obj.AddComponent<MeshFilter>();
            m_render = m_obj.AddComponent<MeshRenderer>();
            m_obj.GetComponent<MeshFilter>().mesh = RectMesh.Create(-GameControl.GameArea.x/2, 0.0f, GameControl.GameArea.x, GameControl.GameArea.y, new Vector3(0.0f, 0.0f, -1.0f)).GetMesh();

            Shader shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null)
            {
                Debug.LogWarning("cannot find shader:Legacy Shaders/Diffuse");
                return;
            }
            m_mat = new Material(shader);
            m_texture = new RenderTexture(1080, 640, 24);
            GameCamera.GJCamera.targetTexture = m_texture;

            m_mat.mainTexture = m_texture;
            m_render.material = m_mat;

            EffectCamera.Instance.CreateCamera();
        }

        

        private void ChangeAlpha()
        {
            if (!m_changeTransparent)
            {//1-->0  变为透明
                if (m_color <= 0) return;
                m_color -= 1f / CHANGE_COUNT;
            }
            else
            {//0-->1  变为不透明
                if (m_color >= 1) return;
                m_color += 1f / CHANGE_COUNT;
            }//可以考虑在变换结束后加个回调
            m_mat.color = new Color(m_color, m_color, m_color);
            m_render.material = m_mat;
        }

        //--------------------------------------
        //public 设置材质的颜色
        //--------------------------------------
        public void SetAlpha(Color color)
        {
            m_mat.color = color;
            m_mat.mainTexture = m_texture;
            m_render.material = m_mat;
        }
        
        /// <summary>
        /// 场景由黑色切换到透明
        /// </summary>
        public void ChangeTransparent()
        {
            Timer.Cancel(m_timer);
            m_changeTransparent = true;
            m_timer = Timer.Regist(0, CHANGE_TIME / CHANGE_COUNT, CHANGE_COUNT, ChangeAlpha);
            m_color = 0f;
        }

        /// <summary>
        /// 场景由透明切换到黑色
        /// </summary>
        public void ChangeBlack()
        {
            Timer.Cancel(m_timer);
            m_changeTransparent = false;
            m_timer = Timer.Regist(0, CHANGE_TIME / CHANGE_COUNT, CHANGE_COUNT, ChangeAlpha);
            m_color = 1f;
        }



        public void Dispose()
        {
            Timer.Cancel(m_timer);
            GameObject.DestroyObject(m_texture);
            GameObject.DestroyObject(m_mat);
            GameObject.DestroyObject(m_obj);
            EffectCamera.Instance.Dispose();
            m_render = null;
        }

    }
}
