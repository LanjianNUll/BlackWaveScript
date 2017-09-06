//******************************************************************
// File Name:					GJGameControl.cs
// Description:					GJGameControl class 
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       挂机游戏控制类
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using FW.Event;
using FW.Task;

namespace FW.Game
{
    public class GJGameControl
    {
        public static readonly GJGameControl Instance = new GJGameControl();
        
        enum TimeState
        {
            Move,
            Fire,
            Idle,
            Failed
        }
        //是否正在做任务
        private bool m_IsDoing = false;
        // 是否做了初始化
        private bool m_isInit = false;

        private TimeState m_State = TimeState.Idle;

        private FWPawn m_hero;

        //--------------------------------------
        //properties 
        //--------------------------------------
        public bool IsInit { get { return m_isInit; } }
        //public bool IsDoing { get { return m_IsDoing; } }

        //--------------------------------------
        //privaate 
        //--------------------------------------
        #region 玩家和地图移动

        private void TerrainMove(Vector2 pos)
        {
            if (pos.x < 0.5f * GameControl.GameArea.x)
                pos.x = 0.5f * GameControl.GameArea.x;
            else if (pos.x > Game.TerrainMgr.Size.x - 0.5f * GameControl.GameArea.x)
                pos.x = Game.TerrainMgr.Size.x - 0.5f * GameControl.GameArea.x;
            Game.GameCamera.Move(pos);
            Game.TerrainMgr.View(Game.GameCamera.Pos);
        }

        /// <summary>
        /// 角色移动
        /// </summary>
        /// <param name="pawn">角色对象</param>
        /// <param name="dir">方向,1为右,-1为左</param>
        private void PlayerMove(FWPawn pawn, int dir)
        {
            pawn.Model.Translate( dir * Time.deltaTime * m_hero.MoveSpeed,0,0);
        }

        /// <summary>
        /// 相机和地图移动
        /// </summary>
        private void TerrainMove()
        {
            Vector2 pos = TerrainMgr.StartPos;
            if(m_hero==null|| m_hero.Model==null|| m_hero.Model.GameObj == null)
            {
                return;
            }
            pos.x = m_hero.Model.GameObj.transform.position.x - pos.x;
            pos.y = Game.GameCamera.Pos.y;
            Game.GameCamera.Move(pos);
            Game.TerrainMgr.View(Game.GameCamera.Pos);
        }

        /// <summary>
        /// 根据摄像机的位置来初始化玩家的位置
        /// </summary>
        private void InitPlayerPos(FWPawn pawn)
        {
            Vector2 pos = TerrainMgr.StartPos;
            pos.x += GameCamera.Pos.x;
            pawn.SetPos(pos);
        }

        //创建玩家pawn
        private void CreatePlayer()
        {
            //生成主角
            m_hero = FWPawnMgr.Create(FWPawnType.Hero,1);//1指玩家角色对应模型的资源id
            //生成武器
            FWWeapon weapon = new FWWeapon(1);
            //绑定武器
            if (m_hero != null)
                m_hero.Attach(weapon);

            //设置出生点
            //PlayerMove(m_hero);
            InitPlayerPos(m_hero);
        }
        //初始化地图
        private void InitTerrain(LevelItem item)
        {
            Game.TerrainMgr.Load(item.Terrain);
            //Game.TerrainMgr.Load("city_01");
            Vector2 pos;
            pos.x = 0.5f * GameControl.GameArea.x;
            pos.y = 0.5f * GameControl.GameArea.y;
            this.TerrainMove(pos);
        }
        #endregion

        #region 几个游戏状态的处理

        //玩家开始移动的处理(包括地图的移动)
        private void MoveStart()
        {
            Debug.Log("MoveStart------------------");
            m_State = TimeState.Move;
            m_hero.Move();
            RoadPointMgr.Instance.RefreshEnemys();
            m_IsDoing = true;
        }
        //玩家开始开火
        private void FireStart()
        {
            Debug.Log("FireStart------------------");
            m_hero.SetOpponent(RoadPointMgr.Instance.GetOneEnemy());
            m_hero.Fire(true);
            RoadPointMgr.Instance.EnemysFire(m_hero);
        }
        //敌人死亡
        private void EnemyDie()
        {
            Debug.Log("EnemyDie------------------");
            if (RoadPointMgr.Instance.CheckAllDie())
            {
                m_hero.Idle();
                //定时器放在这里了
                FWHero hero = m_hero as FWHero;
                if (hero!=null)
                {
                    Timer.Regist(hero.IdleTime, hero.IdleTime, 1, OnIdleEnd);
                }
            }
            else
            {
                m_hero.SetOpponent(RoadPointMgr.Instance.GetOneEnemy());
                //m_hero.Fire(true);
            }
        }

        //玩家待机结束
        private void OnIdleEnd()
        {
            if (m_IsDoing)
                MoveStart();
        }
        /// <summary>
        /// 游戏失败,
        /// </summary>
        private void GameOver()
        {   //状态:此时,主角进入死亡状态和动画,对面的敌人
            //需要做的操作,对面的敌人全部进入待机状态
            RoadPointMgr.Instance.EnemysIdle();
            //通知界面游戏结束,暂时直接让ui监听这个消息,这里不做处理
        }

        //任务完成
        private void GameWin()
        {

        }

        #endregion

