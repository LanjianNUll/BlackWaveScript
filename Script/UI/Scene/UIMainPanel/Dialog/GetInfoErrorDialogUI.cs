//******************************************************************
// File Name:					Class1
// Description:					Class1 class 
// Author:						lanjian
// Date:						3/27/2017 2:37:20 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    /// <summary>
    /// 加载信息错误的 返回重新登录UI
    /// </summary>
    class GetInfoErrorDialogUI : DialogBaseUI
    {
        protected GetInfoErrorDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/GetInfoErrorDialog";
            this.m_DType = DialogType.GetInfoError;
        }

        public static GetInfoErrorDialogUI Create()
        {
            return new GetInfoErrorDialogUI();
        }

        private void GetDialogAbout()
        {
            this.SetDialogParent(UISceneMgr.CurrScene.RootObj.transform.Find("MainPanel/DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            //将MainPanel的层设为4
            UISceneMgr.CurrScene.RootObj.transform.Find("MainPanel").GetComponent<UIPanel>().depth = 4; ;
            this.BindEventLister();
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("confirm")).onClick = OnLoginBackIn;
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            int type = (int)args[0];
            //int 错误码(1:加载角色基本数据,2:创建角色,3:加载背包,4:加载技能,5:加载任务,6:加载统计数据,
            //7:加载邮件,8:加载商店,9:加载好友,10:加载关卡,11:加载实物仓库数据,12:加载实物兑换记录,
            //13:加载挂机任务,14:加载交易信息)
            string warningStr = "";
            if (type == 1)
                warningStr = "加载角色基本数据出错！！错误码"+type;
            if (type == 2)
                warningStr = "创建角色出错！！错误码" + type;
            if (type == 3)
                warningStr = "加载背包出错！！错误码" + type;
            if (type == 4)
                warningStr = "加载技能出错！！错误码" + type;
            if (type == 5)
                warningStr = "加载任务出错！！错误码" + type;
            if (type == 6)
                warningStr = "加载统计数据出错！！错误码" + type;
            if (type == 7)
                warningStr = "加载邮件出错！！错误码" + type;
            if (type == 8)
                warningStr = "加载商店出错！！错误码" + type;
            if (type == 9)
                warningStr = "加载好友出错！！错误码" + type;
            if (type == 10)
                warningStr = "加载关卡出错！！错误码" + type;
            if (type == 11)
                warningStr = "加载实物仓库数据出错！！错误码" + type;
            if (type == 12)
                warningStr = "加载实物兑换记录出错！！错误码" + type;
            if (type == 13)
                warningStr = "加载挂机任务出错！！错误码" + type;
            if (type == 14)
                warningStr = "加载交易信息出错！！错误码" + type;
            warningStr += "!! 请重新登录！";
            m_DialogUIGo.transform.Find("content").GetComponent<UILabel>().text = warningStr;
        }

        //重新登录
        private void OnLoginBackIn(GameObject go)
        {
            //释放当前scence
            PanelMgr.Dispose();
            //将自动登录去掉
            FW.Login.LoginConfig.SetUserAutoLogin(false);
            //重新进入登录界面
            Scene.SceneMgr.Enter(Scene.SceneType.Login);
        }
        
        //--------------------------------------
        //public
        //--------------------------------------

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }
    }
}
