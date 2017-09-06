//******************************************************************
// File Name:					ConsignForSaleDialogUI
// Description:					ConsignForSaleDialogUI class 
// Author:						lanjian
// Date:						3/8/2017 4:49:37 PM
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
    //寄售（武器交易）  这个用的是和武器 配件  一个prefabs  所以 代码大抵都相同
    class ConsignForSaleDialogUI:DialogBaseUI
    {
        protected ConsignForSaleDialogUI()
        {
            this.m_resName = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/ConsignForSaleDialog";
            this.m_DType = DialogType.CondsignForSaleDialog;

        }

        public static ConsignForSaleDialogUI Create()
        {
            return new ConsignForSaleDialogUI();
        }


        private ItemBase m_currentEquipmentBase;
        private UIInput m_pirceInput;
        private UIInput m_soldnumInput;
        private int m_times = 1;   //默认是24小时  1 24 小时  2 48小时  3 72小时
        private UILabel m_handCharge;
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
            Utility.Utility.GetUIEventListener(m_DialogUIGo.transform.Find("BackBtn")).onClick = OnBack;

            Transform bottom = this.m_DialogUIGo.transform.Find("bottom");
            Transform buttonGroupTra = bottom.Find("buttonGroup");
            Utility.Utility.GetUIEventListener(buttonGroupTra.Find("12Hours").gameObject).onClick = On12HoursBtn;
            Utility.Utility.GetUIEventListener(buttonGroupTra.Find("24Hours").gameObject).onClick = On24HoursBtn;
            Utility.Utility.GetUIEventListener(buttonGroupTra.Find("48Hours").gameObject).onClick = On48HoursBtn;
            Utility.Utility.GetUIEventListener(buttonGroupTra.Find("stateBtn").gameObject).onClick = OnListingBtn;

        }

        private void On12HoursBtn(GameObject go)
        {
            m_times = 1;
        }

        private void On24HoursBtn(GameObject go)
        {
            m_times = 2;
        }

        private void On48HoursBtn(GameObject go)
        {
            m_times = 3;
        }

        //上架 
        private void OnListingBtn(GameObject go)
        {
            int price = 1;
            int count = 1;
            if (int.TryParse(m_pirceInput.value, out price) && int.TryParse(m_soldnumInput.value, out count))
            {
                this.CloseDialog();
                //检验下价格和数量是否合法
                DialogMgr.Load(DialogType.ConfirmPutUp);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(m_currentEquipmentBase, price, count, this.m_times));
            }
            else
            {
                Utility.Utility.NotifyStr("你输入的数量有错误！！！");
                return;
            }
        }

        private void FillDataUI(FW.Event.EventArg args)
        {
            Transform top = this.m_DialogUIGo.transform.Find("top");
            string iconPath = "";
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Commodity)
                iconPath = Utility.ConstantValue.CommodityIcon;
            
            Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + m_currentEquipmentBase.Icon + Utility.ConstantValue.UpEndPath);
            top.GetChild(2).GetComponent<UITexture>().SetRect(-texture.width / 2, -texture.height / 2, texture.width, texture.height);
            top.GetChild(2).GetComponent<UITexture>().mainTexture = texture;

            Transform middle = this.m_DialogUIGo.transform.Find("middle");
            middle.Find("name").GetComponent<UILabel>().text = m_currentEquipmentBase.Name;
            middle.Find("level").GetComponent<UILabel>().text = m_currentEquipmentBase.Levellimit.ToString();
            NGUITools.SetActive(middle.Find("desc").gameObject, false);
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Weapon)
                FillWeaponPro(middle, (WeaponBase)m_currentEquipmentBase);
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Accessory)
                FillAccessoryPro(middle.Find("proGroup"), (AccessoryBase)m_currentEquipmentBase);
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Commodity)
            {
                //这边传过来的都是道具 这里写了武器只是为了 后面改动 武器的做
                NGUITools.SetActive(middle.Find("desc").gameObject, true);
                NGUITools.SetActive(middle.Find("proGroup").gameObject, false);
                CommodityBase coom = (CommodityBase)m_currentEquipmentBase;
                middle.Find("desc").GetComponent<UILabel>().text = coom.Desc;
                middle.Find("type").GetComponent<UILabel>().text = "道具";
            }
            Transform bottom = this.m_DialogUIGo.transform.Find("bottom");
            m_pirceInput = bottom.Find("priceInput").GetComponent<UIInput>();
            m_soldnumInput = bottom.Find("soldnum").GetComponent<UIInput>();
            m_handCharge = bottom.Find("servicefee").GetComponent<UILabel>();
            m_pirceInput.value = "1";
            m_soldnumInput.value = "1";
            //手续费
            m_handCharge.text = Utility.ConstantValue.HandCharge+"";
            //武器配件  不能改变数量  道具可以
            NGUITools.SetActive(bottom.Find("soldnum").gameObject,false);
            NGUITools.SetActive(bottom.Find("count").gameObject, false);
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Commodity)
            {
                NGUITools.SetActive( bottom.Find("soldnum").gameObject, true);
            }
            else
            {
                NGUITools.SetActive(bottom.Find("count").gameObject, true);
            }
        }

        //填充武器的属性
        protected void FillWeaponPro(Transform content, WeaponBase weapon)
        {
            Transform propertyTranform = content.Find("proGroup");
            //填充属性
            propertyTranform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = "伤害：";
            propertyTranform.GetChild(0).GetChild(1).GetComponent<UILabel>().text = weapon.Injure + "";
            propertyTranform.GetChild(1).GetChild(0).GetComponent<UILabel>().text = "破甲：";
            propertyTranform.GetChild(1).GetChild(1).GetComponent<UILabel>().text = weapon.SunderArmor + "";
            propertyTranform.GetChild(2).GetChild(0).GetComponent<UILabel>().text = "射速：";
            propertyTranform.GetChild(2).GetChild(1).GetComponent<UILabel>().text = weapon.ShootTime + "";
            propertyTranform.GetChild(3).GetChild(0).GetComponent<UILabel>().text = "精度：";
            propertyTranform.GetChild(3).GetChild(1).GetComponent<UILabel>().text = weapon.AccuracyMax + "";
            propertyTranform.GetChild(4).GetChild(0).GetComponent<UILabel>().text = "射程：";
            propertyTranform.GetChild(4).GetChild(1).GetComponent<UILabel>().text = weapon.FireRange + "";
            propertyTranform.GetChild(5).GetChild(0).GetComponent<UILabel>().text = "控制：";
            propertyTranform.GetChild(5).GetChild(1).GetComponent<UILabel>().text = weapon.SlowTime + "";
            propertyTranform.GetChild(6).GetChild(0).GetComponent<UILabel>().text = "装填速度：";
            propertyTranform.GetChild(6).GetChild(1).GetComponent<UILabel>().text = weapon.ReloadTime + "";
            propertyTranform.GetChild(7).GetChild(0).GetComponent<UILabel>().text = "弹夹容量：";
            propertyTranform.GetChild(7).GetChild(1).GetComponent<UILabel>().text = weapon.BoxAmmoCount + "";
            propertyTranform.GetChild(8).GetChild(0).GetComponent<UILabel>().text = "携弹总量：";
            propertyTranform.GetChild(8).GetChild(1).GetComponent<UILabel>().text = weapon.BackAmmoCount + "";
        }

        //填充配件的属性
        protected void FillAccessoryPro(Transform propertyTranform, AccessoryBase m_currentPart)
        {
            if (m_currentPart.Type == AccessoryType.Muzzle)
            {
                MuzzleAccessory muzzle = (MuzzleAccessory)m_currentPart;
                string[] tips = { "单发伤害", "精准", "后坐力", "重量" };
                string[] baseStr = { muzzle.Damage.ToString(), muzzle.Accuracyrenew.ToString(),
                    muzzle.Backpower.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (m_currentPart.Type == AccessoryType.Barrel)
            {
                BarrelAccessory muzzle = (BarrelAccessory)m_currentPart;
                string[] tips = { "破甲", "穿透", "射程", "停滞时间", "重量" };
                string[] baseStr = { muzzle.Sunder.ToString(), muzzle.Pirerce.ToString(),
                    muzzle.Range.ToString(), muzzle.SlowTime1.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (m_currentPart.Type == AccessoryType.Sight)
            {
                SighyAccessory muzzle = (SighyAccessory)m_currentPart;
                string[] tips = { "精准", "开火精准", "移动精准", "暴击", "重量" };
                string[] baseStr = { muzzle.Accuracy.ToString(), muzzle.Accuracybyfire.ToString(),
                    muzzle.Accuracybymove.ToString(),muzzle.Critical.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (m_currentPart.Type == AccessoryType.Maganize)
            {
                MaganizeAccessory muzzle = (MaganizeAccessory)m_currentPart;
                string[] tips = { "弹夹量", "载弹量", "射速", "重量" };
                string[] baseStr = { muzzle.BoxAmmoCount1.ToString(), muzzle.BackAmmoCount1.ToString(),
                    muzzle.ShootTime1.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (m_currentPart.Type == AccessoryType.MuzzleSuit)
            {
                MuzzleSuitAccessory muzzle = (MuzzleSuitAccessory)m_currentPart;
                string[] tips = { "精准", "伤害", "破甲", "射程", "重量" };
                string[] baseStr = { muzzle.Accuracy.ToString(), muzzle.Damage.ToString(),
                    muzzle.Sunder.ToString(), muzzle.Range.ToString(),
                    muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (m_currentPart.Type == AccessoryType.Trigger)
            {
                TriggerAccessory muzzle = (TriggerAccessory)m_currentPart;
                string[] tips = { "射速", "装填时间", "后坐力", "重量" };
                string[] baseStr = { muzzle.ShootTime1.ToString(), muzzle.Reloadtime.ToString(),
                    muzzle.Backpower.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
        }

        protected void DisPlayPro(Transform propertyTranform, string[] tips, string[] baseStr)
        {
            //隐藏
            for (int i = 0; i < propertyTranform.childCount; i++)
            {
                NGUITools.SetActive(propertyTranform.GetChild(i).gameObject, false);
            }
            for (int i = 0; i < tips.Length; i++)
            {
                NGUITools.SetActive(propertyTranform.GetChild(i).gameObject, true);
                propertyTranform.GetChild(i).GetChild(0).GetComponent<UILabel>().text = tips[i];
                propertyTranform.GetChild(i).GetChild(1).GetComponent<UILabel>().text = baseStr[i];
            }
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public override void GetNeedHideGo()
        {
            base.GetNeedHideGo();
            this.m_needHideGo.Add(PanelMgr.CurrPanel.RootObj.transform.Find("bottom"));
        }

        //返回按钮
        public override void OnBack(GameObject go)
        {
            //返回详情界面
            DialogMgr.Load(DialogType.HandBombAndCommdityDetail);
            DialogMgr.CurrentDialog.ShowCommonDialog(this.m_currentArgs);
        }

        public override void ShowCommonDialog(FW.Event.EventArg args)
        {
            this.m_currentArgs = args;
            m_currentEquipmentBase = (ItemBase)this.m_currentArgs[0];
            this.GetDialogAbout();
            this.FillDataUI(args);
            this.OpenDialog();
        }
    }
}