        #region 几个服务器消息反馈事件的处理
        private void RefreshSceneAndHero(TaskData item)
        {
            //场景切换1
            ShadeMgr.Instance.ChangeBlack();
            DestroySceneAndHero();
            //Debug.LogError("GJGameControl RefreshSceneAndHero----------------------------");
            LevelItem level = LevelDataMgr.Instance.GetLevelItem(item.LevelID);
            //初始化地图
            InitTerrain(level);
            //创建玩家
            this.CreatePlayer();
            //初始化路径节点管理类
            RoadPointMgr.Instance.Init(m_hero, level);
            //场景切换2
            ShadeMgr.Instance.ChangeTransparent();
        }

        private void DestroySceneAndHero()
        {
            //Debug.LogError("GJGameControl DestroySceneAndHero----------------------------");
            if (m_hero != null)
            {
                FWPawnMgr.Remove(m_hero.ID);
                RoadPointMgr.Instance.Dispose();
                Game.TerrainMgr.Dispose();
                m_hero = null;
            }
            
        }

        private void OnTaskInfoResponse()
        {
            TaskData item = TaskMgr.Instance.Item.Data;
            if (!m_IsDoing)//ISDoing==false,任务自然结束,退出界面,刚进来
            {
                RefreshSceneAndHero(item);
            }

            if (TaskMgr.Instance.Item.State == TaskState.NotDoing)
            {
                //派发事件给ui更新数据
            }
            else if (TaskMgr.Instance.Item.State == TaskState.Doing)
            {
                //派发事件给ui更新数据
                DoStartTask();
            }
            Debug.Log("SceneLoadComplete,time:" + Time.time);
        }

        //真正开始任务
        private void DoStartTask()
        {
            //通知时间控制器开始任务
            if (m_isInit == false)
            {
                return;
            }
            m_IsDoing = true;

            MoveStart();
        }

        //结束任务处理
        private void DoEndTask(EventArg args)
        {
            if (args == null)
            {
                //做掉线处理
            }
            m_State = TimeState.Idle;
            m_hero.Idle();
            RoadPointMgr.Instance.EnemysIdle();
            m_IsDoing = false;
        }

        /// <summary>
        /// 先把场景暂停
        /// </summary>
        private void ScenePause()
        {
            m_State = TimeState.Idle;
            if(m_hero!=null)
                m_hero.Idle();
            RoadPointMgr.Instance.EnemysIdle();
        }

        private void OnNetworkBroken()
        {
            ScenePause();
            if (Task.TaskMgr.Instance.Item != null)
            {
                Task.TaskMgr.Instance.Item.SetState(TaskState.NotDoing);
            }
            Utility.Utility.NotifyStr("当前网络断开,无法挂机");
        }

        private void OnNetworkReConnect()
        {
            if (Scene.SceneMgr.CurrScene.Type==Scene.SceneType.Game)
            {
                Task.TaskMgr.Instance.RequestTaskInfo();
            }
        }
         
        #endregion

        //--------------------------------------
        //public 初始化 
        //--------------------------------------
        public void Init()
        {
            //Debug.LogError("GJGameControl Init----------------------------");

            //监听几个游戏状态的事件
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_MoveStart, MoveStart);//任务开始/敌人全部死亡,主角进入倒计时,定时器回调触发(FWHero)
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_EnemyDie, EnemyDie);//敌人死亡触发(FWHero)
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_GameOver, GameOver);//主角死亡触发(FWHero)
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_GameWin, GameWin);//游戏胜利,任务管理器触发

            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_TaskInfo, OnTaskInfoResponse);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_TaskStart, DoStartTask);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_TaskEnd, DoEndTask);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_ScenePause, ScenePause);
            //网络消息
            //Event.FWEvent.Instance.Regist(EventID.NET_RECONN_START, OnNetworkBroken);
            Event.FWEvent.Instance.Regist(EventID.NET_RECONN_SUCCESS, OnNetworkReConnect);

            m_isInit = true;
        }

        public void Update(float deltaTime)
        {

            if (m_State == TimeState.Move)
            {
                if (m_hero == null || m_hero.Model == null || m_hero.Model.GameObj == null)
                {//这里是程序逻辑有问题,先避免报错以后处理(玩家进入待机,会有回调,此时离开界面,会销毁主角,然后报错)
                    return;
                }
                //玩家移动,然后更新摄像机的位置和地图
                PlayerMove(m_hero, 1);
                this.TerrainMove();
                if (RoadPointMgr.Instance.CheckCanAttack(m_hero))
                {
                    this.m_State = TimeState.Fire;
                    FireStart();
                }
            }

        }

        public void Dispose()
        {
            //Debug.LogError("GJGameControl Dispose----------------------------");
            m_State = TimeState.Idle;
            m_IsDoing = false;
            m_isInit = false;

            if (Task.TaskMgr.Instance.Item != null)
            {
                Task.TaskMgr.Instance.Item.SetState(TaskState.NotDoing);
            }
            //取消监听几个游戏状态的事件
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_MoveStart, MoveStart);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_EnemyDie, EnemyDie);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_GameOver, GameOver);//主角死亡触发(FWHero)
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_GameWin, GameWin);//游戏胜利,任务管理器触发

            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_TaskInfo, OnTaskInfoResponse);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_TaskStart, DoStartTask);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_TaskEnd, DoEndTask);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_ScenePause, ScenePause);
            //网络消息
            //Event.FWEvent.Instance.UnRegist(EventID.NET_RECONN_START, OnNetworkBroken);
            Event.FWEvent.Instance.UnRegist(EventID.NET_RECONN_SUCCESS, OnNetworkReConnect);

            DestroySceneAndHero();

        }
    }
}