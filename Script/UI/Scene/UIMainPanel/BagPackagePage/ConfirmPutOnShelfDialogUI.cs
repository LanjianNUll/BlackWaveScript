//******************************************************************
// File Name:					ConfirmPutOnShelfDialogUI
// Description:					ConfirmPutOnShelfDialogUI class 
// Author:						lanjian
// Date:						3/9/2017 2:20:23 PM
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
    /// 确定上架
    /// </summary>
    class ConfirmPutOnShelfDialogUI:DialogBaseUI
    {
        protected ConfirmPutOnShelfDialogUI()
        {
            this.m_resName = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/ConfirmPutOnshelfDialog";
            this.m_DType = DialogType.ConfirmPutUp;

            //注册事件
            FW.Event.FWEvent.Instance.Regist(FW.Event.EventID.Deal_putShelveItem, OnPutShelveBack);
        }

        public static ConfirmPutOnShelfDialogUI Create()
        {
            return new ConfirmPutOnShelfDialogUI();
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
            //上架后，该物品会消失。确定上架？
            ItemBase i = (ItemBase)(this.m_currentArgs[0]);
            string name = i.Name;
            m_DialogUIGo.transform.Find("Tips").GetComponent<UILabel>().text = "上架，" + name + "会消失";
        }

        //上架返回
        private void OnPutShelveBack(FW.Event.EventArg args)
        {
            this.CloseDialog();
            int ret = (int)args[1];
            if (ret == 0)
            {
                Utility.Utility.NotifyStr("物品已经上架，请在交易界面查看详情！！");
                FWPageMgr.Instance.CurrentScorollViewItem.ReLoadpage();
            }
            else if (ret == 8)
                Utility.Utility.NotifyStr("手续费不足，上架失败！！");
            else if (ret == 10)
                Utility.Utility.NotifyStr("该物品已经装备，物品上架失败！！错误码");
            else if (ret == 12)
            {
                Utility.Utility.NotifyStr("上架物品数量已达上限，无法再上架；");
            }
            else if (ret == 14)
            {
                Utility.Utility.NotifyStr("物品已绑定，无法上架；");
            }
            else
            {
                Utility.Utility.NotifyStr("物品上架失败 错误码；"+ret);
            }

            Debug.Log("物品上架返回!错误码:" + ret);

            //判断下是武器还是配件  这里是因为武器详情界面和配件详情不是 dialogUI
            ItemBase i = (ItemBase)(this.m_currentArgs[0]);
            if (i.ItemType == ItemType.Weapon)
            {
                //返回武器列表 要重新刷新数据
                WeaponDetailsDialogUI.Instance.BackWeaponList();
                OnSoldItemDialogUI.Instance.Hide();
            }
            if (i.ItemType == ItemType.Accessory)
            {
                PartsDetailsDialogUI.Instance.BackAccessoryList();
                OnSoldItemDialogUI.Instance.Hide();
            }
        }

        //上架请求确定
        private void OnConfirm(GameObject go)
        {
            ItemBase itemBase = (ItemBase)this.m_currentArgs[0];
            //参数  1 价格  2 数量  3  时间
            itemBase.PutShelveRe((int)this.m_currentArgs[1], (int)this.m_currentArgs[2], (int)this.m_currentArgs[3]);
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("bottom"));
        }

        //取消按钮
        public override void OnCancel(GameObject go)
        {
            this.CloseDialog();
            //禁止左右滑动
            if (PanelMgr.CurrPanel != null)
                PanelMgr.CurrPanel.IsAllowHorMove(false);
            ItemBase i = (ItemBase)(this.m_currentArgs[0]);
            if (i.ItemType == ItemType.Weapon)
            {
                //显示交易配件的界面
                OnSoldItemDialogUI.Instance.ShowOnSoldDialogBoxUI((WeaponBase)i);
                return;
            }
            if (i.ItemType == ItemType.Accessory)
            {
                OnSoldItemDialogUI.Instance.ShowOnSoldDialogBoxUI((AccessoryBase)i);
                return;
            }
            //返回详情界面
            DialogMgr.Load(DialogType.CondsignForSaleDialog);
            DialogMgr.CurrentDialog.ShowCommonDialog(this.m_currentArgs);
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            this.GetDialogAbout();
            this.FillDataUI();
            this.OpenDialog();
        }

        public override void DisPose()
        {
            //注销事件
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Deal_putShelveItem, OnPutShelveBack);
            base.DisPose();
        }
    }
}