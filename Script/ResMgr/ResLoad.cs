//******************************************************************
// File Name:					ResLoad.cs
// Description:					ResLoad class 
// Author:						wuwei
// Date:						2017.01.12
// Reference:
// Using:
// Revision History:
//******************************************************************
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace FW.ResMgr
{
    static class ResLoad
    {
        //--------------------------------------
        //private 
        //--------------------------------------
        private static string RemoveExt(string file)
        {
            int index = file.LastIndexOf('.');
            if (index < 0)
                return file;
            return file.Remove(index);
        }

        //--------------------------------------
        //public 
        //--------------------------------------
        public static UnityEngine.Object Load(string file)
        {
            return Resources.Load(RemoveExt(file));
        }

        public static T Load<T>(string file) where T : UnityEngine.Object
        {
            return Resources.Load<T>(RemoveExt(file));
        }
    }
}