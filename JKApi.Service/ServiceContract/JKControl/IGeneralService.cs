using JKApi.Data.DAL;
using JKViewModels.Administration.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.ServiceContract.JKControl
{
    public interface IGeneralService
    {
      //  IEnumerable<spget_CPIList_Result> GetCPIList() ;
        IEnumerable<CPI> GetCPIList();        
        CPIViewModel SearchCPIById(int id);
        void DeleteCPIById(int id);
        string SaveCPI(Nullable<int> id, Nullable<int> billmonth, Nullable<int> billyear, Nullable<decimal> percent, string description, Nullable<int> applied, Nullable<int> userid);
        //string SaveCPI(CPIViewModel model);
        string UpdateCPI(Nullable<int> id, Nullable<int> billmonth, Nullable<int> billyear, Nullable<decimal> percent, string description, Nullable<int> applied, Nullable<int> userid);
       string EditCPI(CPIViewModel model);
    }
}
