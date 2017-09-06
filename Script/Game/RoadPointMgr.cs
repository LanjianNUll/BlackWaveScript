//******************************************************************
// File Name:					RoadPointMgr.cs
// Description:					RoadPointMgr class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       路径节点管理类,用于挂机游戏的对每个节点敌人的管理
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FW.Task;

namespace FW.Game
{
    public class RoadPointMgr
    {
        public const float LEFT_RIGHT_DIS = 0.6f;
        public static readonly RoadPointMgr Instance = new RoadPointMgr();
        //当前节点中心的位置
        private Vector2 m_pos;
        //当前节点的敌人
        private List<FWPawn> m_enemys=new List<FWPawn>();
        //玩家
        private FWPawn m_opponent;
        //当前走到第几个节点
        private int m_pointIndex = 0;
        //当前进行任务数据
        private LevelItem m_item;

        //--------------------------------------
        //private 获取一个敌人对象,先从回收池取,如果为null则create
        //--------------------------------------
        private FWPawn GetEnemyFromPool()
        {
            FWPawn pawn = FWPawnMgr.GetPawnFromPool();
            if (pawn == null)
            {
                pawn = FWPawnMgr.Create(FWPawnType.Enemy, 2);
                //生成武器
                FWWeapon weapon = new FWWeapon(1);
                //绑定武器
                pawn.Attach(weapon);
                //设置出生点
                //InitPos(pawn);
            }
            return pawn;
        }

        //初始化一个敌人的位置,index表示第几个敌人
        private void InitPos(FWPawn pawn,int index)
        {
            Vector2 pos = TerrainMgr.EnemyStartPos;
            //pos.x += GameCamera.Pos.x + m_opponent.MoveSpeed * FIRE_START_TIME;
            if (m_item.EnemyAppear==EnemyAppearType.TimeAppear)
            {
                pos.x += GameCamera.Pos.x + m_opponent.MoveSpeed * m_item.Param2[0];
            }
            else if (m_item.EnemyAppear==EnemyAppearType.PosAppear)
            {
                pos.x += GameCamera.Pos.x + m_item.Param2[0];
            }else
            {
                Debug.LogWarning("这种敌人刷新方式未实现");
            }
            
            if(m_item.EnemyDistribute==EnemyDistributeType.Simple)
                pawn.SetPos(pos);
            else if (m_item.EnemyDistribute == EnemyDistributeType.LeftRight)
            {
                float total_dis = LEFT_RIGHT_DIS * (m_item.Param1.Length - 1);
                pos.x -= total_dis / 2;
                pos.x += index * LEFT_RIGHT_DIS;
                pawn.SetPos(pos);
            }
            else
            {
                Debug.LogWarning("这种敌人分布方式未实现");
            }
        }

        //得到这个路径节点的位置
        private Vector3 GetPos(Vector2 pos)
        {
            //pos.x += GameCamera.Pos.x + m_opponent.MoveSpeed * FIRE_START_TIME;
            if (m_item.EnemyAppear == EnemyAppearType.TimeAppear)
            {
                pos.x += GameCamera.Pos.x + m_opponent.MoveSpeed * m_item.Param2[0];
            }
            else if (m_item.EnemyAppear == EnemyAppearType.PosAppear)
            {
                pos.x += GameCamera.Pos.x + m_item.Param2[0];
            }
            return pos;
        }

        //--------------------------------------
        //public 初始化
        //--------------------------------------
        public void Init(FWPawn hero, LevelItem item)
        {
            this.m_opponent = hero;
            this.m_item = item;

        }
        /// <summary>
        /// 刷新敌人的位置,这里先回收掉上个节点的敌人,然后刷新(死亡动画完成没有回收,而是在重新刷新的时候回收)
        /// </summary>
        public void RefreshEnemys()
        {
            m_pos = GetPos(TerrainMgr.EnemyStartPos);
            //先回收
            for (int i = 0; i < m_enemys.Count; i++)
            {
                FWPawnMgr.Remove(m_enemys[i].ID);
            }
            m_enemys.Clear();
            //后创建
            for (int i = 0; i < m_item.Param1.Length; i++)
            {
                FWPawn pawn = GetEnemyFromPool();
                InitPos(pawn,i);
                //设置待机状态和血量
                pawn.Idle();
                pawn.SetMaxHp();
                m_enemys.Add(pawn);
            }
            
            m_pointIndex++;
        }
        /// <summary>
        /// 敌人们向玩家攻击
        /// </summary>
        public void EnemysFire(FWPawn pawn)
        {
            m_opponent = pawn;
            for (int i = 0; i < m_enemys.Count; i++)
            {
                if (!m_enemys[i].IsDie)
                {
                    m_enemys[i].SetOpponent(m_opponent);
                    m_enemys[i].Fire(true);
                }
            }
        }
        /// <summary>
        /// 全部敌人进入待机
        /// </summary>
        public void EnemysIdle()
        {
            for (int i = 0; i < m_enemys.Count; i++)
            {
                if (!m_enemys[i].IsDie)
                    m_enemys[i].Idle();
            }
            
        }
        /// <summary>
        /// 得到一个可攻击的敌人
        /// </summary>
        public FWPawn GetOneEnemy()
        {
            for (int i = 0; i < m_enemys.Count; i++)
            {
                if (!m_enemys[i].IsDie)
                    return m_enemys[i];
            }
            return null;
        }
        /// <summary>
        /// 检测这个节点的敌人是否全部死亡
        /// </summary>
        public bool CheckAllDie()
        {
            bool isAllDie = true;
            for (int i = 0; i < m_enemys.Count; i++)
            {
                if (!m_enemys[i].IsDie)
                {
                    isAllDie = false;
                    break;
                }
            }
            return isAllDie;
        }
        /// <summary>
        /// 检测玩家是否走到攻击节点范围内
        /// </summary>
        /// <param name="pawn">玩家自身</param>
        /// <returns></returns>
        public bool CheckCanAttack(FWPawn pawn)
        {
            if (Mathf.Abs(pawn.Pos.x-m_pos.x)<= pawn.AttackRange)
            {
                return true;
            }
            return false;
        }



        public void Dispose()
        {
            for (int i = 0; i < m_enemys.Count; i++)
            {
                FWPawnMgr.Remove(m_enemys[i].ID);
            }
        }
    }
}
