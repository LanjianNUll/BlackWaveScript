//******************************************************************
// File Name:					PutUpSaleAndReadySaleBaseDialogUI
// Description:					PutUpSaleAndReadySaleBaseDialogUI class 
// Author:						lanjian
// Date:						3/14/2017 9:59:24 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Deal;
using FW.Item;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.UI
{
    /// <summary>
    /// 交易的dialog基类   展开的操作在这个类
    /// </summary>
    class PutUpSaleAndReadySaleBaseDialogUI: DialogBaseUI
    {
        protected UIScrollView scrollView;
        protected Transform m_centerTran;
        //控制展开动画的
        protected int m_ItemHeight = 170;
        protected List<GameObject> m_currentGoList = new List<GameObject>();
        protected int CurrentClickNum = -1;            //当前点击了那个item
        protected int PreClickNum = -1;                 //记录上一个点击的index
        protected bool IsExistUnFold;                  // 是否当前还存在没有折叠的
        protected float currentExpandWidth = 590;

        //当前点击的交易Item
        protected DealItemInfo m_currentDealItemInfo;

        //填充每个交易物品方法
        protected void FindItemData(GameObject go, DealItemInfo dealItemInfo)
        {
            //记录物品的key
            go.transform.GetChild(0).GetComponent<UILabel>().text = dealItemInfo.DealKey;
            //头部的pro显示
            Transform brePro = go.transform.Find("brefPro");
            string iconPath = "";
            if (dealItemInfo.Item.ItemType == Item.ItemType.Weapon)
                iconPath = Utility.ConstantValue.WeaponIcon;
            if (dealItemInfo.Item.ItemType == Item.ItemType.Accessory)
                iconPath = Utility.ConstantValue.PartIcon;
            if (dealItemInfo.Item.ItemType == Item.ItemType.Commodity)
                iconPath = Utility.ConstantValue.CommodityIcon;
            Texture texture = ResMgr.ResLoad.Load<Texture>(iconPath + "/" + dealItemInfo.Item.Icon + Utility.ConstantValue.UpEndPath);
            brePro.GetChild(0).GetComponent<UITexture>().SetRect(0, 0, texture.width, texture.height);
            brePro.GetChild(0).GetComponent<Transform>().localPosition = new Vector3(-283, 0, 0);
            brePro.GetChild(0).GetComponent<UITexture>().mainTexture = texture;
            brePro.GetChild(1).GetComponent<UILabel>().text = dealItemInfo.Item.Name;
            brePro.GetChild(2).GetComponent<UILabel>().text = "等级限制："+dealItemInfo.Item.Levellimit.ToString();
            brePro.GetChild(3).GetComponent<UILabel>().text = Utility.Utility.GetColorName(dealItemInfo.Item.Quality);
            brePro.GetChild(4).GetComponent<UILabel>().text = "￥"+ dealItemInfo.Price.ToString();

            Transform content = go.transform.Find("content");
            content.GetChild(1).GetComponent<UITexture>().SetRect(0, 0, texture.width, texture.height);
            content.GetChild(1).GetComponent<Transform>().localPosition = new Vector3(0, 597, 0);
            content.GetChild(1).GetComponent<UITexture>().mainTexture = texture;
            content.Find("name").GetComponent<UILabel>().text = dealItemInfo.Item.Name;
            content.Find("level").GetComponent<UILabel>().text = dealItemInfo.Item.Levellimit.ToString();
            content.Find("value").GetComponent<UILabel>().text = Utility.Utility.Int2DateAndSecond(dealItemInfo.EndTime);
            content.Find("price").GetComponent<UILabel>().text = "￥" + dealItemInfo.Price;
            NGUITools.SetActive(content.Find("commodiytDesc").gameObject, false);
            if (dealItemInfo.Item.ItemType == Item.ItemType.Weapon)
            {
                FillWeaponPro(content, (WeaponBase)dealItemInfo.Item);
                content.Find("level").GetComponent<UILabel>().text = "LV：" + dealItemInfo.Item.Levellimit.ToString();
            }
            if (dealItemInfo.Item.ItemType == Item.ItemType.Accessory)
            {
                FillAccessoryPro(content.Find("prorGroup"), (AccessoryBase)dealItemInfo.Item);
                content.Find("level").GetComponent<UILabel>().text = "LV：" + dealItemInfo.Item.Levellimit.ToString();
            }
            if (dealItemInfo.Item.ItemType == Item.ItemType.Commodity)
            {
                Transform c = content.Find("commodiytDesc");
                NGUITools.SetActive(content.Find("prorGroup").gameObject, false);
                NGUITools.SetActive(c.gameObject, true);
                CommodityBase com = (CommodityBase)dealItemInfo.Item;
                c.GetComponent<UILabel>().text = com.Desc;
                content.Find("level").GetComponent<UILabel>().text = "数量：" + dealItemInfo.ItemCount;
            }
            //根据当前是什么对话框（我的寄售   寄售列表）
            if (this.m_DType == DialogType.MyForSold)
            {
                go.transform.GetChild(2).Find("timetip").GetComponent<UILabel>().text = "预售倒计时";
                go.transform.GetChild(2).Find("Buy").GetChild(0).GetComponent<UILabel>().text = "下架";

                if (dealItemInfo.State == ItemState.WaitForSale)
                {
                    NGUITools.SetActive(go.transform.GetChild(2).Find("Buy").gameObject, false);
                    NGUITools.SetActive(go.transform.GetChild(2).Find("prebuy").gameObject, true);
                    go.transform.GetChild(2).Find("prebuy").GetComponent<UILabel>().text = "预售中";
                }
            }
            else
            {
                go.transform.GetChild(2).Find("timetip").GetComponent<UILabel>().text = "寄售倒计时";
                go.transform.GetChild(2).Find("Buy").GetChild(0).GetComponent<UILabel>().text = "购买";
                if (dealItemInfo.State == ItemState.WaitForSale)
                {
                    go.transform.GetChild(2).Find("Buy").GetChild(0).GetComponent<UILabel>().text = "预购";
                }
            }
        }

        //填充武器的属性
        protected void FillWeaponPro(Transform content, WeaponBase weapon)
        {
            Transform propertyTranform = content.Find("prorGroup");
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

        protected void BindItemListener(GameObject go)
        {
            Utility.Utility.GetUIEventListener(go).onClick = OnItemClick;
            Utility.Utility.GetUIEventListener(go.transform.GetChild(2).Find("Buy").gameObject)
                .onClick = OnBuyItemClick;
        }
        
        protected void Fold()
        {
            m_currentGoList[PreClickNum].transform.Find("content").gameObject.SetActive(false);
            m_currentGoList[PreClickNum].transform.Find("brefPro").gameObject.SetActive(true);
            ModifyBoxColiderSizeAndPositon(false);
            for (int i = PreClickNum; i < m_currentGoList.Count; i++)
            {
                GameObject currentItem = m_currentGoList[i];
                currentItem.transform.localPosition = new Vector3(0, 0 - i * m_ItemHeight, 0);
            }
            IsExistUnFold = false;
        }

        protected void UnFold()
        {
            ModifyBoxColiderSizeAndPositon(true);
            m_currentGoList[CurrentClickNum].transform.Find("content").gameObject.SetActive(true);
            m_currentGoList[CurrentClickNum].transform.Find("brefPro").gameObject.SetActive(false);
            for (int i = CurrentClickNum + 1; i < m_currentGoList.Count; i++)
            {
                GameObject currentItem = m_currentGoList[i];
                Vector3 regin = currentItem.transform.localPosition;
                Vector3 target = currentItem.transform.localPosition + new Vector3(0, 0 - currentExpandWidth, 0);
                GameObjectPositioAnim(currentItem, regin, target);
            }
            IsExistUnFold = true;
            PreClickNum = CurrentClickNum;
        }

        protected void ModifyBoxColiderSizeAndPositon(bool isUnLoldOrLold)
        {
            if (isUnLoldOrLold)
            {
                BoxCollider boxC = m_currentGoList[CurrentClickNum].GetComponent<BoxCollider>();
                boxC.size = new Vector3(934, 750, 1);   //这里是直接在编辑面板上拉好的，可维护性很差
                boxC.center = new Vector3(0, -300, 0);
            }
            else
            {
                BoxCollider boxC = m_currentGoList[PreClickNum].GetComponent<BoxCollider>();
                boxC.size = new Vector3(934, 170, 1);  //这里是直接在编辑面板上拉好的，可维护性很差
                boxC.center = Vector3.zero;
            }
        }

        //重置位置
        protected void ResetPosition()
        {
            for (int i = 0; i < m_currentGoList.Count; i++)
            {
                Vector3 regin = m_currentGoList[i].transform.localPosition;
                Vector3 target = m_currentGoList[i].transform.localPosition = new Vector3(0, 0 - i * m_ItemHeight, 0);
                GameObjectPositioAnim(m_currentGoList[i], regin, target);

            }
            IsExistUnFold = false;
        }

        ///物体移动的动画 目标物体 原来的位置，目标位置，动画时间 默认为0.5s
        protected void GameObjectPositioAnim(GameObject go, Vector3 reginPosition, Vector3 targetPosition,
            float time = 0.5f)
        {
            //m_IsAnimFinish = false;
            TweenPosition ta = go.GetComponent<TweenPosition>();
            if (ta == null)
            {
                ta = go.AddComponent<TweenPosition>();
            }
            ta.ResetToBeginning();
            ta.from = reginPosition;
            ta.to = targetPosition;
            ta.enabled = true;
            ta.duration = time;
            ta.PlayForward();
        }

        //点击那个项的监听
        public virtual void OnItemClick(GameObject itemGo)
        {
            CurrentClickNum = int.Parse(itemGo.name.Substring(8));
            //点击是同一个时候的判读
            if (CurrentClickNum == PreClickNum)
            {
                this.ResetPosition();
                ModifyBoxColiderSizeAndPositon(false);
                m_currentGoList[PreClickNum].transform.Find("content").gameObject.SetActive(false);
                m_currentGoList[PreClickNum].transform.Find("brefPro").gameObject.SetActive(true);
                //同一个时在将上一个弄成-1，解决再次点击同一个不打开的bug
                PreClickNum = -1;
                return;
            }
            if (IsExistUnFold)
            {
                Fold();
            }
            UnFold();
        }
        //购买或下架的按钮监听
        public virtual void OnBuyItemClick(GameObject go) { }
    }
}
