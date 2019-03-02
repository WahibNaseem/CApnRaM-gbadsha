using JKApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using JKViewModels.Customer;
using Dapper;
using JKViewModels;
using JKViewModels.Company;

using JK.Repository.Uow;
using JKApi.Data.DAL;
//using JKApi.Data.JkControl;
using JKApi.Service.Service;
using JKApi.Service.ServiceContract.AccountReceivable;
using JKViewModels.AccountReceivable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JKApi.Service.Helper.Extension;
using JK.FMS.MVC.Areas.Portal.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using JKViewModels.Customer;
using Dapper;
using JKViewModels;
using JKViewModels.Company;



namespace JKApi.Service
{
    public class CPeriodClosed : ServiceContract.CPeriodClosed
    {

        int _PeriodClosedId;
        public int PeriodClosedId
        {
            get
            {
                return _PeriodClosedId;
            }

            set
            {
                _PeriodClosedId = value;
            }
        }


        int _PeriodId;
        public int PeriodId
        {
            get
            {
                return _PeriodId;
            }

            set
            {
                _PeriodId = value;
            }
        }


        int _RegionId;
        public int RegionId
        {
            get
            {
                return _RegionId;
            }

            set
            {
                _RegionId = value;
            }
        }

        int _TransactionStatusListId;
        public int TransactionStatusListId
        {
            get
            {
                return _TransactionStatusListId;
            }

            set
            {
                _TransactionStatusListId = value;
            }
        }

        int _ChargebackFinalized;
        public int ChargebackFinalized
        {
            get
            {
                return _ChargebackFinalized;
            }

            set
            {
                _ChargebackFinalized = value;
            }
        }

        int _FranchiseeReport;
        public int FranchiseeReport
        {
            get
            {
                return _FranchiseeReport;
            }

            set
            {
                _FranchiseeReport = value;
            }
        }

        int _MonthlyBillRun;
        public int MonthlyBillRun
        {
            get
            {
                return _MonthlyBillRun;
            }

            set
            {
                _MonthlyBillRun = value;
            }
        }

        int _Closed;
        public int Closed
        {
            get
            {
                return _Closed;
            }

            set
            {
                _Closed = value;
            }
        }

        int _NegativeDueFinalized;
        public int NegativeDueFinalized
        {
            get
            {
                return _NegativeDueFinalized;
            }

            set
            {
                _NegativeDueFinalized = value;
            }
        }

        int _BillMonth;
        public int BillMonth
        {
            get
            {
                return _BillMonth;
            }

            set
            {
                _BillMonth = value;
            }
        }

        int _BillYear;
        public int BillYear
        {
            get
            {
                return _BillYear;
            }

            set
            {
                _BillYear = value;
            }
        }


        public static bool Validate_EarliestPeriod(int RegionId, int SelectedPeriodId)
        {
            bool isAvalableToClose = false;

            string sqlStr = "SELECT top 1 PC.PeriodClosedId FROM PeriodClosed PC INNER JOIN [Period] P on P.PeriodId = PC.PeriodId " +
                            "WHERE RegionId = " + RegionId.ToString() +
                            " AND closed = 0";

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var count = conn.Query(sqlStr).FirstOrDefault();
                int earliestPeriodAvailableToClose = count == null ? 0 : Convert.ToInt32(count.PeriodClosedId);

                isAvalableToClose = earliestPeriodAvailableToClose == SelectedPeriodId ? true : false;
                return isAvalableToClose;
            }
        }


        public static CPeriodClosed GetPeriodToClose(int RegionId, int SelectedPeriodId)
        {

            string sqlStr = "SELECT top 1 PC.PeriodClosedId, PC.PeriodId, PC.RegionId,PC.TransactionStatusListId, PC.ChargebackFinalized,PC.FranchiseeReport, PC.MonthlyBillRun, " +
                             " PC.Closed, P.BillMonth,  P.BillYear FROM PeriodClosed PC INNER JOIN [Period] P on P.PeriodId = PC.PeriodId WHERE RegionId = " + RegionId.ToString() +
                             " AND PC.PeriodClosedId = " + SelectedPeriodId.ToString();
                           

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
               
                CPeriodClosed PeriodClosedRow = conn.Query<CPeriodClosed>(sqlStr).FirstOrDefault();

               
                return PeriodClosedRow;
            }
        }

        public static bool UpdateStatusClosedPeriod(int PeriodClosedId, int LoginUserId)
        {
            bool _retVal = false;
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@PeriodClosedId", PeriodClosedId);
                parmas.Add("@CreatedBy", LoginUserId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_COM_UpdateStatusClosePeriod", parmas, commandType: CommandType.StoredProcedure))
                {
                    _retVal = true;
                }
                return _retVal;
            }

        }



        public List<CPeriodClosed> GetClosedPeriodForCloseList(int regionId)
        {
            List<CPeriodClosed> lstPeriod = new List<CPeriodClosed>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_COM_ValidatedClosePeriod", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstPeriod =  multipleresult.Read<CPeriodClosed>().ToList();
                    }
                }
                return lstPeriod;
            }

        }










    }
}
