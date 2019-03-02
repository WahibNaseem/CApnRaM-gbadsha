using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.Service
{
    public static class UtilityService
    {

        #region format

        public static string Percentage(decimal s)
        {
            string tmp = "";
            if (s % 1 > 0)
            {
                tmp = s.ToString("0.00");
            }
            else
            {
                tmp = s.ToString("0");
            }

            return Percentage(tmp);
        }
        //===========================================
        public static string Percentage(double s)
        {
            string tmp = "";
            if (s % 1 > 0)
            {
                tmp = s.ToString("0.00");
            }
            else
            {
                tmp = s.ToString("0");
            }

            return Percentage(tmp);
        }
        //===========================================
        public static string Percentage(string s)
        {
            if (s != "")
            {
                s = s.Trim();
                //s = s.Replace("%", "");
                if (s.IndexOf("%") < 0) { s = s + "%"; }
            }
            return s;
        }
        //===========================================
        public static string Decimal(decimal s)
        {
            string tmp = "";
            if (s % 1 > 0)
            {
                tmp = s.ToString("0.00");
            }
            else
            {
                tmp = s.ToString("0");
            }

            return Decimal(tmp);
        }
        //===========================================
        public static string Decimal(double s)
        {

            string tmp = "";
            if (s % 1 > 0)
            {
                tmp = s.ToString("0.00");
            }
            else
            {
                tmp = s.ToString("0");
            }

            return Decimal(tmp);
        }
        //===========================================
        public static string Decimal(string s)
        {
            if (s != "")
            {
                s = s.Trim();
                s = s.Replace("%", "");
                s = s.Replace("$", "");
            }
            return s;
        }


        //===========================================        
        public static string PhoneNo(string s)
        {
            string tmp = "";
            if (!(s == "" || s == null))
            {
                s = s.Trim();
                if (s.Length == 10)
                {
                    tmp = "(" + s.Substring(0, 3) + ") ";
                    tmp += s.Substring(3, 3) + "-" + s.Substring(6, 4);
                }
                else if (s.Length == 7)
                {
                    tmp = s.Substring(0, 3) + "-" + s.Substring(3, 4);
                }
                else { tmp = s; }
            }
            return tmp;
        }
        //===========================================
        public static string Currency(int d)
        {
            //-- this for UI purpose will display number without the negative sign.
            if (d < 0) { d *= -1; }
            return Currency(d.ToString());
        }
        //===========================================
        public static string Currency(double d)
        {
            //--this for UI purpose will display number without the negative sign.
            if (d < 0) { d *= -1; }
            return Currency(d.ToString());
        }
        //===========================================
        public static string Currency(decimal d)
        {
            //-- this for UI purpose will display number without the negative sign.
            //if (d < 0) { d *= -1; }
            return Currency(d.ToString());
        }
        //===========================================
        public static string Currency(string d)
        {
            d = d == "" ? "0" : d;
            d = d.Replace("$", "");
            d = d.Replace("-", "");
            d = d.Replace("\'", "");
            double temp = Convert.ToDouble(d);
            return String.Format("{0:C}", temp);
        }
        public static string CurrencyNegative(string d)
        {
            d = d == "" ? "0" : d;
            d = d.Replace("$", "");
            return String.Format("{0:C}", Convert.ToDouble(d));
        }
        //===========================================
        public static decimal ScrubCurrency(string s)
        {
            decimal r = 0;

            if (s != "")
            {
                s = s.Replace("$", "");
                s = s.Replace("(", "");
                s = s.Replace(")", "");
                s = s.Replace("%", "");
                r = Convert.ToDecimal(s);
            }

            return r;

        }
        //===========================================       
        public static string Email(string s)
        {
            return s.Replace("@", "@<br>").Replace(".", "<br>.");
        }
        //===========================================
        public static string TextToDisplay(string s)
        {
            string tmp = "";
            int x = s.IndexOf("Original Message");
            if (x > 0)
            {
                tmp = s.Substring(x, s.Length - (x + 10));
                tmp = tmp.Replace("Original Message", "<br>Original Message<br>");
                tmp = tmp.Replace("-", " ");
                tmp = tmp.Replace("@", "<br>@");
                s = s.Substring(0, x) + tmp;
            }
            s = s.Replace("\r\n", " ").Trim();
            s = s.Replace("\n", " ");
            s = s.Replace("\r", " ");
            s = s.Replace("\t", " ");
            return s;
        }

        #endregion


        #region Static Utility Methods
        //------------------------------------------------------

        //-- cj 6/4/2014 added this function... need to replace "ScrubCurrencyForDb" with "ScrubNumberForDb"
        public static string StaticScrubNumberForDb(string value)
        {
            if (value == null) { value = ""; }
            string tmp = value.Trim();
            tmp = tmp.Trim();
            tmp = tmp.Replace("$", "");
            tmp = tmp.Replace(",", "");
            tmp = tmp.Replace("%", "");
            return tmp;

        }
        //------------------------------------------------------
        public static string StaticScrubCurrencyForDb(string value)
        {
            if (value == null) { value = ""; }
            string tmp = value.Trim();
            tmp = tmp.Trim();
            tmp = tmp.Replace("$", "");
            tmp = tmp.Replace(",", "");
            return tmp;

        }
        //------------------------------------------------------
        public static string StaticScrubPhoneForDb(string value)
        {
            if (value == null) { value = ""; }
            string tmp = value.Trim();
            tmp = tmp.Trim();
            tmp = tmp.Replace("(", "");
            tmp = tmp.Replace(")", "");
            tmp = tmp.Replace("-", "");
            tmp = tmp.Replace(" ", "");
            tmp = "'" + tmp + "'";
            return tmp;

        }

        //------------------------------------------------------
        public static string StaticScrubTextForDb(string value)
        {
            if (value == null) { value = ""; }
            string tmp = value.Replace("'", "''");
            tmp = "'" + tmp.Trim() + "'";
            return tmp;

        }
        //------------------------------------------------------
        //public static string StaticScrubDateForDb(jDateTime dt)
        //{
        //    if (dt == null)
        //    {
        //        return "null";
        //    }
        //    else
        //    {
        //        return "'" + dt.Value + "'";
        //    }
        //}
        //------------------------------------------------------
        public static string StaticScrubDateForDb(DateTime dt)
        {
            return "'" + dt.ToShortDateString() + "'";
        }
        //------------------------------------------------------
        public static string StaticScrubDateForDb(String dt)
        {
            return "'" + dt + "'";
        }
        //------------------------------------------------------


        public static String FormatPhone(String value)
        {
            String s = "";
            if (value != null && value != "" && value.Length == 10)
            {
                value = value.Replace("(", "");
                value = value.Replace(")", "");
                value = value.Replace(" ", "");
                value = value.Replace("-", "");

                s = String.Format("{0:(###) ###-####}", double.Parse(value));

            }
            return s;
        }

        #endregion


    }
}
