//******************************************************************
// File Name:					EffectMgr.cs
// Description:					EffectMgr class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       特效管理类,提供特效播放接口
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FW.Game;
using FW;

namespace FW.Effect
{
    enum EffectType
    {
        Hited = 1,
        Fire = 2
    }

    class EffectMgr
    {

        public static readonly EffectMgr Instance = new EffectMgr();
        private GameObject m_pool;
        private Dictionary<int, List<ParticleModel>> m_retrieveDic = new Dictionary<int, List<ParticleModel>>();

        private EffectMgr()
        {
            m_pool = new GameObject("EffectPool");
        }

        //--------------------------------------
        //private 
        //--------------------------------------
        /// <summary>
        /// 创建一个特效
        /// </summary>
        /// <param name="resid">特效资源id</param>
        /// <returns></returns>
        private ParticleModel CreateEffect(int resid)
        {
            ParticleModel model = new ParticleModel(resid);
            model.Load(FW.ResMgr.StaticDataDic.GetISValue(resid));
            return model;
        }

        /// <summary>
        /// 实际播放特效的过程
        /// </summary>
        /// <param name="model">特效本身</param>
        /// <param name="parent">特效父节点</param>
        /// <param name="localPos">坐标</param>
        /// <param name="rot">旋转</param>
        private void DoPlayEffect(ParticleModel model, Transform parent, Vector3 localPos, Quaternion rot, bool autoRetrieve)
        {
            model.GameObj.SetActive(true);
            model.GameObj.transform.parent = parent;
            model.GameObj.transform.localPosition = localPos;
            model.GameObj.transform.localRotation = rot;
            model.Play();
            if (autoRetrieve)
            {
                float time = model.GetParticlePlayTime();
                if (time <= 0)
                {
                    Debug.LogWarning("particle life time less than 0???????,name:" + model.GameObj.name);
                    return;
                }
                Timer.Regist(time, time, 1, OnPlayFinish, model);
            }
        }

        //播放完成回调
        private void OnPlayFinish(object[] args)
        {
            ParticleModel model = args[0] as ParticleModel;
            if (model != null)
            {
                List<ParticleModel> list = GetRetrieveEffectList(model.ResID);
                list.Add(model);
                foreach (var item in list)
                {
                    item.GameObj.SetActive(false);
                    item.GameObj.transform.parent = m_pool.transform;
                }
            }
        }

        //根据类型获取特效集合
        private List<ParticleModel> GetRetrieveEffectList(int effectID)
        {
            if (m_retrieveDic.ContainsKey(effectID))
            {
                return m_retrieveDic[effectID];
            }
            else
            {
                Debug.Log("create effect list, id:" + effectID);
                List<ParticleModel> list = new List<ParticleModel>();
                m_retrieveDic.Add(effectID, list);
                return list;
            }
        }


        //--------------------------------------
        //public 
        //--------------------------------------
        /// <summary>
        /// 播放特效接口
        /// </summary>
        /// <param name="effectid">特效资源id</param>
        /// <param name="parent">父节点</param>
        /// <param name="localPos">在父节点下的相对坐标</param>
        /// <param name="rot">在父节点下的相对旋转</param>
        /// <param name="autoRetrieve">是否自动回收</param>
        /// <returns></returns>
        public bool PlayEffect(int effectid, Transform parent, Vector3 localPos, Quaternion rot, bool autoRetrieve = true)
        {
            List<ParticleModel> list = GetRetrieveEffectList(effectid);
            ParticleModel model = null;
            if (list.Count == 0)
            {
                model = CreateEffect(effectid);
            }
            else
            {
                model = list[0];
                list.RemoveAt(0);
            }
            DoPlayEffect(model, parent, localPos, rot, autoRetrieve);
            return true;
        }

        // 播放特效接口
        public bool PlayEffect(int effectid, Transform parent, Vector3 localPos)
        {
            return PlayEffect(effectid, parent, localPos, Quaternion.identity);
        }

        // 播放特效接口
        public bool PlayEffect(int effectid, Transform parent)
        {
            return PlayEffect(effectid, parent, Vector3.zero, Quaternion.identity);
        }

        // 播放特效接口,在跟目录下创建
        public bool PlayEffect(int effectid, Vector3 localPos)
        {
            return PlayEffect(effectid, null, localPos, Quaternion.identity);
        }
    }
}