//******************************************************************
// File Name:					ModelBase.cs
// Description:					ModelBase class 
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

using FW.ResMgr;

namespace FW.Game
{
    public class ModelBase
    {
        private GameObject m_obj;

        //--------------------------------------
        //properties 
        //--------------------------------------
        public GameObject GameObj { get { return this.m_obj; } }

        public Vector3 Pos { get { return this.m_obj.transform.position; } }

        //--------------------------------------
        //public 
        //--------------------------------------
        public virtual void Load(string file)
        {
            this.m_obj = GameObject.Instantiate(ResLoad.Load<GameObject>(file));
        }

        public virtual void Dispose()
        {
            if (this.m_obj != null)
                GameObject.Destroy(this.m_obj);
            this.m_obj = null;
        }

        public virtual void SetPos(Vector3 pos)
        {
            if (this.m_obj == null) return;
            this.m_obj.transform.position = pos;
        }

        public virtual void Tranform(Vector3 rot)
        {
            if (this.m_obj == null) return;
            this.m_obj.transform.rotation = Quaternion.Euler(rot);
        }

        public virtual void Translate(float x, float y, float z)
        {
            if (this.m_obj == null) return;
            this.m_obj.transform.Translate(x, y, z, Space.World);
        }

    }
}
