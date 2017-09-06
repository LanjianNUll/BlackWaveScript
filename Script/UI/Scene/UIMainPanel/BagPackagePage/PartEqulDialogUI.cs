//******************************************************************
// File Name:					PartEqulDialogUI
// Description:					PartEqulDialogUI class 
// Author:						lanjian
// Date:						2/6/2017 10:36:09 AM
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
    class PartEqulDialogUI
    {
        private static PartEqulDialogUI sm_PEDialogUI;
        public static PartEqulDialogUI Instance
        {
            get
            {
                if (sm_PEDialogUI == null) sm_PEDialogUI = new PartEqulDialogUI();
                return sm_PEDialogUI;
            }
        }

        private GameObject m_EqulPartDialogBox;
        private List<AccessoryBase> m_ShowAccList;
        //上次装备的那个配件
        private AccessoryBase m_LastEqulPart;
        //当前选中的配件
        private AccessoryBase currentClickPart = null;
        private AccessoryType whichType = AccessoryType.Unknow;
        //--------------------------------------
        //private
        //--------------------------------------

        private void FillContent(Transform contentTran, AccessoryType wType,
            AccessoryBase clickPart,AccessoryBase lastequlPart)
        {
            
            if (wType == AccessoryType.Muzzle)
            {
                MuzzleAccessory muzzle = (MuzzleAccessory)clickPart;
                MuzzleAccessory Lastmuzzle = (MuzzleAccessory)lastequlPart;
                string[] tips = { "伤害", "精准", "后坐力", "重量" };
                string[] baseStr = { muzzle.Damage.ToString(), muzzle.Accuracyrenew.ToString(),
                    muzzle.Backpower.ToString(), muzzle.Gravity.ToString() };

                string[] addOrSub = null;
                if (Lastmuzzle != null)
                {
                    addOrSub = muzzle.Compare(Lastmuzzle);
                }
                DisplayPartPro(contentTran, tips, baseStr, addOrSub);
            }
            if (wType == AccessoryType.Barrel)
            {
                BarrelAccessory muzzle = (BarrelAccessory)clickPart;
                BarrelAccessory Lastmuzzle = (BarrelAccessory)lastequlPart;
                string[] tips = { "破甲", "穿透", "射程", "停滞时间", "重量" };
                string[] baseStr = { muzzle.Sunder.ToString(), muzzle.Pirerce.ToString(),
                    muzzle.Range.ToString(), muzzle.SlowTime1.ToString(), muzzle.Gravity.ToString() };
                string[] addOrSub = null;
                if (Lastmuzzle != null)
                {
                    addOrSub = muzzle.Compare(Lastmuzzle);
                }
                DisplayPartPro(contentTran, tips, baseStr, addOrSub);
            }
            if (wType == AccessoryType.Sight)
            {
                SighyAccessory muzzle = (SighyAccessory)clickPart;
                SighyAccessory Lastmuzzle = (SighyAccessory)lastequlPart;
                string[] tips = { "精准", "开火精准", "移动精准", "暴击", "重量" };
                string[] baseStr = { muzzle.Accuracy.ToString(), muzzle.Accuracybyfire.ToString(),
                    muzzle.Accuracybymove.ToString(),muzzle.Critical.ToString(), muzzle.Gravity.ToString() };
                string[] addOrSub = null;
                if (Lastmuzzle != null)
                {
                    addOrSub = muzzle.Compare(Lastmuzzle);
                }
                DisplayPartPro(contentTran, tips, baseStr, addOrSub);
            }
            if (wType == AccessoryType.Maganize)
            {
                MaganizeAccessory muzzle = (MaganizeAccessory)clickPart;
                MaganizeAccessory Lastmuzzle = (MaganizeAccessory)lastequlPart;
                string[] tips = { "弹夹量", "载弹量", "射速", "重量" };
                string[] baseStr = { muzzle.BoxAmmoCount1.ToString(), muzzle.BackAmmoCount1.ToString(),
                    muzzle.ShootTime1.ToString(), muzzle.Gravity.ToString() };
                string[] addOrSub = null;
                if (Lastmuzzle != null)
                {
                    addOrSub = muzzle.Compare(Lastmuzzle);
                }
                DisplayPartPro(contentTran, tips, baseStr, addOrSub);
            }
            if (wType == AccessoryType.MuzzleSuit)
            {
                MuzzleSuitAccessory muzzle = (MuzzleSuitAccessory)clickPart;
                MuzzleSuitAccessory Lastmuzzle = (MuzzleSuitAccessory)lastequlPart;
                string[] tips = { "精准", "伤害", "破甲", "射程", "重量" };
                string[] baseStr = { muzzle.Accuracy.ToString(), muzzle.Damage.ToString(),
                    muzzle.Sunder.ToString(), muzzle.Range.ToString(),
                    muzzle.Gravity.ToString() };
                string[] addOrSub = null;
                if (Lastmuzzle != null)
                {
                    addOrSub = muzzle.Compare(Lastmuzzle);
                }
                DisplayPartPro(contentTran, tips, baseStr, addOrSub);
            }

            if (wType == AccessoryType.Trigger)
            {
                TriggerAccessory muzzle = (TriggerAccessory)clickPart;
                TriggerAccessory Lastmuzzle = (TriggerAccessory)lastequlPart;
                string[] tips = { "攻击间隔", "装填时间", "后坐力", "重量" };
                string[] baseStr = { muzzle.ShootTime1.ToString(), muzzle.Reloadtime.ToString(),
                    muzzle.Backpower.ToString(), muzzle.Gravity.ToString() };
                string[] addOrSub = null;
                if (Lastmuzzle != null)
                {
                    addOrSub = muzzle.Compare(Lastmuzzle);
                }
                DisplayPartPro(contentTran, tips, baseStr, addOrSub);
            }
        }

        //响应点击了那个配件
        private void OnClickCurrentPart(GameObject go)
        {
            string currentName = go.transform.parent.gameObject.name;
            int index = int.Parse(currentName.Substring(8));
           
            if (index >=0 && index < m_ShowAccList.Count)
            {
               currentClickPart = m_ShowAccList[index];
            }
            if (currentClickPart == null)
                return;
            Transform contentTran = go.transform.parent.Find("Content");
            FillContent(contentTran,whichType,currentClickPart,m_LastEqulPart);
        }

        private void DisplayPartPro(Transform contentTran, string[] strList, 
            string[] baseStr,string[] addOrSubStr)
        {
            //TIps全部隐藏
            for (int i = 0; i < contentTran.childCount; i++)
            {
                NGUITools.SetActive(contentTran.GetChild(i).gameObject, false);
            }
            for (int i = 0; i < strList.Length; i++)
            {
                NGUITools.SetActive(contentTran.GetChild(i).gameObject, true);
            }
            for (int i = 0; i < strList.Length; i++)
            {
                contentTran.GetChild(i).GetChild(0).GetComponent<UILabel>().text = strList[i];
                contentTran.GetChild(i).GetChild(1).GetComponent<UILabel>().text = baseStr[i];
                if (addOrSubStr != null)
                {
                    //精确到几位
                    if (addOrSubStr[i].Length > 5)
                    {
                        addOrSubStr[i] = addOrSubStr[i].Substring(0, 5);
                    }
                    contentTran.GetChild(i).GetChild(2).GetComponent<UILabel>().text
                        = addOrSubStr[i];
                }
                else
                {
                    NGUITools.SetActive(contentTran.GetChild(i).GetChild(2).gameObject, false);
                }
            }
        }

        //装备了那个配件
        private void OnEqulMentParts(GameObject gp)
        {
            if (currentClickPart != null)
            {
                m_LastEqulPart = currentClickPart;
                WeaponDetailsDialogUI.Instance.CurrentWeapon.Assemble(m_LastEqulPart);
                Debug.Log("当前武器id"+ WeaponDetailsDialogUI.Instance.CurrentWeapon.ID
                    +"装备了配件id"+ m_LastEqulPart.ID);
                //关闭配件装备界面
                //FillHaveEqulPartUI();
                OnCloseShowEqulPart(null);
                ////卸载bu可点击
                m_EqulPartDialogBox.transform.Find("buttonGroup")
                        .GetChild(1).gameObject.GetComponent<UIButton>().isEnabled = true;
                m_EqulPartDialogBox.transform.Find("buttonGroup")
                   .GetChild(1).GetChild(0).GetComponent<UILabel>().depth = 6;
                WeaponDetailsDialogUI.Instance.NoDoInstallOr = false;
            }
        }

        //卸载配件
        private void OnUnInstallEqulPart(GameObject go)
        {
           
            OnCloseShowEqulPart(go);
            if (m_LastEqulPart != null)
            {
                WeaponDetailsDialogUI.Instance.CurrentWeapon.Disperse(m_LastEqulPart);
                Debug.Log("当前武器id" + WeaponDetailsDialogUI.Instance.CurrentWeapon.ID
                    + "卸载了配件id" + m_LastEqulPart.ID);
                m_LastEqulPart = null;
                WeaponDetailsDialogUI.Instance.NoDoInstallOr = false;
            }
            //FillHaveEqulPartUI();
            OnCloseShowEqulPart(null);
            //卸载bu可点击
            //m_EqulPartDialogBox.transform.Find("buttonGroup")
            //        .GetChild(1).gameObject.GetComponent<UIButton>().isEnabled = false;
            //m_EqulPartDialogBox.transform.Find("buttonGroup")
            //   .GetChild(1).GetChild(0).GetComponent<UILabel>().depth = 5;
        }

        private void OnBack(GameObject go)
        {
            WeaponDetailsDialogUI.Instance.NoDoInstallOr = true;
            OnCloseShowEqulPart(go);
        }

        //关闭配件装备界面
        private void OnCloseShowEqulPart(GameObject go)
        {
            m_ShowAccList = null;
            WeaponDetailsDialogUI.Instance.ShowAndRefresh();
            this.CloseDialog();
            //干掉列表的内容  恢复原来的一个prefab
            for (int i = 0; i < m_EqulPartDialogBox.transform.
                GetChild(1).GetChild(0).GetChild(0).childCount; i++)
            {
                GameObject.Destroy(m_EqulPartDialogBox.transform.
                    GetChild(1).GetChild(0).GetChild(0).GetChild(i).gameObject);
            }
        }

        private void CloseDialog()
        {
            NGUITools.SetActive(m_EqulPartDialogBox, false);
            //返回按钮
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform
                .Find("center").Find("backMainforEqulPartDialogBox").gameObject, false);
        }

        private void OpenDialog()
        {
            NGUITools.SetActive(m_EqulPartDialogBox, true);
            //返回按钮
            NGUITools.SetActive(PanelMgr.CurrPanel.RootObj.transform
                .Find("center").Find("backMainforEqulPartDialogBox").gameObject, true);
        }

        //填充数据到已经装备的ui
        private void FillHaveEqulPartUI()
        {
            Transform HaveEquUI = m_EqulPartDialogBox.transform.Find("HaveEqulPartItem");
            if (m_LastEqulPart != null)
            {
                NGUITools.SetActive(HaveEquUI.gameObject, true);
                Texture texture1 = ResMgr.ResLoad.Load<Texture>(
                       Utility.ConstantValue.PartIcon + "/" + m_LastEqulPart.PartsIcon + Utility.ConstantValue.UpEndPath);
                HaveEquUI.GetChild(0).GetComponent<UITexture>().mainTexture = texture1;
                HaveEquUI.GetChild(1).GetComponent<UILabel>().text = m_LastEqulPart.Name;
                FillContent(HaveEquUI.Find("Content"), m_LastEqulPart.Type, m_LastEqulPart, null);
                m_EqulPartDialogBox.transform.GetChild(1).localPosition = Vector3.zero;
            }
            else
            {
                NGUITools.SetActive(HaveEquUI.gameObject, false);
                m_EqulPartDialogBox.transform.GetChild(1).localPosition = new Vector3(0,70,0);
            }
        }

        private void FillDataToEqulPartsDialogBox(string partsName)
        {
            switch (partsName)
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
            currentClickPart = null;
            //获取上次装备的那个配件
            m_LastEqulPart = WeaponDetailsDialogUI.Instance.CurrentWeapon.GetAccessory(whichType);
            Utility.Utility.GetUIEventListener(m_EqulPartDialogBox.transform.Find("buttonGroup")
                .GetChild(0).gameObject).onClick = OnEqulMentParts;
            Utility.Utility.GetUIEventListener(m_EqulPartDialogBox.transform.Find("buttonGroup")
                    .GetChild(1).gameObject).onClick = OnUnInstallEqulPart;
            //如果没有装备的东西按钮置灰不可点击
            if (m_LastEqulPart == null)
            {
                m_EqulPartDialogBox.transform.Find("buttonGroup")
                    .GetChild(1).gameObject.GetComponent<UIButton>().isEnabled = false;
                m_EqulPartDialogBox.transform.Find("buttonGroup")
                   .GetChild(1).GetChild(0).GetComponent<UILabel>().depth = 5;
            }
            //返回及关闭
            Utility.Utility.GetUIEventListener(
            PanelMgr.CurrPanel.RootObj.transform.Find("center").Find("backMainforEqulPartDialogBox")
            .gameObject).onClick = OnBack;
            //填充已装备武器的属性各值
            FillHaveEqulPartUI();
            //获取适合的配件列表
            m_ShowAccList = Role.Role.Instance().KitBag.GetWeaponAceesory(
                WeaponDetailsDialogUI.Instance.CurrentWeapon, whichType);
            //不显示已经装备了的
            m_ShowAccList.Remove(m_LastEqulPart);
            Debug.Log("合适的配件有"+m_ShowAccList.Count);
            List<GameObject> partItemList = new List<GameObject>();

            string equlPartItemPath = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/DialogBox_SomeItem/EqulPartItem";
            GameObject prefab = ResMgr.ResLoad.Load<GameObject>(equlPartItemPath);
            //具体要根据数据 
            for (int i = 0; i < m_ShowAccList.Count; i++)
            {
                GameObject item =GameObject.Instantiate(prefab);
                item.transform.parent = m_EqulPartDialogBox.transform.GetChild(1)
                    .GetChild(0).GetChild(0).transform;
                item.transform.localScale = Vector3.one;
                item.name = "PartItem" + i;
                partItemList.Add(item);
            }
            //重新排列下item  因为前面Destroy时  是没有重新排的
            m_EqulPartDialogBox.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<UIGrid>().Reposition();
            m_EqulPartDialogBox.transform.GetChild(1).GetChild(0).GetComponent<UIScrollView>().ResetPosition();
            if (m_ShowAccList.Count != 0)
            {
                for (int i = 0; i < partItemList.Count; i++)
                {
                    Utility.Utility.GetUIEventListener(partItemList[i].transform.GetChild(0).gameObject)
                        .onClick = OnClickCurrentPart;
                    //name
                    partItemList[i].transform.GetChild(2).GetComponent<UILabel>().text = m_ShowAccList[i].Name;
                    //图标
                    Texture texture = ResMgr.ResLoad.Load<Texture>(
                      Utility.ConstantValue.PartIcon + "/" + m_ShowAccList[i].PartsIcon + Utility.ConstantValue.UpEndPath);
                    Texture texture1 = ResMgr.ResLoad.Load<Texture>(
                       Utility.ConstantValue.PartIcon + "/" + m_ShowAccList[i].PartsIcon + Utility.ConstantValue.EndIconPath);
                    //Debug.Log("配件的图----"+ m_ShowAccList[i].PartsIcon);
                    //partItemList[i].transform.GetChild(0).GetComponent<UISprite>().atlas = ;
                    partItemList[i].transform.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
                    partItemList[i].transform.GetChild(1).GetComponent<UITexture>().mainTexture = texture1;
                }
            }
            //不足一页是禁止滑动
            if (m_ShowAccList.Count <= 6)
                m_EqulPartDialogBox.transform.GetChild(1).GetChild(0).GetComponent<UIScrollView>().enabled = false;
            else
                m_EqulPartDialogBox.transform.GetChild(1).GetChild(0).GetComponent<UIScrollView>().enabled = true;
        }

        //--------------------------------------
        //public
        //--------------------------------------
        public void OnShowEqulParts(GameObject go)
        {

            //Muzzle = 1,         //枪口
            //Barrel,             //枪管
            //Sight,              //瞄具
            //Maganize,           //弹夹
            //MuzzleSuit,         //枪管套件
            //Trigger,   
            //Debug.Log("查看枪口装备情况"+go.name);
            if (m_EqulPartDialogBox == null)
            {
                Transform t = WeaponDetailsDialogUI.Instance.
                    CurrentItem.transform.Find("equlPartDialogBox");
                if (t != null)
                    m_EqulPartDialogBox = t.gameObject;
                if (m_EqulPartDialogBox == null)
                {
                    string EqulPartDialogUIpath = "UIRootPrefabs/BagPackagePanel_PageItem/DialogUIPrefabs/equlPartDialogBox";
                    m_EqulPartDialogBox = Utility.Utility.GetPrefabGameObject(EqulPartDialogUIpath,
                        "equlPartDialogBox", WeaponDetailsDialogUI.Instance.CurrentItem.transform);
                }
            }
            this.OpenDialog();
            //解决一开始不在顶部的bug
            m_EqulPartDialogBox.transform.GetChild(1).GetChild(0).GetComponent<UIScrollView>().ResetPosition();
            FillDataToEqulPartsDialogBox(go.name);
        }

    }
}
