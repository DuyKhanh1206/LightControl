using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightControl.Models
{
    public class LightManagerSetting
    {
        //Danh sách cài đặt lớp chiếu sáng 
        public List<LightSetting> lstLightSetting { get; set; }

        public LightManagerSetting()
        {
            lstLightSetting = new List<LightSetting>();
        }
             
        public class LightSetting
        {
            // loại kết nối là tcp hay khác
            public string LightType { get; set; }
           // tên loại thiết bị IWDV hay IDGB
            public string LightModelName { get; set; }
            // Đường dẫn cài đặt cho loại thiết bị đó(IWDV or IDGB)
            public string LightSettingPath { get; set; }
           
        }
    }
}
