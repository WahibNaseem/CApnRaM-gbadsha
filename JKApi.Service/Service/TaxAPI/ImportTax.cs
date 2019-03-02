using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JKApi.Data.DAL;
using Core.Helpers;
using Core.Net;


using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using JKApi.Service.Helper.Extension;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using JKApi.Service.Service.TaxAPI;
using JK.Repository.Uow;
using JKApi.Service.ServiceContract.TaxAPI;

namespace JKApi.Service.Service.TaxAPI
{
    public class ImportTax : BaseService, IImportTax
    {

        string Baseurl = "https://api.zip-tax.com/request/v30";
        string APIKey = "K8E65AhXIga2fJuz";

        public ImportTax(IJKEfUow uow)
        {
            Uow = uow;
        }

        public ImportTax()
        {

        }



        public void CallAPIAndImportData(List<Address> AddresList, List<portal_spGet_AP_TaxRateAPI_Result> TaxRateList)
        {
            try
            {



                string APIURL = Baseurl + "?" + "key=" + APIKey + "&" + "city={city}" + "&" + "state={state}" + "&" + "postalcode={postalcode}";
                APIURL = APIURL.Replace("https", "http");
                DateTime taxRequestedDate = DateTime.Now;
                using (var context = new jkDatabaseEntities())
                {
                    //If there are no records in TaxRate then insert records
                    if (TaxRateList.Count == 0)
                    {
                        for (int i = 0; i < AddresList.Count; i++)
                        {
                            string _StateName = AddresList[i].StateName;
                            if (String.IsNullOrEmpty(AddresList[i].StateName))
                            {
                                int _stID = Convert.ToInt32(AddresList[i].StateListId);
                                StateList stateList = context.StateLists.SingleOrDefault(o => o.StateListId == _stID);
                                _StateName = stateList.Name;
                            }
                            //string url = APIURL.Replace("{postalcode}", AddresList[0].PostalCode.Trim());
                            string url = APIURL.Replace("{postalcode}", AddresList[i].PostalCode.Trim()).Replace("{city}", AddresList[i].City.Trim()).Replace("{state}", _StateName.Trim());
                            //url = APIURL.Replace("{city}", AddresList[i].City.Trim());
                            //url = APIURL.Replace("{state}", AddresList[i].StateName.Trim());
                            int sentaddressid = AddresList[i].AddressId;
                            JsonHelper jsonHelper = new JsonHelper();
                            HttpClientResponse clientResponse = new HttpClientHelper().HitURL(url);
                            Rootobject APIResponse = jsonHelper.JsonDeserialize<Rootobject>(clientResponse.ClientResponse);


                            JKApi.Data.DAL.TaxRate taxrate = context.TaxRates.SingleOrDefault(o => o.AddressId == sentaddressid);
                            if (taxrate != null && APIResponse.results.Count() > 0)
                            {
                                taxrate.ContractTaxRate = APIResponse.results[0].taxSales * 100;
                                taxrate.LeaseTaxRate = APIResponse.results[0].taxSales * 100;
                                taxrate.SupplyTaxRate = APIResponse.results[0].taxSales * 100;
                                taxrate.taxSales = APIResponse.results[0].taxSales * 100;
                                taxrate.taxUse = APIResponse.results[0].taxUse * 100;
                                taxrate.stateSalesTax = APIResponse.results[0].stateSalesTax * 100;
                                taxrate.stateUseTax = APIResponse.results[0].stateUseTax * 100;
                                taxrate.citySalesTax = APIResponse.results[0].citySalesTax * 100;
                                taxrate.cityUseTax = APIResponse.results[0].cityUseTax * 100;
                                taxrate.countySalesTax = APIResponse.results[0].countySalesTax * 100;
                                taxrate.countyUseTax = APIResponse.results[0].countyUseTax * 100;
                                taxrate.districtSalesTax = APIResponse.results[0].districtSalesTax * 100;
                                taxrate.districtUseTax = APIResponse.results[0].districtUseTax * 100;
                                taxrate.LastUpdatedDate = taxRequestedDate;
                                context.TaxRates.Add(taxrate);
                                context.SaveChanges();
                            }
                            else if (APIResponse.results.Count() > 0)
                            {

                                taxrate = new TaxRate();
                                taxrate.County = APIResponse.results[0].geoCounty;
                                taxrate.CountyTaxCode = APIResponse.results[0].countyTaxCode;
                                taxrate.ContractTaxRate = APIResponse.results[0].taxSales * 100;
                                taxrate.LeaseTaxRate = APIResponse.results[0].taxSales * 100;
                                taxrate.SupplyTaxRate = APIResponse.results[0].taxSales * 100;
                                taxrate.taxSales = APIResponse.results[0].taxSales * 100;
                                taxrate.taxUse = APIResponse.results[0].taxUse * 100;
                                taxrate.stateSalesTax = APIResponse.results[0].stateSalesTax * 100;
                                taxrate.stateUseTax = APIResponse.results[0].stateUseTax * 100;
                                taxrate.citySalesTax = APIResponse.results[0].citySalesTax * 100;
                                taxrate.cityUseTax = APIResponse.results[0].cityUseTax * 100;
                                taxrate.countySalesTax = APIResponse.results[0].countySalesTax * 100;
                                taxrate.countyUseTax = APIResponse.results[0].countyUseTax * 100;
                                taxrate.districtSalesTax = APIResponse.results[0].districtSalesTax * 100;
                                taxrate.districtUseTax = APIResponse.results[0].districtUseTax * 100;
                                taxrate.LastUpdatedDate = taxRequestedDate;
                                taxrate.AddressId = AddresList[i].AddressId;
                                taxrate.classId = AddresList[i].ClassId;
                                taxrate.ContractTaxRate = APIResponse.results[0].taxUse * 100; ;
                                taxrate.LeaseTaxRate = APIResponse.results[0].taxUse * 100; ;
                                taxrate.SupplyTaxRate = APIResponse.results[0].taxUse * 100; ;
                                taxrate.TypeListId = AddresList[i].TypeListId;
                                context.TaxRates.Add(taxrate);
                                context.SaveChanges();


                            }

                        }
                    }
                    else
                    {

                        int sentaddressid = 0;
                        for (int j = 0; j < TaxRateList.Count; j++)
                        {
                            string _StateName = TaxRateList[j].StateName;
                            if (String.IsNullOrEmpty(TaxRateList[j].StateName))
                            {
                                int _ASAddressId = Convert.ToInt32(TaxRateList[j].AddressId);
                                Address _Address = context.Addresses.FirstOrDefault(o => o.AddressId == _ASAddressId);
                                StateList stateList = context.StateLists.SingleOrDefault(o => o.StateListId == _Address.StateListId);
                                _StateName = stateList.Name;
                            }

                            //string urlstring = APIURL.Replace("{postalcode}", TaxRateList[0].PostalCode.Trim()).;
                            string urlstring = APIURL.Replace("{postalcode}", TaxRateList[j].PostalCode.Trim()).Replace("{city}", TaxRateList[j].City.Trim()).Replace("{state}", _StateName.Trim());

                            sentaddressid = (int)TaxRateList[j].AddressId;

                            int taxrateid = (int)TaxRateList[j].TaxRateId;
                            JsonHelper jsonH = new JsonHelper();
                            HttpClientResponse clientResponse = new HttpClientHelper().HitURL(urlstring);
                            Rootobject APIResponse = jsonH.JsonDeserialize<Rootobject>(clientResponse.ClientResponse);

                            if (APIResponse.results.Count() > 0)
                            {


                                JKApi.Data.DAL.TaxRate taxrate = context.TaxRates.SingleOrDefault(o => o.TaxRateId == taxrateid);
                                if (taxrate != null)
                                {
                                    taxrate.CountyTaxCode = APIResponse.results[0].countyTaxCode;
                                    taxrate.ContractTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.LeaseTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.SupplyTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.taxSales = APIResponse.results[0].taxSales * 100;
                                    taxrate.taxUse = APIResponse.results[0].taxUse * 100;
                                    taxrate.stateSalesTax = APIResponse.results[0].stateSalesTax * 100;
                                    taxrate.stateUseTax = APIResponse.results[0].stateUseTax * 100;
                                    taxrate.citySalesTax = APIResponse.results[0].citySalesTax * 100;
                                    taxrate.cityUseTax = APIResponse.results[0].cityUseTax * 100;
                                    taxrate.countySalesTax = APIResponse.results[0].countySalesTax * 100;
                                    taxrate.countyUseTax = APIResponse.results[0].countyUseTax * 100;
                                    taxrate.districtSalesTax = APIResponse.results[0].districtSalesTax * 100;
                                    taxrate.districtUseTax = APIResponse.results[0].districtUseTax * 100;


                                    switch (APIResponse.rCode)
                                    {
                                        case 100:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "SUCCESS";
                                            taxrate.TaxAPIExceptionDesc = "Successful API Request";
                                            break;
                                        case 101:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_KEY";
                                            taxrate.TaxAPIExceptionDesc = "Key format is not valid";
                                            break;
                                        case 102:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_STATE";
                                            taxrate.TaxAPIExceptionDesc = "State format is not valid";
                                            break;
                                        case 103:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_CITY";
                                            taxrate.TaxAPIExceptionDesc = "City format is not valid";
                                            break;
                                        case 104:
                                            taxrate.TaxAPIExceptionReason = "INVALID_POSTAL_CODE";
                                            taxrate.TaxAPIExceptionDesc = "Postal code format is not valid";
                                            break;
                                        case 105:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_FORMAT";
                                            taxrate.TaxAPIExceptionDesc = "Query string format is not valid";
                                            break;
                                        case 106:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "API_ERROR";
                                            taxrate.TaxAPIExceptionDesc = "Api error";
                                            break;
                                        case 107:
                                            taxrate.TaxAPIExceptionReason = "FEATURE_NOT_ENABLED";
                                            taxrate.TaxAPIExceptionDesc = "Requested feature or version not enabled";
                                            break;
                                        case 108:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "REQUEST_LIMIT_MET";
                                            taxrate.TaxAPIExceptionDesc = "Api request limit metd";
                                            break;
                                        case 109:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "ADDRESS_INCOMPLETE";
                                            taxrate.TaxAPIExceptionDesc = "Missing address parameters";
                                            break;
                                        default:
                                            taxrate.TaxAPIExceptionCode = 999;
                                            taxrate.TaxAPIExceptionReason = "SUCCESS";
                                            taxrate.TaxAPIExceptionDesc = "Success Call, No Data Returned";
                                            break;
                                    }
                                    taxrate.LastUpdatedDate = taxRequestedDate;

                                    if (taxrate.County == null)
                                    {
                                        taxrate.County = APIResponse.results[0].geoCounty;
                                    }

                                    context.Entry(taxrate).State = System.Data.Entity.EntityState.Modified;

                                }

                                JKApi.Data.DAL.Address address = context.Addresses.SingleOrDefault(a => a.AddressId == sentaddressid);
                                if (address != null)
                                {

                                    address.CountyTaxCode = APIResponse.results[0].countyTaxCode;

                                }
                                context.Entry(address).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();



                            }
                            else
                            {

                                JKApi.Data.DAL.TaxRate taxrate = context.TaxRates.SingleOrDefault(o => o.TaxRateId == taxrateid);
                                if (taxrate != null)
                                {
                                    switch (APIResponse.rCode)
                                    {
                                        case 100:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "SUCCESS";
                                            taxrate.TaxAPIExceptionDesc = "Successful API Request";
                                            break;
                                        case 101:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_KEY";
                                            taxrate.TaxAPIExceptionDesc = "Key format is not valid";
                                            break;
                                        case 102:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_STATE";
                                            taxrate.TaxAPIExceptionDesc = "State format is not valid";
                                            break;
                                        case 103:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_CITY";
                                            taxrate.TaxAPIExceptionDesc = "City format is not valid";
                                            break;
                                        case 104:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_POSTAL_CODE";
                                            taxrate.TaxAPIExceptionDesc = "Postal code format is not valid";
                                            break;
                                        case 105:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "INVALID_FORMAT";
                                            taxrate.TaxAPIExceptionDesc = "Query string format is not valid";
                                            break;
                                        case 106:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "API_ERROR";
                                            taxrate.TaxAPIExceptionDesc = "Api error";
                                            break;
                                        case 107:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "FEATURE_NOT_ENABLED";
                                            taxrate.TaxAPIExceptionDesc = "Requested feature or version not enabled";
                                            break;
                                        case 108:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "REQUEST_LIMIT_MET";
                                            taxrate.TaxAPIExceptionDesc = "Api request limit metd";
                                            break;
                                        case 109:
                                            taxrate.TaxAPIExceptionCode = APIResponse.rCode;
                                            taxrate.TaxAPIExceptionReason = "ADDRESS_INCOMPLETE";
                                            taxrate.TaxAPIExceptionDesc = "Missing address parameters";
                                            break;
                                        default:
                                            taxrate.TaxAPIExceptionCode = 999;
                                            taxrate.TaxAPIExceptionReason = "SUCCESS";
                                            taxrate.TaxAPIExceptionDesc = "Success Call, No Data Returned";
                                            break;
                                    }
                                    context.Entry(taxrate).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();

                                }

                            }

                        }

                        for (int i = 0; i < AddresList.Count; i++)
                        {

                            string _StateName = AddresList[i].StateName;
                            if (String.IsNullOrEmpty(AddresList[i].StateName))
                            {
                                int _stID = Convert.ToInt32(AddresList[i].StateListId);
                                StateList stateList = context.StateLists.SingleOrDefault(o => o.StateListId == _stID);
                                _StateName = stateList.Name;
                            }
                            //string url = APIURL.Replace("{postalcode}", AddresList[0].PostalCode.Trim());
                            string url = APIURL.Replace("{postalcode}", AddresList[i].PostalCode.Trim()).Replace("{city}", AddresList[i].City.Trim()).Replace("{state}", _StateName.Trim());
                            //url = APIURL.Replace("{city}", AddresList[i].City.Trim());
                            //url = APIURL.Replace("{state}", AddresList[i].StateName.Trim());
                            sentaddressid = AddresList[i].AddressId;
                            JsonHelper jsonHelper = new JsonHelper();
                            HttpClientResponse clientResponse = new HttpClientHelper().HitURL(url);
                            Rootobject APIResponse = jsonHelper.JsonDeserialize<Rootobject>(clientResponse.ClientResponse);

                            if (APIResponse.results.Count() > 0)
                            {
                                JKApi.Data.DAL.TaxRate taxrate = context.TaxRates.FirstOrDefault(o => o.AddressId == sentaddressid);
                                if (taxrate != null)
                                {

                                    taxrate.ContractTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.LeaseTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.SupplyTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.taxSales = APIResponse.results[0].taxSales * 100;
                                    taxrate.taxUse = APIResponse.results[0].taxUse * 100;
                                    taxrate.stateSalesTax = APIResponse.results[0].stateSalesTax * 100;
                                    taxrate.stateUseTax = APIResponse.results[0].stateUseTax * 100;
                                    taxrate.citySalesTax = APIResponse.results[0].citySalesTax * 100;
                                    taxrate.cityUseTax = APIResponse.results[0].cityUseTax * 100;
                                    taxrate.countySalesTax = APIResponse.results[0].countySalesTax * 100;
                                    taxrate.countyUseTax = APIResponse.results[0].countyUseTax * 100;
                                    taxrate.districtSalesTax = APIResponse.results[0].districtSalesTax * 100;
                                    taxrate.districtUseTax = APIResponse.results[0].districtUseTax * 100;
                                    taxrate.LastUpdatedDate = taxRequestedDate;
                                    context.SaveChanges();

                                }
                                else
                                {

                                    taxrate = new TaxRate();
                                    taxrate.County = APIResponse.results[0].geoCounty;
                                    taxrate.CountyTaxCode = APIResponse.results[0].countyTaxCode;
                                    taxrate.ContractTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.LeaseTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.SupplyTaxRate = APIResponse.results[0].taxSales * 100;
                                    taxrate.taxSales = APIResponse.results[0].taxSales * 100;
                                    taxrate.taxUse = APIResponse.results[0].taxUse * 100;
                                    taxrate.stateSalesTax = APIResponse.results[0].stateSalesTax * 100;
                                    taxrate.stateUseTax = APIResponse.results[0].stateUseTax * 100;
                                    taxrate.citySalesTax = APIResponse.results[0].citySalesTax * 100;
                                    taxrate.cityUseTax = APIResponse.results[0].cityUseTax * 100;
                                    taxrate.countySalesTax = APIResponse.results[0].countySalesTax * 100;
                                    taxrate.countyUseTax = APIResponse.results[0].countyUseTax * 100;
                                    taxrate.districtSalesTax = APIResponse.results[0].districtSalesTax * 100;
                                    taxrate.districtUseTax = APIResponse.results[0].districtUseTax * 100;
                                    taxrate.LastUpdatedDate = taxRequestedDate;
                                    taxrate.AddressId = AddresList[i].AddressId;
                                    taxrate.classId = AddresList[i].ClassId;
                                    taxrate.ContractTaxRate = APIResponse.results[0].taxUse * 100; ;
                                    taxrate.LeaseTaxRate = APIResponse.results[0].taxUse * 100; ;
                                    taxrate.SupplyTaxRate = APIResponse.results[0].taxUse * 100; ;
                                    taxrate.TypeListId = AddresList[i].TypeListId;
                                    context.TaxRates.Add(taxrate);
                                    context.SaveChanges();


                                }
                            }


                        }


                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

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


}
