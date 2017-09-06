//******************************************************************
// File Name:					WarningDialogUI
// Description:					WarningDialogUI class 
// Author:						lanjian
// Date:						3/2/2017 3:16:02 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{

    enum warningType
    {
        UnKonw,
        OnlyConfirmAndBack,                         //有确定和返回的
        OnlyConfirm,                                //只有确定
        All,                                        //三个都有

    }
    /// <summary>
    /// 通用的提示dialog
    /// </summary>
    class WarningDialogUI:DialogBaseUI
    {
        protected WarningDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/WarningDialog";
            this.m_DType = DialogType.Waring;
        }

        public static WarningDialogUI Create()
        {
            return new WarningDialogUI();
        }
        private warningType m_currentType;
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetDialogAbout()
        {
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).Find("DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            this.GetNeedHideGo();
            this.BindEventLister();
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("confirm")).onClick = OnConfirm;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("cancel")).onClick = OnCancel;
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
        }

        //填充UI 根据参数显示不同的提示
        private void FillDataUI(FW.Event.EventArg args)
        {
            m_currentType = (warningType)args[0];
            //0  只有确定按钮的提示
            if (m_currentType == warningType.OnlyConfirm)
            {
                NGUITools.SetActive(m_DialogUIGo.transform.Find("cancel").gameObject, false);
                NGUITools.SetActive(m_DialogUIGo.transform.Find("BackBtn").gameObject, false);
                ShowAnnounment((string)args[1]);
            }

            if (m_currentType == warningType.All)
            {
                ShowAnnounment((string)args[1]);
            }
        }

        //显示提示
        private void ShowAnnounment(string announceStr)
        {
            m_DialogUIGo.transform.Find("content").GetComponent<UILabel>().text = announceStr;
        }

        //兑换不足时的bujia
        private void ShowReplace()
        {
            NGUITools.SetActive(m_DialogUIGo.transform.Find("Replace").gameObject,true);
        }

        private void OnConfirm(GameObject go)
        {
            //根据不同的提示来判断确定的行为 
            if (m_currentType == warningType.All)
            {
                this.CloseDialog();
            }

            if (m_currentType == warningType.All)
            {
                //确定下架
                this.CloseDialog();
            }

            if (m_currentType == warningType.OnlyConfirm)
            {
                this.CloseDialog();
            }
        }
        //--------------------------------------
        //public
        //--------------------------------------

        public override void OnBack(GameObject go)
        {
            if (m_currentType == warningType.All)
            {
                DialogMgr.Load(DialogType.MyForSold);
                DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg());
                return;
            }
            this.CloseDialog();
        }

        public override void OnCancel(GameObject go)
        {
            if (m_currentType == warningType.All)
            {
                DialogMgr.Load(DialogType.MyForSold);
                DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg());
                return;
            }
            this.CloseDialog();
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }
    }
}
