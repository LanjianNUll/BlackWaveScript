//******************************************************************
// File Name:					OnSoldItemDialogUI
// Description:					OnSoldItemDialogUI class 
// Author:						lanjian
// Date:						2/15/2017 2:26:36 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
using FW.Item;
namespace FW.UI
{
    /// <summary>
    /// 寄售界面  这个用的是和手雷，其他同一个prefabs  所以 代码大抵都相同
    /// </summary>
    class OnSoldItemDialogUI
    {
        private static OnSoldItemDialogUI sm_OSDialogUI;
        public static OnSoldItemDialogUI Instance
        {
            get
            {
                if (sm_OSDialogUI == null) sm_OSDialogUI = new OnSoldItemDialogUI();
                return sm_OSDialogUI;
            }
        }
        private GameObject m_OnSoldDialogBox;
        private EquipmentBase m_currentEquipmentBase;
        private ItemType m_whichComefrom;

        private UIInput m_pirceInput;
        private UIInput m_soldnumInput;
        private int m_times = 1;
        private bool m_inputNumError = false;
        private UILabel m_handCharge;

        //--------------------------------------
        //private 
        //--------------------------------------
        private void OpenDialog()
        {
            NGUITools.SetActive(m_OnSoldDialogBox, true);
        }

        private void CloseDialog()
        {
            NGUITools.SetActive(m_OnSoldDialogBox, false);
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
                DialogMgr.Load(DialogType.ConfirmPutUp);
                DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(m_currentEquipmentBase,price,count,this.m_times));
                this.CloseDialog();
            }
            else
            {
                Utility.Utility.NotifyStr("你输入的数量有错误！！！");
                return;
            }
            //ForSaleDialogUI.Instance.CurrentRootGO = PanelMgr.CurrPanel.RootObj;
            //ForSaleDialogUI.Instance.OpenDialog(10,"确定是否上架！上架后将移除背包");
        }

        private void OnBackBtn(GameObject go)
        {
            this.CloseDialog();
            //回到原界面(配件 还是武器)
            if(m_whichComefrom.Equals(ItemType.Weapon))
                WeaponDetailsDialogUI.Instance.ShowAndRefresh();
            if (m_whichComefrom.Equals(ItemType.Accessory))
                PartsDetailsDialogUI.Instance.ShowAndRefresh();
        }

        private void FillDataToUI()
        {
            Transform top = this.m_OnSoldDialogBox.transform.Find("top");
            string iconPath = "";
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Weapon)
                 iconPath = Utility.ConstantValue.WeaponIcon;
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Accessory)
                iconPath = Utility.ConstantValue.PartIcon;
            Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + m_currentEquipmentBase.Icon + Utility.ConstantValue.UpEndPath);
            top.GetChild(2).GetComponent<UITexture>().SetRect(-texture.width / 2, -texture.height / 2, texture.width, texture.height);
            top.GetChild(2).GetComponent<UITexture>().mainTexture = texture;

            Transform middle = this.m_OnSoldDialogBox.transform.Find("middle");
            middle.Find("name").GetComponent<UILabel>().text = m_currentEquipmentBase.Name;
            middle.Find("level").GetComponent<UILabel>().text = m_currentEquipmentBase.Levellimit.ToString();
            NGUITools.SetActive(middle.Find("desc").gameObject, false);
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Weapon)
            {
                FillWeaponPro(middle, (WeaponBase)m_currentEquipmentBase);
                middle.Find("type").GetComponent<UILabel>().text = "武器";
            }
            if (m_currentEquipmentBase.ItemType == Item.ItemType.Accessory)
            {
                FillAccessoryPro(middle.Find("proGroup"), (AccessoryBase)m_currentEquipmentBase);
                middle.Find("type").GetComponent<UILabel>().text = "配件";
            }
            Transform bottom = this.m_OnSoldDialogBox.transform.Find("bottom");
            m_pirceInput = bottom.Find("priceInput").GetComponent<UIInput>();
            m_soldnumInput = bottom.Find("soldnum").GetComponent<UIInput>();
            m_handCharge = bottom.Find("servicefee").GetComponent<UILabel>();
            m_pirceInput.value = "1";
            m_soldnumInput.value = "1";
            //手续费
            m_handCharge.text = Utility.ConstantValue.HandCharge + "";
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
        public void Hide()
        {
            this.CloseDialog();
        }
        public void ShowOnSoldDialogBoxUI(EquipmentBase currentEquipmentBase)
        {
            m_whichComefrom = currentEquipmentBase.ItemType;
            m_currentEquipmentBase = currentEquipmentBase;
            Debug.Log("这个装备的类型"+m_currentEquipmentBase.ItemType.ToString());

            if (m_OnSoldDialogBox == null)
            {
                Transform t = PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("ConsignForSaleDialog");
                if (t != null)
                    m_OnSoldDialogBox = t.gameObject;
                if (m_OnSoldDialogBox == null)
                {
                    //string EqulPartDialogUIpath = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/SoldBox";
                    string EqulPartDialogUIpath = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/ConsignForSaleDialog";
                    m_OnSoldDialogBox = Utility.Utility.GetPrefabGameObject(EqulPartDialogUIpath,
                        "ConsignForSaleDialog", PanelMgr.CurrPanel.RootObj.transform.Find("center"));
                }
            }
            this.OpenDialog();
            this.FillDataToUI();

            //返回按钮
            Utility.Utility.GetUIEventListener(m_OnSoldDialogBox.transform.Find("BackBtn").gameObject).onClick = OnBackBtn;
            Transform bottom = m_OnSoldDialogBox.transform.Find("bottom");
            Transform buttonGroupTra = bottom.Find("buttonGroup");
            Utility.Utility.GetUIEventListener(buttonGroupTra.Find("12Hours").gameObject).onClick = On12HoursBtn;
            Utility.Utility.GetUIEventListener(buttonGroupTra.Find("24Hours").gameObject).onClick = On24HoursBtn;
            Utility.Utility.GetUIEventListener(buttonGroupTra.Find("48Hours").gameObject).onClick = On48HoursBtn;
            Utility.Utility.GetUIEventListener(buttonGroupTra.Find("stateBtn").gameObject).onClick = OnListingBtn;
        }
    }
}
