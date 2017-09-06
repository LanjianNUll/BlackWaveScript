//******************************************************************
// File Name:					TaskMgr.cs
// Description:					TaskMgr Class
// Author:						yangyongfang
// Date:						2017.03.20
// Reference:
// Using:                       任务管理类,服务器端通信,任务数据存储,UI层通信
// Revision History:
//******************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Network.Serializer;
using FW;
using FW.Event;
using Network;
using System;

namespace FW.Task
{
    public enum TaskState
    {
        Doing = 1,
        NotDoing = 2,
        Done = 3
    }
    class TaskMgr
    {
        public static TaskMgr Instance = new TaskMgr();
        //剩余任务次数
        private int m_lastCount = 0;
        //当前任务
        private TaskItem m_item;

        private TaskMgr()
        {
            Init();
        }

        //--------------------------------------
        //properties 
        //--------------------------------------
        //任务剩余数量
        public int LastCount { get { return m_lastCount; } }
        //当前进行任务
        public TaskItem Item { get { return m_item; } }
        //--------------------------------------
        //private 
        //--------------------------------------
        private void Init()
        {
            //监听服务器端消息
            NetDispatcherMgr.Inst.Regist(Commond.Request_Task_Info_back, ResponseTaskInfo);
        }

        /// <summary>
        /// 服务器同步任务信息，没请求结束任务成功，都会触发
        /// </summary>
        /// <param name="data"></param>
        private void ResponseTaskInfo(DataObj data)
        {
            Debug.Log("ResponseTaskInfo---data:" + data.ToString() + ",time:" + Time.time);
            if (data == null) return;
            //需要获取剩余任务次数,以及第一个任务的id,根据这个id初始化场景
            if (data.GetUInt16("ret") == 0)
            {
                m_lastCount = data.GetInt32("remainNum");
                TaskState state = (TaskState)data.GetInt8("hangingState");
                int currentTaskID = data.GetInt32("hangingId");
                int leaveTime = data.GetInt32("doneTime");
                ////得到item
                if (m_lastCount == 0)
                {
                    state = TaskState.NotDoing;//对服务器端数据做个优化,如果剩余次数为0,忽略其他数据
                    if (m_item == null) m_item = CreateDefaultTaskItem();
                }
                else if (m_item == null || m_item.ID != currentTaskID)
                {
                    //将上一个任务销毁，创建下一个任务
                    if (m_item != null) m_item.Dispose();
                    m_item = CreateTaskItem(GetTaskData(currentTaskID));
                }
                //刷新item状态
                m_item.SyncTaskProgress(leaveTime);
                m_item.SetState(state);
                FWEvent.Instance.Call(EventID.GAME_TaskInfo, new EventArg((int)state));
            }
            else
            {
                Debug.LogWarning("ResponseTaskInfo-----ret !=0");
            }
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        //是否还有任务可做
        public bool CheckStartTask()
        {
            return m_lastCount > 0;
        }

        //向服务器发送 请求任务数据(同步请求数据)
        public void RequestTaskInfo()
        {
            if (Login.LoginHandler.LoginState!=Login.ELoginState.Game)
            {
                Utility.Utility.NotifyStr("当前网络断开,无法发送请求");
                return;
            }
            Debug.Log("RequestTaskInfo,time:"+Time.time);
            DataObj data = new DataObj();
            data["ret"] = (ushort)0;
            NetMgr.Instance.Request(Commond.Request_Task_Info, data);
        }

        //向服务器发送 任务开始请求
        public void RequestStartTask()
        {
            if (m_item != null)
                m_item.RequestStartTask();
        }
        /// <summary>
        /// 创建一个任务
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public TaskItem CreateTaskItem(TaskData data)
        {
            if (data.EndConditionType == TaskCompleteType.TimeFinish)
            {
                return new DeadlineTask(data.ID, data);
            }
            else
            {
                return new TaskItem(data.ID, data);
            }
        }

        /// <summary>
        /// 创建一个默认的任务
        /// </summary>
        /// <returns></returns>
        public TaskItem CreateDefaultTaskItem()
        {
            return CreateTaskItem(GetDefaultTaskItem());
        }

        /// <summary>
        /// 根据任务id获取关卡数据
        /// </summary>
        public TaskData GetTaskData(int id)
        {
            return TaskDataMgr.Instance.GetTaskItem(id);
        }

        /// <summary>
        /// 得到一个随意的任务数据
        /// </summary>
        /// <returns></returns>
        public TaskData GetDefaultTaskItem()
        {
            return TaskDataMgr.Instance.GetDefaultTaskItem();
        }

        public void Dispose()
        {
            NetDispatcherMgr.Inst.UnRegist(Commond.Request_Task_Info_back, ResponseTaskInfo);
        }
    }
}
