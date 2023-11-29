using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightControl.Models
{
    public class AppData
    {
        private volatile static AppData _singleton;
        static object key = new object();
        public static AppData Getinstance()
        {
            if(_singleton == null)
            {
                lock (key)
                {
                    _singleton = new AppData();
                }
            }
            return _singleton;
        }
        public string sLightManagerSetting = @"C:\Users\k-nguyen\Documents\Visual Studio 2015\Projects\yatteimasu\CodeDesign\LightControl_Vison_1+\LightControl\bin\Debug\Setting\";
        public const string sPathLightInfo = "LightManagerSetting.xml";
        public const string sPathLightSpare = "LightManagerSpareSetting.xml";
        public const string sLightManagerSetting_XsdPath = "LightManagerSettingXsd.xsd";
        public const string sLightSetting_XsdPath = "LightSettingXsd.xsd";

    }
}
