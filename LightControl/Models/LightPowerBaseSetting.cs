using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightControl.Models
{
    public class LightPowerBaseSetting
    {
        public string LightPowerName { get; set; }
        public List<LightCh> lstLightCh { get; set; }
        public int MinLightValue { get; set; }
        public int MaxLightValue { get; set; }      

        public TcpConnectionSetting TcpSetting { get; set; }

        /// <summary>コンストラクタ</summary>
        public LightPowerBaseSetting()
        {
            lstLightCh = new List<LightCh>(); // lstLightCh 01 : LightChNumber 01; name down
            TcpSetting = new TcpConnectionSetting();
        }

        /// <summary>照明Ch毎の設定</summary>     
        public class LightCh
        {
            public int LightChNumber { get; set; }      // Ch number 01 , 02 ,           
            public string LightChName { get; set; }         // light top, down
            public int LightChValueDefaul{get; set; }
           
        }

    }
}
