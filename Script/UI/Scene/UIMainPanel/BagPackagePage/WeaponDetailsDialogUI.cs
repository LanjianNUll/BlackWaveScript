//******************************************************************
// File Name:					WeaponDetailsDialogUI
// Description:					WeaponDetailsDialogUI class 
// Author:						lanjian
// Date:						2/6/2017 10:12:44 AM
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
    class WeaponDetailsDialogUI
    {
        private static WeaponDetailsDialogUI sm_WDDialogUI;
        public static WeaponDetailsDialogUI Instance
        {
            get
            {
                if (sm_WDDialogUI == null) sm_WDDialogUI = new WeaponDetailsDialogUI();
                return sm_WDDialogUI;
            }
        }

        private GameObject m_DiglogBox;
        public GameObject CurrentItem;
        //当前装备
        public WeaponBase CurrentWeapon;
        public bool NoDoInstallOr;              //装备界面是否操作了

        private bool isEqulOrUnEqul = true;
        //当前的parts
        private Dictionary<AccessoryType, AccessoryBase> m_Parts = new Dictionary<AccessoryType, AccessoryBase>();
        //private bool m_currentIsEqulOrNot = false;

        //--------------------------------------
        //private
        //--------------------------------------
        //交易
        private void OnDealWeapon(GameObject go)
        {
           // 绑定的武器不能交易
            if (CurrentWeapon.IsBind)
            {
                Utility.Utility.NotifyStr("当前物品已绑定，不能进行交易！！");
                return;
            }
            CloseDialog();
            //显示交易配件的界面
            OnSoldItemDialogUI.Instance.ShowOnSoldDialogBoxUI(CurrentWeapon);
        }

        //查看武器
        private void OnViewWeapon(GameObject go)
        {

        }

        //装备武器
        private void OnEqulWeapon(GameObject go)
        {
            //装备武器到role上  不做 
            //CurrentWeapon.PCEquipToRole();
        }

        //出售武器
        private void OnSoldWeapon(GameObject go)
        {
            DialogMgr.Load(DialogType.ConfirmToSold);
            //出售的对象  和 价格
            DialogMgr.CurrentDialog.ShowCommonDialog(new Event.EventArg(CurrentWeapon, 1000));
            //ForSaleDialogUI.Instance.CurrentRootGO = PanelMgr.CurrPanel.RootObj;
            //ForSaleDialogUI.Instance.OpenDialog(0,"出售后，该物品会消失。确定以XXX现金出售此物品吗？");
        }

        private void OnShowEqulParts(GameObject go)
        {
            //Muzzle = 1,         //枪口
            //Barrel,             //枪管
            //Sight,              //瞄具
            //Maganize,           //弹夹
            //MuzzleSuit,         //枪管套件
            //Trigger,   
            //Debug.Log("查看枪口装备情况"+go.name);
            this.CloseDialog();
            PartEqulDialogUI.Instance.OnShowEqulParts(go);
            isEqulOrUnEqul = true;
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
            //隐藏改装窗口
            NGUITools.SetActive(m_DiglogBox.transform.GetChild(3).gameObject, false);

            if (!NoDoInstallOr)
            {
                //刷新数据
                this.GetAllParts();
                NoDoInstallOr = false;
            }
            
        }

        private void BackMainWeapon(GameObject go)
        {
            if (CurrentWeapon.IsModify())
            {
                NGUITools.SetActive(m_DiglogBox.transform.GetChild(3).gameObject, true);
                
                if (CurrentWeapon.TotalModifyPrice() > Role.Role.Instance().Cash)
                {
                    m_DiglogBox.transform.GetChild(3).GetChild(1).GetComponent<UILabel>().text = "现金不足!";
                    Utility.Utility.GetUIEventListener(m_DiglogBox.transform.GetChild(3).GetChild(2)).onClick = OnCancelModify;
                    Utility.Utility.GetUIEventListener(m_DiglogBox.transform.GetChild(3).GetChild(3)).onClick = OnCancelModify;
                }
                else
                {
                    m_DiglogBox.transform.GetChild(3).GetChild(1).GetComponent<UILabel>().text = "本次改装将消耗"+ 
                        CurrentWeapon.TotalModifyPrice() + "现金。确定改装吗？";
                    Utility.Utility.GetUIEventListener(m_DiglogBox.transform.GetChild(3).GetChild(2)).onClick = OnComfirmModify;
                    Utility.Utility.GetUIEventListener(m_DiglogBox.transform.GetChild(3).GetChild(3)).onClick = OnCancelModify;
                }
            }
            else
            {
                this.Back();
            }
        }

        private void Back()
        {
            CloseDialog();
            NGUITools.SetActive(CurrentItem.transform.GetChild(0).gameObject, true);
            NGUITools.SetActive(CurrentItem.transform.GetChild(1).gameObject, true);
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("bottom").gameObject, true);
            //可以滑动了
            PanelMgr.CurrPanel.IsAllowHorMove(true);
            //重置
            isEqulOrUnEqul = true;
            this.m_Parts.Clear();
            //注销
            FW.Event.FWEvent.Instance.UnRegist(FW.Event.EventID.Weapon_AccessoryChanged, this.OnReciveUpdateUI);
        }

        //确定返回
        public void OnReciveUpdateUI(FW.Event.EventArg args)
        {
            this.Back();
            Utility.Utility.NotifyStr("配件装备成功！！！");
        }

        private void OnComfirmModify(GameObject go)
        {
            CurrentWeapon.CommitModify();
        }

        private void OnCancelModify(GameObject go)
        {
            //清理修改的
            this.CurrentWeapon.clearModify();
            this.Back();
        }

        private WeaponBase FindWeaponInAll(string wID)
        {
            List<WeaponBase> m_MainWeaponList = Role.Role.Instance().KitBag.GetWeapons(WeaponType.Main);
            List<WeaponBase> m_SecondlyWeaponList = Role.Role.Instance().KitBag.GetWeapons(WeaponType.Second);
            List<WeaponBase> m_MeleeWeaponList = Role.Role.Instance().KitBag.GetWeapons(WeaponType.Melee);
            for (int i = 0; i < m_MainWeaponList.Count; i++)
            {
                if (m_MainWeaponList[i].ID.Equals(wID))
                    CurrentWeapon = m_MainWeaponList[i];
            }
            for (int i = 0; i < m_SecondlyWeaponList.Count; i++)
            {
                if (m_SecondlyWeaponList[i].ID.Equals(wID))
                    CurrentWeapon = m_SecondlyWeaponList[i];
            }
            for (int i = 0; i < m_MeleeWeaponList.Count; i++)
            {
                if (m_MeleeWeaponList[i].ID.Equals(wID))
                    CurrentWeapon = m_MeleeWeaponList[i];
            }
            return CurrentWeapon;
        }

        private AccessoryType GetAccType(string partsTypeName)
        {
            AccessoryType whichType = AccessoryType.Unknow;
            switch (partsTypeName)
            {
                case "Muzzle": whichType = AccessoryType.Muzzle; break;
                case "Barrel": whichType = AccessoryType.Barrel; break;
                case "Sight": whichType = AccessoryType.Sight; break;
                case "Maganize": whichType = AccessoryType.Maganize; break;
                case "MuzzleSuit": whichType = AccessoryType.MuzzleSuit; break;
                case "Trigger": whichType = AccessoryType.Trigger; break;
                default:
                    break;
            }
            return whichType;
        }

        //进入该界面获取武器已经装备的武器配件
        private void GetAllParts()
        {
            this.m_Parts.Clear();
            this.m_Parts.Add(AccessoryType.Muzzle,CurrentWeapon.GetAccessory(AccessoryType.Muzzle));
            this.m_Parts.Add(AccessoryType.Barrel,CurrentWeapon.GetAccessory(AccessoryType.Barrel));
            this.m_Parts.Add(AccessoryType.Sight,CurrentWeapon.GetAccessory(AccessoryType.Sight));
            this.m_Parts.Add(AccessoryType.Maganize,CurrentWeapon.GetAccessory(AccessoryType.Maganize));
            this.m_Parts.Add(AccessoryType.MuzzleSuit,CurrentWeapon.GetAccessory(AccessoryType.MuzzleSuit));
            this.m_Parts.Add(AccessoryType.Trigger,CurrentWeapon.GetAccessory(AccessoryType.Trigger));
            FillParts(m_DiglogBox.transform.GetChild(2));
        }

        private void FillDateTips(AccessoryBase acccessory, Transform item,params string[] list)
        {
            string partIconStr = null;
            if(acccessory!=null)
                partIconStr = acccessory.PartsIcon;
            Utility.Utility.GetUIEventListener(item.GetChild(0)
                .gameObject).onClick = OnShowEqulParts;
            if (partIconStr != null)
            {
                Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.PartIcon+"/" + partIconStr + "_up");
                item.GetChild(0).GetChild(1).GetComponent<UITexture>().mainTexture = texture;
            }
            //如果是空 就未装备该部位的配件  显示文字
            if (list.Length == 0)
            {
                NGUITools.SetActive(item.GetChild(0).GetChild(0).gameObject, true);
                NGUITools.SetActive(item.GetChild(0).GetChild(1).gameObject, false);
            }
            else
            {
                NGUITools.SetActive(item.GetChild(0).GetChild(0).gameObject, false);
                NGUITools.SetActive(item.GetChild(0).GetChild(1).gameObject, true);
            }
            NGUITools.SetActive(item.GetChild(0).GetChild(2).gameObject, false);
            if (acccessory != null && !acccessory.IsBind)
                NGUITools.SetActive(item.GetChild(0).GetChild(2).gameObject, true);
            //现将全部的tips隐藏
            for (int i = 1; i < item.childCount; i++)
            {
                NGUITools.SetActive(item.GetChild(i).gameObject, false);
            }
            for (int i = 0,j = 1; i < list.Length; i+=2,j++)
            {
                NGUITools.SetActive(item.GetChild(j).gameObject, true);
                item.GetChild(j).GetChild(0).GetComponent<UILabel>().text = list[i];
                item.GetChild(j).GetChild(1).GetComponent<UILabel>().text = list[i+1];
                NGUITools.SetActive(item.GetChild(j).GetChild(2).gameObject, false);
                NGUITools.SetActive(item.GetChild(j).GetChild(3).gameObject, false);
            }
        }

        private void FillWeaponPro()
        {
            //top
            Transform topTranform = m_DiglogBox.transform.GetChild(0);
            topTranform.Find("gunName").GetComponent<UILabel>().text = CurrentWeapon.Name;
            topTranform.Find("gunLevel").GetComponent<UILabel>().text = "LV"+ CurrentWeapon.Levellimit;
            topTranform.Find("gunCategroy").GetComponent<UILabel>().text = CurrentWeapon.FunctionType;
            Transform propertyTranform = topTranform.Find("property");
            //填充属性
            propertyTranform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = "伤害：";
            propertyTranform.GetChild(0).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.Injure + "";
            propertyTranform.GetChild(1).GetChild(0).GetComponent<UILabel>().text = "破甲：";
            propertyTranform.GetChild(1).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.SunderArmor + "";
            propertyTranform.GetChild(2).GetChild(0).GetComponent<UILabel>().text = "射速：";
            propertyTranform.GetChild(2).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.ShootTime + "";
            propertyTranform.GetChild(3).GetChild(0).GetComponent<UILabel>().text = "精度：";
            propertyTranform.GetChild(3).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.AccuracyMax + "";
            propertyTranform.GetChild(4).GetChild(0).GetComponent<UILabel>().text = "射程：";
            propertyTranform.GetChild(4).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.FireRange + "";
            propertyTranform.GetChild(5).GetChild(0).GetComponent<UILabel>().text = "控制：";
            propertyTranform.GetChild(5).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.SlowTime + "";
            propertyTranform.GetChild(6).GetChild(0).GetComponent<UILabel>().text = "装填速度：";
            propertyTranform.GetChild(6).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.ReloadTime + "";
            propertyTranform.GetChild(7).GetChild(0).GetComponent<UILabel>().text = "弹夹容量：";
            propertyTranform.GetChild(7).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.BoxAmmoCount + "";
            propertyTranform.GetChild(8).GetChild(0).GetComponent<UILabel>().text = "携弹总量：";
            propertyTranform.GetChild(8).GetChild(1).GetComponent<UILabel>().text = CurrentWeapon.BackAmmoCount + "";

            Utility.Utility.GetUIEventListener(topTranform.Find("buttonGroup").GetChild(0).gameObject)
                .onClick = OnDealWeapon;
            Utility.Utility.GetUIEventListener(topTranform.Find("buttonGroup").GetChild(1).gameObject)
                .onClick = OnViewWeapon;
            Utility.Utility.GetUIEventListener(topTranform.Find("buttonGroup").GetChild(2).gameObject)
                .onClick = OnEqulWeapon;
            Utility.Utility.GetUIEventListener(topTranform.Find("buttonGroup").GetChild(3).gameObject)
                .onClick = OnSoldWeapon;
            //middle 图片
            Texture texture = ResMgr.ResLoad.Load<Texture>(Utility.ConstantValue.WeaponIcon + "/"+ CurrentWeapon.BagIcon + "_down");
            m_DiglogBox.transform.GetChild(1).Find("gunPic").GetComponent<UITexture>().mainTexture = texture;
        }
        //填充6个配件
        private void FillParts(Transform bottomTransform)
        {
            List<AccessoryBase> parts = new List<AccessoryBase>();
            parts.Add(this.m_Parts[AccessoryType.Muzzle]);
            parts.Add(this.m_Parts[AccessoryType.Barrel]);
            parts.Add(this.m_Parts[AccessoryType.Sight]);
            parts.Add(this.m_Parts[AccessoryType.Maganize]);
            parts.Add(this.m_Parts[AccessoryType.MuzzleSuit]);
            parts.Add(this.m_Parts[AccessoryType.Trigger]);
            //将配件的tip传进去   如果没有就不显示
            if (parts[0] != null)
                FillDateTips((MuzzleAccessory)parts[0], bottomTransform.GetChild(0), "伤害", ((MuzzleAccessory)parts[0]).Damage + "", "精准",
                 ((MuzzleAccessory)parts[0]).Accuracyrenew + "", "后坐力", ((MuzzleAccessory)parts[0]).Backpower + "", "重量",
                 ((MuzzleAccessory)parts[0]).Gravity + "");
            else
                FillDateTips(null, bottomTransform.GetChild(0));
            if (parts[1] != null)
                FillDateTips((BarrelAccessory)parts[1], bottomTransform.GetChild(1),
                "破甲", ((BarrelAccessory)parts[1]).Sunder + "", "穿透", ((BarrelAccessory)parts[1]).Pirerce + "",
                "射程", ((BarrelAccessory)parts[1]).Range + "",
                "停滞时间", ((BarrelAccessory)parts[1]).SlowTime1 + "", "重量", ((BarrelAccessory)parts[1]).Gravity + "");
            else
                FillDateTips(null, bottomTransform.GetChild(1));
            if (parts[2] != null)
                FillDateTips((SighyAccessory)parts[2], bottomTransform.GetChild(2),
                "精准", ((SighyAccessory)parts[2]).Accuracy + "", "开火精准", ((SighyAccessory)parts[2]).Accuracybyfire + "",
                "移动精准", ((SighyAccessory)parts[2]).Accuracybymove + "",
                "暴击", ((SighyAccessory)parts[2]).Critical + "", "重量", ((SighyAccessory)parts[2]).Gravity + "");
            else
                FillDateTips(null, bottomTransform.GetChild(2));
            if (parts[3] != null)
                FillDateTips((MaganizeAccessory)parts[3], bottomTransform.GetChild(3),
                    "弹夹量", ((MaganizeAccessory)parts[3]).BoxAmmoCount1 + "", "载弹量", ((MaganizeAccessory)parts[3]).BackAmmoCount1 + "",
                    "射速", ((MaganizeAccessory)parts[3]).ShootTime1 + "", "重量", ((MaganizeAccessory)parts[3]).Gravity + "");
            else
                FillDateTips(null, bottomTransform.GetChild(3));
            if (parts[4] != null)
                FillDateTips((MuzzleSuitAccessory)parts[4], bottomTransform.GetChild(4),
                    "精准", ((MuzzleSuitAccessory)parts[4]).Accuracy + "", "伤害", ((MuzzleSuitAccessory)parts[4]).Damage + "",
                    "破甲", ((MuzzleSuitAccessory)parts[4]).Sunder + "",
                    "射程", ((MuzzleSuitAccessory)parts[4]).Range + "", "重量", ((MuzzleSuitAccessory)parts[4]).Gravity + "");
            else
                FillDateTips(null, bottomTransform.GetChild(4));
            if (parts[5] != null)
                FillDateTips((TriggerAccessory)parts[5], bottomTransform.GetChild(5),
                    "攻击间隔", ((TriggerAccessory)parts[5]).ShootTime1 + "", "装填时间", ((TriggerAccessory)parts[5]).Reloadtime + "",
                    "后坐力", ((TriggerAccessory)parts[5]).Backpower + "", "重量", ((TriggerAccessory)parts[5]).Gravity + "");
            else
                FillDateTips(null, bottomTransform.GetChild(5));
        }
        //--------------------------------------
        //public
        //--------------------------------------
        //返回武器列表
        public void BackWeaponList()
        {
            this.BackMainWeapon(null);
        }
        
        //显示和重新刷新数据
        public void ShowAndRefresh()
        {
            this.OpenDialog();
        }

        public void FillDigLogBox(string gunID)
        {
            Utility.Utility.GetUIEventListener(
                PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("backMainWeapon")
                .gameObject).onClick = BackMainWeapon;
            CurrentWeapon = null;
            CurrentWeapon = FindWeaponInAll(gunID);
            //在主武器，副武器，近战中找
            if (CurrentWeapon == null)
            {
                Debug.Log("未获取到任何数据");
                NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("backMainWeapon")
                    .gameObject, true);
                return;
            }
            //武器详情
            if (m_DiglogBox == null)
            {
                Transform t = CurrentItem.transform.Find("diglogBox");
                if (t != null)
                    m_DiglogBox = t.gameObject;
                if (m_DiglogBox == null)
                {
                    //用resourec加载过来
                    string weaponDialogUIpath = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/WEdiglogBox";
                    m_DiglogBox = Utility.Utility.GetPrefabGameObject(weaponDialogUIpath,
                        "diglogBox", CurrentItem.transform);
                }
            }
            FillWeaponPro();
            //bottom
            //Transform bottomTransform = m_DiglogBox.transform.GetChild(2);
            //获取各个部位的配件
            if(isEqulOrUnEqul)
            {
                GetAllParts();
            }
            FillParts(m_DiglogBox.transform.GetChild(2));
            //显示
            this.OpenDialog();
            //主武器和副武器才有配件  
            if (CurrentWeapon.WeaponType == WeaponType.Melee)
            {
                NGUITools.SetActive(m_DiglogBox.transform.Find("bottom").gameObject,false);
            }
        }
    }
}
