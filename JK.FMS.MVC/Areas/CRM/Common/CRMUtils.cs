using JK.Resources;
using JKViewModels.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using static JK.FMS.MVC.Areas.CRM.Common.CRMEnums;

namespace JK.FMS.MVC.Areas.CRM.Common
{
    public static class CRMUtils
    {
        public static string GetProviderType(AccountProviderType type)
        {
            var name = Enum.GetName(typeof(AccountProviderType), type) ?? string.Empty;
            return JKCRMResource.ResourceManager.GetString(name) ?? "N/A";
        }

        public static int GetProvidertypeIndex(string name)
        {
            int nameIndex = (int)Enum.Parse(typeof(AccountProviderType), name);
            return nameIndex;
        }

        public static List<string> GetStateDescription<T>()
        {
            Type enumType = typeof(T);
            var descriptionList = new List<string>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var field = e.GetType().GetField(e.ToString());
                var attribute = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                descriptionList.Add(attribute[0].Description);
            }
            return descriptionList;
        }

        public static string GetStateDescriptionByValue<T>(string value)
        {
            Type enumType = typeof(T);

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                if (e.ToString() == value)
                {
                    var field = e.GetType().GetField(e.ToString());
                    var attribute = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    return attribute[0].Description;
                }
            }
            return "";
        }
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            // throw new ArgumentException("Not found.", "description");
            return default(T);

        }

        public static List<KeyValuePair<string, int>> GetEnumValuesAndDescriptions<T>()
        {
            Type enumType = typeof(T);

            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T is not System.Enum");

            List<KeyValuePair<string, int>> enumValList = new List<KeyValuePair<string, int>>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var fi = e.GetType().GetField(e.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                enumValList.Add(new KeyValuePair<string, int>((attributes.Length > 0) ? attributes[0].Description : e.ToString(), (int)e));
            }

            return enumValList;
        }


        public static int GetBestTimeIndex(string name)
        {
            int nameIndex = (int)Enum.Parse(typeof(CallTime), name);
            return nameIndex;
        }

        public static string GetBestTimeName(bool value)
        {
            int index = Convert.ToInt32(value);
            CallTime name = (CallTime)index;

            return JKCRMResource.ResourceManager.GetString(name.ToString()) ?? "N/A";
        }

        public static List<string> GetBestTimeList()
        {
            var valueList = new List<string>();
            var lists = Enum.GetValues(typeof(CallTime)).Cast<CallTime>();
            foreach (var list in lists)
            {
                valueList.Add(JKCRMResource.ResourceManager.GetString(list.ToString()) ?? "N/A");
            }
            return valueList;
        }

        #region CRMUtils > Format

        public static string FormatUsPhoneNumber(string value)
        {
            if (value != "" && value != null)
            {
                value = new System.Text.RegularExpressions.Regex(@"\D").Replace(value, string.Empty);
                value = value.TrimStart('1');
                if (value.Length == 7)
                    return Convert.ToInt64(value).ToString("###-####");
                if (value.Length == 10)
                    return Convert.ToInt64(value).ToString("(###) ###-####");
                if (value.Length > 10)
                    return Convert.ToInt64(value)
                        .ToString("(###) ###-#### " + new string('#', (value.Length - 10)));
            }
            return value;
        }

        public static string FormatUsCurrency(decimal? value)
        {
            if (value != 0)
                return string.Format("{0:C}", value);
            return "";
        }

        #endregion

        public static TimeSpan ToTimeSpan(this string timeString)
        {
            var dt = DateTime.ParseExact(timeString, "h:mm:ss tt", System.Globalization.CultureInfo.CurrentUICulture);
            return dt.TimeOfDay;
        }

        public static string SchedulePurpose(this int? purpose)
        {
            switch (purpose)
            {
                case 1:
                    return "Past Proposal";
                case 2:
                    return "Cold Call";
                case 3:
                    return "Prospecting Email";
                case 4:
                    return "First Visit";
                case 5:
                    return "Qualifying Lead";
                case 6:
                    return "Proposal Delivery";
                case 7:
                    return "Proposal Follow up";
                case 8:
                    return "Call Back";
                case 9:
                    return "Follow Up";
                case 10:
                    return "Walk Thru";
                case 11:
                    return "Meeting";
                case 12:
                    return "Other";
                default:
                    return "";
            }
        }

        public static int SchedulePurposIndex(this string purpose)
        {
            if (purpose == "Past Proposal")
                return 1;
            else if (purpose == "Cold Call")
                return 2;
            else if (purpose == "Prospecting Email")
                return 3;
            if (purpose == "First Visit")
                return 4;
            else if (purpose == "Qualifying Lead")
                return 5;
            else if (purpose == "Proposal Delivery")
                return 6;
            if (purpose == "Proposal Follow up")
                return 7;
            else if (purpose == "Call Back")
                return 8;
            else if (purpose == "Follow-up")
                return 9;
            if (purpose == "Walk Thru")
                return 10;
            else if (purpose == "Meeting")
                return 11;
            else if (purpose == "Other")
                return 12;
            else
                return 0;

        }

        public static int JobType(string type)
        {
            if (type == "fullTime")
                return 1;
            else
                return 2;
        }
        public static string JobTypeValue(int type)
        {
            if (type == 1)
                return "fullTime";
            else
                return "partTime";
        }


        //Render PartialView To String
        public static string RenderPartialViewToString(ControllerContext controllercontext, string viewname, object model)
        {
            controllercontext.Controller.ViewData.Model = model;
            using(var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllercontext, viewname);
                var ViewContext = new ViewContext(controllercontext, viewResult.View,controllercontext.Controller.ViewData, controllercontext.Controller.TempData, sw);
                viewResult.View.Render(ViewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllercontext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
            
        }
    }

}