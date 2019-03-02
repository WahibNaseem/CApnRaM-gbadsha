using BLL;
using Core.Helpers;
using Core.Logger;
using Core.Net;
using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxImportLogic
{
    public class LogicExecuter
    {
        public void CallAPIAndImportData(string APIURL, string APIKey)
        {
            APIURL = APIURL + "?" + "key=" + APIKey + "&" + "postalcode={postalcode}";

            ImportTaxManager importTaxManager = new ImportTaxManager();

            List<Address> TaxAddressList = importTaxManager.GetAllAddress();


           //foreach (Address TaxAddress in TaxAddressList)
           Parallel.ForEach(TaxAddressList, new ParallelOptions { MaxDegreeOfParallelism = -1 }, TaxAddress =>
            {
                try
                {
                    if (TaxAddress.PostalCode != null)
                    {
                        string url = APIURL.Replace("{postalcode}", TaxAddress.PostalCode.Trim());
                         

                        JsonHelper jsonHelper = new JsonHelper();

                        HttpClientResponse clientResponse = new HttpClientHelper().HitURL(url);
                        Rootobject APIResponse = jsonHelper.JsonDeserialize<Rootobject>(clientResponse.ClientResponse);


                        if (APIResponse.rCode!=100)
                        {
                           // LoggerFactory.CreateLogger().Log("REquest to API Fail=>"+ url, LogType.Info);
                            return;
                        }

                        ImportTaxManager taxManager = new ImportTaxManager();

                        TaxRate taxrate = taxManager.GetByAddressId(TaxAddress.AddressId);

                        if (taxrate == null)
                        {
                            taxrate = new TaxRate();
                        }

                        if (APIResponse.results.Length>0)
                        {
                            taxrate.citySalesTax = APIResponse.results[0].citySalesTax * 100;
                            taxrate.cityUseTax = APIResponse.results[0].cityUseTax * 100;
                            taxrate.countySalesTax = APIResponse.results[0].countySalesTax * 100;
                            taxrate.countyUseTax = APIResponse.results[0].countyUseTax * 100;
                            taxrate.districtSalesTax = APIResponse.results[0].districtSalesTax * 100;
                            taxrate.districtUseTax = APIResponse.results[0].districtUseTax * 100;
                            taxrate.stateSalesTax = APIResponse.results[0].stateSalesTax * 100;
                            taxrate.stateUseTax = APIResponse.results[0].stateUseTax * 100;
                            taxrate.taxSales = APIResponse.results[0].taxSales * 100;
                            taxrate.taxUse = APIResponse.results[0].taxUse * 100;

                            if (taxrate.AddressId > 0)
                                taxManager.UpdateTaxRate(taxrate);
                            else
                                taxManager.Add(taxrate);
                        }

                      //  LoggerFactory.CreateLogger().Log("Working on Postalcode=>" + TaxAddress.PostalCode+"   Finished Successfully", LogType.Info);
                    }
                }
                catch (Exception ex)
                {
                    LoggerFactory.CreateLogger().Log(ex.ToString(), LogType.Info);
                }


            });
        }
    }



    public class Rootobject
    {
        public string version { get; set; }
        public int rCode { get; set; }
        public Result[] results { get; set; }
    }

    public class Result
    {
        public string geoPostalCode { get; set; }
        public string geoCity { get; set; }
        public string geoCounty { get; set; }
        public string geoState { get; set; }
        public decimal taxSales { get; set; }
        public decimal taxUse { get; set; }
        public string txbService { get; set; }
        public string txbFreight { get; set; }
        public decimal stateSalesTax { get; set; }
        public decimal stateUseTax { get; set; }
        public decimal citySalesTax { get; set; }
        public decimal cityUseTax { get; set; }
        public string cityTaxCode { get; set; }
        public decimal countySalesTax { get; set; }
        public decimal countyUseTax { get; set; }
        public string countyTaxCode { get; set; }
        public decimal districtSalesTax { get; set; }
        public decimal districtUseTax { get; set; }
    }

}
