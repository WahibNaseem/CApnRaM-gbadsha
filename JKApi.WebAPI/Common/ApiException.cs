using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Http.ModelBinding;

namespace JKApi.WebAPI.Common
{
    public class ApiException : Exception
    {
        public enum ErrorCode
        {
            [Description("No error.")]
            None = 0,
            [Description("Invalid request data.")]
            InvalidRequestData = 1001,
            [Description("The is an error when execute data.")]
            FailExecute = 9001,
            [Description("The request data is invalid.")]
            InvalidRequest = 9002,
            [Description("Uploaded file is not provided.")]
            NoUploadFile = 3001,
            [Description("Custom Error.")]
            CustomError = 5001,
        }

        public ApiException(ErrorCode code, string message = null, Exception cause = null) : base(message, cause)
        {
            Code = code;
        }

        public ErrorCode Code { get; set; }

        public static string GetDescription(ErrorCode code)
        {
            var type = code.GetType();
            var name = Enum.GetName(type, code);
            string description = null;

            if (name != null)
            {
                var fieldInfo = type.GetField(name);
                if (fieldInfo != null)
                {
                    var attribute = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attribute != null)
                    {
                        description = attribute.Description;
                    }
                }
            }

            return description;
        }

        public static IList<string> GetErrors(ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage + " " + x.Exception).ToList();
        }
    }
}