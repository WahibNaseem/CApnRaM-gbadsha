#region Using namespaces.
using System.Web.Script.Serialization;
using System.Data;
using System.Collections.Generic;
using System;

#endregion

namespace JKApi.Core
{
    public static class JSONHelper
    {

        public static string ToJSON(this object obj)
        {
            var serializer = new JavaScriptSerializer();
            try
            {
                return serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}