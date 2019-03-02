using JK.Repository.Uow;
using JKApi.Data.DAL;
//using JKApi.Data.JkControl;
using JKApi.Service.Service;
using JKApi.Service.ServiceContract.AccountPayable;
using JKViewModels.AccountsPayable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JKApi.Service.Helper.Extension;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using JKViewModels.Franchise;
using Dapper;
using AutoMapper;

namespace JKApi.Service.AccountPayable
{
    public class AccountPayableService : BaseService, IAccountPayableService
    {

        const int FRANCHISEE_DUE = 1;
        const int ACCOUNTING_FEE_REBATE = 2;
        const int TURN_AROUND = 3;
        const string CLOSE = "CLOSE";
        const string OPEN = "OPEN";
        const string PENDING = "PENDING";

        #region ConstructorCalls

        public AccountPayableService(IJKEfUow uow)
        {
            Uow = uow;
        }

        public AccountPayableService()
        {
        }

        #endregion

        public List<SearchDateList> GetAll_SearchDateList()
        {
            List<SearchDateList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.SearchDateLists.ToList();
                //    data = context.SearchDateLists. Select(x => new SearchDateList {Name= x.Name, SearchDateListId= x.SearchDateListId });
                return data;
            }
        }

        public List<CheckBookTransactionTypeList> GetAll_CheckTypeList()
        {
            List<CheckBookTransactionTypeList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.CheckBookTransactionTypeLists.ToList();
                return data;
            }
        }

        public AccountingFeeRebateFullViewModel GetAccountingFeeRebate(string RegionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                AccountingFeeRebateDetailsViewModel vafd = new AccountingFeeRebateDetailsViewModel();
                AccountingFeeRebateFullViewModel vm = new AccountingFeeRebateFullViewModel();
                vafd.AccountingFeeRebateList = context.portal_spCreate_AP_AccountingFeeRebateProcess(RegionId).ToList();
                vm.AccountingFeeRebateList = GetAccountingFeeRebateList(vafd.AccountingFeeRebateList);
                return vm;
            }

        }

        public List<JKViewModels.AccountsPayable.AccountingFeeRebateFullViewModel> GetAccountingFeeRebateList(List<portal_spCreate_AP_AccountingFeeRebateProcess_Result> AccountingFeeRebateList)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<portal_spCreate_AP_AccountingFeeRebateProcess_Result, AccountingFeeRebateFullViewModel>();
            });
            var mapper = config.CreateMapper();
            var accountViewModels = mapper.Map<IEnumerable<portal_spCreate_AP_AccountingFeeRebateProcess_Result>, IEnumerable<AccountingFeeRebateFullViewModel>>(AccountingFeeRebateList) as List<AccountingFeeRebateFullViewModel>;
            return accountViewModels;
        }


        #region Chargeback

        public List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> GetInvoiceFranchiseeListForChargeback(int periodid, string regionIds = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Database.CommandTimeout = 180;
                List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> resultList = context.portal_spGet_AP_InvoiceFranchiseeListForChargeback(regionIds ?? SelectedRegionId.ToString(), periodid).ToList();

                return resultList;
            }
        }

        public List<portal_spGet_AP_TurnAroundSummaryList_Result> GetTurnAroundSummaryList( DateTime PaymentDate, string regionIds)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Database.CommandTimeout = 180;
                List<portal_spGet_AP_TurnAroundSummaryList_Result> resultList = context.portal_spGet_AP_TurnAroundSummaryList(regionIds ?? SelectedRegionId.ToString(), PaymentDate).ToList();

                return resultList;
            }
        }

        public List<portal_spGet_AP_TurnAroundListSummary_Result> GetTurnAroundListSummary(string regionIds, DateTime PaymentDateFrom, DateTime PaymentDateTo)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Database.CommandTimeout = 180;
                List<portal_spGet_AP_TurnAroundListSummary_Result> resultList = context.portal_spGet_AP_TurnAroundListSummary(regionIds ?? SelectedRegionId.ToString(), PaymentDateFrom, PaymentDateTo).ToList();

                return resultList;
            }
        }


        public List<portal_spGet_AP_TurnAroundDetailsList_Result> GetTurnAroundDetailsList(DateTime paymentdate, string regionIds = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Database.CommandTimeout = 180;
                List<portal_spGet_AP_TurnAroundDetailsList_Result> resultList = context.portal_spGet_AP_TurnAroundDetailsList(regionIds ?? SelectedRegionId.ToString(), paymentdate, -1).ToList();

                return resultList;
            }
        }

        public List<portal_spGet_AP_InvoiceFranchiseeSummaryForChargeback_Result> GetInvoiceFranchiseeSummaryForChargeback(int periodid, string regionIds = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //context.Database.CommandTimeout = 180;
                List<portal_spGet_AP_InvoiceFranchiseeSummaryForChargeback_Result> resultList = context.portal_spGet_AP_InvoiceFranchiseeSummaryForChargeback(regionIds ?? SelectedRegionId.ToString(), periodid).ToList();

                return resultList;
            }
        }


        public string InsertFranchiseeAccountingFeeRebate(List<AccountingFeeRebateFullViewModel> inputdata, int AccFeeRebatePeriodId)
        {
            AccountingFeeRebateFullViewModel vm = new AccountingFeeRebateFullViewModel();
            vm.CreatedBy = this.LoginUserId;
            vm.CreatedDate = DateTime.Now;
            vm.AccountingFeeRebateListString = new List<string>();

            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel accountingFeeRebateTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel apbillTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            int masterTrxId = -1;
           
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                var period = context.Periods.Where(p => p.PeriodId == AccFeeRebatePeriodId).FirstOrDefault();

                foreach (var AFeeRebateItem in inputdata)
                {

                    accountingFeeRebateTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(26, (int)AFeeRebateItem.RegionId);
                    DateTime transactionDate;
                    transactionDate = DateTime.Now;

                    /***********************************************************************************************/
                    /*Save transaction*/
                    /***********************************************************************************************/

                    /*SAVE MASTERTRX*/

                       
                        MasterTrx franchiseeMasterTrx = new MasterTrx();
                        franchiseeMasterTrx.MasterTrxTypeListId = 26; // Accounting Fee rebate
                        franchiseeMasterTrx.TypeListId = 2; // franchisee
                        franchiseeMasterTrx.ClassId = AFeeRebateItem.FranchiseeId;
                        franchiseeMasterTrx.TrxDate = transactionDate;
                        franchiseeMasterTrx.PeriodId = AccFeeRebatePeriodId;
                        franchiseeMasterTrx.BillMonth = period.BillMonth; // DateTime.Now.Month;
                        franchiseeMasterTrx.BillYear = period.BillYear; // DateTime.Now.Year;
                        franchiseeMasterTrx.StatusId = 4; // open                               
                        franchiseeMasterTrx.RegionId = AFeeRebateItem.RegionId;
                        franchiseeMasterTrx.CreatedBy = vm.CreatedBy;
                        franchiseeMasterTrx.CreatedDate = vm.CreatedDate;

                        context.MasterTrxes.Add(franchiseeMasterTrx);
                        context.SaveChanges();

                        masterTrxId = franchiseeMasterTrx.MasterTrxId;
          
                        APBill apbill = new APBill();
                        MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();

                  
                        /***********************************************************************************************/
                        /*Save ApBill transaction*/
                        /***********************************************************************************************/

                        apbillTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(26, (int) AFeeRebateItem.RegionId);
                        string nextAPBillTrxNumber = CompanySvc.GetNextTransactionNumberConfig(26, (int)AFeeRebateItem.RegionId, vm.CreatedDate);

                        apbill.MasterTrxId = masterTrxId;
                        apbill.TypeListId = 2;
                        apbill.ClassId = AFeeRebateItem.FranchiseeId;
                        apbill.TransactionStatusListId = 4; // open
                        apbill.TransactionNumber = nextAPBillTrxNumber;
                        apbill.RegionId = AFeeRebateItem.RegionId;
                        apbill.CheckTypeListId = 2;
                       
                        apbill.TransactionDate = transactionDate;
                        apbill.IsDelete = false;
                        apbill.CheckAmount = AFeeRebateItem.Balance;
                        apbill.PeriodId = AccFeeRebatePeriodId;
                        apbill.BillMonth = period.BillMonth; // DateTime.Now.Month;
                        apbill.BillYear = period.BillYear; // DateTime.Now.Year;
                        apbill.PayTypeListId = 1; //Check
                        apbill.CreatedBy = vm.CreatedBy;
                        apbill.CreatedDate = vm.CreatedDate;
                        apbill.CheckBookTransactionTypeListId = 2;
                        apbill.SourceTypeListId = 16;
                        apbill.SourceId = AFeeRebateItem.AccountingFeeRebateId;





                        context.APBills.Add(apbill);
                        context.SaveChanges();

                        apbillTransactionNumberConfigViewModel.LastNumber = apbillTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(apbillTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();  
                }
            }

            string result = String.Join(",", vm.AccountingFeeRebateListString);
            return result;
        }

        public string InsertFranchiseeTurnAroundTransaction(List<portal_spGet_AP_TurnAroundDetailsList_Result> inputdata, int TARPeriodId, DateTime TransactionDate)
        {
            TurnAroundTransactionViewModel vm = new TurnAroundTransactionViewModel();
            vm.CreatedBy = this.LoginUserId;
            vm.CreatedDate = DateTime.Now;
            vm.FranchiseeTurnAroundTrxList = new List<string>();

            int FranId = -1;
            int masterTrxId = -1;
            decimal totalFranchiseeTurnAroundAmount = 0.00m;


            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel turnaroundTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel apbillTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                foreach (var TARItem in inputdata)
                {

                    turnaroundTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(56, (int)TARItem.RegionId);
                    //DateTime transactionDate;
                    //transactionDate = DateTime.Now;

                    //var periodClosed = context.PeriodCloseds.Where(p => p.PeriodClosedId == TARPeriodId).FirstOrDefault();
                    var period = context.Periods.Where(p => p.PeriodId == TARPeriodId).FirstOrDefault();

                    /***********************************************************************************************/
                    /*Save TurnAround transaction*/
                    /***********************************************************************************************/

                    /*SAVE MASTERTRX*/


                    if (TARItem.FranchiseeId != FranId)
                    {

                        totalFranchiseeTurnAroundAmount = 0.00m;
                        MasterTrx franchiseeMasterTrx = new MasterTrx();
                        franchiseeMasterTrx.MasterTrxTypeListId = 56; // TurnAround check
                        franchiseeMasterTrx.TypeListId = 2; // franchisee
                        franchiseeMasterTrx.ClassId = TARItem.FranchiseeId;
                        franchiseeMasterTrx.TrxDate = TransactionDate;
                        franchiseeMasterTrx.PeriodId = period.PeriodId;
                        franchiseeMasterTrx.BillMonth = period.BillMonth; // DateTime.Now.Month;
                        franchiseeMasterTrx.BillYear = period.BillYear; // DateTime.Now.Year;

                        franchiseeMasterTrx.StatusId = 4; // open
                                                          //franchiseeMasterTrx.BillMonth = TARItem.BillMonth;
                                                          //franchiseeMasterTrx.BillYear = TARItem.BillYear;
                        franchiseeMasterTrx.RegionId = TARItem.RegionId;
                        franchiseeMasterTrx.CreatedBy = vm.CreatedBy;
                        franchiseeMasterTrx.CreatedDate = vm.CreatedDate;

                        context.MasterTrxes.Add(franchiseeMasterTrx);
                        context.SaveChanges();

                        masterTrxId = franchiseeMasterTrx.MasterTrxId;

                        foreach (var TurnA in inputdata)
                        {
                            if (TurnA.FranchiseeId == TARItem.FranchiseeId)
                            {
                                totalFranchiseeTurnAroundAmount = totalFranchiseeTurnAroundAmount + (decimal)TurnA.TurnAroundCheckAmount;

                            }
                        }


                    }

                    string nextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(56, (int)TARItem.RegionId, vm.CreatedDate);

                    TurnAround turnaroundTrx = new TurnAround();
                    turnaroundTrx.MasterTrxId = masterTrxId;
                    turnaroundTrx.TransactionStatusListId = 4; // open
                    turnaroundTrx.TransactionNumber = nextTrxNumber;
                    turnaroundTrx.TransactionDate = TransactionDate;
                    turnaroundTrx.RegionId = TARItem.RegionId;
                    turnaroundTrx.PaymentBillingFranchiseeId = TARItem.PaymentBillingFranchiseeId;
                    turnaroundTrx.FranchiseeId = TARItem.FranchiseeId;
                    turnaroundTrx.CustomerId = TARItem.CustomerId;
                    turnaroundTrx.InvoiceId = TARItem.InvoiceId;
                    turnaroundTrx.NegativeDueAmount = TARItem.NegativeDueAmount;
                    turnaroundTrx.NegativeDueId = TARItem.NegativedueId;
                    turnaroundTrx.TurnAroundAmount = TARItem.TurnAroundCheckAmount;
                    turnaroundTrx.PaymentAmount = TARItem.PaymentAmount;
                    turnaroundTrx.CustomerNo = TARItem.CustomerNo;
                    turnaroundTrx.CustomerName = TARItem.CustomerName;
                    turnaroundTrx.InvoiceNo = TARItem.InvoiceNo;
                    turnaroundTrx.PeriodId = period.PeriodId;
                    turnaroundTrx.CreatedBy = vm.CreatedBy;
                    turnaroundTrx.CreatedDate = vm.CreatedDate;
                    turnaroundTrx.ChargebackId = TARItem.ChargebackId;
                    turnaroundTrx.ChargebackAmount = TARItem.ChargebackAmount;
                    turnaroundTrx.BillingPayId = TARItem.BillingPayId;
                    turnaroundTrx.MasterTrxDetailId = TARItem.MasterTrxDetailId;
                    turnaroundTrx.FeeTotal = TARItem.FeeAmount;

                    context.TurnArounds.Add(turnaroundTrx);
                    context.SaveChanges();

                    turnaroundTransactionNumberConfigViewModel.LastNumber = turnaroundTransactionNumberConfigViewModel.LastNumber + 1;
                    CompanySvc.SaveTransactionNumberConfig(turnaroundTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();



                    APBill apbill = new APBill();
                    MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();

                    if (TARItem.FranchiseeId != FranId)
                    {
                        /***********************************************************************************************/
                        /*Save ApBill transaction*/
                        /***********************************************************************************************/

                        apbillTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(56, (int)TARItem.RegionId);
                        string nextAPBillTrxNumber = CompanySvc.GetNextTransactionNumberConfig(56, (int)TARItem.RegionId, vm.CreatedDate);


                        apbill.MasterTrxId = masterTrxId;
                        apbill.TypeListId = 2;
                        apbill.ClassId = TARItem.FranchiseeId;
                        apbill.TransactionStatusListId = 4; // open
                        apbill.TransactionNumber = nextAPBillTrxNumber;
                        apbill.RegionId = TARItem.RegionId;
                        apbill.CheckTypeListId = 3;
                        //apbill.FranchiseeTurnaroundCheckId = turnaroundTrx.TurnAroundId;
                        apbill.TransactionDate = TransactionDate;
                        apbill.IsDelete = false;
                        apbill.CheckAmount = totalFranchiseeTurnAroundAmount;
                        apbill.PeriodId = period.PeriodId;
                        apbill.BillMonth = period.BillMonth; // DateTime.Now.Month;
                        apbill.BillYear = period.BillYear; // DateTime.Now.Year;
                        apbill.PayTypeListId = 1; //Check
                        apbill.CreatedBy = vm.CreatedBy;
                        apbill.CreatedDate = vm.CreatedDate;
                        apbill.CheckBookTransactionTypeListId = 3;
                        apbill.SourceTypeListId = 9;
                        context.APBills.Add(apbill);
                        context.SaveChanges();

                        apbillTransactionNumberConfigViewModel.LastNumber = apbillTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(apbillTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();





                        /***********************************************************************************************/
                        /*Save MasterTrxDetail transaction*/
                        /***********************************************************************************************/


                        //franchiseeMasterTrxDetail.MasterTrxId = masterTrxId;
                        //franchiseeMasterTrxDetail.InvoiceId = TARItem.InvoiceId;
                        ////franchiseeMasterTrxDetail.LineNo = fcdvm.LineNo;
                        //franchiseeMasterTrxDetail.MasterTrxTypeListId = 56; // franchisee chargeback
                        //franchiseeMasterTrxDetail.HeaderId = turnaroundTrx.TurnAroundId;
                        //franchiseeMasterTrxDetail.RegionId = TARItem.RegionId;
                        //franchiseeMasterTrxDetail.InvoiceId = TARItem.InvoiceId;
                        //franchiseeMasterTrxDetail.ServiceTypeListId = 3; // chargeback
                        //franchiseeMasterTrxDetail.ClassId = TARItem.FranchiseeId;
                        //franchiseeMasterTrxDetail.TypelistId = 2;
                        //franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                        //franchiseeMasterTrxDetail.ExtendedPrice = TARItem.PaymentAmount;
                        //franchiseeMasterTrxDetail.FeesDetail = TARItem.FeeAmount > 0 ? true : false;
                        //franchiseeMasterTrxDetail.TaxDetail = false;
                        //franchiseeMasterTrxDetail.TotalFee = TARItem.FeeAmount;
                        //franchiseeMasterTrxDetail.Total = TARItem.TurnAroundCheckAmount;

                        //franchiseeMasterTrxDetail.CreatedBy = vm.CreatedBy;
                        //franchiseeMasterTrxDetail.CreatedDate = vm.CreatedDate;
                        //franchiseeMasterTrxDetail.IsDelete = false;
                        //franchiseeMasterTrxDetail.Transactiondate = transactionDate;

                        //context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                        //context.SaveChanges();

                    }

                    var masterTrxDetailId = context.MasterTrxDetails.Where(m => m.MasterTrxDetailId == TARItem.MasterTrxDetailId).FirstOrDefault();
                    masterTrxDetailId.IsTARPaid = true;
                    context.Entry(masterTrxDetailId).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                    //var payBillingFranchisee = context.PaymentBillingFranchisees.Where(o => o.PaymentBillingFranchiseeId == TARItem.PaymentBillingFranchiseeId).FirstOrDefault();
                    //payBillingFranchisee.IsTARPaid = true;
                    //context.SaveChanges();

                    FranId = (int)TARItem.FranchiseeId;
                    vm.FranchiseeTurnAroundTrxList.Add(turnaroundTrx.MasterTrxId.ToString());
                }
            }

            string result = String.Join(",", vm.FranchiseeTurnAroundTrxList);
            return result;
        }
        public string InsertFranchiseeChargebackTransaction(List<portal_spGet_AP_InvoiceFranchiseeListForChargeback_Result> inputdata, DateTime trxDate)
        {
            FranchiseeChargebackTransactionViewModel vm = new FranchiseeChargebackTransactionViewModel();
            vm.CreatedBy = this.LoginUserId;
            vm.CreatedDate = DateTime.Now;
            vm.FranchiseeChargebackTrxList = new List<string>();


            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel chargebackTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                foreach (var CBItem in inputdata)
                {

                    chargebackTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(10, (int)CBItem.RegionId);
                    DateTime CBTrxDate = DateTime.Parse(CBItem.BillMonth + "/25/" + CBItem.BillYear).Date;
                    DateTime transactionDate;

                    if (DateTime.Now.Date > CBTrxDate)
                    {
                        transactionDate = new DateTime((int)CBItem.BillYear, (int)CBItem.BillMonth, DateTime.DaysInMonth((int)CBItem.BillYear, (int)CBItem.BillMonth));
                    }
                    else
                    {
                        transactionDate = DateTime.Parse(CBItem.BillMonth + "/25/" + CBItem.BillYear).Date;
                    }


                    /*SAVE MASTERTRX*/
                    /*VALIDATION: make sure record does not exists*/

                    MasterTrx franchiseeMasterTrx = context.MasterTrxes.Where(o =>
                        o.MasterTrxTypeListId == 10 &&
                        o.RegionId == (int)(CBItem.RegionId) &&
                        o.BillMonth == CBItem.BillMonth &&
                        o.BillYear == CBItem.BillYear &&
                        o.PeriodId == CBItem.PeriodId &&
                        o.ClassId == CBItem.FranchiseeId).FirstOrDefault();

                    Chargeback chargeback = (franchiseeMasterTrx != null) ? context.Chargebacks.Where(o => o.MasterTrxId == franchiseeMasterTrx.MasterTrxId).FirstOrDefault() : null;

                    /*END VALIDATION: make sure record does not exists*/

                    if (franchiseeMasterTrx == null)
                    {
                        franchiseeMasterTrx = new MasterTrx();
                        franchiseeMasterTrx.MasterTrxTypeListId = 10; // franchisee chargeback
                        franchiseeMasterTrx.TypeListId = 2; // franchisee
                        franchiseeMasterTrx.ClassId = Convert.ToInt32(CBItem.FranchiseeId);
                        franchiseeMasterTrx.TrxDate = transactionDate;

                        franchiseeMasterTrx.StatusId = 1; // open
                        franchiseeMasterTrx.BillMonth = CBItem.BillMonth;
                        franchiseeMasterTrx.BillYear = CBItem.BillYear;
                        franchiseeMasterTrx.RegionId = CBItem.RegionId;
                        franchiseeMasterTrx.CreatedBy = vm.CreatedBy;
                        franchiseeMasterTrx.CreatedDate = vm.CreatedDate;

                        context.MasterTrxes.Add(franchiseeMasterTrx);
                        context.SaveChanges();
                    }

                    if (chargeback == null)
                    {

                        string nextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(10, (int)CBItem.RegionId, vm.CreatedDate);

                        chargeback = new Chargeback();
                        chargeback.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        chargeback.FranchiseeId = CBItem.FranchiseeId;
                        chargeback.TransactionStatusListId = 4; // open
                        chargeback.TransactionNumber = nextTrxNumber;
                        chargeback.BillingPayId = CBItem.BillingPayId;
                        chargeback.RegionId = CBItem.RegionId;
                        chargeback.PeriodId = CBItem.PeriodId;
                        chargeback.TransactionDate = transactionDate;
                        chargeback.ServiceTypeListId = 3;
                        chargeback.CreatedBy = vm.CreatedBy;
                        chargeback.CreatedDate = vm.CreatedDate;

                        context.Chargebacks.Add(chargeback);
                        context.SaveChanges();

                        chargebackTransactionNumberConfigViewModel.LastNumber = chargebackTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(chargebackTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                    }

                    MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
                    franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                    franchiseeMasterTrxDetail.InvoiceId = CBItem.InvoiceId;
                    //franchiseeMasterTrxDetail.LineNo = fcdvm.LineNo;
                    franchiseeMasterTrxDetail.MasterTrxTypeListId = 10; // franchisee chargeback
                    franchiseeMasterTrxDetail.HeaderId = chargeback.ChargebackId;
                    franchiseeMasterTrxDetail.RegionId = CBItem.RegionId;
                    franchiseeMasterTrxDetail.ServiceTypeListId = 3; // chargeback

                    franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                    franchiseeMasterTrxDetail.ExtendedPrice = CBItem.Balance;
                    franchiseeMasterTrxDetail.FeesDetail = true;
                    franchiseeMasterTrxDetail.TaxDetail = false;
                    franchiseeMasterTrxDetail.TotalFee = CBItem.TotalFee;
                    franchiseeMasterTrxDetail.Total = CBItem.ChargebackAmount;

                    franchiseeMasterTrxDetail.CreatedBy = vm.CreatedBy;
                    franchiseeMasterTrxDetail.CreatedDate = vm.CreatedDate;
                    franchiseeMasterTrxDetail.IsDelete = false;
                    franchiseeMasterTrxDetail.Transactiondate = transactionDate;
                    franchiseeMasterTrxDetail.PeriodId = CBItem.PeriodId;
                    franchiseeMasterTrxDetail.ClassId = CBItem.FranchiseeId;
                    franchiseeMasterTrxDetail.TypelistId = 2;
                    franchiseeMasterTrxDetail.FRDeduction = true;
                    franchiseeMasterTrxDetail.FRRevenues = false;
                    franchiseeMasterTrxDetail.DetailDescription = "Chargeback Period " + CBItem.BillMonth.ToString() + "/" + CBItem.BillYear.ToString();

                    context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                    context.SaveChanges();

                    // Update PeriodClosed
                    PeriodClosed periodClosed = new PeriodClosed();
                    periodClosed = context.PeriodCloseds.Where(p => (p.PeriodId == CBItem.PeriodId && p.RegionId == CBItem.RegionId)).FirstOrDefault();

                    periodClosed.ChargebackFinalized = true;
                    context.SaveChanges();

                    // update billing pay

                    var billingPay = context.BillingPays.Where(o => o.InvoiceId == CBItem.InvoiceId).FirstOrDefault();
                    if (billingPay != null)
                    {
                        billingPay.HasBeenChargedBack = true;
                        billingPay.IsChargebackPaid = false;
                        billingPay.ChargebackDate = franchiseeMasterTrx.TrxDate;

                        context.Entry(billingPay).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }

                    vm.FranchiseeChargebackTrxList.Add(chargeback.MasterTrxId.ToString());

                }
            }

            string result = String.Join(",", vm.FranchiseeChargebackTrxList);
            return result;
        }

        public IEnumerable<portal_spGet_AP_TurnAroundListSummary_Result> GetTurnAroundListListSummary(string regionIds = "", DateTime? from = null, DateTime? to = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var list = context.portal_spGet_AP_TurnAroundListSummary(regionIds ?? SelectedRegionId.ToString(), from, to).ToList();
                return list;
            }
        }

        public IEnumerable<portal_spGet_AP_TurnAroundDetailsList_Result> GetTurnAroundDetailList(string regionIds = "", DateTime? payDate = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var list = context.portal_spGet_AP_TurnAroundDetailsList(regionIds ?? SelectedRegionId.ToString(), payDate, -1).ToList();
                return list;
            }
        }


        public IEnumerable<portal_spGet_AP_TurnAroundList_Result> GetTurnAroundListList(string regionIds = "", DateTime? from = null, DateTime? to = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var list = context.portal_spGet_AP_TurnAroundList(regionIds ?? SelectedRegionId.ToString(), from, to).ToList();
                return list;
            }
        }


        /*
        public bool InsertFranchiseeChargebackTransaction(FranchiseeChargebackTransactionViewModel inputdata)
        {
            JKApi.Service.Service.Administration.Company.CompanyService CompanySvc = new JKApi.Service.Service.Administration.Company.CompanyService();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel chargebackTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                foreach (FranchiseeChargebackViewModel fcvm in inputdata.Chargebacks)
                {chargebackTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(10, fcvm.RegionId);
                    

                    // franchisee chargeback mastertrx

                    MasterTrx franchiseeMasterTrx = context.MasterTrxes.Where(o =>
                        o.MasterTrxTypeListId == 10 &&
                        o.RegionId == fcvm.RegionId &&
                        o.BillMonth == fcvm.BillMonth &&
                        o.BillYear == fcvm.BillYear &&
                        o.ClassId == fcvm.FranchiseeId).FirstOrDefault();

                    Chargeback chargeback = (franchiseeMasterTrx != null) ? context.Chargebacks.Where(o => o.MasterTrxId == franchiseeMasterTrx.MasterTrxId).FirstOrDefault() : null;

                    if (franchiseeMasterTrx == null)
                    {
                        franchiseeMasterTrx = new MasterTrx();
                        franchiseeMasterTrx.MasterTrxTypeListId = 10; // franchisee chargeback
                        franchiseeMasterTrx.TypeListId = 2; // franchisee
                        franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
                        franchiseeMasterTrx.TrxDate = new DateTime(inputdata.CreatedDate.Year, inputdata.CreatedDate.Month, 1);
                        franchiseeMasterTrx.StatusId = 1; // open
                        franchiseeMasterTrx.BillMonth = fcvm.BillMonth;
                        franchiseeMasterTrx.BillYear = fcvm.BillYear;
                        franchiseeMasterTrx.RegionId = fcvm.RegionId;
                        franchiseeMasterTrx.CreatedBy = inputdata.CreatedBy;
                        franchiseeMasterTrx.CreatedDate = inputdata.CreatedDate;

                        context.MasterTrxes.Add(franchiseeMasterTrx);
                        context.SaveChanges();
                    }

                    if (chargeback == null)
                    {
                        // franchisee chargeback

                        string nextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(10, fcvm.RegionId, inputdata.CreatedDate);

                        chargeback = new Chargeback();
                        chargeback.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        chargeback.FranchiseeId = fcvm.FranchiseeId;
                        chargeback.TransactionStatusListId = 4; // open
                        chargeback.TransactionNumber = nextTrxNumber;
                        chargeback.RegionId = fcvm.RegionId;
                        chargeback.CreatedBy = inputdata.CreatedBy;
                        chargeback.CreatedDate = inputdata.CreatedDate;

                        context.Chargebacks.Add(chargeback);
                        context.SaveChanges();

                        chargebackTransactionNumberConfigViewModel.LastNumber = chargebackTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(chargebackTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                    }

                    // compute franchisee chargeback fees

                    foreach (FranchiseeChargebackDetailViewModel fcdvm in fcvm.Distributions)
                    {
                        decimal distFees = 0;

                        List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcdvm.MasterTrxDetailId).ToList();
                        List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

                        foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                        {
                            MasterTrxFeeDetail feeDetail = new MasterTrxFeeDetail();
                            if (feeDef.FeePercentage != null) // percentage
                            {
                                feeDetail.FeePercentage = feeDef.FeePercentage;
                                feeDetail.Amount = Math.Round((decimal)(fcdvm.Balance * feeDetail.FeePercentage / 100.0M), 2);
                            }
                            else // flat amount
                            {
                                feeDetail.Amount = feeDef.Amount;
                                feeDetail.FeePercentage = null;
                            }
                            feeDetail.FeeId = feeDef.FeeId;
                            feeDetail.AmountTypeListId = null; // neither debit nor credit; not used in balance calculation
                            feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                            feeDetail.CreatedBy = inputdata.CreatedBy;
                            feeDetail.CreatedDate = inputdata.CreatedDate;

                            distFees += feeDetail.Amount ?? 0;

                            franchiseeFees.Add(feeDetail);
                        }

                        // franchisee chargeback mastertrxdetail

                        decimal balanceMinusFees = Math.Round((decimal)(fcdvm.Balance - distFees), 2); // chargeback amount after taking out fees

                        MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
                        franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        franchiseeMasterTrxDetail.InvoiceId = fcdvm.InvoiceId;
                        franchiseeMasterTrxDetail.LineNo = fcdvm.LineNo;
                        franchiseeMasterTrxDetail.MasterTrxTypeListId = 10; // franchisee chargeback
                        franchiseeMasterTrxDetail.HeaderId = chargeback.ChargebackId;
                        franchiseeMasterTrxDetail.RegionId = fcvm.RegionId;
                        franchiseeMasterTrxDetail.ServiceTypeListId = 3; // chargeback

                        franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                        franchiseeMasterTrxDetail.ExtendedPrice = fcdvm.Balance;
                        franchiseeMasterTrxDetail.FeesDetail = true;
                        franchiseeMasterTrxDetail.TaxDetail = false;
                        franchiseeMasterTrxDetail.TotalFee = distFees;
                        franchiseeMasterTrxDetail.Total = balanceMinusFees;

                        franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
                        franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
                        franchiseeMasterTrxDetail.IsDelete = false;

                        context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                        context.SaveChanges();

                        // insert franchisee fees

                        foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
                        {
                            feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

                            context.MasterTrxFeeDetails.Add(feeDetail);
                            context.SaveChanges();
                        }

                        // update billing pay

                        var billingPay = context.BillingPays.Where(o => o.InvoiceId == fcdvm.InvoiceId).FirstOrDefault();
                        if (billingPay != null)
                        {
                            billingPay.HasBeenChargedBack = true;
                            billingPay.ChargebackDate = franchiseeMasterTrx.TrxDate;

                            context.Entry(billingPay).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }

                    }
                }

                return true;
            }
        }
        */

        public IEnumerable<portal_spGetChargeBackList_Result> GetChargeBackList(int? regionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var list = context.portal_spGetChargeBackList(regionId ?? SelectedRegionId).ToList();
                return list;
            }
        }

        #endregion

        #region Franchisee Pay

        public void GenerateFranchiseeReports(int billMonth, int billYear, int? regionId = null)
        {
            int userid = JKApi.Core.Common.ClaimView.Instance.GetCLAIM_PERSON_INFORMATION().UserId;

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                
                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {

                    var parmas = new DynamicParameters();
                    parmas.Add("@RegionId", regionId ?? SelectedRegionId);
                    parmas.Add("@BillMonth", billMonth);
                    parmas.Add("@BillYear", billYear);
                    parmas.Add("@CreatedBy", userid);
                    conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_Lease", parmas, commandType: CommandType.StoredProcedure);



                    var parmas1 = new DynamicParameters();
                    parmas1.Add("@RegionId", regionId ?? SelectedRegionId);
                    parmas1.Add("@BillMonth", billMonth);
                    parmas1.Add("@BillYear", billYear);
                    parmas1.Add("@CreatedBy", userid);
                    conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_FinderFee", parmas1, commandType: CommandType.StoredProcedure);
                }

                context.Database.CommandTimeout = 180;
                context.portal_spCreate_AP_GenerateFranchiseeReports(regionId ?? SelectedRegionId, billMonth, billYear, userid);
            }
        }

        public List<portal_spGet_AP_GeneratedFranchiseeReportList_Result> GetGeneratedFranchiseeReportList(int? billMonth = null, int? billYear = null, string regionIds = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var resultList = context.portal_spGet_AP_GeneratedFranchiseeReportList(regionIds ?? SelectedRegionId.ToString(), billMonth, billYear).ToList();

                return resultList;
            }
        }

        public List<portal_spGet_AP_FranchiseeReportList_Result> GetFranchiseeReportList(int? billMonth = null, int? billYear = null, string regionIds = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var resultList = context.portal_spGet_AP_FranchiseeReportList(regionIds ?? SelectedRegionId.ToString(), billMonth, billYear).ToList();

                return resultList;
            }
        }


        public List<portal_spGet_AP_FinalizedFranchiseeReportList_Result> GetFinalizedFranchiseeReportList(int? billMonth = null, int? billYear = null, string regionIds = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var resultList = context.portal_spGet_AP_FinalizedFranchiseeReportList(regionIds ?? SelectedRegionId.ToString(), billMonth, billYear).ToList();


                return resultList;
            }
        }

        public List<portal_spGet_AP_AvailableFranchiseeReportGeneratePeriods_Result> GetAvailableFranchiseeReportGeneratePeriods(string RegionId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_AvailableFranchiseeReportGeneratePeriods_Result> lstPeriods = context.portal_spGet_AP_AvailableFranchiseeReportGeneratePeriods(RegionId).ToList();
                return lstPeriods;
            }



        }

        public List<portal_spGet_AP_FinalizedFranchiseeReportPeriods_Result> GetFinalizedFranchiseeReportPeriods(int? regionId = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var resultList = context.portal_spGet_AP_FinalizedFranchiseeReportPeriods(regionId ?? SelectedRegionId).ToList();

                return resultList;
            }
        }

        public bool IsCheckSystemGenerated(int CheckBookTransactionTypeListId)
        {
            bool IsSystemGenerated = false;
            if (CheckBookTransactionTypeListId != -1)
            {
                string sqlStr = "SELECT * FROM CheckBookTransactionTypeList where CheckBookTransactionTypeListId = " + CheckBookTransactionTypeListId;
               

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    //CheckBookTransactionTypeList chkTransactionTypeList = new CheckBookTransactionTypeList();
                    var chkTransactionTypeList = conn.Query(sqlStr).FirstOrDefault();
                    IsSystemGenerated = chkTransactionTypeList.IsSystemGenerated != null ? (bool)chkTransactionTypeList.IsSystemGenerated : false;

                }
            }

            return IsSystemGenerated;
        }

