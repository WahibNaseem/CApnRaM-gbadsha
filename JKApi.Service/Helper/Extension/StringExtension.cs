using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.Helper.Extension
{
    public static class StringExtension
    {
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }
    }
}
