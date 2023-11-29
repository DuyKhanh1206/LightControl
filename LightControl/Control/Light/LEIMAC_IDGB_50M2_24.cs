using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightControl.Models;

namespace LightControl.Control.Light
{
    public class LEIMAC_IDGB_50M2_24:LightPowerBase
    {
        public LEIMAC_IDGB_50M2_24() : base(null)
        {
        }
        public override bool ChangeLightValue(List<LightPowerBaseSetting.LightCh> lstLightCh, int SetValue)
        {
           
            if (SetValue < _lightPowerBaseSetting.MinLightValue || SetValue > 999 )
            {
                return false;
            }
            else if (SetValue > _lightPowerBaseSetting.MaxLightValue && SetValue <= 999)
            {
                SetValue = (SetValue * _lightPowerBaseSetting.MaxLightValue) / 999;
            }
            string sCommand = "W11";
            string sValidResults = "W11ACK0";
            
            foreach (var ChNum in lstLightCh)
            {
                sCommand += ChNum.LightChNumber.ToString("00") + SetValue.ToString("0000");
            }
            List<string> lstCommands = new List<string>();
            lstCommands.Add(sCommand);   
            lstCommands.Add(sValidResults);  

            return Request(lstCommands);
        }
        
        /// <summary>Add Command and  flowStatus: Light Command Off </summary> 
        public override bool Terminate()
        {
            bool _bRet = true;

            if (_bRet == true)
            {
                foreach (var lightCh in _lightPowerBaseSetting.lstLightCh)
                {
                    _lstFlowData.Insert(0, new FlowData(FlowStatus.Terminate, new List<string>() { "W11"+lightCh.LightChNumber.ToString("00") + "0000", "W11ACK0" }));
                }
            }
            else
            {
                _bRet = false;
            }
            if (true == LightManager.Getinstance().lpbs.StopedThread())
            {
            }
            return _bRet;
        }
        /// <summary> Add Command polling </summary>
        protected override bool PollingCommand()
        {
            string sCommand = "R080000";
            string sValidResults = "R08010000020000";

            List<string> lstCommands = new List<string>();
            lstCommands.Add(sCommand);
            lstCommands.Add(sValidResults);
            return Request(lstCommands);
        }

        //protected override FlowStatus FlowPolling(LightPowerBase lpb)
        //{
        //    Console.WriteLine("Running {0} in: Polling", lpb._lightPowerBaseSetting.LightPowerName);
        //    bool _bRet = true;
        //    string sResult = string.Empty;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (lpb._tcpConnection.Request("R080000", out sResult) == true) 
        //        {
        //            _bRet = true;
        //            break;
        //        }
        //        _bRet = false;
        //    }

        //    if (_bRet == true)
        //    {
        //        if (sResult != "R08010000020000")
        //            _bRet = false;
        //    }

        //    return _bRet ? FlowStatus.Polling : FlowStatus.Error;
        //}
    }
}
