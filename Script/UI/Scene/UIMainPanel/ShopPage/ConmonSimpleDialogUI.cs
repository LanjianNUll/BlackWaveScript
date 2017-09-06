//******************************************************************
// File Name:					ConmonSimpleDialogUI
// Description:					ConmonSimpleDialogUI class 
// Author:						lanjian
// Date:						2/24/2017 5:27:31 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Store;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    /// <summary>
    /// 商店确定购买
    /// </summary>
    class ConmonSimpleDialogUI:DialogBaseUI
    {
        protected ConmonSimpleDialogUI()
        {
            this.m_resName = "UIRootPrefabs/ConmonRes/conmonSimpleShopDialog";
            this.m_DType = DialogType.ConmonSimple;
        }

        public static ConmonSimpleDialogUI Create()
        {
            return new ConmonSimpleDialogUI();
        }

        private StoreItem m_currentStoreItem;                      //当前商品
        //--------------------------------------
        //private 
        //--------------------------------------
        private void GetDialog()
        {
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            this.GetNeedHideGo();
            this.BindEventLister();
            //隐藏返回按钮（这里可以根据弹框的类型判断）
            NGUITools.SetActive(this.m_DialogUIGo.transform.Find("back").gameObject, false);
        }
        
        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(this.m_DialogUIGo.transform.Find("confirm")).onClick = OnConfirm;
            Utility.Utility.GetUIEventListener(this.m_DialogUIGo.transform.Find("cancel")).onClick = OnCanCel;
            Utility.Utility.GetUIEventListener(this.m_DialogUIGo.transform.Find("back")).onClick = OnBack;
        }

        //确定
        private void OnConfirm(GameObject go)
        {
            m_currentStoreItem.Buy();
            this.CloseDialog();
        }

        //取消
        private void OnCanCel(GameObject go)
        {
            this.CloseDialog();
        }

        //显示tips  
        private void FillDataToUI(string tips)
        {
            NGUITools.SetActive(this.m_DialogUIGo.transform.Find("tips").gameObject,true);
            this.m_DialogUIGo.transform.Find("tips").GetComponent<UILabel>().text = tips;
        }

        //--------------------------------------
        //public
        //--------------------------------------
        //获取显示这个对话框  其他需要隐藏的界面
        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            Transform parent = this.m_DialogUIGo.transform.parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).gameObject != this.m_DialogUIGo)
                    m_needHideGo.Add(parent.GetChild(i));
            }
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.GetDialog();
            m_currentStoreItem = (StoreItem)args[0];
            FillDataToUI("确定花费"+ m_currentStoreItem.Price+ "金币购买"+ m_currentStoreItem.Item.Name);
            this.OpenDialog();
        }
    }
}
