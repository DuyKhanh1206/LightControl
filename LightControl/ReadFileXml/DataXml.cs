using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightControl.ReadFileXml
{
    public class DataXml
    {
       
        public static bool Load<ClassType>(string path, out ClassType ReadData, out string ErrorMessage)
        {
            bool _bRet = true;
            ReadData = default(ClassType);
            ErrorMessage = string.Empty;
            if (true == _bRet)
            {
                if (false == System.IO.File.Exists(path))
                {
                    ErrorMessage = "Đường dẫn file không tồn tại: ユーザ設定ファイルが見つかりません。";
                    _bRet = false;
                }
            }
            if (true == _bRet)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path, new UTF8Encoding(false)))
                    {
                        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(ClassType));                        
                        object obj = xs.Deserialize(sr);
                        ReadData = (ClassType)obj;
                    }
                }
                catch (Exception ex)
                {
                    string sFileName = Path.GetFileName(path);
                    ErrorMessage = sFileName + "の読み込みに失敗しました。" ;
                    ErrorMessage += ex.Message; // có thể dùng hoặc không
                    _bRet = false;
                }
            }
            return _bRet;
        }
    }
}
