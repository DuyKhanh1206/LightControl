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
       
        public static bool Load<ClassType>(string sXmlPath,string sXsdPath, out ClassType ReadData, out string ErrorMessage)
        {
            bool _bRet = true;
            ReadData = default(ClassType);
            ErrorMessage = string.Empty;
            if (true == _bRet)
            {
                if (false == System.IO.File.Exists(sXmlPath))
                {
                    ErrorMessage = "ユーザ設定ファイルが見つかりません。";
                    _bRet = false;
                }
                if (false == System.IO.File.Exists(sXsdPath))
                {
                    ErrorMessage += "ユーザ設定ファイルが見つかりません。";
                    _bRet = false;
                }
            }

            if (true == _bRet)
            {
                if (ReadFileXml.XSDUtils.ValidateXml(sXmlPath, sXsdPath) == false)
                {
                    string sFileName = Path.GetFileName(sXmlPath);
                    ErrorMessage = sFileName + "の整合性チェックに失敗しました。\n" + sFileName + "の内容を確認して下さい。";
                    _bRet = false;
                }
            }
            if (true == _bRet)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(sXmlPath, new UTF8Encoding(false)))
                    {
                        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(ClassType));                        
                        object obj = xs.Deserialize(sr);
                        ReadData = (ClassType)obj;
                    }
                }
                catch (Exception ex)
                {
                    string sFileName = Path.GetFileName(sXmlPath);
                    ErrorMessage = sFileName + "の読み込みに失敗しました。" ;
                    ErrorMessage += ex.Message; // có thể dùng hoặc không
                    _bRet = false;
                }
            }
            return _bRet;
        }
    }
}
