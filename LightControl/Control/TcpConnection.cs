using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightControl.Models;


namespace LightControl.Control
{
    public class TcpConnection
    {
        private TcpConnectionSetting _tcpConnectionSetting;
        private System.Net.Sockets.NetworkStream _netWorkStream;
        private System.Net.Sockets.TcpClient _tcpClient;
        /// <summary>Initialize</summary>
        public TcpConnection()
        {
            _tcpConnectionSetting = null;
            _netWorkStream = null;
            _tcpClient = null;
        }

        /// <summary>   IP チェックする </summary>
        static public bool IsIPAddressCorrect(string IpTest)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^((\d|[01]?\d\d|2[0-4]\d|25[0-5])\.){3}(\d|[01]?\d\d|2[0-4]\d|25[0-5])$");
            return reg.IsMatch(IpTest);
        }
        /// <summary>   Port チェックする </summary>
        static public bool IsPortCorrect(int PortTest)
        {
            if (PortTest < 0 || PortTest > 65535)
                return false;
            return true;
        }

        /// <summary> Ping xem địa chỉ IP và Port có thể kết nối hay không </summary>
        private bool PingCheckIp(string sIpTest, int iPort)
        {
            using (var ping = new System.Net.NetworkInformation.Ping())
            {
                System.Net.NetworkInformation.PingReply rep;
                try
                {
                    rep = ping.Send(sIpTest, iPort);
                    if (rep.Status == System.Net.NetworkInformation.IPStatus.Success)
                        return true;
                }
                catch 
                {
                }
            }
            return false;
        }

        /// <summary> Setting data Connection   設定Load </summary>
        public bool TcpSetting(TcpConnectionSetting TcpConnectionSetting)
        {
            bool _bRet = true;
            try
            {
                _tcpClient = new System.Net.Sockets.TcpClient();
                _tcpConnectionSetting = new TcpConnectionSetting();
                _tcpConnectionSetting = TcpConnectionSetting;
            }
            catch
            {
                _bRet = false;
            }
          
            return _bRet;
        }

        /// <summary> Connection With TCP/IP    オープンする </summary>
        public bool Connection()
        {
            bool _bRet = true;
            try
            {
                if (IsIPAddressCorrect(_tcpConnectionSetting.IpAdress) && IsPortCorrect(_tcpConnectionSetting.PortNum))
                {
                    if(PingCheckIp(_tcpConnectionSetting.IpAdress, _tcpConnectionSetting.PortNum))
                    {
                        _tcpClient.ConnectAsync(_tcpConnectionSetting.IpAdress, _tcpConnectionSetting.PortNum).Wait(_tcpConnectionSetting.OpenTimeOut);
                        _netWorkStream = _tcpClient.GetStream();
                        _netWorkStream.ReadTimeout = _tcpConnectionSetting.ReadTimeOut;
                        _netWorkStream.WriteTimeout = _tcpConnectionSetting.WriteTimeOut;
                    }
                    else
                    {
                        Console.WriteLine("ping out");
                        _bRet = false;
                    }
                }
                else
                {
                    _bRet = false;
                }
            }
            catch
            {
                _bRet = false;
            }
            return _bRet;
        }

        /// <summary> DisConnection  クローズする  </summary>
        public bool DisConnection()
        {
            bool _bRet = true;
            try
            {
                _tcpClient.Close();
                _netWorkStream.Close();
            }
            catch 
            {
                _bRet = false;
            }
            _netWorkStream = null;
            _tcpClient = null;
            _tcpConnectionSetting = null;
            return _bRet;
        }

        /// <summary> コマンドを送信して応答を読み取る Gửi command và đọc phản hồi </summary>
        public bool Request(string command, out string receiveData)
        {
            bool _bRet = true;
            receiveData = string.Empty;

            if (_bRet == true)
            {
                _bRet &= SendCommand(command);
            }

            if (_bRet == true)
            {
                _bRet &= ReadFeedback(out receiveData);
            }
            return _bRet;
        }

        /// <summary> コマンドを送信する gửi command </summary>
        public bool SendCommand(string command)
        {
            bool _bRet = true;
            try
            {
                byte[] sendByte = System.Text.Encoding.UTF8.GetBytes(command);
                _netWorkStream.Write(sendByte, 0, sendByte.Length);
            }
            catch
            {
                _bRet = false;
            }
            return _bRet;           
        }

        /// <summary> フィードバックを読む đọc phản hồi </summary>
        public bool ReadFeedback(out string receiveData)
        {
            bool _bRet = true;
            receiveData = string.Empty;
            try
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    byte[] readbyte = new byte[128];
                    int readSize = 0;
                    try
                    {
                        do
                        {
                            readSize = _netWorkStream.Read(readbyte, 0, readbyte.Length);
                            if (readSize == 0)
                            {
                                Console.WriteLine("サーバが切断しました"); 
                                _bRet = false;
                                break; // server disconnect thì vào đây
                            }
                            ms.Write(readbyte, 0, readSize);
                        } while (_netWorkStream.DataAvailable);

                        string response = System.Text.Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                        response = response.TrimEnd('\n');
                        if (response != null && response != "")
                        {
                            receiveData = response;
                        }
                    }
                    catch(Exception ex)
                    {
                        var a = (System.Net.Sockets.SocketException)ex.InnerException;
                        if (a != null)
                        {
                            if (a.ErrorCode == 10060)
                            {
                                Console.WriteLine("TimeOut Server");
                            }
                            else
                            {
                                Console.WriteLine("読み込みエラーが発生しました" + a.ErrorCode);
                            }
                        }
                        else if (ex.HResult == -2146233080 || ex.HResult == -2146233079)
                        {
                            Console.WriteLine("サーバが切断しました");
                        }
                        else
                        {
                            Console.WriteLine("読み込みエラーが発生しました" + ex.HResult);
                        }
                        _bRet = false;
                    }
                }
            }
            catch
            {
                _bRet = false;
            }
            return _bRet;
        }
    }
}
