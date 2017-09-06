//******************************************************************
// File Name:					ExchangeItemOrder
// Description:					ExchangeItemOrder class 
// Author:						lanjian
// Date:						3/7/2017 11:11:19 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using Network.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Exchange
{
    /// <summary>
    /// 订单状态
    /// </summary>
    enum OrderState
    {
        Unknow,
        ApplyFor,                   //申请中
        Delivered,                  //已发货
        DenyDeliverd,               //拒绝发货
        Finished,                   //完成
    }

    //兑换订单类
    class ExchangeItemOrder
    {
        private string m_orderId;
        private int m_ExchangeItemId;
        private CurrencyType m_currentype;
        private int m_totalPay;
        private int m_count;
        private OrderState m_state;
        private int m_datetime;
        private Address m_address;

        private ExchangePrizeItem m_item;

        public string OrderId { get { return this.m_orderId; } }
        public int ExchangeItemId { get { return this.m_ExchangeItemId; } }
        public CurrencyType CurrenttTp { get { return this.m_currentype; } }
        public int TotalPay { get { return this.m_totalPay; } }
        public int Count { get { return this.m_count; } }
        public OrderState State { get { return this.m_state; } }
        public int DateTime { get { return this.m_datetime; } }
        public Address Addr { get { return this.m_address; } }
        public ExchangePrizeItem Item { get { return this.m_item; } }

        public ExchangeItemOrder(DataObj data)
        {
            this.m_orderId = data.GetString("order_id");
            this.m_ExchangeItemId = data.GetInt32("prize_id");
            this.m_currentype = (CurrencyType)data.GetInt32("currency_type");
            this.m_totalPay = data.GetInt32("total");
            this.m_count = data.GetInt32("count");
            this.m_state = (OrderState)data.GetInt8("state");
            this.m_datetime = data.GetInt32("datetime");
            this.m_address = new Address(data.GetDataObj("addr"));
            this.m_item = new ExchangePrizeItem(m_ExchangeItemId);
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public void Dispose()
        {
            
        }

    }
}
