//******************************************************************
// File Name:					PersonImageProctor
// Description:					PersonImageProctor class 
// Author:						lanjian
// Date:						3/20/2017 5:06:57 PM
// Reference:
// Using:
// Revision History:
//******************************************************************
using FW.ResMgr;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FW.Role
{
    class PersonImageProctor
    {
        List<PersonImage> m_personImage;
        public PersonImageProctor()
        {
            m_personImage = new List<PersonImage>();
        }

        //--------------------------------------
        //private 
        //--------------------------------------v
        private void GetAllPersonImage()
        {
            m_personImage.Clear();
            JsonConfig jsonConfig = DatasMgr.RoleCfg;
            m_personImage.Add(new PersonImage(jsonConfig.GetJsonItem("102100001"), 102100001));
            m_personImage.Add(new PersonImage(jsonConfig.GetJsonItem("102100002"), 102100002));
            m_personImage.Add(new PersonImage(jsonConfig.GetJsonItem("102100003"), 102100003));
            m_personImage.Add(new PersonImage(jsonConfig.GetJsonItem("102100004"), 102100004));
        }

        //--------------------------------------
        //public 
        //--------------------------------------v

        public void Init()
        {
            //test
            GetAllPersonImage();
        }

        public List<PersonImage> GetPeronsImages()
        {
            return this.m_personImage;
        }

        public void Dispose()
        {
            m_personImage.Clear();
        }

    }
}
