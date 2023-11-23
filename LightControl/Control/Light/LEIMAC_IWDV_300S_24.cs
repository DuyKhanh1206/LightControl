using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightControl.Models;

namespace LightControl.Control.Light
{
    public class LEIMAC_IWDV_300S_24:LightPowerBase
    {
        public LEIMAC_IWDV_300S_24() : base(null)
        {
        }
        
        public override bool ChangeLightValue(List<LightPowerBaseSetting.LightCh> lstLightCh, int SetValue)
        {
            //int SetValue = (int)(SetValue * (999.0 / 100.0));   ______ Nếu tỉ lệ đầu vào là tỉ lệ %
            if (SetValue < _lightPowerBaseSetting.MinLightValue && SetValue > _lightPowerBaseSetting.MaxLightValue)
            {
                //foreach(var df in _lightPowerBaseSetting.lstLightCh)
                //{
                //    SetValue = df.LightChValueDefaul;
                //}
                return false;
            }
            string sCommand = "W12";
            string sValidResults = "W12ACK ";
            foreach(var ChNum in lstLightCh)
            {                
                sCommand += SetValue.ToString("0000");
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
                _lstFlowData.Insert(0, new FlowData(FlowStatus.Terminate, new List<string>() { "W120000", "W12ACK " }));              
            }
            if (true == LightManager.Getinstance().lpbs.StopedThread())
            {
            }
            return _bRet;
        }

        /// <summary> Add Command polling </summary>
        protected override bool PollingCommand(LightPowerBase lpb)
        {
            string sCommand = "R080000";
            string sValidResults = "R080000";
           
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
        //        if (sResult != "R080000")
        //            _bRet = false;
        //    }

        //    return _bRet ? FlowStatus.Polling : FlowStatus.Error;
        //}
    }
}
