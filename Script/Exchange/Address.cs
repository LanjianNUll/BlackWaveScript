//******************************************************************
// File Name:					Address
// Description:					Address class 
// Author:						lanjian
// Date:						3/7/2017 11:19:27 AM
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
    class Address
    {
        private string m_name;                      //收件人姓名 
        private string m_phone;                     //电话
        private string m_email;                     //电子邮箱   
        private string m_city;                      //城市
        private string m_detailAddr;                //详细地址

        public Address(string name, string phone, string email, string city, string detalAddr)
        {
            this.m_name = name;
            this.m_phone = phone;
            this.m_email = email;
            this.m_city = city;
            this.m_detailAddr = detalAddr;
        }
        public Address(DataObj data)
        {
            this.m_name = data.GetString("name");
            this.m_phone = data.GetString("phone");
            this.m_email = data.GetString("mail");
            this.m_city = data.GetString("city");
            this.m_detailAddr = data.GetString("detailAddr");
        }

        public string Name { get { return this.m_name; } }
        public string Phone { get { return this.m_phone; } }
        public string Email { get { return this.m_email; } }
        public string City { get { return this.m_city; } }
        public string DetailAddr { get { return this.m_detailAddr; } }


        //返回一个DataObj
        public DataObj GetDataObj()
        {
            DataObj data = new DataObj();
            data["name"] = this.Name;
            data["phone"] = this.Phone;
            data["mail"] = this.Email;
            data["city"] = this.City;
            data["detailAddr"] = this.DetailAddr;
            return data;
        }

    }
}
