//******************************************************************
// File Name:					EffectCamera.cs
// Description:					EffectCamera class 
// Author:						yangyongfang
// Date:						2017.04.05
// Reference:
// Using:                       主摄像机看到的画面映射到某纹理,被该摄像机看到,对看到画面做进一步加工
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;

namespace FW.Game
{
    class EffectCamera
    {
        public static EffectCamera Instance = new EffectCamera();
        private Camera m_camera;//摄像机

        public void CreateCamera()
        {
            //if (m_camera != null) return;
            GameObject obj = new GameObject("gameCameraProjection");
            m_camera = obj.AddComponent<Camera>();
            m_camera.orthographic = true;
            m_camera.orthographicSize = 0.5f * GameControl.SceneHeight;
            m_camera.transform.localPosition = new Vector3(0.0f, m_camera.orthographicSize, -10.0f);
            m_camera.clearFlags = CameraClearFlags.SolidColor;
            m_camera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            m_camera.cullingMask = 1 << 9;
            m_camera.nearClipPlane = 0.0f;
            m_camera.farClipPlane = 20.0f;
            m_camera.rect = new Rect(0.0f, 1.0f - GameControl.RectScale - GameControl.RectTop, 1.0f, GameControl.RectScale);
            //m_camera.depth = 2;
            //camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        }

        public void Dispose()
        {
            GameObject.DestroyObject(m_camera.gameObject);
            //m_camera = null;
        }
    }
}