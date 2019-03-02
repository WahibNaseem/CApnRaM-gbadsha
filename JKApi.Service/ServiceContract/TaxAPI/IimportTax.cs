using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Data.DAL;
using System.Collections.Generic;

namespace JKApi.Service.ServiceContract.TaxAPI
{
    public interface IImportTax
    {

        void CallAPIAndImportData(List<Address> AddresList, List<portal_spGet_AP_TaxRateAPI_Result> TaxRate);
    }
}
