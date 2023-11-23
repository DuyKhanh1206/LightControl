using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;


namespace LightControl.ReadFileXml
{
    public class XSDUtils
    {
        public static bool XmlValidate(string xmlPath, string xsdString)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(xmlPath);

                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(null, XmlReader.Create(new StringReader(xsdString)));

                xml.Schemas.Add(schemas);
                xml.Validate(ValidationEventHandler);

                Console.WriteLine("Validation succeeded.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Validation failed: {ex.Message}");
                return false;
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                throw new Exception($"Validation Error: {e.Message}");
            }
        }

    }
}

