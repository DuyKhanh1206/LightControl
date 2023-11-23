using LightControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightControl.Control
{
    public class LightManager
    {
        private static volatile LightManager _singleton;
        static object key = new object();
        public static LightManager Getinstance()
        {
            if(_singleton == null)
            {
                lock (key)
                {
                    _singleton = new LightManager();
                }
            }
            return _singleton;
        }
        
        private const string sPathLightInfo = "LightManagerSetting.xml";
        private const string sPathLightSpare = "LightManagerSpareSetting.xml";
        private const int ReLoad = 3;

        public List<LightPowerBase> _lstLightPowerBase;
        public LightPowerBase lpbs;
        public List<string> LstLightPowerBaseName;
        public LightManager()
        {
            _lstLightPowerBase = new List<LightPowerBase>();
            
        }
        public bool LoadLightInfo(string sPath)
        {
            bool _bRet = true;
            LightManagerSetting DataManagrSetting = new LightManagerSetting();
            LstLightPowerBaseName = new List<string>();
            string message = string.Empty;
            
            for (int i = 0; i < ReLoad; i++)
            {
                _bRet = true;
                if (_bRet)
                {
                    string spath = System.IO.Path.Combine(sPath, sPathLightInfo);
                    if (ReadFileXml.DataXml.Load<LightManagerSetting>(spath, out DataManagrSetting, out message) == false)
                    {
                        spath = System.IO.Path.Combine(spath, sPathLightSpare);
                        if (ReadFileXml.DataXml.Load<LightManagerSetting>(spath, out DataManagrSetting, out message) == false)
                        {
                            _bRet = false;
                            continue;
                        }                        
                    }
                }
                if (_bRet)
                {
                    foreach (var settingLight in DataManagrSetting.lstLightSetting)
                    {
                       
                        LightPowerBase lpb = LightPowerInstance(settingLight);
                        if (lpb == null)
                        {
                            _bRet = false;
                            break;
                        }

                        if (lpb.Load(System.IO.Path.Combine(sPath, settingLight.LightSettingPath)) == false)
                        {
                            _bRet = false;
                            break;
                        }
                        LstLightPowerBaseName.Add(lpb._lightPowerBaseSetting.LightPowerName);
                        _lstLightPowerBase.Add(lpb);
                    }
                }
                if (_bRet)
                {
                    lpbs = new LightPowerBase(_lstLightPowerBase);
                    if (!lpbs.StartThread())
                    {
                        _lstLightPowerBase.Clear();
                        _bRet = false;
                        continue;
                    }
                }
                if (!_bRet)
                {
                    continue;
                }
                return _bRet;
            }
            return _bRet;
           
        }
        private LightPowerBase LightPowerInstance(LightManagerSetting.LightSetting SingleSetting)
        {
            string sNameSpace = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().First(x => x.Name == GetType().Name).Namespace;
            LightPowerBase lps = null;
            Type instanceType = null;
            switch (SingleSetting.LightType)
            {
                case "TCPIP":
                    {
                        switch (SingleSetting.LightModelName)
                        {
                            case "LEIMAC_IWDV_300S_24":
                                instanceType = Type.GetType(sNameSpace + ".Light.LEIMAC_IWDV_300S_24");
                                if (instanceType != null)
                                    lps = (LightPowerBase)Activator.CreateInstance(instanceType, new object[] { });//≒ var lps = new ControlLightIWDV()                      
                                break;

                            case "LEIMAC_IDGB_50M2_24":
                                instanceType = Type.GetType(sNameSpace + ".Light.LEIMAC_IDGB_50M2_24");
                                if (instanceType != null)
                                    lps = (LightPowerBase)Activator.CreateInstance(instanceType, new object[] { });
                                break;
                                
                            case "LEIMAC_IWDV_600M2_24":
                                instanceType = Type.GetType(sNameSpace + ".Light.LEIMAC_IWDV_600S_24");
                                if (instanceType != null)
                                    lps = (LightPowerBase)Activator.CreateInstance(instanceType, new object[] { });
                                break;

                        }
                        break;
                    }
                case "DIO?"://TCPIP以外
                    {
                        break;
                    }
            }
            return lps;
        }
         

        /// <summary> Lấy ra lps với tên tương ứng </summary>
        public bool GetLightPower(string LightPowerName, out LightPowerBase lpb)
        {
            bool _bRet = true;
            lpb = _lstLightPowerBase.Find(x => x._lightPowerBaseSetting.LightPowerName == LightPowerName);
            if (lpb == null)
            {
                _bRet = false;
            }
            return _bRet;
        }
        /// <summary> </summary> 
        private bool GetLight(string LightChName, LightPowerBase lpb, out List<LightPowerBaseSetting.LightCh> lstLightCh)
        {
            bool _bRet = true;
            lstLightCh = lpb._lightPowerBaseSetting.lstLightCh.FindAll(x => x.LightChName == LightChName); 
            if (lstLightCh.Count <= 0)
            {
                _bRet = false;
            }
            return _bRet;
        }

        /// <summary>  電源内のすべてのライトを同じ値に変更します。thay đổi tất cả đèn của 1 power</summary> 
        public bool ChangeLightValue(string LightPowerName, int value)
        {
            bool _bRet = true;
            LightPowerBase lpb;
            if (GetLightPower(LightPowerName, out lpb))
            {
               foreach( var x in lpb._lightPowerBaseSetting.lstLightCh)
                {
                    ChangeLightChValue(LightPowerName, x.LightChName, value);
                }
            }
            else
            {
                _bRet = false;
            }
            return _bRet;
        }
        /// <summary> CHポートで値を変更します thay đổi 1 cổng Ch bất kì của 1 đèn </summary> 
        public bool ChangeLightChValue(string LightPowerName, string LightChName, int value)
        {
            bool _bRet = true;
            LightPowerBase lpb;
            List<LightPowerBaseSetting.LightCh> lstLightCh;
            if (GetLightPower(LightPowerName, out lpb))
            {
                if (GetLight(LightChName, lpb, out lstLightCh))
                {
                    _bRet &= lpb.ChangeLightValue(lstLightCh, value);                    
                }
                else
                {
                    _bRet = false;
                }
            }
            else
            {
                _bRet = false;
            }
            return _bRet;
        }

        /// <summary>コマンドを使用してすべての電源の値を変更します。Read only </summary> 
        public bool AllLightChangeIsCommand(string value)
        {
            bool _bRet = true;

            foreach (var lpb in _lstLightPowerBase)
            {
                _bRet &= SettingIsCommand(lpb._lightPowerBaseSetting.LightPowerName, value);
            }
            return _bRet;
        }

        /// <summary>すべてのソース値を 1 つの特定の値に変更します。 </summary> 
        public bool AllLightChangeIsValue(int value)
        {
            bool _bRet = true;
            // set các trường hợp nếu > 255 đối với IDGB
            foreach (var lpb in _lstLightPowerBase)
            {
                _bRet &= ChangeLightValue(lpb._lightPowerBaseSetting.LightPowerName, value);
            }
            return _bRet;
        }
        
        //Buộc tắt tất cả các lps
        //public bool AllLightTerminate()
        //{
        //    bool _bRet = true;

        //    foreach (var x in _lstLightPowerBase)
        //    {
        //        _bRet &= x.Terminate();
        //    }
        //    return _bRet;
        //}

        public bool AllLightTerminateNew()
        {
            return _lstLightPowerBase.All(x => x.Terminate());
        }

        /// <summary>コマンド文字列を送信する gửi đi 1 chuỗi lệnh </summary> 
        public bool SettingIsCommand(string LightPowerName, string command)
        {
            bool _bRet = true;
            LightPowerBase lpb;
            if (GetLightPower(LightPowerName, out lpb))
            {
                lpb.LpbSettingIsCommand(command);
            }
            else
            {
                _bRet = false;
            }
            return _bRet;
        }

    }
}
