//******************************************************************
// File Name:					PartsDetailsDialogUI
// Description:					PartsDetailsDialogUI class 
// Author:						lanjian
// Date:						2/8/2017 5:09:31 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using FW.Item;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    class PartsDetailsDialogUI
    {
        private static PartsDetailsDialogUI sm_PDialogUI;
        public static PartsDetailsDialogUI Instance
        {
            get
            {
                if (sm_PDialogUI == null) sm_PDialogUI = new PartsDetailsDialogUI();
                return sm_PDialogUI;
            }
        }
        //当前配件
        private AccessoryBase m_currentPart;
        private GameObject m_DiglogBox;
        public GameObject CurrentItem;
        private string m_currentCategroy;
        //--------------------------------------
        //private
        //--------------------------------------
        private void BackMainWeapon(GameObject go)
        {
            this.CloseDialog();
            NGUITools.SetActive(CurrentItem.transform.GetChild(0).gameObject, true);
            NGUITools.SetActive(CurrentItem.transform.GetChild(1).gameObject, true);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("bottom").gameObject, true);
            //可以滑动了
            PanelMgr.CurrPanel.IsAllowHorMove(true);
        }

        private void CloseDialog()
        {
            NGUITools.SetActive(m_DiglogBox, false);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("backMainWeapon")
               .gameObject, false);
        }

        private void OpenDialog()
        {
            NGUITools.SetActive(m_DiglogBox, true);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("backMainWeapon")
               .gameObject, true);
        }

        private AccessoryBase FindPartInAll(string partID)
        {
            List<List<AccessoryBase>> partList = new List<List<AccessoryBase>>();
            partList.Add(Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Muzzle));
            partList.Add(Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Barrel));
            partList.Add(Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Sight));
            partList.Add(Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Maganize));
            partList.Add(Role.Role.Instance().KitBag.GetAccessory(AccessoryType.MuzzleSuit));
            partList.Add(Role.Role.Instance().KitBag.GetAccessory(AccessoryType.Trigger));
            //Debug.Log(partList.Count+"_88888--"+ partID + "88888_"+partList[0].Count);
            foreach (var itemList in partList)
            {
                foreach (var part in itemList)
                    {
                        if (part.ID.Equals(partID))
                        {
                            //Debug.Log("_88888--name" + part.Name);
                            return part;
                        }
                    }
            }
            return null;
        }

        private string GetPartTypeStr(AccessoryType aType)
        {
            string partTypeName = "UnKnow";
            switch (aType)
            {
                case AccessoryType.Muzzle: partTypeName = "Muzzle" ; m_currentCategroy = "枪口"; break;
                case AccessoryType.Barrel: partTypeName = "Barrel" ; m_currentCategroy = "枪管"; break;
                case AccessoryType.Sight: partTypeName = "Sight" ; m_currentCategroy = "瞄具"; break;
                case AccessoryType.Maganize: partTypeName = "Maganize" ; m_currentCategroy = "弹夹"; break;
                case AccessoryType.MuzzleSuit: partTypeName = "MuzzleSuit" ; m_currentCategroy = "枪管套件"; break;
                case AccessoryType.Trigger: partTypeName = "Trigger" ; m_currentCategroy = "扳机套件"; break;
                default:
                    break;
            }
            return partTypeName;
        }

        //交易配件
        private void OnForSaleWeapon(GameObject go)
        {
            //绑定的武器不能交易
            if (m_currentPart.IsBind)
            {
                Utility.Utility.NotifyStr("当前物品已绑定，不能进行交易！！");
                return;
            }
            //显示出售界面
            this.CloseDialog();
            OnSoldItemDialogUI.Instance.ShowOnSoldDialogBoxUI(m_currentPart);
        }

        //出售配件
        private void OnSoldWeapon(GameObject go)
        {
            DialogMgr.Load(DialogType.ConfirmToSold);
            //出售的对象  和 价格
            DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(m_currentPart, 1000));
           
            //ForSaleDialogUI.Instance.CurrentRootGO = PanelMgr.CurrPanel.RootObj;
            //ForSaleDialogUI.Instance.OpenDialog(1, "出售后，该物品会消失。确定以XXX现金出售此物品吗？");
        }

        private void DisPlayPro(Transform propertyTranform, string[] tips, string[] baseStr)
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
        //返回武器列表Accessory
        public void BackAccessoryList()
        {
            this.BackMainWeapon(null);
        }

        public void ShowAndRefresh()
        {
            this.OpenDialog();
        }

        public void FillDigLogBox(string partId)
        {
            PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("backMainWeapon")
                .gameObject.AddComponent<UIEventListener>().onClick = BackMainWeapon;
            m_currentPart = FindPartInAll(partId);
            //配件详情dialog
            if (m_DiglogBox == null)
            {
                Transform t = CurrentItem.transform.Find("diglogBox");
                if (t != null)
                    m_DiglogBox = t.gameObject;
                if (m_DiglogBox == null)
                {
                    //用resourec加载过来
                    string weaponDialogUIpath = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/PartdiglogBox";
                    m_DiglogBox = Utility.Utility.GetPrefabGameObject(weaponDialogUIpath,
                        "diglogBox", CurrentItem.transform);
                }
            }
            Transform topTranform = m_DiglogBox.transform.GetChild(0);
            Transform MiddleTranform = m_DiglogBox.transform.GetChild(1);
            if(m_currentPart !=null)
            {
                topTranform.Find("gunName").GetComponent<UILabel>().text = m_currentPart.Name;
                topTranform.Find("gunLevel").GetComponent<UILabel>().text = "LV"+ m_currentPart.Levellimit;
                topTranform.Find("gunCategroy").GetComponent<UILabel>().text = m_currentCategroy;
                Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.PartIcon +
                    "/" + m_currentPart.PartsIcon + "_down");
                MiddleTranform.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                string typeStr = GetPartTypeStr(m_currentPart.Type);
                topTranform.Find("gunCategroy").GetComponent<UILabel>().text = typeStr;
                Transform propertyTranform = topTranform.Find("property");
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
                //出售
                Utility.Utility.GetUIEventListener(topTranform.Find("buttonGroup").GetChild(0).gameObject)
                .onClick = OnSoldWeapon;
                //交易按钮 
                Utility.Utility.GetUIEventListener(topTranform.Find("buttonGroup").GetChild(1).gameObject)
                .onClick = OnForSaleWeapon;
                //显示
                this.OpenDialog();
            }
        }
    }
}
