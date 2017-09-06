//******************************************************************
// File Name:					HandBombAndCommdityDetailDialogUI
// Description:					HandBombAndCommdityDetailDialogUI class 
// Author:						lanjian
// Date:						3/8/2017 2:35:02 PM
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
    class HandBombAndCommdityDetailDialogUI:DialogBaseUI
    {
        protected HandBombAndCommdityDetailDialogUI()
        {
            this.m_resName = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/HandbombAndCommdityDetailDialog";
            this.m_DType = DialogType.HandBombAndCommdityDetail;
        }

        public static HandBombAndCommdityDetailDialogUI Create()
        {
            return new HandBombAndCommdityDetailDialogUI();
        }

        private Transform m_topTrans;
        //private Transform m_porpertyTrans;
        private Transform m_MiddleTrans;
        private Transform m_buttonGroupTrans;

       
        //--------------------------------------
        //private
        //--------------------------------------
        private void GetDialogAbout()
        {
            m_topTrans = this.m_DialogUIGo.transform.Find("Top");
            m_buttonGroupTrans = m_topTrans.Find("buttonGroup");
            //m_porpertyTrans = m_topTrans.Find("property");
            m_MiddleTrans = this.m_DialogUIGo.transform.Find("Middle");
            this.SetDialogParent(PanelMgr.CurrPanel.RootObj.transform.GetChild(0).Find("DialogGroup"));
            NGUITools.SetActive(this.m_DialogUIGo, false);
            this.GetNeedHideGo();
            this.BindEventLister();
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            //arg ：ItemBase物品对象   pageIndex  1武器页 2配件页 3手雷页 4其他页   tabindex  页面的tab
            ItemBase item = (ItemBase)args[0];
            int pageIndex = (int)args[1];
            if (pageIndex == 3)
            {
                Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.HandBombIcon +"/" + item.Icon + Utility.ConstantValue.UpEndPath);
                m_MiddleTrans.GetChild(0).GetComponent<UITexture>().SetRect(-texture.width / 2, -texture.height / 2, texture.width, texture.height);
                m_MiddleTrans.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                
                CommodityBase cItem = (CommodityBase)item;
                FillCommodityPro(cItem);
            }
            //其他 道具和消耗品
            if (pageIndex == 4)
            {
                CommodityBase cItem = (CommodityBase)item;
                FillCommodityPro(cItem);
            }

            CommodityBase cItem1 = (CommodityBase)item;
            FillCommodityPro(cItem1);
        }

        private void FillCommodityPro(CommodityBase commodity)
        {
            m_topTrans.Find("gunName").GetComponent<UILabel>().text = commodity.Name;
            m_topTrans.Find("gunLevel").GetComponent<UILabel>().text = "LV待定";
            m_topTrans.Find("gunCategroy").GetComponent<UILabel>().text = "道具";
            m_topTrans.Find("Des").GetComponent<UILabel>().text = commodity.Desc;
            string iconPath = Utility.ConstantValue.CommodityIcon;

            Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + commodity.Icon + Utility.ConstantValue.UpEndPath);
            m_MiddleTrans.GetChild(0).GetComponent<UITexture>().SetRect(-texture.width / 2, -texture.height / 2, texture.width, texture.height);
            m_MiddleTrans.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
            Transform propertyTranform = m_topTrans.Find("property");
            NGUITools.SetActive(propertyTranform.gameObject, false);
        }

        //填充属性
        private void FillWeaponPro(WeaponBase Weapon)
        {
            m_topTrans.Find("gunName").GetComponent<UILabel>().text = Weapon.Name;
            m_topTrans.Find("gunLevel").GetComponent<UILabel>().text = "LV待定";
            m_topTrans.Find("gunCategroy").GetComponent<UILabel>().text = "手雷武器";
            NGUITools.SetActive(m_topTrans.Find("Des").gameObject, false);
            Transform propertyTranform = m_topTrans.Find("property");
            //填充属性
            propertyTranform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = "伤害：";
            propertyTranform.GetChild(0).GetChild(1).GetComponent<UILabel>().text = Weapon.Injure + "";
            propertyTranform.GetChild(1).GetChild(0).GetComponent<UILabel>().text = "破甲：";
            propertyTranform.GetChild(1).GetChild(1).GetComponent<UILabel>().text = Weapon.SunderArmor + "";
            propertyTranform.GetChild(2).GetChild(0).GetComponent<UILabel>().text = "射速：";
            propertyTranform.GetChild(2).GetChild(1).GetComponent<UILabel>().text = Weapon.ShootTime + "";
            propertyTranform.GetChild(3).GetChild(0).GetComponent<UILabel>().text = "精度：";
            propertyTranform.GetChild(3).GetChild(1).GetComponent<UILabel>().text = Weapon.AccuracyMax + "";
            propertyTranform.GetChild(4).GetChild(0).GetComponent<UILabel>().text = "射程：";
            propertyTranform.GetChild(4).GetChild(1).GetComponent<UILabel>().text = Weapon.FireRange + "";
            propertyTranform.GetChild(5).GetChild(0).GetComponent<UILabel>().text = "控制：";
            propertyTranform.GetChild(5).GetChild(1).GetComponent<UILabel>().text = Weapon.SlowTime + "";
            propertyTranform.GetChild(6).GetChild(0).GetComponent<UILabel>().text = "装填速度：";
            propertyTranform.GetChild(6).GetChild(1).GetComponent<UILabel>().text = Weapon.ReloadTime + "";
            propertyTranform.GetChild(7).GetChild(0).GetComponent<UILabel>().text = "弹夹容量：";
            propertyTranform.GetChild(7).GetChild(1).GetComponent<UILabel>().text = Weapon.BoxAmmoCount + "";
            propertyTranform.GetChild(8).GetChild(0).GetComponent<UILabel>().text = "携弹总量：";
            propertyTranform.GetChild(8).GetChild(1).GetComponent<UILabel>().text = Weapon.BackAmmoCount + "";
        }

        private void BindEventLister()
        {
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;
            Utility.Utility.GetUIEventListener(m_buttonGroupTrans.Find("equlWeapons")).onClick = OnEqulMent;
            Utility.Utility.GetUIEventListener(m_buttonGroupTrans.Find("viewWeapons")).onClick = OnViewWeapon;
            Utility.Utility.GetUIEventListener(m_buttonGroupTrans.Find("sold")).onClick = OnSold; ;
            Utility.Utility.GetUIEventListener(m_buttonGroupTrans.Find("Deal")).onClick = OnDeal;
        }

        private void OnEqulMent(GameObject go)
        {

        }

        private void OnViewWeapon(GameObject go)
        {

        }

        private void OnSold(GameObject go)
        {
            this.CloseDialog();
            DialogMgr.Load(DialogType.ConfirmToSold);
            //出售的对象  和 价格
            DialogMgr.CurrentDialog.ShowCommonDialog(this.m_currentArgs);
        }

        private void OnDeal(GameObject go)
        {
            this.CloseDialog();
            ItemBase item = (ItemBase)this.m_currentArgs[0];
            if (item.IsBind)
            {
                Utility.Utility.NotifyStr("当前物品已绑定，不能进行交易！！");
                return;
            }
            //进入交易界面
            DialogMgr.Load(DialogType.CondsignForSaleDialog);
            DialogMgr.CurrentDialog.ShowCommonDialog(m_currentArgs);
        }

        //--------------------------------------
        //public
        //--------------------------------------

        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("bottom"));
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
