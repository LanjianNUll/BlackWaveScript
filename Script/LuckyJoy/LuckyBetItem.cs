//******************************************************************
// File Name:					LuckyJoyItem
// Description:					LuckyJoyItem class 
// Author:						lanjian
// Date:						5/19/2017 9:59:30 AM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.LuckyJoy
{
    class LuckyBetItem
    {
        private bool[] m_betLine;                   
        private int m_totalBetMoney;
        private int m_BaseBetMoney;
        
        public bool[] BetLine { get { return this.m_betLine; }}
        public int BetMoney { get { return this.m_totalBetMoney; } }
        public int BaseBetMoney { get { return this.m_BaseBetMoney; } }

        public LuckyBetItem(bool[] betLine,int BaseBetMoney ,int totalBetMoney)
        {
            this.m_betLine = betLine;
            this.m_totalBetMoney = totalBetMoney;
            this.m_BaseBetMoney = BaseBetMoney;
        }
    }
}
