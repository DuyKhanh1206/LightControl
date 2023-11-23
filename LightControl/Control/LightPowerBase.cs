﻿using LightControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightControl.Control
{
    public class LightPowerBase
    {
        public LightPowerBaseSetting _lightPowerBaseSetting;
        public TcpConnection _tcpConnection;
        protected DateTime _lastPollingRequestTime;
        private volatile bool _bThreadFlag;
        private Thread _threadMain;
        protected List<FlowData> _lstFlowData;
        private List<LightPowerBase> _lstLightPowerBases;
        private const int perStreamTime = 200;
        private int _reLoadNumber;
        private int ErrorNum;

        #region Event
        public delegate void DelegateResultNotification(object sender, ResultNotificationEvented arg);
        public event DelegateResultNotification ResultNotificationAction;
        public class ResultNotificationEvented : EventArgs
        {
            public bool Confirm { get; set; }
            public string Message { get; set; }
            public ResultNotificationEvented(bool confirm, string msg)
            {
                Confirm = confirm;
                Message = msg;
            }
        }

        public delegate void TerminateCompleted(object sender, EventArgs arg);
        public event TerminateCompleted TerminateCompletedAction;
        protected virtual void OnTerminateCompleted()
        {
            TerminateCompletedAction?.Invoke(this, new EventArgs());
        }
        #endregion

        /// <summary> Initialize</summary> 
        public LightPowerBase(List<LightPowerBase> lightPowerBases)
        {
            if (lightPowerBases != null)
            {
                _lstLightPowerBases = new List<LightPowerBase>();
                _lstLightPowerBases = lightPowerBases;
                _bThreadFlag = false;
                _threadMain = null;

            }
            else
            {
                _lightPowerBaseSetting = new LightPowerBaseSetting();
                _tcpConnection = new TcpConnection();
                _lastPollingRequestTime = DateTime.Now;
                _lstFlowData = new List<FlowData>();
                _lstFlowData.Add(new FlowData(FlowStatus.Initialize));
            }
        }
        public bool Load(string sPath)
        {
            bool _bRet = true;
            string message;
            LightPowerBaseSetting ReadData;

            if (ReadFileXml.DataXml.Load<LightPowerBaseSetting>(sPath, out ReadData, out message) == false)
            {
                _bRet = false;
            }
            else
            {
                _lightPowerBaseSetting = ReadData;
                _reLoadNumber = _lightPowerBaseSetting.TcpSetting.ReLoadNumber;
            }
            return _bRet;
        }

        protected class FlowData
        {
            public FlowStatus FlowStatus { get; private set; }
            public List<string> lstCommand { get; private set; }

            public FlowData(FlowStatus flowStatus, List<string> lstcommand = default(List<string>))
            {
                FlowStatus = flowStatus;
                lstCommand = lstcommand;
            }
        }

        /// <summary> Add Command and command validate  </summary> 
        public bool Request(List<string> RequestCommand)
        {
            bool _bRet = true;           
            if (_lstFlowData.Count >= 4)
                _bRet = false;
            else
                _lstFlowData.Insert(0, new FlowData(FlowStatus.Request, RequestCommand));
            return _bRet;
        }              

        #region Thread
        public bool StopedThread()
        {     
            // count power finish      
            if (_threadMain == null)
                return true;
            _bThreadFlag = false;
            do
            {
                _threadMain.Join(100);
                //_threadMain.Abort();
                Console.WriteLine("Thread Stoped Is : " + _threadMain.IsAlive);
            } while (_threadMain.IsAlive);
            // có nên load lại từ đầu? tạo 1 chu trình lặp. nếu 3 lần k đk thì thông báo và thoát khỏi chương trình
            return true;
        }

        public bool StartThread()
        {
            if (true == _bThreadFlag)
                return false;
            _threadMain = new Thread(ThreadMain);        
            _bThreadFlag = true;
            _threadMain.IsBackground = true;
            _threadMain.Start();
            return true;
        }

        private void ThreadMain()
        {
            while (_bThreadFlag)
            {
                foreach (var lightPowerBase in _lstLightPowerBases)
                {
                    DateTime StartTime = DateTime.Now;

                    if (lightPowerBase._lstFlowData.Count > 0)
                    {
                        FlowData switchData = lightPowerBase._lstFlowData[0];
                        lightPowerBase._lstFlowData.RemoveAt(0);
                        switch (switchData.FlowStatus)
                        {
                            case FlowStatus.Initialize:
                                {
                                    lightPowerBase._lstFlowData.Add(new FlowData(FlowInitialize(lightPowerBase)));
                                    break;
                                }
                            case FlowStatus.Open:
                                {
                                    lightPowerBase._lstFlowData.Add(new FlowData(FlowOpen(lightPowerBase)));
                                    break;
                                }
                            case FlowStatus.Polling:
                                {
                                    lightPowerBase._lstFlowData.Add(new FlowData(lightPowerBase.FlowPolling(lightPowerBase)));
                                    break;
                                }
                            case FlowStatus.Request:
                                {
                                    FlowRequest(lightPowerBase, switchData);
                                    break;
                                }
                            case FlowStatus.Error:
                                {
                                    lightPowerBase._lstFlowData.Add(new FlowData(FlowError(lightPowerBase)));
                                    break;
                                }
                            case FlowStatus.Terminate:
                                {
                                    lightPowerBase._lstFlowData.Add(new FlowData(FlowTerminate(lightPowerBase, switchData)));
                                    break;
                                }
                            case FlowStatus.Finished:
                                {
                                    //FlowFinished();
                                    break;
                                }
                            default: break;
                        }
                    }

                    DateTime EndTime = DateTime.Now;
                    TimeSpan TimePeriod = EndTime - StartTime;
                    if ((int)TimePeriod.TotalMilliseconds < perStreamTime)
                    {
                        Thread.Sleep(perStreamTime - (int)TimePeriod.TotalMilliseconds);
                    }
                    else
                    {

                    }
                    EndTime = DateTime.Now;
                    TimePeriod = EndTime - StartTime;
                }
                Console.WriteLine("--------------------------------------------");                
            }            
        }

        #endregion

        /// settup bằng câu lệnh
        public bool LpbSettingIsCommand(string sCommand)
        {
            List<string> lstCommands = new List<string>();
            lstCommands.Add(sCommand);
            return Request(lstCommands);
        }

        public virtual bool Terminate()
        {
            return true;
        }
        /// <summary> 特定の CH のソース値を変更します  </summary> 
        public virtual bool ChangeLightValue(List<LightPowerBaseSetting.LightCh> lstLightCh, int SetValue)
        {
            return true;
        }
        /// <summary> ポーリングコマンド </summary> 
        protected virtual bool PollingCommand(LightPowerBase lpb)
        {
            return true;
        }
        public enum FlowStatus
        {
            Initialize,
            Open,
            Polling,
            Error,
            Request,
            Terminate,
            None,
            Finished,
        }

        /// <summary> イニシャライズ  </summary> 
        private FlowStatus FlowInitialize(LightPowerBase lpb )
        {
            Console.WriteLine("Running {0} in: Initialize",lpb._lightPowerBaseSetting.LightPowerName);
            for (int i = 0; i < lpb._reLoadNumber; i++)
            {           
                if (!lpb._tcpConnection.TcpSetting(lpb._lightPowerBaseSetting.TcpSetting))
                {
                    continue;
                }
                return FlowStatus.Open;
            }
            return FlowStatus.Error;           
        }

        /// <summary>オープンする</summary>
        private  FlowStatus FlowOpen(LightPowerBase lpb)
        {
            Console.WriteLine("Running {0} in: Open", lpb._lightPowerBaseSetting.LightPowerName);
            for (int i = 0; i < lpb._reLoadNumber; i++)
            {
                if (!lpb._tcpConnection.Connection())
                {
                    continue;
                }

                return FlowStatus.Polling;
            }
            return FlowStatus.Error; 
        }

        /// <summary> ポーリング , chờ & check kết nối khi không có command nào </summary> 
        private FlowStatus  FlowPolling(LightPowerBase lpb)
        {
            Console.WriteLine("Running {0} in: Polling", lpb._lightPowerBaseSetting.LightPowerName);
            bool _bRet = true;
            if(lpb != null)
            {
                _bRet &= lpb.PollingCommand(lpb);
            }
            if (_bRet)
                return FlowStatus.Polling;
            return FlowStatus.Error;
        }
       

        /// <summary>リクエストする</summary>
        private void FlowRequest(LightPowerBase lpb , FlowData flowData)
        {
            bool _bRet = true;
            string sResult = string.Empty;
            for (int i = 0; i < lpb._reLoadNumber; i++)
            {
                if (lpb._tcpConnection.Request(flowData.lstCommand[0], out sResult) == true)
                {
                    _bRet = true;
                    break;
                }
                _bRet = false;
            } 
            if (_bRet == true)
            {
                if(flowData.lstCommand[0] != "R080000")
                {
                    if (flowData.lstCommand.Count >= 2)
                    {
                        _bRet &= flowData.lstCommand[1] == sResult;                      
                    }
                    lpb.ResultNotificationAction(this, new ResultNotificationEvented(_bRet, sResult));
                }
                else
                {

                }
            }
            else
            {
                lpb._lstFlowData.Insert(0, new FlowData(FlowStatus.Error));
            }
        }
        /// <summary> Flow Error </summary> 
        private  FlowStatus FlowError(LightPowerBase lpb)
        {
            Console.WriteLine("Running {0} in: Error", lpb._lightPowerBaseSetting.LightPowerName);
            lpb._tcpConnection.DisConnection();
            lpb._lstFlowData.Clear();
             lpb.ErrorNum++;
            if (lpb.ErrorNum >= lpb._reLoadNumber)
            {
                return FlowStatus.Terminate;
            }            
            return  FlowStatus.Initialize;
        }

        /// <summary>フローを終了する</summary>
        private  FlowStatus FlowTerminate(LightPowerBase lpb,FlowData terminateCommand)
        {
            Console.WriteLine("đang chạy {0} ở Terminate", lpb._lightPowerBaseSetting.LightPowerName);

            string sResult = string.Empty;
            for (int i = 0; i < lpb._reLoadNumber; i++)
            {               
                if(terminateCommand.lstCommand != null)
                {
                    if (lpb._tcpConnection.Request(terminateCommand.lstCommand[0], out sResult) == true)
                    {
                        break;
                    }
                }               
            }
            lpb._tcpConnection.DisConnection();
            lpb._lstFlowData.Clear();           
            return FlowStatus.Finished;
        }
        private void FlowFinished()
        {
            Console.WriteLine("Running {0} in: Finish");
            //TerminateCompletedAction(this, OnTerminateCompleted( ));
            OnTerminateCompleted();

            //LightManager.Getinstance().lpbs.StopedThread();
            //Form1.ActiveForm.Close();
        }
    }
}
