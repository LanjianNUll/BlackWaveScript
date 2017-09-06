//******************************************************************
// File Name:					DealFitterItem
// Description:					DealFitterItem class 
// Author:						lanjian
// Date:						3/13/2017 11:58:57 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.Item;
using FW.ResMgr;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Deal
{
    //交易类型分类
    class DealFitterItem
    {
        private int m_subType;
        private int m_DealTradeType;                    //交易类型
        private string m_FitterIcon;                    //该类型的图标
        private string m_FitterName;
        private ItemType m_ItemType;

        public int DealTradeType { get { return this.m_DealTradeType; } }
        public string Icon { get { return this.m_FitterIcon; } }
        public string Name { get { return this.m_FitterName; } }
        public int SubType { get { return this.m_subType; } }
        public ItemType IType { get { return this.m_ItemType; } }

        public DealFitterItem(int dealTradeType, JsonItem jsonItem, ItemType itemType)
        {
            this.m_ItemType = itemType;
            this.m_DealTradeType = dealTradeType;
            this.Init(jsonItem);
        }

        private void Init(JsonItem jsonItem)
        {
            if (jsonItem == null) return;
            this.m_FitterName = jsonItem.Get("typename").AsString();
            this.m_FitterIcon = DatasMgr.GetRes(jsonItem.Get("iconID").AsInt());
            this.m_subType = jsonItem.Get("type").AsInt();
        }
    }
}
