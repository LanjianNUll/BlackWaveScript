//******************************************************************
// File Name:					TaskItem.cs
// Description:					TaskItem Class
// Author:						yangyongfang
// Date:						2017.03.30
// Reference:
// Using:                       任务类,任务的进度管理
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System;
using FW.Event;
using Network.Serializer;
using Network;

namespace FW.Task
{
    /// <summary>
    /// 任务基类
    /// </summary>
    class TaskItem
    {
        //任务配置数据
        private TaskData m_data;
        //任务id
        private int m_id;
        //任务状态
        private TaskState m_state;

        public TaskItem(int id,TaskData data)
        {
            this.m_id = id;
            this.m_data = data;
            Init();
        }


        //--------------------------------------
        //properties 
        //--------------------------------------
        public int ID { get { return m_id; } }

        public TaskData Data { get { return m_data; } }

        public TaskState State { get { return m_state; } }

        protected virtual void Init()
        {
            //注册请求任务开始和任务结束（请求任务奖励）
            NetDispatcherMgr.Inst.Regist(Commond.Request_Start_Task_back, ResponseStartTask);
            NetDispatcherMgr.Inst.Regist(Commond.Request_Get_Task_Reward_back, ResponseEndTask);
        }

        //--------------------------------------
        //private  
        //--------------------------------------
        //服务器反馈 任务开始请求返回
        private void ResponseStartTask(DataObj data)
        {
            int ret = data.GetUInt16("ret");
            int taskID = data.GetInt32("hangingId");
            if (taskID != this.ID) return;
            if (ret == 0)/*0:成功  1:当前在挂机中  2:今日挂机任务全部完成 3失败*/
            {
                this.InitTaskProgress();
                this.SetState(TaskState.Doing);
                FWEvent.Instance.Call(EventID.GAME_TaskStart);
            }
            else
            {
                Debug.Log("任务开始请求返回失败，错误码（/*0:成功  1:当前在挂机中  2:今日挂机任务全部完成 3失败*/）" + ret);
            }
        }

        //服务器反馈 请求结束任务返回
        private void ResponseEndTask(DataObj data)
        {
            int ret = data.GetUInt16("ret");
            int taskID = data.GetInt32("hangingId");
            if (taskID != this.ID) return;
            if (ret == 0)/*0:成功  1:当前没有开始挂机  2:挂机时间未到 3失败*/
            {
                this.SetState(TaskState.NotDoing);
                FWEvent.Instance.Call(EventID.GAME_TaskEnd, new EventArg(this.Data));
            }
            else
            {
                Debug.Log("任务结束请求返回失败，错误码（/*0:成功  1:当前没有开始挂机  2:挂机时间未到 3失败*/）" + ret);
            }
        }
        //--------------------------------------
        //public 
        //--------------------------------------

        //向服务器发送 任务开始请求
        public void RequestStartTask()
        {
            if (Login.LoginHandler.LoginState != Login.ELoginState.Game)
            {
                Utility.Utility.NotifyStr("当前网络断开,无法发送请求");
                return;
            }
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Start_Task, data);
        }

        //向服务器发送 结束任务请求
        public void RequestEndTask()
        {
            if (Login.LoginHandler.LoginState != Login.ELoginState.Game)
            {
                Utility.Utility.NotifyStr("当前网络断开,无法发送请求");
                return;
            }
            Debug.Log("RequestEndTask");
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Get_Task_Reward, data);
        }

        /// <summary>
        /// 初始化任务进度
        /// </summary>
        public virtual void InitTaskProgress()
        {

        }

        /// <summary>
        /// 同步任务进度
        /// </summary>
        /// <param name="param"></param>
        public virtual void SyncTaskProgress(object param)
        {

        }

        /// <summary>
        /// 检测任务是否完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual bool CheckTaskComplete(object param)
        {
            return true;
        }

        /// <summary>
        /// 设置任务状态
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetState(TaskState state)
        {
            this.m_state = state;
        }

        /// <summary>
        /// 销毁时处理
        /// </summary>
        public virtual void Dispose()
        {
            //注销请求任务开始和任务结束（请求任务奖励）
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Start_Task_back, ResponseStartTask);
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Get_Task_Reward_back, ResponseEndTask);
        }
    }

    /// <summary>
    /// 以时间判断结束的任务子类
    /// </summary>
    class DeadlineTask :TaskItem
    {
        private DateTime m_taskEndTime = DateTime.MinValue;
        private Int64 m_timer;
        private bool m_isTimerActive = false;

        public DeadlineTask(int id,TaskData data):base(id,data)
        {
        }

        protected override void Init()
        {
            base.Init();
            m_timer = Timer.Regist(0, 1, OnTimerCallback);
        }

        /// <summary>
        /// 初始化任务进度
        /// </summary>
        public override void InitTaskProgress()
        {
            int time = Data.EndConditionParam;
            m_taskEndTime = DateTime.Now.AddSeconds(time);
        }

        /// <summary>
        /// 同步任务进度
        /// </summary>
        /// <param name="param"></param>
        public override void SyncTaskProgress(object param)
        {
            int leaveTime = (int)param;
            m_taskEndTime = DateTime.Now.AddSeconds(leaveTime);
            if(State==TaskState.Doing)
                CheckTaskComplete(param);
        }

        /// <summary>
        /// 检测任务是否完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public override bool CheckTaskComplete(object param)
        {
            if (Data.EndConditionType == TaskCompleteType.TimeFinish)
            {
                int time = (int)param;
                if (time <= 0)
                {
                    PauseTimer();
                    //请求任务结束
                    this.RequestEndTask();
                    //TaskMgr.Instance.RequestEndTask();
                    FW.Event.FWEvent.Instance.Call(EventID.GAME_ScenePause);
                    return true;
                }
            }
            return false;
        }

        public override void SetState(TaskState state)
        {
            base.SetState(state);
            if (State == TaskState.NotDoing)
            {
                PauseTimer();
            }
            else if (State == TaskState.Doing)
            {
                StartTimer();
            }
        }

        //开启一个定时器
        public void StartTimer()
        {
            m_isTimerActive = true;

        }

        //暂停一个定时器
        public void PauseTimer()
        {
            m_isTimerActive = false;
        }

        //定时回调
        public void OnTimerCallback()
        {
            if (m_isTimerActive)
            {
                TimeSpan span = m_taskEndTime - DateTime.Now;
                double seconds = span.TotalSeconds;
                int time = Mathf.RoundToInt((float)seconds);
                if (time < 0) time = 0;
                Event.FWEvent.Instance.Call(EventID.GAME_SecondInvoke, new EventArg(time));
                if(time==0)
                    CheckTaskComplete(time);
            }
        }

        public override void Dispose()
        {
            Timer.Cancel(m_timer);
            m_timer = -1;
        }
    }
}