public TurnAroundDetailsViewModel GetTurnAroundDetailsForCheck(int checkBookId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkBook = context.CheckBooks.Where(o => o.CheckBookId == checkBookId && o.SourceTypeListId == 9).FirstOrDefault();
                if (checkBook == null)
                    return null;

                return GetTurnaroundDetailsForAPBill((int)checkBook.MasterTrxId);
            }
        }

       



        public TurnAroundDetailsViewModel GetTurnaroundDetailsForAPBill(int MasterTrxId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var apBill = context.APBills.Where(o => o.MasterTrxId == MasterTrxId).FirstOrDefault();
                if (apBill == null)
                    return null;

                if (apBill.MasterTrxId == null)
                    return null;

                return GetTurnaroundCheckDetails(apBill.RegionId.ToString(), (int)apBill.MasterTrxId);
            }
        }


        public TurnAroundDetailsViewModel GetTurnaroundCheckDetails(string RegionId, int masterTrxId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                TurnAroundDetailsViewModel vm = new TurnAroundDetailsViewModel();
                vm.TurnAround = context.TurnArounds.Where(o => o.MasterTrxId == masterTrxId).FirstOrDefault();

                vm.TurnAroundCheckDetails = context.portal_spGet_AP_TurnAroundCheckDetails(RegionId, masterTrxId).ToList();
                return vm;
            }

        }



        public AccountingFeeRebateDetailsViewModel GetAccountingFeeRebateDetailsForCheck(int checkBookId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkBook = context.CheckBooks.Where(o => o.CheckBookId == checkBookId && o.SourceTypeListId == 9).FirstOrDefault();
                if (checkBook == null)
                    return null;

                return AccountingFeeRebateForAPBill((int)checkBook.MasterTrxId);
            }
        }


        public AccountingFeeRebateDetailsViewModel AccountingFeeRebateForAPBill(int MasterTrxId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var apBill = context.APBills.Where(o => o.MasterTrxId == MasterTrxId).FirstOrDefault();
                if (apBill == null)
                    return null;

                if (apBill.MasterTrxId == null)
                    return null;

                return GetAccountingFeeRebateCheckDetails(apBill.RegionId.ToString(), (int)apBill.MasterTrxId);
            }
        }

        public AccountingFeeRebateDetailsViewModel GetAccountingFeeRebateCheckDetails(string RegionId, int masterTrxId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                AccountingFeeRebateDetailsViewModel vm = new AccountingFeeRebateDetailsViewModel();
                vm.AccountingFeeRebateCheckList = context.portal_spGet_AP_AccountingFeeRebateCheckDetails(RegionId, masterTrxId).ToList();
                return vm;
            }

        }




        public FranchiseeReportDetailsViewModel GetFranchiseeReportDetailsForCheck(int checkBookId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkBook = context.CheckBooks.Where(o => o.CheckBookId == checkBookId && o.SourceTypeListId == 9).FirstOrDefault();
                if (checkBook == null)
                    return null;

                return GetFranchiseeReportDetailsForAPBill((int)checkBook.SourceId);
            }
        }



        public FranchiseeReportDetailsViewModel GetFranchiseeReportDetailsForAPBill(int apBillId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var apBill = context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (apBill == null)
                    return null;

                if (apBill.FranchiseeReportId == null)
                    return null;

                return GetFranchiseeReportDetails((int)apBill.FranchiseeReportId);
            }
        }

        public FranchiseeReportFinalizedDetailsViewModel GetFranchiseeReportFinalizedDetailsForCheck(int checkBookId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var checkBook = context.CheckBooks.Where(o => o.CheckBookId == checkBookId && o.SourceTypeListId == 9).FirstOrDefault();
                if (checkBook == null)
                    return null;

                return GetFranchiseeReportFinalizedDetailsForAPBill((int)checkBook.SourceId);
            }
        }

        public FranchiseeReportFinalizedDetailsViewModel GetFranchiseeReportFinalizedDetailsForAPBill(int apBillId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var apBill = context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (apBill == null)
                    return null;

                if (apBill.SourceId == null)
                    return null;

                return GetFranchiseeReportDetailsFinalized((int)apBill.SourceId);
            }
        }


        public FranchiseeReportFinalizedDetailsViewModel GetFranchiseeReportDetailsFinalized(int franchiseeReportId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                FranchiseeReportFinalizedDetailsViewModel vm = new FranchiseeReportFinalizedDetailsViewModel();

                vm.Report = context.FranchiseeReportFinalizeds.Where(o => o.FranchiseeReportFinalizedId == franchiseeReportId).FirstOrDefault();

                List<portal_spGet_AP_FranchiseeReportFinalizedDetails_Result> finResultList = context.portal_spGet_AP_FranchiseeReportFinalizedDetails(franchiseeReportId).ToList();
                List<portal_spGet_AP_FranchiseeReportFinalizedDetailsByCustomerAndServiceType_Result> resultList2 = context.portal_spGet_AP_FranchiseeReportFinalizedDetailsByCustomerAndServiceType(franchiseeReportId).ToList();

                vm.DetailsByTransaction = finResultList;
                List<FranchiseeReportFinalizedDetailsByServiceViewModel> svms = new List<FranchiseeReportFinalizedDetailsByServiceViewModel>();
                foreach (portal_spGet_AP_FranchiseeReportFinalizedDetailsByCustomerAndServiceType_Result res in resultList2)
                {
                    /*
                    string serviceType = res.ServiceType ?? "Unspecified";

                    var svm = svms.Where(o => o.ServiceType == serviceType).FirstOrDefault();
                    if (svm == null)
                    {
                        svm = new FranchiseeReportFinalizedDetailsByServiceViewModel();
                        svm.ServiceType = serviceType;
                        svm.ServiceTypeListId = (int)res.ServiceTypeListId;
                        svm.ServiceTypeGroupListName = res.ServiceTypeGroupListName;
                        svm.CreatedBy = (int)res.CreatedBy;
                        svm.DisplayOrder = res.DisplayOrder; //(int)(res.DisplayOrder ?? 9999);
                        svm.Details = new List<portal_spGet_AP_FranchiseeReportFinalizedDetailsByCustomerAndServiceType_Result>();
                        svms.Add(svm);
                    }

                    svm.Details.Add(res);
                    */


                    string serviceType = res.ServiceType ?? "Unspecified";
                    string serviceTypeGroupListName = res.ServiceTypeGroupListName ?? "Unspecified";

                    if ((int)res.CreatedBy == -1)
                    {
                        var svm = svms.Where(o => o.ServiceType == serviceType).FirstOrDefault();
                        if (svm == null)
                        {
                            svm = new FranchiseeReportFinalizedDetailsByServiceViewModel();
                            svm.ServiceType = serviceType;
                            svm.ServiceTypeListId = (int)res.ServiceTypeListId;
                            svm.ServiceTypeGroupListName = res.ServiceTypeGroupListName;
                            svm.CreatedBy = (int)res.CreatedBy;
                            svm.DisplayOrder = res.DisplayOrder; //(int)(res.DisplayOrder ?? 9999);
                            svm.Details = new List<portal_spGet_AP_FranchiseeReportFinalizedDetailsByCustomerAndServiceType_Result>();
                            svms.Add(svm);
                        }
                        svm.Details.Add(res);
                    }
                    else
                    {
                        var svm = svms.Where(o => o.ServiceTypeGroupListName == serviceTypeGroupListName).FirstOrDefault();
                        if (svm == null)
                        {
                            svm = new FranchiseeReportFinalizedDetailsByServiceViewModel();
                            svm.ServiceType = res.ServiceTypeGroupListName;
                            svm.ServiceTypeListId = (int)res.ServiceTypeListId;
                            svm.ServiceTypeGroupListName = res.ServiceTypeGroupListName;
                            svm.CreatedBy = (int)res.CreatedBy;
                            svm.DisplayOrder = res.DisplayOrder; //(int)(res.DisplayOrder ?? 9999);
                            svm.Details = new List<portal_spGet_AP_FranchiseeReportFinalizedDetailsByCustomerAndServiceType_Result>();
                            svms.Add(svm);
                        }
                        svm.Details.Add(res);

                    }

                }
                svms = svms.OrderBy(o => o.DisplayOrder).ToList();
                vm.DetailsByService = svms;

                List<portal_spGet_AP_FranchiseeReportFinalizedDeductions_Result> resultList3 = context.portal_spGet_AP_FranchiseeReportFinalizedDeductions(franchiseeReportId).ToList();

                // service types: chargeback, finders fee, lease, misc r.o., supplies, special misc.
                List<int> showServiceTypeIds = new List<int> { 3, 4, 11, 14, 18, 47 };

                List<FranchiseeReportFinalizedDeductionViewModel> dvms = new List<FranchiseeReportFinalizedDeductionViewModel>();
                foreach (portal_spGet_AP_FranchiseeReportFinalizedDeductions_Result res in resultList3)
                {
                    int serviceTypeListId = res.ServiceTypeListId ?? -1;

                    string billMonthYears = string.Format("{0:00}/{1}", res.BillMonth, res.BillYear);
                    var dvm = dvms.Where(o => o.ServiceTypeListId == serviceTypeListId).FirstOrDefault();
                    //var dvm = dvms.Where(o => o.ServiceType == serviceType).FirstOrDefault();

                    if (dvm == null)
                    {

                        dvm = new FranchiseeReportFinalizedDeductionViewModel();
                        dvm.ServiceType = res.ServiceType;
                        dvm.ServiceTypeListId = (int)res.ServiceTypeListId;
                        dvm.BillMonthYears = billMonthYears;
                        dvm.DisplayOrder = (int)(res.DisplayOrder ?? 9999);
                        //dvm.DisplaySubReport = (bool)(res.DisplaySubReport ?? false);
                        dvm.DisplaySubReport = (bool)(res.DisplaySubReport ?? false); //showServiceTypeIds.Contains(res.ServiceTypeListId ?? -1); // todo: find more elegant solution
                        dvm.DisplayLeaseMessage = (bool)(res.DisplayLeaseMessage);
                        //dvm.IsSpecialDeduction = (bool)(res.IsSpecial ?? false);
                        dvm.ReSell = (int)res.ReSell;
                        dvm.FeeId = (int)res.FeeId;
                        //dvm.MinumumRoyaltyAmount = (decimal)res.MinumumRoyaltyAmount;
                        //dvm.UseMinumumRoyaltyAmount = (int)res.UseMinumumRoyaltyAmount;
                        dvm.ServiceTypeGroupListId = (int)res.ServiceTypeGroupListId;
                        dvm.GroupName = res.GroupName;
                        dvm.Deductions = new List<portal_spGet_AP_FranchiseeReportFinalizedDeductions_Result>();
                        dvms.Add(dvm);
                    }

                    dvm.Deductions.Add(res);
                }
                dvms = dvms.OrderBy(o => o.DisplayOrder).ThenBy(o => o.ServiceTypeListId).ToList();
                vm.Deductions = dvms;

                return vm;
            }


        }

        public FranchiseeReportDetailsViewModel GetFranchiseeReportDetails(int franchiseeReportId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {


                FranchiseeReportDetailsViewModel vm = new FranchiseeReportDetailsViewModel();

                vm.Report = context.FranchiseeReports.Where(o => o.FranchiseeReportId == franchiseeReportId).FirstOrDefault();

                List<portal_spGet_AP_FranchiseeReportDetails_Result> resultList = context.portal_spGet_AP_FranchiseeReportDetails(franchiseeReportId).ToList();
                List<portal_spGet_AP_FranchiseeReportDetailsByCustomerAndServiceType_Result> resultList2 = context.portal_spGet_AP_FranchiseeReportDetailsByCustomerAndServiceType(franchiseeReportId).ToList();

                vm.DetailsByTransaction = resultList;
                List<FranchiseeReportDetailsByServiceViewModel> svms = new List<FranchiseeReportDetailsByServiceViewModel>();
                foreach (portal_spGet_AP_FranchiseeReportDetailsByCustomerAndServiceType_Result res in resultList2)
                {
                    string serviceType = res.ServiceType ?? "Unspecified";
                    string serviceTypeGroupListName = res.ServiceTypeGroupListName ?? "Unspecified";

                    if ((int)res.CreatedBy == -1)
                    {
                        var svm = svms.Where(o => o.ServiceType == serviceType).FirstOrDefault();
                        if (svm == null)
                        {
                            svm = new FranchiseeReportDetailsByServiceViewModel();
                            svm.ServiceType = serviceType;
                            svm.ServiceTypeListId = (int)res.ServiceTypeListId;
                            svm.ServiceTypeGroupListName = res.ServiceTypeGroupListName;
                            svm.CreatedBy = (int)res.CreatedBy;
                            svm.DisplayOrder = res.DisplayOrder; //(int)(res.DisplayOrder ?? 9999);
                            svm.Details = new List<portal_spGet_AP_FranchiseeReportDetailsByCustomerAndServiceType_Result>();
                            svms.Add(svm);
                        }
                        svm.Details.Add(res);
                    }
                    else
                    {
                        var svm = svms.Where(o => o.ServiceTypeGroupListName == serviceTypeGroupListName).FirstOrDefault();
                        if (svm == null)
                        {
                            svm = new FranchiseeReportDetailsByServiceViewModel();
                            svm.ServiceType = res.ServiceTypeGroupListName;
                            svm.ServiceTypeListId = (int)res.ServiceTypeListId;
                            svm.ServiceTypeGroupListName = res.ServiceTypeGroupListName;
                            svm.CreatedBy = (int)res.CreatedBy;
                            svm.DisplayOrder = res.DisplayOrder; //(int)(res.DisplayOrder ?? 9999);
                            svm.Details = new List<portal_spGet_AP_FranchiseeReportDetailsByCustomerAndServiceType_Result>();
                            svms.Add(svm);
                        }
                        svm.Details.Add(res);

                    }


                }
                svms = svms.OrderBy(o => o.DisplayOrder).ToList();
                vm.DetailsByService = svms;

                List<portal_spGet_AP_FranchiseeReportDeductions_Result> resultList3 = context.portal_spGet_AP_FranchiseeReportDeductions(franchiseeReportId).ToList();

                // service types: chargeback, finders fee, lease, misc r.o., supplies, special misc.
                List<int> showServiceTypeIds = new List<int> { 3, 4, 11, 14, 18, 47 };

                List<FranchiseeReportDeductionViewModel> dvms = new List<FranchiseeReportDeductionViewModel>();
                foreach (portal_spGet_AP_FranchiseeReportDeductions_Result res in resultList3)
                {
                    int serviceTypeListId = res.ServiceTypeListId ?? -1;

                    string billMonthYears = string.Format("{0:00}/{1}", res.BillMonth, res.BillYear);
                    var dvm = dvms.Where(o => o.ServiceTypeListId == serviceTypeListId).FirstOrDefault();
                    //var dvm = dvms.Where(o => o.ServiceType == serviceType).FirstOrDefault();

                    if (dvm == null)
                    {

                        dvm = new FranchiseeReportDeductionViewModel();
                        dvm.ServiceType = res.ServiceType;
                        dvm.ServiceTypeListId = (int)res.ServiceTypeListId;
                        dvm.BillMonthYears = billMonthYears;
                        dvm.DisplayOrder = (int)(res.DisplayOrder ?? 9999);
                        //dvm.DisplaySubReport = (bool)(res.DisplaySubReport ?? false);
                        dvm.DisplaySubReport = (bool)(res.DisplaySubReport ?? false); //showServiceTypeIds.Contains(res.ServiceTypeListId ?? -1); // todo: find more elegant solution
                        dvm.DisplayLeaseMessage = (bool)(res.DisplayLeaseMessage);
                        //dvm.IsSpecialDeduction = (bool)(res.IsSpecial ?? false);
                        dvm.ReSell = (int)res.ReSell;
                        dvm.FeeId = (int)res.FeeId;
                        //dvm.MinumumRoyaltyAmount = (decimal)res.MinumumRoyaltyAmount;
                        //dvm.UseMinumumRoyaltyAmount = (int)res.UseMinumumRoyaltyAmount;
                        dvm.ServiceTypeGroupListId = (int)res.ServiceTypeGroupListId;
                        dvm.GroupName = res.GroupName;
                        dvm.Deductions = new List<portal_spGet_AP_FranchiseeReportDeductions_Result>();
                        dvms.Add(dvm);
                    }

                    dvm.Deductions.Add(res);
                }
                dvms = dvms.OrderBy(o => o.DisplayOrder).ThenBy(o=>o.ServiceTypeListId).ToList();
                vm.Deductions = dvms;

                return vm;
            }


        }

        public List<portal_spGet_AP_FranchiseeReportDeductions_Result> GetFranchiseeReportDeductions(int franchiseeReportId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_FranchiseeReportDeductions_Result> resultList = context.portal_spGet_AP_FranchiseeReportDeductions(franchiseeReportId).ToList();

                return resultList;
            }
        }

        public bool DeleteGeneratedFranchiseeReport(int franchiseeReportId)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var report = context.FranchiseeReports.Where(o => o.FranchiseeReportId == franchiseeReportId && o.IsFinalized != 1).FirstOrDefault();

                if (report != null)
                {
                    context.Entry(report).State = System.Data.Entity.EntityState.Deleted;

                    context.FranchiseeReportDetails.RemoveRange(context.FranchiseeReportDetails.Where(o => o.FranchiseeReportId == franchiseeReportId));

                    return context.SaveChanges() == 0;
                }

                return false;
            }

        }

        public bool ClearGeneratedFranchiseeReport(string RegionIds, int PeriodId)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var result = context.portal_spGet_AP_ClearGeneratedFranchiseeReportList(RegionIds, PeriodId); 
            }

            return false;

        }



        public bool FinalizeFranchiseeReportCreate(string selectedRegionId, int periodId, string franchiseeReportId, int userId)
        {

            /*franchiseeReportId is not used here, but we will leave it in case they want checkboxes back*/
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var report = context.portal_spCreate_AP_FinalizeFranchiseeReports(selectedRegionId, periodId, franchiseeReportId, userId);
                return true;

            }
        }


        public bool FinalizeFranchiseeReport(int franchiseeReportId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var report = context.FranchiseeReports.Where(o => o.FranchiseeReportId == franchiseeReportId && o.IsFinalized != 1).FirstOrDefault();

                if (report != null)
                {
                    report.IsFinalized = 1;
                    report.FinalizedBy = this.LoginUserId;
                    report.FinalizedDate = DateTime.Now;
                    report.TransactionStatusListId = 14; // finalized
                    context.Entry(report).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        //public List<portal_spGet_AP_FranchiseeTurnaroundCheckList_Result> GetFranchiseeTurnaroundCheckList(int? regionId, DateTime startDate, DateTime endDate, int TurnAroundCheckType)
        //{
        //    using (jkDatabaseEntities context = new jkDatabaseEntities())
        //    {
        //        var resultList = context.portal_spGet_AP_FranchiseeTurnaroundCheckList(regionId ?? SelectedRegionId, startDate, endDate, TurnAroundCheckType).ToList();

        //        return resultList;
        //    }
        //}

        //public List<portal_spGet_AP_FranchiseeTurnaroundCheckDetails_Result> GetFranchiseeTurnaroundCheckDetails(int? regionId, int franchiseeId, DateTime startDate, DateTime endDate)
        //{
        //    using (jkDatabaseEntities context = new jkDatabaseEntities())
        //    {
        //        var resultList = context.portal_spGet_AP_FranchiseeTurnaroundCheckDetails(regionId ?? SelectedRegionId, franchiseeId, startDate, endDate).ToList();

        //        return resultList;
        //    }
        //}

        public int InsertFranchiseeTurnaroundCheck(FranchiseeTurnaroundCheckViewModel inputdata)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                // turnaround check

                FranchiseeTurnaroundCheck check = new FranchiseeTurnaroundCheck();
                check.RegionId = inputdata.RegionId;
                check.FranchiseeId = inputdata.FranchiseeId;
                check.StartDate = inputdata.StartDate;
                check.EndDate = inputdata.EndDate;
                check.Amount = inputdata.Amount;
                check.CreatedBy = inputdata.CreatedBy;
                check.CreatedDate = inputdata.CreatedDate;

                context.FranchiseeTurnaroundChecks.Add(check);
                context.SaveChanges();

                return check.FranchiseeTurnaroundCheckId;
            }
        }

        public List<portal_spGet_AP_APBillListByType_Result> GetAPBillListForCheckType(int? regionId, int checkTypeId, int TransactionStausId, string APBillList)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_APBillListByType_Result> results = context.portal_spGet_AP_APBillListByType(regionId ?? this.SelectedRegionId, checkTypeId, TransactionStausId, APBillList).OrderBy(l => l.FranchiseeNo).ToList();

                return results;
            }
        }

        public List<portal_spGet_AP_PendingAPBillListByType_Result> GetPendingAPBillListForCheckType(int? regionId, int? @CheckbookTransactionTypeListId, int trxStatus, string APBillList)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_PendingAPBillListByType_Result> results = context.portal_spGet_AP_PendingAPBillListByType(regionId, CheckbookTransactionTypeListId, trxStatus, APBillList).ToList();

                return results;
            }
        }

        public List<portal_spGet_AP_APBillListById_Result> GetAPBillListForCheckById(int? regionId, string APBillListId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_APBillListById_Result> results = context.portal_spGet_AP_APBillListById(regionId ?? this.SelectedRegionId, APBillListId).ToList();

                return results;
            }
        }


        public List<portal_spGet_AP_OpenNotPrintedAPBillListByType_Result> GetOpenNotPrintedAPBillListForCheckType(int? regionId, int checkTypeId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AP_OpenNotPrintedAPBillListByType_Result> results = context.portal_spGet_AP_OpenNotPrintedAPBillListByType(regionId ?? this.SelectedRegionId, checkTypeId).ToList();

                return results;
            }
        }

        public int UpdateOpenNotPrintedAPBill(int apbillId)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var apBill = context.APBills.Where(o => o.APBillId == apbillId).FirstOrDefault();
                var checkBook = context.CheckBooks.Where(c => c.MasterTrxId == apBill.MasterTrxId).FirstOrDefault();
                

                if (apBill == null) return -1;
                apBill.TransactionStatusListId = 3; //Completed
              
                if (context.SaveChanges() > 0)
                {
                    switch (apBill.CheckBookTransactionTypeListId)
                    {
                        case FRANCHISEE_DUE:
                            var frMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == apBill.MasterTrxId).FirstOrDefault();
                            frMasterTrx.StatusId = 3;
                            frMasterTrx.ModifiedBy = this.LoginUserId;
                            frMasterTrx.ModifiedDate = DateTime.Now;
                            context.Entry(frMasterTrx).State = System.Data.Entity.EntityState.Modified;

                            apBill.TransactionStatusListId = 3; //paid
                            apBill.ModifiedBy = this.LoginUserId;
                            apBill.ModifiedDate = DateTime.Now;
                            context.Entry(apBill).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            break;

                        case ACCOUNTING_FEE_REBATE:
                            var masterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == apBill.MasterTrxId).FirstOrDefault();
                            masterTrx.StatusId = 3;
                            masterTrx.ModifiedBy = this.LoginUserId;
                            masterTrx.ModifiedDate = DateTime.Now;
                            context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                            apBill.TransactionStatusListId = 3; 
                            apBill.ModifiedBy = this.LoginUserId;
                            apBill.ModifiedDate = DateTime.Now;
                            context.Entry(apBill).State = System.Data.Entity.EntityState.Modified;

                            var accountingFeeRebate = context.AccountingFeeRebates.Where(a => a.AccountingFeeRebateId == apBill.SourceId).FirstOrDefault();
                            accountingFeeRebate.TransactionStatusListId = 3; //completed
                            context.Entry(accountingFeeRebate).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            break;

                        case TURN_AROUND:
                            var turnAround = context.TurnArounds.Where(t => t.MasterTrxId == apBill.MasterTrxId).ToList();

                            foreach (TurnAround item in turnAround)
                            {
                                var TARmasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == item.MasterTrxId).FirstOrDefault();
                                TARmasterTrx.StatusId = 3;
                                TARmasterTrx.ModifiedBy = this.LoginUserId;
                                TARmasterTrx.ModifiedDate = DateTime.Now;
                                context.Entry(TARmasterTrx).State = System.Data.Entity.EntityState.Modified;

                                item.TransactionStatusListId = 3; // completed
                                context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                            }
                            break;
                    }
                }


                return apbillId;
            }

        }

        

        public int InsertCheckBookFromAPBill(int apBillId, int bankId, DateTime trxDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var trxPeriod = context.Periods.Where(p => p.BillMonth == trxDate.Month && p.BillYear == trxDate.Year).FirstOrDefault();

                var apBill = context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (apBill == null) return -1;

                if (apBill.CheckAmount <= 0)
                {
                   return -1;
                }

                var masterTrxDtl = context.MasterTrxDetails.Where(m => m.MasterTrxId == apBill.MasterTrxId).FirstOrDefault();
                var checkbookTransactionTypeList = context.CheckBookTransactionTypeLists.Where(c => c.CheckBookTransactionTypeListId == apBill.CheckBookTransactionTypeListId).FirstOrDefault();
                var manualCheck = context.ManualChecks.Where(o => o.ManualCheckId == apBill.ManualCheckId).FirstOrDefault();
                if (manualCheck != null)
                        bankId = (int)(manualCheck?.BankId);

                var bank = new Bank();

                if (bankId == 0)
                {
                    bank = context.Banks.Where(o => o.RegionId == apBill.RegionId).FirstOrDefault();
                }
                else
                {
                    bank = context.Banks.Where(o => o.BankId == bankId).FirstOrDefault();
                }

                if (bank == null)
                   return -1;

                string chkMemo = "";

                if (IsCheckSystemGenerated(Convert.ToInt32(checkbookTransactionTypeList.CheckBookTransactionTypeListId)))
                {

                    switch (apBill.CheckBookTransactionTypeListId)
                    {
                        case FRANCHISEE_DUE:
                            DateTime frTrxdate = new DateTime(Convert.ToInt32(apBill.BillYear), Convert.ToInt32(apBill.BillMonth), 1);
                            chkMemo = "Due Franchisee for " + frTrxdate.ToString("MMMM") + " " + apBill.BillYear;
                            break;
                        case ACCOUNTING_FEE_REBATE:
                            chkMemo = "Accounting Fee Rebate";
                            break;
                        case TURN_AROUND:
                            chkMemo = "TAR - Charge Back Paid";
                            break;
                        default:
                            chkMemo = masterTrxDtl.DetailDescription;
                            break;

                    }
                }else
                {
                    chkMemo = manualCheck?.Memo;
                } 

                CheckBook cb = new CheckBook();
                   cb.FundTypeListId = 3; // check
                   cb.BankId = bankId;
                   cb.TypeListId = apBill.TypeListId;
                   cb.ClassId = apBill.ClassId;
                   cb.RegionId = apBill.RegionId;
                   cb.MasterTrxId = apBill.MasterTrxId;
                   
                    /* 10-8-2018 Maria: use the month and year of the date the user selects on the UI (Check Date) to get the perid Id.
                     *  This date is passed as a parameter in this function (See var trxPeriod above)
                    */
                   cb.BillMonth = trxPeriod.BillMonth;  //apBill.BillMonth;
                   cb.BillYear = trxPeriod.BillYear; //apBill.BillYear;
                   cb.PeriodId = trxPeriod.PeriodId.ToString();

                   cb.ReferenceNumber = Convert.ToString(bank.NextCheckNumber);
                   cb.Memo = chkMemo;
                   cb.TransactionStatusListId = 4; // open
                   cb.SourceTypeListId = 9; // AP Bill
                   cb.SourceId = apBillId;
                   cb.CreatedBy = this.LoginUserId;
                   cb.CreatedDate = DateTime.Now;
                   cb.TransactionDate = trxDate;
                   cb.Amount = apBill.CheckAmount;
                   cb.Reconciled = false;
                   cb.CheckBookTransactionTypeListId = apBill.CheckBookTransactionTypeListId;
                   cb.CheckBookAmountTypeListId = checkbookTransactionTypeList.CheckBookAmountTypeListId;
                   cb.MasterTrxTypeListId = 22;
                   cb.IsDelete = false;
                   cb.Notes = chkMemo;


                CheckBookTransactionTypeList oCBT = context.CheckBookTransactionTypeLists.SingleOrDefault(o => o.CheckBookTransactionTypeListId == apBill.CheckBookTransactionTypeListId);
                if (oCBT != null)
                {
                    cb.MasterTrxTypeListId = oCBT.MasterTrxTypeListId;
                    cb.ServiceTypeListId = oCBT.ServiceTypeListId;
                    cb.Code = oCBT.Code;
                }

                context.CheckBooks.Add(cb);

                   bank.NextCheckNumber = bank.NextCheckNumber + 1;
                   context.Entry(bank).State = System.Data.Entity.EntityState.Modified;

                   context.SaveChanges();

                    //if (context.SaveChanges() > 0)
                    //{
                    //    CheckSourceDataWorker(apBill, context, trxDate);
                    //}

                return cb.CheckBookId;
            } 
        }


        public void CheckSourceDataWorker(int apBillId, int CheckBookTransactionTypeListId, string TransactionStatus)
        {
          
            if (IsCheckSystemGenerated(CheckBookTransactionTypeListId))
            {
                switch (CheckBookTransactionTypeListId)
                {
                    case FRANCHISEE_DUE:
                        FranchiseeDueCheck(apBillId, TransactionStatus);
                        break;
                    case ACCOUNTING_FEE_REBATE:
                        AccountingFeeRebateWorker(apBillId, TransactionStatus);
                        break;
                    case TURN_AROUND:
                        TrunAroundWorker(apBillId, TransactionStatus);
                        break;
                    default:
                        APBillCheck(apBillId, TransactionStatus);
                        break;
                }
            } else
            {

                ManualCheck(apBillId, TransactionStatus);
            }

        }

        public int UndoCheck(int apBillId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var apBillObj = context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (apBillObj == null) return -1;

                var masterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == apBillObj.MasterTrxId).FirstOrDefault();
                var checkbook = context.CheckBooks.Where(m => m.MasterTrxId == apBillObj.MasterTrxId).FirstOrDefault();

                if (checkbook != null)
                {
                    context.Entry(checkbook).State = System.Data.Entity.EntityState.Deleted;
                }


                context.SaveChanges();
                return apBillObj.APBillId;
            }

        }


        public int FranchiseeDueCheck(int apBillId, string TransactionStatus)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                var apBillObj = context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (apBillObj == null) return -1;

                var franchiseeReport = context.FranchiseeReportFinalizeds.Where(o => o.FranchiseeReportFinalizedId == apBillObj.SourceId && apBillObj.SourceTypeListId == 48).FirstOrDefault();
                var masterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == apBillObj.MasterTrxId).FirstOrDefault();
                var checkbook = context.CheckBooks.Where(m => m.MasterTrxId == apBillObj.MasterTrxId).FirstOrDefault();

                switch (TransactionStatus) {
                    case CLOSE:
                       
                        masterTrx.StatusId = 3;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        apBillObj.TransactionStatusListId = 6; //paid
                        apBillObj.ModifiedBy = this.LoginUserId;
                        apBillObj.ModifiedDate = DateTime.Now;
                        context.Entry(apBillObj).State = System.Data.Entity.EntityState.Modified;

                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 3;
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                         }


                        /*General ledger*/
                        GeneralLedger GLDebit = new GeneralLedger();
                        GLDebit.MasterTrxId = apBillObj.MasterTrxId;
                        GLDebit.LedgerAcctId = 1; /*1101 Acct*/
                        GLDebit.Debit = apBillObj.CheckAmount;
                        GLDebit.Credit = 0.00m;
                        GLDebit.IsDelete = false;
                        GLDebit.RegionId = apBillObj.RegionId;
                        GLDebit.PeriodId = apBillObj.PeriodId;

                        context.GeneralLedgers.Add(GLDebit);

                        GeneralLedger GLCredit = new GeneralLedger();
                        GLCredit.MasterTrxId = apBillObj.MasterTrxId;
                        GLCredit.LedgerAcctId = 5; /*2030 Acct*/
                        GLCredit.Debit = 0.00m;
                        GLCredit.Credit = apBillObj.CheckAmount;
                        GLCredit.IsDelete = false;
                        GLCredit.RegionId = apBillObj.RegionId;
                        GLCredit.PeriodId = apBillObj.PeriodId;

                        context.GeneralLedgers.Add(GLCredit);
                        context.SaveChanges();
                        break;

                    case OPEN:
                        masterTrx.StatusId = 4;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        apBillObj.TransactionStatusListId = 4; 
                        apBillObj.ModifiedBy = this.LoginUserId;
                        apBillObj.ModifiedDate = DateTime.Now;
                        context.Entry(apBillObj).State = System.Data.Entity.EntityState.Modified;

                        
                        if (checkbook != null)
                        {
                           context.Entry(checkbook).State = System.Data.Entity.EntityState.Deleted;
                        }
                        context.SaveChanges();
                        break;

                    case PENDING:
                        masterTrx.StatusId = 20;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        apBillObj.TransactionStatusListId = 20;
                        apBillObj.ModifiedBy = this.LoginUserId;
                        apBillObj.ModifiedDate = DateTime.Now;
                        context.Entry(apBillObj).State = System.Data.Entity.EntityState.Modified;

                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 6;
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }
                        context.SaveChanges();
                        break;


                }

                return apBillObj.APBillId;
            }
        


           }

        public int AccountingFeeRebateWorker(int apBillId, string TransactionStatus)
        {

            using (jkDatabaseEntities Context = new jkDatabaseEntities())
            {

                var afAapBill = Context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (afAapBill == null) return -1;

                var masterTrx = Context.MasterTrxes.Where(m => m.MasterTrxId == afAapBill.MasterTrxId).FirstOrDefault();
                var checkbook = Context.CheckBooks.Where(m => m.MasterTrxId == afAapBill.MasterTrxId).FirstOrDefault();

                switch (TransactionStatus)
                {
                    case CLOSE:
                        masterTrx.StatusId = 3;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        afAapBill.TransactionStatusListId = 6; //paid

                        afAapBill.ModifiedBy = this.LoginUserId;
                        afAapBill.ModifiedDate = DateTime.Now;
                        Context.Entry(afAapBill).State = System.Data.Entity.EntityState.Modified;


                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 3;
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }


                        var accountingFeeRebate = Context.AccountingFeeRebates.Where(a => a.AccountingFeeRebateId == afAapBill.SourceId).FirstOrDefault();
                        accountingFeeRebate.TransactionStatusListId = 3; //completed
                        Context.Entry(accountingFeeRebate).State = System.Data.Entity.EntityState.Modified;


                        AccountingFeeRebate AcctFeeRebate = new AccountingFeeRebate();
                        AcctFeeRebate.ClassId = afAapBill.ClassId;
                        AcctFeeRebate.TypeListId = afAapBill.TypeListId;
                        AcctFeeRebate.RegionId = afAapBill.RegionId;
                        AcctFeeRebate.periodId = afAapBill.PeriodId;
                        AcctFeeRebate.Percentage = 0.00m;
                        AcctFeeRebate.GrossRevenue = 0.00m;
                        AcctFeeRebate.Amount = 0.00m;
                        AcctFeeRebate.TransactionDate = afAapBill.TransactionDate;
                        AcctFeeRebate.TransactionStatusListId = 4;
                        AcctFeeRebate.CreatedBy = this.LoginUserId;
                        AcctFeeRebate.CreatedDate = DateTime.Now;
                        AcctFeeRebate.Balance = 0.00m;
                        AcctFeeRebate.PaidAmount = afAapBill.CheckAmount;
                        AcctFeeRebate.PaidDate = afAapBill.TransactionDate;
                        Context.AccountingFeeRebates.Add(AcctFeeRebate);
                        Context.SaveChanges();
                        break;

                    case OPEN:
                        masterTrx.StatusId = 4;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        afAapBill.TransactionStatusListId = 4; //open

                        afAapBill.ModifiedBy = this.LoginUserId;
                        afAapBill.ModifiedDate = DateTime.Now;
                        Context.Entry(afAapBill).State = System.Data.Entity.EntityState.Modified;


                        if (checkbook != null)
                        {
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Deleted;
                        }
                        Context.SaveChanges();
                        break;

                    case PENDING:
                        masterTrx.StatusId = 20; //pending
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        afAapBill.TransactionStatusListId = 20; //pending

                        afAapBill.ModifiedBy = this.LoginUserId;
                        afAapBill.ModifiedDate = DateTime.Now;
                        Context.Entry(afAapBill).State = System.Data.Entity.EntityState.Modified;


                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 6; //paid
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }
                        Context.SaveChanges();
                        break;

                }
                return afAapBill.APBillId;
            }

        }

        public int TrunAroundWorker(int apBillId, string TransactionStatus)
        {

            using (jkDatabaseEntities Context = new jkDatabaseEntities())
            {
                var totalItems = 1;
                decimal negativeDueBalance = 0.00m;

                var tarApBill = Context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (tarApBill == null) return -1;

                var turnAround = Context.TurnArounds.Where(o => o.MasterTrxId == tarApBill.MasterTrxId).ToList();
                var masterTrx = Context.MasterTrxes.Where(m => m.MasterTrxId == tarApBill.MasterTrxId).FirstOrDefault();
                var checkbook = Context.CheckBooks.Where(m => m.MasterTrxId == tarApBill.MasterTrxId).FirstOrDefault();

                switch (TransactionStatus)
                {
                    case CLOSE:
                        masterTrx.StatusId = 3;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        tarApBill.TransactionStatusListId = 6; //paid
                        tarApBill.ModifiedBy = this.LoginUserId;
                        tarApBill.ModifiedDate = DateTime.Now;
                        Context.Entry(tarApBill).State = System.Data.Entity.EntityState.Modified;


                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 3;
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }

                        foreach (TurnAround item in turnAround)
                        {

                            item.TransactionStatusListId = 3; // completed
                            Context.Entry(item).State = System.Data.Entity.EntityState.Modified;

                            if (item.NegativeDueId != -1)
                            {
                                if (totalItems == 1)
                                {
                                    var currNegativeDue = Context.NegativeDues.Where(o => o.NegativeDueId == item.NegativeDueId).FirstOrDefault();
                                    negativeDueBalance = Convert.ToDecimal(currNegativeDue.Balance);
                                    currNegativeDue.TransactionStatusListId = 3;
                                    Context.Entry(currNegativeDue).State = System.Data.Entity.EntityState.Modified;
                                }

                                negativeDueBalance = Convert.ToDecimal(negativeDueBalance - item.NegativeDueAmount);

                                NegativeDue negativeDue = new NegativeDue();
                                negativeDue.ClassId = item.FranchiseeId;
                                negativeDue.TypeListId = 2;
                                negativeDue.CustomerId = item.CustomerId;
                                negativeDue.RegionId = item.RegionId;
                                negativeDue.PeriodId = item.PeriodId;
                                negativeDue.Amount = 0.00m;
                                negativeDue.AppliedAmount = item.NegativeDueAmount;
                                negativeDue.ApplySourceId = 2; /*TAR Payment*/
                                negativeDue.Rollover = false;
                                negativeDue.Balance = negativeDueBalance;
                                negativeDue.TransactionDate = item.TransactionDate;
                                negativeDue.TransactionStatusListId = totalItems == turnAround.Count ? 4 : 3;
                                negativeDue.CreatedBy = this.LoginUserId;
                                negativeDue.CreatedDate = DateTime.Now;
                                Context.NegativeDues.Add(negativeDue);
                            }
                            totalItems++;

                        }

                        Context.SaveChanges();
                        break;

                    case OPEN:
                        masterTrx.StatusId = 4;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        tarApBill.TransactionStatusListId = 4; //paid
                        tarApBill.ModifiedBy = this.LoginUserId;
                        tarApBill.ModifiedDate = DateTime.Now;
                        Context.Entry(tarApBill).State = System.Data.Entity.EntityState.Modified;


                        if (checkbook != null)
                        {
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Deleted;
                        }
                        Context.SaveChanges();
                        break;

                    case PENDING:
                        masterTrx.StatusId = 20;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        tarApBill.TransactionStatusListId = 20; //completed
                        tarApBill.ModifiedBy = this.LoginUserId;
                        tarApBill.ModifiedDate = DateTime.Now;
                        Context.Entry(tarApBill).State = System.Data.Entity.EntityState.Modified;


                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 6; //paid
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }
                        Context.SaveChanges();
                        break;
                }

                return tarApBill.APBillId;
            }



        }

        public int ManualCheck(int apBillId, string TransactionStatus)
        {
            using (jkDatabaseEntities Context = new jkDatabaseEntities())
            {

                var afAapBill = Context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (afAapBill == null) return -1;

                var masterTrx = Context.MasterTrxes.Where(m => m.MasterTrxId == afAapBill.MasterTrxId).FirstOrDefault();
                var checkbook = Context.CheckBooks.Where(m => m.MasterTrxId == afAapBill.MasterTrxId).FirstOrDefault();
                var masterTrxDetail = Context.MasterTrxDetails.Where(d => d.MasterTrxId == afAapBill.MasterTrxId).FirstOrDefault();

                switch (TransactionStatus)
                {
                    case CLOSE:
                        masterTrx.StatusId = 3;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        afAapBill.TransactionStatusListId = 6; //paid

                        afAapBill.ModifiedBy = this.LoginUserId;
                        afAapBill.ModifiedDate = DateTime.Now;
                        Context.Entry(afAapBill).State = System.Data.Entity.EntityState.Modified;

                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 3;
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }

                        if (masterTrxDetail != null)
                        {
                            masterTrxDetail.TransactionStatusListId = 3;
                            masterTrxDetail.ModifiedBy = this.LoginUserId;
                            masterTrxDetail.ModifiedDate = DateTime.Now;
                            Context.Entry(masterTrxDetail).State = System.Data.Entity.EntityState.Modified;
                        }

                        Context.SaveChanges();
                        break;

                    case OPEN:
                        masterTrx.StatusId = 4;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        afAapBill.TransactionStatusListId = 4; //paid

                        afAapBill.ModifiedBy = this.LoginUserId;
                        afAapBill.ModifiedDate = DateTime.Now;
                        Context.Entry(afAapBill).State = System.Data.Entity.EntityState.Modified;

                        if (checkbook != null)
                        {
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Deleted;
                        }

                        Context.SaveChanges();
                        break;

                    case PENDING:
                        masterTrx.StatusId = 20;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        afAapBill.TransactionStatusListId = 20; //completed

                        afAapBill.ModifiedBy = this.LoginUserId;
                        afAapBill.ModifiedDate = DateTime.Now;
                        Context.Entry(afAapBill).State = System.Data.Entity.EntityState.Modified;

                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 6; //paid
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }

                        Context.SaveChanges();
                        break;
                }

                        return afAapBill.APBillId;

            }
        }

        public int APBillCheck(int apBillId, string TransactionStatus)
        {
            using (jkDatabaseEntities Context = new jkDatabaseEntities())
            {

                var ApBill = Context.APBills.Where(o => o.APBillId == apBillId).FirstOrDefault();
                if (ApBill == null) return -1;

                var masterTrx = Context.MasterTrxes.Where(m => m.MasterTrxId == ApBill.MasterTrxId).FirstOrDefault();
                var checkbook = Context.CheckBooks.Where(m => m.MasterTrxId == ApBill.MasterTrxId).FirstOrDefault();

                switch (TransactionStatus)
                {
                    case CLOSE:
                        masterTrx.StatusId = 3;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        ApBill.TransactionStatusListId = 6; //paid

                        ApBill.ModifiedBy = this.LoginUserId;
                        ApBill.ModifiedDate = DateTime.Now;
                        Context.Entry(ApBill).State = System.Data.Entity.EntityState.Modified;

                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 3;
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }

                        Context.SaveChanges();
                        break;

                    case OPEN:
                        masterTrx.StatusId = 4;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        ApBill.TransactionStatusListId = 4; //paid

                        ApBill.ModifiedBy = this.LoginUserId;
                        ApBill.ModifiedDate = DateTime.Now;
                        Context.Entry(ApBill).State = System.Data.Entity.EntityState.Modified;

                        if (checkbook != null)
                        {
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Deleted;
                        }

                        Context.SaveChanges();
                        break;

                    case PENDING:
                        masterTrx.StatusId = 20;
                        masterTrx.ModifiedBy = this.LoginUserId;
                        masterTrx.ModifiedDate = DateTime.Now;
                        Context.Entry(masterTrx).State = System.Data.Entity.EntityState.Modified;

                        ApBill.TransactionStatusListId = 20; //pending

                        ApBill.ModifiedBy = this.LoginUserId;
                        ApBill.ModifiedDate = DateTime.Now;
                        Context.Entry(ApBill).State = System.Data.Entity.EntityState.Modified;

                        if (checkbook != null)
                        {
                            checkbook.TransactionStatusListId = 6;//paid
                            checkbook.ModifiedBy = this.LoginUserId;
                            checkbook.ModifiedDate = DateTime.Now;
                            Context.Entry(checkbook).State = System.Data.Entity.EntityState.Modified;
                        }

                        Context.SaveChanges();
                        break;
                }

                return ApBill.APBillId;

            }
        }



        public List<FPBillingPay> GetFPInvoiceListWithSearchForPayment(string searchtext, bool consolidated, int? regionId, int month, int year)
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<FPBillingPay> lstARInvoiceListView = new List<FPBillingPay>();

                // todo: make new SP to filter invoices properly for payment
                List<portal_spGet_FP_BilingPayList_Result> lstARInvoiceListViewModel = context.portal_spGet_FP_BilingPayList(regionId ?? SelectedRegionId, searchtext, false, consolidated, month, year).ToList();

                foreach (portal_spGet_FP_BilingPayList_Result o in lstARInvoiceListViewModel)
                {
                    lstARInvoiceListView.Add(new FPBillingPay()
                    {
                        BillingPayId = o.BillingPayId,
                        CreatedDate = o.CreatedDate,
                        TransactionNumber = o.TransactionNumber,
                        FranchiseeId = o.FranchiseeId,
                        FranchiseeNo = o.FranchiseeNo,
                        FranchiseeName = o.FranchiseeName,
                        DetailDescription = o.DetailDescription != null ? o.DetailDescription : "",
                        InvoiceNo = o.InvoiceNo,
                        InvoiceId = o.InvoiceId,
                        ContractAmount = o.ContractAmount,
                        TotalFee = o.TotalFee,
                        Total = o.Total,
                        RegionName = o.RegionName,
                    });
                }

                return lstARInvoiceListView;
            }
            // new BillRunSummaryDetailViewModel();
        }

        public int InsertAPBillTransactionForFranchiseeReport(int franchiseeReportId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                FranchiseeReportFinalized report = context.FranchiseeReportFinalizeds.Where(o => o.FranchiseeReportFinalizedId == franchiseeReportId).FirstOrDefault();
                if ((decimal)report.TotalPayment > 0)
                {
                    APBillTransactionViewModel vm = new APBillTransactionViewModel();

                    vm.RegionId = (int)report.RegionId;
                    vm.CreatedBy = this.LoginUserId;
                    vm.CreatedDate = DateTime.Now;

                    APBillViewModel apbvm = new APBillViewModel();

                    apbvm.TypeListId = 2;
                    apbvm.ClassId = (int)report.FranchiseeId;
                    apbvm.CheckBookTransactionTypeListId = 1; // franchisee due
                    apbvm.PayTypeListId = 1;
                    apbvm.IsManual = false;
                    apbvm.BillMonth = (int)report.BillMonth;
                    apbvm.BillYear = (int)report.BillYear;
                    apbvm.PeriodId = (int)report.PeriodId;
                    apbvm.TransactionStatusListId = 4;
                    apbvm.CheckAmount = (decimal)report.TotalPayment;
                    apbvm.FranchiseeReportId = franchiseeReportId;
                    apbvm.ManualCheckId = -1;

                    vm.APBill = apbvm;

                    return InsertAPBillTransaction(vm);
                }
                else
                {
                    return 0;
                }
            }
        }

        public int InsertAPBillTransaction(APBillTransactionViewModel inputdata)
        {
            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel transactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            transactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(16, inputdata.RegionId);

            string nextTrxNumber = null;
            if (transactionNumberConfigViewModel != null)
                nextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(16, inputdata.RegionId, inputdata.CreatedDate);

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                FranchiseeReportFinalized report = null;
                FranchiseeTurnaroundCheck turnaroundCheck = null;
                ManualCheck manualCheck = null;

                if (inputdata.APBill.CheckBookTransactionTypeListId == 1) // Franchisee Due -> FranchiseeReport
                    report = context.FranchiseeReportFinalizeds.Where(o => o.FranchiseeReportFinalizedId == inputdata.APBill.FranchiseeReportId).FirstOrDefault();
                else if (inputdata.APBill.CheckBookTransactionTypeListId == 33) // Turnaround Check
                    turnaroundCheck = context.FranchiseeTurnaroundChecks.Where(o => o.FranchiseeTurnaroundCheckId == inputdata.APBill.FranchiseeTurnaroundCheckId).FirstOrDefault();
                else if (inputdata.APBill.IsManual) // Manual Check -> ManualCheck
                    manualCheck = context.ManualChecks.Where(o => o.ManualCheckId == inputdata.APBill.ManualCheckId).FirstOrDefault();

                if (report == null && manualCheck == null && turnaroundCheck == null) // no data? abort
                    return -1;

                // AP bill mastertrx

                MasterTrx masterTrx = new MasterTrx();
                masterTrx.MasterTrxTypeListId = 16; // AP bill
                masterTrx.ClassId = inputdata.APBill.ClassId;
                masterTrx.TypeListId = inputdata.APBill.TypeListId; // 1 = customer, 2 = franchisee
                masterTrx.RegionId = inputdata.RegionId;
                masterTrx.TrxDate = inputdata.CreatedDate;
                masterTrx.BillMonth = inputdata.APBill.BillMonth;
                masterTrx.BillYear = inputdata.APBill.BillYear;
                masterTrx.StatusId = 4; // open
                masterTrx.CreatedBy = inputdata.CreatedBy;
                masterTrx.CreatedDate = inputdata.CreatedDate;

                context.MasterTrxes.Add(masterTrx);
                context.SaveChanges();

                // AP bill

                APBill apBill = new APBill();
                apBill.RegionId = inputdata.RegionId;
                apBill.CheckTypeListId = inputdata.APBill.CheckBookTransactionTypeListId;
                apBill.MasterTrxId = masterTrx.MasterTrxId;
                apBill.FranchiseeReportId = report?.FranchiseeReportFinalizedId;
                apBill.FranchiseeTurnaroundCheckId = turnaroundCheck?.FranchiseeTurnaroundCheckId;
                apBill.ManualCheckId = manualCheck?.ManualCheckId;
                apBill.TransactionStatusListId = 4; // open
                apBill.TypeListId = inputdata.APBill.TypeListId;
                apBill.ClassId = inputdata.APBill.ClassId;
                apBill.BillMonth = inputdata.APBill.BillMonth;
                apBill.BillYear = inputdata.APBill.BillYear;
                apBill.PeriodId = inputdata.APBill.PeriodId;
                apBill.PayTypeListId = inputdata.APBill.PayTypeListId;
                apBill.CheckAmount = inputdata.APBill.CheckAmount;
                apBill.CreatedBy = inputdata.CreatedBy;
                apBill.CreatedDate = inputdata.CreatedDate;
                apBill.TransactionDate = inputdata.CreatedDate;
                apBill.PayTypeListId = 1; /*1 = Check*/
                apBill.CheckBookTransactionTypeListId = inputdata.APBill.CheckBookTransactionTypeListId;

                context.APBills.Add(apBill);
                context.SaveChanges();

                // master trx detail

                MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
                masterTrxDetail.MasterTrxTypeListId = 16; // AP bill
                masterTrxDetail.HeaderId = apBill.APBillId;
                masterTrxDetail.RegionId = inputdata.RegionId;
                masterTrxDetail.TypelistId = inputdata.APBill.TypeListId;
                masterTrxDetail.ClassId = inputdata.APBill.ClassId;

                masterTrxDetail.AmountTypeListId = 1; // credit
                masterTrxDetail.FeesDetail = false;
                masterTrxDetail.TaxDetail = false;
                masterTrxDetail.Total = report?.TotalPayment ?? turnaroundCheck?.Amount ?? manualCheck.Amount;

                masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                masterTrxDetail.IsDelete = false;

                context.MasterTrxDetails.Add(masterTrxDetail);
                context.SaveChanges();

                if (transactionNumberConfigViewModel != null)
                {
                    transactionNumberConfigViewModel.LastNumber = transactionNumberConfigViewModel.LastNumber + 1;
                    CompanySvc.SaveTransactionNumberConfig(transactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                }

                return apBill.APBillId;
            }
        }

        public FranchiseBillingDetailViewModel GetFranchiseBillingDetails(int billPayId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var feetypes = context.Fees.ToList();
                var servicetypes = context.ServiceTypeLists.Select(s => new { s.ServiceTypeListid, s.name }).ToList();
                var query = from b in context.BillingPays
                            let f = context.Franchisees.FirstOrDefault(e => e.FranchiseeId == b.ClassId.Value)
                            let mtx = context.MasterTrxes.FirstOrDefault(e => e.MasterTrxId == b.MasterTrxId.Value)
                            let mtxDetails = context.MasterTrxDetails.Where(m => m.MasterTrxId == b.MasterTrxId).ToList()
                            let inv = context.Invoices.FirstOrDefault(e => e.InvoiceId == b.InvoiceId)
                            let cust = context.Customers.FirstOrDefault(e => e.CustomerId == inv.ClassId)
                            where b.BillingPayId == billPayId
                            select new FranchiseBillingDetailViewModel
                            {
                                BillingNo = b.TransactionNumber,
                                BillingDate = b.CreatedDate,
                                FranchiseeId = f.FranchiseeId,
                                CustomerId = cust.CustomerId,
                                CustomerNo = cust.CustomerNo,
                                FranchiseeNo = f.FranchiseeNo,
                                FranchiseeName = f.Name,
                                CustomerName = cust.Name,
                                Transaction = new FranchiseBillingDetailTrx { BillingMonthYear = mtx.BillMonth + "/" + mtx.BillYear, Description = inv.InvoiceDescription, InvoiceNo = inv.InvoiceNo, InvoiceID = inv.InvoiceId },
                                TransactionDetails = mtxDetails.Select(t => new FranchiseBillingDetailTrxDetail
                                {
                                    Id = t.MasterTrxDetailId,
                                    ServiceTypeListid = t.ServiceTypeListId,
                                    FeeDetail = t.FeesDetail,
                                    ContractPrice = t.UnitPrice,
                                    Fee = t.TotalFee,
                                    LineNo = t.LineNo,
                                    PayFranchisee = t.Total,
                                    Quantity = t.Quantity,
                                    ServiceDetail = t.DetailDescription
                                }).ToList()
                            };

                var result = query.FirstOrDefault();

                result.FromAddress = context.Addresses.Where(a => a.ClassId == result.FranchiseeId && a.TypeListId == 2)
                                                    .Select(a => new FranchiseBillingDetailAddress
                                                    {
                                                        Phone = context.Phones.Where(p => p.ClassId == result.FranchiseeId && p.TypeListId == 2 && p.ContactTypeListId == 2).Select(c => c.Phone1).FirstOrDefault(),
                                                        Address1 = a.Address1,
                                                        Address2 = a.Address2,
                                                        Address3 = a.City + " " + a.StateName + " " + a.PostalCode
                                                    }).FirstOrDefault();
                result.ToAddress = context.Addresses.Where(a => a.ClassId == result.CustomerId && a.TypeListId == 1)
                                                    .Select(a => new FranchiseBillingDetailAddress
                                                    {
                                                        Address1 = a.Address1,
                                                        Address2 = a.Address2,
                                                        Address3 = a.City + " " + a.StateName + " " + a.PostalCode,
                                                        Phone = context.Phones.Where(p => p.ClassId == result.CustomerId && p.TypeListId == 1 && p.ContactTypeListId == 1).Select(c => c.Phone1).FirstOrDefault()
                                                    }).FirstOrDefault();

                var mtxFeeDetails = context.MasterTrxFeeDetails.Join(result.TransactionDetails.Where(m => m.FeeDetail == true)
                                            .Select(m => m.Id), m => m.MasterTrxDetailId, n => n, (m, n) => m).ToList();
                result.Transaction.ServiceType = servicetypes.Join(result.TransactionDetails, x => x.ServiceTypeListid, y => y.ServiceTypeListid, (x, y) => x.name).FirstOrDefault();
                result.FeeDetails = mtxFeeDetails.Select(m => new FranchiseBillingDetailFeeDetail
                {
                    FeeAmount = m.Amount,
                    FeeType = feetypes.Where(f => f.FeeId == m.FeeId).Select(n => n.Name).FirstOrDefault(),
                    feePercentage = m.FeePercentage
                }).ToList();

                return result;
            }
        }
        public FranchiseBillingDetailViewModel GetFranchiseBillingDetailsWithbillno(string billno)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var feetypes = context.Fees.ToList();
                var servicetypes = context.ServiceTypeLists.Select(s => new { s.ServiceTypeListid, s.name }).ToList();
                var query = from b in context.BillingPays
                            let f = context.Franchisees.FirstOrDefault(e => e.FranchiseeId == b.ClassId.Value)
                            let mtx = context.MasterTrxes.FirstOrDefault(e => e.MasterTrxId == b.MasterTrxId.Value)
                            let mtxDetails = context.MasterTrxDetails.Where(m => m.MasterTrxId == b.MasterTrxId).ToList()
                            let inv = context.Invoices.FirstOrDefault(e => e.InvoiceId == b.InvoiceId)
                            let cust = context.Customers.FirstOrDefault(e => e.CustomerId == inv.ClassId)
                            where b.TransactionNumber == billno.Trim()
                            select new FranchiseBillingDetailViewModel
                            {
                                BillingNo = b.TransactionNumber,
                                BillingDate = b.CreatedDate,
                                FranchiseeId = f.FranchiseeId,
                                CustomerId = cust.CustomerId,
                                CustomerNo = cust.CustomerNo,
                                FranchiseeNo = f.FranchiseeNo,
                                FranchiseeName = f.Name,
                                CustomerName = cust.Name,
                                Transaction = new FranchiseBillingDetailTrx { BillingMonthYear = mtx.BillMonth + "/" + mtx.BillYear, Description = inv.InvoiceDescription, InvoiceNo = inv.InvoiceNo, InvoiceID = inv.InvoiceId },
                                TransactionDetails = mtxDetails.Select(t => new FranchiseBillingDetailTrxDetail
                                {
                                    Id = t.MasterTrxDetailId,
                                    ServiceTypeListid = t.ServiceTypeListId,
                                    FeeDetail = t.FeesDetail,
                                    ContractPrice = t.UnitPrice,
                                    Fee = t.TotalFee,
                                    LineNo = t.LineNo,
                                    PayFranchisee = t.Total,
                                    Quantity = t.Quantity,
                                    ServiceDetail = t.DetailDescription
                                }).ToList()
                            };

                var result = query.FirstOrDefault();

                result.FromAddress = context.Addresses.Where(a => a.ClassId == result.FranchiseeId && a.TypeListId == 2)
                                                    .Select(a => new FranchiseBillingDetailAddress
                                                    {
                                                        Phone = context.Phones.Where(p => p.ClassId == result.FranchiseeId && p.TypeListId == 2 && p.ContactTypeListId == 2).Select(c => c.Phone1).FirstOrDefault(),
                                                        Address1 = a.Address1,
                                                        Address2 = a.Address2,
                                                        Address3 = a.City + " " + a.StateName + " " + a.PostalCode
                                                    }).FirstOrDefault();
                result.ToAddress = context.Addresses.Where(a => a.ClassId == result.CustomerId && a.TypeListId == 1)
                                                    .Select(a => new FranchiseBillingDetailAddress
                                                    {
                                                        Address1 = a.Address1,
                                                        Address2 = a.Address2,
                                                        Address3 = a.City + " " + a.StateName + " " + a.PostalCode,
                                                        Phone = context.Phones.Where(p => p.ClassId == result.CustomerId && p.TypeListId == 1 && p.ContactTypeListId == 1).Select(c => c.Phone1).FirstOrDefault()
                                                    }).FirstOrDefault();

                var mtxFeeDetails = context.MasterTrxFeeDetails.Join(result.TransactionDetails.Where(m => m.FeeDetail == true)
                                            .Select(m => m.Id), m => m.MasterTrxDetailId, n => n, (m, n) => m).ToList();
                result.Transaction.ServiceType = servicetypes.Join(result.TransactionDetails, x => x.ServiceTypeListid, y => y.ServiceTypeListid, (x, y) => x.name).FirstOrDefault();
                result.FeeDetails = mtxFeeDetails.Select(m => new FranchiseBillingDetailFeeDetail
                {
                    FeeAmount = m.Amount,
                    FeeType = feetypes.Where(f => f.FeeId == m.FeeId).Select(n => n.Name).FirstOrDefault()
                }).ToList();

                return result;
            }
        }

        public IEnumerable<portal_spGet_F_ChargebackListForFranchiseeViewModel> GetFranchiseeWiseChargebackSummaryOrDetailsResult(string regionIds, bool isSummaryView, DateTime? spnStartDate, DateTime? spnEndDate, int month, int year, string ProcCBTrx, int PeriodId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = string.Empty;
                if (isSummaryView)
                {
                    query = "EXEC portal_spGet_F_FranchiseeChargebackSummaryAndDetailsReport @RegionId,@StartDate,@EndDate,@SummaryReport,@BillMonth,@BillYear,@CBTrx,@PeriodId";
                }
                else
                {
                    query = "EXEC portal_spGet_F_FranchiseeChargebackSummaryAndDetailsReport @RegionId,@StartDate,@EndDate,@SummaryReport,@BillMonth,@BillYear,@CBTrx,@PeriodId";
                }

                var region = "0";
                if (String.IsNullOrEmpty(regionIds) || regionIds == null || regionIds == "null")
                {
                    regionIds = region;
                }

                var result = conn.Query<portal_spGet_F_ChargebackListForFranchiseeViewModel>(query.ToString(), new
                {
                    RegionId = regionIds,
                    StartDate = spnStartDate,
                    EndDate = spnEndDate,
                    SummaryReport = isSummaryView,
                    BillMonth = month,
                    BillYear = year,
                    CBTrx = ProcCBTrx,
                    PeriodId = PeriodId
                });

                return result;
            }
        }


        public List<ChargebackHistoryReportViewModel> GetChargebackHistoryReportList(string regionId, DateTime? startDate, DateTime? endDate, int month = 0, int year = 0)
        {
            List<ChargebackHistoryReportViewModel> lstChargebackHistoryReport = new List<ChargebackHistoryReportViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                parmas.Add("@LStartDate", startDate);
                parmas.Add("@LEndDate", endDate);
                parmas.Add("@BillMonth", month);
                parmas.Add("@BillYear", year);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AP_ChargebackHistoryReport", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstChargebackHistoryReport = multipleresult.Read<ChargebackHistoryReportViewModel>().ToList();
                    }
                }
                return lstChargebackHistoryReport;
            }

        }

        public List<ChargebackHistoryReportSummaryViewModel> GetChargebackHistorySummaryReportList(string regionId, DateTime? startDate, DateTime? endDate, int month = 0, int year = 0)
        {
            List<ChargebackHistoryReportSummaryViewModel> lstChargebackHistorySummaryReport = new List<ChargebackHistoryReportSummaryViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                parmas.Add("@LStartDate", startDate);
                parmas.Add("@LEndDate", endDate);
                parmas.Add("@BillMonth", month);
                parmas.Add("@BillYear", year);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AP_ChargebackHistorySummaryReport", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstChargebackHistorySummaryReport = multipleresult.Read<ChargebackHistoryReportSummaryViewModel>().ToList();
                    }
                }
                return lstChargebackHistorySummaryReport;
            }

        }
        #endregion


        public bool InsertFranchiseeManualTrasactionFromWriteCheck(DateTime TransactionDate, int ClassId, int TypeListId, decimal Amount, string FMDescription, int checkTypeListId)
        {
            using (var context = new jkDatabaseEntities())
            {

                CheckBookTransactionTypeList oCBT = context.CheckBookTransactionTypeLists.SingleOrDefault(o => o.CheckBookTransactionTypeListId == checkTypeListId);

                if (oCBT != null)
                {
                    int? _ServiceTypeListId = oCBT.ServiceTypeListId != null ? oCBT.ServiceTypeListId : 0;
                    ServiceTypeList oServiceTypeList = context.ServiceTypeLists.SingleOrDefault(s => s.ServiceTypeListid == _ServiceTypeListId && s.CreateFranchiseeManualTransaction == true);
                    if (oServiceTypeList != null)
                    {
                        int _BillMonth = TransactionDate.Month;
                        int _BillYear = TransactionDate.Year;
                        var PR = context.Periods.SingleOrDefault(o => o.BillMonth == _BillMonth && o.BillYear == _BillYear);

                        int? RegionId = 0;
                        if (TypeListId == 1)
                            RegionId = context.Customers.SingleOrDefault(o => o.CustomerId == ClassId).RegionId;
                        else if (TypeListId == 2)
                            RegionId = context.Franchisees.SingleOrDefault(o => o.FranchiseeId == ClassId).RegionId;
                        if (TypeListId == 13)
                            RegionId = context.Companies.SingleOrDefault(o => o.CompanyId == ClassId).RegionId;

                        MasterTrx oMasterTrx = new MasterTrx();
                        //oMasterTrx.AccountTypeListId;
                        oMasterTrx.BillMonth = _BillMonth;
                        oMasterTrx.BillYear = _BillYear;
                        oMasterTrx.ClassId = ClassId;
                        oMasterTrx.CreatedBy = LoginUserId;
                        oMasterTrx.CreatedDate = DateTime.Now;
                        oMasterTrx.MasterTrxTypeListId = oCBT.MasterTrxTypeListId;
                        oMasterTrx.PeriodId = PR.PeriodId;
                        oMasterTrx.RegionId = RegionId;
                        oMasterTrx.StatusId = 3;
                        oMasterTrx.TrxDate = TransactionDate;
                        oMasterTrx.TypeListId = TypeListId;
                        context.MasterTrxes.Add(oMasterTrx);
                        context.SaveChanges();

                        FranchiseeManualTransaction oFranchiseeManualTransaction = new FranchiseeManualTransaction();

                        oFranchiseeManualTransaction.ClassId = ClassId;
                        oFranchiseeManualTransaction.TypeListId = TypeListId;
                        oFranchiseeManualTransaction.FranchiseeManualTransactionDescription = FMDescription;

                        //oFranchiseeManualTransaction.FranchsieeTrxTypeListId;
                        oFranchiseeManualTransaction.IsCredit = false;
                        oFranchiseeManualTransaction.IsDelete = false;
                        oFranchiseeManualTransaction.MasterTrxId = oMasterTrx.MasterTrxId;
                        oFranchiseeManualTransaction.MasterTrxTypeListId = oCBT.MasterTrxTypeListId;
                        oFranchiseeManualTransaction.PeriodId = PR.PeriodId;
                        oFranchiseeManualTransaction.RegionId = RegionId;
                        oFranchiseeManualTransaction.ServiceTypeListId = oServiceTypeList.ServiceTypeListid;
                        oFranchiseeManualTransaction.TransactionDate = TransactionDate;
                        //oFranchiseeManualTransaction.TransactionNumber;
                        oFranchiseeManualTransaction.TransactionStatusListId = 3;
                        oFranchiseeManualTransaction.CreatedBy = LoginUserId;
                        oFranchiseeManualTransaction.ModifiedBy = LoginUserId;
                        oFranchiseeManualTransaction.ModifiedDate = DateTime.Now;

                        context.FranchiseeManualTransactions.Add(oFranchiseeManualTransaction);
                        context.SaveChanges();

                        oMasterTrx.HeaderId = oFranchiseeManualTransaction.FranchiseeManualTransactionId;
                        context.SaveChanges();


                        MasterTrxDetail oMasterTrxDetail = new MasterTrxDetail();
                        oMasterTrxDetail.AccountRebate = 0;
                        oMasterTrxDetail.AmountTypeListId = 2;
                        oMasterTrxDetail.BPPAdmin = 0;
                        oMasterTrxDetail.ClassId = ClassId;
                        oMasterTrxDetail.ClientSupplies = false;
                        oMasterTrxDetail.Commission = false;
                        oMasterTrxDetail.CommissionTotal = 0;
                        //oMasterTrxDetail.CPIPercentage;
                        oMasterTrxDetail.CreatedBy = LoginUserId;
                        oMasterTrxDetail.CreatedDate = DateTime.Now;
                        oMasterTrxDetail.DetailDescription = FMDescription;
                        oMasterTrxDetail.ExtendedPrice = Amount;
                        oMasterTrxDetail.ExtraWork = 0;
                        oMasterTrxDetail.FeesDetail = false;
                        oMasterTrxDetail.FRDeduction = true;
                        oMasterTrxDetail.FRRevenues = false;
                        oMasterTrxDetail.HeaderId = oFranchiseeManualTransaction.FranchiseeManualTransactionId;
                        oMasterTrxDetail.IsDelete = false;
                        oMasterTrxDetail.LineNo = 1;
                        oMasterTrxDetail.MasterTrxId = oMasterTrx.MasterTrxId;
                        //oMasterTrxDetail.MasterTrxStatusId;
                        oMasterTrxDetail.MasterTrxTypeListId = oCBT.MasterTrxTypeListId;
                        oMasterTrxDetail.PeriodId = PR.PeriodId;
                        oMasterTrxDetail.Quantity = 1;
                        oMasterTrxDetail.RegionId = RegionId;
                        //oMasterTrxDetail.ReSell;
                        oMasterTrxDetail.ServiceTypeListId = oServiceTypeList.ServiceTypeListid;
                        oMasterTrxDetail.SourceId = -1;
                        //oMasterTrxDetail.SourceTypeListId;
                        oMasterTrxDetail.TaxDetail = false;
                        oMasterTrxDetail.Total = Amount;
                        oMasterTrxDetail.TotalFee = 0;
                        oMasterTrxDetail.TotalTax = 0;
                        oMasterTrxDetail.Transactiondate = TransactionDate;
                        //oMasterTrxDetail.TurnaroundPaymentStatusListId;
                        oMasterTrxDetail.TypelistId = TypeListId;
                        oMasterTrxDetail.UnitPrice = Amount;

                        context.MasterTrxDetails.Add(oMasterTrxDetail);
                        context.SaveChanges();
                    }
                }

                return true;
            }
        }

        public bool InsertFranchiseeManualTrasactionFromWriteCheck(List<int> cbList)
        {
            using (var context = new jkDatabaseEntities())
            {

                foreach(int _cbId  in cbList)
                {

                    CheckBook checkBook = context.CheckBooks.FirstOrDefault(o => o.CheckBookId == _cbId);

                    DateTime TransactionDate = (DateTime)checkBook.TransactionDate;
                    int ClassId = (int)checkBook.ClassId;
                    int TypeListId = (int)checkBook.TypeListId;
                    decimal Amount = (decimal)checkBook.Amount;
                    string FMDescription = (checkBook.Memo!=null? checkBook.Memo : (checkBook.Notes!=null? checkBook.Notes:""));



                    CheckBookTransactionTypeList oCBT = context.CheckBookTransactionTypeLists.SingleOrDefault(o => o.CheckBookTransactionTypeListId == checkBook.CheckBookTransactionTypeListId);

                    if (oCBT != null)
                    {
                        int? _ServiceTypeListId = oCBT.ServiceTypeListId;// != null ? oCBT.ServiceTypeListId : 0;
                        //ServiceTypeList oServiceTypeList = context.ServiceTypeLists.SingleOrDefault(s => s.ServiceTypeListid == _ServiceTypeListId && s.CreateFranchiseeManualTransaction == true);
                        //if (_ServiceTypeListId>0)
                        //{
                            int _BillMonth = TransactionDate.Month;
                            int _BillYear = TransactionDate.Year;
                            var PR = context.Periods.SingleOrDefault(o => o.BillMonth == _BillMonth && o.BillYear == _BillYear);

                            int? RegionId = 0;
                            if (TypeListId == 1)
                                RegionId = context.Customers.SingleOrDefault(o => o.CustomerId == ClassId).RegionId;
                            else if (TypeListId == 2)
                                RegionId = context.Franchisees.SingleOrDefault(o => o.FranchiseeId == ClassId).RegionId;
                            if (TypeListId == 13)
                                RegionId = context.Companies.SingleOrDefault(o => o.CompanyId == ClassId).RegionId;


                            RegionId = (RegionId != null ? RegionId : checkBook.RegionId);
                            //MasterTrx oMasterTrx = new MasterTrx();
                            ////oMasterTrx.AccountTypeListId;
                            //oMasterTrx.BillMonth = _BillMonth;
                            //oMasterTrx.BillYear = _BillYear;
                            //oMasterTrx.ClassId = ClassId;
                            //oMasterTrx.CreatedBy = LoginUserId;
                            //oMasterTrx.CreatedDate = DateTime.Now;
                            //oMasterTrx.MasterTrxTypeListId = oCBT.MasterTrxTypeListId;
                            //oMasterTrx.PeriodId = PR.PeriodId;
                            //oMasterTrx.RegionId = RegionId;
                            //oMasterTrx.StatusId = 3;
                            //oMasterTrx.TrxDate = TransactionDate;
                            //oMasterTrx.TypeListId = TypeListId;
                            //context.MasterTrxes.Add(oMasterTrx);
                            //context.SaveChanges();

                            FranchiseeManualTransaction oFranchiseeManualTransaction = new FranchiseeManualTransaction();

                            oFranchiseeManualTransaction.ClassId = ClassId;
                            oFranchiseeManualTransaction.TypeListId = TypeListId;
                            oFranchiseeManualTransaction.FranchiseeManualTransactionDescription = FMDescription;

                            //oFranchiseeManualTransaction.FranchsieeTrxTypeListId;
                            oFranchiseeManualTransaction.IsCredit = false;
                            oFranchiseeManualTransaction.IsDelete = false;
                            oFranchiseeManualTransaction.MasterTrxId = checkBook.MasterTrxId;
                            oFranchiseeManualTransaction.MasterTrxTypeListId = oCBT.MasterTrxTypeListId;
                            oFranchiseeManualTransaction.PeriodId = PR.PeriodId;
                            oFranchiseeManualTransaction.RegionId = RegionId;
                            oFranchiseeManualTransaction.ServiceTypeListId = oCBT.ServiceTypeListId;
                            oFranchiseeManualTransaction.TransactionDate = TransactionDate;
                            //oFranchiseeManualTransaction.TransactionNumber;
                            oFranchiseeManualTransaction.TransactionStatusListId = 3;
                            oFranchiseeManualTransaction.CreatedBy = LoginUserId;
                            oFranchiseeManualTransaction.ModifiedBy = LoginUserId;
                            oFranchiseeManualTransaction.ModifiedDate = DateTime.Now;

                            context.FranchiseeManualTransactions.Add(oFranchiseeManualTransaction);
                            context.SaveChanges();

                            //oMasterTrx.HeaderId = oFranchiseeManualTransaction.FranchiseeManualTransactionId;
                            //context.SaveChanges();


                            MasterTrxDetail oMasterTrxDetail = new MasterTrxDetail();
                            oMasterTrxDetail.AccountRebate = 0;
                            oMasterTrxDetail.AmountTypeListId = 2;
                            oMasterTrxDetail.BPPAdmin = 0;
                            oMasterTrxDetail.ClassId = ClassId;
                            oMasterTrxDetail.ClientSupplies = false;
                            oMasterTrxDetail.Commission = false;
                            oMasterTrxDetail.CommissionTotal = 0;
                            //oMasterTrxDetail.CPIPercentage;
                            oMasterTrxDetail.CreatedBy = LoginUserId;
                            oMasterTrxDetail.CreatedDate = DateTime.Now;
                            oMasterTrxDetail.DetailDescription = FMDescription;
                            oMasterTrxDetail.ExtendedPrice = Amount;
                            oMasterTrxDetail.ExtraWork = 0;
                            oMasterTrxDetail.FeesDetail = false;
                            oMasterTrxDetail.FRDeduction = true;
                            oMasterTrxDetail.FRRevenues = false;
                            oMasterTrxDetail.HeaderId = oFranchiseeManualTransaction.FranchiseeManualTransactionId;
                            oMasterTrxDetail.IsDelete = false;
                            oMasterTrxDetail.LineNo = 1;
                            oMasterTrxDetail.MasterTrxId = checkBook.MasterTrxId;
                            //oMasterTrxDetail.MasterTrxStatusId;
                            oMasterTrxDetail.MasterTrxTypeListId = oCBT.MasterTrxTypeListId;
                            oMasterTrxDetail.PeriodId = PR.PeriodId;
                            oMasterTrxDetail.Quantity = 1;
                            oMasterTrxDetail.RegionId = RegionId;
                            //oMasterTrxDetail.ReSell;
                            oMasterTrxDetail.ServiceTypeListId = oCBT.ServiceTypeListId;
                            oMasterTrxDetail.SourceId = -1;
                            //oMasterTrxDetail.SourceTypeListId;
                            oMasterTrxDetail.TaxDetail = false;
                            oMasterTrxDetail.Total = Amount;
                            oMasterTrxDetail.TotalFee = 0;
                            oMasterTrxDetail.TotalTax = 0;
                            oMasterTrxDetail.Transactiondate = TransactionDate;
                            //oMasterTrxDetail.TurnaroundPaymentStatusListId;
                            oMasterTrxDetail.TypelistId = TypeListId;
                            oMasterTrxDetail.UnitPrice = Amount;

                            context.MasterTrxDetails.Add(oMasterTrxDetail);
                            context.SaveChanges();
                        //}
                    }





                }
                

                

                return true;
            }
        }

        public List<NegativeDueViewModel> GetNagativeDue(int franchiseeStatus, string regionIds = "")
        {

            NegativeDueViewModel oCommon = new NegativeDueViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionIds);
                parmas.Add("@franchiseeStatus", franchiseeStatus);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AP_NegativeDue", parmas, commandType: CommandType.StoredProcedure))
                {
                    var lst = multipleresult.Read<NegativeDueViewModel>().ToList();
                    return lst;
                }
            }
        }

        public bool AddNegativeDue(decimal ParAmt, int NdId)
        {
            using (var context = new jkDatabaseEntities())
            {
                var oCBT = context.NegativeDues.SingleOrDefault(o => o.NegativeDueId == NdId);
                if (oCBT != null)
                {
                    oCBT.TransactionStatusListId = 3;
                    context.SaveChanges();

                    NegativeDue oNegativeDue = new NegativeDue();
                    oNegativeDue.ClassId = oCBT.ClassId;
                    oNegativeDue.TypeListId = oCBT.TypeListId;
                    oNegativeDue.CustomerId = oCBT.CustomerId;
                    oNegativeDue.RegionId = oCBT.RegionId;
                    oNegativeDue.PeriodId = Convert.ToInt32(ClaimView.GetCLAIM_PERIOD_ID()); 
                    oNegativeDue.ApplySourceId = oCBT.ApplySourceId;
                    oNegativeDue.Rollover = false;
                    oNegativeDue.Amount = (decimal)(0.00);
                    oNegativeDue.AppliedAmount = ParAmt;
                    oNegativeDue.Balance = (decimal)(oCBT.Balance - ParAmt);
                    oNegativeDue.TransactionDate = oCBT.TransactionDate;
                    oNegativeDue.TransactionStatusListId = 1;
                    oNegativeDue.TransactionNumber = oCBT.TransactionNumber;
                    oNegativeDue.CreatedBy = oCBT.CreatedBy;
                    oNegativeDue.CreatedDate = DateTime.UtcNow;
                    oNegativeDue.ModifiedBy = oCBT.ModifiedBy;
                    oNegativeDue.ModifiedDate = oCBT.ModifiedDate;
                    oNegativeDue.ImpId = oCBT.ImpId;
                    context.NegativeDues.Add(oNegativeDue);
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool AddNegativeDueRoll(int NdId)
        {
            using (var context = new jkDatabaseEntities())
            {
                NegativeDue oCBT = context.NegativeDues.SingleOrDefault(o => o.NegativeDueId == NdId);
                if (oCBT != null)
                {
                    oCBT.TransactionStatusListId = 3;
                    context.SaveChanges();

                    NegativeDue oNegativeDue = new NegativeDue();
                    oNegativeDue.ClassId = oCBT.ClassId;
                    oNegativeDue.TypeListId = oCBT.TypeListId;
                    oNegativeDue.CustomerId = oCBT.CustomerId;
                    oNegativeDue.RegionId = oCBT.RegionId;
                    oNegativeDue.PeriodId = Convert.ToInt32(ClaimView.GetCLAIM_PERIOD_ID());
                    oNegativeDue.ApplySourceId = oCBT.ApplySourceId;
                    oNegativeDue.Rollover = true;
                    oNegativeDue.Amount = oCBT.Amount;
                    oNegativeDue.AppliedAmount = oCBT.AppliedAmount;
                    oNegativeDue.Balance = oCBT.Balance;
                    oNegativeDue.TransactionDate = oCBT.TransactionDate;
                    oNegativeDue.TransactionStatusListId = 1;
                    oNegativeDue.TransactionNumber = oCBT.TransactionNumber;
                    oNegativeDue.CreatedBy = oCBT.CreatedBy;
                    oNegativeDue.CreatedDate = DateTime.UtcNow;
                    oNegativeDue.ModifiedBy = oCBT.ModifiedBy;
                    oNegativeDue.ModifiedDate = oCBT.ModifiedDate;
                    oNegativeDue.ImpId = oCBT.ImpId;
                    context.NegativeDues.Add(oNegativeDue);
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool AddNegativeDueFullPayment(int NdId)
        {
            using (var context = new jkDatabaseEntities())
            {
                var oCBT = context.NegativeDues.SingleOrDefault(o => o.NegativeDueId == NdId);
                if (oCBT != null)
                {
                    oCBT.TransactionStatusListId = 3;
                    context.SaveChanges();

                    NegativeDue oNegativeDue = new NegativeDue();
                    oNegativeDue.ClassId = oCBT.ClassId;
                    oNegativeDue.TypeListId = oCBT.TypeListId;
                    oNegativeDue.CustomerId = oCBT.CustomerId;
                    oNegativeDue.RegionId = oCBT.RegionId;
                    oNegativeDue.PeriodId = Convert.ToInt32(ClaimView.GetCLAIM_PERIOD_ID());
                    oNegativeDue.ApplySourceId = oCBT.ApplySourceId;
                    oNegativeDue.Rollover = false;
                    oNegativeDue.Amount = (decimal)(0.00);
                    oNegativeDue.AppliedAmount = oCBT.Balance;
                    oNegativeDue.Balance = (decimal)(0.00);
                    oNegativeDue.TransactionDate = oCBT.TransactionDate;
                    oNegativeDue.TransactionStatusListId = 1;
                    oNegativeDue.TransactionNumber = oCBT.TransactionNumber;
                    oNegativeDue.CreatedBy = oCBT.CreatedBy;
                    oNegativeDue.CreatedDate = DateTime.UtcNow;
                    oNegativeDue.ModifiedBy = oCBT.ModifiedBy;
                    oNegativeDue.ModifiedDate = oCBT.ModifiedDate;
                    oNegativeDue.ImpId = oCBT.ImpId;
                    context.NegativeDues.Add(oNegativeDue);
                    context.SaveChanges();
                }
            }
            return true;
        }

        /* 
         * UpdateNegativeDue
         * Used by the AccountsPayable/NegativeDue page to process user selected records to process negative dues 
         * German Sosa 10/24/2018 
         */
        public bool UpdateNegativeDue(int UserId, int SelectedPeriodId, int NegativeDueId, decimal RolloverAmount, decimal BalanceAfterRollover)
        {
            using (var context = new jkDatabaseEntities())
            {
                var oND = context.NegativeDues.SingleOrDefault(o => o.NegativeDueId == NegativeDueId);
                if (oND != null)
                {
                    oND.TransactionStatusListId = 3;
                    context.SaveChanges();

                    NegativeDue oNegativeDue = new NegativeDue();
                    oNegativeDue.ClassId = oND.ClassId;
                    oNegativeDue.TypeListId = oND.TypeListId;
                    oNegativeDue.CustomerId = oND.CustomerId;
                    oNegativeDue.RegionId = oND.RegionId;
                    oNegativeDue.PeriodId = SelectedPeriodId;
                    oNegativeDue.ApplySourceId = RolloverAmount>0m?2:0;
                    oNegativeDue.Rollover = RolloverAmount>0m?true:false; /* Determines if the transaction is rollover? - German Sosa */
                    oNegativeDue.Amount = 0;
                    oNegativeDue.AppliedAmount = RolloverAmount;
                    oNegativeDue.Balance = BalanceAfterRollover;
                    oNegativeDue.TransactionDate = oND.TransactionDate;
                    oNegativeDue.TransactionStatusListId = RolloverAmount > 0m?1:4;
                    oNegativeDue.TransactionNumber = oND.TransactionNumber;
                    oNegativeDue.CreatedBy = UserId;
                    oNegativeDue.CreatedDate = DateTime.UtcNow;
                    oNegativeDue.ModifiedBy = oND.ModifiedBy;
                    oNegativeDue.ModifiedDate = oND.ModifiedDate;
                    oNegativeDue.ImpId = null;
                    context.NegativeDues.Add(oNegativeDue);
                    context.SaveChanges();
                    /* We need to add recordset save exception to verify if it was saved
                     * and based on that return true or false and issue saving*/
                }
            }
            return true;
        }

        public bool UpdatePeriodClosed(int PeriodClosedId)
        {
            using(var context = new jkDatabaseEntities())
            {
                PeriodClosed periodClosed = context.PeriodCloseds.Where(p => p.PeriodClosedId == PeriodClosedId).FirstOrDefault();
                periodClosed.NegativeDueFinalized = true;
                context.Entry(periodClosed).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }

        }


        public Period GetPeriod(int PeriodId)
        {
            using (var context = new jkDatabaseEntities())
            {
                Period period = new Period();
                period = context.Periods.Where(p => p.PeriodId == PeriodId).FirstOrDefault();
                return period;
            }

        }


        public PeriodClosed GetPeriodClosed(int PeriodClosedId)
        {
            using (var context = new jkDatabaseEntities())
            {
                PeriodClosed periodclosed = new PeriodClosed();
                periodclosed = context.PeriodCloseds.Where(p => p.PeriodClosedId == PeriodClosedId).FirstOrDefault();
                return periodclosed;
            }

        }

        public PeriodClosed GetPreviousPeriod(int PeriodClosedId, int RegionId)
        {
            using (var context = new jkDatabaseEntities())
            {
                PeriodClosed periodclosed = new PeriodClosed();
                periodclosed = context.PeriodCloseds.Where(p => p.PeriodClosedId == PeriodClosedId).FirstOrDefault();

                PeriodClosed previousPeriod = new PeriodClosed();
                previousPeriod = context.PeriodCloseds.Where(v => v.PeriodId == (periodclosed.PeriodId - 1) && v.RegionId == RegionId).FirstOrDefault();
                return previousPeriod;
            }

        }

    }

    public class FranchiseeReportDetailsViewModel
    {
        public FranchiseeReport Report { get; set; }
        public List<portal_spGet_AP_FranchiseeReportDetails_Result> DetailsByTransaction { get; set; }
        public List<FranchiseeReportDetailsByServiceViewModel> DetailsByService { get; set; }
        public List<FranchiseeReportDeductionViewModel> Deductions { get; set; }

        //public IEnumerable<FranchiseeReportDeductionViewModel> RegularDeductions { get { return this.Deductions.Where(o => !o.IsSpecialDeduction); } }
        // public IEnumerable<FranchiseeReportDeductionViewModel> SpecialDeductions { get { return this.Deductions.Where(o => o.IsSpecialDeduction); } }

        public string MonthYearDisplay { get { return string.Format("{0} {1}", this.Report.BillMonth.HasValue ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)this.Report.BillMonth).ToUpper() : "", this.Report.BillYear); } }

        public string NumberDisplay(decimal? value, int? amountTypeListId = null, bool showDollarSign = false)
        {
            if (value == null)
                return "";

            if (value == 0)
                amountTypeListId = 1;
            if (amountTypeListId == null && value != null)
                amountTypeListId = ((decimal)value >= 0) ? 1 : 2;

            string leftParen = (amountTypeListId == 2) ? "(" : "";
            string rightParen = (amountTypeListId == 2) ? ")" : "";
            string dollarSign = showDollarSign ? "$" : "";

            return string.Format(CultureInfo.InvariantCulture, "{1}{3}{0:N2}{2}", Math.Abs((decimal)value), leftParen, rightParen, dollarSign);
        }



        public string PaymentNumberDisplay(portal_spGet_AP_FranchiseeReportDeductions_Result deduction)
        {
            int currentPaymentInt = -1;

            if (int.TryParse(deduction.CurrentPaymentNo, out currentPaymentInt))
            {
                if (currentPaymentInt == deduction.TotalPaymentNo)
                    return "Final payment";
                return string.Format("{0} of {1}", currentPaymentInt, deduction.TotalPaymentNo);
            }

            return deduction.CurrentPaymentNo;
        }
    }


    public class TurnAroundDetailsViewModel
    {
        public TurnAround TurnAround { get; set; }
        public List<portal_spGet_AP_TurnAroundCheckDetails_Result> TurnAroundCheckDetails { get; set; }
        public decimal Total { get { return TurnAroundCheckDetails.Sum(o => (decimal)o.TurnAroundAmount); } }

        public decimal TotalNegativeDueAmount { get { return TurnAroundCheckDetails.Sum(o => (decimal)o.NegativeDueAmount); } }

        public decimal TotalPaymentAmount { get { return TurnAroundCheckDetails.Sum(o => (decimal)o.PaymentAmount); } }

        public string NumberDisplay(decimal? value, int? amountTypeListId = null, bool showDollarSign = false)
        {
            if (value == null)
                return "";

            if (value == 0)
                amountTypeListId = 1;
            if (amountTypeListId == null && value != null)
                amountTypeListId = ((decimal)value >= 0) ? 1 : 2;

            string leftParen = (amountTypeListId == 2) ? "(" : "";
            string rightParen = (amountTypeListId == 2) ? ")" : "";
            string dollarSign = showDollarSign ? "$" : "";

            return string.Format(CultureInfo.InvariantCulture, "{1}{3}{0:N2}{2}", Math.Abs((decimal)value), leftParen, rightParen, dollarSign);
        }
    }


    public class FranchiseeReportDetailsByServiceViewModel
    {
        public string ServiceType { get; set; }
        public int ServiceTypeListId { get; set; }
        public int DisplayOrder { get; set; }
        public int CreatedBy { get; set; }
        public string ServiceTypeGroupListName { get; set; }
        public string BillMonthYears { get; set; }

        public List<portal_spGet_AP_FranchiseeReportDetailsByCustomerAndServiceType_Result> Details { get; set; }
    }

    public class FranchiseeReportDeductionViewModel
    {
        public string ServiceType { get; set; }
        public int ServiceTypeListId { get; set; }
        public string BillMonthYears { get; set; }

        public int DisplayOrder { get; set; }
        public bool DisplaySubReport { get; set; }
        public Nullable<bool> IsSpecialDeduction { get; set; }
        public bool? DisplayLeaseMessage { get; internal set; }

        public int ReSell { get; set; }
        public int FeeId { get; set; }
        //public int UseMinumumRoyaltyAmount { get; set; }
        //public decimal MinumumRoyaltyAmount { get; set; }
        public int ServiceTypeGroupListId { get; set; }
        public string GroupName { get; set; }
        public List<portal_spGet_AP_FranchiseeReportDeductions_Result> Deductions { get; set; }

        public decimal Subtotal { get { return Deductions.Sum(o => (decimal)o.Subtotal); } }
        public decimal Tax { get { return Deductions.Sum(o => (decimal)o.Tax); } }
        public decimal Total { get { return Deductions.Sum(o => (decimal)o.Total); } }

        //public decimal Subtotal { get; }
        //public decimal Tax { get; }
        //public decimal Total { get; }

        //public decimal Subtotal { get { return Deductions.Sum(o => (decimal)o.Subtotal * ((o.AmountTypeListId == 1) ? -1 : 1)); } }
        //public decimal Tax { get { return Deductions.Sum(o => (decimal)o.Tax * ((o.AmountTypeListId == 1) ? -1 : 1)); } }
        //public decimal Total { get { return Deductions.Sum(o => (decimal)o.Total * ((o.AmountTypeListId == 1) ? -1 : 1)); } }
    }



    public class FranchiseeReportFinalizedDetailsViewModel
    {
        public FranchiseeReportFinalized Report { get; set; }
        public List<portal_spGet_AP_FranchiseeReportFinalizedDetails_Result> DetailsByTransaction { get; set; }
        public List<FranchiseeReportFinalizedDetailsByServiceViewModel> DetailsByService { get; set; }
        public List<FranchiseeReportFinalizedDeductionViewModel> Deductions { get; set; }

        //public IEnumerable<FranchiseeReportDeductionViewModel> RegularDeductions { get { return this.Deductions.Where(o => !o.IsSpecialDeduction); } }
        // public IEnumerable<FranchiseeReportDeductionViewModel> SpecialDeductions { get { return this.Deductions.Where(o => o.IsSpecialDeduction); } }

        public string MonthYearDisplay { get { return string.Format("{0} {1}", this.Report.BillMonth.HasValue ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((int)this.Report.BillMonth).ToUpper() : "", this.Report.BillYear); } }

        public string NumberDisplay(decimal? value, int? amountTypeListId = null, bool showDollarSign = false)
        {
            if (value == null)
                return "";

            if (value == 0)
                amountTypeListId = 1;
            if (amountTypeListId == null && value != null)
                amountTypeListId = ((decimal)value >= 0) ? 1 : 2;

            string leftParen = (amountTypeListId == 2) ? "(" : "";
            string rightParen = (amountTypeListId == 2) ? ")" : "";
            string dollarSign = showDollarSign ? "$" : "";

            return string.Format(CultureInfo.InvariantCulture, "{1}{3}{0:N2}{2}", Math.Abs((decimal)value), leftParen, rightParen, dollarSign);
        }





        public string PaymentNumberDisplay(portal_spGet_AP_FranchiseeReportFinalizedDeductions_Result deduction)
        {
            int currentPaymentInt = -1;

            if (int.TryParse(deduction.CurrentPaymentNo, out currentPaymentInt))
            {
                if (currentPaymentInt == deduction.TotalPaymentNo)
                    return "Final payment";
                return string.Format("{0} of {1}", currentPaymentInt, deduction.TotalPaymentNo);
            }

            return deduction.CurrentPaymentNo;
        }
    }



    public class FranchiseeReportFinalizedDeductionViewModel
    {
        public string ServiceType { get; set; }
        public int ServiceTypeListId { get; set; }
        public string BillMonthYears { get; set; }

        public int DisplayOrder { get; set; }
        public bool DisplaySubReport { get; set; }
        public Nullable<bool> IsSpecialDeduction { get; set; }
        public bool? DisplayLeaseMessage { get; internal set; }

        public int ReSell { get; set; }
        public int FeeId { get; set; }
        public int UseMinumumRoyaltyAmount { get; set; }
        public decimal MinumumRoyaltyAmount { get; set; }
        public int ServiceTypeGroupListId { get; set; }
        public string GroupName { get; set; }
        public List<portal_spGet_AP_FranchiseeReportFinalizedDeductions_Result> Deductions { get; set; }

        public decimal Subtotal { get { return Deductions.Sum(o => (decimal)o.Subtotal); } }
        public decimal Tax { get { return Deductions.Sum(o => (decimal)o.Tax); } }
        public decimal Total { get { return Deductions.Sum(o => (decimal)o.Total); } }

        //public decimal Subtotal { get; }
        //public decimal Tax { get; }
        //public decimal Total { get; }

        //public decimal Subtotal { get { return Deductions.Sum(o => (decimal)o.Subtotal * ((o.AmountTypeListId == 1) ? -1 : 1)); } }
        //public decimal Tax { get { return Deductions.Sum(o => (decimal)o.Tax * ((o.AmountTypeListId == 1) ? -1 : 1)); } }
        //public decimal Total { get { return Deductions.Sum(o => (decimal)o.Total * ((o.AmountTypeListId == 1) ? -1 : 1)); } }
    }

    public class FranchiseeReportFinalizedDetailsByServiceViewModel
    {
        public string ServiceType { get; set; }
        public int ServiceTypeListId { get; set; }
        public int DisplayOrder { get; set; }
        public string BillMonthYears { get; set; }
        public int CreatedBy { get; set; }
        public string ServiceTypeGroupListName { get; set; }
        public List<portal_spGet_AP_FranchiseeReportFinalizedDetailsByCustomerAndServiceType_Result> Details { get; set; }
    }


    public class FranchiseeTurnaroundCheckViewModel
    {
        public int RegionId { get; set; }
        public int FranchiseeId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal Amount { get; set; }

        public int CreatedBy;
        public DateTime CreatedDate;
    }




    public class AccountingFeeRebateDetailsViewModel
    {
        public AccountingFeeRebateViewModel AccountingFeeRebate { get; set; }
        public List<portal_spCreate_AP_AccountingFeeRebateProcess_Result> AccountingFeeRebateList { get; set; }
        public List<portal_spGet_AP_AccountingFeeRebateCheckDetails_Result> AccountingFeeRebateCheckList { get; set; }
        public decimal Total { get { return AccountingFeeRebateCheckList.Sum(o => (decimal)o.CheckAmount); } }

        public string NumberDisplay(decimal? value, int? amountTypeListId = null, bool showDollarSign = false)
        {
            if (value == null)
                return "";

            if (value == 0)
                amountTypeListId = 1;
            if (amountTypeListId == null && value != null)
                amountTypeListId = ((decimal)value >= 0) ? 1 : 2;

            string leftParen = (amountTypeListId == 2) ? "(" : "";
            string rightParen = (amountTypeListId == 2) ? ")" : "";
            string dollarSign = showDollarSign ? "$" : "";

            return string.Format(CultureInfo.InvariantCulture, "{1}{3}{0:N2}{2}", Math.Abs((decimal)value), leftParen, rightParen, dollarSign);
        }

    }

  


}
