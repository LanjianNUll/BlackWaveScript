//******************************************************************
// File Name:					ParticleModel.cs
// Description:					ParticleModel class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                        粒子特效类
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using FW.ResMgr;

namespace FW.Effect
{
    class ParticleModel
    {
        private int m_resID;
        private ParticleSystem m_particle;
        //private Effect.EffectType m_type;

        private GameObject m_obj;

        //--------------------------------------
        //properties 
        //--------------------------------------
        public GameObject GameObj { get { return this.m_obj; } }

        /// <summary>
        /// 特效类型
        /// </summary>
        //public Effect.EffectType Type { get { return m_type; } }

        public int ResID { get { return m_resID; } }

        //--------------------------------------
        //public 
        //--------------------------------------
        public ParticleModel(int id)
        {
            this.m_resID = id;
        }

        public void Load(string file)
        {
            this.m_obj = GameObject.Instantiate(ResLoad.Load<GameObject>(file));
            if (this.GameObj != null)
                this.m_particle = this.GameObj.GetComponentInChildren<ParticleSystem>();
        }
        //void Awake()
        //{
        //    this.m_particle = GameObj.GetComponentInChildren<ParticleSystem>();
        //}

        //播放动画
        public void Play()
        {
            if (this.m_particle == null) return;
            m_particle.Play();
            //m_particle.
        }

        //获取粒子播放完成的时间
        public float GetParticlePlayTime()
        {
            if (this.m_particle == null) return 0.5f;

            if (m_particle.emission.enabled)
            {
                if (m_particle.loop)
                {
                    return -1f;
                }

                float duration = 0f;
                //if (m_particle.emission.rate.<= 0)
                //{
                //    dunration  = m_particle.startDelay + m_particle.startLifetime;
                //}
                //else
                //{
                duration = m_particle.startDelay + Mathf.Max(m_particle.duration, m_particle.startLifetime);
                //}
                //Debug.LogError("name:"+GameObj.name+",tiem:"+duration);
                return duration;
            }
            return 0.5f;
        }
    }
}
