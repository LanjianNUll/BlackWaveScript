//******************************************************************
// File Name:					LuckyJoyBetInputDialog
// Description:					LuckyJoyBetInputDialog class 
// Author:						lanjian
// Date:						5/18/2017 5:25:22 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class LuckyJoyBetInputDialog : DialogBaseUI
    {
        protected LuckyJoyBetInputDialog()
        {
            this.m_resName = "UIRootPrefabs/LuckJoyPanel/LuckJoyBetInputDialog";
            this.m_DType = DialogType.LuckyJoy;
        }

        public static LuckyJoyBetInputDialog Create()
        {
            return new LuckyJoyBetInputDialog();
        }

        private UILabel m_totalLabel;
        private UIInput m_inputLabel;
        private Transform m_topTran;
        private Transform m_btnGroupTran;
        private bool[] lineState = new bool[5];
        private int m_moneyCount = 0;
        private int m_baseMoney = 0;

        //--------------------------------------
        //private
        //--------------------------------------
        private void GetDialogAbout()
        {
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).Find("DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            m_topTran = this.m_DialogUIGo.transform.GetChild(0);
            m_totalLabel = this.m_topTran.GetChild(4).GetComponent<UILabel>();
            m_btnGroupTran = this.m_topTran.GetChild(6);
            m_inputLabel = this.m_topTran.GetChild(9).GetComponent<UIInput>();
            this.GetNeedHideGo();
            this.BindEventLister();
        }

        private void BindEventLister()
        {
            for (int i = 0; i < m_btnGroupTran.childCount; i++)
            {
                Utility.Utility.GetUIEventListener(m_btnGroupTran.GetChild(i)).onClick = OnLineClick;
            }
            Utility.Utility.GetUIEventListener(this.m_topTran.GetChild(7)).onClick = OnConfirm;
            Utility.Utility.GetUIEventListener(this.m_topTran.GetChild(8)).onClick = OnCancel;
            m_inputLabel.onChange.Add(new EventDelegate(OnInputChange));
        }

        //输入框变化
        private void OnInputChange()
        {
            this.ChangeMonyCount();
        }

        //改变金额
        private void ChangeMonyCount()
        {
            int betCount = 0;
            for (int i = 0; i < lineState.Length; i++)
            {
                if (lineState[i])
                    betCount++;
            }
            int baseCount = 0;
            if (int.TryParse(this.m_inputLabel.value, out baseCount))
            {
                m_totalLabel.text = betCount * baseCount + "";
                this.m_baseMoney = baseCount;
                this.m_moneyCount = betCount * baseCount;
            }
            else
            {
                m_totalLabel.text = "0";
                this.m_baseMoney = 0;
                this.m_moneyCount = 0;
                Utility.Utility.NotifyStr("输入或格式有误，请重新输入！！");
            }
        }

        //点击每个下注线
        private void OnLineClick(GameObject go)
        {
            int index = int.Parse(go.name);
            lineState[index] = !lineState[index];
            if (lineState[index])
                go.GetComponent<UISprite>().spriteName = "circle_big_down";
            else
                go.GetComponent<UISprite>().spriteName = "circle_big_up";
            this.ChangeMonyCount();
        }

        private void OnConfirm(GameObject go)
        {
            if (this.m_baseMoney == 0 || this.m_moneyCount == 0)
            {
                Utility.Utility.NotifyStr("请选择下注的金额或下注的线数！");
                return;
            }
            if (this.m_baseMoney < 10)
            {
                Utility.Utility.NotifyStr("单线最低投注10钻石,请重新调整押注！！");
                return;
            }
            if (this.m_moneyCount > 1000)
            {
                Utility.Utility.NotifyStr("你的押注总数已经超过1000钻石，小赌怡情！！");
                return;
            }
            this.CloseDialog();
            LuckyJoy.LuckyBetItem luckBetItem = new LuckyJoy.LuckyBetItem(this.lineState, this.m_baseMoney,this.m_moneyCount);
            //返回摇奖界面
            DialogMgr.Load(DialogType.LuckyJoy);
            DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(luckBetItem));
        }
        
        //--------------------------------------
        //public
        //--------------------------------------
        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).GetChild(0));
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.GetChild(1));
        }

        public override void OnCancel(GameObject go)
        {
            this.CloseDialog();
            //返回摇奖界面
            DialogMgr.Load(DialogType.LuckyJoy);
            DialogMgr.CurrentDialog.ShowCommonDialog(null);
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.GetDialogAbout();
            this.OpenDialog();
        }

        public override void DisPose()
        {
            base.DisPose();
        }
    }
}
