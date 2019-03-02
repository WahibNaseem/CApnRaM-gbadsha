using AuthorizeNet.Api.Contracts.V1;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for AuthorizenetCommon
/// </summary>
public static class AuthorizenetCommon
{
    /// <summary>
    /// IstransactionResponsePropertyExist
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public static bool IstransactionResponsePropertyExist(dynamic response)
    {
        bool retval = false;
        try
        {
            dynamic temp = response.transactionResponse;
            retval = true;
        }
        catch (RuntimeBinderException)
        {

        }
        return retval;
    }
   
    /// <summary>
    /// CheckAuthorizeNetApiResponse
    /// </summary>
    /// <param name="apiResponse"></param>
    /// <returns></returns>
    public static bool CheckAuthorizeNetApiResponse(dynamic apiResponse)
    {
        bool result = false;
        if (apiResponse != null && apiResponse.messages.resultCode == messageTypeEnum.Ok)
        {
            if (apiResponse != null && apiResponse.messages.message != null)
            {
                result = true;
            }
        }
        return result;
    }
    /// <summary>
    /// GetErrorFromResponse
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    /// <summary>
    /// SetCurrencySymbol
    /// </summary>
    /// <param name="price"></param>
    /// <returns></returns>
    public static string SetCurrencySymbol(decimal price)
    {
        return String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C2}", price);
    }
    /// <summary>
    /// GetIp
    /// </summary>
    /// <returns></returns>
    public static string GetIp()
    {
        string strHostName = "";

        try
        {
            strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            string ipaddress = Convert.ToString(ipEntry.AddressList[1]);
            return ipaddress;
        }
        catch
        {
            return strHostName;
        }
    }
}