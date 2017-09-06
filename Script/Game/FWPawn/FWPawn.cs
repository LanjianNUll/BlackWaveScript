//******************************************************************
// File Name:					FWPawn.cs
// Description:					FWPawn class 
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
using FW.Game;
using FW.ResMgr;

namespace FW.Game
{
    public class FWPawn
    {
        private readonly Vector3 EFFECT_LOCAL_POS = new Vector3(0.22f, 0.3f, 0.6f);
        /// <summary>
        /// 角色状态枚举
        /// </summary>
        protected enum PawnState
        {
            Idle,
            Walk,
            Fire,
            Die
        }
        protected static Dictionary<PawnActionType, string> sm_actionName;
        /// <summary>
        /// 角色id,序列号
        /// </summary>
        private Int64 m_id;
        //资源模型id
        private int m_resID;
        //模型组件
        private SkinModel m_model;
        //是否是自己
        private bool m_isSelf = true;

        //当前血量
        private int m_hp = 0;
        //最大满血值
        private int m_hpMax = 30;
        //攻击范围
        private float m_attackRange = 5f;
        //移动速度
        private float m_moveSpeed = 5f;
        //攻击力
        private int m_attackPower = 0;
        //角色当前所处状态
        private PawnState state = PawnState.Idle;
        //被击特效资源id
        private int m_hitResID = 4;
        //被击特效模型
        //private ParticleModel m_hitModel;
        //武器
        private FWWeapon m_weapon;
        /// <summary>
        /// 当前对手
        /// </summary>
        private FWPawn m_opponent;


        static FWPawn()
        {
            sm_actionName = new Dictionary<PawnActionType, string>();
            sm_actionName.Add(PawnActionType.Idle, "Idle");
            sm_actionName.Add(PawnActionType.Walk, "RunF");
            sm_actionName.Add(PawnActionType.Fight, "Fire");
            sm_actionName.Add(PawnActionType.Death, "Dead");
        }

