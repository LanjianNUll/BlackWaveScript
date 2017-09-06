//******************************************************************
// File Name:					ShopPageBase
// Description:					ShopPageBase class 
// Author:						lanjian
// Date:						3/1/2017 3:39:16 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using FW.Event;
using UnityEngine;
using FW.Item;
using FW.Store;

namespace FW.UI
{
    class ShopPageBase : ScrollViewItemBase
    {
        public override void FillItem(EventArg eventArg){ }

        protected int m_selfPageCapatiy;                   //每页的容量
        protected GameObject m_currentClickBuyItem;        //点前点击的那个Go
        protected StoreItem m_currentStoreItem;            //当前点击的那个商品


        public virtual void OnShowConfirmBuy(GameObject go){ }
        
        //填充道具是武器的属性界面
        public void FillDataContent1(WeaponBase currentWeaponBase, GameObject go)
        {
            go.transform.Find("Content1/ObjectName (1)").GetComponent<UILabel>().text = currentWeaponBase.Name;
            Transform content = go.transform.Find("Content1/content");
            content.GetChild(0).GetChild(0).GetComponent<UILabel>().text = "伤害：";
            content.GetChild(0).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.Injure + "";
            content.GetChild(1).GetChild(0).GetComponent<UILabel>().text = "破甲：";
            content.GetChild(1).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.SunderArmor + "";
            content.GetChild(2).GetChild(0).GetComponent<UILabel>().text = "射速：";
            content.GetChild(2).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.ShootTime + "";
            content.GetChild(3).GetChild(0).GetComponent<UILabel>().text = "精准：";
            content.GetChild(3).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.AccuracyMax + "";
            content.GetChild(4).GetChild(0).GetComponent<UILabel>().text = "射程：";
            content.GetChild(4).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.FireRange + "";
            content.GetChild(5).GetChild(0).GetComponent<UILabel>().text = "控制：";
            content.GetChild(5).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.SlowTime + "";
            content.GetChild(6).GetChild(0).GetComponent<UILabel>().text = "装填速度：";
            content.GetChild(6).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.ReloadTime + "";
            content.GetChild(7).GetChild(0).GetComponent<UILabel>().text = "弹夹容量：";
            content.GetChild(7).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.BoxAmmoCount + "";
            content.GetChild(8).GetChild(0).GetComponent<UILabel>().text = "携弹总量：";
            content.GetChild(8).GetChild(1).GetComponent<UILabel>().text = currentWeaponBase.BackAmmoCount + "";
            //隐藏多余的
            for (int i = 9; i < content.childCount; i++)
            {
                NGUITools.SetActive(content.GetChild(i).gameObject, false);
            }
        }

