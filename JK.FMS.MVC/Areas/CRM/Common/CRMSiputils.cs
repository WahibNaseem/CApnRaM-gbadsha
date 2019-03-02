using System;
//using SIPVoipSDK;
using System.Configuration;
using JKApi.Core;
using Application.Web.Core;
using System.Threading;
using System.Threading.Tasks;

namespace JK.FMS.MVC.Areas.CRM.Common
{
    public class CRMSipUtils : ViewControllerBase
    {
        private static CRMSipUtils instance;
        // private CAbtoPhone _abToPhone;
        private CRMSipUtils()
        {
            //try
            //{
            //    _abToPhone = new CAbtoPhoneClass();
            //    var phoneCfg = _abToPhone.Config;
            //    phoneCfg.LicenseUserId = ConfigurationManager.AppSettings["VOIPLicenseUserId"];
            //    phoneCfg.LicenseKey = ConfigurationManager.AppSettings["VOIPLicenseKey"];
            //    phoneCfg.RegDomain = ConfigurationManager.AppSettings["RegDomain"];
            //    phoneCfg.RegUser = ConfigurationManager.AppSettings["RegUser"];
            //    phoneCfg.RegPass = ConfigurationManager.AppSettings["RegPassword"];
            //    phoneCfg.RegExpire = Convert.ToInt32(ConfigurationManager.AppSettings["RegExpire"]);

            //    _abToPhone.ApplyConfig();
            //    _abToPhone.Initialize();

            //}
            //catch (Exception ex)
            //{
            //    NLogger.Error("SIP Phone Class Exception : " + ex.ToString());
            //}
        }
        public static CRMSipUtils GetCRMInstance()
        {
            if (instance == null)
            {
                instance = new CRMSipUtils();
            }
            return instance;
        }
        public string Call(bool isCall, string phoneNumber)
        {
            //if (isCall)
            //{
            //    try
            //    {
            //        if (_abToPhone == null)
            //        {
            //            try
            //            {
            //                _abToPhone = new CAbtoPhoneClass();
            //                var phoneCfg = _abToPhone.Config;
            //                phoneCfg.LicenseUserId = ConfigurationManager.AppSettings["VOIPLicenseUserId"];
            //                phoneCfg.LicenseKey = ConfigurationManager.AppSettings["VOIPLicenseKey"];
            //                phoneCfg.RegDomain = ConfigurationManager.AppSettings["RegDomain"];
            //                phoneCfg.RegUser = ConfigurationManager.AppSettings["RegUser"];
            //                phoneCfg.RegPass = ConfigurationManager.AppSettings["RegPassword"];
            //                phoneCfg.RegExpire = Convert.ToInt32(ConfigurationManager.AppSettings["RegExpire"]);
            //                _abToPhone.ApplyConfig();
            //                _abToPhone.Initialize();

            //            }
            //            catch (Exception ex)
            //            {
            //                NLogger.Error("SIP Phone Config Class Exception : " + ex.ToString());
            //                return ex.ToString();
            //            }
            //        }
            //        if (_abToPhone != null)
            //        {
            //            Thread.Sleep(3000);
            //            _abToPhone.StartCall2(phoneNumber);


            //            return "Connected";
            //        }
            //        else { return "Instance not created"; }
            //    }
            //    catch (Exception ex)
            //    {

            //        NLogger.Error("Start Call Issue : " + ex.Message);
            //        return ex.ToString();
            //    }
            //}
            //else
            //{
            //    _abToPhone.HangUpLastCall();
            //    return ("Disconnect");
            //}

            return string.Empty;
        }
    }
}