        public FWPawn(Int64 id, int resID, bool isSelf)
        {
            m_id = id;
            m_isSelf = isSelf;
            this.Init(resID);
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        public Int64 ID { get { return this.m_id; } }
        //是否死亡
        public bool IsDie { get { return this.state == PawnState.Die; } }
        //是否是自己
        public bool IsSelf { get { return this.m_isSelf; } }
        //攻击范围
        public float AttackRange { get { return this.m_attackRange; } }
        //角色的坐标
        public Vector3 Pos { get { return this.m_model.GameObj.transform.position; } }
        //血量
        public int Hp { get { return m_hp; } }
        //移动速度
        public float MoveSpeed { get { return m_moveSpeed; } }
        //获得模型对象
        public SkinModel Model { get { return m_model; } }


        //--------------------------------------
        //protected 
        //--------------------------------------
        protected virtual void Init(int resID)
        {
            this.m_resID = resID;

            this.m_model = new SkinModel();
            this.m_model.Load(FW.ResMgr.StaticDataDic.GetISValue(resID));
            this.m_model.Tranform(new Vector3(0.0f, -90.0f, 0.0f));

            this.Play(PawnActionType.Idle, true, null);

            //this.m_hitModel = new ParticleModel();
            //this.m_hitModel.Load(FW.ResMgr.StaticDataDic.GetISValue(this.m_hitResID));
            //gameObj = GameObject.Instantiate(ResLoad.Load<GameObject>(FW.ResMgr.StaticDataDic.GetISValue(m_hitResID)));
            //this.m_hitModel = gameObj.AddComponent<ParticleModel>();
            //AttachHitedEffect();
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        //设置HP
        public void SetHp(int hp)
        {
            this.m_hp = hp;
        }

        //设置HP为最大值
        public void SetMaxHp()
        {
            this.m_hp = m_hpMax;
        }

        //设置攻击力
        public void SetAttackPower(int attackPower)
        {
            this.m_attackPower = attackPower;
        }

        //播放动画
        public void Play(PawnActionType type, bool loop, SkinBehaviour.AnimFinishCallback callback)
        {
            if (this.m_model == null) return;
            string name;
            if (sm_actionName.TryGetValue(type, out name))
            {
                this.m_model.Play(name, loop, callback);
            }
        }

        /// <summary>
        /// 角色移动
        /// </summary>
        public virtual void Move()
        {
            Play(PawnActionType.Walk, true, () =>
             {
            //Debug
        });
            this.state = PawnState.Walk;
        }

        /// <summary>
        /// 角色开火
        /// </summary>
        public virtual void Fire(bool hasCB)
        {
            if (m_opponent == null || m_opponent.IsDie)
            {
                Debug.LogWarning("m_opponent==null||m_opponent.IsDie,name:" + Model.GameObj.name);
                return;
            }
            if (this.state != PawnState.Fire)
            {
                if (hasCB)
                    Play(PawnActionType.Fight, true, FireFinishCallback);
                else
                    Play(PawnActionType.Fight, true, null);
                this.state = PawnState.Fire;
            }
            m_weapon.Fire();
            //if (pawn != null)//第一次开火,需要一个对手
            //{
            //    if (hasCB)
            //        Play(PawnActionType.Fight, true, FireFinishCallback);
            //    else
            //        Play(PawnActionType.Fight, true,null);

            //    this.state = PawnState.Fire;
            //}
            //m_weapon.Fire();
            //if(m_opponent!=null&& !m_opponent.IsDie)//如果对手不为null且没死,才有被击操作
            //    m_opponent.Hited(this);
        }
        /// <summary>
        /// 设置一个对手
        /// </summary>
        /// <param name="pawn"></param>
        public void SetOpponent(FWPawn pawn)
        {
            this.m_opponent = pawn;
        }

        /// <summary>
        /// 角色待机状态
        /// </summary>
        public virtual void Idle()
        {
            Play(PawnActionType.Idle, true, null);
            this.state = PawnState.Idle;
        }

        /// <summary>
        /// 角色被攻击
        /// </summary>
        /// <param name="pawn">是谁在攻击</param>
        public void Hited(FWPawn pawn)
        {
            //m_hitModel.Play();
            Effect.EffectMgr.Instance.PlayEffect( m_hitResID, GetHitedEffectParent(), EFFECT_LOCAL_POS, Quaternion.identity);
            if (this.m_hp > pawn.m_attackPower)
            {
                this.m_hp -= pawn.m_attackPower;
            }
            else
            {
                this.m_hp = 0;
                Died();
            }
            //Debug.LogError((IsSelf ? "Hero" : "Enemy")+"hited,hp:" +m_hp+"attacker:"+pawn.Model.name+",time:"+Time.time);
        }

        /// <summary>
        /// 角色死亡
        /// </summary>
        public virtual void Died()
        {
            this.state = PawnState.Die;
            Play(PawnActionType.Death, false, DieFinishCallback);
        }

        //对象销毁
        public void Dispose()
        {
            if (this.m_model != null)
            {
                this.m_model.Dispose();
            }
        }

        //设置位置
        public void SetPos(Vector2 pos)
        {
            if (this.m_model == null) return;
            this.m_model.SetPos(pos);
        }

        //附加武器
        public void Attach(FWWeapon weapon)
        {
            if (weapon == null) return;
            //找出绑定点
            string bonename = "b_Root/b_Bip/b_Hips/b_Spine/b_Spine1/b_Spine2/b_RightClav/b_RightArm/b_RightForeArm/b_RightHand/b_RightWeapon";
            Transform transform = this.m_model.GetBoneTransform(bonename, true);
            if (transform == null)
                Debug.LogFormat("{0} don't exist!!!", bonename);
            //如果绑定了武器，则删除
            if (transform.childCount > 0)
            {
            }
            weapon.AttachTo(this, transform);
            this.m_weapon = weapon;
        }
        //隐藏模型
        public void HideModel()
        {
            this.m_model.GameObj.SetActive(false);
        }
        //显示模型
        public void ShowModel()
        {
            this.m_model.GameObj.SetActive(true);
        }

        //获取附加特效父节点
        public Transform GetHitedEffectParent()
        {
            string bonename = "b_Root/b_Bip";
            Transform transform = this.m_model.GetBoneTransform(bonename, true);
            if (transform == null)
            {
                Debug.LogFormat("{0} don't exist!!!", bonename);
                return null;
            }
            return transform;

            //this.m_hitModel.GameObj.transform.parent = transform;
            //this.m_hitModel.GameObj.transform.localPosition = new Vector3(0.22f,0.3f,0.6f);
            //this.m_hitModel.GameObj.transform.localRotation = Quaternion.identity;
            //this.m_hitModel.GameObj.transform.localScale = Vector3.one;
        }

        //开火动画结束的回调
        public void FireFinishCallback()
        {
            if (state == PawnState.Fire)
            {
                Fire(true);
            }
        }

        //死亡动画结束的回调
        public void DieFinishCallback()
        {

        }
    }
}