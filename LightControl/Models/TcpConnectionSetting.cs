using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightControl.Models
{
    public class TcpConnectionSetting
    {   
        public string IpAdress { get; set; }       
        public int PortNum { get; set; }       
        public int OpenTimeOut { get; set; }       
        public int ReadTimeOut { get; set; }     
        public int WriteTimeOut { get; set; }
        public int ReLoadNumber { get; set; }
    }
}
