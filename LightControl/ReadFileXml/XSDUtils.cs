using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;


namespace LightControl.ReadFileXml
{
    public class XSDUtils
    {
        public static bool ValidateXml(string XmlPath, string XsdPath)
        {
            bool _bRet = true;
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(XmlPath);

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(null, XsdPath);

                xmlDocument.Schemas.Add(schemaSet);
                xmlDocument.Validate(ValidationEventHandler); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _bRet = false;
            }

            return _bRet;
        }


        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            { 
                Console.WriteLine($"XSD Validation Error: {e.Message}");
            }
        }

    }
}

