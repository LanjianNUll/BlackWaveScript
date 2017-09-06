//******************************************************************
// File Name:					FWWeapon.cs
// Description:					FWWeapon class 
// Author:						wuwei
// Date:						2017.01.17
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
    public class FWWeapon
    {
        private int m_resID;
        private SkinModel m_model;
        private int m_effectID=3;
        //private ParticleModel m_effectModel;

        private FWPawn m_pawn;
        private float m_damagekRange=10;
        private bool m_isAttackLot = false;

        public FWWeapon(int resID)
        {
            this.m_resID = resID;
            this.Init();
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public FWPawn Owner { get { return this.m_pawn; } }

        public ModelBase Model { get { return this.m_model; } }

        public int ResID { get { return this.m_resID; } }

        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init()
        {
            this.m_model = new SkinModel();
            this.m_model.Load("res/model/weapons/Gun_AR_M4A1STD/Gun_AR_M4A1STD");

            //this.m_effectModel = new ParticleModel();
            //this.m_effectModel.Load(FW.ResMgr.StaticDataDic.GetISValue(m_effectID));
            //AttackEffect();

            //GameObject gameObj= GameObject.Instantiate(ResLoad.Load<GameObject>("res/model/weapons/Gun_AR_M4A1STD/Gun_AR_M4A1STD"));
            //this.m_model = gameObj.AddComponent<SkinModel>();

            //gameObj = GameObject.Instantiate(ResLoad.Load<GameObject>(FW.ResMgr.StaticDataDic.GetISValue(m_effectID)));
            //this.m_effectModel = gameObj.AddComponent<ParticleModel>();
            //AttackEffect();
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Dispose()
        {
            if(this.m_model != null)
            {
                this.m_model.Dispose();
            }
        }
        /// <summary>
        /// 把武器绑定到一个角色身上
        /// </summary>
        /// <param name="pawn">角色</param>
        /// <param name="parent">武器的父节点</param>
        public void AttachTo(FWPawn pawn, Transform parent)
        {
            if (this.Model == null) return;
            this.m_pawn = pawn;
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.Euler(Vector3.zero);
            Vector3 scale = Vector3.one;
            string bonename = "b_Root/RightHandMale3P";
            Transform transform = this.m_model.GetBoneTransform(bonename, true);
            if(transform != null)
            {
                rot = Quaternion.Inverse(transform.localRotation);
                pos = transform.localPosition * -1.0f;
                scale = transform.localScale;
            }
            this.m_model.AttachTo(parent, pos, rot, scale);
        }
        /// <summary>
        /// 获取特效父节点
        /// </summary>
        public Transform GetEffectParent()
        {
            string bonename = "b_Root";
            Transform transform = this.m_model.GetBoneTransform(bonename, true);
            if (transform == null)
            {
                Debug.LogFormat("{0} don't exist!!!", bonename);
                return null;
            }
            return transform;

            //this.m_effectModel.GameObj.transform.parent = transform;
            //this.m_effectModel.GameObj.transform.localPosition = Vector3.zero;
            //this.m_effectModel.GameObj.transform.localRotation = Quaternion.Euler(0,0,90);
            //this.m_effectModel.GameObj.transform.localScale = Vector3.one;
        }
        /// <summary>
        /// 武器开火,执行武器需要做的操作
        /// </summary>
        public void Fire()
        {
            //if (m_effectModel == null) return;
            //m_effectModel.Play();
            Effect.EffectMgr.Instance.PlayEffect(m_effectID, GetEffectParent(), Vector3.zero, Quaternion.Euler(0, 0, 90));
            //攻击判定
            //遍历敌人 打中最近的
            Dictionary<Int64, FWPawn> Pawns = FWPawnMgr.Pawns;
            FWPawn temp = null;float disance = 0;float prev_distance = float.MaxValue;
            foreach (KeyValuePair<Int64, FWPawn> pair in Pawns)
            {
                if (m_pawn.IsSelf!=pair.Value.IsSelf&& !pair.Value.IsDie&& CheckCanAttack(m_pawn, pair.Value, out disance))//不是自己且能攻击到
                {
                    if (m_isAttackLot)
                    {
                        pair.Value.Hited(m_pawn);
                    }else
                    {
                        if (disance<prev_distance)
                        {
                            temp = pair.Value;
                            prev_distance = disance;
                        }
                    }
                }
            }
            if (temp!=null)
            {
                temp.Hited(m_pawn);
            }
        }
        /// <summary>
        /// 检测攻击者能否打到被攻击者
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="hiter">被攻击者</param>
        /// <param name="distance">两者间的距离(是返回值,而不是传入值)</param>
        /// <returns></returns>
        private bool CheckCanAttack(FWPawn attacker,FWPawn hiter,out float distance)
        {
            distance = Mathf.Abs(attacker.Pos.x - hiter.Pos.x);
            return distance <= m_damagekRange;
        }
    }
}