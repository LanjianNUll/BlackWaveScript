//******************************************************************
// File Name:					ConfirmToSoldItemDialogUI
// Description:					ConfirmToSoldItemDialogUI class 
// Author:						lanjian
// Date:						3/9/2017 10:59:56 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Item;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    /// <summary>
    /// 确定出售的界面
    /// </summary>
    class ConfirmToSoldItemDialogUI:DialogBaseUI
    {
        protected ConfirmToSoldItemDialogUI()
        {
            this.m_resName = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/ConfirmToSoldItemDialog";
            this.m_DType = DialogType.ConfirmToSold;
        }

        public static ConfirmToSoldItemDialogUI Create()
        {
            return new ConfirmToSoldItemDialogUI();
        }

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
        }

        private void FillDataUI()
        {
            //出售后，该物品会消失。确定以XXX现金出售此物品吗？
            ItemBase i = (ItemBase)(this.m_currentArgs[0]);
            string name = i.Name;
            m_DialogUIGo.transform.Find("Tips").GetComponent<UILabel>().text = "出售后，"+name+"会消失";
        }

        private void OnConfirm(GameObject go)
        {
            this.CloseDialog();

            //判断下是武器还是配件  这里是因为武器详情界面和配件详情不是 dialogUI
            ItemBase i = (ItemBase)(this.m_currentArgs[0]);
            if (i.ItemType == ItemType.Weapon)
            {
                //返回武器列表 要重新刷新数据
                WeaponDetailsDialogUI.Instance.BackWeaponList();
            }
            if (i.ItemType == ItemType.Accessory)
            {
                PartsDetailsDialogUI.Instance.BackAccessoryList();
            }
            Utility.Utility.NotifyStr("物品已出售，请到邮件中提取收益！！");
        }
        //--------------------------------------
        //public
        //--------------------------------------
        //取消按钮
        public override void OnCancel(GameObject go)
        {
            ItemBase i = (ItemBase)(this.m_currentArgs[0]);
            if (i.ItemType == ItemType.Weapon || i.ItemType == ItemType.Accessory)
            {
                this.CloseDialog();
                return;
            }
            //返回详情界面
            DialogMgr.Load(DialogType.HandBombAndCommdityDetail);
            DialogMgr.CurrentDialog.ShowCommonDialog(this.m_currentArgs);
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            this.GetDialogAbout();
            this.FillDataUI();
            this.OpenDialog();
        }
    }
}
