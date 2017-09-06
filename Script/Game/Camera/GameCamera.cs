//******************************************************************
// File Name:					GameCamera.cs
// Description:					GameCamera class 
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
    static class GameCamera
    {
        private static GameObject sm_obj;
        private static Camera sm_camera;

        static GameCamera()
        {
            sm_obj = new GameObject("gameCamera");
            sm_camera = null;
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public static Vector2 Pos { get { return sm_obj.transform.position; } }

        public static Camera GJCamera { get { return sm_camera; } }
        //--------------------------------------
        //public 
        //--------------------------------------
        public static void Create()
        {
            if (sm_camera != null) return;
            sm_camera = sm_obj.AddComponent<Camera>();
            sm_camera.orthographic = true;
            sm_camera.orthographicSize = 0.5f * GameControl.SceneHeight;
            sm_camera.transform.localPosition = new Vector3(0.0f, sm_camera.orthographicSize, -10.0f);
            sm_camera.clearFlags = CameraClearFlags.SolidColor;
            sm_camera.backgroundColor = new Color(0.0f,0.0f,0.0f,0.0f);
            sm_camera.cullingMask = 1;
            sm_camera.cullingMask |= 1 << 8;
            sm_camera.nearClipPlane = 0.0f;
            sm_camera.farClipPlane = 20.0f;
            sm_camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        }

        //是进游戏
        public static void EnterGame(bool isGame)
        {
            //if (isGame)
            //{
            //    sm_camera.rect = new Rect(0.0f, 1.0f - GameControl.RectScale - GameControl.RectTop, 1.0f, GameControl.RectScale);
            //}
            //else
            //{
            //    sm_camera.transform.localPosition = new Vector3(0.0f, sm_camera.orthographicSize, -10.0f);
            //    sm_camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            //}

        }

        public static void Dispose()
        {
            if (sm_camera == null) return;
            GameObject.Destroy(sm_camera);
            sm_camera = null;
        }

        public static void Move(Vector2 pos)
        {
            if (sm_camera == null) return;
            Vector3 camPos = sm_obj.transform.position;
            camPos.x = pos.x;
            camPos.y = pos.y;
            sm_obj.transform.position = camPos;
        }
    }
}