using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class XmlHelper
    {
        public string XmlSerializer<T> (T t)
        {
            string data = "";
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
                ser.Serialize(ms, t);
                data = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
            catch(Exception ex)
            {
                return "";
            }
            return data;
        }

        public T XmlDeserialize<T>(string data)
        {
            T t = default(T);
            try
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                t = (T)ser.Deserialize(stream);
            }
            catch (Exception ex)
            {
                return t;
            }
            return t;
        }
    }
}
