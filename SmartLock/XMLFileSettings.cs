using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace SmartLock
{
    public class XMLFileSettings
    {
        //Класс определяющий какие настройки есть в программе
        public class PropsFields
        {
            public string XMLFileName = Environment.CurrentDirectory + @"\TestKey\settings.xml";
            public string login = @"admin";     //настройки для хранения
            public string pass = "";
            public string bground = "finn.jpg";
            public string kFolder = Environment.CurrentDirectory + @"\TestKey";
            public string port = "";
            public int fails = 0;               //количество неудачных попыток    
            public string mailFrom = "";
            public string mailPass = "";
            public string mailSmtp = "";
            public string mailPort = "";
            public string mailTo = "";
        }
        //Класс работы с настройками
        public class Props
        {
            public PropsFields Fields;

            public Props()
            {
                Fields = new PropsFields();
            }
            //Запись настроек в файл
            public void WriteXml()
            {
                XmlSerializer ser = new XmlSerializer(typeof(PropsFields));

                TextWriter writer = new StreamWriter(Fields.XMLFileName);
                ser.Serialize(writer, Fields);
                writer.Close();
            }
            //Чтение насроек из файла
            public void ReadXml()
            {
                if (File.Exists(Fields.XMLFileName))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(PropsFields));
                    TextReader reader = new StreamReader(Fields.XMLFileName);
                    Fields = ser.Deserialize(reader) as PropsFields;
                    reader.Close();
                }
                else
                {
                    MessageBox.Show($"Отсутствует файл настроек {Fields.XMLFileName}.");
                }
            }
        }

    }
}