        //填充道具是配件的属性界面
        public void FillDataContent1(AccessoryBase currentpartBase, GameObject go)
        {
            go.transform.Find("Content1/ObjectName (1)").GetComponent<UILabel>().text = currentpartBase.Name;
            Transform propertyTranform = go.transform.Find("Content1/content");
            if (currentpartBase.Type == AccessoryType.Muzzle)
            {
                MuzzleAccessory muzzle = (MuzzleAccessory)currentpartBase;
                string[] tips = { "单发伤害", "精准", "后坐力", "重量" };
                string[] baseStr = { muzzle.Damage.ToString(), muzzle.Accuracyrenew.ToString(),
                    muzzle.Backpower.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (currentpartBase.Type == AccessoryType.Barrel)
            {
                BarrelAccessory muzzle = (BarrelAccessory)currentpartBase;
                string[] tips = { "破甲", "穿透", "射程", "停滞时间", "重量" };
                string[] baseStr = { muzzle.Sunder.ToString(), muzzle.Pirerce.ToString(),
                    muzzle.Range.ToString(), muzzle.SlowTime1.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (currentpartBase.Type == AccessoryType.Sight)
            {
                SighyAccessory muzzle = (SighyAccessory)currentpartBase;
                string[] tips = { "精准", "开火精准", "移动精准", "暴击", "重量" };
                string[] baseStr = { muzzle.Accuracy.ToString(), muzzle.Accuracybyfire.ToString(),
                    muzzle.Accuracybymove.ToString(),muzzle.Critical.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (currentpartBase.Type == AccessoryType.Maganize)
            {
                MaganizeAccessory muzzle = (MaganizeAccessory)currentpartBase;
                string[] tips = { "弹夹量", "载弹量", "射速", "重量" };
                string[] baseStr = { muzzle.BoxAmmoCount1.ToString(), muzzle.BackAmmoCount1.ToString(),
                    muzzle.ShootTime1.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (currentpartBase.Type == AccessoryType.MuzzleSuit)
            {
                MuzzleSuitAccessory muzzle = (MuzzleSuitAccessory)currentpartBase;
                string[] tips = { "精准", "伤害", "破甲", "射程", "重量" };
                string[] baseStr = { muzzle.Accuracy.ToString(), muzzle.Damage.ToString(),
                    muzzle.Sunder.ToString(), muzzle.Range.ToString(),
                    muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
            if (currentpartBase.Type == AccessoryType.Trigger)
            {
                TriggerAccessory muzzle = (TriggerAccessory)currentpartBase;
                string[] tips = { "射速", "装填时间", "后坐力", "重量" };
                string[] baseStr = { muzzle.ShootTime1.ToString(), muzzle.Reloadtime.ToString(),
                    muzzle.Backpower.ToString(), muzzle.Gravity.ToString() };
                DisPlayPro(propertyTranform, tips, baseStr);
            }
        }

        public void DisPlayPro(Transform propertyTranform, string[] tips, string[] baseStr)
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

        //计算页数   tabIndex
        public int CalculatePageCount(List<StoreItem> list, int tabIndex = 0)
        {
            if (list.Count == 0)
                return 0;
            int PageCount = (list.Count / m_selfPageCapatiy) + 1;
            if (PageCount == 1)
            {
                //禁止上下页滑动
                this.CurrentItem.transform.GetChild(1).GetChild(tabIndex).GetComponent<UIScrollView>().enabled = false;
            }
            return PageCount;
        }

        //计算页数   tabIndex
        public int CalculatePageCount(List<PayItem> list, int tabIndex = 0)
        {
            if (list.Count == 0)
                return 0;
            int PageCount = (list.Count / m_selfPageCapatiy) + 1;
            if (PageCount == 1)
            {
                //禁止上下页滑动
                this.CurrentItem.transform.GetChild(1).GetChild(tabIndex).GetComponent<UIScrollView>().enabled = false;
            }
            return PageCount;
        }


        //处理点击按钮相关的处理
        public void ConfirmButtonDealWith(GameObject go,List<StoreItem> storeItemList)
        {
            string id = go.transform.parent.Find("gunid").GetComponent<UILabel>().text;
            m_currentClickBuyItem = go.transform.parent.gameObject;
            for (int i = 0; i < storeItemList.Count; i++)
            {
                if (storeItemList[i].ID.ToString().Equals(id))
                    m_currentStoreItem = storeItemList[i];
            }
            if (m_currentStoreItem.IsBought)
            {
                Utility.Utility.NotifyStr("你已经购买过此" + m_currentStoreItem.Item.Name + "！！不能重复购买");
                return;
            }
            DialogMgr.Load(DialogType.ConmonSimple);
            DialogMgr.CurrentDialog.ShowCommonDialog(new FW.Event.EventArg(m_currentStoreItem));
        }

        //将购买过的物品UI置灰
        public void FindHaveBuyItemSetIsBought()
        {
            m_currentClickBuyItem.transform.Find("Objectpicbtn").GetComponent<UITexture>().color = Color.gray;
        }

        //购买后的反馈
        public void OnBuyItemInfo(EventArg args)
        {
            //商品购买反馈(参数：EventArg(StoreItem 商品对象, bool 0成功1未成功))
            StoreItem backBuyStoreItem = (StoreItem)args[0];
            bool isGetBuySucess = (bool)args[1];
            if (isGetBuySucess)
            {
                FindHaveBuyItemSetIsBought();
                Utility.Utility.NotifyStr("购买" + backBuyStoreItem.Item.Name + "成功！！");
            }
            else
            {
                Utility.Utility.NotifyStr("购买" + backBuyStoreItem.Item.Name + "失败!");
            }
        }

        //再次点击是隐藏Tips
        string lastid = "";
        protected void OnShowOrHideTips(GameObject go)
        {
            GameObject cont = go.transform.GetComponent<UIToggledObjects>().activate[0];
            string id = go.transform.parent.Find("gunid").GetComponent<UILabel>().text;
            if (!string.IsNullOrEmpty(lastid))
            {
                if (id.Equals(lastid))
                {
                    NGUITools.SetActive(cont, false);
                    lastid = "";
                }
                else
                {
                    NGUITools.SetActive(cont, true);
                    lastid = id;
                }
            }
            else
            {
                NGUITools.SetActive(cont, true);
                lastid = id;
            }
        }

        //填充数据到每一个项
        public void FillDateInPageItem(int pageIndex, GameObject pageGo, List<StoreItem> storeList)
        {
            int displayNum = (pageIndex + 1) * m_selfPageCapatiy < storeList.Count ? m_selfPageCapatiy : storeList.Count - pageIndex * m_selfPageCapatiy;
            int ABeginIndex = pageIndex * m_selfPageCapatiy;
            for (int i = 0; i < displayNum; i++)
            {
                //点击显示确定对话框
                Utility.Utility.GetUIEventListener(pageGo.transform.GetChild(i).Find("buyButton"))
                       .onClick = OnShowConfirmBuy;
                //点击显示tip
                Utility.Utility.GetUIEventListener(pageGo.transform.GetChild(i).Find("Objectpicbtn"))
                      .onClick = OnShowOrHideTips;
                pageGo.transform.GetChild(i).Find("gunid").GetComponent<UILabel>().text = storeList[ABeginIndex + i].ID.ToString();
                //物品的类型  武器还是配件 还是道具
                string itemName = "";  // 名称
                string desStr = "";
                string iconpath = "";
                //Debug.Log("第" + (ABeginIndex + i) + "道具" + storeList[ABeginIndex + i].ItemType);
                if (storeList[ABeginIndex + i].ItemType == Item.ItemType.Commodity)
                {
                    CommodityBase currentComdityBase = (CommodityBase)(storeList[ABeginIndex + i].Item);
                    itemName = currentComdityBase.Name;
                    desStr = currentComdityBase.Desc;
                    iconpath = Utility.ConstantValue.CommodityIcon+"/" + currentComdityBase.BagIcon + "_up";
                    //Debug.Log("道具的图标"+ iconpath);
                    List<GameObject> activateList = new List<GameObject>();
                    activateList.Add(pageGo.transform.GetChild(i).Find("Content").gameObject);
                    Utility.Utility.GetUIEventListener(pageGo.transform.GetChild(i).Find("Objectpicbtn"))
                        .GetComponent<UIToggledObjects>().activate = activateList;
                }
                if (storeList[ABeginIndex + i].ItemType == Item.ItemType.Accessory)
                {
                    AccessoryBase currentComdityBase = (AccessoryBase)(storeList[ABeginIndex + i].Item);
                    itemName = currentComdityBase.Name;
                    desStr = currentComdityBase.Desc;
                    iconpath = Utility.ConstantValue.PartIcon+ "/" + currentComdityBase.PartsIcon+ "_up";
                    List<GameObject> activateList = new List<GameObject>();
                    activateList.Add(pageGo.transform.GetChild(i).Find("Content1").gameObject);
                    Utility.Utility.GetUIEventListener(pageGo.transform.GetChild(i).Find("Objectpicbtn"))
                        .GetComponent<UIToggledObjects>().activate = activateList;
                    FillDataContent1(currentComdityBase, pageGo.transform.GetChild(i).gameObject);
                }
                if (storeList[ABeginIndex + i].ItemType == Item.ItemType.Weapon)
                {
                    WeaponBase currentComdityBase = (WeaponBase)(storeList[ABeginIndex + i].Item);
                    itemName = currentComdityBase.Name;
                    desStr = currentComdityBase.Desc;
                    iconpath = Utility.ConstantValue.WeaponIcon + "/" + currentComdityBase.BagIcon + "_up";
                    List<GameObject> activateList = new List<GameObject>();
                    activateList.Add(pageGo.transform.GetChild(i).Find("Content1").gameObject);
                    Utility.Utility.GetUIEventListener(pageGo.transform.GetChild(i).Find("Objectpicbtn"))
                        .GetComponent<UIToggledObjects>().activate = activateList;
                    FillDataContent1(currentComdityBase, pageGo.transform.GetChild(i).gameObject);
                }
                //Debug.Log("图片的路径为"+ iconpath);
                //test  测试 
                //iconpath = "Texture/ShopIcon/esp";
                Texture texture1 = ResMgr.ResLoad.Load<Texture>(iconpath);
                if(texture1 == null)
                    texture1 = ResMgr.ResLoad.Load<Texture>("res/UITexture/ShopIcon/esp");
                pageGo.transform.GetChild(i).Find("Objectpicbtn").GetComponent<UITexture>().SetRect(0, 0, texture1.width, texture1.height);
                int offset = -130;
                if (storeList[ABeginIndex + i].Type == StoreItemType.Supply)
                    offset = -157;
                pageGo.transform.GetChild(i).Find("Objectpicbtn").GetComponent<Transform>().localPosition = new Vector3(0, offset, 0);
                pageGo.transform.GetChild(i).Find("Objectpicbtn").GetComponent<UITexture>().mainTexture = texture1;
                pageGo.transform.GetChild(i).Find("ObjectName").GetComponent<UILabel>().text = itemName;
                pageGo.transform.GetChild(i).Find("Content/ObjectName (1)").GetComponent<UILabel>().text = itemName;
                pageGo.transform.GetChild(i).Find("ObjectNum").GetComponent<UILabel>().text = storeList[ABeginIndex + i].Price.ToString();
                pageGo.transform.GetChild(i).Find("Content/content").GetComponent<UILabel>().text = desStr;
                //是否可以购买
                if (storeList[ABeginIndex + i].IsBought)
                {
                    pageGo.transform.GetChild(i).Find("Objectpicbtn").GetComponent<UITexture>().color = Color.gray;
                }
            }

            //隐藏这页多余的多余的
            for (int i = 0; i < m_selfPageCapatiy - displayNum; i++)
            {
                NGUITools.SetActive(pageGo.transform.GetChild(m_selfPageCapatiy-1 - i).gameObject, false);
            }
        }
    }
}
