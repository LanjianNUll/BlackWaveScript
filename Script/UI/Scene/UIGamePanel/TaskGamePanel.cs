//******************************************************************
// File Name:					TaskGamePanel
// Description:					TaskGamePanel class 
// Author:						lanjian
// Date:						3/21/2017 2:45:33 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Event;
using FW.Item;
using FW.Task;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace FW.UI
{
    class TaskGamePanel
    {
        private GameObject m_TaskGO;    
        private GameObject m_center;                //一个对话框
        private GameObject m_loadingtip;            //加载前的提示

        private UILabel m_timeLimit;
        private UILabel m_lastCount;
        private long m_time;
        //--------------------------------------
        //private 
        //--------------------------------------
        private void FindUI()
        {
            m_TaskGO = UISceneMgr.CurrScene.RootObj.transform.Find("Middle").gameObject;
            m_center = UISceneMgr.CurrScene.RootObj.transform.Find("center").gameObject;
            m_loadingtip = UISceneMgr.CurrScene.RootObj.transform.Find("loadingtip").gameObject;
            m_timeLimit = m_TaskGO.transform.Find("timelimit").GetChild(0).GetComponent<UILabel>();
            m_lastCount = m_TaskGO.transform.Find("tasktip/wordvalue").GetComponent<UILabel>();
            Utility.Utility.GetUIEventListener(m_TaskGO.transform.Find("startTaskbtn").gameObject)
                .onClick = OnStartTask;
            Utility.Utility.GetUIEventListener(m_center.transform.GetChild(3).gameObject).onClick = OnConfirm;
        }

        private void OnConfirm(GameObject go)
        {
            NGUITools.SetActive(this.m_center, false);
            //更新界面,剩余次数和时间
            TaskMgr.Instance.RequestTaskInfo();
        }

        //开始任务请求
        private void OnStartTask(GameObject go)
        {
            if (!TaskMgr.Instance.CheckStartTask())
            {
                Utility.Utility.NotifyStr("任务剩余次数不足,无法开始任务");
                return;
            }
            TaskMgr.Instance.RequestStartTask();
        }

        //真正开始任务
        private void DoStartTask()
        {
            this.FinsishTaseUI(false);
        }

        //结束任务处理
        private void DoEndTask(EventArg args)
        {
            if (args == null)
            {
                Utility.Utility.NotifyStr("当前任务失败！！");
                return;
            }
            TaskData data = (TaskData)args[0];
            this.ShowTaskReward(data);
            this.FinsishTaseUI(false);
        }

        //服务器端数据反馈,界面刷新
        private void OnTaskInfoResponse(EventArg args)
        {
            NGUITools.SetActive(m_TaskGO, true);
            NGUITools.SetActive(m_loadingtip, false);
            //(int 1:正在挂机 2:没有挂机)
            int state = (int)args[0];
            if (state == 1)
            {
                this.FinsishTaseUI(false);
            }
            if (state == 2)
            {
                this.FinsishTaseUI();
            }
            m_lastCount.text = TaskMgr.Instance.LastCount.ToString();
        }

        private void FinsishTaseUI(bool isFinsished = true)
        {
            NGUITools.SetActive(this.m_TaskGO.transform.Find("timelimit").gameObject, !isFinsished);
            NGUITools.SetActive(this.m_TaskGO.transform.Find("tasktip").gameObject, isFinsished);
            NGUITools.SetActive(this.m_TaskGO.transform.Find("startTaskbtn").gameObject, isFinsished);
        }

        //显示任务奖励
        private void ShowTaskReward(TaskData taskData)
        {
            NGUITools.SetActive(this.m_center, true);
            int[] item_ids = taskData.ItemIDs;
            int[] item_amounts = taskData.ItemAmounts;
            List<string> itemNames = GetNames(item_ids);
            string showStr = "";
            for (int i = 0; i < itemNames.Count; i++)
            {
                showStr += itemNames[i] + " X " + item_amounts[i]+ "\n";
            }
            this.m_center.transform.Find("Content").GetComponent<UILabel>().text = showStr;
        }

        private List<string> GetNames(int [] ids)
        {
            List<string> strs = new List<string>();
            List<string> icons = new List<string>();
            for (int i = 0; i < ids.Length; i++)
            {
                ResMgr.JsonItem jsonItem = FW.ResMgr.DatasMgr.GetJsonItem(ids[i]);
                strs.Add(jsonItem.Get("name").AsString());
                //210  武器   220 配件   204 道具
                if (ids[i] / 1000000 == 210)
                {
                    icons.Add(Utility.ConstantValue.WeaponIcon + "/" + ResMgr.DatasMgr.GetRes(jsonItem.Get("bagIcon").AsInt()));
                }
                if (ids[i] / 1000000 == 220)
                {
                    icons.Add(Utility.ConstantValue.PartIcon + "/" + ResMgr.DatasMgr.GetRes(jsonItem.Get("partsIcon").AsInt()));
                }
                if (ids[i] / 1000000 == 204)
                {
                    icons.Add(Utility.ConstantValue.CommodityIcon + "/" + ResMgr.DatasMgr.GetRes(jsonItem.Get("bagIcon").AsInt()));
                }
            }
            return strs;
        }

        //倒计时显示
        private void OnTimerChange(FW.Event.EventArg args)
        {
            m_timeLimit.text = Utility.Utility.GetTimeString((int)args[0]);
        }

        //
        int i = 0;
        private void OnChangeLoadAnim()
        {
            string str = "";
            if (i % 3 == 0)
                str = "挂机加载中.";
            if (i % 3 == 1)
                str = "挂机加载中..";
            if (i % 3 == 2)
                str = "挂机加载中...";
            if (m_loadingtip == null) return;
            m_loadingtip.GetComponent<UILabel>().text = str;
            if (i == 6)
                i = 0;
            i++;
        }
        //--------------------------------------
        //public 
        //--------------------------------------
        public void Init()
        {
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_TaskInfo, OnTaskInfoResponse);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_TaskStart, DoStartTask);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_TaskEnd, DoEndTask);
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.GAME_SecondInvoke, OnTimerChange);

            FindUI();
            Debug.Log("任务初始化");
            //请求任务初始化
            TaskMgr.Instance.RequestTaskInfo();
            //隐藏任务状态条
            NGUITools.SetActive(m_TaskGO,false);
            NGUITools.SetActive(m_loadingtip,true);
            m_time = Timer.Regist(0, 0.3f, OnChangeLoadAnim);
        }

        public void DisPose()
        {
            Timer.Cancel(m_time);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_TaskInfo, OnTaskInfoResponse);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_TaskStart, DoStartTask);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_TaskEnd, DoEndTask);
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.GAME_SecondInvoke, OnTimerChange);
        }
    }
}
