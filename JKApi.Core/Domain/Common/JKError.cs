using JKApi.Business.Enumeration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Business.Common
{
    public class JKError
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }

        public JKError(int errorCode, string errorMessage, string errorDetails)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.ErrorDetails = errorDetails;
        }

        public JKError(ErrorCode error, Exception ex)
        {
            this.ErrorCode = (int)error;
            this.ErrorMessage = GetDescription(error);
            this.ErrorDetails = ex.Message;
        }

        public JKError(ErrorCode error, string message)
        {
            this.ErrorCode = (int)error;
            this.ErrorMessage = GetDescription(error);
            this.ErrorDetails = message;
        }

        public String GetDescription(ErrorCode code)
        {
            Type type = code.GetType();
            string name = Enum.GetName(type, code);
            string description = null;

            if (name != null)
            {
                FieldInfo fieldInfo = type.GetField(name);
                if (fieldInfo != null)
                {
                    DescriptionAttribute attribute = Attribute
                        .GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attribute != null)
                    {
                        description = attribute.Description;
                    }
                }
            }

            return description;
        }
    }
}
