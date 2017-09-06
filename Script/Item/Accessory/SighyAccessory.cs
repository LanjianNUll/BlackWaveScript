//******************************************************************
// File Name:					SighyAccessory
// Description:					SighyAccessory class 
// Author:						lanjian
// Date:						12/29/2016 3:26:31 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using System;
using System.Collections.Generic;

using Network.Serializer;
using FW.ResMgr;

namespace FW.Item
{
    /// <summary>
    /// 瞄具配件
    /// </summary>
    class SighyAccessory : AccessoryBase
    {
        public static AccessoryBase Create(string id, JsonItem item)
        {
            return new SighyAccessory(id, item);
        }

        public SighyAccessory(string id, JsonItem item) : base(id, item)
        {
            this.m_type = AccessoryType.Sight;
        }
        //--------------------------------------
        //properties 
        //--------------------------------------
        //精准直   开火精准影响系数   移动精准影响系数  暴击率
        public float Accuracy { get { return this.JsonItem.Get("accuracy").AsFloat(); } }
        public float Accuracybyfire { get { return this.JsonItem.Get("accuracybyfire").AsFloat(); } }
        public float Accuracybymove { get { return this.JsonItem.Get("accuracybymove").AsFloat(); } }
        public float Critical { get { return this.JsonItem.Get("critical").AsFloat(); } }

        //--------------------------------------
        //public 
        //--------------------------------------
        //返回对比的字符串
        public string[] Compare(SighyAccessory muzzle)
        {
            string[] addOrSub = new string[5];
            if (this.Accuracy - muzzle.Accuracy > 0)
            {
                addOrSub[0] = "+" + (this.Accuracy - muzzle.Accuracy);
            }
            else
            {
                addOrSub[0] = (this.Accuracy - muzzle.Accuracy).ToString();
            }
            if (this.Accuracybyfire - muzzle.Accuracybyfire > 0)
            {
                addOrSub[1] = "+" + (this.Accuracybyfire - muzzle.Accuracybyfire);
            }
            else
            {
                addOrSub[1] = (this.Accuracybyfire - muzzle.Accuracybyfire).ToString();
            }
            if (this.Accuracybymove - muzzle.Accuracybymove > 0)
            {
                addOrSub[2] = "+" + (this.Accuracybymove - muzzle.Accuracybymove);
            }
            else
            {
                addOrSub[2] = (this.Accuracybymove - muzzle.Accuracybymove).ToString();
            }
            if (this.Critical - muzzle.Critical > 0)
            {
                addOrSub[3] = "+" + (this.Critical - muzzle.Critical);
            }
            else
            {
                addOrSub[3] = (this.Critical - muzzle.Critical).ToString();
            }
            if (this.Gravity - muzzle.Gravity > 0)
            {
                addOrSub[4] = "+" + (this.Gravity - muzzle.Gravity);
            }
            else
            {
                addOrSub[4] = (this.Gravity - muzzle.Gravity).ToString();
            }
            if (String.IsNullOrEmpty(addOrSub[0]))
                return null;
            return addOrSub;
        }
    }
}
