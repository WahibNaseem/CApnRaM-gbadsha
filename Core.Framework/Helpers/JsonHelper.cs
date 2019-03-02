using Core.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class JsonHelper
    {
        public string JsonSerializer<T>(T t)
        {
            string jsonString ="";
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, t);
                jsonString = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
            }
            catch(Exception ex)
            {
                LoggerFactory.CreateLogger().Log(ex.Message, LogType.Error);
               
            }
            return jsonString;
        }
        /// <summary>
        /// JSON Deserialization
        /// </summary>
        public T JsonDeserialize<T>(string jsonString)
        {
            T obj = default(T);
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                obj = (T)ser.ReadObject(ms);
                return obj;
            }
            catch(Exception ex)
            {
                LoggerFactory.CreateLogger().Log(ex.Message, LogType.Error);
               
            }
            return obj;
        }

    }
}
