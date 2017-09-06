//******************************************************************
// File Name:					SkinModel.cs
// Description:					SkinModel class 
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
    public class SkinModel : ModelBase
    {

        private SkinBehaviour m_behaviour;
        //深度搜索骨骼节点
        private Transform SearchTransform(Transform trans, string boneName)
        {
            Transform result = trans.FindChild(boneName);
            if (result != null)
                return result;
            for (int i = 0; i < trans.childCount; i++)
            {
                Transform tr = trans.GetChild(i);
                result = SearchTransform(tr, boneName);
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public override void Load(string file)
        {
            base.Load(file);
            if (this.GameObj != null)
                this.m_behaviour = this.GameObj.AddComponent<SkinBehaviour>();
        }

        //播放动画
        public void Play(string name, bool loop = false, SkinBehaviour.AnimFinishCallback callback = null)
        {
            m_behaviour.Play(name, loop, callback);
        }


        //找出骨骼节点
        public Transform GetBoneTransform(string boneName, bool search = false)
        {
            if (this.GameObj == null) return null;
            if (search)
            {
                return this.SearchTransform(this.GameObj.transform, boneName);
            }
            Transform trans = this.GameObj.transform.FindChild(boneName);
            return trans;
        }

        //绑定到父节点
        public void AttachTo(Transform parent)
        {
            this.AttachTo(parent, Vector3.zero, Quaternion.Euler(Vector3.zero), Vector3.one);
        }

        public void AttachTo(Transform parent, Vector3 pos, Quaternion rot, Vector3 scale)
        {
            if (this.GameObj == null) return;
            this.GameObj.transform.parent = parent;
            this.GameObj.transform.localPosition = pos;
            this.GameObj.transform.localRotation = rot;
            this.GameObj.transform.localScale = Vector3.one;
        }


        public override void Dispose()
        {
            m_behaviour.Dispose();
            base.Dispose();
        }
    }
}
