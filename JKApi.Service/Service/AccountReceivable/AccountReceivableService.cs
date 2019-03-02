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

namespace JKApi.Service.AccountReceivable
{
    public class AccountReceivableService : BaseService, IAccountReceivableService
    {
        #region ConstructorCalls

        public AccountReceivableService(IJKEfUow uow)
        {
            Uow = uow;
        }

        public AccountReceivableService()
        {
        }

        #endregion



        public List<CustomerSearchModel> GetCustomerListData(string searchtext)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                searchtext = searchtext.ToLower();
                var contactTypeList = (int)Business.Enumeration.ContactTypeList.Main;
                var customers = context.Customers
                    .Where(c => (SelectedRegionId == 0 || c.RegionId == SelectedRegionId) && c.StatusListId == contactTypeList)
                    .Select(c => new CustomerSearchModel { CustomerId = c.CustomerId, CustomerNo = c.CustomerNo, Name = c.Name }).ToList();
                var startsWithNo = customers.Where(o => o.CustomerNo.StartsWith(searchtext, StringComparison.InvariantCultureIgnoreCase)).ToList();
                var startsWithName = customers.Except(startsWithNo).Where(o => o.Name.StartsWith(searchtext, StringComparison.InvariantCultureIgnoreCase)).ToList();
                var containsAll = customers
                    .Except(startsWithNo)
                    .Except(startsWithName).Where(o => searchtext == "" || (o.CustomerNo.ToLower().Contains(searchtext) || o.Name.ToLower().Contains(searchtext)))
                    .OrderBy(c => c.Name).ToList();

                startsWithNo.AddRange(startsWithName);
                startsWithNo.AddRange(containsAll);

                return startsWithNo;
            }
        }

        public portal_spGet_AR_CustomerDetail_Result GetCustomerDetailData(int customerid)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                portal_spGet_AR_CustomerDetail_Result oCustomerDetail = context.portal_spGet_AR_CustomerDetail(customerid).FirstOrDefault();
                return oCustomerDetail;
            }
        }

        public IEnumerable<Franchisee> GetCustomerDistributionDetailFranchiseedata(int customerid)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                List<Franchisee> lstFr = context.portal_spGet_C_CustomerDistributionDetailFranchisees(customerid).MapEnumerable<Franchisee, portal_spGet_C_CustomerDistributionDetailFranchisees_Result>().ToList();


                //List<Franchisee> lstFr = (from d in context.Distributions
                //                          join f in context.Franchisees on d.FranchiseeId equals f.FranchiseeId
                //                          where d.CustomerId == customerid
                //                          select new Franchisee()
                //                          {
                //                              FranchiseeId = f.FranchiseeId,
                //                              FranchiseeNo = f.FranchiseeNo,
                //                              Name = f.Name
                //                          }).ToList<Franchisee>();

                return lstFr;
            }
        }


        public List<ContractDetailServiceTypeList> GetContractDetailServiceTypeList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.ContractDetailServiceTypeLists.ToList();
            }
        }



        public List<Franchisee> GetFranchiseeListData(string searchtext)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<Franchisee> lstFranchisee = context.Franchisees.Where(o => (o.FranchiseeNo.Contains(searchtext) || o.Name.Contains(searchtext)) && (o.RegionId == SelectedRegionId || SelectedRegionId == 0)).ToList();
                return lstFranchisee;
            }
        }

        public List<Franchisee> GetFranchiseeListData()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<Franchisee> lstFranchisee = context.Franchisees.Where(o => o.RegionId == SelectedRegionId || SelectedRegionId == 0).ToList();
                return lstFranchisee;
            }
        }

        public List<portal_spGet_AR_GenerateInvoiceList_Result> GetGenerateInvoiceList(int? regionId = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spGet_AR_GenerateInvoiceList_Result> lstCustomer = context.portal_spGet_AR_GenerateInvoiceList(regionId ?? SelectedRegionId).ToList();
                return lstCustomer;
            }
        }

        public bool GenerateInvoice(int MasterTmpTrxId, int status)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                if (status == 1)
                { context.portal_spGet_AR_GenerateInvoiceByMasterTmpTrxId(MasterTmpTrxId); }
                else
                {
                    ManualInvoiceTmp oManualInvoiceTmp = new ManualInvoiceTmp();
                    var itemToRemove = context.ManualInvoiceTmps.SingleOrDefault(tt => tt.ManualInvoiceTmpId == MasterTmpTrxId); //returns a single item.

                    if (itemToRemove != null)
                    {
                        itemToRemove.IsActive = true;
                        itemToRemove.IsDelete = false;
                        itemToRemove.ModifiedBy = LoginUserId;
                        itemToRemove.ModifiedDate = DateTime.UtcNow;
                        itemToRemove.TransactionStatusListId = 12;
                        context.SaveChanges();

                    }
                }

                return true;
            }
        }

        public void savePendingMessage(string message, int customerID, int status, int MasterTmpTrxId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.SpInsertPendingData(int.Parse(ClaimView.GetCLAIM_USERID()), message, status == 2 ? true : false, status == 1 ? true : false, customerID, DateTime.Now, "AccountReceviableInvoice", MasterTmpTrxId, null, null, null, null, null);
            }
        }
        public void saveCommonPendingMessage(string message, int status, int MasterTmpTrxId, string EntrySource, int ClassId, int TypeListId, int MasterTrxTypeListId, int HeaderId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //   @UID int= null,
                //   @Message nvarchar(max) = null,
                //@IsRejacted bit,
                //   @IsApproved bit,
                //@CustomerID int,
                //   @MessageDate datetime,
                //@EntrySource nvarchar(50),
                //   @MasterTmpTrxId int = null

                //
                context.SpInsertPendingData(
                    LoginUserId,
                    message,
                    status == 2 ? true : false,
                    status == 1 ? true : false,
                    TypeListId == 1 ? ClassId : 0,
                    DateTime.Now,
                    EntrySource,
                    MasterTmpTrxId
                    , ClassId, TypeListId, MasterTrxTypeListId, HeaderId, null);
            }
        }


        public bool InsertCustomerTransaction(CustomerTransactionCommonViewModel inputdata, bool IsMaintenanceTrasaction = false)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //Maintenance Temp Detail



                Customer oCustomer = context.Customers.SingleOrDefault(o => o.CustomerId == inputdata.CustomerId);
                Contract oContract = context.Contracts.SingleOrDefault(o => o.CustomerId == inputdata.CustomerId && o.isActive == true);
                Period oPeriod = context.Periods.SingleOrDefault(o => o.BillMonth == inputdata.InvoiceDate.Month && o.BillYear == inputdata.InvoiceDate.Year);


                if (IsMaintenanceTrasaction)
                {
                    MaintenanceTempDetail oMaintenanceTempDetail = new MaintenanceTempDetail();
                    MaintenanceTempDetail oMaintenanceTempDetailFranchisee = new MaintenanceTempDetail();
                    foreach (CustomerTransactionViewModel o in inputdata.CustomerTransactions)
                    {
                        oMaintenanceTempDetail = new MaintenanceTempDetail();

                        oMaintenanceTempDetail.CustomerId = oCustomer.CustomerId;


                        if (oContract != null)
                            oMaintenanceTempDetail.INV_AccountTypeListId = oContract.AccountTypeListId;

                        oMaintenanceTempDetail.INV_InvoiceDate = inputdata.InvoiceDate;
                        oMaintenanceTempDetail.INV_InvoiceDescription = inputdata.InvoiceDescription;
                        oMaintenanceTempDetail.INV_IsDelete = false;
                        oMaintenanceTempDetail.INV_MasterTrxTypeListId = 5;
                        oMaintenanceTempDetail.INV_TransactionStatusListId = 1;//pending approval
                        oMaintenanceTempDetail.INV_BillMonth = oPeriod.BillMonth;
                        oMaintenanceTempDetail.INV_BillYear = oPeriod.BillYear;
                        oMaintenanceTempDetail.INV_PeriodId = oPeriod.PeriodId;
                        if (!inputdata.CustomerTransactions.FirstOrDefault().TaxExcempt)
                        {
                            oMaintenanceTempDetail.INV_TaxRateId = inputdata.CustomerTransactions.FirstOrDefault().TaxRate;
                            oMaintenanceTempDetail.INV_TaxPercentage = inputdata.CustomerTransactions.FirstOrDefault().TaxPercentage;
                        }
                        else
                        {
                            oMaintenanceTempDetail.INV_TaxRateId = 0;
                            oMaintenanceTempDetail.INV_TaxPercentage = 0;
                        }

                        oMaintenanceTempDetail.INV_AccountRebate = o.AccountRebate;
                        oMaintenanceTempDetail.INV_BPPAdmin = o.BPPAdmin;
                        oMaintenanceTempDetail.INV_Commission = o.Commission;
                        oMaintenanceTempDetail.INV_CommissionTotal = o.CommissionTotal;
                        oMaintenanceTempDetail.INV_Description = o.Description;
                        oMaintenanceTempDetail.INV_ExtendedPrice = o.ExtendedPrice;
                        oMaintenanceTempDetail.INV_ExtraWork = o.ExtraWork;
                        oMaintenanceTempDetail.INV_LineNo = o.LineNo;
                        oMaintenanceTempDetail.INV_MarkUpTotal = o.MarkUpTotal;
                        oMaintenanceTempDetail.INV_PrintInvoice = o.PrintInvoice;
                        oMaintenanceTempDetail.INV_Quantity = o.Quantity;
                        oMaintenanceTempDetail.INV_ServiceTypeListId = o.ServiceTypeListId;
                        oMaintenanceTempDetail.INV_TaxAmount = o.TaxAmount;
                        oMaintenanceTempDetail.INV_TaxExcempt = o.TaxExcempt;
                        oMaintenanceTempDetail.INV_Total = o.Total;
                        oMaintenanceTempDetail.INV_UnitPrice = o.UnitPrice;

                        context.MaintenanceTempDetails.Add(oMaintenanceTempDetail);
                        context.SaveChanges();

                        //FranchiseeTransaction
                        foreach (FranchiseeTransactionViewModel c in inputdata.FranchiseeTransactions)
                        {
                            oMaintenanceTempDetailFranchisee = new MaintenanceTempDetail();

                            oMaintenanceTempDetailFranchisee.CustomerId = oCustomer.CustomerId;
                            if (oContract != null) oMaintenanceTempDetailFranchisee.INV_AccountTypeListId = oContract.AccountTypeListId;

                            oMaintenanceTempDetailFranchisee.INV_InvoiceDate = inputdata.InvoiceDate;
                            oMaintenanceTempDetailFranchisee.INV_InvoiceDescription = inputdata.InvoiceDescription;
                            oMaintenanceTempDetailFranchisee.INV_IsDelete = false;
                            oMaintenanceTempDetailFranchisee.INV_MasterTrxTypeListId = 5;
                            oMaintenanceTempDetailFranchisee.INV_TransactionStatusListId = 1;//pending approval
                            oMaintenanceTempDetailFranchisee.INV_BillMonth = oPeriod.BillMonth;
                            oMaintenanceTempDetailFranchisee.INV_BillYear = oPeriod.BillYear;
                            oMaintenanceTempDetailFranchisee.INV_PeriodId = oPeriod.PeriodId;
                            if (!inputdata.CustomerTransactions.FirstOrDefault().TaxExcempt)
                            {
                                oMaintenanceTempDetailFranchisee.INV_TaxRateId = inputdata.CustomerTransactions.FirstOrDefault().TaxRate;
                                oMaintenanceTempDetailFranchisee.INV_TaxPercentage = inputdata.CustomerTransactions.FirstOrDefault().TaxPercentage;
                            }
                            else
                            {
                                oMaintenanceTempDetailFranchisee.INV_TaxRateId = 0;
                                oMaintenanceTempDetailFranchisee.INV_TaxPercentage = 0;
                            }
                            oMaintenanceTempDetailFranchisee.INV_Total = c.Amount;
                            oMaintenanceTempDetailFranchisee.INV_FranchiseeId = c.FranchiseeId;
                            oMaintenanceTempDetailFranchisee.INV_LineNo = o.LineNo;
                            oMaintenanceTempDetailFranchisee.INV_CustomerTransactionId = oMaintenanceTempDetail.MaintenanceTempDetailId;
                            oMaintenanceTempDetailFranchisee.MaintenanceTempId = 0;//MaintenanceTempId

                            context.MaintenanceTempDetails.Add(oMaintenanceTempDetailFranchisee);
                            context.SaveChanges();
                        }

                    }
                }
                else
                {
                    ManualInvoiceTmp oManualInvoiceTmp = new ManualInvoiceTmp();
                    ManualInvoiceTmpDetail oManualInvoiceTmpDetail = new ManualInvoiceTmpDetail();
                    ManualInvoiceTmpDistribution oManualInvoiceTmpDistribution = new ManualInvoiceTmpDistribution();
                    bool isNew = false;
                    if (inputdata.MasterTmpTrxId > 0)
                    {
                        oManualInvoiceTmp = context.ManualInvoiceTmps.SingleOrDefault(o => o.ManualInvoiceTmpId == inputdata.MasterTmpTrxId);
                        if (oManualInvoiceTmp != null)
                        {
                            oManualInvoiceTmp.ModifiedBy = LoginUserId;
                            oManualInvoiceTmp.ModifiedDate = DateTime.Now;
                            oManualInvoiceTmp.InvoiceDate = inputdata.InvoiceDate;
                            oManualInvoiceTmp.InvoiceDescription = inputdata.InvoiceDescription;
                            oManualInvoiceTmp.IsActive = true;
                            oManualInvoiceTmp.IsDelete = false;
                            oManualInvoiceTmp.MasterTrxTypeListId = 5;

                            oManualInvoiceTmp.TransactionStatusListId = 1;//pending approval

                            oManualInvoiceTmp.BillMonth = oPeriod.BillMonth;
                            oManualInvoiceTmp.BillYear = oPeriod.BillYear;
                            oManualInvoiceTmp.PeriodId = oPeriod.PeriodId;






                            foreach (CustomerTransactionViewModel o in inputdata.CustomerTransactions)
                            {

                                isNew = false;

                                oManualInvoiceTmpDetail = context.ManualInvoiceTmpDetails.SingleOrDefault(ct => ct.LineNo == o.LineNo && (int)ct.ManualInvoiceTmpId == inputdata.MasterTmpTrxId);
                                if (oManualInvoiceTmpDetail != null)
                                {
                                    oManualInvoiceTmpDetail.AccountRebate = o.AccountRebate;
                                    oManualInvoiceTmpDetail.BPPAdmin = o.BPPAdmin;
                                    oManualInvoiceTmpDetail.Commission = o.Commission;
                                    oManualInvoiceTmpDetail.CommissionTotal = o.CommissionTotal;
                                    oManualInvoiceTmpDetail.Description = o.Description;
                                    oManualInvoiceTmpDetail.ExtendedPrice = o.ExtendedPrice;
                                    oManualInvoiceTmpDetail.ExtraWork = o.ExtraWork;
                                    oManualInvoiceTmpDetail.LineNo = o.LineNo;
                                    oManualInvoiceTmpDetail.MarkUpTotal = o.MarkUpTotal;
                                    oManualInvoiceTmpDetail.ModifiedBy = LoginUserId;
                                    oManualInvoiceTmpDetail.ModifiedDate = DateTime.UtcNow;
                                    oManualInvoiceTmpDetail.PrintInvoice = o.PrintInvoice;
                                    oManualInvoiceTmpDetail.Quantity = o.Quantity;
                                    oManualInvoiceTmpDetail.ServiceTypeListId = o.ServiceTypeListId;
                                    oManualInvoiceTmpDetail.Tax = o.TaxAmount;
                                    oManualInvoiceTmpDetail.TaxExcempt = o.TaxExcempt;
                                    oManualInvoiceTmpDetail.Total = o.Total;
                                    oManualInvoiceTmpDetail.UnitPrice = o.UnitPrice;
                                    oManualInvoiceTmpDetail.ClientSupplies = o.ClientSupplies;
                                }
                                else
                                {
                                    oManualInvoiceTmpDetail = new ManualInvoiceTmpDetail();
                                    oManualInvoiceTmpDetail.AccountRebate = o.AccountRebate;
                                    oManualInvoiceTmpDetail.BPPAdmin = o.BPPAdmin;
                                    oManualInvoiceTmpDetail.Commission = o.Commission;
                                    oManualInvoiceTmpDetail.CommissionTotal = o.CommissionTotal;
                                    oManualInvoiceTmpDetail.Description = o.Description;
                                    oManualInvoiceTmpDetail.ExtendedPrice = o.ExtendedPrice;
                                    oManualInvoiceTmpDetail.ExtraWork = o.ExtraWork;
                                    oManualInvoiceTmpDetail.LineNo = o.LineNo;
                                    oManualInvoiceTmpDetail.MarkUpTotal = o.MarkUpTotal;
                                    oManualInvoiceTmpDetail.ModifiedBy = LoginUserId;
                                    oManualInvoiceTmpDetail.ModifiedDate = DateTime.UtcNow;
                                    oManualInvoiceTmpDetail.PrintInvoice = o.PrintInvoice;
                                    oManualInvoiceTmpDetail.Quantity = o.Quantity;
                                    oManualInvoiceTmpDetail.ServiceTypeListId = o.ServiceTypeListId;
                                    oManualInvoiceTmpDetail.Tax = o.TaxAmount;
                                    oManualInvoiceTmpDetail.TaxExcempt = o.TaxExcempt;
                                    oManualInvoiceTmpDetail.Total = o.Total;
                                    oManualInvoiceTmpDetail.UnitPrice = o.UnitPrice;
                                    oManualInvoiceTmpDetail.ClientSupplies = o.ClientSupplies;
                                    oManualInvoiceTmpDetail.ManualInvoiceTmpId = oManualInvoiceTmp.ManualInvoiceTmpId;
                                    context.ManualInvoiceTmpDetails.Add(oManualInvoiceTmpDetail);

                                    isNew = true;
                                }
                                context.SaveChanges();


                                List<ManualInvoiceTmpDistribution> lstFranchiseeTransaction = new List<ManualInvoiceTmpDistribution>();
                                foreach (FranchiseeTransactionViewModel c in inputdata.FranchiseeTransactions)
                                {
                                    if (c.CustomerTransactionId == o.LineNo)
                                    {
                                        if (!isNew)
                                        {
                                            oManualInvoiceTmpDistribution = context.ManualInvoiceTmpDistributions.SingleOrDefault(ct => ct.ManualInvoiceTmpDetailId == oManualInvoiceTmpDetail.ManualInvoiceTmpDetailId && ct.FranchiseeId == c.FranchiseeId);

                                            if (oManualInvoiceTmpDistribution != null)
                                            {
                                                oManualInvoiceTmpDistribution.Amount = c.Amount;
                                                oManualInvoiceTmpDistribution.FranchiseeId = c.FranchiseeId;
                                                oManualInvoiceTmpDistribution.LineNo = oManualInvoiceTmpDetail.LineNo;
                                                oManualInvoiceTmpDistribution.ManualInvoiceTmpDetailId = oManualInvoiceTmpDetail.ManualInvoiceTmpDetailId;
                                                oManualInvoiceTmpDistribution.ModifiedBy = LoginUserId;
                                                oManualInvoiceTmpDistribution.ModifiedDate = DateTime.UtcNow;
                                                context.SaveChanges();
                                            }
                                            else
                                            {
                                                oManualInvoiceTmpDistribution = new ManualInvoiceTmpDistribution();
                                                oManualInvoiceTmpDistribution.Amount = c.Amount;
                                                oManualInvoiceTmpDistribution.FranchiseeId = c.FranchiseeId;
                                                oManualInvoiceTmpDistribution.LineNo = oManualInvoiceTmpDetail.LineNo;
                                                oManualInvoiceTmpDistribution.ManualInvoiceTmpDetailId = oManualInvoiceTmpDetail.ManualInvoiceTmpDetailId;
                                                oManualInvoiceTmpDistribution.ManualInvoiceTmpId = oManualInvoiceTmpDetail.ManualInvoiceTmpId;
                                                oManualInvoiceTmpDistribution.ModifiedBy = LoginUserId;
                                                oManualInvoiceTmpDistribution.ModifiedDate = DateTime.UtcNow;
                                                context.ManualInvoiceTmpDistributions.Add(oManualInvoiceTmpDistribution);
                                                context.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            oManualInvoiceTmpDistribution = new ManualInvoiceTmpDistribution();
                                            oManualInvoiceTmpDistribution.Amount = c.Amount;
                                            oManualInvoiceTmpDistribution.FranchiseeId = c.FranchiseeId;
                                            oManualInvoiceTmpDistribution.LineNo = oManualInvoiceTmpDetail.LineNo;
                                            oManualInvoiceTmpDistribution.ManualInvoiceTmpDetailId = oManualInvoiceTmpDetail.ManualInvoiceTmpDetailId;
                                            oManualInvoiceTmpDistribution.ManualInvoiceTmpId = oManualInvoiceTmpDetail.ManualInvoiceTmpId;
                                            oManualInvoiceTmpDistribution.ModifiedBy = LoginUserId;
                                            oManualInvoiceTmpDistribution.ModifiedDate = DateTime.UtcNow;
                                            context.ManualInvoiceTmpDistributions.Add(oManualInvoiceTmpDistribution);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }

                            if (inputdata.FranchiseeTransactions.Count == 1)
                            {
                                if (inputdata.FranchiseeTransactions[0].CustomerTransactionId == -1)
                                {
                                    oManualInvoiceTmpDistribution = new ManualInvoiceTmpDistribution();
                                    oManualInvoiceTmpDistribution.Amount = inputdata.FranchiseeTransactions[0].Amount;
                                    oManualInvoiceTmpDistribution.FranchiseeId = inputdata.FranchiseeTransactions[0].FranchiseeId;
                                    oManualInvoiceTmpDistribution.LineNo = oManualInvoiceTmpDetail.LineNo;
                                    oManualInvoiceTmpDistribution.ManualInvoiceTmpDetailId = -1;
                                    oManualInvoiceTmpDistribution.ManualInvoiceTmpId = oManualInvoiceTmpDetail.ManualInvoiceTmpId;
                                    oManualInvoiceTmpDistribution.ModifiedBy = LoginUserId;
                                    oManualInvoiceTmpDistribution.ModifiedDate = DateTime.UtcNow;
                                    context.ManualInvoiceTmpDistributions.Add(oManualInvoiceTmpDistribution);
                                    context.SaveChanges();
                                }
                            }
                        }
                        else
                            return false;
                    }
                    else
                    {


                        oManualInvoiceTmp.CustomerId = oCustomer.CustomerId;
                        oManualInvoiceTmp.RegionId = oCustomer.RegionId;
                        if (oContract != null) oManualInvoiceTmp.AccountTypeListId = oContract.AccountTypeListId;

                        oManualInvoiceTmp.InvoiceDate = inputdata.InvoiceDate;
                        oManualInvoiceTmp.InvoiceDescription = inputdata.InvoiceDescription;
                        oManualInvoiceTmp.IsActive = true;
                        oManualInvoiceTmp.IsDelete = false;
                        oManualInvoiceTmp.MasterTrxTypeListId = 5;
                        oManualInvoiceTmp.TransactionStatusListId = 1;//pending approval
                        oManualInvoiceTmp.CreatedBy = LoginUserId;
                        oManualInvoiceTmp.CreatedDate = DateTime.Now;

                        oManualInvoiceTmp.BillMonth = oPeriod.BillMonth;
                        oManualInvoiceTmp.BillYear = oPeriod.BillYear;
                        oManualInvoiceTmp.PeriodId = oPeriod.PeriodId;


                        if (!inputdata.CustomerTransactions.FirstOrDefault().TaxExcempt)
                        {
                            oManualInvoiceTmp.TaxRateId = inputdata.CustomerTransactions.FirstOrDefault().TaxRate;
                            oManualInvoiceTmp.TaxPercentage = inputdata.CustomerTransactions.FirstOrDefault().TaxPercentage;
                        }
                        else
                        {
                            oManualInvoiceTmp.TaxRateId = 0;
                            oManualInvoiceTmp.TaxPercentage = 0;
                        }

                        context.ManualInvoiceTmps.Add(oManualInvoiceTmp);
                        context.SaveChanges();


                        foreach (CustomerTransactionViewModel o in inputdata.CustomerTransactions)
                        {
                            oManualInvoiceTmpDetail = new ManualInvoiceTmpDetail();
                            oManualInvoiceTmpDetail.AccountRebate = o.AccountRebate;
                            oManualInvoiceTmpDetail.BPPAdmin = o.BPPAdmin;
                            oManualInvoiceTmpDetail.Commission = o.Commission;
                            oManualInvoiceTmpDetail.CommissionTotal = o.CommissionTotal;
                            oManualInvoiceTmpDetail.Description = o.Description;
                            oManualInvoiceTmpDetail.ExtendedPrice = o.ExtendedPrice;
                            oManualInvoiceTmpDetail.ExtraWork = o.ExtraWork;
                            oManualInvoiceTmpDetail.LineNo = o.LineNo;
                            oManualInvoiceTmpDetail.MarkUpTotal = o.MarkUpTotal;
                            oManualInvoiceTmpDetail.ModifiedBy = LoginUserId;
                            oManualInvoiceTmpDetail.ModifiedDate = DateTime.UtcNow;
                            oManualInvoiceTmpDetail.PrintInvoice = o.PrintInvoice;
                            oManualInvoiceTmpDetail.Quantity = o.Quantity;
                            oManualInvoiceTmpDetail.ServiceTypeListId = o.ServiceTypeListId;
                            oManualInvoiceTmpDetail.Tax = o.TaxAmount;
                            oManualInvoiceTmpDetail.TaxExcempt = o.TaxExcempt;
                            oManualInvoiceTmpDetail.Total = o.Total;
                            oManualInvoiceTmpDetail.UnitPrice = o.UnitPrice;
                            oManualInvoiceTmpDetail.ClientSupplies = o.ClientSupplies;
                            oManualInvoiceTmpDetail.ManualInvoiceTmpId = oManualInvoiceTmp.ManualInvoiceTmpId;
                            context.ManualInvoiceTmpDetails.Add(oManualInvoiceTmpDetail);
                            context.SaveChanges();


                            List<ManualInvoiceTmpDistribution> lstFranchiseeTransaction = new List<ManualInvoiceTmpDistribution>();
                            foreach (FranchiseeTransactionViewModel c in inputdata.FranchiseeTransactions)
                            {
                                if (c.CustomerTransactionId == o.LineNo)
                                {
                                    oManualInvoiceTmpDistribution = new ManualInvoiceTmpDistribution();
                                    oManualInvoiceTmpDistribution.Amount = c.Amount;
                                    oManualInvoiceTmpDistribution.FranchiseeId = c.FranchiseeId;
                                    oManualInvoiceTmpDistribution.LineNo = oManualInvoiceTmpDetail.LineNo;
                                    oManualInvoiceTmpDistribution.ManualInvoiceTmpDetailId = oManualInvoiceTmpDetail.ManualInvoiceTmpDetailId;
                                    oManualInvoiceTmpDistribution.ManualInvoiceTmpId = oManualInvoiceTmpDetail.ManualInvoiceTmpId;
                                    oManualInvoiceTmpDistribution.ModifiedBy = LoginUserId;
                                    oManualInvoiceTmpDistribution.ModifiedDate = DateTime.UtcNow;
                                    context.ManualInvoiceTmpDistributions.Add(oManualInvoiceTmpDistribution);
                                    context.SaveChanges();

                                }
                            }
                        }
                        if (inputdata.FranchiseeTransactions.Count == 1)
                        {
                            if (inputdata.FranchiseeTransactions[0].CustomerTransactionId == -1)
                            {
                                oManualInvoiceTmpDistribution = new ManualInvoiceTmpDistribution();
                                oManualInvoiceTmpDistribution.Amount = inputdata.FranchiseeTransactions[0].Amount;
                                oManualInvoiceTmpDistribution.FranchiseeId = inputdata.FranchiseeTransactions[0].FranchiseeId;
                                oManualInvoiceTmpDistribution.LineNo = oManualInvoiceTmpDetail.LineNo;
                                oManualInvoiceTmpDistribution.ManualInvoiceTmpDetailId = -1;
                                oManualInvoiceTmpDistribution.ManualInvoiceTmpId = oManualInvoiceTmpDetail.ManualInvoiceTmpId;
                                oManualInvoiceTmpDistribution.ModifiedBy = LoginUserId;
                                oManualInvoiceTmpDistribution.ModifiedDate = DateTime.UtcNow;
                                context.ManualInvoiceTmpDistributions.Add(oManualInvoiceTmpDistribution);
                                context.SaveChanges();
                            }
                        }
                    }
                }
                return true;
            }
        }

        public bool DeleteManualInvoice(int MasterTmpTrxId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                //CustomerTransactionTmp oCustomerTransaction = new CustomerTransactionTmp();
                //FranchiseeTransactionTmp oFranchiseeTransaction = new FranchiseeTransactionTmp();
                ManualInvoiceTmp oManualInvoiceTmp = new ManualInvoiceTmp();
                var itemToRemove = context.ManualInvoiceTmps.SingleOrDefault(tt => tt.ManualInvoiceTmpId == MasterTmpTrxId); //returns a single item.

                if (itemToRemove != null)
                {
                    itemToRemove.IsActive = false;
                    itemToRemove.IsDelete = true;
                    itemToRemove.ModifiedBy = LoginUserId;
                    itemToRemove.ModifiedDate = DateTime.UtcNow;
                    itemToRemove.TransactionStatusListId = 13;
                    context.SaveChanges();

                }
                //foreach (CustomerTransactionTmp oB in itemToRemove)
                //{
                //    var itemToFRemove = context.FranchiseeTransactionTmps.Where(tt => tt.CustomerTransactionTmpId == oB.CustomerTransactionTmpId).ToList(); //returns a single item.
                //    if (itemToFRemove.Count > 0)
                //    {
                //        foreach (FranchiseeTransactionTmp oF in itemToFRemove)
                //        {
                //            oF.IsDelete = true;
                //            oF.TransactionStatusListId = 13;
                //            context.SaveChanges();
                //        }
                //    }

                //    if (itemToRemove.Count > 0)
                //    {
                //        oB.IsDelete = true;
                //        oB.TransactionStatusListId = 13;
                //        context.SaveChanges();
                //    }
                //}


                //if (itemToRemove.Count>0)
                //{
                //    context.CustomerTransactionTmps.RemoveRange(itemToRemove);
                //    context.SaveChanges();
                //}


                //var itemToRemoveT = context.MasterTmpTrxes.SingleOrDefault(tt => tt.MasterTmpTrxId == MasterTmpTrxId); //returns a single item.

                //if (itemToRemoveT != null)
                //{
                //    context.MasterTmpTrxes.Remove(itemToRemoveT);
                //    context.SaveChanges();
                //}




                return true;
            }
        }



        public bool InsertApplyOverflowPaymentTransaction(FullManualPaymentViewModel inputdata)
        {
            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var PR = context.Periods.SingleOrDefault(p => p.BillMonth == inputdata.TransactionDate.Month && p.BillYear == inputdata.TransactionDate.Year);


                int _masterTrxDetailOPID = 0;
                if (inputdata.OInvoiceId > 0)
                {
                    MasterTrxDetail masterTrxDetailOP = new MasterTrxDetail();
                    masterTrxDetailOP.MasterTrxId = null;
                    masterTrxDetailOP.InvoiceId = inputdata.OInvoiceId;
                    masterTrxDetailOP.MasterTrxTypeListId = 24; // customer payment
                    string _DetailDESC = "";
                    foreach (MPInvoiceViewModel ivm in inputdata.Invoices)
                    {
                        Invoice _OPInvoice = context.Invoices.SingleOrDefault(i => i.InvoiceId == ivm.InvoiceId);
                        if (_DetailDESC == "")
                        {
                            _DetailDESC = "Move to " + _OPInvoice.InvoiceNo;
                        }
                        else
                        {
                            _DetailDESC += ", " + _OPInvoice.InvoiceNo;
                        }
                    }
                    masterTrxDetailOP.DetailDescription = _DetailDESC;
                    //masterTrxDetailOP.HeaderId = 
                    masterTrxDetailOP.RegionId = inputdata.RegionId;
                    masterTrxDetailOP.AmountTypeListId = 2; // credit
                    masterTrxDetailOP.FeesDetail = false;
                    masterTrxDetailOP.TaxDetail = false;
                    masterTrxDetailOP.TotalTax = 0;
                    masterTrxDetailOP.TotalFee = 0;
                    masterTrxDetailOP.Quantity = 1;
                    masterTrxDetailOP.UnitPrice = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;
                    masterTrxDetailOP.Total = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;
                    masterTrxDetailOP.ExtendedPrice = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;
                    masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                    masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                    masterTrxDetailOP.IsDelete = false;
                    masterTrxDetailOP.PeriodId = PR.PeriodId;
                    masterTrxDetailOP.TypelistId = 1; // customer
                    masterTrxDetailOP.ClassId = inputdata.CustomerId;
                    masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                    context.MasterTrxDetails.Add(masterTrxDetailOP);
                    context.SaveChanges();
                    _masterTrxDetailOPID = masterTrxDetailOP.MasterTrxDetailId;


                }

                MasterTrx customerMasterTrx = new MasterTrx();
                customerMasterTrx.TypeListId = 1; // customer
                customerMasterTrx.ClassId = inputdata.CustomerId;
                customerMasterTrx.MasterTrxTypeListId = 2; // customer payment
                customerMasterTrx.TrxDate = inputdata.TransactionDate;
                customerMasterTrx.RegionId = inputdata.RegionId;
                customerMasterTrx.StatusId = 3; // open
                customerMasterTrx.BillMonth = PR.BillMonth;
                customerMasterTrx.BillYear = PR.BillYear;
                customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                customerMasterTrx.CreatedBy = LoginUserId;
                customerMasterTrx.CreatedDate = DateTime.Now;

                //customerMasterTrx.AccountTypeListId;              
                //customerMasterTrx.HeaderId;

                context.MasterTrxes.Add(customerMasterTrx);
                context.SaveChanges();


                Payment customerPayment = new Payment();
                customerPayment.MasterTrxId = customerMasterTrx.MasterTrxId;
                customerPayment.TypeListId = 1; // customer
                customerPayment.ClassId = inputdata.CustomerId;
                customerPayment.RegionId = inputdata.RegionId;
                customerPayment.PaymentMethodListId = inputdata.PaymentMethodListId;
                customerPayment.PaymentNo = inputdata.ReferenceNo;
                customerPayment.TransactionStatusListId = 3;// allPaidInFull ? 6 : 7; // paid : paid partial
                customerPayment.TempTransactionStatusListId = 3;// allPaidInFull ? 6 : 7; // paid : paid partial
                customerPayment.CheckAmount = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;

                //GET INVOICE NUMBER FOR Payment Number 
                int InvIdT = inputdata.Invoices[0].InvoiceId;
                Invoice _invoiceOject = context.Invoices.Where(o => o.InvoiceId == InvIdT).FirstOrDefault();
                customerPayment.TransactionNumber = "PMT" + _invoiceOject.InvoiceNo.Trim(); // customerNextTrxNumber;

                customerPayment.TransactionDate = inputdata.TransactionDate;
                customerPayment.IsDelete = false;
                customerPayment.CreatedBy = inputdata.CreatedBy;
                customerPayment.CreatedDate = inputdata.CreatedDate;
                customerPayment.PaymentDescription = inputdata.Notes;
                customerPayment.PeriodId = (PR != null ? PR.PeriodId : 0);
                customerPayment.CheckAmount = inputdata.PaymentAmount;
                context.Payments.Add(customerPayment);
                context.SaveChanges();

                customerMasterTrx.HeaderId = customerPayment.PaymentId;
                context.SaveChanges();


                decimal totalCustomerPayment = 0;
                decimal totalCustomerTaxes = 0;

                int custMasterTrxDetailId = 0;
                foreach (MPInvoiceViewModel ivm in inputdata.Invoices)
                {
                    if (ivm.InvoicePayment > 0)
                    {
                        foreach (ManualPaymentViewModel cvm in ivm.CustomerPayment.Payments)
                        {
                            if (cvm.ExtendedPrice > 0)
                            {



                                MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                                masterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId;
                                masterTrxDetail.InvoiceId = ivm.InvoiceId;
                                masterTrxDetail.LineNo = cvm.LineNo;
                                masterTrxDetail.MasterTrxTypeListId = 2; // customer payment
                                masterTrxDetail.HeaderId = customerPayment.PaymentId;
                                masterTrxDetail.RegionId = inputdata.RegionId;
                                masterTrxDetail.AmountTypeListId = 1; // credit
                                masterTrxDetail.FeesDetail = false;
                                masterTrxDetail.TaxDetail = true;
                                masterTrxDetail.TotalTax = Math.Round(cvm.Tax,2);
                                masterTrxDetail.Total = cvm.PaymentAmount;
                                masterTrxDetail.UnitPrice = cvm.ExtendedPrice;
                                masterTrxDetail.ExtendedPrice = cvm.ExtendedPrice;
                                masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                masterTrxDetail.IsDelete = false;
                                //masterTrxDetail.ServiceTypeListId =
                                masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                                masterTrxDetail.TypelistId = 1; // customer
                                masterTrxDetail.ClassId = inputdata.CustomerId;
                                masterTrxDetail.Transactiondate = inputdata.TransactionDate;
                                masterTrxDetail.BPPAdmin = 1;
                                masterTrxDetail.AccountRebate = 1;
                                masterTrxDetail.FRRevenues = false;
                                masterTrxDetail.FRDeduction = false;
                                masterTrxDetail.DetailDescription = inputdata.Notes;
                                List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                                if (_masterINVDetail.Count > 0)
                                {
                                    //masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                    //masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                    masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                    //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                    masterTrxDetail.Quantity = 0;
                                    masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                    masterTrxDetail.TotalFee = 0;
                                    masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                    masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                    masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                    masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                    masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                }

                                masterTrxDetail.SourceId = _masterTrxDetailOPID;
                                masterTrxDetail.SourceTypeListId = 47;

                                context.MasterTrxDetails.Add(masterTrxDetail);
                                context.SaveChanges();


                                custMasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                                // insert customer taxes

                                MasterTrxTax customerTax = new MasterTrxTax();
                                customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                                customerTax.Amount = cvm.Tax;
                                customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.PaymentAmount, 2);
                                customerTax.AmountTypeListId = 1; // credit
                                customerTax.CreatedBy = inputdata.CreatedBy;
                                customerTax.CreatedDate = inputdata.CreatedDate;

                                customerTax.InvoiceId = ivm.InvoiceId;
                                customerTax.RegionId = inputdata.RegionId;
                                customerTax.PeriodId = (PR != null ? PR.PeriodId : 0);
                                customerTax.CustomerId = inputdata.CustomerId;
                                customerTax.FRRevenues = false;
                                customerTax.FRDeduction = false;
                                //customerTax.FranchiseeId = null;
                                context.MasterTrxTaxes.Add(customerTax);
                                context.SaveChanges();

                                totalCustomerPayment += cvm.Total;
                                totalCustomerTaxes += cvm.Tax;

                                var invoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
                                var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                                var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

                                List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
                                decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
                                decimal totalTransactions = 0.00m;
                                decimal grandTotalTransactions = 0.00m;

                                foreach (var trx in invoiceTransactions)
                                {
                                    totalTransactions = totalTransactions + (decimal)trx.Total;
                                }
                                grandTotalTransactions = totalTransactions + cvm.PaymentAmount;


                                if (grandTotalTransactions >= invoiceTotal)
                                {
                                    invoice.TransactionStatusListId = 6; /*6 = Paid*/
                                    invoiceMasterTrx.StatusId = 6;
                                }
                                else
                                {
                                    invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
                                    invoiceMasterTrx.StatusId = 7;
                                }
                                context.SaveChanges();





                            }



                        }
                        //Franchisee Billing
                        foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
                        {
                            // compute franchisee payment fees

                            decimal totalFees = 0;

                            List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
                            List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

                            foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                            {
                                MasterTrxFeeDetail feeDetail = new MasterTrxFeeDetail();
                                if (feeDef.FeePercentage != null) // percentage
                                {
                                    feeDetail.FeePercentage = feeDef.FeePercentage;
                                    feeDetail.Amount = (decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M);// Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
                                }
                                else // flat amount
                                {
                                    feeDetail.Amount = feeDef.Amount;
                                    feeDetail.FeePercentage = null;
                                }
                                feeDetail.FeeId = feeDef.FeeId;
                                feeDetail.AmountTypeListId = 1; // credit
                                feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                                feeDetail.CreatedBy = inputdata.CreatedBy;
                                feeDetail.CreatedDate = inputdata.CreatedDate;
                                feeDetail.FranchiseeId = fcvm.FranchiseeId;
                                feeDetail.BillingPayId = fcvm.BillingPayId;

                                totalFees += feeDetail.Amount ?? 0;

                                franchiseeFees.Add(feeDetail);
                            }

                            decimal paymentMinusFees = (decimal)(fcvm.Payment.PaymentAmount - totalFees);// Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

                            MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
                            franchiseeMasterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId; //franchiseeMasterTrx.MasterTrxId;
                            franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
                            franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
                            franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
                            franchiseeMasterTrxDetail.HeaderId = fcvm.BillingPayId;// franchiseePayment.PaymentBillingFranchiseeId; //customerPayment.PaymentId;
                            franchiseeMasterTrxDetail.RegionId = inputdata.RegionId;

                            franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                            franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
                            franchiseeMasterTrxDetail.FeesDetail = true;
                            franchiseeMasterTrxDetail.TaxDetail = false;
                            franchiseeMasterTrxDetail.TotalFee = totalFees;
                            franchiseeMasterTrxDetail.Total = paymentMinusFees;

                            franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
                            franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
                            franchiseeMasterTrxDetail.IsDelete = false;
                            franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                            franchiseeMasterTrxDetail.TypelistId = 2; // customer
                            franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
                            franchiseeMasterTrxDetail.Transactiondate = inputdata.TransactionDate;

                            franchiseeMasterTrxDetail.BPPAdmin = 1;
                            franchiseeMasterTrxDetail.AccountRebate = 1;
                            franchiseeMasterTrxDetail.AccountRebate = 1;
                            franchiseeMasterTrxDetail.Commission = false;
                            franchiseeMasterTrxDetail.CommissionTotal = 0;
                            franchiseeMasterTrxDetail.FRRevenues = false;
                            franchiseeMasterTrxDetail.FRDeduction = false;
                            franchiseeMasterTrxDetail.DetailDescription = inputdata.Notes;

                            List<MasterTrxDetail> _masterINVDetail1 = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                            if (_masterINVDetail1.Count > 0)
                            {
                                //franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                //franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail1[0].ServiceTypeListId;
                                //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                franchiseeMasterTrxDetail.Quantity = 0;
                                franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail1[0].CPIPercentage;
                                franchiseeMasterTrxDetail.TotalTax = 0;
                                franchiseeMasterTrxDetail.Commission = _masterINVDetail1[0].Commission;
                                franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail1[0].CommissionTotal;
                                franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail1[0].ExtraWork;
                                franchiseeMasterTrxDetail.ReSell = _masterINVDetail1[0].ReSell;
                                franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail1[0].ClientSupplies;

                            }
                            franchiseeMasterTrxDetail.SourceId = _masterTrxDetailOPID;
                            franchiseeMasterTrxDetail.SourceTypeListId = 47;

                            context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                            context.SaveChanges();


                            // insert franchisee fees

                            foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
                            {
                                feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

                                context.MasterTrxFeeDetails.Add(feeDetail);
                                context.SaveChanges();
                            }


                        }

                        //foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
                        //{
                        //    // compute franchisee payment fees

                        //    decimal totalFees = 0;

                        //    List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
                        //    List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

                        //    foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                        //    {
                        //        MasterTrxFeeDetail feeDetail = new MasterTrxFeeDetail();
                        //        if (feeDef.FeePercentage != null) // percentage
                        //        {
                        //            feeDetail.FeePercentage = feeDef.FeePercentage;
                        //            feeDetail.Amount = Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
                        //        }
                        //        else // flat amount
                        //        {
                        //            feeDetail.Amount = feeDef.Amount;
                        //            feeDetail.FeePercentage = null;
                        //        }
                        //        feeDetail.FeeId = feeDef.FeeId;
                        //        feeDetail.AmountTypeListId = 1; // credit
                        //        feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                        //        feeDetail.CreatedBy = inputdata.CreatedBy;
                        //        feeDetail.CreatedDate = inputdata.CreatedDate;

                        //        totalFees += feeDetail.Amount ?? 0;

                        //        franchiseeFees.Add(feeDetail);
                        //    }

                        //    // franchisee payment mastertrx

                        //    MasterTrx franchiseeMasterTrx = new MasterTrx();
                        //    franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
                        //    franchiseeMasterTrx.TypeListId = 2; // franchisee
                        //    franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
                        //    franchiseeMasterTrx.TrxDate = inputdata.TransactionDate;
                        //    franchiseeMasterTrx.RegionId = inputdata.RegionId;
                        //    franchiseeMasterTrx.StatusId = 3; // open

                        //    franchiseeMasterTrx.BillMonth = inputdata.TransactionDate.Month;
                        //    franchiseeMasterTrx.BillYear = inputdata.TransactionDate.Year;
                        //    franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                        //    franchiseeMasterTrx.CreatedBy = LoginUserId;
                        //    franchiseeMasterTrx.CreatedDate = DateTime.Now;

                        //    context.MasterTrxes.Add(franchiseeMasterTrx);
                        //    context.SaveChanges();


                        //    // franchisee payment

                        //    string franchiseeNextTrxNumber = "BILPMT" + _invoiceOject.InvoiceNo.Trim(); ;



                        //    PaymentBillingFranchisee franchiseePayment = new PaymentBillingFranchisee();
                        //    franchiseePayment.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        //    franchiseePayment.PaymentId = customerPayment.PaymentId;
                        //    franchiseePayment.FranchiseeId = fcvm.FranchiseeId;
                        //    franchiseePayment.BillingPayId = fcvm.BillingPayId;
                        //    franchiseePayment.RegionId = inputdata.RegionId;
                        //    franchiseePayment.LineNo = Convert.ToString(fcvm.Payment.LineNo);
                        //    franchiseePayment.Amount = fcvm.Payment.PaymentAmount;
                        //    franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
                        //    franchiseePayment.TransactionDate = inputdata.TransactionDate;
                        //    franchiseePayment.CreatedBy = inputdata.CreatedBy;
                        //    franchiseePayment.CreatedDate = inputdata.CreatedDate;
                        //    franchiseePayment.IsTARPaid = fcvm.IsTARPaid;
                        //    franchiseePayment.IsTurnaroundPayment = fcvm.IsTurnAroundPayment;
                        //    franchiseePayment.TransactionStatusListId = 3;
                        //    context.PaymentBillingFranchisees.Add(franchiseePayment);
                        //    context.SaveChanges();

                        //    franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
                        //    context.SaveChanges();


                        //    decimal paymentMinusFees = Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

                        //    MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
                        //    franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        //    franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
                        //    franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
                        //    franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
                        //    franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentBillingFranchiseeId; //customerPayment.PaymentId;
                        //    franchiseeMasterTrxDetail.RegionId = inputdata.RegionId;

                        //    franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                        //    franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
                        //    franchiseeMasterTrxDetail.FeesDetail = true;
                        //    franchiseeMasterTrxDetail.TaxDetail = false;
                        //    franchiseeMasterTrxDetail.TotalFee = totalFees;
                        //    franchiseeMasterTrxDetail.Total = paymentMinusFees;

                        //    franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
                        //    franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
                        //    franchiseeMasterTrxDetail.IsDelete = false;
                        //    franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                        //    franchiseeMasterTrxDetail.TypelistId = 2; // customer
                        //    franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
                        //    franchiseeMasterTrxDetail.Transactiondate = inputdata.TransactionDate;



                        //    franchiseeMasterTrxDetail.BPPAdmin = 1;
                        //    franchiseeMasterTrxDetail.AccountRebate = 1;
                        //    franchiseeMasterTrxDetail.AccountRebate = 1;
                        //    franchiseeMasterTrxDetail.Commission = false;
                        //    franchiseeMasterTrxDetail.CommissionTotal = 0;
                        //    franchiseeMasterTrxDetail.FRRevenues = false;
                        //    franchiseeMasterTrxDetail.FRDeduction = false;
                        //    franchiseeMasterTrxDetail.DetailDescription = inputdata.Notes;

                        //    List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                        //    if (_masterINVDetail.Count > 0)
                        //    {
                        //        //franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                        //        //franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                        //        franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                        //        //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                        //        franchiseeMasterTrxDetail.Quantity = 0;
                        //        franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                        //        franchiseeMasterTrxDetail.TotalTax = 0;
                        //        franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
                        //        franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                        //        franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                        //        franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                        //        franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                        //    }
                        //    franchiseeMasterTrxDetail.SourceId = _masterTrxDetailOPID;
                        //    franchiseeMasterTrxDetail.SourceTypeListId = 47;

                        //    context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                        //    context.SaveChanges();


                        //    // insert franchisee fees

                        //    foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
                        //    {
                        //        feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

                        //        context.MasterTrxFeeDetails.Add(feeDetail);
                        //        context.SaveChanges();
                        //    }


                        //}
                    }
                    else
                    {
                        ivm.OverflowAmount = ivm.OverflowAmount - Math.Abs(ivm.InvoicePayment);
                    }

                    if (ivm.OverflowAmount > 0)
                    {
                        MasterTrxDetail masterTrxDetailOP = new MasterTrxDetail();
                        masterTrxDetailOP.MasterTrxId = customerMasterTrx.MasterTrxId;
                        masterTrxDetailOP.InvoiceId = ivm.InvoiceId;
                        masterTrxDetailOP.MasterTrxTypeListId = 17; // customer payment
                        masterTrxDetailOP.HeaderId = customerPayment.PaymentId;
                        masterTrxDetailOP.RegionId = inputdata.RegionId;
                        masterTrxDetailOP.AmountTypeListId = 1; // credit
                        masterTrxDetailOP.FeesDetail = false;
                        masterTrxDetailOP.TaxDetail = true;
                        masterTrxDetailOP.TotalTax = 0;
                        masterTrxDetailOP.TotalFee = 0;
                        masterTrxDetailOP.Quantity = 1;
                        masterTrxDetailOP.UnitPrice = ivm.OverflowAmount;
                        masterTrxDetailOP.Total = ivm.OverflowAmount;
                        masterTrxDetailOP.ExtendedPrice = ivm.OverflowAmount;
                        masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                        masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                        masterTrxDetailOP.IsDelete = false;
                        masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
                        masterTrxDetailOP.TypelistId = 1; // customer
                        masterTrxDetailOP.ClassId = inputdata.CustomerId;
                        masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                        masterTrxDetailOP.FRRevenues = false;
                        masterTrxDetailOP.FRDeduction = false;
                        context.MasterTrxDetails.Add(masterTrxDetailOP);
                        context.SaveChanges();
                    }


                }
                //if ((totalCustomerPayment + totalCustomerTaxes) > 0)
                //{
                //    // general ledger for customer payment -- debit from A/R Janitorial Services
                //    GeneralLedgerTrx oGeneralLedgerTrx = new GeneralLedgerTrx();

                //    oGeneralLedgerTrx.MasterTrxId = customerMasterTrx.MasterTrxId;
                //    oGeneralLedgerTrx.LedgerAccountId = 3;
                //    oGeneralLedgerTrx.MasterTrxTypeListId = 2;
                //    oGeneralLedgerTrx.TrxDate = DateTime.Now;
                //    oGeneralLedgerTrx.Debit = totalCustomerPayment + totalCustomerTaxes;
                //    oGeneralLedgerTrx.Credit = 0;
                //    oGeneralLedgerTrx.Amount = totalCustomerPayment + totalCustomerTaxes;
                //    oGeneralLedgerTrx.AmountTypeListId = 2;
                //    oGeneralLedgerTrx.IsDelete = false;
                //    oGeneralLedgerTrx.RegionId = inputdata.RegionId;
                //    oGeneralLedgerTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                //    oGeneralLedgerTrx.CreatedBy = LoginUserId;
                //    oGeneralLedgerTrx.CreatedDate = DateTime.Now;
                //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
                //    context.SaveChanges();


                //    // general ledger for customer payment -- debit from A/R Janitorial Services
                //    GeneralLedgerTrx oGeneralLedgerTrxM = new GeneralLedgerTrx();

                //    oGeneralLedgerTrxM.MasterTrxId = customerMasterTrx.MasterTrxId;
                //    oGeneralLedgerTrxM.LedgerAccountId = 3;
                //    oGeneralLedgerTrxM.MasterTrxTypeListId = 7;
                //    oGeneralLedgerTrxM.TrxDate = DateTime.Now;
                //    oGeneralLedgerTrxM.Debit = 0;
                //    oGeneralLedgerTrxM.Credit = totalCustomerPayment + totalCustomerTaxes;
                //    oGeneralLedgerTrxM.Amount = totalCustomerPayment + totalCustomerTaxes;
                //    oGeneralLedgerTrxM.AmountTypeListId = 1;
                //    oGeneralLedgerTrxM.IsDelete = false;
                //    oGeneralLedgerTrxM.RegionId = inputdata.RegionId;
                //    oGeneralLedgerTrxM.PeriodId = (PR != null ? PR.PeriodId : 0);
                //    oGeneralLedgerTrxM.CreatedBy = LoginUserId;
                //    oGeneralLedgerTrxM.CreatedDate = DateTime.Now;
                //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrxM);
                //    context.SaveChanges();
                //}



                return true;
            }
        }



        public bool InsertManualPaymentTransactionInTemp(FullManualPaymentViewModel inputdata)
        {
            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();

            JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(2, inputdata.RegionId);
            franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(7, inputdata.RegionId);

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var PR = context.Periods.SingleOrDefault(p => p.BillMonth == inputdata.TransactionDate.Month && p.BillYear == inputdata.TransactionDate.Year);

                List<int> lstCust = inputdata.Invoices.Select(g => g.InvoiceCustomerId).Distinct().ToList<int>();



                PaymentTempMasterTrx customerMasterTrx = new PaymentTempMasterTrx();
                customerMasterTrx.TypeListId = 1; // customer
                customerMasterTrx.ClassId = inputdata.CustomerId;
                customerMasterTrx.MasterTrxTypeListId = 2; // customer payment
                customerMasterTrx.TrxDate = inputdata.TransactionDate;
                customerMasterTrx.RegionId = inputdata.RegionId;
                customerMasterTrx.StatusId = 1; // open
                customerMasterTrx.BillMonth = PR.BillMonth;
                customerMasterTrx.BillYear = PR.BillYear;
                customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                customerMasterTrx.CreatedBy = LoginUserId;
                customerMasterTrx.CreatedDate = DateTime.Now;

                //customerMasterTrx.AccountTypeListId;              
                //customerMasterTrx.HeaderId;

                context.PaymentTempMasterTrxes.Add(customerMasterTrx);
                context.SaveChanges();

                int InvIdT = 0;
                foreach (var _oCustId in lstCust)
                {

                    PaymentTemp customerPayment = new PaymentTemp();
                    customerPayment.MasterTrxId = customerMasterTrx.PaymentTempMasterTrxId;
                    customerPayment.TypeListId = 1; // customer
                    customerPayment.ClassId = _oCustId;
                    customerPayment.RegionId = inputdata.RegionId;
                    customerPayment.PaymentMethodListId = inputdata.PaymentMethodListId;
                    customerPayment.PaymentNo = inputdata.ReferenceNo;
                    customerPayment.TransactionStatusListId = 1;// allPaidInFull ? 6 : 7; // paid : paid partial
                    customerPayment.TempTransactionStatusListId = 3;// allPaidInFull ? 6 : 7; // paid : paid partial
                    customerPayment.CheckAmount = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;

                    //GET INVOICE NUMBER FOR Payment Number 
                    InvIdT = inputdata.Invoices.Where(p => p.InvoiceCustomerId == _oCustId).FirstOrDefault().InvoiceId;
                    Invoice _invoiceOject = context.Invoices.Where(o => o.InvoiceId == InvIdT).FirstOrDefault();

                    customerPayment.TransactionNumber = "PMT" + customerTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", customerMasterTrx.BillMonth) + (customerTransactionNumberConfigViewModel.LastNumber + 1).ToString();
                    // customerPayment.TransactionNumber = "PMT" + _invoiceOject.InvoiceNo.Trim(); // customerNextTrxNumber;

                    customerPayment.TransactionDate = inputdata.TransactionDate;
                    customerPayment.IsDelete = false;
                    customerPayment.CreatedBy = inputdata.CreatedBy;
                    customerPayment.CreatedBy = inputdata.CreatedBy;
                    customerPayment.ModifiedBy = inputdata.CreatedBy;
                    customerPayment.ModifiedDate = inputdata.CreatedDate;
                    customerPayment.PaymentDescription = inputdata.Notes;
                    customerPayment.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerPayment.CheckAmount = inputdata.PaymentAmount;
                    customerPayment.MasterTrxTypeListId = 2;
                    context.PaymentTemps.Add(customerPayment);
                    context.SaveChanges();

                    customerMasterTrx.HeaderId = customerPayment.PaymentTempId;
                    context.SaveChanges();

                    if (customerTransactionNumberConfigViewModel != null)
                    {
                        customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                    }


                    decimal totalCustomerPayment = 0;
                    decimal totalCustomerTaxes = 0;

                    int custMasterTrxDetailId = 0;
                    foreach (MPInvoiceViewModel ivm in inputdata.Invoices.Where(p => p.InvoiceCustomerId == _oCustId))
                    {
                        if (ivm.InvoicePayment > 0)
                        {
                            foreach (ManualPaymentViewModel cvm in ivm.CustomerPayment.Payments)
                            {
                                if (cvm.ExtendedPrice > 0)
                                {
                                    PaymentTempDetail masterTrxDetail = new PaymentTempDetail();
                                    masterTrxDetail.MasterTrxId = customerMasterTrx.PaymentTempMasterTrxId;
                                    masterTrxDetail.InvoiceId = ivm.InvoiceId;
                                    masterTrxDetail.LineNo = cvm.LineNo;
                                    masterTrxDetail.MasterTrxTypeListId = 2; // customer payment
                                    masterTrxDetail.HeaderId = customerPayment.PaymentTempId;
                                    masterTrxDetail.RegionId = inputdata.RegionId;
                                    masterTrxDetail.AmountTypeListId = 1; // credit
                                    masterTrxDetail.FeesDetail = false;
                                    masterTrxDetail.TaxDetail = true;
                                    masterTrxDetail.TotalTax = cvm.Tax;
                                    masterTrxDetail.Total = cvm.PaymentAmount;
                                    masterTrxDetail.ExtendedPrice = cvm.ExtendedPrice;
                                    masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                    masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                    masterTrxDetail.IsDelete = false;
                                    //masterTrxDetail.ServiceTypeListId =
                                    masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    masterTrxDetail.TypelistId = 1; // customer
                                    masterTrxDetail.ClassId = _oCustId;
                                    masterTrxDetail.Transactiondate = inputdata.TransactionDate;
                                    masterTrxDetail.BPPAdmin = 1;
                                    masterTrxDetail.AccountRebate = 1;
                                    masterTrxDetail.AccountRebate = 1;
                                    masterTrxDetail.Commission = false;
                                    masterTrxDetail.CommissionTotal = 0;
                                    masterTrxDetail.FRRevenues = false;
                                    masterTrxDetail.FRDeduction = false;
                                    masterTrxDetail.DetailDescription = inputdata.Notes;

                                    masterTrxDetail.UnitPrice = 0;
                                    masterTrxDetail.CPIPercentage = 0;
                                    masterTrxDetail.TotalFee = 0;

                                    masterTrxDetail.SourceId = 0;
                                    masterTrxDetail.SourceTypeListId = 0;
                                    masterTrxDetail.ServiceTypeListId = 0;
                                    //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                    masterTrxDetail.Quantity = 0;
                                    masterTrxDetail.CPIPercentage = 0;
                                    masterTrxDetail.TotalFee = 0;
                                    masterTrxDetail.Commission = false;
                                    masterTrxDetail.CommissionTotal = 0;
                                    masterTrxDetail.ExtraWork = 0;
                                    masterTrxDetail.ReSell = false;
                                    masterTrxDetail.ClientSupplies = false;

                                    List<PaymentTempDetail> _masterINVDetail = context.PaymentTempDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                                    if (_masterINVDetail.Count > 0)
                                    {
                                        masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                        masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                        masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                        //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                        masterTrxDetail.Quantity = 0;
                                        masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                        masterTrxDetail.TotalFee = 0;
                                        masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                        masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                        masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                        masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                        masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                    }



                                    context.PaymentTempDetails.Add(masterTrxDetail);
                                    context.SaveChanges();


                                    custMasterTrxDetailId = masterTrxDetail.PaymentTempDetailId;
                                    // insert customer taxes

                                    PaymentTempDetailTax customerTax = new PaymentTempDetailTax();
                                    customerTax.MasterTrxDetailId = masterTrxDetail.PaymentTempDetailId;
                                    customerTax.Amount = cvm.Tax;
                                    customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.PaymentAmount, 2);
                                    customerTax.AmountTypeListId = 1; // credit
                                    customerTax.CreatedBy = inputdata.CreatedBy;
                                    customerTax.CreatedDate = inputdata.CreatedDate;

                                    customerTax.InvoiceId = ivm.InvoiceId;
                                    customerTax.RegionId = inputdata.RegionId;
                                    customerTax.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    customerTax.CustomerId = _oCustId;
                                    customerTax.FRRevenues = false;
                                    customerTax.FRDeduction = false;
                                    //customerTax.FranchiseeId = null;
                                    context.PaymentTempDetailTaxes.Add(customerTax);
                                    context.SaveChanges();

                                    totalCustomerPayment += cvm.Total;
                                    totalCustomerTaxes += cvm.Tax;

                                    var invoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
                                    var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                                    var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

                                    List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
                                    decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
                                    decimal totalTransactions = 0.00m;
                                    decimal grandTotalTransactions = 0.00m;

                                    foreach (var trx in invoiceTransactions)
                                    {
                                        totalTransactions = totalTransactions + (decimal)trx.Total;
                                    }
                                    grandTotalTransactions = totalTransactions + cvm.PaymentAmount;


                                    if (grandTotalTransactions >= invoiceTotal)
                                    {
                                        invoice.TransactionStatusListId = 6; /*6 = Paid*/
                                        invoiceMasterTrx.StatusId = 6;
                                    }
                                    else
                                    {
                                        invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
                                        invoiceMasterTrx.StatusId = 7;
                                    }
                                    context.SaveChanges();
                                }

                            }
                            foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
                            {
                                // compute franchisee payment fees

                                decimal totalFees = 0;

                                List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
                                List<PaymentTempDetailFee> franchiseeFees = new List<PaymentTempDetailFee>();

                                foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                                {
                                    PaymentTempDetailFee feeDetail = new PaymentTempDetailFee();
                                    if (feeDef.FeePercentage != null) // percentage
                                    {
                                        feeDetail.FeePercentage = feeDef.FeePercentage;
                                        feeDetail.Amount = (decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M);// Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
                                    }
                                    else // flat amount
                                    {
                                        feeDetail.Amount = feeDef.Amount;
                                        feeDetail.FeePercentage = null;
                                    }
                                    feeDetail.FeeId = feeDef.FeeId;
                                    feeDetail.AmountTypeListId = 1; // credit
                                    feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                                    feeDetail.CreatedBy = inputdata.CreatedBy;
                                    feeDetail.CreatedDate = inputdata.CreatedDate;
                                    feeDetail.FranchiseeId = fcvm.FranchiseeId;
                                    feeDetail.BillingPayId = fcvm.BillingPayId;
                                    totalFees += feeDetail.Amount ?? 0;

                                    franchiseeFees.Add(feeDetail);
                                }

                                // franchisee payment mastertrx

                                PaymentTempMasterTrx franchiseeMasterTrx = new PaymentTempMasterTrx();
                                franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
                                franchiseeMasterTrx.TypeListId = 2; // franchisee
                                franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
                                franchiseeMasterTrx.TrxDate = inputdata.TransactionDate;
                                franchiseeMasterTrx.RegionId = inputdata.RegionId;
                                franchiseeMasterTrx.StatusId = 1; // open

                                franchiseeMasterTrx.BillMonth = inputdata.TransactionDate.Month;
                                franchiseeMasterTrx.BillYear = inputdata.TransactionDate.Year;
                                franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                                franchiseeMasterTrx.CreatedBy = LoginUserId;
                                franchiseeMasterTrx.CreatedDate = DateTime.Now;
                                franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                                context.PaymentTempMasterTrxes.Add(franchiseeMasterTrx);
                                context.SaveChanges();


                                // franchisee payment

                                string franchiseeNextTrxNumber = franchiseeTransactionNumberConfigViewModel.Prefix.Trim() + franchiseeTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", inputdata.TransactionDate.Month) + (franchiseeTransactionNumberConfigViewModel.LastNumber + 1).ToString();
                                //string franchiseeNextTrxNumber = "BILPMT" + _invoiceOject.InvoiceNo.Trim(); ;



                                PaymentTemp franchiseePayment = new PaymentTemp();
                                franchiseePayment.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
                                franchiseePayment.PaymentId = customerPayment.PaymentTempId;
                                franchiseePayment.ClassId = fcvm.FranchiseeId;
                                franchiseePayment.TypeListId = 2;
                                franchiseePayment.BillingPayId = fcvm.BillingPayId;
                                franchiseePayment.RegionId = inputdata.RegionId;
                                franchiseePayment.LineNo = Convert.ToString(fcvm.Payment.LineNo);
                                franchiseePayment.Amount = fcvm.Payment.PaymentAmount;
                                franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
                                franchiseePayment.TransactionDate = inputdata.TransactionDate;
                                franchiseePayment.CreatedBy = inputdata.CreatedBy;
                                franchiseePayment.CreatedDate = inputdata.CreatedDate;
                                franchiseePayment.IsTARPaid = fcvm.IsTARPaid;
                                franchiseePayment.IsTurnaroundPayment = fcvm.IsTurnAroundPayment;
                                franchiseePayment.TransactionStatusListId = 1;
                                franchiseePayment.PeriodId = (PR != null ? PR.PeriodId : 0);
                                franchiseePayment.IsBillingFranchisee = true;
                                franchiseePayment.MasterTrxTypeListId = 7;
                                context.PaymentTemps.Add(franchiseePayment);
                                context.SaveChanges();

                                franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
                                context.SaveChanges();

                                if (franchiseeTransactionNumberConfigViewModel != null)
                                {
                                    franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
                                    CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                                }


                                decimal paymentMinusFees = (decimal)(fcvm.Payment.PaymentAmount - totalFees);// Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

                                PaymentTempDetail franchiseeMasterTrxDetail = new PaymentTempDetail();
                                franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
                                franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
                                franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
                                franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
                                franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentTempId; //customerPayment.PaymentId;
                                franchiseeMasterTrxDetail.RegionId = inputdata.RegionId;

                                franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                                franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
                                franchiseeMasterTrxDetail.FeesDetail = true;
                                franchiseeMasterTrxDetail.TaxDetail = false;
                                franchiseeMasterTrxDetail.TotalFee = totalFees;
                                franchiseeMasterTrxDetail.Total = paymentMinusFees;

                                franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                franchiseeMasterTrxDetail.IsDelete = false;
                                franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                                franchiseeMasterTrxDetail.TypelistId = 2; // customer
                                franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
                                franchiseeMasterTrxDetail.Transactiondate = inputdata.TransactionDate;



                                franchiseeMasterTrxDetail.BPPAdmin = 1;
                                franchiseeMasterTrxDetail.AccountRebate = 1;
                                franchiseeMasterTrxDetail.AccountRebate = 1;
                                franchiseeMasterTrxDetail.Commission = false;
                                franchiseeMasterTrxDetail.CommissionTotal = 0;
                                franchiseeMasterTrxDetail.FRRevenues = false;
                                franchiseeMasterTrxDetail.FRDeduction = false;
                                franchiseeMasterTrxDetail.DetailDescription = inputdata.Notes;
                                List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                                if (_masterINVDetail.Count > 0)
                                {
                                    franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                    franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                    franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                    //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                    franchiseeMasterTrxDetail.Quantity = 0;
                                    franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                    franchiseeMasterTrxDetail.TotalTax = 0;
                                    franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                    franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                    franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                    franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                    franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                }

                                context.PaymentTempDetails.Add(franchiseeMasterTrxDetail);
                                context.SaveChanges();


                                // insert franchisee fees

                                foreach (PaymentTempDetailFee feeDetail in franchiseeFees)
                                {
                                    feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.PaymentTempDetailId; // set the id after insertion

                                    context.PaymentTempDetailFees.Add(feeDetail);
                                    context.SaveChanges();
                                }


                            }
                        }
                        else
                        {
                            ivm.OverflowAmount = ivm.OverflowAmount - Math.Abs(ivm.InvoicePayment);
                        }

                        if (ivm.OverflowAmount > 0)
                        {
                            PaymentTempDetail masterTrxDetailOP = new PaymentTempDetail();
                            masterTrxDetailOP.MasterTrxId = customerMasterTrx.PaymentTempMasterTrxId;
                            masterTrxDetailOP.InvoiceId = ivm.InvoiceId;
                            masterTrxDetailOP.MasterTrxTypeListId = 17; // customer payment
                            masterTrxDetailOP.HeaderId = customerPayment.PaymentTempId;
                            masterTrxDetailOP.RegionId = inputdata.RegionId;
                            masterTrxDetailOP.AmountTypeListId = 1; // credit
                            masterTrxDetailOP.FeesDetail = false;
                            masterTrxDetailOP.TaxDetail = true;
                            masterTrxDetailOP.TotalTax = 0;
                            masterTrxDetailOP.TotalFee = 0;
                            masterTrxDetailOP.Quantity = 1;
                            masterTrxDetailOP.UnitPrice = ivm.OverflowAmount;
                            masterTrxDetailOP.Total = ivm.OverflowAmount;
                            masterTrxDetailOP.ExtendedPrice = ivm.OverflowAmount;
                            masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                            masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                            masterTrxDetailOP.IsDelete = false;
                            masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
                            masterTrxDetailOP.TypelistId = 1; // customer
                            masterTrxDetailOP.ClassId = ivm.InvoiceCustomerId;
                            masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                            context.PaymentTempDetails.Add(masterTrxDetailOP);
                            context.SaveChanges();
                        }
                    }
                }







                return true;
            }
        }

        public bool InsertManualPaymentTransactionUpdated(FullManualPaymentViewModel inputdata)
        {
            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();

            JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(2, inputdata.RegionId);
            franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(7, inputdata.RegionId);

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var PR = context.Periods.SingleOrDefault(p => p.BillMonth == inputdata.TransactionDate.Month && p.BillYear == inputdata.TransactionDate.Year);

                List<int> lstCust = inputdata.Invoices.Select(g => g.InvoiceCustomerId).Distinct().ToList<int>();



                MasterTrx customerMasterTrx = new MasterTrx();
                customerMasterTrx.TypeListId = 1; // customer
                customerMasterTrx.ClassId = inputdata.CustomerId;
                customerMasterTrx.MasterTrxTypeListId = 2; // customer payment
                customerMasterTrx.TrxDate = inputdata.TransactionDate;
                customerMasterTrx.RegionId = inputdata.RegionId;
                customerMasterTrx.StatusId = 1; // open
                customerMasterTrx.BillMonth = PR.BillMonth;
                customerMasterTrx.BillYear = PR.BillYear;
                customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                customerMasterTrx.CreatedBy = LoginUserId;
                customerMasterTrx.CreatedDate = DateTime.Now;

                //customerMasterTrx.AccountTypeListId;              
                //customerMasterTrx.HeaderId;

                context.MasterTrxes.Add(customerMasterTrx);
                context.SaveChanges();

                int InvIdT = 0;
                foreach (var _oCustId in lstCust)
                {

                    Payment customerPayment = new Payment();
                    customerPayment.MasterTrxId = customerMasterTrx.MasterTrxId;
                    customerPayment.TypeListId = 1; // customer
                    customerPayment.ClassId = _oCustId;
                    customerPayment.RegionId = inputdata.RegionId;
                    customerPayment.PaymentMethodListId = inputdata.PaymentMethodListId;
                    customerPayment.PaymentNo = inputdata.ReferenceNo;
                    customerPayment.TransactionStatusListId = 1;// allPaidInFull ? 6 : 7; // paid : paid partial
                    customerPayment.TempTransactionStatusListId = 3;// allPaidInFull ? 6 : 7; // paid : paid partial
                    customerPayment.CheckAmount = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;

                    //GET INVOICE NUMBER FOR Payment Number 
                    InvIdT = inputdata.Invoices.Where(p => p.InvoiceCustomerId == _oCustId).FirstOrDefault().InvoiceId;
                    Invoice _invoiceOject = context.Invoices.Where(o => o.InvoiceId == InvIdT).FirstOrDefault();

                    customerPayment.TransactionNumber = "PMT" + customerTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", customerMasterTrx.BillMonth) + (customerTransactionNumberConfigViewModel.LastNumber + 1).ToString();
                    // customerPayment.TransactionNumber = "PMT" + _invoiceOject.InvoiceNo.Trim(); // customerNextTrxNumber;

                    customerPayment.TransactionDate = inputdata.TransactionDate;
                    customerPayment.IsDelete = false;
                    customerPayment.CreatedBy = inputdata.CreatedBy;
                    customerPayment.CreatedBy = inputdata.CreatedBy;
                    customerPayment.ModifiedBy = inputdata.CreatedBy;
                    customerPayment.ModifiedDate = inputdata.CreatedDate;
                    customerPayment.PaymentDescription = inputdata.Notes;
                    customerPayment.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerPayment.CheckAmount = inputdata.PaymentAmount;
                    context.Payments.Add(customerPayment);
                    context.SaveChanges();

                    customerMasterTrx.HeaderId = customerPayment.PaymentId;
                    context.SaveChanges();

                    if (customerTransactionNumberConfigViewModel != null)
                    {
                        customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                    }


                    decimal totalCustomerPayment = 0;
                    decimal totalCustomerTaxes = 0;

                    int custMasterTrxDetailId = 0;
                    foreach (MPInvoiceViewModel ivm in inputdata.Invoices.Where(p => p.InvoiceCustomerId == _oCustId))
                    {
                        if (ivm.InvoicePayment > 0)
                        {
                            foreach (ManualPaymentViewModel cvm in ivm.CustomerPayment.Payments)
                            {
                                if (cvm.ExtendedPrice > 0)
                                {
                                    MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                                    masterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId;
                                    masterTrxDetail.InvoiceId = ivm.InvoiceId;
                                    masterTrxDetail.LineNo = cvm.LineNo;
                                    masterTrxDetail.MasterTrxTypeListId = 2; // customer payment
                                    masterTrxDetail.HeaderId = customerPayment.PaymentId;
                                    masterTrxDetail.RegionId = inputdata.RegionId;
                                    masterTrxDetail.AmountTypeListId = 1; // credit
                                    masterTrxDetail.FeesDetail = false;
                                    masterTrxDetail.TaxDetail = true;
                                    masterTrxDetail.TotalTax = Math.Round(cvm.Tax,2);
                                    masterTrxDetail.Total = cvm.PaymentAmount;
                                    masterTrxDetail.ExtendedPrice = cvm.ExtendedPrice;
                                    masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                    masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                    masterTrxDetail.IsDelete = false;
                                    //masterTrxDetail.ServiceTypeListId =
                                    masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    masterTrxDetail.TypelistId = 1; // customer
                                    masterTrxDetail.ClassId = _oCustId;
                                    masterTrxDetail.Transactiondate = inputdata.TransactionDate;
                                    masterTrxDetail.BPPAdmin = 1;
                                    masterTrxDetail.AccountRebate = 1;
                                    masterTrxDetail.AccountRebate = 1;
                                    masterTrxDetail.Commission = false;
                                    masterTrxDetail.CommissionTotal = 0;
                                    masterTrxDetail.FRRevenues = false;
                                    masterTrxDetail.FRDeduction = false;
                                    masterTrxDetail.DetailDescription = inputdata.Notes;

                                    masterTrxDetail.UnitPrice = 0;
                                    masterTrxDetail.CPIPercentage = 0;
                                    masterTrxDetail.TotalFee = 0;

                                    masterTrxDetail.SourceId = 0;
                                    masterTrxDetail.SourceTypeListId = 0;
                                    masterTrxDetail.ServiceTypeListId = 0;
                                    //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                    masterTrxDetail.Quantity = 0;
                                    masterTrxDetail.CPIPercentage = 0;
                                    masterTrxDetail.TotalFee = 0;
                                    masterTrxDetail.Commission = false;
                                    masterTrxDetail.CommissionTotal = 0;
                                    masterTrxDetail.ExtraWork = 0;
                                    masterTrxDetail.ReSell = false;
                                    masterTrxDetail.ClientSupplies = false;

                                    List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                                    if (_masterINVDetail.Count > 0)
                                    {
                                        masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                        masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                        masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                        //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                        masterTrxDetail.Quantity = 0;
                                        masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                        masterTrxDetail.TotalFee = 0;
                                        masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                        masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                        masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                        masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                        masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                    }



                                    context.MasterTrxDetails.Add(masterTrxDetail);
                                    context.SaveChanges();


                                    custMasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                                    // insert customer taxes

                                    MasterTrxTax customerTax = new MasterTrxTax();
                                    customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                                    customerTax.Amount = cvm.Tax;
                                    customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.PaymentAmount, 2);
                                    customerTax.AmountTypeListId = 1; // credit
                                    customerTax.CreatedBy = inputdata.CreatedBy;
                                    customerTax.CreatedDate = inputdata.CreatedDate;

                                    customerTax.InvoiceId = ivm.InvoiceId;
                                    customerTax.RegionId = inputdata.RegionId;
                                    customerTax.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    customerTax.CustomerId = _oCustId;
                                    customerTax.FRRevenues = false;
                                    customerTax.FRDeduction = false;
                                    //customerTax.FranchiseeId = null;
                                    context.MasterTrxTaxes.Add(customerTax);
                                    context.SaveChanges();

                                    totalCustomerPayment += cvm.Total;
                                    totalCustomerTaxes += cvm.Tax;

                                    var invoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
                                    var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                                    var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

                                    List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
                                    decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
                                    decimal totalTransactions = 0.00m;
                                    decimal grandTotalTransactions = 0.00m;

                                    foreach (var trx in invoiceTransactions)
                                    {
                                        totalTransactions = totalTransactions + (decimal)trx.Total;
                                    }
                                    grandTotalTransactions = totalTransactions + cvm.PaymentAmount;


                                    if (grandTotalTransactions >= invoiceTotal)
                                    {
                                        invoice.TransactionStatusListId = 6; /*6 = Paid*/
                                        invoiceMasterTrx.StatusId = 6;
                                    }
                                    else
                                    {
                                        invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
                                        invoiceMasterTrx.StatusId = 7;
                                    }
                                    context.SaveChanges();
                                }

                            }
                            foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
                            {
                                // compute franchisee payment fees

                                decimal totalFees = 0;

                                List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
                                List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

                                foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                                {
                                    MasterTrxFeeDetail feeDetail = new MasterTrxFeeDetail();
                                    if (feeDef.FeePercentage != null) // percentage
                                    {
                                        feeDetail.FeePercentage = feeDef.FeePercentage;
                                        feeDetail.Amount = (decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M);// Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
                                    }
                                    else // flat amount
                                    {
                                        feeDetail.Amount = feeDef.Amount;
                                        feeDetail.FeePercentage = null;
                                    }
                                    feeDetail.FeeId = feeDef.FeeId;
                                    feeDetail.AmountTypeListId = 1; // credit
                                    feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                                    feeDetail.CreatedBy = inputdata.CreatedBy;
                                    feeDetail.CreatedDate = inputdata.CreatedDate;
                                    feeDetail.FranchiseeId = fcvm.FranchiseeId;
                                    feeDetail.BillingPayId = fcvm.BillingPayId;
                                    totalFees += feeDetail.Amount ?? 0;

                                    franchiseeFees.Add(feeDetail);
                                }

                                // franchisee payment mastertrx

                                MasterTrx franchiseeMasterTrx = new MasterTrx();
                                franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
                                franchiseeMasterTrx.TypeListId = 2; // franchisee
                                franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
                                franchiseeMasterTrx.TrxDate = inputdata.TransactionDate;
                                franchiseeMasterTrx.RegionId = inputdata.RegionId;
                                franchiseeMasterTrx.StatusId = 1; // open

                                franchiseeMasterTrx.BillMonth = inputdata.TransactionDate.Month;
                                franchiseeMasterTrx.BillYear = inputdata.TransactionDate.Year;
                                franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                                franchiseeMasterTrx.CreatedBy = LoginUserId;
                                franchiseeMasterTrx.CreatedDate = DateTime.Now;
                                franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                                context.MasterTrxes.Add(franchiseeMasterTrx);
                                context.SaveChanges();


                                // franchisee payment

                                string franchiseeNextTrxNumber = franchiseeTransactionNumberConfigViewModel.Prefix.Trim() + franchiseeTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", inputdata.TransactionDate.Month) + (franchiseeTransactionNumberConfigViewModel.LastNumber + 1).ToString();
                                //string franchiseeNextTrxNumber = "BILPMT" + _invoiceOject.InvoiceNo.Trim(); ;



                                PaymentBillingFranchisee franchiseePayment = new PaymentBillingFranchisee();
                                franchiseePayment.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                                franchiseePayment.PaymentId = customerPayment.PaymentId;
                                franchiseePayment.FranchiseeId = fcvm.FranchiseeId;
                                franchiseePayment.BillingPayId = fcvm.BillingPayId;
                                franchiseePayment.RegionId = inputdata.RegionId;
                                franchiseePayment.LineNo = Convert.ToString(fcvm.Payment.LineNo);
                                franchiseePayment.Amount = fcvm.Payment.PaymentAmount;
                                franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
                                franchiseePayment.TransactionDate = inputdata.TransactionDate;
                                franchiseePayment.CreatedBy = inputdata.CreatedBy;
                                franchiseePayment.CreatedDate = inputdata.CreatedDate;
                                franchiseePayment.IsTARPaid = fcvm.IsTARPaid;
                                franchiseePayment.IsTurnaroundPayment = fcvm.IsTurnAroundPayment;
                                franchiseePayment.TransactionStatusListId = 1;
                                franchiseePayment.PeriodId = (PR != null ? PR.PeriodId : 0);
                                context.PaymentBillingFranchisees.Add(franchiseePayment);
                                context.SaveChanges();

                                franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
                                context.SaveChanges();

                                if (franchiseeTransactionNumberConfigViewModel != null)
                                {
                                    franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
                                    CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                                }


                                decimal paymentMinusFees = (decimal)(fcvm.Payment.PaymentAmount - totalFees);// Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

                                MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
                                franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                                franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
                                franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
                                franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
                                franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentBillingFranchiseeId; //customerPayment.PaymentId;
                                franchiseeMasterTrxDetail.RegionId = inputdata.RegionId;

                                franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                                franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
                                franchiseeMasterTrxDetail.FeesDetail = true;
                                franchiseeMasterTrxDetail.TaxDetail = false;
                                franchiseeMasterTrxDetail.TotalFee = totalFees;
                                franchiseeMasterTrxDetail.Total = paymentMinusFees;

                                franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                franchiseeMasterTrxDetail.IsDelete = false;
                                franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                                franchiseeMasterTrxDetail.TypelistId = 2; // customer
                                franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
                                franchiseeMasterTrxDetail.Transactiondate = inputdata.TransactionDate;



                                franchiseeMasterTrxDetail.BPPAdmin = 1;
                                franchiseeMasterTrxDetail.AccountRebate = 1;
                                franchiseeMasterTrxDetail.AccountRebate = 1;
                                franchiseeMasterTrxDetail.Commission = false;
                                franchiseeMasterTrxDetail.CommissionTotal = 0;
                                franchiseeMasterTrxDetail.FRRevenues = false;
                                franchiseeMasterTrxDetail.FRDeduction = false;
                                franchiseeMasterTrxDetail.DetailDescription = inputdata.Notes;
                                List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                                if (_masterINVDetail.Count > 0)
                                {
                                    franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                    franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                    franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                    //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                    franchiseeMasterTrxDetail.Quantity = 0;
                                    franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                    franchiseeMasterTrxDetail.TotalTax = 0;
                                    franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                    franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                    franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                    franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                    franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                }

                                context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                                context.SaveChanges();


                                // insert franchisee fees

                                foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
                                {
                                    feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

                                    context.MasterTrxFeeDetails.Add(feeDetail);
                                    context.SaveChanges();
                                }


                            }
                        }
                        else
                        {
                            ivm.OverflowAmount = ivm.OverflowAmount - Math.Abs(ivm.InvoicePayment);
                        }

                        if (ivm.OverflowAmount > 0)
                        {
                            MasterTrxDetail masterTrxDetailOP = new MasterTrxDetail();
                            masterTrxDetailOP.MasterTrxId = customerMasterTrx.MasterTrxId;
                            masterTrxDetailOP.InvoiceId = ivm.InvoiceId;
                            masterTrxDetailOP.MasterTrxTypeListId = 17; // customer payment
                            masterTrxDetailOP.HeaderId = customerPayment.PaymentId;
                            masterTrxDetailOP.RegionId = inputdata.RegionId;
                            masterTrxDetailOP.AmountTypeListId = 1; // credit
                            masterTrxDetailOP.FeesDetail = false;
                            masterTrxDetailOP.TaxDetail = true;
                            masterTrxDetailOP.TotalTax = 0;
                            masterTrxDetailOP.TotalFee = 0;
                            masterTrxDetailOP.Quantity = 1;
                            masterTrxDetailOP.UnitPrice = ivm.OverflowAmount;
                            masterTrxDetailOP.Total = ivm.OverflowAmount;
                            masterTrxDetailOP.ExtendedPrice = ivm.OverflowAmount;
                            masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                            masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                            masterTrxDetailOP.IsDelete = false;
                            masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
                            masterTrxDetailOP.TypelistId = 1; // customer
                            masterTrxDetailOP.ClassId = ivm.InvoiceCustomerId;
                            masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                            context.MasterTrxDetails.Add(masterTrxDetailOP);
                            context.SaveChanges();
                        }

                        ////update original invoice's status 
                        //Invoice existingInvoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
                        //if (existingInvoice != null)
                        //{
                        //    existingInvoice.TransactionStatusListId = ivm.PaidInFull ? 6 : 7; // paid : paid partial
                        //    existingInvoice.ModifiedBy = inputdata.CreatedBy;
                        //    existingInvoice.ModifiedDate = inputdata.CreatedDate;
                        //    context.Entry(existingInvoice).State = EntityState.Modified;
                        //    context.SaveChanges();
                        //}
                    }
                    //if ((totalCustomerPayment + totalCustomerTaxes) > 0)
                    //{
                    //    // general ledger for customer payment -- debit from A/R Janitorial Services
                    //    GeneralLedgerTrx oGeneralLedgerTrx = new GeneralLedgerTrx();

                    //    oGeneralLedgerTrx.Amount = totalCustomerPayment + totalCustomerTaxes;
                    //    oGeneralLedgerTrx.AmountTypeListId = 2;
                    //    oGeneralLedgerTrx.CreatedBy = LoginUserId;
                    //    oGeneralLedgerTrx.CreatedDate = DateTime.Now;
                    //    oGeneralLedgerTrx.Credit = 0;
                    //    oGeneralLedgerTrx.Debit = totalCustomerPayment + totalCustomerTaxes;
                    //    oGeneralLedgerTrx.IsDelete = false;
                    //    oGeneralLedgerTrx.LedgerAccountId = 151;//1193 : Payment (151) = > 1101 : Cash - Special Trust(1)
                    //    oGeneralLedgerTrx.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //    oGeneralLedgerTrx.MasterTrxTypeListId = 2;
                    //    oGeneralLedgerTrx.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                    //    oGeneralLedgerTrx.RegionId = inputdata.RegionId;
                    //    oGeneralLedgerTrx.TrxDate = inputdata.TransactionDate;
                    //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
                    //    context.SaveChanges();


                    //    // general ledger for customer payment -- debit from A/R Janitorial Services
                    //    GeneralLedgerTrx oGeneralLedgerTrxM = new GeneralLedgerTrx();

                    //    oGeneralLedgerTrx.Amount = totalCustomerPayment + totalCustomerTaxes;
                    //    oGeneralLedgerTrx.AmountTypeListId = 1;
                    //    oGeneralLedgerTrx.CreatedBy = LoginUserId;
                    //    oGeneralLedgerTrx.CreatedDate = DateTime.Now;
                    //    oGeneralLedgerTrx.Credit = totalCustomerPayment + totalCustomerTaxes;
                    //    oGeneralLedgerTrx.Debit = 0;
                    //    oGeneralLedgerTrx.IsDelete = false;
                    //    oGeneralLedgerTrx.LedgerAccountId = 3;
                    //    oGeneralLedgerTrx.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //    oGeneralLedgerTrx.MasterTrxTypeListId = 2;
                    //    oGeneralLedgerTrx.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                    //    oGeneralLedgerTrx.RegionId = inputdata.RegionId;
                    //    oGeneralLedgerTrx.TrxDate = inputdata.TransactionDate;
                    //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
                    //    context.SaveChanges();
                    //}


                }





                //Comment because insert by post functionality
                ////Checkbook
                //if (inputdata.PaymentAmount > 0)
                //{
                //    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                //    {
                //        var parmas = new DynamicParameters();
                //        parmas.Add("@TransactionDate", inputdata.TransactionDate);
                //        parmas.Add("@RegionId", inputdata.RegionId);
                //        parmas.Add("@CreatedBy", LoginUserId);
                //        parmas.Add("@CustomerId", inputdata.CustomerId);
                //        parmas.Add("@ApplyAmount", inputdata.PaymentAmount);
                //        parmas.Add("@ReferenceNo", inputdata.ReferenceNo);
                //        parmas.Add("@Notes", inputdata.Notes);
                //        parmas.Add("@CallFromTemp", true);
                //        using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_CheckBookInsert", parmas, commandType: CommandType.StoredProcedure)) { }
                //    }


                //    PaymentMethodList oPaymentMethodList = context.PaymentMethodLists.FirstOrDefault(o => o.PaymentMethodListId == inputdata.PaymentMethodListId);

                //    // general ledger for customer payment -- debit from A/R Janitorial Services
                //    GeneralLedgerTrx oGeneralLedgerTrx = new GeneralLedgerTrx();

                //    oGeneralLedgerTrx.Amount = inputdata.PaymentAmount;
                //    oGeneralLedgerTrx.AmountTypeListId = 1;
                //    oGeneralLedgerTrx.CreatedBy = LoginUserId;
                //    oGeneralLedgerTrx.CreatedDate = DateTime.Now;
                //    oGeneralLedgerTrx.Credit = inputdata.PaymentAmount;
                //    oGeneralLedgerTrx.Debit = 0;
                //    oGeneralLedgerTrx.IsDelete = false;
                //    oGeneralLedgerTrx.LedgerAccountId = 151;//1193 : Payment (151) = > 1101 : Cash - Special Trust(1)
                //                                            //oGeneralLedgerTrx.MasterTrxId =null;

                //    oGeneralLedgerTrx.Description = "Manual Payment with "+ oPaymentMethodList.Name +(String.IsNullOrEmpty(inputdata.ReferenceNo)?"":(" "+ inputdata.ReferenceNo));

                //    oGeneralLedgerTrx.MasterTrxTypeListId = 2;
                //    oGeneralLedgerTrx.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                //    oGeneralLedgerTrx.RegionId = inputdata.RegionId;
                //    oGeneralLedgerTrx.TrxDate = inputdata.TransactionDate;
                //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
                //    context.SaveChanges();


                //    // general ledger for customer payment -- debit from A/R Janitorial Services
                //    GeneralLedgerTrx oGeneralLedgerTrxCash = new GeneralLedgerTrx();

                //    oGeneralLedgerTrxCash.Amount = inputdata.PaymentAmount;
                //    oGeneralLedgerTrxCash.AmountTypeListId = 2;
                //    oGeneralLedgerTrxCash.CreatedBy = LoginUserId;
                //    oGeneralLedgerTrxCash.CreatedDate = DateTime.Now;
                //    oGeneralLedgerTrxCash.Credit = 0;
                //    oGeneralLedgerTrxCash.Debit = inputdata.PaymentAmount;
                //    oGeneralLedgerTrxCash.IsDelete = false;
                //    oGeneralLedgerTrxCash.LedgerAccountId = 1;
                //    oGeneralLedgerTrx.Description = "Manual Payment with " + oPaymentMethodList.Name + (String.IsNullOrEmpty(inputdata.ReferenceNo) ? "" : (" " + inputdata.ReferenceNo));
                //    //oGeneralLedgerTrxCashrx.MasterTrxId =null;
                //    oGeneralLedgerTrxCash.MasterTrxTypeListId = 2;
                //    oGeneralLedgerTrxCash.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                //    oGeneralLedgerTrxCash.RegionId = inputdata.RegionId;
                //    oGeneralLedgerTrxCash.TrxDate = inputdata.TransactionDate;
                //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
                //    context.SaveChanges();

                //}






                return true;
            }
        }

        //public bool InsertManualPaymentTransactionUpdated(FullManualPaymentViewModel inputdata)
        //{
        //    JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();

        //    JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
        //    JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

        //    customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(2, inputdata.RegionId);
        //    franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(7, inputdata.RegionId);

        //    using (jkDatabaseEntities context = new jkDatabaseEntities())
        //    {
        //        var PR = context.Periods.SingleOrDefault(p => p.BillMonth == inputdata.TransactionDate.Month && p.BillYear == inputdata.TransactionDate.Year);
        //        MasterTrx customerMasterTrx = new MasterTrx();
        //        customerMasterTrx.TypeListId = 1; // customer
        //        customerMasterTrx.ClassId = inputdata.CustomerId;
        //        customerMasterTrx.MasterTrxTypeListId = 2; // customer payment
        //        customerMasterTrx.TrxDate = inputdata.TransactionDate;
        //        customerMasterTrx.RegionId = inputdata.RegionId;
        //        customerMasterTrx.StatusId = 1; // open
        //        customerMasterTrx.BillMonth = PR.BillMonth;
        //        customerMasterTrx.BillYear = PR.BillYear;
        //        customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        customerMasterTrx.CreatedBy = LoginUserId;
        //        customerMasterTrx.CreatedDate = DateTime.Now;

        //        //customerMasterTrx.AccountTypeListId;              
        //        //customerMasterTrx.HeaderId;

        //        context.MasterTrxes.Add(customerMasterTrx);
        //        context.SaveChanges();


        //        Payment customerPayment = new Payment();
        //        customerPayment.MasterTrxId = customerMasterTrx.MasterTrxId;
        //        customerPayment.TypeListId = 1; // customer
        //        customerPayment.ClassId = inputdata.CustomerId;
        //        customerPayment.RegionId = inputdata.RegionId;
        //        customerPayment.PaymentMethodListId = inputdata.PaymentMethodListId;
        //        customerPayment.PaymentNo = inputdata.ReferenceNo;
        //        customerPayment.TransactionStatusListId = 1;// allPaidInFull ? 6 : 7; // paid : paid partial
        //        customerPayment.TempTransactionStatusListId = 3;// allPaidInFull ? 6 : 7; // paid : paid partial
        //        customerPayment.CheckAmount = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;

        //        //GET INVOICE NUMBER FOR Payment Number 
        //        int InvIdT = inputdata.Invoices[0].InvoiceId;
        //        Invoice _invoiceOject = context.Invoices.Where(o => o.InvoiceId == InvIdT).FirstOrDefault();

        //        customerPayment.TransactionNumber = "PMT" + customerTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", customerMasterTrx.BillMonth) + (customerTransactionNumberConfigViewModel.LastNumber + 1).ToString();
        //        // customerPayment.TransactionNumber = "PMT" + _invoiceOject.InvoiceNo.Trim(); // customerNextTrxNumber;

        //        customerPayment.TransactionDate = inputdata.TransactionDate;
        //        customerPayment.IsDelete = false;
        //        customerPayment.CreatedBy = inputdata.CreatedBy;
        //        customerPayment.CreatedBy = inputdata.CreatedBy;
        //        customerPayment.ModifiedBy = inputdata.CreatedBy;
        //        customerPayment.ModifiedDate = inputdata.CreatedDate;
        //        customerPayment.PaymentDescription = inputdata.Notes;
        //        customerPayment.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        customerPayment.CheckAmount = inputdata.PaymentAmount;
        //        context.Payments.Add(customerPayment);
        //        context.SaveChanges();

        //        customerMasterTrx.HeaderId = customerPayment.PaymentId;
        //        context.SaveChanges();


        //        if (customerTransactionNumberConfigViewModel != null)
        //        {
        //            customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
        //            CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
        //        }

        //        //Checkbook
        //        if (inputdata.PaymentAmount > 0)
        //        {
        //            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
        //            {
        //                var parmas = new DynamicParameters();
        //                parmas.Add("@TransactionDate", inputdata.TransactionDate);
        //                parmas.Add("@RegionId", inputdata.RegionId);
        //                parmas.Add("@CreatedBy", LoginUserId);
        //                parmas.Add("@CustomerId", inputdata.CustomerId);
        //                parmas.Add("@ApplyAmount", inputdata.PaymentAmount);
        //                parmas.Add("@ReferenceNo", inputdata.ReferenceNo);
        //                parmas.Add("@Notes", inputdata.Notes);
        //                parmas.Add("@CallFromTemp", true);
        //                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_CheckBookInsert", parmas, commandType: CommandType.StoredProcedure)) { }
        //            }
        //        }


        //        decimal totalCustomerPayment = 0;
        //        decimal totalCustomerTaxes = 0;

        //        int custMasterTrxDetailId = 0;
        //        foreach (MPInvoiceViewModel ivm in inputdata.Invoices)
        //        {
        //            if (ivm.InvoicePayment > 0)
        //            {
        //                foreach (ManualPaymentViewModel cvm in ivm.CustomerPayment.Payments)
        //                {
        //                    if (cvm.ExtendedPrice > 0)
        //                    {
        //                        MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
        //                        masterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId;
        //                        masterTrxDetail.InvoiceId = ivm.InvoiceId;
        //                        masterTrxDetail.LineNo = cvm.LineNo;
        //                        masterTrxDetail.MasterTrxTypeListId = 2; // customer payment
        //                        masterTrxDetail.HeaderId = customerPayment.PaymentId;
        //                        masterTrxDetail.RegionId = inputdata.RegionId;
        //                        masterTrxDetail.AmountTypeListId = 1; // credit
        //                        masterTrxDetail.FeesDetail = false;
        //                        masterTrxDetail.TaxDetail = true;
        //                        masterTrxDetail.TotalTax = cvm.Tax;
        //                        masterTrxDetail.Total = cvm.PaymentAmount;
        //                        masterTrxDetail.ExtendedPrice = cvm.ExtendedPrice;
        //                        masterTrxDetail.CreatedBy = inputdata.CreatedBy;
        //                        masterTrxDetail.CreatedDate = inputdata.CreatedDate;
        //                        masterTrxDetail.IsDelete = false;
        //                        //masterTrxDetail.ServiceTypeListId =
        //                        masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                        masterTrxDetail.TypelistId = 1; // customer
        //                        masterTrxDetail.ClassId = inputdata.CustomerId;
        //                        masterTrxDetail.Transactiondate = inputdata.TransactionDate;
        //                        masterTrxDetail.BPPAdmin = 1;
        //                        masterTrxDetail.AccountRebate = 1;
        //                        masterTrxDetail.AccountRebate = 1;
        //                        masterTrxDetail.Commission = false;
        //                        masterTrxDetail.CommissionTotal = 0;
        //                        masterTrxDetail.FRRevenues = false;
        //                        masterTrxDetail.FRDeduction = false;
        //                        masterTrxDetail.DetailDescription = inputdata.Notes;

        //                        masterTrxDetail.UnitPrice = 0;
        //                        masterTrxDetail.CPIPercentage = 0;
        //                        masterTrxDetail.TotalFee = 0;

        //                        masterTrxDetail.SourceId = 0;
        //                        masterTrxDetail.SourceTypeListId = 0;
        //                        masterTrxDetail.ServiceTypeListId = 0;
        //                        //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //                        masterTrxDetail.Quantity = 0;
        //                        masterTrxDetail.CPIPercentage = 0;
        //                        masterTrxDetail.TotalFee = 0;
        //                        masterTrxDetail.Commission = false;
        //                        masterTrxDetail.CommissionTotal = 0;
        //                        masterTrxDetail.ExtraWork = 0;
        //                        masterTrxDetail.ReSell = false;
        //                        masterTrxDetail.ClientSupplies = false;

        //                        List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

        //                        if (_masterINVDetail.Count > 0)
        //                        {
        //                            masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
        //                            masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
        //                            masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
        //                            //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //                            masterTrxDetail.Quantity = 0;
        //                            masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
        //                            masterTrxDetail.TotalFee = 0;
        //                            masterTrxDetail.Commission = _masterINVDetail[0].Commission;
        //                            masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
        //                            masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
        //                            masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
        //                            masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

        //                        }



        //                        context.MasterTrxDetails.Add(masterTrxDetail);
        //                        context.SaveChanges();


        //                        custMasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
        //                        // insert customer taxes

        //                        MasterTrxTax customerTax = new MasterTrxTax();
        //                        customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
        //                        customerTax.Amount = cvm.Tax;
        //                        customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.PaymentAmount, 2);
        //                        customerTax.AmountTypeListId = 1; // credit
        //                        customerTax.CreatedBy = inputdata.CreatedBy;
        //                        customerTax.CreatedDate = inputdata.CreatedDate;

        //                        customerTax.InvoiceId = ivm.InvoiceId;
        //                        customerTax.RegionId = inputdata.RegionId;
        //                        customerTax.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                        customerTax.CustomerId = inputdata.CustomerId;
        //                        customerTax.FRRevenues = false;
        //                        customerTax.FRDeduction = false;
        //                        //customerTax.FranchiseeId = null;
        //                        context.MasterTrxTaxes.Add(customerTax);
        //                        context.SaveChanges();

        //                        totalCustomerPayment += cvm.Total;
        //                        totalCustomerTaxes += cvm.Tax;

        //                        var invoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
        //                        var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
        //                        var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

        //                        List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
        //                        decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
        //                        decimal totalTransactions = 0.00m;
        //                        decimal grandTotalTransactions = 0.00m;

        //                        foreach (var trx in invoiceTransactions)
        //                        {
        //                            totalTransactions = totalTransactions + (decimal)trx.Total;
        //                        }
        //                        grandTotalTransactions = totalTransactions + cvm.PaymentAmount;


        //                        if (grandTotalTransactions >= invoiceTotal)
        //                        {
        //                            invoice.TransactionStatusListId = 6; /*6 = Paid*/
        //                            invoiceMasterTrx.StatusId = 6;
        //                        }
        //                        else
        //                        {
        //                            invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
        //                            invoiceMasterTrx.StatusId = 7;
        //                        }
        //                        context.SaveChanges();
        //                    }

        //                }
        //                foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
        //                {
        //                    // compute franchisee payment fees

        //                    decimal totalFees = 0;

        //                    List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
        //                    List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

        //                    foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
        //                    {
        //                        MasterTrxFeeDetail feeDetail = new MasterTrxFeeDetail();
        //                        if (feeDef.FeePercentage != null) // percentage
        //                        {
        //                            feeDetail.FeePercentage = feeDef.FeePercentage;
        //                            feeDetail.Amount = Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
        //                        }
        //                        else // flat amount
        //                        {
        //                            feeDetail.Amount = feeDef.Amount;
        //                            feeDetail.FeePercentage = null;
        //                        }
        //                        feeDetail.FeeId = feeDef.FeeId;
        //                        feeDetail.AmountTypeListId = 1; // credit
        //                        feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
        //                        feeDetail.CreatedBy = inputdata.CreatedBy;
        //                        feeDetail.CreatedDate = inputdata.CreatedDate;

        //                        totalFees += feeDetail.Amount ?? 0;

        //                        franchiseeFees.Add(feeDetail);
        //                    }

        //                    // franchisee payment mastertrx

        //                    MasterTrx franchiseeMasterTrx = new MasterTrx();
        //                    franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
        //                    franchiseeMasterTrx.TypeListId = 2; // franchisee
        //                    franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
        //                    franchiseeMasterTrx.TrxDate = inputdata.TransactionDate;
        //                    franchiseeMasterTrx.RegionId = inputdata.RegionId;
        //                    franchiseeMasterTrx.StatusId = 1; // open

        //                    franchiseeMasterTrx.BillMonth = inputdata.TransactionDate.Month;
        //                    franchiseeMasterTrx.BillYear = inputdata.TransactionDate.Year;
        //                    franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                    franchiseeMasterTrx.CreatedBy = LoginUserId;
        //                    franchiseeMasterTrx.CreatedDate = DateTime.Now;
        //                    franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                    context.MasterTrxes.Add(franchiseeMasterTrx);
        //                    context.SaveChanges();


        //                    // franchisee payment

        //                    string franchiseeNextTrxNumber = franchiseeTransactionNumberConfigViewModel.Prefix.Trim() + franchiseeTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", inputdata.TransactionDate.Month) + (franchiseeTransactionNumberConfigViewModel.LastNumber + 1).ToString();
        //                    //string franchiseeNextTrxNumber = "BILPMT" + _invoiceOject.InvoiceNo.Trim(); ;



        //                    PaymentBillingFranchisee franchiseePayment = new PaymentBillingFranchisee();
        //                    franchiseePayment.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
        //                    franchiseePayment.PaymentId = customerPayment.PaymentId;
        //                    franchiseePayment.FranchiseeId = fcvm.FranchiseeId;
        //                    franchiseePayment.BillingPayId = fcvm.BillingPayId;
        //                    franchiseePayment.RegionId = inputdata.RegionId;
        //                    franchiseePayment.LineNo = Convert.ToString(fcvm.Payment.LineNo);
        //                    franchiseePayment.Amount = fcvm.Payment.PaymentAmount;
        //                    franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
        //                    franchiseePayment.TransactionDate = inputdata.TransactionDate;
        //                    franchiseePayment.CreatedBy = inputdata.CreatedBy;
        //                    franchiseePayment.CreatedDate = inputdata.CreatedDate;
        //                    franchiseePayment.IsTARPaid = fcvm.IsTARPaid;
        //                    franchiseePayment.IsTurnaroundPayment = fcvm.IsTurnAroundPayment;
        //                    franchiseePayment.TransactionStatusListId = 1;
        //                    franchiseePayment.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                    context.PaymentBillingFranchisees.Add(franchiseePayment);
        //                    context.SaveChanges();

        //                    franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
        //                    context.SaveChanges();

        //                    if (franchiseeTransactionNumberConfigViewModel != null)
        //                    {
        //                        franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
        //                        CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
        //                    }


        //                    decimal paymentMinusFees = Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

        //                    MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
        //                    franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
        //                    franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
        //                    franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
        //                    franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
        //                    franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentBillingFranchiseeId; //customerPayment.PaymentId;
        //                    franchiseeMasterTrxDetail.RegionId = inputdata.RegionId;

        //                    franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
        //                    franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
        //                    franchiseeMasterTrxDetail.FeesDetail = true;
        //                    franchiseeMasterTrxDetail.TaxDetail = false;
        //                    franchiseeMasterTrxDetail.TotalFee = totalFees;
        //                    franchiseeMasterTrxDetail.Total = paymentMinusFees;

        //                    franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
        //                    franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
        //                    franchiseeMasterTrxDetail.IsDelete = false;
        //                    franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                    franchiseeMasterTrxDetail.TypelistId = 2; // customer
        //                    franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
        //                    franchiseeMasterTrxDetail.Transactiondate = inputdata.TransactionDate;



        //                    franchiseeMasterTrxDetail.BPPAdmin = 1;
        //                    franchiseeMasterTrxDetail.AccountRebate = 1;
        //                    franchiseeMasterTrxDetail.AccountRebate = 1;
        //                    franchiseeMasterTrxDetail.Commission = false;
        //                    franchiseeMasterTrxDetail.CommissionTotal = 0;
        //                    franchiseeMasterTrxDetail.FRRevenues = false;
        //                    franchiseeMasterTrxDetail.FRDeduction = false;
        //                    franchiseeMasterTrxDetail.DetailDescription = inputdata.Notes;
        //                    List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
        //                    if (_masterINVDetail.Count > 0)
        //                    {
        //                        franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
        //                        franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
        //                        franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
        //                        //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //                        franchiseeMasterTrxDetail.Quantity = 0;
        //                        franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
        //                        franchiseeMasterTrxDetail.TotalTax = 0;
        //                        franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
        //                        franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
        //                        franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
        //                        franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
        //                        franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

        //                    }

        //                    context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
        //                    context.SaveChanges();


        //                    // insert franchisee fees

        //                    foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
        //                    {
        //                        feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

        //                        context.MasterTrxFeeDetails.Add(feeDetail);
        //                        context.SaveChanges();
        //                    }


        //                }
        //            }
        //            else
        //            {
        //                ivm.OverflowAmount = ivm.OverflowAmount - Math.Abs(ivm.InvoicePayment);
        //            }

        //            if (ivm.OverflowAmount > 0)
        //            {
        //                MasterTrxDetail masterTrxDetailOP = new MasterTrxDetail();
        //                masterTrxDetailOP.MasterTrxId = customerMasterTrx.MasterTrxId;
        //                masterTrxDetailOP.InvoiceId = ivm.InvoiceId;
        //                masterTrxDetailOP.MasterTrxTypeListId = 17; // customer payment
        //                masterTrxDetailOP.HeaderId = customerPayment.PaymentId;
        //                masterTrxDetailOP.RegionId = inputdata.RegionId;
        //                masterTrxDetailOP.AmountTypeListId = 1; // credit
        //                masterTrxDetailOP.FeesDetail = false;
        //                masterTrxDetailOP.TaxDetail = true;
        //                masterTrxDetailOP.TotalTax = 0;
        //                masterTrxDetailOP.TotalFee = 0;
        //                masterTrxDetailOP.Quantity = 1;
        //                masterTrxDetailOP.UnitPrice = ivm.OverflowAmount;
        //                masterTrxDetailOP.Total = ivm.OverflowAmount;
        //                masterTrxDetailOP.ExtendedPrice = ivm.OverflowAmount;
        //                masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
        //                masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
        //                masterTrxDetailOP.IsDelete = false;
        //                masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
        //                masterTrxDetailOP.TypelistId = 1; // customer
        //                masterTrxDetailOP.ClassId = inputdata.CustomerId;
        //                masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
        //                context.MasterTrxDetails.Add(masterTrxDetailOP);
        //                context.SaveChanges();
        //            }

        //            ////update original invoice's status 
        //            //Invoice existingInvoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
        //            //if (existingInvoice != null)
        //            //{
        //            //    existingInvoice.TransactionStatusListId = ivm.PaidInFull ? 6 : 7; // paid : paid partial
        //            //    existingInvoice.ModifiedBy = inputdata.CreatedBy;
        //            //    existingInvoice.ModifiedDate = inputdata.CreatedDate;
        //            //    context.Entry(existingInvoice).State = EntityState.Modified;
        //            //    context.SaveChanges();
        //            //}
        //        }
        //        if ((totalCustomerPayment + totalCustomerTaxes) > 0)
        //        {
        //            // general ledger for customer payment -- debit from A/R Janitorial Services
        //            GeneralLedgerTrx oGeneralLedgerTrx = new GeneralLedgerTrx();

        //            oGeneralLedgerTrx.Amount = totalCustomerPayment + totalCustomerTaxes;
        //            oGeneralLedgerTrx.AmountTypeListId = 2;
        //            oGeneralLedgerTrx.CreatedBy = LoginUserId;
        //            oGeneralLedgerTrx.CreatedDate = DateTime.Now;
        //            oGeneralLedgerTrx.Credit = 0;
        //            oGeneralLedgerTrx.Debit = totalCustomerPayment + totalCustomerTaxes;
        //            oGeneralLedgerTrx.IsDelete = false;
        //            oGeneralLedgerTrx.LedgerAccountId = 151;
        //            oGeneralLedgerTrx.MasterTrxId = customerMasterTrx.MasterTrxId;
        //            oGeneralLedgerTrx.MasterTrxTypeListId = 2;
        //            oGeneralLedgerTrx.PeriodId = (PR != null ? PR.PeriodId : 0); ;
        //            oGeneralLedgerTrx.RegionId = inputdata.RegionId;
        //            oGeneralLedgerTrx.TrxDate = inputdata.TransactionDate;
        //            context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
        //            context.SaveChanges();


        //            // general ledger for customer payment -- debit from A/R Janitorial Services
        //            GeneralLedgerTrx oGeneralLedgerTrxM = new GeneralLedgerTrx();

        //            oGeneralLedgerTrx.Amount = totalCustomerPayment + totalCustomerTaxes;
        //            oGeneralLedgerTrx.AmountTypeListId = 1;
        //            oGeneralLedgerTrx.CreatedBy = LoginUserId;
        //            oGeneralLedgerTrx.CreatedDate = DateTime.Now;
        //            oGeneralLedgerTrx.Credit = totalCustomerPayment + totalCustomerTaxes;
        //            oGeneralLedgerTrx.Debit = 0;
        //            oGeneralLedgerTrx.IsDelete = false;
        //            oGeneralLedgerTrx.LedgerAccountId = 3;
        //            oGeneralLedgerTrx.MasterTrxId = customerMasterTrx.MasterTrxId;
        //            oGeneralLedgerTrx.MasterTrxTypeListId = 2;
        //            oGeneralLedgerTrx.PeriodId = (PR != null ? PR.PeriodId : 0); ;
        //            oGeneralLedgerTrx.RegionId = inputdata.RegionId;
        //            oGeneralLedgerTrx.TrxDate = inputdata.TransactionDate;
        //            context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
        //            context.SaveChanges();
        //        }



        //        return true;
        //    }
        //}
        public bool InsertManualPaymentTransaction(ManualPaymentTransactionViewModel inputdata)
        {
            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();

            JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(2, inputdata.RegionId);

            string customerNextTrxNumber = null;
            if (customerTransactionNumberConfigViewModel != null)
                customerNextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(2, inputdata.RegionId, inputdata.CreatedDate);

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {



                if (inputdata.overflowApplyCredit == true)
                {

                    Payment customerPayment = context.Payments.SingleOrDefault(p => p.PaymentId == inputdata.overflowSourceId);
                    if (customerPayment != null)
                    {
                        customerPayment.ModifiedBy = LoginUserId;
                        customerPayment.ModifiedDate = DateTime.Now;


                        MasterTrx customerMasterTrx = context.MasterTrxes.SingleOrDefault(p => p.MasterTrxId == customerPayment.MasterTrxId);
                        bool allPaidInFull = inputdata.Invoices.All(o => o.PaidInFull);


                        decimal totalCustomerPayment = 0;
                        decimal totalCustomerTaxes = 0;

                        int custMasterTrxDetailId = 0;

                        foreach (ManualPaymentInvoiceViewModel ivm in inputdata.Invoices)
                        {
                            foreach (ManualPaymentViewModel cvm in ivm.CustomerPayment.Payments)
                            {
                                MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                                masterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId;
                                masterTrxDetail.InvoiceId = ivm.InvoiceId;
                                masterTrxDetail.LineNo = cvm.LineNo;
                                masterTrxDetail.MasterTrxTypeListId = 2; // customer payment
                                masterTrxDetail.HeaderId = customerPayment.PaymentId;
                                masterTrxDetail.RegionId = inputdata.RegionId;
                                masterTrxDetail.AmountTypeListId = 1; // credit
                                masterTrxDetail.FeesDetail = false;
                                masterTrxDetail.TaxDetail = true;
                                masterTrxDetail.TotalTax = Math.Round(cvm.Tax,2);
                                masterTrxDetail.Total = cvm.PaymentAmount;
                                masterTrxDetail.ExtendedPrice = cvm.ExtendedPrice;
                                masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                masterTrxDetail.IsDelete = false;
                                masterTrxDetail.PeriodId = customerMasterTrx.PeriodId;
                                masterTrxDetail.TypelistId = 1; // customer
                                masterTrxDetail.ClassId = inputdata.CustomerId;
                                masterTrxDetail.Transactiondate = inputdata.TransactionDate;
                                masterTrxDetail.DetailDescription = inputdata.Notes;

                                List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                                if (_masterINVDetail.Count > 0)
                                {
                                    masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                    masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                    masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                    //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                    masterTrxDetail.Quantity = 0;
                                    masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                    masterTrxDetail.TotalFee = 0;
                                    masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                    masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                    masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                    masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                    masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                }
                                context.MasterTrxDetails.Add(masterTrxDetail);
                                context.SaveChanges();

                                custMasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                                // insert customer taxes

                                MasterTrxTax customerTax = new MasterTrxTax();
                                customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                                customerTax.Amount = cvm.Tax;
                                customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.PaymentAmount, 2);
                                customerTax.AmountTypeListId = 1; // credit
                                customerTax.CreatedBy = inputdata.CreatedBy;
                                customerTax.CreatedDate = inputdata.CreatedDate;
                                customerTax.InvoiceId = ivm.InvoiceId;
                                customerTax.RegionId = inputdata.RegionId;
                                customerTax.PeriodId = customerMasterTrx.PeriodId;
                                customerTax.CustomerId = inputdata.CustomerId;
                                //customerTax.FranchiseeId = null;
                                context.MasterTrxTaxes.Add(customerTax);
                                context.SaveChanges();

                                totalCustomerPayment += cvm.Total;
                                totalCustomerTaxes += cvm.Tax;

                                var invoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
                                var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                                var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

                                List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
                                decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
                                decimal totalTransactions = 0.00m;
                                decimal grandTotalTransactions = 0.00m;

                                foreach (var trx in invoiceTransactions)
                                {
                                    totalTransactions = totalTransactions + (decimal)trx.Total;
                                }
                                grandTotalTransactions = totalTransactions + cvm.PaymentAmount;


                                if (grandTotalTransactions >= invoiceTotal)
                                {
                                    invoice.TransactionStatusListId = 6; /*6 = Paid*/
                                    invoiceMasterTrx.StatusId = 6;
                                }
                                else
                                {
                                    invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
                                    invoiceMasterTrx.StatusId = 7;
                                }

                                context.SaveChanges();
                            }
                            foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
                            {
                                // compute franchisee payment fees

                                decimal totalFees = 0;

                                List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
                                List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

                                foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                                {
                                    MasterTrxFeeDetail feeDetail = new MasterTrxFeeDetail();
                                    if (feeDef.FeePercentage != null) // percentage
                                    {
                                        feeDetail.FeePercentage = feeDef.FeePercentage;
                                        feeDetail.Amount = (decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M);// Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
                                    }
                                    else // flat amount
                                    {
                                        feeDetail.Amount = feeDef.Amount;
                                        feeDetail.FeePercentage = null;
                                    }
                                    feeDetail.FeeId = feeDef.FeeId;
                                    feeDetail.AmountTypeListId = 1; // credit
                                    feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                                    feeDetail.CreatedBy = inputdata.CreatedBy;
                                    feeDetail.CreatedDate = inputdata.CreatedDate;
                                    feeDetail.FranchiseeId = fcvm.FranchiseeId;
                                    feeDetail.BillingPayId = fcvm.BillingPayId;
                                    totalFees += feeDetail.Amount ?? 0;

                                    franchiseeFees.Add(feeDetail);
                                }

                                // franchisee payment mastertrx

                                MasterTrx franchiseeMasterTrx = new MasterTrx();
                                franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
                                franchiseeMasterTrx.TypeListId = 2; // franchisee
                                franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
                                franchiseeMasterTrx.TrxDate = inputdata.TransactionDate;
                                franchiseeMasterTrx.RegionId = inputdata.RegionId;
                                franchiseeMasterTrx.StatusId = 3; // open

                                franchiseeMasterTrx.BillMonth = inputdata.TransactionDate.Month;
                                franchiseeMasterTrx.BillYear = inputdata.TransactionDate.Year;
                                franchiseeMasterTrx.PeriodId = customerMasterTrx.PeriodId;
                                franchiseeMasterTrx.CreatedBy = LoginUserId;
                                franchiseeMasterTrx.CreatedDate = DateTime.Now;

                                context.MasterTrxes.Add(franchiseeMasterTrx);
                                context.SaveChanges();


                                // franchisee payment

                                string franchiseeNextTrxNumber = null;

                                franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(7, inputdata.RegionId);
                                if (franchiseeTransactionNumberConfigViewModel != null)
                                    franchiseeNextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(7, inputdata.RegionId, inputdata.CreatedDate);

                                PaymentBillingFranchisee franchiseePayment = new PaymentBillingFranchisee();
                                franchiseePayment.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                                franchiseePayment.PaymentId = customerPayment.PaymentId;
                                franchiseePayment.FranchiseeId = fcvm.FranchiseeId;
                                franchiseePayment.BillingPayId = fcvm.BillingPayId;
                                franchiseePayment.RegionId = inputdata.RegionId;
                                franchiseePayment.LineNo = Convert.ToString(fcvm.Payment.LineNo);
                                franchiseePayment.Amount = fcvm.Payment.PaymentAmount;
                                franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
                                franchiseePayment.TransactionDate = inputdata.TransactionDate;
                                franchiseePayment.CreatedBy = inputdata.CreatedBy;
                                franchiseePayment.CreatedDate = inputdata.CreatedDate;
                                franchiseePayment.IsTARPaid = fcvm.IsTARPaid;
                                franchiseePayment.IsTurnaroundPayment = fcvm.IsTurnAroundPayment;
                                franchiseePayment.TransactionStatusListId = 1;
                                context.PaymentBillingFranchisees.Add(franchiseePayment);
                                context.SaveChanges();

                                franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
                                context.SaveChanges();

                                if (franchiseeTransactionNumberConfigViewModel != null)
                                {
                                    franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
                                    CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                                }


                                decimal paymentMinusFees = (decimal)(fcvm.Payment.PaymentAmount - totalFees); // Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

                                MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
                                franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                                franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
                                franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
                                franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
                                franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentBillingFranchiseeId; //customerPayment.PaymentId;
                                franchiseeMasterTrxDetail.RegionId = inputdata.RegionId;

                                franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                                franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
                                franchiseeMasterTrxDetail.FeesDetail = true;
                                franchiseeMasterTrxDetail.TaxDetail = false;
                                franchiseeMasterTrxDetail.TotalFee = totalFees;
                                franchiseeMasterTrxDetail.Total = paymentMinusFees;

                                franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                franchiseeMasterTrxDetail.IsDelete = false;
                                franchiseeMasterTrxDetail.PeriodId = customerMasterTrx.PeriodId;
                                franchiseeMasterTrxDetail.TypelistId = 2; // customer
                                franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
                                franchiseeMasterTrxDetail.Transactiondate = inputdata.TransactionDate;
                                franchiseeMasterTrxDetail.DetailDescription = inputdata.Notes;

                                List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                                if (_masterINVDetail.Count > 0)
                                {
                                    franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                    franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                    franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                    //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                    franchiseeMasterTrxDetail.Quantity = 0;
                                    franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                    franchiseeMasterTrxDetail.TotalTax = 0;
                                    franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                    franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                    franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                    franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                    franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                }

                                context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                                context.SaveChanges();


                                // insert franchisee fees

                                foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
                                {
                                    feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

                                    context.MasterTrxFeeDetails.Add(feeDetail);
                                    context.SaveChanges();
                                }


                            }
                        }

                        // deduct customer credit (amount taken from previous customer credit balance)

                        if (inputdata.CreditAmount > 0)
                        {
                            //MasterTrx masterTrx = new MasterTrx();
                            //masterTrx.TypeListId = 1; // customer
                            //masterTrx.ClassId = inputdata.CustomerId;
                            //masterTrx.MasterTrxTypeListId = 24; // overpayment applied
                            //masterTrx.TrxDate = inputdata.TransactionDate;
                            //masterTrx.RegionId = inputdata.RegionId;
                            //masterTrx.PeriodId = customerPayment.PeriodId;
                            //masterTrx.StatusId = 4; // open
                            //masterTrx.CreatedBy = inputdata.CreatedBy;
                            //masterTrx.CreatedDate = inputdata.CreatedDate;

                            //context.MasterTrxes.Add(masterTrx);
                            //context.SaveChanges();

                            //MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                            //masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
                            //masterTrxDetail.InvoiceId = null;
                            //masterTrxDetail.LineNo = null;
                            //masterTrxDetail.MasterTrxTypeListId = 24; // overpayment applied
                            //masterTrxDetail.SourceTypeListId = 3; // payment
                            //masterTrxDetail.SourceId = customerPayment.PaymentId;
                            //masterTrxDetail.RegionId = inputdata.RegionId;
                            //masterTrxDetail.PeriodId = customerPayment.PeriodId;

                            //masterTrxDetail.AmountTypeListId = 2; // debit
                            //masterTrxDetail.FeesDetail = false;
                            //masterTrxDetail.TaxDetail = false;
                            //masterTrxDetail.Total = inputdata.CreditAmount;

                            //masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                            //masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                            //masterTrxDetail.IsDelete = false;


                            //masterTrxDetail.BPPAdmin = 1;
                            //masterTrxDetail.AccountRebate = 1;
                            //masterTrxDetail.AccountRebate = 1;
                            //masterTrxDetail.Commission = false;
                            //masterTrxDetail.CommissionTotal = 0;
                            //masterTrxDetail.FRRevenues = true;
                            //masterTrxDetail.FRDeduction = false;


                            //context.MasterTrxDetails.Add(masterTrxDetail);
                            //context.SaveChanges();

                            //Overflow overflow = new Overflow();

                            //overflow.AmountTypeListId = 1;
                            //overflow.CheckAmount = inputdata.PaymentAmount;
                            ////string _outCheckNumber = "0";
                            ////string.TryParse(inputdata.ReferenceNo, out _outCheckNumber);
                            //overflow.CheckNumber = inputdata.ReferenceNo;
                            //overflow.MasterTrxDetailId = custMasterTrxDetailId;
                            //overflow.PeriodId = customerMasterTrx.PeriodId;

                            //overflow.TypeListId = 1; // customer
                            //overflow.ClassId = inputdata.CustomerId;
                            //overflow.MasterTrxTypeListId = 24; // overpayment applied
                            //overflow.TransactionDate = inputdata.TransactionDate;
                            //overflow.RegionId = inputdata.RegionId;
                            //overflow.IsActive = true;
                            //overflow.Amount = inputdata.CreditAmount;
                            //overflow.TransactionStatusListId = 4; // open
                            //overflow.CreatedBy = inputdata.CreatedBy;
                            //overflow.CreatedDate = inputdata.CreatedDate;

                            //context.Overflows.Add(overflow);
                            //context.SaveChanges();


                            MasterTrxDetail masterTrxDetail = context.MasterTrxDetails.SingleOrDefault(m => m.MasterTrxDetailId == custMasterTrxDetailId);
                            MasterTrxDetail masterTrxDetailOP = new MasterTrxDetail();
                            masterTrxDetailOP.MasterTrxId = customerMasterTrx.MasterTrxId;
                            masterTrxDetailOP.InvoiceId = masterTrxDetail.InvoiceId;
                            //masterTrxDetailOP.LineNo = masterTrxDetail.LineNo;
                            masterTrxDetailOP.MasterTrxTypeListId = 2; // customer payment
                            masterTrxDetailOP.HeaderId = customerPayment.PaymentId;
                            masterTrxDetailOP.RegionId = inputdata.RegionId;
                            masterTrxDetailOP.AmountTypeListId = 1; // credit
                            masterTrxDetailOP.FeesDetail = false;
                            masterTrxDetailOP.TaxDetail = true;
                            masterTrxDetailOP.TotalTax = 0;
                            masterTrxDetailOP.TotalFee = 0;
                            masterTrxDetailOP.Total = inputdata.Balance;
                            masterTrxDetailOP.ExtendedPrice = inputdata.Balance;
                            masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                            masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                            masterTrxDetailOP.IsDelete = false;
                            masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
                            masterTrxDetailOP.TypelistId = 1; // customer
                            masterTrxDetailOP.ClassId = inputdata.CustomerId;
                            masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                            context.MasterTrxDetails.Add(masterTrxDetailOP);
                            context.SaveChanges();

                            //masterTrxDetail.HeaderId = overflow.OverflowId;
                            //context.SaveChanges();

                            totalCustomerPayment += inputdata.CreditAmount;
                        }

                        // give customer credit (remaining balance after payments)

                        if (inputdata.Balance > 0)
                        {
                            //Overflow overflow = new Overflow();

                            //overflow.AmountTypeListId = 2;
                            //overflow.CheckAmount = inputdata.PaymentAmount;
                            ////int _outCheckNumber = 0;
                            ////int.TryParse(inputdata.ReferenceNo, out _outCheckNumber);
                            //overflow.CheckNumber = inputdata.ReferenceNo;
                            //overflow.MasterTrxDetailId = custMasterTrxDetailId;
                            //overflow.PeriodId = customerMasterTrx.PeriodId;
                            //overflow.TypeListId = 1; // customer
                            //overflow.ClassId = inputdata.CustomerId;
                            //overflow.MasterTrxTypeListId = 17; // overpayment
                            //overflow.TransactionDate = inputdata.TransactionDate;
                            //overflow.RegionId = inputdata.RegionId;
                            //overflow.IsActive = true;
                            //overflow.Amount = inputdata.Balance;
                            //overflow.SourceTypeListId = 3; // payment
                            //overflow.SourceId = customerPayment.PaymentId;
                            //overflow.TransactionStatusListId = 4; // open
                            //overflow.CreatedBy = inputdata.CreatedBy;
                            //overflow.CreatedDate = inputdata.CreatedDate;

                            //context.Overflows.Add(overflow);
                            //context.SaveChanges();


                            MasterTrxDetail masterTrxDetail = context.MasterTrxDetails.SingleOrDefault(m => m.MasterTrxDetailId == custMasterTrxDetailId);
                            MasterTrxDetail masterTrxDetailOP = new MasterTrxDetail();
                            masterTrxDetailOP.MasterTrxId = customerMasterTrx.MasterTrxId;
                            masterTrxDetailOP.InvoiceId = masterTrxDetail.InvoiceId;
                            //masterTrxDetailOP.LineNo = masterTrxDetail.LineNo;
                            masterTrxDetailOP.MasterTrxTypeListId = 2; // customer payment
                            masterTrxDetailOP.HeaderId = customerPayment.PaymentId;
                            masterTrxDetailOP.RegionId = inputdata.RegionId;
                            masterTrxDetailOP.AmountTypeListId = 1; // credit
                            masterTrxDetailOP.FeesDetail = false;
                            masterTrxDetailOP.TaxDetail = true;
                            masterTrxDetailOP.TotalTax = 0;
                            masterTrxDetailOP.TotalFee = 0;
                            masterTrxDetailOP.Total = inputdata.Balance;
                            masterTrxDetailOP.ExtendedPrice = inputdata.Balance;
                            masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                            masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                            masterTrxDetailOP.IsDelete = false;
                            masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
                            masterTrxDetailOP.TypelistId = 1; // customer
                            masterTrxDetailOP.ClassId = inputdata.CustomerId;
                            masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                            context.MasterTrxDetails.Add(masterTrxDetailOP);
                            context.SaveChanges();

                        }

                        //if ((totalCustomerPayment + totalCustomerTaxes) > 0)
                        //{
                        //    // general ledger for customer payment -- debit from A/R Janitorial Services
                        //    GeneralLedgerTrx oGeneralLedgerTrx = new GeneralLedgerTrx();

                        //    oGeneralLedgerTrx.MasterTrxId = customerMasterTrx.MasterTrxId;
                        //    oGeneralLedgerTrx.LedgerAccountId = 3;
                        //    oGeneralLedgerTrx.MasterTrxTypeListId = 2;
                        //    oGeneralLedgerTrx.TrxDate = DateTime.Now;
                        //    oGeneralLedgerTrx.Debit = totalCustomerPayment + totalCustomerTaxes;
                        //    oGeneralLedgerTrx.Credit = 0;
                        //    oGeneralLedgerTrx.Amount = totalCustomerPayment + totalCustomerTaxes;
                        //    oGeneralLedgerTrx.AmountTypeListId = 2;
                        //    oGeneralLedgerTrx.IsDelete = false;
                        //    oGeneralLedgerTrx.RegionId = inputdata.RegionId;
                        //    oGeneralLedgerTrx.PeriodId = customerMasterTrx.PeriodId;
                        //    oGeneralLedgerTrx.CreatedBy = LoginUserId;
                        //    oGeneralLedgerTrx.CreatedDate = DateTime.Now;
                        //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
                        //    context.SaveChanges();


                        //    // general ledger for customer payment -- debit from A/R Janitorial Services
                        //    GeneralLedgerTrx oGeneralLedgerTrxM = new GeneralLedgerTrx();

                        //    oGeneralLedgerTrxM.MasterTrxId = customerMasterTrx.MasterTrxId;
                        //    oGeneralLedgerTrxM.LedgerAccountId = 3;
                        //    oGeneralLedgerTrxM.MasterTrxTypeListId = 7;
                        //    oGeneralLedgerTrxM.TrxDate = DateTime.Now;
                        //    oGeneralLedgerTrxM.Debit = 0;
                        //    oGeneralLedgerTrxM.Credit = totalCustomerPayment + totalCustomerTaxes;
                        //    oGeneralLedgerTrxM.Amount = totalCustomerPayment + totalCustomerTaxes;
                        //    oGeneralLedgerTrxM.AmountTypeListId = 1;
                        //    oGeneralLedgerTrxM.IsDelete = false;
                        //    oGeneralLedgerTrxM.RegionId = inputdata.RegionId;
                        //    oGeneralLedgerTrxM.PeriodId = customerMasterTrx.PeriodId;
                        //    oGeneralLedgerTrxM.CreatedBy = LoginUserId;
                        //    oGeneralLedgerTrxM.CreatedDate = DateTime.Now;
                        //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrxM);
                        //    context.SaveChanges();
                        //}

                        if (customerTransactionNumberConfigViewModel != null)
                        {
                            customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
                            CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                        }


                    }
                }
                else
                {
                    int InvIdT = inputdata.Invoices[0].InvoiceId;
                    // customer payment mastertrx
                    Invoice _invoiceOject = context.Invoices.Where(o => o.InvoiceId == InvIdT).FirstOrDefault();
                    //int CreditCount = context.Credits.Where(o => o.InvoiceId == vm.InvoiceId && o.IsDelete != true).Count();

                    MasterTrx customerMasterTrx = new MasterTrx();
                    customerMasterTrx.TypeListId = 1; // customer
                    customerMasterTrx.ClassId = inputdata.CustomerId;
                    customerMasterTrx.MasterTrxTypeListId = 2; // customer payment
                    customerMasterTrx.TrxDate = inputdata.TransactionDate;
                    customerMasterTrx.RegionId = inputdata.RegionId;
                    customerMasterTrx.StatusId = 1; // open

                    customerMasterTrx.BillMonth = inputdata.TransactionDate.Month;
                    customerMasterTrx.BillYear = inputdata.TransactionDate.Year;
                    var PR = context.Periods.SingleOrDefault(p => p.BillMonth == customerMasterTrx.BillMonth && p.BillYear == customerMasterTrx.BillYear);
                    customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerMasterTrx.CreatedBy = LoginUserId;
                    customerMasterTrx.CreatedDate = DateTime.Now;
                    context.MasterTrxes.Add(customerMasterTrx);
                    context.SaveChanges();

                    // customer payment

                    bool allPaidInFull = inputdata.Invoices.All(o => o.PaidInFull);

                    Payment customerPayment = new Payment();
                    customerPayment.MasterTrxId = customerMasterTrx.MasterTrxId;
                    customerPayment.TypeListId = 1; // customer
                    customerPayment.ClassId = inputdata.CustomerId;
                    customerPayment.RegionId = inputdata.RegionId;
                    customerPayment.PaymentMethodListId = inputdata.PaymentMethodListId;
                    customerPayment.PaymentNo = inputdata.ReferenceNo;
                    customerPayment.TransactionStatusListId = 1;// allPaidInFull ? 6 : 7; // paid : paid partial
                    customerPayment.TempTransactionStatusListId = 3;// allPaidInFull ? 6 : 7; // paid : paid partial
                    customerPayment.CheckAmount = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;
                    //string _LastChar = "";
                    //if (CreditCount > 0)
                    //    _LastChar = ((char)(65 + CreditCount - 1)).ToString();

                    customerPayment.TransactionNumber = "PMT" + customerTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", customerMasterTrx.BillMonth) + customerNextTrxNumber;

                    //customerPayment.TransactionNumber = "PMT" + _invoiceOject.InvoiceNo.Trim();// customerNextTrxNumber;

                    //customerPayment.TransactionNumber = customerNextTrxNumber;
                    customerPayment.TransactionDate = inputdata.TransactionDate;
                    customerPayment.IsDelete = false;
                    customerPayment.CreatedBy = inputdata.CreatedBy;
                    customerPayment.CreatedDate = inputdata.CreatedDate;
                    customerPayment.PaymentDescription = inputdata.Notes;
                    customerPayment.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerPayment.InvoiceId = inputdata.Invoices[0].InvoiceId;
                    //customerPayment.CheckAmount
                    context.Payments.Add(customerPayment);
                    context.SaveChanges();

                    customerMasterTrx.HeaderId = customerPayment.PaymentId;
                    context.SaveChanges();
                    // checkbook






                    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                    {
                        var parmas = new DynamicParameters();
                        parmas.Add("@TransactionDate", inputdata.TransactionDate);
                        parmas.Add("@RegionId", inputdata.RegionId);
                        parmas.Add("@CreatedBy", LoginUserId);
                        parmas.Add("@CustomerId", inputdata.CustomerId);
                        parmas.Add("@ApplyAmount", inputdata.PaymentAmount);
                        parmas.Add("@ReferenceNo", inputdata.ReferenceNo);
                        parmas.Add("@Notes", inputdata.Notes);
                        parmas.Add("@CallFromTemp", true);
                        using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_CheckBookInsert", parmas, commandType: CommandType.StoredProcedure)) { }
                    }


                    //CheckBook cb = new CheckBook();
                    //cb.ReferenceNumber = inputdata.ReferenceNo;
                    //cb.Notes = inputdata.Notes;
                    //cb.TransactionStatusListId = 4; // open
                    //cb.Reconciled = false;
                    //cb.FundTypeListId = inputdata.PaymentMethodListId;
                    //cb.TypeListId = 1; // customer
                    //cb.ClassId = inputdata.CustomerId;
                    //cb.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //cb.SourceTypeListId = 2; // customer payment
                    //cb.SourceId = customerPayment.PaymentId;
                    //cb.CreatedBy = inputdata.CreatedBy;
                    //cb.CreatedDate = inputdata.CreatedDate;

                    //context.CheckBooks.Add(cb);
                    //context.SaveChanges();

                    decimal totalCustomerPayment = 0;
                    decimal totalCustomerTaxes = 0;

                    int custMasterTrxDetailId = 0;
                    int custMasterTrxDetailINVId = 0;


                    foreach (ManualPaymentInvoiceViewModel ivm in inputdata.Invoices)
                    {
                        foreach (ManualPaymentViewModel cvm in ivm.CustomerPayment.Payments)
                        {
                            MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                            masterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId;
                            masterTrxDetail.InvoiceId = ivm.InvoiceId;
                            masterTrxDetail.LineNo = cvm.LineNo;
                            masterTrxDetail.MasterTrxTypeListId = 2; // customer payment
                            masterTrxDetail.HeaderId = customerPayment.PaymentId;
                            masterTrxDetail.RegionId = inputdata.RegionId;
                            masterTrxDetail.AmountTypeListId = 1; // credit
                            masterTrxDetail.FeesDetail = false;
                            masterTrxDetail.TaxDetail = true;
                            masterTrxDetail.TotalTax = Math.Round(cvm.Tax,2);
                            masterTrxDetail.Total = cvm.PaymentAmount;
                            masterTrxDetail.ExtendedPrice = cvm.ExtendedPrice;
                            masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                            masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                            masterTrxDetail.IsDelete = false;

                            //                        masterTrxDetail.ServiceTypeListId =

                            masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                            masterTrxDetail.TypelistId = 1; // customer
                            masterTrxDetail.ClassId = inputdata.CustomerId;
                            masterTrxDetail.Transactiondate = inputdata.TransactionDate;

                            masterTrxDetail.BPPAdmin = 1;
                            masterTrxDetail.AccountRebate = 1;
                            masterTrxDetail.AccountRebate = 1;
                            masterTrxDetail.Commission = false;
                            masterTrxDetail.CommissionTotal = 0;
                            masterTrxDetail.FRRevenues = false;
                            masterTrxDetail.FRDeduction = false;
                            masterTrxDetail.DetailDescription = inputdata.Notes;

                            List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                            if (_masterINVDetail.Count > 0)
                            {
                                masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                masterTrxDetail.Quantity = 0;
                                masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                masterTrxDetail.TotalFee = 0;
                                masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                            }


                            context.MasterTrxDetails.Add(masterTrxDetail);
                            context.SaveChanges();


                            custMasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                            // insert customer taxes

                            MasterTrxTax customerTax = new MasterTrxTax();
                            customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                            customerTax.Amount = cvm.Tax;
                            customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.PaymentAmount, 2);
                            customerTax.AmountTypeListId = 1; // credit
                            customerTax.CreatedBy = inputdata.CreatedBy;
                            customerTax.CreatedDate = inputdata.CreatedDate;

                            customerTax.InvoiceId = ivm.InvoiceId;
                            customerTax.RegionId = inputdata.RegionId;
                            customerTax.PeriodId = (PR != null ? PR.PeriodId : 0);
                            customerTax.CustomerId = inputdata.CustomerId;
                            customerTax.FRRevenues = false;
                            customerTax.FRDeduction = false;
                            //customerTax.FranchiseeId = null;

                            context.MasterTrxTaxes.Add(customerTax);
                            context.SaveChanges();

                            totalCustomerPayment += cvm.Total;
                            totalCustomerTaxes += cvm.Tax;

                            var invoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
                            var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                            var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

                            List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
                            decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
                            decimal totalTransactions = 0.00m;
                            decimal grandTotalTransactions = 0.00m;

                            foreach (var trx in invoiceTransactions)
                            {
                                totalTransactions = totalTransactions + (decimal)trx.Total;
                            }
                            grandTotalTransactions = totalTransactions + cvm.PaymentAmount;


                            if (grandTotalTransactions >= invoiceTotal)
                            {
                                invoice.TransactionStatusListId = 6; /*6 = Paid*/
                                invoiceMasterTrx.StatusId = 6;
                            }
                            else
                            {
                                invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
                                invoiceMasterTrx.StatusId = 7;
                            }

                            context.SaveChanges();


                        }

                        // update original invoice's status 
                        // Comment 
                        //Invoice existingInvoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
                        //if (existingInvoice != null)
                        //{
                        //    existingInvoice.TransactionStatusListId = ivm.PaidInFull ? 6 : 7; // paid : paid partial
                        //    existingInvoice.ModifiedBy = inputdata.CreatedBy;
                        //    existingInvoice.ModifiedDate = inputdata.CreatedDate;
                        //    context.Entry(existingInvoice).State = EntityState.Modified;
                        //    context.SaveChanges();
                        //}

                        foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
                        {
                            // compute franchisee payment fees

                            decimal totalFees = 0;

                            List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
                            List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

                            foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                            {
                                MasterTrxFeeDetail feeDetail = new MasterTrxFeeDetail();
                                if (feeDef.FeePercentage != null) // percentage
                                {
                                    feeDetail.FeePercentage = feeDef.FeePercentage;
                                    feeDetail.Amount = (decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M);// Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
                                }
                                else // flat amount
                                {
                                    feeDetail.Amount = feeDef.Amount;
                                    feeDetail.FeePercentage = null;
                                }
                                feeDetail.FeeId = feeDef.FeeId;
                                feeDetail.AmountTypeListId = 1; // credit
                                feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                                feeDetail.CreatedBy = inputdata.CreatedBy;
                                feeDetail.CreatedDate = inputdata.CreatedDate;
                                feeDetail.FranchiseeId = fcvm.FranchiseeId;
                                feeDetail.BillingPayId = fcvm.BillingPayId;
                                totalFees += feeDetail.Amount ?? 0;

                                franchiseeFees.Add(feeDetail);
                            }

                            // franchisee payment mastertrx

                            MasterTrx franchiseeMasterTrx = new MasterTrx();
                            franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
                            franchiseeMasterTrx.TypeListId = 2; // franchisee
                            franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
                            franchiseeMasterTrx.TrxDate = inputdata.TransactionDate;
                            franchiseeMasterTrx.RegionId = inputdata.RegionId;
                            franchiseeMasterTrx.StatusId = 1; // open

                            franchiseeMasterTrx.BillMonth = inputdata.TransactionDate.Month;
                            franchiseeMasterTrx.BillYear = inputdata.TransactionDate.Year;
                            franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                            franchiseeMasterTrx.CreatedBy = LoginUserId;
                            franchiseeMasterTrx.CreatedDate = DateTime.Now;

                            context.MasterTrxes.Add(franchiseeMasterTrx);
                            context.SaveChanges();


                            // franchisee payment

                            string franchiseeNextTrxNumber = null;

                            franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(7, inputdata.RegionId);
                            if (franchiseeTransactionNumberConfigViewModel != null)
                                franchiseeNextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(7, inputdata.RegionId, inputdata.CreatedDate);

                            PaymentBillingFranchisee franchiseePayment = new PaymentBillingFranchisee();
                            franchiseePayment.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                            franchiseePayment.PaymentId = customerPayment.PaymentId;
                            franchiseePayment.FranchiseeId = fcvm.FranchiseeId;
                            franchiseePayment.BillingPayId = fcvm.BillingPayId;
                            franchiseePayment.RegionId = inputdata.RegionId;
                            franchiseePayment.LineNo = Convert.ToString(fcvm.Payment.LineNo);
                            franchiseePayment.Amount = fcvm.Payment.PaymentAmount;
                            franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
                            franchiseePayment.TransactionDate = inputdata.TransactionDate;
                            franchiseePayment.CreatedBy = inputdata.CreatedBy;
                            franchiseePayment.CreatedDate = inputdata.CreatedDate;
                            franchiseePayment.IsTARPaid = fcvm.IsTARPaid;
                            franchiseePayment.IsTurnaroundPayment = fcvm.IsTurnAroundPayment;
                            franchiseePayment.TransactionStatusListId = 1;
                            context.PaymentBillingFranchisees.Add(franchiseePayment);
                            context.SaveChanges();

                            franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
                            context.SaveChanges();

                            if (franchiseeTransactionNumberConfigViewModel != null)
                            {
                                franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
                                CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                            }


                            decimal paymentMinusFees = (decimal)(fcvm.Payment.PaymentAmount - totalFees); // Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

                            MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
                            franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                            franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
                            franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
                            franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
                            franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentBillingFranchiseeId; //customerPayment.PaymentId;
                            franchiseeMasterTrxDetail.RegionId = inputdata.RegionId;

                            franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                            franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
                            franchiseeMasterTrxDetail.FeesDetail = true;
                            franchiseeMasterTrxDetail.TaxDetail = false;
                            franchiseeMasterTrxDetail.TotalFee = totalFees;
                            franchiseeMasterTrxDetail.Total = paymentMinusFees;

                            franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
                            franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
                            franchiseeMasterTrxDetail.IsDelete = false;
                            franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                            franchiseeMasterTrxDetail.TypelistId = 2; // customer
                            franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
                            franchiseeMasterTrxDetail.Transactiondate = inputdata.TransactionDate;



                            franchiseeMasterTrxDetail.BPPAdmin = 1;
                            franchiseeMasterTrxDetail.AccountRebate = 1;
                            franchiseeMasterTrxDetail.AccountRebate = 1;
                            franchiseeMasterTrxDetail.Commission = false;
                            franchiseeMasterTrxDetail.CommissionTotal = 0;
                            franchiseeMasterTrxDetail.FRRevenues = false;
                            franchiseeMasterTrxDetail.FRDeduction = false;
                            franchiseeMasterTrxDetail.DetailDescription = inputdata.Notes;
                            List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                            if (_masterINVDetail.Count > 0)
                            {
                                franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                franchiseeMasterTrxDetail.Quantity = 0;
                                franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                franchiseeMasterTrxDetail.TotalTax = 0;
                                franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                            }


                            context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                            context.SaveChanges();


                            // insert franchisee fees

                            foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
                            {
                                feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

                                context.MasterTrxFeeDetails.Add(feeDetail);
                                context.SaveChanges();
                            }


                        }


                        if (ivm.overflowAmount > 0)
                        {
                            //Overflow overflow = new Overflow();

                            //overflow.AmountTypeListId = 2;
                            //overflow.CheckAmount = inputdata.PaymentAmount;
                            ////int _outCheckNumber = 0;
                            ////int.TryParse(inputdata.ReferenceNo, out _outCheckNumber);
                            //overflow.CheckNumber = inputdata.ReferenceNo;
                            //overflow.MasterTrxDetailId = custMasterTrxDetailId;
                            //overflow.PeriodId = (PR != null ? PR.PeriodId : 0);
                            //overflow.TypeListId = 1; // customer
                            //overflow.ClassId = inputdata.CustomerId;
                            //overflow.MasterTrxTypeListId = 17; // overpayment
                            //overflow.TransactionDate = inputdata.TransactionDate;
                            //overflow.RegionId = inputdata.RegionId;
                            //overflow.IsActive = true;
                            //overflow.Amount = inputdata.Balance;
                            //overflow.SourceTypeListId = 3; // payment
                            //overflow.SourceId = customerPayment.PaymentId;
                            //overflow.TransactionStatusListId = 4; // open
                            //overflow.CreatedBy = inputdata.CreatedBy;
                            //overflow.CreatedDate = inputdata.CreatedDate;
                            //overflow.InvoiceId = ivm.InvoiceId;
                            //context.Overflows.Add(overflow);
                            //context.SaveChanges();


                            MasterTrxDetail masterTrxDetail = context.MasterTrxDetails.SingleOrDefault(m => m.MasterTrxDetailId == custMasterTrxDetailId);
                            MasterTrxDetail masterTrxDetailOP = new MasterTrxDetail();
                            masterTrxDetailOP.MasterTrxId = customerMasterTrx.MasterTrxId;
                            masterTrxDetailOP.InvoiceId = masterTrxDetail.InvoiceId;
                            //masterTrxDetailOP.LineNo = masterTrxDetail.LineNo;
                            masterTrxDetailOP.MasterTrxTypeListId = 2; // customer payment
                            masterTrxDetailOP.HeaderId = customerPayment.PaymentId;
                            masterTrxDetailOP.RegionId = inputdata.RegionId;
                            masterTrxDetailOP.AmountTypeListId = 1; // credit
                            masterTrxDetailOP.FeesDetail = false;
                            masterTrxDetailOP.TaxDetail = true;
                            masterTrxDetailOP.TotalTax = 0;
                            masterTrxDetailOP.TotalFee = 0;
                            masterTrxDetailOP.Total = inputdata.Balance;
                            masterTrxDetailOP.ExtendedPrice = inputdata.Balance;
                            masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                            masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                            masterTrxDetailOP.IsDelete = false;
                            masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
                            masterTrxDetailOP.TypelistId = 1; // customer
                            masterTrxDetailOP.ClassId = inputdata.CustomerId;
                            masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                            context.MasterTrxDetails.Add(masterTrxDetailOP);
                            context.SaveChanges();
                        }

                    }

                    // deduct customer credit (amount taken from previous customer credit balance)

                    if (inputdata.CreditAmount > 0)
                    {
                        MasterTrxDetail masterTrxDetail = context.MasterTrxDetails.SingleOrDefault(m => m.MasterTrxDetailId == custMasterTrxDetailId);
                        MasterTrxDetail masterTrxDetailOP = new MasterTrxDetail();
                        masterTrxDetailOP.MasterTrxId = customerMasterTrx.MasterTrxId;
                        masterTrxDetailOP.InvoiceId = masterTrxDetail.InvoiceId;
                        //masterTrxDetailOP.LineNo = masterTrxDetail.LineNo;
                        masterTrxDetailOP.MasterTrxTypeListId = 2; // customer payment
                        masterTrxDetailOP.HeaderId = customerPayment.PaymentId;
                        masterTrxDetailOP.RegionId = inputdata.RegionId;
                        masterTrxDetailOP.AmountTypeListId = 1; // credit
                        masterTrxDetailOP.FeesDetail = false;
                        masterTrxDetailOP.TaxDetail = true;
                        masterTrxDetailOP.TotalTax = 0;
                        masterTrxDetailOP.TotalFee = 0;
                        masterTrxDetailOP.Total = inputdata.Balance;
                        masterTrxDetailOP.ExtendedPrice = inputdata.Balance;
                        masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                        masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                        masterTrxDetailOP.IsDelete = false;
                        masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
                        masterTrxDetailOP.TypelistId = 1; // customer
                        masterTrxDetailOP.ClassId = inputdata.CustomerId;
                        masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                        context.MasterTrxDetails.Add(masterTrxDetailOP);
                        context.SaveChanges();

                        //MasterTrx masterTrx = new MasterTrx();
                        //masterTrx.TypeListId = 1; // customer
                        //masterTrx.ClassId = inputdata.CustomerId;
                        //masterTrx.MasterTrxTypeListId = 24; // overpayment applied
                        //masterTrx.TrxDate = inputdata.TransactionDate;
                        //masterTrx.RegionId = inputdata.RegionId;
                        //masterTrx.PeriodId = customerPayment.PeriodId;
                        //masterTrx.StatusId = 4; // open
                        //masterTrx.CreatedBy = inputdata.CreatedBy;
                        //masterTrx.CreatedDate = inputdata.CreatedDate;

                        //context.MasterTrxes.Add(masterTrx);
                        //context.SaveChanges();

                        //MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                        //masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
                        //masterTrxDetail.InvoiceId = null;
                        //masterTrxDetail.LineNo = null;
                        //masterTrxDetail.MasterTrxTypeListId = 24; // overpayment applied
                        //masterTrxDetail.SourceTypeListId = 3; // payment
                        //masterTrxDetail.SourceId = customerPayment.PaymentId;
                        //masterTrxDetail.RegionId = inputdata.RegionId;
                        //masterTrxDetail.PeriodId = customerPayment.PeriodId;

                        //masterTrxDetail.AmountTypeListId = 2; // debit
                        //masterTrxDetail.FeesDetail = false;
                        //masterTrxDetail.TaxDetail = false;
                        //masterTrxDetail.Total = inputdata.CreditAmount;

                        //masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                        //masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                        //masterTrxDetail.IsDelete = false;

                        //masterTrxDetail.BPPAdmin = 1;
                        //masterTrxDetail.AccountRebate = 1;
                        //masterTrxDetail.AccountRebate = 1;
                        //masterTrxDetail.Commission = false;
                        //masterTrxDetail.CommissionTotal = 0;
                        //masterTrxDetail.FRRevenues = true;
                        //masterTrxDetail.FRDeduction = false;

                        //context.MasterTrxDetails.Add(masterTrxDetail);
                        //context.SaveChanges();

                        //Overflow overflow = new Overflow();

                        //overflow.AmountTypeListId = 1;
                        //overflow.CheckAmount = inputdata.PaymentAmount;
                        ////string _outCheckNumber = "0";
                        ////string.TryParse(inputdata.ReferenceNo, out _outCheckNumber);
                        //overflow.CheckNumber = inputdata.ReferenceNo;
                        //overflow.MasterTrxDetailId = custMasterTrxDetailId;
                        //overflow.PeriodId = (PR != null ? PR.PeriodId : 0);

                        //overflow.TypeListId = 1; // customer
                        //overflow.ClassId = inputdata.CustomerId;
                        //overflow.MasterTrxTypeListId = 24; // overpayment applied
                        //overflow.TransactionDate = inputdata.TransactionDate;
                        //overflow.RegionId = inputdata.RegionId;
                        //overflow.IsActive = true;
                        //overflow.Amount = inputdata.CreditAmount;
                        //overflow.TransactionStatusListId = 4; // open
                        //overflow.CreatedBy = inputdata.CreatedBy;
                        //overflow.CreatedDate = inputdata.CreatedDate;

                        //context.Overflows.Add(overflow);
                        //context.SaveChanges();

                        //masterTrxDetail.HeaderId = overflow.OverflowId;
                        //context.SaveChanges();

                        totalCustomerPayment += inputdata.CreditAmount;
                    }

                    // give customer credit (remaining balance after payments)

                    //if (inputdata.Balance > 0)
                    //{
                    //    Overflow overflow = new Overflow();

                    //    overflow.AmountTypeListId = 2;
                    //    overflow.CheckAmount = inputdata.PaymentAmount;
                    //    //int _outCheckNumber = 0;
                    //    //int.TryParse(inputdata.ReferenceNo, out _outCheckNumber);
                    //    overflow.CheckNumber = inputdata.ReferenceNo;
                    //    overflow.MasterTrxDetailId = custMasterTrxDetailId;
                    //    overflow.PeriodId = (PR != null ? PR.PeriodId : 0);
                    //    overflow.TypeListId = 1; // customer
                    //    overflow.ClassId = inputdata.CustomerId;
                    //    overflow.MasterTrxTypeListId = 17; // overpayment
                    //    overflow.TransactionDate = inputdata.TransactionDate;
                    //    overflow.RegionId = inputdata.RegionId;
                    //    overflow.IsActive = true;
                    //    overflow.Amount = inputdata.Balance;
                    //    overflow.SourceTypeListId = 3; // payment
                    //    overflow.SourceId = customerPayment.PaymentId;
                    //    overflow.TransactionStatusListId = 4; // open
                    //    overflow.CreatedBy = inputdata.CreatedBy;
                    //    overflow.CreatedDate = inputdata.CreatedDate;
                    //    overflow.InvoiceId = custMasterTrxDetailINVId;
                    //    context.Overflows.Add(overflow);
                    //    context.SaveChanges();
                    //}

                    //if ((totalCustomerPayment + totalCustomerTaxes) > 0)
                    //{
                    //    // general ledger for customer payment -- debit from A/R Janitorial Services
                    //    GeneralLedgerTrx oGeneralLedgerTrx = new GeneralLedgerTrx();

                    //    oGeneralLedgerTrx.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //    oGeneralLedgerTrx.LedgerAccountId = 3;
                    //    oGeneralLedgerTrx.MasterTrxTypeListId = 2;
                    //    oGeneralLedgerTrx.TrxDate = DateTime.Now;
                    //    oGeneralLedgerTrx.Debit = totalCustomerPayment + totalCustomerTaxes;
                    //    oGeneralLedgerTrx.Credit = 0;
                    //    oGeneralLedgerTrx.Amount = totalCustomerPayment + totalCustomerTaxes;
                    //    oGeneralLedgerTrx.AmountTypeListId = 2;
                    //    oGeneralLedgerTrx.IsDelete = false;
                    //    oGeneralLedgerTrx.RegionId = inputdata.RegionId;
                    //    oGeneralLedgerTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                    //    oGeneralLedgerTrx.CreatedBy = LoginUserId;
                    //    oGeneralLedgerTrx.CreatedDate = DateTime.Now;
                    //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrx);
                    //    context.SaveChanges();


                    //    // general ledger for customer payment -- debit from A/R Janitorial Services
                    //    GeneralLedgerTrx oGeneralLedgerTrxM = new GeneralLedgerTrx();

                    //    oGeneralLedgerTrxM.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //    oGeneralLedgerTrxM.LedgerAccountId = 3;
                    //    oGeneralLedgerTrxM.MasterTrxTypeListId = 7;
                    //    oGeneralLedgerTrxM.TrxDate = DateTime.Now;
                    //    oGeneralLedgerTrxM.Debit = 0;
                    //    oGeneralLedgerTrxM.Credit = totalCustomerPayment + totalCustomerTaxes;
                    //    oGeneralLedgerTrxM.Amount = totalCustomerPayment + totalCustomerTaxes;
                    //    oGeneralLedgerTrxM.AmountTypeListId = 1;
                    //    oGeneralLedgerTrxM.IsDelete = false;
                    //    oGeneralLedgerTrxM.RegionId = inputdata.RegionId;
                    //    oGeneralLedgerTrxM.PeriodId = (PR != null ? PR.PeriodId : 0);
                    //    oGeneralLedgerTrxM.CreatedBy = LoginUserId;
                    //    oGeneralLedgerTrxM.CreatedDate = DateTime.Now;
                    //    context.GeneralLedgerTrxes.Add(oGeneralLedgerTrxM);
                    //    context.SaveChanges();



                    //    ////[dbo].[GeneralLedgerTrx]([MasterTrxId],[LedgerAccountId],[MasterTrxTypeListId],[TrxDate],[Debit],[Credit],[Amount],[AmountTypeListId],[IsDelete],[RegionId],[PeriodId],[CreatedBy],[CreatedDate])
                    //    //GeneralLedger glCustomerPayment2 = new GeneralLedger();
                    //    //glCustomerPayment2.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //    //glCustomerPayment2.LedgerAcctId = 3; // A/R Janitorial Services
                    //    //glCustomerPayment2.Credit = 0;
                    //    //glCustomerPayment2.Debit = totalCustomerPayment + totalCustomerTaxes;
                    //    //glCustomerPayment2.IsDelete = false;

                    //    //context.GeneralLedgers.Add(glCustomerPayment2);
                    //    //context.SaveChanges();

                    //    //// general ledger for customer payment -- credit to Undeposit, Pre-Post Bank Account

                    //    //GeneralLedger glCustomerPayment3 = new GeneralLedger();
                    //    //glCustomerPayment3.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //    //glCustomerPayment3.LedgerAcctId = 8; // Undeposit, Pre-Post Bank Account
                    //    //glCustomerPayment3.Credit = totalCustomerPayment + totalCustomerTaxes;
                    //    //glCustomerPayment3.Debit = 0;
                    //    //glCustomerPayment3.IsDelete = false;

                    //    //context.GeneralLedgers.Add(glCustomerPayment3);
                    //    //context.SaveChanges();
                    //}

                    if (customerTransactionNumberConfigViewModel != null)
                    {
                        customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                    }
                }




                return true;
            }
        }

        public bool InsertOrUpdateCreditTransaction(CreditTransactionViewModel vm)
        {
            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(3, vm.RegionId);

            string customerNextTrxNumber = null;

            JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Invoice _invoiceOject = context.Invoices.Single(o => o.InvoiceId == vm.InvoiceId);
                var PR = context.Periods.SingleOrDefault(p => p.BillMonth == vm.BillMonth && p.BillYear == vm.BillYear);
                int CreditCount = context.Credits.Where(o => o.InvoiceId == vm.InvoiceId && o.IsDelete != true).Count();

                Credit customerCredit = null;
                if (vm.Id != -1)
                    customerCredit = context.Credits.Where(o => o.CreditId == vm.Id).FirstOrDefault();

                // customer credit mastertrx

                MasterTrx customerMasterTrx = null;
                if (customerCredit != null)
                    customerMasterTrx = context.MasterTrxes.Where(o => o.MasterTrxId == customerCredit.MasterTrxId).FirstOrDefault();

                if (customerMasterTrx == null)
                {
                    customerMasterTrx = new MasterTrx();
                    customerMasterTrx.MasterTrxTypeListId = 3; // customer credit
                    customerMasterTrx.ClassId = vm.CustomerCredit.CustomerId;
                    customerMasterTrx.TypeListId = 1; // customer
                    customerMasterTrx.RegionId = vm.RegionId;
                    customerMasterTrx.StatusId = 1; // open
                    customerMasterTrx.TrxDate = vm.CreatedDate;
                    customerMasterTrx.BillMonth = vm.BillMonth;
                    customerMasterTrx.BillYear = vm.BillYear;
                    customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerMasterTrx.CreatedBy = LoginUserId;
                    customerMasterTrx.CreatedDate = DateTime.Now;
                    context.MasterTrxes.Add(customerMasterTrx);
                }
                else
                {
                    // todo: add mastertrx modifiedby/date
                    customerMasterTrx.TrxDate = vm.CreatedDate;
                    customerMasterTrx.BillMonth = vm.BillMonth;
                    customerMasterTrx.BillYear = vm.BillYear;
                    customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerMasterTrx.ModifiedBy = LoginUserId;
                    customerMasterTrx.ModifiedDate = DateTime.Now;
                    context.Entry(customerMasterTrx).State = EntityState.Modified;
                }

                context.SaveChanges();

                // customer credit

                if (customerCredit == null)
                {
                    customerCredit = new Credit();
                    customerCredit.MasterTrxId = customerMasterTrx.MasterTrxId;
                    customerCredit.InvoiceId = vm.InvoiceId;
                    customerCredit.TypeListId = 1; // customer
                    customerCredit.ClassId = vm.CustomerCredit.CustomerId;
                    customerCredit.RegionId = vm.RegionId;
                    customerCredit.TransactionStatusListId = 1; // pending approval
                    customerCredit.IsDelete = false;

                    string _LastChar = "";
                    if (CreditCount > 0) _LastChar = ((char)(65 + CreditCount - 1)).ToString();

                    customerCredit.TransactionNumber = "CR" + _invoiceOject.InvoiceNo.Trim() + _LastChar.Trim();// customerNextTrxNumber;
                    customerCredit.CreatedBy = vm.CreatedBy;
                    customerCredit.CreatedDate = DateTime.Now;
                    customerCredit.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerCredit.TransactionDate = vm.CreatedDate;
                    customerCredit.CreditReasonListId = vm.CreditReasonListId;
                    customerCredit.CreditDescription = vm.CreditDescription;
                    context.Credits.Add(customerCredit);
                }
                else
                {
                    customerCredit.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerCredit.TransactionDate = vm.CreatedDate;
                    customerCredit.CreditReasonListId = vm.CreditReasonListId;
                    customerCredit.CreditDescription = vm.CreditDescription;
                    customerCredit.ModifiedBy = vm.CreatedBy;
                    customerCredit.ModifiedDate = vm.CreatedDate;
                    context.Entry(customerCredit).State = EntityState.Modified;
                }
                context.SaveChanges();

                //Update HeaderId In MasterTrx
                customerMasterTrx.HeaderId = customerCredit.CreditId;
                context.SaveChanges();

                decimal totalCustomerCredit = 0;
                decimal totalCustomerTaxes = 0;
                int lastMasterTrxDetailId = 0;

                int cMasterConter = 0;

                foreach (CreditViewModel cvm in vm.CustomerCredit.Credits)
                {
                    cMasterConter++;
                    MasterTrxDetail masterTrxDetail = null;
                    if (vm.Id != -1)
                        masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxId == customerMasterTrx.MasterTrxId && o.LineNo == cvm.LineNo).FirstOrDefault();

                    if (masterTrxDetail == null)
                    {
                        if (cvm.CreditAmount == 0)
                            continue; // do not create a new credit for a line item with 0 credit amt

                        masterTrxDetail = new MasterTrxDetail();
                        masterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId;
                        masterTrxDetail.InvoiceId = vm.InvoiceId;
                        masterTrxDetail.LineNo = cvm.LineNo;
                        masterTrxDetail.MasterTrxTypeListId = 3; // customer credit
                        masterTrxDetail.HeaderId = customerCredit.CreditId;
                        masterTrxDetail.RegionId = vm.RegionId;
                        masterTrxDetail.CreatedBy = vm.CreatedBy;
                        masterTrxDetail.CreatedDate = vm.CreatedDate;
                        masterTrxDetail.IsDelete = false;

                        masterTrxDetail.ServiceTypeListId = cvm.ServiceTypeListId;
                        masterTrxDetail.Quantity = 1;
                        masterTrxDetail.UnitPrice = cvm.CreditAmount;
                        masterTrxDetail.ExtendedPrice = cvm.CreditAmount;
                        masterTrxDetail.FeesDetail = false;
                        masterTrxDetail.TaxDetail = true;
                        masterTrxDetail.TotalTax = Math.Round(cvm.Tax,2);
                        masterTrxDetail.Total = cvm.Total;
                        masterTrxDetail.AmountTypeListId = 1; // credit
                        masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                        masterTrxDetail.ClassId = vm.CustomerCredit.CustomerId;
                        masterTrxDetail.TypelistId = 1;
                        masterTrxDetail.Transactiondate = vm.CreatedDate;
                        masterTrxDetail.DetailDescription = vm.CreditDescription;
                        masterTrxDetail.FRRevenues = true;
                        masterTrxDetail.BPPAdmin = 0;
                        masterTrxDetail.FRDeduction = false;

                        List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == vm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                        if (_masterINVDetail.Count > 0)
                        {
                            masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                            masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                            masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                            //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                            masterTrxDetail.Quantity = 0;
                            masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                            masterTrxDetail.TotalFee = 0;
                            masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                            masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                            masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                            masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                            masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                        }

                        context.MasterTrxDetails.Add(masterTrxDetail);
                    }
                    else
                    {
                        masterTrxDetail.ServiceTypeListId = cvm.ServiceTypeListId;
                        masterTrxDetail.Quantity = 1;
                        masterTrxDetail.UnitPrice = cvm.CreditAmount;
                        masterTrxDetail.ExtendedPrice = cvm.CreditAmount;
                        masterTrxDetail.FeesDetail = false;
                        masterTrxDetail.TaxDetail = true;
                        masterTrxDetail.TotalTax = Math.Round(cvm.Tax,2);
                        masterTrxDetail.Total = cvm.Total;
                        masterTrxDetail.AmountTypeListId = 1; // credit
                        masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                        masterTrxDetail.ClassId = vm.CustomerCredit.CustomerId;
                        masterTrxDetail.TypelistId = 1;
                        masterTrxDetail.Transactiondate = vm.CreatedDate;
                        masterTrxDetail.DetailDescription = vm.CreditDescription;
                        masterTrxDetail.ModifiedBy = vm.CreatedBy;
                        masterTrxDetail.ModifiedDate = vm.CreatedDate;
                        masterTrxDetail.FRRevenues = true;
                        masterTrxDetail.FRDeduction = false;
                        masterTrxDetail.BPPAdmin = 0;
                        context.Entry(masterTrxDetail).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                    lastMasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;

                    //Insert customer taxes
                    MasterTrxTax customerTax = null;
                    if (vm.Id != -1)
                        customerTax = context.MasterTrxTaxes.Where(o => o.MasterTrxDetailId == masterTrxDetail.MasterTrxDetailId).FirstOrDefault();

                    if (customerTax == null)
                    {
                        customerTax = new MasterTrxTax();
                        customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                        customerTax.Amount = cvm.Tax;
                        customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.CreditAmount, 2);
                        customerTax.AmountTypeListId = 1; // credit
                        customerTax.IsDelete = false;
                        customerTax.InvoiceId = masterTrxDetail.InvoiceId;
                        customerTax.RegionId = masterTrxDetail.RegionId;
                        customerTax.PeriodId = masterTrxDetail.PeriodId;
                        customerTax.CustomerId = masterTrxDetail.ClassId;
                        customerTax.CreatedBy = vm.CreatedBy;
                        customerTax.CreatedDate = vm.CreatedDate;
                        customerTax.FRRevenues = true;
                        foreach (var item in vm.FranchiseeCredits)
                        {
                            customerTax.FranchiseeId = ((masterTrxDetail.LineNo == item.Credit?.LineNo) ? item.FranchiseeId : 0);
                        }
                        context.MasterTrxTaxes.Add(customerTax);
                    }
                    else
                    {
                        customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                        customerTax.Amount = cvm.Tax;
                        customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.CreditAmount, 2);
                        customerTax.AmountTypeListId = 1; // credit
                        customerTax.IsDelete = false;
                        customerTax.InvoiceId = masterTrxDetail.InvoiceId;
                        customerTax.RegionId = masterTrxDetail.RegionId;
                        customerTax.PeriodId = masterTrxDetail.PeriodId;
                        customerTax.CustomerId = masterTrxDetail.ClassId;
                        customerTax.ModifiedBy = vm.CreatedBy;
                        customerTax.ModifiedDate = vm.CreatedDate;
                        customerTax.FRRevenues = true;
                        foreach (var item in vm.FranchiseeCredits)
                        {
                            customerTax.FranchiseeId = ((masterTrxDetail.LineNo == item.Credit?.LineNo) ? item.FranchiseeId : 0);
                        }




                        context.Entry(customerTax).State = EntityState.Modified;
                    }

                    var invoice = context.Invoices.Where(o => o.InvoiceId == masterTrxDetail.InvoiceId).FirstOrDefault();
                    var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                    var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

                    List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
                    decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
                    decimal totalTransactions = 0.00m;
                    decimal grandTotalTransactions = 0.00m;

                    foreach (var trx in invoiceTransactions)
                    {
                        totalTransactions = totalTransactions + (decimal)trx.Total;
                    }
                    grandTotalTransactions = totalTransactions + cvm.Total;


                    if (grandTotalTransactions >= invoiceTotal)
                    {
                        invoice.TransactionStatusListId = 6; /*6 = Paid*/
                        invoiceMasterTrx.StatusId = 6;
                    }
                    else
                    {
                        invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
                        invoiceMasterTrx.StatusId = 7;
                    }

                    context.SaveChanges();

                    totalCustomerCredit += cvm.CreditAmount;
                    totalCustomerTaxes += cvm.Tax;
                }

                // update original invoice's status

                Invoice existingInvoice = context.Invoices.Where(o => o.InvoiceId == vm.InvoiceId).FirstOrDefault();
                if (existingInvoice != null)
                {
                    existingInvoice.TransactionStatusListId = vm.PaidInFull ? 6 : 7; // paid : paid partial
                    existingInvoice.ModifiedBy = vm.CreatedBy;
                    existingInvoice.ModifiedDate = vm.CreatedDate;
                    context.Entry(existingInvoice).State = EntityState.Modified;
                    context.SaveChanges();
                }

                // extra credit

                if (vm.IsExtraCredit)
                {
                    decimal unappliedCredit = vm.ApplyTotalCredit - vm.TotalCredit;

                    Overflow overflowPayment = new Overflow();

                    overflowPayment.AmountTypeListId = 1;
                    overflowPayment.CheckAmount = vm.ApplyTotalCredit;
                    //string _outCheckNumber = "0";
                    //string.TryParse(inputdata.ReferenceNo, out _outCheckNumber);
                    overflowPayment.CheckNumber = "";
                    overflowPayment.MasterTrxDetailId = lastMasterTrxDetailId;
                    overflowPayment.PeriodId = (PR != null ? PR.PeriodId : 0);

                    overflowPayment.TypeListId = 1; // customer
                    overflowPayment.ClassId = vm.CustomerCredit.CustomerId;
                    overflowPayment.MasterTrxTypeListId = 24; // overpayment applied
                    overflowPayment.TransactionDate = vm.CreatedDate;
                    overflowPayment.RegionId = vm.RegionId;
                    overflowPayment.IsActive = true;
                    overflowPayment.Amount = unappliedCredit;
                    overflowPayment.TransactionStatusListId = 4; // open
                    overflowPayment.CreatedBy = vm.CreatedBy;
                    overflowPayment.CreatedDate = vm.CreatedDate;

                    context.Overflows.Add(overflowPayment);
                    context.SaveChanges();

                    //decimal unappliedCredit = vm.TotalCredit;

                    //MasterTrxDetail masterTrxDetail = null;
                    //if (vm.Id != -1)
                    //    masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxTypeListId == 18 && o.SourceTypeListId == 4 && o.SourceId == customerCredit.CreditId).FirstOrDefault();

                    //MasterTrx masterTrx = null;
                    //if (masterTrxDetail != null)
                    //    masterTrx = context.MasterTrxes.Where(o => o.MasterTrxId == masterTrxDetail.MasterTrxId).FirstOrDefault();

                    //if (masterTrx == null)
                    //{
                    //    masterTrx = new MasterTrx();
                    //    masterTrx.TypeListId = 1; // customer
                    //    masterTrx.ClassId = customerCredit.ClassId;
                    //    masterTrx.MasterTrxTypeListId = 18; // extra credit
                    //    masterTrx.TrxDate = vm.CreatedDate;
                    //    masterTrx.RegionId = vm.RegionId;
                    //    masterTrx.StatusId = 1; // open
                    //    masterTrx.BillMonth = vm.CreatedDate.Month;
                    //    masterTrx.BillYear = vm.CreatedDate.Year;
                    //    masterTrx.HeaderId = customerCredit.CreditId;

                    //    context.MasterTrxes.Add(masterTrx);
                    //}
                    //else
                    //{
                    //    context.Entry(masterTrx).State = EntityState.Modified;
                    //}

                    //context.SaveChanges();

                    //if (masterTrxDetail == null)
                    //{
                    //    masterTrxDetail = new MasterTrxDetail();
                    //    masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
                    //    masterTrxDetail.InvoiceId = null;
                    //    masterTrxDetail.LineNo = null;
                    //    masterTrxDetail.MasterTrxTypeListId = 18; // extra credit
                    //    masterTrxDetail.SourceTypeListId = 4; // credit
                    //    masterTrxDetail.SourceId = customerCredit.CreditId;
                    //    masterTrxDetail.RegionId = vm.RegionId;

                    //    masterTrxDetail.CreatedBy = vm.CreatedBy;
                    //    masterTrxDetail.CreatedDate = vm.CreatedDate;
                    //    masterTrxDetail.IsDelete = false;

                    //    context.MasterTrxDetails.Add(masterTrxDetail);
                    //}
                    //else
                    //{
                    //    masterTrxDetail.ModifiedBy = vm.CreatedBy;
                    //    masterTrxDetail.ModifiedDate = vm.CreatedDate;

                    //    context.Entry(masterTrxDetail).State = EntityState.Modified;
                    //}

                    //masterTrxDetail.ServiceTypeListId = null;
                    //masterTrxDetail.FeesDetail = false;
                    //masterTrxDetail.TaxDetail = false;
                    //masterTrxDetail.Total = unappliedCredit;
                    //masterTrxDetail.AmountTypeListId = 1; // credit

                    //context.SaveChanges();

                    totalCustomerCredit += unappliedCredit;
                }

                //// general ledger for customer credit -- credit to Business Svc. Income

                //if (totalCustomerCredit > 0)
                //{
                //    GeneralLedgerTrx oGL = new GeneralLedgerTrx();
                //    oGL.Amount = totalCustomerCredit;
                //    oGL.AmountTypeListId = 2;
                //    oGL.CreatedBy = LoginUserId;
                //    oGL.CreatedDate = DateTime.Now;
                //    oGL.Credit = 0;
                //    oGL.Debit = totalCustomerCredit;
                //    oGL.IsDelete = false;
                //    oGL.LedgerAccountId = 139;
                //    oGL.MasterTrxId = customerMasterTrx.MasterTrxId;
                //    oGL.MasterTrxTypeListId = 3;
                //    oGL.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                //    oGL.RegionId = vm.RegionId;
                //    oGL.TrxDate = vm.CreatedDate;

                //    MasterTrx oTMasterTrx = context.MasterTrxes.FirstOrDefault(m => m.HeaderId == vm.InvoiceId && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5));
                //    if (oTMasterTrx != null)
                //    {
                //        AccountTypeList oTAccountTypeList = context.AccountTypeLists.FirstOrDefault(m => m.AccountTypeListId == oTMasterTrx.AccountTypeListId);
                //        if (oTAccountTypeList != null)
                //        {
                //            oGL.LedgerAccountId = oTAccountTypeList.LedgerSubAcctId;
                //        }
                //    }

                //    context.GeneralLedgerTrxes.Add(oGL);
                //    context.SaveChanges();
                //}

                //// general ledger for customer credit -- credit to Customer Sales Tax

                //if (totalCustomerTaxes > 0)
                //{

                //    GeneralLedgerTrx oGL = new GeneralLedgerTrx();
                //    oGL.Amount = totalCustomerCredit;
                //    oGL.AmountTypeListId = 2;
                //    oGL.CreatedBy = LoginUserId;
                //    oGL.CreatedDate = DateTime.Now;
                //    oGL.Credit = 0;
                //    oGL.Debit = totalCustomerCredit;
                //    oGL.IsDelete = false;
                //    oGL.LedgerAccountId = 6;
                //    oGL.MasterTrxId = customerMasterTrx.MasterTrxId;
                //    oGL.MasterTrxTypeListId = 3;
                //    oGL.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                //    oGL.RegionId = vm.RegionId;
                //    oGL.TrxDate = vm.CreatedDate;
                //    context.GeneralLedgerTrxes.Add(oGL);
                //    context.SaveChanges();
                //}

                //// general ledger for customer credit -- debit from A/R Janitorial Svc.

                //if ((totalCustomerCredit + totalCustomerTaxes) > 0)
                //{
                //    GeneralLedgerTrx oGL = new GeneralLedgerTrx();
                //    oGL.Amount = totalCustomerCredit;
                //    oGL.AmountTypeListId = 1;
                //    oGL.CreatedBy = LoginUserId;
                //    oGL.CreatedDate = DateTime.Now;
                //    oGL.Credit = 0;
                //    oGL.Debit = totalCustomerCredit;
                //    oGL.IsDelete = false;
                //    oGL.LedgerAccountId = 3;
                //    oGL.MasterTrxId = customerMasterTrx.MasterTrxId;
                //    oGL.MasterTrxTypeListId = 3;
                //    oGL.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                //    oGL.RegionId = vm.RegionId;
                //    oGL.TrxDate = vm.CreatedDate;
                //    context.GeneralLedgerTrxes.Add(oGL);
                //    context.SaveChanges();
                //}




                foreach (FranchiseeCreditViewModel fcvm in vm.FranchiseeCredits)
                {
                    CreditFranchisee franchiseeCredit = null;
                    if (vm.Id != -1)
                        franchiseeCredit = context.CreditFranchisees.Where(o => o.CreditId == customerCredit.CreditId && o.FranchiseeId == fcvm.FranchiseeId).FirstOrDefault();

                    MasterTrx franchiseeMasterTrx = null;
                    if (franchiseeCredit != null)
                        franchiseeMasterTrx = context.MasterTrxes.Where(o => o.MasterTrxId == franchiseeCredit.MasterTrxId).FirstOrDefault();

                    MasterTrxDetail franchiseeMasterTrxDetail = null;
                    if (vm.Id != -1)
                        franchiseeMasterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxId == franchiseeMasterTrx.MasterTrxId && o.LineNo == fcvm.Credit.LineNo).FirstOrDefault();

                    // compute franchisee credit fees

                    decimal totalFees = 0;

                    List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Credit.BaseMasterTrxDetailId).ToList();
                    List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

                    foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                    {
                        MasterTrxFeeDetail feeDetail = null;
                        if (vm.Id != -1 && franchiseeMasterTrxDetail != null)
                            feeDetail = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == franchiseeMasterTrxDetail.MasterTrxDetailId && o.FeeId == feeDef.FeeId).FirstOrDefault();

                        if (feeDetail == null)
                        {
                            feeDetail = new MasterTrxFeeDetail();
                            feeDetail.MasterTrxFeeDetailId = -1;
                            feeDetail.FeeId = feeDef.FeeId;
                            feeDetail.AmountTypeListId = 1; // credit
                            feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                            feeDetail.CreatedBy = vm.CreatedBy;
                            feeDetail.CreatedDate = vm.CreatedDate;
                            feeDetail.FRRevenues = false;
                            feeDetail.FRDeduction = true;
                            feeDetail.RegionId = vm.RegionId;
                            feeDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                            feeDetail.BillingPayId = fcvm.BillingPayId;
                            feeDetail.FranchiseeId = fcvm.FranchiseeId;
                        }

                        if (feeDef.FeePercentage != null) // percentage
                        {
                            feeDetail.FeePercentage = feeDef.FeePercentage;
                            feeDetail.Amount = (decimal)(fcvm.Credit.CreditAmount * feeDetail.FeePercentage / 100.0M);// Math.Round((decimal)(fcvm.Credit.CreditAmount * feeDetail.FeePercentage / 100.0M), 2);
                        }
                        else // flat amount
                        {
                            feeDetail.Amount = feeDef.Amount;
                            feeDetail.FeePercentage = null;
                        }

                        totalFees += feeDetail.Amount ?? 0;

                        franchiseeFees.Add(feeDetail);
                    }

                    // franchisee credit mastertrx

                    if (franchiseeMasterTrx == null)
                    {
                        franchiseeMasterTrx = new MasterTrx();
                        franchiseeMasterTrx.MasterTrxTypeListId = 8; // franchisee credit
                        franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
                        franchiseeMasterTrx.TypeListId = 2; // franchisee
                        franchiseeMasterTrx.RegionId = vm.RegionId;
                        franchiseeMasterTrx.TrxDate = vm.CreatedDate;
                        franchiseeMasterTrx.BillMonth = vm.BillMonth;
                        franchiseeMasterTrx.BillYear = vm.BillYear;
                        franchiseeMasterTrx.StatusId = 1; // open
                        franchiseeMasterTrx.CreatedBy = vm.CreatedBy;
                        franchiseeMasterTrx.CreatedDate = DateTime.Now;
                        franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                        context.MasterTrxes.Add(franchiseeMasterTrx);
                    }
                    else
                    {
                        // todo: add mastertrx modifiedby/date
                        franchiseeMasterTrx.TrxDate = vm.CreatedDate;
                        franchiseeMasterTrx.BillMonth = vm.BillMonth;
                        franchiseeMasterTrx.BillYear = vm.BillYear;
                        franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                        franchiseeMasterTrx.ModifiedBy = vm.CreatedBy;
                        franchiseeMasterTrx.ModifiedDate = DateTime.Now;
                        context.Entry(franchiseeMasterTrx).State = EntityState.Modified;
                    }

                    context.SaveChanges();

                    // franchisee credit

                    franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(8, vm.RegionId);
                    string franchiseeNextTrxNumber = null;

                    if (franchiseeCredit == null)
                    {
                        if (franchiseeTransactionNumberConfigViewModel != null)
                            franchiseeNextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(8, vm.RegionId, vm.CreatedDate);

                        franchiseeCredit = new CreditFranchisee();
                        franchiseeCredit.BillingPayId = fcvm.BillingPayId;
                        franchiseeCredit.FranchiseeId = fcvm.FranchiseeId;
                        franchiseeCredit.RegionId = vm.RegionId;
                        franchiseeCredit.TransactionStatusListId = 1; // pending approval
                        franchiseeCredit.TransactionNumber = franchiseeNextTrxNumber;
                        franchiseeCredit.isActive = true;
                        franchiseeCredit.CreatedBy = vm.CreatedBy;
                        franchiseeCredit.CreatedDate = vm.CreatedDate;

                        franchiseeCredit.PeriodId = (PR != null ? PR.PeriodId : 0);
                        franchiseeCredit.TransactionDate = vm.CreatedDate;
                        franchiseeCredit.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        franchiseeCredit.CreditId = customerCredit.CreditId;
                        franchiseeCredit.CreditFranchiseeDescription = vm.CreditDescription;
                        context.CreditFranchisees.Add(franchiseeCredit);
                    }
                    else
                    {
                        // todo: add creditfranchisee modifiedby/date
                        franchiseeCredit.CreditFranchiseeDescription = vm.CreditDescription;
                        franchiseeCredit.PeriodId = (PR != null ? PR.PeriodId : 0);
                        franchiseeCredit.TransactionDate = vm.CreatedDate;
                        franchiseeCredit.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        franchiseeCredit.CreditId = customerCredit.CreditId;
                        franchiseeCredit.CreatedBy = LoginUserId;
                        franchiseeCredit.CreatedDate = DateTime.Now;
                        context.Entry(franchiseeCredit).State = EntityState.Modified;
                    }
                    context.SaveChanges();

                    franchiseeMasterTrx.HeaderId = franchiseeCredit.CreditFranchiseeId;
                    context.SaveChanges();

                    decimal creditMinusFees = (decimal)(fcvm.Credit.CreditAmount - totalFees);// Math.Round((decimal)(fcvm.Credit.CreditAmount - totalFees), 2); // credit amount after taking out fees

                    if (franchiseeMasterTrxDetail == null)
                    {
                        franchiseeMasterTrxDetail = new MasterTrxDetail();
                        franchiseeMasterTrxDetail.InvoiceId = vm.InvoiceId;
                        franchiseeMasterTrxDetail.LineNo = fcvm.Credit.LineNo;
                        franchiseeMasterTrxDetail.MasterTrxTypeListId = 8; // franchisee credit
                        franchiseeMasterTrxDetail.HeaderId = franchiseeCredit.CreditFranchiseeId;
                        franchiseeMasterTrxDetail.RegionId = vm.RegionId;
                        franchiseeMasterTrxDetail.ServiceTypeListId = fcvm.Credit.ServiceTypeListId != 0 ? fcvm.Credit.ServiceTypeListId : vm.CustomerCredit.Credits[0].ServiceTypeListId;

                        franchiseeMasterTrxDetail.CreatedBy = vm.CreatedBy;
                        franchiseeMasterTrxDetail.CreatedDate = vm.CreatedDate;
                        franchiseeMasterTrxDetail.IsDelete = false;

                        franchiseeMasterTrxDetail.Quantity = 1;
                        franchiseeMasterTrxDetail.UnitPrice = fcvm.Credit.CreditAmount;
                        franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Credit.CreditAmount;
                        franchiseeMasterTrxDetail.FeesDetail = true;
                        franchiseeMasterTrxDetail.TaxDetail = false;
                        franchiseeMasterTrxDetail.TotalFee = totalFees;
                        franchiseeMasterTrxDetail.Total = creditMinusFees;
                        franchiseeMasterTrxDetail.AmountTypeListId = 2; // credit
                        franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                        franchiseeMasterTrxDetail.ClassId = franchiseeMasterTrx.ClassId;
                        franchiseeMasterTrxDetail.TypelistId = 2;
                        franchiseeMasterTrxDetail.Transactiondate = vm.CreatedDate;
                        franchiseeMasterTrxDetail.DetailDescription = vm.CreditDescription;
                        franchiseeMasterTrxDetail.FRRevenues = true;
                        franchiseeMasterTrxDetail.FRDeduction = false;
                        franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        franchiseeMasterTrxDetail.BPPAdmin = 0;

                        List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == vm.InvoiceId && m.LineNo == fcvm.Credit.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                        if (_masterINVDetail.Count > 0)
                        {
                            franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                            franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                            franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                            //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                            franchiseeMasterTrxDetail.Quantity = 0;
                            franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                            franchiseeMasterTrxDetail.TotalTax = 0;
                            franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
                            franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                            franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                            franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                            franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                        }

                        List<FranchiseeBillSetting> lstFranchiseeBillSetting = context.FranchiseeBillSettings.Where(h => h.FranchiseeId == franchiseeMasterTrx.ClassId).ToList();
                        if (lstFranchiseeBillSetting.Count > 0)
                        {
                            if (lstFranchiseeBillSetting[0].BBPAdministrationFee == true)
                            {
                                franchiseeMasterTrxDetail.BPPAdmin = 1;
                            }
                            else { franchiseeMasterTrxDetail.BPPAdmin = 0; }
                            if (lstFranchiseeBillSetting[0].AccountRebate == true)
                            {
                                franchiseeMasterTrxDetail.AccountRebate = 1;
                            }
                            else { franchiseeMasterTrxDetail.AccountRebate = 0; }

                        }


                        context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                    }
                    else
                    {
                        franchiseeMasterTrxDetail.ModifiedBy = vm.CreatedBy;
                        franchiseeMasterTrxDetail.ModifiedDate = DateTime.Now;

                        franchiseeMasterTrxDetail.Quantity = 1;
                        franchiseeMasterTrxDetail.UnitPrice = fcvm.Credit.CreditAmount;
                        franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Credit.CreditAmount;
                        franchiseeMasterTrxDetail.FeesDetail = true;
                        franchiseeMasterTrxDetail.TaxDetail = false;
                        franchiseeMasterTrxDetail.TotalFee = totalFees;
                        franchiseeMasterTrxDetail.Total = creditMinusFees;
                        franchiseeMasterTrxDetail.AmountTypeListId = 2; // credit
                        franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                        franchiseeMasterTrxDetail.ClassId = franchiseeMasterTrx.ClassId;
                        franchiseeMasterTrxDetail.TypelistId = 2;
                        franchiseeMasterTrxDetail.Transactiondate = vm.CreatedDate;
                        franchiseeMasterTrxDetail.DetailDescription = vm.CreditDescription;
                        franchiseeMasterTrxDetail.FRRevenues = true;
                        franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                        franchiseeMasterTrxDetail.FRDeduction = false;
                        franchiseeMasterTrxDetail.BPPAdmin = 0;

                        List<FranchiseeBillSetting> lstFranchiseeBillSetting = context.FranchiseeBillSettings.Where(h => h.FranchiseeId == franchiseeMasterTrx.ClassId).ToList();
                        if (lstFranchiseeBillSetting.Count > 0)
                        {
                            if (lstFranchiseeBillSetting[0].BBPAdministrationFee == true)
                            {
                                franchiseeMasterTrxDetail.BPPAdmin = 1;
                            }
                            else { franchiseeMasterTrxDetail.BPPAdmin = 0; }
                            if (lstFranchiseeBillSetting[0].AccountRebate == true)
                            {
                                franchiseeMasterTrxDetail.AccountRebate = 1;
                            }
                            else { franchiseeMasterTrxDetail.AccountRebate = 0; }

                        }
                        context.Entry(franchiseeMasterTrxDetail).State = EntityState.Modified;
                    }
                    context.SaveChanges();

                    if (franchiseeNextTrxNumber != null)
                    {
                        franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                    }

                    // insert franchisee fees

                    foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
                    {
                        feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

                        if (feeDetail.MasterTrxFeeDetailId == -1)
                            context.MasterTrxFeeDetails.Add(feeDetail);
                        else
                            context.Entry(feeDetail).State = EntityState.Modified;
                    }

                    context.SaveChanges();



                    //if ((totalFees + creditMinusFees) > 0)
                    //{
                    //    GeneralLedgerTrx oGL = new GeneralLedgerTrx();
                    //    oGL.Amount = (totalFees + creditMinusFees);
                    //    oGL.AmountTypeListId = 1;
                    //    oGL.CreatedBy = LoginUserId;
                    //    oGL.CreatedDate = DateTime.Now;
                    //    oGL.Credit = (totalFees + creditMinusFees);
                    //    oGL.Debit = 0;
                    //    oGL.IsDelete = false;
                    //    oGL.LedgerAccountId = 7;
                    //    oGL.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //    oGL.MasterTrxTypeListId = 8;
                    //    oGL.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                    //    oGL.RegionId = vm.RegionId;
                    //    oGL.TrxDate = vm.CreatedDate;
                    //    context.GeneralLedgerTrxes.Add(oGL);
                    //    context.SaveChanges();
                    //}
                    //if (creditMinusFees > 0)
                    //{
                    //    GeneralLedgerTrx oGL = new GeneralLedgerTrx();
                    //    oGL.Amount = creditMinusFees;
                    //    oGL.AmountTypeListId = 2;
                    //    oGL.CreatedBy = LoginUserId;
                    //    oGL.CreatedDate = DateTime.Now;
                    //    oGL.Credit = 0;
                    //    oGL.Debit = creditMinusFees;
                    //    oGL.IsDelete = false;
                    //    oGL.LedgerAccountId = 7;
                    //    oGL.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //    oGL.MasterTrxTypeListId = 8;
                    //    oGL.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                    //    oGL.RegionId = vm.RegionId;
                    //    oGL.TrxDate = vm.CreatedDate;
                    //    context.GeneralLedgerTrxes.Add(oGL);
                    //    context.SaveChanges();
                    //}
                    //if (totalFees > 0)
                    //{
                    //    GeneralLedgerTrx oGL = new GeneralLedgerTrx();
                    //    foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                    //    {
                    //        oGL = new GeneralLedgerTrx();
                    //        oGL.Amount = totalFees;
                    //        oGL.AmountTypeListId = 2;
                    //        oGL.CreatedBy = LoginUserId;
                    //        oGL.CreatedDate = DateTime.Now;
                    //        oGL.Credit = 0;
                    //        oGL.Debit = totalFees;
                    //        oGL.IsDelete = false;
                    //        //oGL.LedgerAccountId = 7;
                    //        oGL.MasterTrxId = customerMasterTrx.MasterTrxId;
                    //        oGL.MasterTrxTypeListId = 8;
                    //        oGL.PeriodId = (PR != null ? PR.PeriodId : 0); ;
                    //        oGL.RegionId = vm.RegionId;
                    //        oGL.TrxDate = vm.CreatedDate;


                    //        Fee oFee = context.Fees.Where(o => o.FeeId == feeDef.FeeId).FirstOrDefault();
                    //        if (oFee != null)
                    //        {
                    //            oGL.LedgerAccountId = oFee.LedgerAccountId;
                    //        }
                    //        if (feeDef.FeePercentage != null) // percentage
                    //        {
                    //            oGL.Amount = Math.Round((decimal)(fcvm.Credit.CreditAmount * feeDef.FeePercentage / 100.0M), 2);
                    //        }
                    //        else // flat amount
                    //        {
                    //            oGL.Amount = feeDef.Amount;
                    //        }
                    //        context.GeneralLedgerTrxes.Add(oGL);
                    //        context.SaveChanges();
                    //    }

                    //}



                }

                if (customerNextTrxNumber != null)
                {
                    customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
                    CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                }

                return true;
            }
        }



        public bool InsertOrUpdateCreditTransactionInTemp(CreditTransactionViewModel vm)
        {
            bool retVal = true;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var _Period = context.Periods.SingleOrDefault(p => p.BillMonth == vm.BillMonth && p.BillYear == vm.BillYear);
                var _Contract = context.Contracts.Where(p => p.CustomerId == vm.CustomerCredit.CustomerId && p.isActive == true).FirstOrDefault();

                CreditTemp oCreditTemp = new CreditTemp();
                oCreditTemp.InvoiceId = vm.InvoiceId;
                oCreditTemp.AccountTypeListId = _Contract != null ? _Contract.AccountTypeListId : 0;
                oCreditTemp.CreditDescription = vm.CreditDescription;
                oCreditTemp.CreditReasonListId = vm.CreditReasonListId;
                oCreditTemp.MasterTrxTypeListId = 3;
                oCreditTemp.TransactionDate = vm.CreatedDate;
                oCreditTemp.TransactionStatusListId = 1;
                oCreditTemp.PeriodId = _Period.PeriodId;
                oCreditTemp.RegionId = vm.RegionId;
                oCreditTemp.CreatedBy = LoginUserId;
                oCreditTemp.CreatedDate = DateTime.Now;
                oCreditTemp.TransactionNumber = DateTime.Now.ToString("MMddyyyyHHmmss");
                oCreditTemp.CustomerId = vm.CustomerCredit.CustomerId;


                context.CreditTemps.Add(oCreditTemp);
                context.SaveChanges();

                List<CreditTempDetail> lstCreditTempDetail = new List<CreditTempDetail>();
                CreditTempDetail oCreditTempDetail;
                foreach (CreditViewModel cvm in vm.CustomerCredit.Credits)
                {
                    oCreditTempDetail = new CreditTempDetail();
                    //oCreditTempDetail.CreditTempDetailId;
                    oCreditTempDetail.CreditTempId = oCreditTemp.CreditTempId;
                    oCreditTempDetail.ClassId = oCreditTemp.CustomerId;
                    oCreditTempDetail.TypelistId = 1;
                    oCreditTempDetail.MasterTrxTypeListId = 3;
                    oCreditTempDetail.ServiceTypeListId = cvm.ServiceTypeListId;
                    //oCreditTempDetail.FC_BillingPayId;
                    //oCreditTempDetail.FC_CreditTempDetailId;
                    //oCreditTempDetail.FC_TransactionNumber;
                    //oCreditTempDetail.SourceId;
                    //oCreditTempDetail.SourceTypeListId;
                    oCreditTempDetail.LineNo = cvm.LineNo;
                    oCreditTempDetail.DetailDescription = vm.CreditDescription;
                    oCreditTempDetail.ExtendedPrice = cvm.CreditAmount;
                    oCreditTempDetail.FeesDetail = false;
                    oCreditTempDetail.TotalFee = 0;
                    oCreditTempDetail.TaxDetail = true;
                    oCreditTempDetail.TotalTax = Math.Round(cvm.Tax,2);
                    oCreditTempDetail.Total = cvm.Total;
                    oCreditTempDetail.AmountTypeListId = 1;
                    oCreditTempDetail.TransactionStatusListId = 1;
                    oCreditTempDetail.IsDelete = false;
                    oCreditTempDetail.CreatedBy = LoginUserId;
                    oCreditTempDetail.CreatedDate = DateTime.Now;
                    oCreditTempDetail.BaseMasterTrxDetailId = cvm.BaseMasterTrxDetailId;
                    //oCreditTempDetail.ModifiedBy;
                    //oCreditTempDetail.ModifiedDate;

                    context.CreditTempDetails.Add(oCreditTempDetail);
                    context.SaveChanges();

                    lstCreditTempDetail.Add(oCreditTempDetail);
                }

                foreach (FranchiseeCreditViewModel fcvm in vm.FranchiseeCredits)
                {


                    decimal totalFees = 0;

                    List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Credit.BaseMasterTrxDetailId).ToList();


                    foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                    {
                        decimal? _feeAmount = feeDef.Amount;
                        if (feeDef.FeePercentage != null) // percentage
                        {
                            _feeAmount = (decimal)(fcvm.Credit.CreditAmount * feeDef.FeePercentage / 100.0M);// Math.Round((decimal)(fcvm.Credit.CreditAmount * feeDef.FeePercentage / 100.0M), 2);
                        }

                        totalFees += _feeAmount ?? 0;
                    }


                    var _creditTempDetail = lstCreditTempDetail.Where(j => j.LineNo == fcvm.Credit.LineNo).FirstOrDefault();
                    oCreditTempDetail = new CreditTempDetail();
                    //oCreditTempDetail.CreditTempDetailId;
                    oCreditTempDetail.CreditTempId = oCreditTemp.CreditTempId;
                    oCreditTempDetail.ClassId = fcvm.FranchiseeId;
                    oCreditTempDetail.TypelistId = 2;
                    oCreditTempDetail.MasterTrxTypeListId = 8;
                    oCreditTempDetail.ServiceTypeListId = fcvm.Credit.ServiceTypeListId;
                    oCreditTempDetail.FC_BillingPayId = fcvm.BillingPayId;
                    oCreditTempDetail.FC_CreditTempDetailId = (_creditTempDetail != null ? _creditTempDetail.CreditTempDetailId : -1);
                    //oCreditTempDetail.FC_TransactionNumber;
                    //oCreditTempDetail.SourceId;
                    //oCreditTempDetail.SourceTypeListId;
                    oCreditTempDetail.LineNo = fcvm.Credit.LineNo;
                    oCreditTempDetail.DetailDescription = vm.CreditDescription;
                    oCreditTempDetail.ExtendedPrice = fcvm.Credit.CreditAmount;
                    oCreditTempDetail.FeesDetail = true;
                    oCreditTempDetail.TotalFee = totalFees;
                    oCreditTempDetail.TaxDetail = false;
                    oCreditTempDetail.TotalTax = 0;
                    oCreditTempDetail.Total = fcvm.Credit.CreditAmount - totalFees;
                    oCreditTempDetail.AmountTypeListId = 1;
                    oCreditTempDetail.TransactionStatusListId = 1;
                    oCreditTempDetail.IsDelete = false;
                    oCreditTempDetail.CreatedBy = LoginUserId;
                    oCreditTempDetail.CreatedDate = DateTime.Now;
                    //oCreditTempDetail.ModifiedBy;
                    //oCreditTempDetail.ModifiedDate;
                    oCreditTempDetail.BaseMasterTrxDetailId = fcvm.Credit.BaseMasterTrxDetailId;

                    context.CreditTempDetails.Add(oCreditTempDetail);
                    context.SaveChanges();
                }
            }

            return retVal;
        }

        public bool InsertOrUpdateCreditTaxTransactionInTemp(int _invoiceId, decimal _ApplyTaxAmount, DateTime _TrxDate, string TrxDesc, int reasonListId)
        {
            bool retVal = true;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Invoice invoice = context.Invoices.SingleOrDefault(p => p.InvoiceId == _invoiceId);
                CreditDetailViewModel creditDetailViewModel = GetCreditDetailForInvoiceN(_invoiceId);
                var invoiceDetails = GetInvoiceDetailData(_invoiceId);
                MasterTrxDetail masterTrxDetail = context.MasterTrxDetails.FirstOrDefault(p => p.InvoiceId == _invoiceId && p.MasterTrxTypeListId == 5);
                var _Period = context.Periods.SingleOrDefault(p => p.BillMonth == invoiceDetails.InvoiceDetail.BillMonth && p.BillYear == invoiceDetails.InvoiceDetail.BillYear);
                var _Contract = context.Contracts.Where(p => p.CustomerId == invoice.ClassId && p.isActive == true).FirstOrDefault();

                CreditTemp oCreditTemp = new CreditTemp();
                oCreditTemp.InvoiceId = invoiceDetails.InvoiceDetail.InvoiceId;
                oCreditTemp.AccountTypeListId = _Contract != null ? _Contract.AccountTypeListId : 0;
                oCreditTemp.CreditDescription = TrxDesc;
                oCreditTemp.CreditReasonListId = reasonListId;
                oCreditTemp.MasterTrxTypeListId = 58;// for tax credit
                oCreditTemp.TransactionDate = _TrxDate;
                oCreditTemp.TransactionStatusListId = 1;
                oCreditTemp.PeriodId = _Period.PeriodId;
                oCreditTemp.RegionId = invoiceDetails.InvoiceDetail.RegionId;
                oCreditTemp.CreatedBy = LoginUserId;
                oCreditTemp.CreatedDate = DateTime.Now;
                oCreditTemp.TransactionNumber = DateTime.Now.ToString("MMddyyyyHHmmss");
                oCreditTemp.CustomerId = invoice.ClassId;


                context.CreditTemps.Add(oCreditTemp);
                context.SaveChanges();

                CreditTempDetail oCreditTempDetail = new CreditTempDetail();
                //oCreditTempDetail.CreditTempDetailId;
                oCreditTempDetail.CreditTempId = oCreditTemp.CreditTempId;
                oCreditTempDetail.ClassId = oCreditTemp.CustomerId;
                oCreditTempDetail.TypelistId = 1;
                oCreditTempDetail.MasterTrxTypeListId = 58;//58 for tax credit
                oCreditTempDetail.ServiceTypeListId = masterTrxDetail.ServiceTypeListId;
                oCreditTempDetail.LineNo = masterTrxDetail.LineNo;
                oCreditTempDetail.DetailDescription = TrxDesc;
                oCreditTempDetail.ExtendedPrice = 0;// Only tax credit
                oCreditTempDetail.FeesDetail = false;
                oCreditTempDetail.TotalFee = 0;
                oCreditTempDetail.TaxDetail = true;
                oCreditTempDetail.TotalTax = _ApplyTaxAmount;
                oCreditTempDetail.Total = _ApplyTaxAmount;
                oCreditTempDetail.AmountTypeListId = 1;
                oCreditTempDetail.TransactionStatusListId = 1;
                oCreditTempDetail.IsDelete = false;
                oCreditTempDetail.CreatedBy = LoginUserId;
                oCreditTempDetail.CreatedDate = DateTime.Now;
                oCreditTempDetail.BaseMasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                //oCreditTempDetail.ModifiedBy;
                //oCreditTempDetail.ModifiedDate;

                context.CreditTempDetails.Add(oCreditTempDetail);
                context.SaveChanges();


            }

            return retVal;
        }



        public bool InsertOrUpdateBalanceAdjustmentRefund(List<CreditTransactionViewModel> vm, int mastertrxid)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                if (vm.Count() > 0)
                {
                    var _mainObject = vm.FirstOrDefault();
                    Period _period = context.Periods.Where(f => f.BillMonth == _mainObject.BillMonth && f.BillYear == _mainObject.BillYear).FirstOrDefault();

                    foreach (var item in vm)
                    {
                        APBill _apbill = new APBill();
                        _apbill.BillMonth = item.BillMonth;
                        _apbill.BillYear = item.BillYear;
                        _apbill.CheckAmount = Math.Abs(item.TotalCredit);
                        //_apbill.CheckBookTransactionListId;
                        _apbill.CheckBookTransactionTypeListId = 17;
                        _apbill.CheckTypeListId = 19;
                        _apbill.ClassId = item.CustomerCredit.CustomerId;
                        _apbill.CreatedBy = LoginUserId;
                        _apbill.CreatedDate = DateTime.Now;
                        //_apbill.FranchiseeReportFinalizedId;
                        //_apbill.FranchiseeReportId;
                        //_apbill.FranchiseeTurnaroundCheckId;
                        _apbill.IsDelete = false;
                        //_apbill.ManualCheckId;
                        _apbill.MasterTrxId = mastertrxid;
                        //_apbill.PayTypeListId;
                        _apbill.PeriodId = _period.PeriodId;
                        _apbill.RegionId = SelectedRegionId;
                        //_apbill.SourceId;
                        //_apbill.SourceTypeListId;
                        _apbill.TransactionDate = item.CreatedDate;
                        _apbill.TransactionNumber = "";
                        _apbill.TransactionStatusListId = 4;
                        _apbill.TypeListId = 1;
                        context.APBills.Add(_apbill);
                        context.SaveChanges();

                        //MasterTrx _masterTrx = new MasterTrx();
                        //_masterTrx.BillMonth = item.BillMonth;
                        //_masterTrx.BillYear = item.BillYear;
                        //_masterTrx.ClassId = item.CustomerCredit.CustomerId;
                        //_masterTrx.CreatedBy = LoginUserId;
                        //_masterTrx.CreatedDate = DateTime.Now;
                        //_masterTrx.HeaderId = _apbill.APBillId;
                        //_masterTrx.MasterTrxTypeListId = 43;
                        //_masterTrx.PeriodId = _period.PeriodId;
                        //_masterTrx.RegionId = SelectedRegionId;
                        //_masterTrx.StatusId = 3;
                        //_masterTrx.TrxDate = item.CreatedDate;
                        //_masterTrx.TypeListId = 1;
                        //context.MasterTrxes.Add(_masterTrx);
                        //context.SaveChanges();

                        //_apbill.MasterTrxId = _masterTrx.MasterTrxId;
                        //context.SaveChanges();



                        //decimal totalCustomerCredit = 0;
                        //decimal totalCustomerTaxes = 0;

                        //foreach (CreditViewModel cvm in item.CustomerCredit.Credits)
                        //{
                        //    MasterTrxDetail masterTrxDetail = null;
                        //    if (item.Id != -1)
                        //        masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxId == _apbill.MasterTrxId && o.LineNo == cvm.LineNo).FirstOrDefault();

                        //    if (masterTrxDetail == null)
                        //    {
                        //        if (cvm.CreditAmount == 0)
                        //            continue; // do not create a new credit for a line item with 0 credit amt

                        //        masterTrxDetail = new MasterTrxDetail();
                        //        masterTrxDetail.MasterTrxId = _apbill.MasterTrxId;
                        //        masterTrxDetail.InvoiceId = item.InvoiceId;
                        //        masterTrxDetail.LineNo = cvm.LineNo;
                        //        masterTrxDetail.MasterTrxTypeListId = 43; // customer credit
                        //        masterTrxDetail.HeaderId = _apbill.APBillId;
                        //        masterTrxDetail.RegionId = item.RegionId;

                        //        masterTrxDetail.CreatedBy = item.CreatedBy;
                        //        masterTrxDetail.CreatedDate = item.CreatedDate;
                        //        masterTrxDetail.IsDelete = false;
                        //        masterTrxDetail.DetailDescription = item.CreditDescription;

                        //        masterTrxDetail.Transactiondate = DateTime.Now;
                        //        masterTrxDetail.ClassId = item.CustomerCredit.CustomerId;
                        //        masterTrxDetail.TypelistId = 1;
                        //        masterTrxDetail.PeriodId = ClaimView.GetCLAIM_PERIOD_ID();

                        //        List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == item.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                        //        if (_masterINVDetail.Count > 0)
                        //        {
                        //            masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                        //            masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                        //            masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                        //            //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                        //            masterTrxDetail.Quantity = 0;
                        //            masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                        //            masterTrxDetail.TotalFee = 0;
                        //            masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                        //            masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                        //            masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                        //            masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                        //            masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                        //        }

                        //        context.MasterTrxDetails.Add(masterTrxDetail);
                        //    }
                        //    else
                        //    {
                        //        masterTrxDetail.ModifiedBy = item.CreatedBy;
                        //        masterTrxDetail.ModifiedDate = item.CreatedDate;

                        //        context.Entry(masterTrxDetail).State = EntityState.Modified;
                        //    }

                        //    masterTrxDetail.ServiceTypeListId = 75;
                        //    masterTrxDetail.ExtendedPrice = Math.Abs(cvm.CreditAmount);
                        //    masterTrxDetail.FeesDetail = false;
                        //    masterTrxDetail.TaxDetail = true;
                        //    masterTrxDetail.TotalTax = Math.Abs(cvm.Tax);
                        //    masterTrxDetail.Total = Math.Abs(cvm.Total);
                        //    masterTrxDetail.AmountTypeListId = (cvm.CreditAmount < 0 ? 2 : 1); // credit
                        //    context.SaveChanges();

                        //    //// insert customer taxes
                        //    //MasterTrxTax customerTax = null;
                        //    //if (item.Id != -1)
                        //    //    customerTax = context.MasterTrxTaxes.Where(o => o.MasterTrxDetailId == masterTrxDetail.MasterTrxDetailId).FirstOrDefault();

                        //    //if (customerTax == null)
                        //    //{
                        //    //    customerTax = new MasterTrxTax();
                        //    //    customerTax.CreatedBy = item.CreatedBy;
                        //    //    customerTax.CreatedDate = item.CreatedDate;
                        //    //    context.MasterTrxTaxes.Add(customerTax);
                        //    //}
                        //    //else
                        //    //{
                        //    //    customerTax.ModifiedBy = item.CreatedBy;
                        //    //    customerTax.ModifiedDate = item.CreatedDate;
                        //    //    context.Entry(customerTax).State = EntityState.Modified;
                        //    //}

                        //    //customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                        //    //customerTax.Amount = Math.Abs(cvm.Tax);
                        //    //customerTax.TaxratePercentage = Math.Abs(Decimal.Round(cvm.Tax * 100.00M / cvm.CreditAmount, 2));
                        //    //customerTax.AmountTypeListId = 1; // credit
                        //    //context.SaveChanges();

                        //    //totalCustomerCredit += Math.Abs(cvm.CreditAmount);
                        //    //totalCustomerTaxes += Math.Abs(cvm.Tax);
                        //}




                    }
                }
            }


            return true;





            //    JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
            //JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            //customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(3, vm.RegionId);

            //string customerNextTrxNumber = null;

            //JKViewModels.Administration.Company.TransactionNumberConfigViewModel 
            //    franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{

            //    Period _period = context.Periods.Where(f => f.BillMonth == vm.BillMonth && f.BillYear == vm.BillYear).FirstOrDefault();


            //    APBill _apbill = new APBill();
            //    //_apbill.APBillId;
            //    _apbill.BillMonth = vm.BillMonth;
            //    _apbill.BillYear = vm.BillYear;
            //    //_apbill.CheckAmount;
            //    //_apbill.CheckBookTransactionListId;
            //    _apbill.CheckBookTransactionTypeListId = 17;
            //    _apbill.CheckTypeListId = 19;
            //    _apbill.ClassId = vm.CustomerCredit.CustomerId;
            //    _apbill.CreatedBy = LoginUserId;
            //    _apbill.CreatedDate =DateTime.Now;
            //    //_apbill.FranchiseeReportFinalizedId;
            //    //_apbill.FranchiseeReportId;
            //    //_apbill.FranchiseeTurnaroundCheckId;
            //    _apbill.IsDelete = false;
            //    //_apbill.ManualCheckId;
            //    //_apbill.MasterTrxId;               
            //    //_apbill.PayTypeListId;
            //    _apbill.PeriodId = _period.PeriodId;
            //    _apbill.RegionId = SelectedRegionId;
            //    //_apbill.SourceId;
            //    //_apbill.SourceTypeListId;
            //    _apbill.TransactionDate = vm.CreatedDate;
            //    _apbill.TransactionNumber = "";
            //    _apbill.TransactionStatusListId = 3;
            //    _apbill.TypeListId = 1;
            //    context.APBills.Add(_apbill);
            //    context.SaveChanges();


            //    MasterTrx _masterTrx = new MasterTrx();                
            //    _masterTrx.BillMonth = vm.BillMonth;
            //    _masterTrx.BillYear = vm.BillYear;
            //    _masterTrx.ClassId = vm.CustomerCredit.CustomerId;
            //    _masterTrx.CreatedBy = LoginUserId;
            //    _masterTrx.CreatedDate = DateTime.Now;
            //    _masterTrx.HeaderId = _apbill.APBillId;
            //    _masterTrx.MasterTrxTypeListId = 43;
            //    _masterTrx.PeriodId = _period.PeriodId;
            //    _masterTrx.RegionId = SelectedRegionId;
            //    _masterTrx.StatusId = 3;
            //    _masterTrx.TrxDate = vm.CreatedDate;
            //    _masterTrx.TypeListId = 1;
            //    context.MasterTrxes.Add(_masterTrx);
            //    context.SaveChanges();

            //    _apbill.MasterTrxId = _masterTrx.MasterTrxId;
            //    context.SaveChanges();


            //    //Adjustment customerCredit = null;
            //    //if (vm.Id != -1)
            //    //    customerCredit = context.Adjustments.Where(o => o.AdjustmentId == vm.Id).FirstOrDefault();
            //    //// customer credit mastertrx
            //    //MasterTrx customerMasterTrx = null;
            //    //if (customerCredit != null)
            //    //    customerMasterTrx = context.MasterTrxes.Where(o => o.MasterTrxId == customerCredit.MasterTrxId).FirstOrDefault();
            //    //if (customerMasterTrx == null)
            //    //{
            //    //    customerMasterTrx = new MasterTrx();
            //    //    customerMasterTrx.MasterTrxTypeListId = 43; // Adjustment 
            //    //    customerMasterTrx.ClassId = vm.CustomerCredit.CustomerId;
            //    //    customerMasterTrx.TypeListId = 1; // customer
            //    //    customerMasterTrx.RegionId = vm.RegionId;
            //    //    customerMasterTrx.TrxDate = vm.CreatedDate;
            //    //    customerMasterTrx.BillMonth = vm.BillMonth;
            //    //    customerMasterTrx.BillYear = vm.BillYear;
            //    //    customerMasterTrx.StatusId = 1; // open
            //    //    context.MasterTrxes.Add(customerMasterTrx);
            //    //}
            //    //else
            //    //{
            //    //    // todo: add mastertrx modifiedby/date
            //    //    context.Entry(customerMasterTrx).State = EntityState.Modified;
            //    //}
            //    //context.SaveChanges();
            //    //// customer credit
            //    //if (customerCredit == null)
            //    //{
            //    //    if (customerTransactionNumberConfigViewModel != null)
            //    //        customerNextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(3, vm.RegionId, vm.CreatedDate);
            //    //    customerCredit = new Adjustment();
            //    //    customerCredit.MasterTrxId = customerMasterTrx.MasterTrxId;
            //    //    customerCredit.InvoiceId = vm.InvoiceId;
            //    //    customerCredit.TypeListId = 1; // customer
            //    //    customerCredit.ClassId = vm.CustomerCredit.CustomerId;
            //    //    customerCredit.RegionId = vm.RegionId;
            //    //    customerCredit.TransactionStatusListId = 1; // pending approval
            //    //    customerCredit.IsDelete = false;
            //    //    customerCredit.TransactionNumber = customerNextTrxNumber;
            //    //    customerCredit.CreatedBy = vm.CreatedBy;
            //    //    customerCredit.CreatedDate = vm.CreatedDate;
            //    //    customerCredit.TransactionDate = DateTime.Now;
            //    //    customerCredit.PeriodId = ClaimView.GetCLAIM_PERIOD_ID();                    
            //    //    context.Adjustments.Add(customerCredit);
            //    //}
            //    //else
            //    //{
            //    //    customerCredit.ModifiedBy = vm.CreatedBy;
            //    //    customerCredit.ModifiedDate = vm.CreatedDate;
            //    //    context.Entry(customerCredit).State = EntityState.Modified;
            //    //}
            //    //customerCredit.AdjustmentReasonListId = vm.CreditReasonListId;
            //    //customerCredit.AdjustmentDescription = vm.CreditDescription;
            //    //context.SaveChanges();

            //    decimal totalCustomerCredit = 0;
            //    decimal totalCustomerTaxes = 0;

            //    foreach (CreditViewModel cvm in vm.CustomerCredit.Credits)
            //    {
            //        MasterTrxDetail masterTrxDetail = null;
            //        if (vm.Id != -1)
            //            masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxId == _apbill.MasterTrxId && o.LineNo == cvm.LineNo).FirstOrDefault();

            //        if (masterTrxDetail == null)
            //        {
            //            if (cvm.CreditAmount == 0)
            //                continue; // do not create a new credit for a line item with 0 credit amt

            //            masterTrxDetail = new MasterTrxDetail();
            //            masterTrxDetail.MasterTrxId = _apbill.MasterTrxId;
            //            masterTrxDetail.InvoiceId = vm.InvoiceId;
            //            masterTrxDetail.LineNo = cvm.LineNo;
            //            masterTrxDetail.MasterTrxTypeListId = 43; // customer credit
            //            masterTrxDetail.HeaderId = _apbill.APBillId;
            //            masterTrxDetail.RegionId = vm.RegionId;

            //            masterTrxDetail.CreatedBy = vm.CreatedBy;
            //            masterTrxDetail.CreatedDate = vm.CreatedDate;
            //            masterTrxDetail.IsDelete = false;
            //            masterTrxDetail.DetailDescription = vm.CreditDescription;

            //            masterTrxDetail.Transactiondate = DateTime.Now;
            //            masterTrxDetail.ClassId = vm.CustomerCredit.CustomerId;
            //            masterTrxDetail.TypelistId = 1;
            //            masterTrxDetail.PeriodId = ClaimView.GetCLAIM_PERIOD_ID();

            //            List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == vm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

            //            if (_masterINVDetail.Count > 0)
            //            {
            //                masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
            //                masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
            //                masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
            //                //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
            //                masterTrxDetail.Quantity = 0;
            //                masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
            //                masterTrxDetail.TotalFee = 0;
            //                masterTrxDetail.Commission = _masterINVDetail[0].Commission;
            //                masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
            //                masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
            //                masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
            //                masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

            //            }

            //            context.MasterTrxDetails.Add(masterTrxDetail);
            //        }
            //        else
            //        {
            //            masterTrxDetail.ModifiedBy = vm.CreatedBy;
            //            masterTrxDetail.ModifiedDate = vm.CreatedDate;

            //            context.Entry(masterTrxDetail).State = EntityState.Modified;
            //        }

            //        masterTrxDetail.ServiceTypeListId = 75;
            //        masterTrxDetail.ExtendedPrice = Math.Abs(cvm.CreditAmount);
            //        masterTrxDetail.FeesDetail = false;
            //        masterTrxDetail.TaxDetail = true;
            //        masterTrxDetail.TotalTax = Math.Abs(cvm.Tax);
            //        masterTrxDetail.Total = Math.Abs(cvm.Total);
            //        masterTrxDetail.AmountTypeListId = (cvm.CreditAmount<0?2:1); // credit
            //        context.SaveChanges();

            //        // insert customer taxes
            //        MasterTrxTax customerTax = null;
            //        if (vm.Id != -1)
            //            customerTax = context.MasterTrxTaxes.Where(o => o.MasterTrxDetailId == masterTrxDetail.MasterTrxDetailId).FirstOrDefault();

            //        if (customerTax == null)
            //        {
            //            customerTax = new MasterTrxTax();
            //            customerTax.CreatedBy = vm.CreatedBy;
            //            customerTax.CreatedDate = vm.CreatedDate;
            //            context.MasterTrxTaxes.Add(customerTax);
            //        }
            //        else
            //        {
            //            customerTax.ModifiedBy = vm.CreatedBy;
            //            customerTax.ModifiedDate = vm.CreatedDate;
            //            context.Entry(customerTax).State = EntityState.Modified;
            //        }

            //        customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
            //        customerTax.Amount = Math.Abs(cvm.Tax);
            //        customerTax.TaxratePercentage = Math.Abs(Decimal.Round(cvm.Tax * 100.00M / cvm.CreditAmount, 2));
            //        customerTax.AmountTypeListId = 1; // credit
            //        context.SaveChanges();

            //        totalCustomerCredit += Math.Abs(cvm.CreditAmount);
            //        //totalCustomerTaxes += Math.Abs(cvm.Tax);
            //    }

            //    //// update original invoice's status

            //    //Invoice existingInvoice = context.Invoices.Where(o => o.InvoiceId == vm.InvoiceId).FirstOrDefault();
            //    //if (existingInvoice != null)
            //    //{
            //    //    existingInvoice.TransactionStatusListId = vm.PaidInFull ? 6 : 7; // paid : paid partial
            //    //    existingInvoice.ModifiedBy = vm.CreatedBy;
            //    //    existingInvoice.ModifiedDate = vm.CreatedDate;
            //    //    context.Entry(existingInvoice).State = EntityState.Modified;
            //    //    context.SaveChanges();
            //    //}

            //    //// extra credit
            //    //if (vm.IsExtraCredit)
            //    //{
            //    //    decimal unappliedCredit = vm.TotalCredit;

            //    //    MasterTrxDetail masterTrxDetail = null;
            //    //    if (vm.Id != -1)
            //    //        masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxTypeListId == 18 && o.SourceTypeListId == 4 && o.SourceId == customerCredit.AdjustmentId).FirstOrDefault();

            //    //    MasterTrx masterTrx = null;
            //    //    if (masterTrxDetail != null)
            //    //        masterTrx = context.MasterTrxes.Where(o => o.MasterTrxId == masterTrxDetail.MasterTrxId).FirstOrDefault();

            //    //    if (masterTrx == null)
            //    //    {
            //    //        masterTrx = new MasterTrx();
            //    //        masterTrx.TypeListId = 1; // customer
            //    //        masterTrx.ClassId = customerCredit.ClassId;
            //    //        masterTrx.MasterTrxTypeListId = 18; // extra credit
            //    //        masterTrx.TrxDate = vm.CreatedDate;
            //    //        masterTrx.RegionId = vm.RegionId;
            //    //        masterTrx.StatusId = 1; // open

            //    //        context.MasterTrxes.Add(masterTrx);
            //    //    }
            //    //    else
            //    //    {
            //    //        context.Entry(masterTrx).State = EntityState.Modified;
            //    //    }

            //    //    context.SaveChanges();

            //    //    if (masterTrxDetail == null)
            //    //    {
            //    //        masterTrxDetail = new MasterTrxDetail();
            //    //        masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
            //    //        masterTrxDetail.InvoiceId = null;
            //    //        masterTrxDetail.LineNo = null;
            //    //        masterTrxDetail.MasterTrxTypeListId = 18; // extra credit
            //    //        masterTrxDetail.SourceTypeListId = 4; // credit
            //    //        masterTrxDetail.SourceId = customerCredit.AdjustmentId;
            //    //        masterTrxDetail.RegionId = vm.RegionId;

            //    //        masterTrxDetail.CreatedBy = vm.CreatedBy;
            //    //        masterTrxDetail.CreatedDate = vm.CreatedDate;
            //    //        masterTrxDetail.IsDelete = false;

            //    //        context.MasterTrxDetails.Add(masterTrxDetail);
            //    //    }
            //    //    else
            //    //    {
            //    //        masterTrxDetail.ModifiedBy = vm.CreatedBy;
            //    //        masterTrxDetail.ModifiedDate = vm.CreatedDate;

            //    //        context.Entry(masterTrxDetail).State = EntityState.Modified;
            //    //    }

            //    //    masterTrxDetail.ServiceTypeListId = null;
            //    //    masterTrxDetail.FeesDetail = false;
            //    //    masterTrxDetail.TaxDetail = false;
            //    //    masterTrxDetail.Total = unappliedCredit;
            //    //    masterTrxDetail.AmountTypeListId = 1; // credit

            //    //    context.SaveChanges();

            //    //    totalCustomerCredit += unappliedCredit;
            //    //}

            //    //// general ledger for customer credit -- credit to Business Svc. Income

            //    //if (totalCustomerCredit > 0)
            //    //{
            //    //    GeneralLedger glCustomerCredit2 = null;
            //    //    if (vm.Id != -1)
            //    //        glCustomerCredit2 = context.GeneralLedgers.Where(o => o.MasterTrxId == customerMasterTrx.MasterTrxId && o.LedgerAcctId == 1).FirstOrDefault();

            //    //    if (glCustomerCredit2 == null)
            //    //    {
            //    //        glCustomerCredit2 = new GeneralLedger();
            //    //        glCustomerCredit2.LedgerAcctId = 1; // Business Svc. Income
            //    //        glCustomerCredit2.IsDelete = false;

            //    //        context.GeneralLedgers.Add(glCustomerCredit2);
            //    //    }
            //    //    else
            //    //    {
            //    //        context.Entry(glCustomerCredit2).State = EntityState.Modified;
            //    //    }
            //    //    glCustomerCredit2.MasterTrxId = customerMasterTrx.MasterTrxId;
            //    //    glCustomerCredit2.Credit = totalCustomerCredit;
            //    //    glCustomerCredit2.Debit = 0;
            //    //    context.SaveChanges();
            //    //}
            //    //// general ledger for customer credit -- credit to Customer Sales Tax
            //    //if (totalCustomerTaxes > 0)
            //    //{
            //    //    GeneralLedger glCustomerCredit3 = null;
            //    //    if (vm.Id != -1)
            //    //        glCustomerCredit3 = context.GeneralLedgers.Where(o => o.MasterTrxId == customerMasterTrx.MasterTrxId && o.LedgerAcctId == 5).FirstOrDefault();
            //    //    if (glCustomerCredit3 == null)
            //    //    {
            //    //        glCustomerCredit3 = new GeneralLedger();
            //    //        glCustomerCredit3.LedgerAcctId = 5; // Customer Sales Tax
            //    //        glCustomerCredit3.IsDelete = false;
            //    //        context.GeneralLedgers.Add(glCustomerCredit3);
            //    //    }
            //    //    else
            //    //    {
            //    //        context.Entry(glCustomerCredit3).State = EntityState.Modified;
            //    //    }
            //    //    glCustomerCredit3.MasterTrxId = customerMasterTrx.MasterTrxId;
            //    //    glCustomerCredit3.Credit = totalCustomerTaxes;
            //    //    glCustomerCredit3.Debit = 0;
            //    //    context.SaveChanges();
            //    //}
            //    //// general ledger for customer credit -- debit from A/R Janitorial Svc.
            //    //if ((totalCustomerCredit + totalCustomerTaxes) > 0)
            //    //{
            //    //    GeneralLedger glCustomerCredit1 = null;
            //    //    if (vm.Id != -1)
            //    //        glCustomerCredit1 = context.GeneralLedgers.Where(o => o.MasterTrxId == customerMasterTrx.MasterTrxId && o.LedgerAcctId == 3).FirstOrDefault();
            //    //    if (glCustomerCredit1 == null)
            //    //    {
            //    //        glCustomerCredit1 = new GeneralLedger();
            //    //        glCustomerCredit1.LedgerAcctId = 3; // A/R Janitorial Svc.
            //    //        glCustomerCredit1.IsDelete = false;
            //    //        context.GeneralLedgers.Add(glCustomerCredit1);
            //    //    }
            //    //    else
            //    //    {
            //    //        context.Entry(glCustomerCredit1).State = EntityState.Modified;
            //    //    }
            //    //    glCustomerCredit1.MasterTrxId = customerMasterTrx.MasterTrxId;
            //    //    glCustomerCredit1.Credit = 0;
            //    //    glCustomerCredit1.Debit = totalCustomerCredit + totalCustomerTaxes;
            //    //    context.SaveChanges();
            //    //}


            //    //if (customerNextTrxNumber != null)
            //    //{
            //    //    customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
            //    //    CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
            //    //}

            //    return true;
            //}
        }
        public bool InsertOrUpdateBalanceAdjustment(CreditTransactionViewModel vm,int MaterTrsTypeListId, out int outMastertrxId)
        {

            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(61, vm.RegionId);


            string customerNextTrxNumber = null;

            JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                Adjustment customerCredit = null;
                if (vm.Id != -1)
                    customerCredit = context.Adjustments.Where(o => o.AdjustmentId == vm.Id).FirstOrDefault();

                // customer credit mastertrx

                MasterTrx customerMasterTrx = null;
                if (customerCredit != null)
                    customerMasterTrx = context.MasterTrxes.Where(o => o.MasterTrxId == customerCredit.MasterTrxId).FirstOrDefault();

                if (customerMasterTrx == null)
                {
                    customerMasterTrx = new MasterTrx();
                    customerMasterTrx.MasterTrxTypeListId = MaterTrsTypeListId; // Adjustment to Refund Check
                    customerMasterTrx.ClassId = vm.CustomerCredit.CustomerId;
                    customerMasterTrx.TypeListId = 1; // customer
                    customerMasterTrx.RegionId = vm.RegionId;
                    customerMasterTrx.TrxDate = vm.CreatedDate;
                    customerMasterTrx.BillMonth = vm.BillMonth;
                    customerMasterTrx.BillYear = vm.BillYear;
                    customerMasterTrx.StatusId = 1; // open

                    context.MasterTrxes.Add(customerMasterTrx);
                }
                else
                {
                    // todo: add mastertrx modifiedby/date

                    context.Entry(customerMasterTrx).State = EntityState.Modified;
                }

                context.SaveChanges();

                outMastertrxId = customerMasterTrx.MasterTrxId;

                // customer credit

                if (customerCredit == null)
                {

                    customerNextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(61, vm.RegionId, vm.CreatedDate);

                    customerCredit = new Adjustment();
                    customerCredit.MasterTrxId = customerMasterTrx.MasterTrxId;
                    customerCredit.InvoiceId = vm.InvoiceId;
                    customerCredit.TypeListId = 1; // customer
                    customerCredit.ClassId = vm.CustomerCredit.CustomerId;
                    customerCredit.RegionId = vm.RegionId;
                    customerCredit.TransactionStatusListId = 1; // pending approval
                    customerCredit.IsDelete = false;
                    customerCredit.TransactionNumber = customerNextTrxNumber;
                    customerCredit.CreatedBy = vm.CreatedBy;
                    customerCredit.CreatedDate = vm.CreatedDate;

                    customerCredit.TransactionDate = DateTime.Now;
                    customerCredit.PeriodId = ClaimView.GetCLAIM_PERIOD_ID();

                    customerCredit.AdjustmentDescription = vm.Note;

                    context.Adjustments.Add(customerCredit);
                }
                else
                {
                    customerCredit.ModifiedBy = vm.CreatedBy;
                    customerCredit.ModifiedDate = vm.CreatedDate;

                    context.Entry(customerCredit).State = EntityState.Modified;
                }

                customerCredit.AdjustmentReasonListId = vm.CreditReasonListId;
                customerCredit.AdjustmentDescription = vm.Note;

                context.SaveChanges();

                decimal totalCustomerCredit = 0;
                decimal totalCustomerTaxes = 0;

                foreach (CreditViewModel cvm in vm.CustomerCredit.Credits)
                {
                    MasterTrxDetail masterTrxDetail = null;
                    if (vm.Id != -1)
                        masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxId == customerMasterTrx.MasterTrxId && o.LineNo == cvm.LineNo).FirstOrDefault();

                    if (masterTrxDetail == null)
                    {
                        if (cvm.CreditAmount == 0)
                            continue; // do not create a new credit for a line item with 0 credit amt

                        masterTrxDetail = new MasterTrxDetail();
                        masterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId;
                        masterTrxDetail.InvoiceId = vm.InvoiceId;
                        masterTrxDetail.LineNo = cvm.LineNo;
                        masterTrxDetail.MasterTrxTypeListId = MaterTrsTypeListId; // Adjustment to Refund Check
                        masterTrxDetail.HeaderId = customerCredit.AdjustmentId;
                        masterTrxDetail.RegionId = vm.RegionId;

                        masterTrxDetail.CreatedBy = vm.CreatedBy;
                        masterTrxDetail.CreatedDate = vm.CreatedDate;
                        masterTrxDetail.IsDelete = false;
                        masterTrxDetail.DetailDescription = vm.Note;

                        masterTrxDetail.Transactiondate = DateTime.Now;
                        masterTrxDetail.ClassId = vm.CustomerCredit.CustomerId;
                        masterTrxDetail.TypelistId = 1;
                        masterTrxDetail.PeriodId = ClaimView.GetCLAIM_PERIOD_ID();

                        List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == vm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                        if (_masterINVDetail.Count > 0)
                        {
                            masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                            masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                            masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                            //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                            masterTrxDetail.Quantity = 0;
                            masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                            masterTrxDetail.TotalFee = 0;
                            masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                            masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                            masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                            masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                            masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                        }

                        context.MasterTrxDetails.Add(masterTrxDetail);
                    }
                    else
                    {
                        masterTrxDetail.ModifiedBy = vm.CreatedBy;
                        masterTrxDetail.ModifiedDate = vm.CreatedDate;

                        context.Entry(masterTrxDetail).State = EntityState.Modified;
                    }

                    masterTrxDetail.ServiceTypeListId = (cvm.CreditAmount < 0 ? 77 : 76);

                    //Commented By Rakesh :: Need to Discuss with Peter Sir

                    //masterTrxDetail.ExtendedPrice = Math.Abs(cvm.CreditAmount);
                    //masterTrxDetail.FeesDetail = false;
                    //masterTrxDetail.TaxDetail = true;
                    //masterTrxDetail.TotalTax = Math.Abs(cvm.Tax);
                    //masterTrxDetail.Total = Math.Abs(cvm.Total);
                    //masterTrxDetail.AmountTypeListId = (cvm.CreditAmount < 0 ? 2 : 1); // credit

                    masterTrxDetail.ExtendedPrice = 0;
                    masterTrxDetail.FeesDetail = false;
                    masterTrxDetail.TaxDetail = true;
                    masterTrxDetail.TotalTax = 0;
                    masterTrxDetail.Total = Math.Abs(vm.TotalCredit);
                    masterTrxDetail.AmountTypeListId = (cvm.CreditAmount < 0 ? 2 : 1); // credit

                    context.SaveChanges();

                    // insert customer taxes

                    MasterTrxTax customerTax = null;
                    if (vm.Id != -1)
                        customerTax = context.MasterTrxTaxes.Where(o => o.MasterTrxDetailId == masterTrxDetail.MasterTrxDetailId).FirstOrDefault();

                    if (customerTax == null)
                    {
                        customerTax = new MasterTrxTax();

                        customerTax.CreatedBy = vm.CreatedBy;
                        customerTax.CreatedDate = vm.CreatedDate;

                        context.MasterTrxTaxes.Add(customerTax);
                    }
                    else
                    {
                        customerTax.ModifiedBy = vm.CreatedBy;
                        customerTax.ModifiedDate = vm.CreatedDate;

                        context.Entry(customerTax).State = EntityState.Modified;
                    }

                    customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                    customerTax.Amount = Math.Abs(cvm.Tax);
                    customerTax.TaxratePercentage = Math.Abs(Decimal.Round(cvm.Tax * 100.00M / cvm.CreditAmount, 2));
                    customerTax.AmountTypeListId = (cvm.CreditAmount < 0 ? 2 : 1); // credit

                    context.SaveChanges();

                    totalCustomerCredit += Math.Abs(cvm.CreditAmount);
                    totalCustomerTaxes += Math.Abs(cvm.Tax);
                }

                // update original invoice's status

                Invoice existingInvoice = context.Invoices.Where(o => o.InvoiceId == vm.InvoiceId).FirstOrDefault();
                if (existingInvoice != null)
                {
                    existingInvoice.TransactionStatusListId = vm.PaidInFull ? 6 : 7; // paid : paid partial
                    existingInvoice.ModifiedBy = vm.CreatedBy;
                    existingInvoice.ModifiedDate = vm.CreatedDate;
                    context.Entry(existingInvoice).State = EntityState.Modified;
                    context.SaveChanges();
                }

                // extra credit

                if (vm.IsExtraCredit)
                {
                    decimal unappliedCredit = vm.TotalCredit;

                    MasterTrxDetail masterTrxDetail = null;
                    if (vm.Id != -1)
                        masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxTypeListId == 18 && o.SourceTypeListId == 4 && o.SourceId == customerCredit.AdjustmentId).FirstOrDefault();

                    MasterTrx masterTrx = null;
                    if (masterTrxDetail != null)
                        masterTrx = context.MasterTrxes.Where(o => o.MasterTrxId == masterTrxDetail.MasterTrxId).FirstOrDefault();

                    if (masterTrx == null)
                    {
                        masterTrx = new MasterTrx();
                        masterTrx.TypeListId = 1; // customer
                        masterTrx.ClassId = customerCredit.ClassId;
                        masterTrx.MasterTrxTypeListId = 18; // extra credit
                        masterTrx.TrxDate = vm.CreatedDate;
                        masterTrx.RegionId = vm.RegionId;
                        masterTrx.StatusId = 1; // open

                        context.MasterTrxes.Add(masterTrx);
                    }
                    else
                    {
                        context.Entry(masterTrx).State = EntityState.Modified;
                    }

                    context.SaveChanges();

                    if (masterTrxDetail == null)
                    {
                        masterTrxDetail = new MasterTrxDetail();
                        masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
                        masterTrxDetail.InvoiceId = null;
                        masterTrxDetail.LineNo = null;
                        masterTrxDetail.MasterTrxTypeListId = 18; // extra credit
                        masterTrxDetail.SourceTypeListId = 4; // credit
                        masterTrxDetail.SourceId = customerCredit.AdjustmentId;
                        masterTrxDetail.RegionId = vm.RegionId;

                        masterTrxDetail.CreatedBy = vm.CreatedBy;
                        masterTrxDetail.CreatedDate = vm.CreatedDate;
                        masterTrxDetail.IsDelete = false;

                        context.MasterTrxDetails.Add(masterTrxDetail);
                    }
                    else
                    {
                        masterTrxDetail.ModifiedBy = vm.CreatedBy;
                        masterTrxDetail.ModifiedDate = vm.CreatedDate;

                        context.Entry(masterTrxDetail).State = EntityState.Modified;
                    }

                    masterTrxDetail.ServiceTypeListId = null;
                    masterTrxDetail.FeesDetail = false;
                    masterTrxDetail.TaxDetail = false;
                    masterTrxDetail.Total = unappliedCredit;
                    masterTrxDetail.AmountTypeListId = 1; // credit

                    context.SaveChanges();

                    totalCustomerCredit += unappliedCredit;
                }

                // general ledger for customer credit -- credit to Business Svc. Income

                if (totalCustomerCredit > 0)
                {
                    GeneralLedger glCustomerCredit2 = null;
                    if (vm.Id != -1)
                        glCustomerCredit2 = context.GeneralLedgers.Where(o => o.MasterTrxId == customerMasterTrx.MasterTrxId && o.LedgerAcctId == 1).FirstOrDefault();

                    if (glCustomerCredit2 == null)
                    {
                        glCustomerCredit2 = new GeneralLedger();
                        glCustomerCredit2.LedgerAcctId = 1; // Business Svc. Income
                        glCustomerCredit2.IsDelete = false;

                        context.GeneralLedgers.Add(glCustomerCredit2);
                    }
                    else
                    {
                        context.Entry(glCustomerCredit2).State = EntityState.Modified;
                    }

                    glCustomerCredit2.MasterTrxId = customerMasterTrx.MasterTrxId;
                    glCustomerCredit2.Credit = totalCustomerCredit;
                    glCustomerCredit2.Debit = 0;

                    context.SaveChanges();
                }

                // general ledger for customer credit -- credit to Customer Sales Tax

                if (totalCustomerTaxes > 0)
                {
                    GeneralLedger glCustomerCredit3 = null;
                    if (vm.Id != -1)
                        glCustomerCredit3 = context.GeneralLedgers.Where(o => o.MasterTrxId == customerMasterTrx.MasterTrxId && o.LedgerAcctId == 5).FirstOrDefault();

                    if (glCustomerCredit3 == null)
                    {
                        glCustomerCredit3 = new GeneralLedger();
                        glCustomerCredit3.LedgerAcctId = 5; // Customer Sales Tax
                        glCustomerCredit3.IsDelete = false;

                        context.GeneralLedgers.Add(glCustomerCredit3);
                    }
                    else
                    {
                        context.Entry(glCustomerCredit3).State = EntityState.Modified;
                    }

                    glCustomerCredit3.MasterTrxId = customerMasterTrx.MasterTrxId;
                    glCustomerCredit3.Credit = totalCustomerTaxes;
                    glCustomerCredit3.Debit = 0;

                    context.SaveChanges();
                }

                // general ledger for customer credit -- debit from A/R Janitorial Svc.

                if ((totalCustomerCredit + totalCustomerTaxes) > 0)
                {
                    GeneralLedger glCustomerCredit1 = null;
                    if (vm.Id != -1)
                        glCustomerCredit1 = context.GeneralLedgers.Where(o => o.MasterTrxId == customerMasterTrx.MasterTrxId && o.LedgerAcctId == 3).FirstOrDefault();

                    if (glCustomerCredit1 == null)
                    {
                        glCustomerCredit1 = new GeneralLedger();
                        glCustomerCredit1.LedgerAcctId = 3; // A/R Janitorial Svc.
                        glCustomerCredit1.IsDelete = false;

                        context.GeneralLedgers.Add(glCustomerCredit1);
                    }
                    else
                    {
                        context.Entry(glCustomerCredit1).State = EntityState.Modified;
                    }

                    glCustomerCredit1.MasterTrxId = customerMasterTrx.MasterTrxId;
                    glCustomerCredit1.Credit = 0;
                    glCustomerCredit1.Debit = totalCustomerCredit + totalCustomerTaxes;

                    context.SaveChanges();
                }


                if (customerNextTrxNumber != null)
                {
                    customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
                    CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                }

                return true;
            }
        }
        public int InsertUpdateCustomerCreditTransactionMaintenanceTemp(CreditTransactionViewModel vm, int MaintenanceTempId)
        {
            int retVal = 0;
            CustomerMaintenanceViewModel inputData = new CustomerMaintenanceViewModel();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                MaintenanceTemp oMaintenanceTemp = context.MaintenanceTemps.SingleOrDefault(o => o.MaintenanceTempId == MaintenanceTempId);
                foreach (var cc in vm.CustomerCredit.Credits)
                {
                    MaintenanceTempDetail oMaintenanceDetailTemp = new MaintenanceTempDetail();

                    oMaintenanceDetailTemp.MaintenanceDetailTypeListId = 12;
                    oMaintenanceDetailTemp.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                    oMaintenanceDetailTemp.CustomerId = oMaintenanceTemp.ClassId;
                    oMaintenanceDetailTemp.CRContractDetailId = cc.BaseMasterTrxDetailId;
                    oMaintenanceDetailTemp.CRApplyAmount = cc.CreditAmount;
                    oMaintenanceDetailTemp.CRTaxAmount = cc.Tax;
                    oMaintenanceDetailTemp.CRLineNumber = cc.LineNo;
                    oMaintenanceDetailTemp.CRInvoiceId = vm.InvoiceId;
                    oMaintenanceDetailTemp.CRDate = oMaintenanceTemp.EffectiveDate;
                    oMaintenanceDetailTemp.CRAmount = cc.Total;
                    oMaintenanceDetailTemp.CRDescription = vm.CreditDescription;
                    oMaintenanceDetailTemp.CRReasonId = vm.CreditReasonListId;
                    oMaintenanceDetailTemp.CDServiceTypeListId = cc.ServiceTypeListId;
                    oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                    oMaintenanceDetailTemp.CreatedDate = DateTime.Now;
                    oMaintenanceDetailTemp.RecordType = "INVOICEITEM";
                    context.MaintenanceTempDetails.Add(oMaintenanceDetailTemp);
                    context.SaveChanges();
                }

                foreach (var cc in vm.FranchiseeCredits)
                {
                    MaintenanceTempDetail oMaintenanceDetailTemp = new MaintenanceTempDetail();

                    oMaintenanceDetailTemp.MaintenanceDetailTypeListId = 13;
                    oMaintenanceDetailTemp.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                    oMaintenanceDetailTemp.CustomerId = oMaintenanceTemp.ClassId;
                    oMaintenanceDetailTemp.CRContractDetailId = cc.Credit.BaseMasterTrxDetailId;
                    oMaintenanceDetailTemp.CRApplyAmount = cc.Credit.CreditAmount;
                    oMaintenanceDetailTemp.CRTaxAmount = cc.Credit.Tax;
                    oMaintenanceDetailTemp.CRLineNumber = cc.Credit.LineNo;
                    oMaintenanceDetailTemp.DInvoiceId = vm.InvoiceId;
                    oMaintenanceDetailTemp.CRInvoiceId = cc.BillingPayId;
                    oMaintenanceDetailTemp.CRDate = oMaintenanceTemp.EffectiveDate;
                    oMaintenanceDetailTemp.CRAmount = cc.Credit.Total;
                    oMaintenanceDetailTemp.CRDescription = vm.CreditDescription;
                    oMaintenanceDetailTemp.CRReasonId = vm.CreditReasonListId;
                    oMaintenanceDetailTemp.CDServiceTypeListId = cc.Credit.ServiceTypeListId;
                    oMaintenanceDetailTemp.CRFranchiseeId = cc.FranchiseeId;
                    oMaintenanceDetailTemp.CreatedBy = LoginUserId;
                    oMaintenanceDetailTemp.CreatedDate = DateTime.Now;
                    oMaintenanceDetailTemp.RecordType = "BILLINGPAYITEM";
                    context.MaintenanceTempDetails.Add(oMaintenanceDetailTemp);
                    context.SaveChanges();
                }
            }
            return retVal;

        }

        public bool ApproveCredit(int creditId, int status, string note = "")
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Credit credit = context.Credits.Where(o => o.CreditId == creditId).FirstOrDefault();
                if (credit == null && credit.TransactionStatusListId != 1)
                    return false;

                credit.TransactionStatusListId = status; // 4 = open, 12 - Reject

                credit.ModifiedBy = this.LoginUserId;
                credit.ModifiedDate = DateTime.Now;
                context.Entry(credit).State = EntityState.Modified;



                var _invoiceBal = context.fn_GetInvoiceBalance(credit.InvoiceId).FirstOrDefault();
                var _creditAmount = context.MasterTrxDetails.Where(c => c.MasterTrxId == credit.MasterTrxId).Sum(p => p.ExtendedPrice);
                var _masterTrx = context.MasterTrxes.SingleOrDefault(c => c.MasterTrxId == credit.MasterTrxId);
                if (status == 3)
                {
                    _masterTrx.StatusId = status;
                }

                var _CreditFranchisees = context.CreditFranchisees.Where(c => c.CreditId == creditId).ToList();
                foreach (var item in _CreditFranchisees)
                {
                    item.TransactionStatusListId = status;
                    var _LastMTFC = context.MasterTrxes.SingleOrDefault(c => c.MasterTrxId == item.MasterTrxId);
                    _LastMTFC.StatusId = status;
                }

                //var _billingPaymasterTrx = context.MasterTrxes.Where(c => c.InvoiceId == _billingPays.InvoiceId);



                var _LastMTDCC = context.MasterTrxDetails.Where(c => c.MasterTrxId == credit.MasterTrxId).Count();
                var _LastMTD = context.MasterTrxDetails.Where(c => c.MasterTrxId == credit.MasterTrxId).ToList().LastOrDefault();

                var _extraAmount = _creditAmount - _invoiceBal.BalanceTotal;

                if (_extraAmount > 0 && status != 12 && status != 4)
                {
                    Overflow overflowPayment = new Overflow();
                    overflowPayment.AmountTypeListId = 1;
                    overflowPayment.CheckAmount = _creditAmount;
                    //string _outCheckNumber = "0";
                    //string.TryParse(inputdata.ReferenceNo, out _outCheckNumber);
                    overflowPayment.CheckNumber = "";
                    overflowPayment.MasterTrxDetailId = _LastMTD.MasterTrxDetailId;
                    overflowPayment.PeriodId = _LastMTD.PeriodId;
                    overflowPayment.TypeListId = 1; // customer
                    overflowPayment.ClassId = credit.ClassId;
                    overflowPayment.MasterTrxTypeListId = 24; // overpayment applied
                    overflowPayment.TransactionDate = _LastMTD.Transactiondate;
                    overflowPayment.RegionId = _LastMTD.RegionId;
                    overflowPayment.IsActive = true;
                    overflowPayment.Amount = _extraAmount;
                    overflowPayment.TransactionStatusListId = 4; // open
                    overflowPayment.CreatedBy = LoginUserId;
                    overflowPayment.CreatedDate = DateTime.Now;
                    context.Overflows.Add(overflowPayment);
                    context.SaveChanges();



                    MasterTrxDetail masterTrxDetail = null;
                    //if (credit.CreditId != -1)
                    //    masterTrxDetail = context.MasterTrxDetails.Where(o => o.MasterTrxTypeListId == 18 && o.SourceTypeListId == 4 && o.SourceId == credit.CreditId).FirstOrDefault();

                    MasterTrx masterTrx = null;
                    //if (masterTrxDetail != null)
                    //    masterTrx = context.MasterTrxes.Where(o => o.MasterTrxId == masterTrxDetail.MasterTrxId).FirstOrDefault();

                    if (masterTrx == null)
                    {
                        masterTrx = new MasterTrx();
                        masterTrx.TypeListId = 1; // customer
                        masterTrx.ClassId = credit.ClassId;
                        masterTrx.MasterTrxTypeListId = 18; // extra credit
                        masterTrx.TrxDate = _LastMTD.Transactiondate;
                        masterTrx.RegionId = _LastMTD.RegionId;
                        masterTrx.StatusId = status; // open
                        masterTrx.BillMonth = ((DateTime)_LastMTD.Transactiondate).Month;
                        masterTrx.BillYear = ((DateTime)_LastMTD.Transactiondate).Year;
                        masterTrx.HeaderId = overflowPayment.OverflowId;

                        context.MasterTrxes.Add(masterTrx);
                    }
                    else
                    {
                        context.Entry(masterTrx).State = EntityState.Modified;
                    }

                    context.SaveChanges();

                    if (masterTrxDetail == null)
                    {
                        masterTrxDetail = new MasterTrxDetail();
                        masterTrxDetail.MasterTrxId = masterTrx.MasterTrxId;
                        masterTrxDetail.InvoiceId = null;
                        masterTrxDetail.LineNo = null;
                        masterTrxDetail.MasterTrxTypeListId = 18; // extra credit
                        masterTrxDetail.SourceTypeListId = 4; // credit
                        masterTrxDetail.SourceId = credit.CreditId;
                        masterTrxDetail.RegionId = credit.RegionId;

                        masterTrxDetail.CreatedBy = credit.CreatedBy;
                        masterTrxDetail.CreatedDate = credit.CreatedDate;
                        masterTrxDetail.IsDelete = false;

                        context.MasterTrxDetails.Add(masterTrxDetail);
                    }
                    else
                    {
                        masterTrxDetail.ModifiedBy = credit.CreatedBy;
                        masterTrxDetail.ModifiedDate = credit.CreatedDate;

                        context.Entry(masterTrxDetail).State = EntityState.Modified;
                    }

                    masterTrxDetail.ServiceTypeListId = null;
                    masterTrxDetail.FeesDetail = false;
                    masterTrxDetail.TaxDetail = false;
                    masterTrxDetail.Total = _extraAmount;
                    masterTrxDetail.AmountTypeListId = 1; // credit

                    context.SaveChanges();

                }
                else
                    context.SaveChanges();
                //// extra credit

                //if (vm.IsExtraCredit)
                //{
                //    decimal unappliedCredit = vm.ApplyTotalCredit - vm.TotalCredit;

                //    Overflow overflowPayment = new Overflow();

                //    overflowPayment.AmountTypeListId = 1;
                //    overflowPayment.CheckAmount = vm.ApplyTotalCredit;
                //    //string _outCheckNumber = "0";
                //    //string.TryParse(inputdata.ReferenceNo, out _outCheckNumber);
                //    overflowPayment.CheckNumber = "";
                //    overflowPayment.MasterTrxDetailId = lastMasterTrxDetailId;
                //    overflowPayment.PeriodId = (PR != null ? PR.PeriodId : 0);

                //    overflowPayment.TypeListId = 1; // customer
                //    overflowPayment.ClassId = vm.CustomerCredit.CustomerId;
                //    overflowPayment.MasterTrxTypeListId = 24; // overpayment applied
                //    overflowPayment.TransactionDate = vm.CreatedDate;
                //    overflowPayment.RegionId = vm.RegionId;
                //    overflowPayment.IsActive = true;
                //    overflowPayment.Amount = unappliedCredit;
                //    overflowPayment.TransactionStatusListId = 4; // open
                //    overflowPayment.CreatedBy = vm.CreatedBy;
                //    overflowPayment.CreatedDate = vm.CreatedDate;

                //    context.Overflows.Add(overflowPayment);
                //    context.SaveChanges();
                //}


                int CustomerId = (int)credit.ClassId;

                //send Notification
                savePendingMessage(note, CustomerId, status, -1);

            }

            return true;
        }

        public bool ApproveTempCredit(int creditId, int status, string note = "")
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CreditTemp creditTemp = context.CreditTemps.SingleOrDefault(o => o.CreditTempId == creditId);
                if (creditTemp == null)
                    return false;
                bool isTaxCredit = creditTemp.MasterTrxTypeListId == 58;
                List<CreditTempDetail> creditTempDetailCUS = null;
                List<CreditTempDetail> creditTempDetailFR = null;
                if (isTaxCredit)
                    creditTempDetailCUS = context.CreditTempDetails.Where(o => o.CreditTempId == creditId && o.MasterTrxTypeListId == 58).ToList();
                else
                    creditTempDetailCUS = context.CreditTempDetails.Where(o => o.CreditTempId == creditId && o.MasterTrxTypeListId == 3).ToList();
                creditTempDetailFR = context.CreditTempDetails.Where(o => o.CreditTempId == creditId && o.MasterTrxTypeListId == 8).ToList();
                if (status == 3)
                {
                    JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
                    JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
                    customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(3, (int)creditTemp.RegionId);
                    string customerNextTrxNumber = null;
                    JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

                    Invoice _invoiceOject = context.Invoices.Single(o => o.InvoiceId == creditTemp.InvoiceId);
                    var PR = context.Periods.SingleOrDefault(p => p.BillMonth == creditTemp.TransactionDate.Value.Month && p.BillYear == creditTemp.TransactionDate.Value.Year);
                    int CreditCount = context.Credits.Where(o => o.InvoiceId == creditTemp.InvoiceId && o.IsDelete != true).Count();
                    
                    MasterTrx customerMasterTrx = new MasterTrx();
                    customerMasterTrx.MasterTrxTypeListId = creditTemp.MasterTrxTypeListId; // customer credit
                    customerMasterTrx.ClassId = creditTemp.CustomerId;
                    customerMasterTrx.TypeListId = 1; // customer
                    customerMasterTrx.RegionId = creditTemp.RegionId;
                    customerMasterTrx.StatusId = 3; // open
                    customerMasterTrx.TrxDate = creditTemp.TransactionDate;
                    customerMasterTrx.BillMonth = (PR != null ? PR.BillMonth : 0);
                    customerMasterTrx.BillYear = (PR != null ? PR.BillYear : 0);
                    customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerMasterTrx.CreatedBy = LoginUserId;
                    customerMasterTrx.CreatedDate = DateTime.Now;
                    context.MasterTrxes.Add(customerMasterTrx);
                    context.SaveChanges();

                    // customer credit

                    Credit customerCredit = new Credit();
                    customerCredit.MasterTrxId = customerMasterTrx.MasterTrxId;
                    customerCredit.InvoiceId = creditTemp.InvoiceId;
                    customerCredit.TypeListId = 1; // customer
                    customerCredit.ClassId = creditTemp.CustomerId;
                    customerCredit.RegionId = creditTemp.RegionId;
                    customerCredit.TransactionStatusListId = 3;
                    customerCredit.IsDelete = false;
                    string _LastChar = "";
                    if (CreditCount > 0) _LastChar = ((char)(65 + CreditCount - 1)).ToString();
                    customerCredit.TransactionNumber = "CR" + _invoiceOject.InvoiceNo.Trim() + _LastChar.Trim();
                    customerCredit.CreatedBy = creditTemp.CreatedBy;
                    customerCredit.CreatedDate = DateTime.Now;
                    customerCredit.PeriodId = (PR != null ? PR.PeriodId : 0);
                    customerCredit.TransactionDate = creditTemp.TransactionDate;
                    customerCredit.CreditReasonListId = creditTemp.CreditReasonListId;
                    customerCredit.CreditDescription = creditTemp.CreditDescription;
                    context.Credits.Add(customerCredit);
                    context.SaveChanges();

                    //Update HeaderId In MasterTrx
                    customerMasterTrx.HeaderId = customerCredit.CreditId;
                    context.SaveChanges();

                    decimal totalCustomerCredit = 0;
                    decimal totalCustomerTaxes = 0;
                    int lastMasterTrxDetailId = 0;
                    int cMasterConter = 0;
                    foreach (var cvm in creditTempDetailCUS)
                    {
                        cMasterConter++;
                        //For Tx credit transaction extended price would be zero
                        if (!isTaxCredit && cvm.ExtendedPrice == 0)
                            continue; // do not create a new credit for a line item with 0 credit amt
                       
                        MasterTrxDetail masterTrxDetail = new MasterTrxDetail();
                        masterTrxDetail.MasterTrxId = customerMasterTrx.MasterTrxId;
                        masterTrxDetail.InvoiceId = creditTemp.InvoiceId;
                        masterTrxDetail.LineNo = cvm.LineNo;
                        masterTrxDetail.MasterTrxTypeListId = isTaxCredit ? 58 : 3; // customer credit or customer tax credit
                        masterTrxDetail.HeaderId = customerCredit.CreditId;
                        masterTrxDetail.RegionId = creditTemp.RegionId;
                        masterTrxDetail.CreatedBy = creditTemp.CreatedBy;
                        masterTrxDetail.CreatedDate = creditTemp.CreatedDate;
                        masterTrxDetail.IsDelete = false;

                        masterTrxDetail.ServiceTypeListId = cvm.ServiceTypeListId;
                        masterTrxDetail.Quantity = 1;
                        masterTrxDetail.UnitPrice = cvm.ExtendedPrice;
                        masterTrxDetail.ExtendedPrice = cvm.ExtendedPrice;
                        masterTrxDetail.FeesDetail = false;
                        masterTrxDetail.TaxDetail = true;
                        masterTrxDetail.TotalTax = Math.Round((decimal)cvm.TotalTax,2);
                        masterTrxDetail.Total = cvm.Total;
                        masterTrxDetail.AmountTypeListId = 1; // credit
                        masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                        masterTrxDetail.ClassId = creditTemp.CustomerId;
                        masterTrxDetail.TypelistId = 1;
                        masterTrxDetail.Transactiondate = creditTemp.TransactionDate;
                        masterTrxDetail.DetailDescription = creditTemp.CreditDescription;
                        masterTrxDetail.FRRevenues = true;
                        masterTrxDetail.BPPAdmin = 0;
                        masterTrxDetail.FRDeduction = false;

                        List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == creditTemp.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                        if (_masterINVDetail.Count > 0)
                        {
                            masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                            masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                            masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                            //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                            masterTrxDetail.Quantity = 0;
                            masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                            masterTrxDetail.TotalFee = 0;
                            masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                            masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                            masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                            masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                            masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                        }
                        masterTrxDetail.TransactionStatusListId = 3;
                        context.MasterTrxDetails.Add(masterTrxDetail);


                        context.SaveChanges();
                        lastMasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;

                        //Insert customer taxes

                        MasterTrxTax customerTax = new MasterTrxTax();
                        customerTax.MasterTrxDetailId = masterTrxDetail.MasterTrxDetailId;
                        customerTax.Amount = cvm.TotalTax;
                        if (!isTaxCredit)
                            customerTax.TaxratePercentage = Decimal.Round((decimal)cvm.TotalTax * 100.00M / (decimal)cvm.ExtendedPrice, 2);
                        customerTax.AmountTypeListId = 1; // credit
                        customerTax.IsDelete = false;
                        customerTax.InvoiceId = masterTrxDetail.InvoiceId;
                        customerTax.RegionId = masterTrxDetail.RegionId;
                        customerTax.PeriodId = masterTrxDetail.PeriodId;
                        customerTax.CustomerId = masterTrxDetail.ClassId;
                        customerTax.CreatedBy = creditTemp.CreatedBy;
                        customerTax.CreatedDate = creditTemp.CreatedDate;
                        customerTax.FRRevenues = true;

                        foreach (var item in creditTempDetailFR)
                        {
                            customerTax.FranchiseeId = ((masterTrxDetail.LineNo == item.LineNo) ? item.ClassId : 0);
                        }
                        context.MasterTrxTaxes.Add(customerTax);


                        var invoice = context.Invoices.Where(o => o.InvoiceId == masterTrxDetail.InvoiceId).FirstOrDefault();
                        var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                        var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                        
                        List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
                        decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
                        decimal totalTransactions = 0.00m;
                        decimal grandTotalTransactions = 0.00m;

                        foreach (var trx in invoiceTransactions)
                        {
                            totalTransactions = totalTransactions + (decimal)trx.Total;
                        }
                        grandTotalTransactions = totalTransactions + (decimal)cvm.Total;


                        if (grandTotalTransactions >= invoiceTotal)
                        {
                            invoice.TransactionStatusListId = 6; /*6 = Paid*/
                            invoiceMasterTrx.StatusId = 6;
                        }
                        else
                        {
                            invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
                            invoiceMasterTrx.StatusId = 7;
                        }

                        context.SaveChanges();

                        totalCustomerCredit += (decimal)cvm.ExtendedPrice;
                        totalCustomerTaxes += (decimal)cvm.TotalTax;
                    }

                    if (!isTaxCredit)
                        foreach (var fcvm in creditTempDetailFR)
                        {
                            decimal totalFees = 0;

                            List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.BaseMasterTrxDetailId).ToList();
                            List<MasterTrxFeeDetail> franchiseeFees = new List<MasterTrxFeeDetail>();

                            foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                            {
                                MasterTrxFeeDetail feeDetail = null;
                                if (feeDetail == null)
                                {
                                    feeDetail = new MasterTrxFeeDetail();
                                    feeDetail.MasterTrxFeeDetailId = -1;
                                    feeDetail.FeeId = feeDef.FeeId;
                                    feeDetail.AmountTypeListId = 1; // credit
                                    feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                                    feeDetail.CreatedBy = creditTemp.CreatedBy;
                                    feeDetail.CreatedDate = creditTemp.CreatedDate;
                                    feeDetail.FRRevenues = false;
                                    feeDetail.FRDeduction = true;
                                    feeDetail.RegionId = creditTemp.RegionId;
                                    feeDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    feeDetail.BillingPayId = fcvm.FC_BillingPayId;
                                    feeDetail.FranchiseeId = fcvm.ClassId;
                                }

                                if (feeDef.FeePercentage != null) // percentage
                                {
                                    feeDetail.FeePercentage = feeDef.FeePercentage;
                                    feeDetail.Amount = (decimal)(fcvm.ExtendedPrice * feeDetail.FeePercentage / 100.0M); // Math.Round((decimal)(fcvm.ExtendedPrice * feeDetail.FeePercentage / 100.0M), 2);
                                }
                                else // flat amount
                                {
                                    feeDetail.Amount = feeDef.Amount;
                                    feeDetail.FeePercentage = null;
                                }

                                totalFees += feeDetail.Amount ?? 0;

                                franchiseeFees.Add(feeDetail);
                            }

                            // franchisee credit mastertrx

                            MasterTrx franchiseeMasterTrx = new MasterTrx();
                            franchiseeMasterTrx.MasterTrxTypeListId = 8; // franchisee credit
                            franchiseeMasterTrx.ClassId = fcvm.ClassId;
                            franchiseeMasterTrx.TypeListId = 2; // franchisee
                            franchiseeMasterTrx.RegionId = creditTemp.RegionId;
                            franchiseeMasterTrx.TrxDate = creditTemp.TransactionDate;
                            franchiseeMasterTrx.BillMonth = PR.BillMonth;
                            franchiseeMasterTrx.BillYear = PR.BillYear;
                            franchiseeMasterTrx.StatusId = 3; // open
                            franchiseeMasterTrx.CreatedBy = creditTemp.CreatedBy;
                            franchiseeMasterTrx.CreatedDate = DateTime.Now;
                            franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                            context.MasterTrxes.Add(franchiseeMasterTrx);

                            context.SaveChanges();

                            // franchisee credit

                            franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(8, (int)creditTemp.RegionId);
                            string franchiseeNextTrxNumber = null;


                            if (franchiseeTransactionNumberConfigViewModel != null)
                                franchiseeNextTrxNumber = CompanySvc.GetNextTransactionNumberConfig(8, (int)creditTemp.RegionId, (DateTime)creditTemp.CreatedDate);

                            CreditFranchisee franchiseeCredit = new CreditFranchisee();
                            franchiseeCredit.BillingPayId = fcvm.FC_BillingPayId;
                            franchiseeCredit.FranchiseeId = fcvm.ClassId;
                            franchiseeCredit.RegionId = creditTemp.RegionId;
                            franchiseeCredit.TransactionStatusListId = 1; // pending approval
                            franchiseeCredit.TransactionNumber = franchiseeNextTrxNumber;
                            franchiseeCredit.isActive = true;
                            franchiseeCredit.CreatedBy = creditTemp.CreatedBy;
                            franchiseeCredit.CreatedDate = creditTemp.CreatedDate;
                            franchiseeCredit.PeriodId = (PR != null ? PR.PeriodId : 0);
                            franchiseeCredit.TransactionDate = creditTemp.TransactionDate;
                            franchiseeCredit.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                            franchiseeCredit.CreditId = customerCredit.CreditId;
                            franchiseeCredit.CreditFranchiseeDescription = creditTemp.CreditDescription;
                            context.CreditFranchisees.Add(franchiseeCredit);

                            context.SaveChanges();

                            franchiseeMasterTrx.HeaderId = franchiseeCredit.CreditFranchiseeId;
                            context.SaveChanges();

                            decimal creditMinusFees = (decimal)(fcvm.ExtendedPrice - totalFees); // Math.Round((decimal)(fcvm.ExtendedPrice - totalFees), 2); // credit amount after taking out fees


                            MasterTrxDetail franchiseeMasterTrxDetail = new MasterTrxDetail();
                            franchiseeMasterTrxDetail.InvoiceId = creditTemp.InvoiceId;
                            franchiseeMasterTrxDetail.LineNo = fcvm.LineNo;
                            franchiseeMasterTrxDetail.MasterTrxTypeListId = 8; // franchisee credit
                            franchiseeMasterTrxDetail.HeaderId = franchiseeCredit.CreditFranchiseeId;
                            franchiseeMasterTrxDetail.RegionId = creditTemp.RegionId;
                            franchiseeMasterTrxDetail.ServiceTypeListId = fcvm.ServiceTypeListId != 0 ? fcvm.ServiceTypeListId : creditTempDetailCUS[0].ServiceTypeListId;

                            franchiseeMasterTrxDetail.CreatedBy = creditTemp.CreatedBy;
                            franchiseeMasterTrxDetail.CreatedDate = creditTemp.CreatedDate;
                            franchiseeMasterTrxDetail.IsDelete = false;

                            franchiseeMasterTrxDetail.Quantity = 1;
                            franchiseeMasterTrxDetail.UnitPrice = fcvm.ExtendedPrice;
                            franchiseeMasterTrxDetail.ExtendedPrice = fcvm.ExtendedPrice;
                            franchiseeMasterTrxDetail.FeesDetail = true;
                            franchiseeMasterTrxDetail.TaxDetail = false;
                            franchiseeMasterTrxDetail.TotalFee = totalFees;
                            franchiseeMasterTrxDetail.Total = creditMinusFees;
                            franchiseeMasterTrxDetail.AmountTypeListId = 2; // credit
                            franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                            franchiseeMasterTrxDetail.ClassId = franchiseeMasterTrx.ClassId;
                            franchiseeMasterTrxDetail.TypelistId = 2;
                            franchiseeMasterTrxDetail.Transactiondate = creditTemp.TransactionDate;
                            franchiseeMasterTrxDetail.DetailDescription = creditTemp.CreditDescription;
                            franchiseeMasterTrxDetail.FRRevenues = true;
                            franchiseeMasterTrxDetail.FRDeduction = false;
                            franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.MasterTrxId;
                            franchiseeMasterTrxDetail.BPPAdmin = 0;
                            franchiseeMasterTrxDetail.TransactionStatusListId = 3;

                            List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == creditTemp.InvoiceId && m.LineNo == fcvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                            if (_masterINVDetail.Count > 0)
                            {
                                franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                franchiseeMasterTrxDetail.Quantity = 0;
                                franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                franchiseeMasterTrxDetail.TotalTax = 0;
                                franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                            }

                            List<FranchiseeBillSetting> lstFranchiseeBillSetting = context.FranchiseeBillSettings.Where(h => h.FranchiseeId == franchiseeMasterTrx.ClassId).ToList();
                            if (lstFranchiseeBillSetting.Count > 0)
                            {
                                if (lstFranchiseeBillSetting[0].BBPAdministrationFee == true)
                                {
                                    franchiseeMasterTrxDetail.BPPAdmin = 1;
                                }
                                else { franchiseeMasterTrxDetail.BPPAdmin = 0; }
                                if (lstFranchiseeBillSetting[0].AccountRebate == true)
                                {
                                    franchiseeMasterTrxDetail.AccountRebate = 1;
                                }
                                else { franchiseeMasterTrxDetail.AccountRebate = 0; }

                            }
                            context.MasterTrxDetails.Add(franchiseeMasterTrxDetail);
                            context.SaveChanges();
                            if (franchiseeNextTrxNumber != null)
                            {
                                franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
                                CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                            }

                            // insert franchisee fees

                            foreach (MasterTrxFeeDetail feeDetail in franchiseeFees)
                            {
                                feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.MasterTrxDetailId; // set the id after insertion

                                if (feeDetail.MasterTrxFeeDetailId == -1)
                                    context.MasterTrxFeeDetails.Add(feeDetail);
                                else
                                    context.Entry(feeDetail).State = EntityState.Modified;
                            }

                            context.SaveChanges();
                        }

                    if (customerNextTrxNumber != null)
                    {
                        customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
                        CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                    }
                    
                    //// update original invoice's status
                    Invoice existingInvoice = context.Invoices.Where(o => o.InvoiceId == creditTemp.InvoiceId).FirstOrDefault();
                    if (existingInvoice != null)
                    {
                        var __INVBal = context.fn_GetInvoiceBalance(creditTemp.InvoiceId).FirstOrDefault();
                        if (__INVBal != null)
                        {
                            existingInvoice.TransactionStatusListId = __INVBal.BalanceTotal <= 0 ? 6 : 7; // paid : paid partial
                            existingInvoice.ModifiedBy = creditTemp.CreatedBy;
                            existingInvoice.ModifiedDate = creditTemp.CreatedDate;
                            context.Entry(existingInvoice).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }


                creditTemp.TransactionStatusListId = status;
                creditTemp.ModifiedBy = LoginUserId;
                creditTemp.ModifiedDate = DateTime.Now;
                foreach (var item in creditTempDetailCUS) item.TransactionStatusListId = status;
                foreach (var item in creditTempDetailFR) item.TransactionStatusListId = status;
                context.SaveChanges();
                int CustomerId = (int)creditTemp.CustomerId;

                //send Notification
                savePendingMessage(note, CustomerId, status, -1);

            }

            return true;
        }

        public bool DeleteCredit(int creditId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Credit credit = context.Credits.Where(o => o.CreditId == creditId).FirstOrDefault();
                if (credit == null && credit.TransactionStatusListId != 1)
                    return false;

                credit.TransactionStatusListId = 13; // 4 = open, 12 - Reject
                credit.IsDelete = true;
                credit.ModifiedBy = this.LoginUserId;
                credit.ModifiedDate = DateTime.Now;
                context.Entry(credit).State = EntityState.Modified;

                MasterTrx oMasterTrx = context.MasterTrxes.SingleOrDefault(o => o.MasterTrxId == credit.MasterTrxId);
                oMasterTrx.StatusId = 13;


                List<MasterTrxDetail> masterTrxDetails = context.MasterTrxDetails.Where(o => o.MasterTrxId == credit.MasterTrxId).ToList();
                foreach (var item in masterTrxDetails)
                {
                    item.IsDelete = true;
                    item.TransactionStatusListId = 13;
                }

                List<CreditFranchisee> lstCreditFranchisee = context.CreditFranchisees.Where(o => o.CreditId == credit.CreditId).ToList();
                foreach (var item in lstCreditFranchisee)
                {
                    item.isActive = false;
                    item.TransactionStatusListId = 13;

                    MasterTrx cfMasterTrx = context.MasterTrxes.SingleOrDefault(o => o.MasterTrxId == item.MasterTrxId);
                    if (cfMasterTrx != null)
                        oMasterTrx.StatusId = 13;

                    List<MasterTrxDetail> cfmasterTrxDetails = context.MasterTrxDetails.Where(o => o.MasterTrxId == item.MasterTrxId).ToList();
                    foreach (var item1 in cfmasterTrxDetails)
                    {
                        item1.IsDelete = true;
                        item1.TransactionStatusListId = 13;
                    }
                }





                context.SaveChanges();

            }
            return true;
        }


        public bool DeleteCreditTemp(int creditId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CreditTemp credit = context.CreditTemps.Where(o => o.CreditTempId == creditId).FirstOrDefault();
                if (credit == null && credit.TransactionStatusListId != 1)
                    return false;

                credit.TransactionStatusListId = 13; // 4 = open, 12 - Reject
                credit.ModifiedBy = this.LoginUserId;
                credit.ModifiedDate = DateTime.Now;
                context.Entry(credit).State = EntityState.Modified;

                List<CreditTempDetail> lstCreditTempDetail = context.CreditTempDetails.Where(o => o.CreditTempId == creditId).ToList();
                foreach (var item in lstCreditTempDetail)
                {
                    item.TransactionStatusListId = 13;
                    item.ModifiedBy = LoginUserId;
                    item.ModifiedDate = DateTime.Now;
                }
                context.SaveChanges();

            }
            return true;
        }


        public bool GetBillValidated(int month, int year, int batchid)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var ListItem = context.portal_spGet_AR_BillRunSummaryDetailValidatedPeriod(SelectedRegionId, month, year);
                if (ListItem != null && ListItem.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public BillRunSummaryDetailViewModel GetBillRunSummaryDetail(int month, int year, int batchid, string selectedRegionId = "0")
        {
            //SELECT COUNT(InvoiceId) as TotalInvoiceCount, ISNULL(SUM(GL.Credit), 0) AS TotalInvoiceAmount, MAX(INV.InvoiceDate) AS InvoiceCreatedOn
            //FROM[dbo].[MasterTrx] AS MT
            //INNER JOIN[dbo].[Invoice] as INV On MT.[MasterTrxId] = INV.MasterTrxId AND MT.[MasterTrxTypeListId] = 1
            //INNER JOIN[dbo].[GeneralLedger] AS GL ON MT.[MasterTrxId] = GL.[MasterTrxId]
            //WHERE MT.[BillMonth] = 1 AND MT.[BillYear] = 2017
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //var aaa = context.portal_spGet_AR_BillRunSummaryDetail(selectedRegionId, month, year, batchid);

                List<portal_spGet_AR_BillRunSummaryDetail_Result> lst = context.portal_spGet_AR_BillRunSummaryDetail(selectedRegionId, month, year, batchid).ToList();
                BillRunSummaryDetailViewModel oBillRunSummaryDetailViewModel = new BillRunSummaryDetailViewModel();

                if (lst.Count > 0)
                {
                    decimal _TotalInvoiceAmount = 0;
                    int _TotalInvoiceCount = 0;


                    string _InvoiceCreatedOn, _BatchNumber = "", _BatchIds = "";

                    _InvoiceCreatedOn = lst.FirstOrDefault().InvoiceCreatedOn != null ? lst.FirstOrDefault().InvoiceCreatedOn.ToString() : "";
                    foreach (portal_spGet_AR_BillRunSummaryDetail_Result ob in lst)
                    {
                        _TotalInvoiceCount += (int)ob.TotalInvoiceCount;
                        _TotalInvoiceAmount += ob.TotalInvoiceAmount;
                        _BatchNumber += ob.BatchNumber.ToString() + ",";
                        _BatchIds += ob.BatchId.ToString() + ",";
                    }
                    oBillRunSummaryDetailViewModel.TotalInvoiceCount = _TotalInvoiceCount;
                    oBillRunSummaryDetailViewModel.InvoiceCreatedOn = _InvoiceCreatedOn != null ? _InvoiceCreatedOn.ToString() : "";
                    oBillRunSummaryDetailViewModel.TotalInvoiceAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", _TotalInvoiceAmount);
                    oBillRunSummaryDetailViewModel.BatchNumber = _BatchNumber.TrimEnd(',');
                    oBillRunSummaryDetailViewModel.BatchIds = _BatchIds.TrimEnd(',');
                }


                //if (batchid > 0)
                //{
                //    oBillRunSummaryDetailViewModel.TotalInvoiceCount = (int)lst.FirstOrDefault().TotalInvoiceCount;
                //    oBillRunSummaryDetailViewModel.InvoiceCreatedOn = lst.FirstOrDefault().InvoiceCreatedOn != null ? lst.FirstOrDefault().InvoiceCreatedOn.ToString() : "";
                //    oBillRunSummaryDetailViewModel.TotalInvoiceAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalInvoiceAmount));

                //    oBillRunSummaryDetailViewModel.BatchNumber = lst.FirstOrDefault().BatchNumber.ToString();

                //}
                //else
                //{
                //    oBillRunSummaryDetailViewModel.TotalInvoiceCount = (int)lst.Sum(a => a.TotalInvoiceCount);
                //    oBillRunSummaryDetailViewModel.InvoiceCreatedOn = Convert.ToDateTime(lst.Max(a => a.InvoiceCreatedOn)).ToString("MM/dd/yyyy hh:mm tt");

                //    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                //    Console.WriteLine("{0:c}", 4321.2);
                //    oBillRunSummaryDetailViewModel.TotalInvoiceAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalInvoiceAmount));

                //    oBillRunSummaryDetailViewModel.BatchNumber = 0;
                //}


                return oBillRunSummaryDetailViewModel;
            }
        }

        public bool GetUndoBillRun(int batchid, int month, int year, string selectedRegionId)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var aaa = context.portal_spCreate_AR_BillRunUndo(batchid);

                Period oP = context.Periods.SingleOrDefault(o => o.BillMonth == month && o.BillYear == year);
                if (selectedRegionId != "" && oP != null)
                {
                    if (selectedRegionId.Contains(','))
                    {
                        foreach (string _strId in selectedRegionId.Split(','))
                        {
                            PeriodClosed PC = context.PeriodCloseds.SingleOrDefault(o => o.RegionId.ToString() == _strId.Trim() && o.PeriodId == oP.PeriodId);
                            if (PC != null)
                            {
                                PC.MonthlyBillRun = false;
                                context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        PeriodClosed PC = context.PeriodCloseds.SingleOrDefault(o => o.RegionId.ToString() == selectedRegionId.Trim() && o.PeriodId == oP.PeriodId);
                        if (PC != null)
                        {
                            PC.MonthlyBillRun = false;
                            context.SaveChanges();
                        }
                    }
                }



                return true;
            }
        }

        public List<ARInvoiceListViewModel> GetOpenInvoiceListWithSearch(int month, int year, string searchtext, int invoicetypelistid, string regionId = "")
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<ARInvoiceListViewModel> lstARInvoiceListView = new List<ARInvoiceListViewModel>();

                List<portal_spGet_AR_OpenInvoiceListWithSearch_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_OpenInvoiceListWithSearch(!string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString(), month, year, searchtext, invoicetypelistid).ToList();

                foreach (portal_spGet_AR_OpenInvoiceListWithSearch_Result o in lstARInvoiceListViewModel)
                {
                    lstARInvoiceListView.Add(new ARInvoiceListViewModel()
                    {
                        InvoiceId = o.InvoiceId,
                        InvoiceDate = o.InvoiceDate,
                        InvoiceNo = o.InvoiceNo,
                        CustomerId = o.CustomerId,
                        CustomerNo = o.CustomerNo,
                        CustomerName = o.CustomerName,
                        Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
                        Ebill = o.EBill != null ? ((bool)o.EBill == true ? "E" : "") : "",
                        PrintInvoice = o.PrintInvoice != null ? ((bool)o.PrintInvoice == true ? "P" : "") : "",
                        Amount = o.Amount,
                        Balance = o.Balance,
                        DueDate = o.DueDate,
                        StatusId = o.StatusId,
                        Region = o.Region,
                        ContractAmount = o.ContractAmount,
                        TaxAmount = o.TaxAmount,
                        IsOpen = string.Equals(o.TransactionStatus, "Open", StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(o.TransactionStatus, "Paid Partial", StringComparison.InvariantCultureIgnoreCase) ? "Y" : "N",
                        TransactionStatus = o.TransactionStatus
                    });
                }

                return lstARInvoiceListView;
            }
            // new BillRunSummaryDetailViewModel();
        }
        public List<ARInvoiceListViewModel> GetInvoiceListWithSearch(int month, int year, string searchtext, int invoicetypelistid, string regionId = "", string sDate = "", string eDate = "")
        {


            List<ARInvoiceListViewModel> lstARInvoiceListView = new List<ARInvoiceListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@BillMonth", month);
                parmas.Add("@BillYear", year);
                parmas.Add("@SearchText", searchtext);
                parmas.Add("@InvoiceTypeListId", invoicetypelistid);
                parmas.Add("@sDate", sDate);
                parmas.Add("@eDate", eDate);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_InvoiceListWithSearchN", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {

                        List<portal_spGet_AR_InvoiceListWithSearch_Result> lstARInvoiceListViewModel = multipleresult.Read<portal_spGet_AR_InvoiceListWithSearch_Result>().ToList();

                        foreach (portal_spGet_AR_InvoiceListWithSearch_Result o in lstARInvoiceListViewModel)
                        {
                            lstARInvoiceListView.Add(new ARInvoiceListViewModel()
                            {
                                InvoiceId = o.InvoiceId,
                                InvoiceDate = o.InvoiceDate,
                                InvoiceNo = o.InvoiceNo,
                                CustomerId = o.CustomerId,
                                CustomerNo = o.CustomerNo,
                                CustomerName = o.CustomerName,
                                Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
                                Ebill = o.EBill != null ? ((bool)o.EBill == true ? "E" : "") : "",
                                PrintInvoice = o.PrintInvoice != null ? ((bool)o.PrintInvoice == true ? "P" : "") : "",
                                Amount = o.Amount,
                                Balance = o.Balance,
                                DueDate = o.DueDate,
                                StatusId = o.StatusId,
                                Region = o.Region,
                                ContractAmount = o.ContractAmount,
                                TaxAmount = o.TaxAmount,
                                IsOpen = string.Equals(o.TransactionStatus, "Open", StringComparison.InvariantCultureIgnoreCase)
                                || string.Equals(o.TransactionStatus, "Paid Partial", StringComparison.InvariantCultureIgnoreCase) ? "Y" : "N",
                                TransactionStatus = o.TransactionStatus,
                                BillingPayId = (o.BillingPayId != null ? o.BillingPayId : 0),
                                TransactionNumber = o.TransactionNumber,
                                Cpi = o.CPI.ToString(),

                                BillingPayCount = !String.IsNullOrEmpty(o.BillingPayIds) ? (o.BillingPayIds.Contains(',') ? o.BillingPayIds.Split(',').Count() : 1) : 0,
                                BillingPayIds = !String.IsNullOrEmpty(o.BillingPayIds) ? o.BillingPayIds : "",
                                BillingPayNumbers = !String.IsNullOrEmpty(o.BillingPayNumbers) ? o.BillingPayNumbers : ""

                            });
                        }
                    }
                }
                //return lstARInvoiceListView;
            }

            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{
            //    //ARInvoiceListViewModel

            //    List<ARInvoiceListViewModel> lstARInvoiceListView = new List<ARInvoiceListViewModel>();

            //    List<portal_spGet_AR_InvoiceListWithSearch_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_InvoiceListWithSearch(!string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString(), month, year, searchtext, invoicetypelistid).ToList();

            //    foreach (portal_spGet_AR_InvoiceListWithSearch_Result o in lstARInvoiceListViewModel)
            //    {
            //        lstARInvoiceListView.Add(new ARInvoiceListViewModel()
            //        {
            //            InvoiceId = o.InvoiceId,
            //            InvoiceDate = o.InvoiceDate,
            //            InvoiceNo = o.InvoiceNo,
            //            CustomerId = o.CustomerId,
            //            CustomerNo = o.CustomerNo,
            //            CustomerName = o.CustomerName,
            //            Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
            //            Ebill = o.EBill != null ? ((bool)o.EBill == true ? "E" : "") : "",
            //            PrintInvoice = o.PrintInvoice != null ? ((bool)o.PrintInvoice == true ? "P" : "") : "",
            //            Amount = o.Amount,
            //            Balance = o.Balance,
            //            DueDate = o.DueDate,
            //            StatusId = o.StatusId,
            //            Region = o.Region,
            //            ContractAmount = o.ContractAmount,
            //            TaxAmount = o.TaxAmount,
            //            IsOpen = string.Equals(o.TransactionStatus, "Open", StringComparison.InvariantCultureIgnoreCase)
            //            || string.Equals(o.TransactionStatus, "Paid Partial", StringComparison.InvariantCultureIgnoreCase) ? "Y" : "N",
            //            TransactionStatus = o.TransactionStatus,
            //            BillingPayId = (o.BillingPayId != null ? o.BillingPayId : 0),
            //            TransactionNumber = o.TransactionNumber,
            //            Cpi = o.CPI.ToString(),

            //            BillingPayCount = !String.IsNullOrEmpty(o.BillingPayIds) ? (o.BillingPayIds.Contains(',') ? o.BillingPayIds.Split(',').Count() : 1) : 0,
            //            BillingPayIds = !String.IsNullOrEmpty(o.BillingPayIds) ? o.BillingPayIds : "",
            //            BillingPayNumbers = !String.IsNullOrEmpty(o.BillingPayNumbers) ? o.BillingPayNumbers : ""

            //        });
            //    }

            return lstARInvoiceListView;
            //}
            // new BillRunSummaryDetailViewModel();
        }

        public List<InvoiceListViewModel> GetInvoiceList(string regionId, string sDate, string eDate)
        {
            List<InvoiceListViewModel> lstInvoiceListViewModel = new List<InvoiceListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", !string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString());
                parmas.Add("@sDate", sDate);
                parmas.Add("@eDate", eDate);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_InvoiceList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstInvoiceListViewModel = multipleresult.Read<InvoiceListViewModel>().ToList();
                    }
                }
            }

            return lstInvoiceListViewModel;
        }

        public List<ARInvoiceListViewModel> GetInvoiceListWithSearch(int month, int year, string searchtext, int filterby, int searchBy, string searchValue, bool eomOnly, bool consolidatedInvoice, int invoicetypelistid, string regionId = "")
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<ARInvoiceListViewModel> lstARInvoiceListView = new List<ARInvoiceListViewModel>();

                List<portal_spGet_AR_InvoiceSearchListByNameAndInvoiceNo_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_InvoiceSearchListByNameAndInvoiceNo(regionId, month, year, searchtext, filterby, searchBy, searchValue, eomOnly, consolidatedInvoice).ToList();

                foreach (portal_spGet_AR_InvoiceSearchListByNameAndInvoiceNo_Result o in lstARInvoiceListViewModel)
                {
                    lstARInvoiceListView.Add(new ARInvoiceListViewModel()
                    {
                        InvoiceId = o.InvoiceId,
                        InvoiceDate = o.InvoiceDate,
                        InvoiceNo = o.InvoiceNo,
                        CustomerId = o.CustomerId,
                        CustomerNo = o.CustomerNo,
                        CustomerName = o.CustomerName,
                        Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
                        Ebill = o.EBill != null ? ((bool)o.EBill == true ? "E" : "") : "",
                        PrintInvoice = o.PrintInvoice != null ? ((bool)o.PrintInvoice == true ? "P" : "") : "",
                        Amount = o.Amount,
                        Balance = o.Balance,
                        DueDate = o.DueDate,
                        StatusId = o.StatusId

                    });
                }

                return lstARInvoiceListView;
            }
            // new BillRunSummaryDetailViewModel();
        }

        public List<ARInvoiceListViewModel> GetInvoiceListWithSearchForCredit(string regionId = null, DateTime? fromdate = null, DateTime? todate = null, string searchtext = "", bool? closed = null, bool? consolidated = null, int sb = 0, int customerId = 0)
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<ARInvoiceListViewModel> lstARInvoiceListView = new List<ARInvoiceListViewModel>();

                List<portal_spGet_AR_InvoiceSearchListForCustomerCredits_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_InvoiceSearchListForCustomerCredits(!string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString(), fromdate, todate, (searchtext != "" ? searchtext.Trim() : ""), closed, consolidated, sb, customerId).ToList();

                foreach (portal_spGet_AR_InvoiceSearchListForCustomerCredits_Result o in lstARInvoiceListViewModel)
                {
                    lstARInvoiceListView.Add(new ARInvoiceListViewModel()
                    {
                        InvoiceId = o.InvoiceId,
                        InvoiceDate = o.InvoiceDate,
                        InvoiceNo = o.InvoiceNo,
                        CustomerId = o.CustomerId,
                        CustomerNo = o.CustomerNo,
                        CustomerName = o.CustomerName,
                        Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
                        Ebill = o.EBill != null ? ((bool)o.EBill == true ? "E" : "") : "",
                        PrintInvoice = o.PrintInvoice != null ? ((bool)o.PrintInvoice == true ? "P" : "") : "",
                        Amount = o.Amount,
                        Balance = o.Balance,
                        DueDate = o.DueDate,
                        StatusId = o.StatusId
                    });
                }

                return lstARInvoiceListView;
            }
            // new BillRunSummaryDetailViewModel();
        }

        public List<ARLBInvoiceListViewModel> GetOpenInvoiceListForLockbox(int regionId, string DateFrom, string DateTo, string transactionStatusListId = "4,7", bool consolidated = false)
        {
            List<ARLBInvoiceListViewModel> lstARInvoiceListView = new List<ARLBInvoiceListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                parmas.Add("@TransactionStatusListId", transactionStatusListId);
                if (DateFrom != "")
                    parmas.Add("@DateFrom", DateFrom);
                if (DateTo != "")
                    parmas.Add("@DateTo", DateTo);
                if (consolidated == true)
                    parmas.Add("@Consolidated", true);

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_OpenInvoiceListForLockbox", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {

                        lstARInvoiceListView = multipleresult.Read<ARLBInvoiceListViewModel>().ToList();

                        //foreach (portal_spGet_AR_OpenInvoiceListWithSearch_Result o in lstARInvoiceListViewModel)
                        //{
                        //    lstARInvoiceListView.Add(new ARLBInvoiceListViewModel()
                        //    {
                        //        InvoiceId = o.InvoiceId,
                        //        InvoiceDate = o.InvoiceDate,
                        //        InvoiceNo = o.InvoiceNo,
                        //        CustomerId = o.CustomerId,
                        //        CustomerNo = o.CustomerNo,
                        //        CustomerName = o.CustomerName,
                        //        Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
                        //        Amount = o.Amount,
                        //        Balance = o.Balance
                        //    });
                        //}
                    }
                }
                return lstARInvoiceListView;
            }

        }

        public List<ARInvoiceListViewModel> GetInvoiceListWithSearchForPayment(string regionId, string searchtext, bool consolidated, string OCValue, int month, int year)
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<ARInvoiceListViewModel> lstARInvoiceListView = new List<ARInvoiceListViewModel>();

                // todo: make new SP to filter invoices properly for payment
                List<portal_spGet_AR_InvoiceListForManualPayment_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_InvoiceListForManualPayment(!string.IsNullOrWhiteSpace(regionId) ? regionId : SelectedRegionId.ToString(), searchtext, false, consolidated, OCValue, month, year).ToList();

                foreach (portal_spGet_AR_InvoiceListForManualPayment_Result o in lstARInvoiceListViewModel)
                {
                    lstARInvoiceListView.Add(new ARInvoiceListViewModel()
                    {
                        InvoiceId = o.InvoiceId,
                        InvoiceDate = o.InvoiceDate,
                        InvoiceNo = o.InvoiceNo,
                        CustomerId = o.CustomerId,
                        CustomerNo = o.CustomerNo,
                        CustomerName = o.CustomerName,
                        Description = o.InvoiceDescription != null ? o.InvoiceDescription : "",
                        Ebill = o.EBill != null ? ((bool)o.EBill == true ? "E" : "") : "",
                        PrintInvoice = o.PrintInvoice != null ? ((bool)o.PrintInvoice == true ? "P" : "") : "",
                        Amount = o.Amount,
                        Balance = o.Balance,
                        DueDate = o.DueDate,
                        StatusId = o.StatusId,
                        HasMultipleLineItems = (o.HasMultipleLineItems == 1)
                    });
                }

                return lstARInvoiceListView;
            }
            // new BillRunSummaryDetailViewModel();
        }
        public List<int> GetPaymentIdsforApprove(int paymnetId)
        {
            List<int> relVal = new List<int>();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var oPayments = context.Payments.SingleOrDefault(c => c.PaymentId == paymnetId);
                if (oPayments != null)
                {
                    relVal = context.Payments.Where(c => c.MasterTrxId == oPayments.MasterTrxId).Select(d => d.PaymentId).ToList<int>();

                }
            }
            return relVal;
        }


        public List<ARInvoiceListViewModel> GetInvoiceWithSearchForManualPayment(string rgId, string OCValue, string st = "", int sb = 0)
        {
            List<ARInvoiceListViewModel> lstARLogList = new List<ARInvoiceListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", rgId);
                parmas.Add("@SearchText", st);
                parmas.Add("@SearchBy", sb);
                parmas.Add("@OCValue", OCValue);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_InvoiceForManualPayment", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        List<ARInvoiceListViewModel> lstResult = multipleresult.Read<ARInvoiceListViewModel>().ToList();
                        foreach (ARInvoiceListViewModel o in lstResult)
                        {
                            lstARLogList.Add(new ARInvoiceListViewModel()
                            {
                                InvoiceId = o.InvoiceId,
                                InvoiceDate = o.InvoiceDate,
                                InvoiceNo = o.InvoiceNo,
                                CustomerId = o.CustomerId,
                                CustomerNo = o.CustomerNo,
                                CustomerName = o.CustomerName,
                                Description = o.Description != null ? o.Description : "",
                                Ebill = o.Ebill != null ? (o.Ebill == "1" ? "E" : "") : "",
                                PrintInvoice = o.PrintInvoice != null ? (o.PrintInvoice == "1" ? "P" : "") : "",
                                Amount = o.Amount,
                                Balance = o.Balance,
                                DueDate = o.DueDate,
                                StatusId = o.StatusId,
                                HasMultipleLineItems = (o.HasMultipleLineItems == true),
                                ParentCustomerId = o.ParentCustomerId
                            });
                        }

                    }
                }
            }
            return lstARLogList;
        }

        public decimal GetCustomerBalance(string Customerno)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var oCus = context.Customers.Where(c => c.CustomerNo.Trim() == Customerno.Trim() && c.RegionId == SelectedRegionId).FirstOrDefault();
                if (oCus != null)
                {
                    List<Overflow> lst = context.Overflows.Where(c => c.ClassId == oCus.CustomerId && c.TypeListId == 1).ToList();
                    if (lst.Count() > 0)
                    {
                        var _debitAmount = lst.ToList().Where(o => o.AmountTypeListId == 1).Sum(v => v.Amount).Value;
                        var _creditAmount = lst.ToList().Where(o => o.AmountTypeListId == 2).Sum(v => v.Amount).Value;

                        return _creditAmount - _debitAmount;

                    }


                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        public List<Overflow> GetCustomerOverflowBalance(string Customerno, out decimal BalanceAMT)
        {
            BalanceAMT = 0;
            List<Overflow> lst = new List<Overflow>();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var oCus = context.Customers.Where(c => c.CustomerNo.Trim() == Customerno.Trim() && c.RegionId == SelectedRegionId).FirstOrDefault();
                if (oCus != null)
                {
                    lst = context.Overflows.Where(c => c.ClassId == oCus.CustomerId && c.TypeListId == 1).ToList();
                    if (lst.Count() > 0)
                    {
                        var _debitAmount = lst.ToList().Where(o => o.AmountTypeListId == 1).Sum(v => v.Amount).Value;
                        var _creditAmount = lst.ToList().Where(o => o.AmountTypeListId == 2).Sum(v => v.Amount).Value;

                        BalanceAMT = _creditAmount - _debitAmount;
                        return lst;
                    }
                    else
                        return lst;
                }
                else
                    return lst;
            }
        }

        public List<Overflow> GetCustomerOverflowBalanceById(int CustomerId, out decimal BalanceAMT)
        {
            BalanceAMT = 0;
            List<Overflow> lst = new List<Overflow>();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Customer oCus = context.Customers.Where(c => c.CustomerId == CustomerId).FirstOrDefault();
                if (oCus != null)
                {
                    lst = context.Overflows.Where(c => c.ClassId == oCus.CustomerId && c.TypeListId == 1).ToList();
                    if (lst.Count() > 0)
                    {
                        var _debitAmount = lst.ToList().Where(o => o.AmountTypeListId == 1).Sum(v => v.Amount).Value;
                        var _creditAmount = lst.ToList().Where(o => o.AmountTypeListId == 2).Sum(v => v.Amount).Value;

                        BalanceAMT = _creditAmount - _debitAmount;
                        return lst;
                    }
                    else
                        return lst;
                }
                else
                    return lst;
            }
        }

        public List<ARInvoiceListViewModel> GetInvoiceWithSearchForBalanceAdjustment(string rgId, string OCValue, decimal amountFrom, decimal amountTo, string st = "", int sb = 0, int servicet = 0)
        {
            List<ARInvoiceListViewModel> lstARLogList = new List<ARInvoiceListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", rgId);
                parmas.Add("@SearchText", st);
                parmas.Add("@SearchBy", sb);
                parmas.Add("@OCValue", OCValue);
                if (servicet == 77)
                {
                    parmas.Add("@amountfrom", -amountTo);
                    parmas.Add("@amountTo", -amountFrom);
                }
                else
                {
                    parmas.Add("@amountfrom", amountFrom);
                    parmas.Add("@amountTo", amountTo);
                }


                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_InvoiceForBalanceAdjustment", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        List<ARInvoiceListViewModel> lstResult = multipleresult.Read<ARInvoiceListViewModel>().ToList();
                        foreach (ARInvoiceListViewModel o in lstResult)
                        {
                            lstARLogList.Add(new ARInvoiceListViewModel()
                            {
                                InvoiceId = o.InvoiceId,
                                InvoiceDate = o.InvoiceDate,
                                InvoiceNo = o.InvoiceNo,
                                CustomerId = o.CustomerId,
                                CustomerNo = o.CustomerNo,
                                CustomerName = o.CustomerName,
                                Description = o.Description != null ? o.Description : "",
                                Ebill = o.Ebill != null ? (o.Ebill == "1" ? "E" : "") : "",
                                PrintInvoice = o.PrintInvoice != null ? (o.PrintInvoice == "1" ? "P" : "") : "",
                                Amount = o.Amount,
                                Balance = o.Balance,
                                DueDate = o.DueDate,
                                StatusId = o.StatusId,
                                HasMultipleLineItems = (o.HasMultipleLineItems == true)
                            });
                        }

                    }
                }
            }
            return lstARLogList;
        }


        public BillRunSummaryDetailViewModel GenerateInvoiceBillRun(int month, int year, string selectedRegionId = "")
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                int Ibatchid_1 = 0;
                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {




                    var parmas = new DynamicParameters();
                    parmas.Add("@RegionId", selectedRegionId);
                    parmas.Add("@BillMonth", month);
                    parmas.Add("@BillYear", year);
                    parmas.Add("@CreatedBy", LoginUserId);
                    parmas.Add("@BatchId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    int retVal = conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_Invoice", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure);
                    Ibatchid_1 = parmas.Get<int>("@BatchId");


                    var parmas1 = new DynamicParameters();
                    parmas1.Add("@RegionId", selectedRegionId);
                    parmas1.Add("@BillMonth", month);
                    parmas1.Add("@BillYear", year);
                    parmas1.Add("@CreatedBy", LoginUserId);
                    parmas1.Add("@IBatchId", Ibatchid_1);
                    parmas1.Add("@BatchId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    retVal = conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_BillingPay", parmas1, commandTimeout: 1000, commandType: CommandType.StoredProcedure);


                }
                //ObjectParameter batchid = new ObjectParameter("BatchId", typeof(int));
                //context.portal_spCreate_AR_BillRunGenerate(SelectedRegionId, month, year, 1, batchid);


                //ObjectParameter Ibatchid = new ObjectParameter("BatchId", typeof(int));
                //context.portal_spCreate_AR_GenerateBillRun_Invoice(selectedRegionId, month, year, LoginUserId, Ibatchid);

                //ObjectParameter Bbatchid = new ObjectParameter("BatchId", typeof(int));
                //context.portal_spCreate_AR_GenerateBillRun_BillingPay(selectedRegionId, month, year, LoginUserId, (int)Ibatchid.Value, Bbatchid);




                //List<portal_spGet_AR_BillRunSummaryDetail_Result> lst = context.portal_spGet_AR_BillRunSummaryDetail(selectedRegionId, month, year, (int)Ibatchid.Value).ToList();
                List<portal_spGet_AR_BillRunSummaryDetail_Result> lst = context.portal_spGet_AR_BillRunSummaryDetail(selectedRegionId, month, year, Ibatchid_1).ToList();
                BillRunSummaryDetailViewModel oBillRunSummaryDetailViewModel = new BillRunSummaryDetailViewModel();

                if (lst.Count > 0)
                {
                    decimal _TotalInvoiceAmount = 0;
                    int _TotalInvoiceCount = 0;


                    string _InvoiceCreatedOn, _BatchNumber = "", _BatchIds = "";

                    _InvoiceCreatedOn = lst.FirstOrDefault().InvoiceCreatedOn != null ? lst.FirstOrDefault().InvoiceCreatedOn.ToString() : "";
                    foreach (portal_spGet_AR_BillRunSummaryDetail_Result ob in lst)
                    {
                        _TotalInvoiceCount += (int)ob.TotalInvoiceCount;
                        _TotalInvoiceAmount += ob.TotalInvoiceAmount;
                        _BatchNumber += ob.BatchNumber.ToString() + ",";
                        _BatchIds += ob.BatchId.ToString() + ",";
                    }
                    oBillRunSummaryDetailViewModel.TotalInvoiceCount = _TotalInvoiceCount;
                    oBillRunSummaryDetailViewModel.InvoiceCreatedOn = _InvoiceCreatedOn != null ? _InvoiceCreatedOn.ToString() : "";
                    oBillRunSummaryDetailViewModel.TotalInvoiceAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", _TotalInvoiceAmount);
                    oBillRunSummaryDetailViewModel.BatchNumber = _BatchNumber.TrimEnd(',');
                    oBillRunSummaryDetailViewModel.BatchIds = _BatchIds.TrimEnd(',');

                    Period oP = context.Periods.SingleOrDefault(o => o.BillMonth == month && o.BillYear == year);
                    if (selectedRegionId != "" && oP != null)
                    {
                        if (selectedRegionId.Contains(','))
                        {
                            foreach (string _strId in selectedRegionId.Split(','))
                            {
                                PeriodClosed PC = context.PeriodCloseds.SingleOrDefault(o => o.RegionId.ToString() == _strId.Trim() && o.PeriodId == oP.PeriodId);
                                if (PC != null)
                                {
                                    if (PC.ChargebackFinalized == null)
                                        PC.ChargebackFinalized = false;
                                    if (PC.FranchiseeReport == null)
                                        PC.FranchiseeReport = false;

                                    PC.MonthlyBillRun = true;
                                    if (PC.Closed == null)
                                        PC.Closed = false;
                                    context.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            PeriodClosed PC = context.PeriodCloseds.SingleOrDefault(o => o.RegionId.ToString() == selectedRegionId.Trim() && o.PeriodId == oP.PeriodId);
                            if (PC != null)
                            {
                                if (PC.ChargebackFinalized == null)
                                    PC.ChargebackFinalized = false;
                                if (PC.FranchiseeReport == null)
                                    PC.FranchiseeReport = false;

                                if (PC.Closed == null)
                                    PC.Closed = false;


                                PC.MonthlyBillRun = true;
                                context.SaveChanges();
                            }
                        }
                    }
                }

                //List<portal_spGet_AR_BillRunSummaryDetail_Result> lst = context.portal_spGet_AR_BillRunSummaryDetail(selectedRegionId, month, year, (int)Ibatchid.Value).ToList();
                //BillRunSummaryDetailViewModel oBillRunSummaryDetailViewModel = new BillRunSummaryDetailViewModel();
                //if ((int)Ibatchid.Value > 0 && lst != null && lst.Count > 0)
                //{
                //    oBillRunSummaryDetailViewModel.TotalInvoiceCount = (int)lst.FirstOrDefault().TotalInvoiceCount;
                //    oBillRunSummaryDetailViewModel.InvoiceCreatedOn = lst.FirstOrDefault().InvoiceCreatedOn != null ? lst.FirstOrDefault().InvoiceCreatedOn.ToString() : "";
                //    oBillRunSummaryDetailViewModel.TotalInvoiceAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalInvoiceAmount));

                //    oBillRunSummaryDetailViewModel.BatchNumber = (long)lst.FirstOrDefault().BatchNumber;

                //}
                //else
                //{
                //    oBillRunSummaryDetailViewModel.TotalInvoiceCount = (int)lst.Sum(a => a.TotalInvoiceCount);
                //    oBillRunSummaryDetailViewModel.InvoiceCreatedOn = Convert.ToDateTime(lst.Max(a => a.InvoiceCreatedOn)).ToString("MM/dd/yyyy hh:mm tt");


                //    oBillRunSummaryDetailViewModel.TotalInvoiceAmount = String.Format(CultureInfo.CurrentCulture, "{0:C}", lst.Sum(a => a.TotalInvoiceAmount));


                //    var aae = lst.Select(l => new { BatchNumber = string.Join(",", l.BatchNumber.ToString().ToArray()) }).ToString();

                //    oBillRunSummaryDetailViewModel.BatchNumber = 0;
                //}

                return oBillRunSummaryDetailViewModel;
            }
        }

        public InvoiceDetailViewModel GetInvoiceDetailData(int invoiceid)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                InvoiceDetailViewModel oInvoiceDetailViewModel = new InvoiceDetailViewModel();

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceid);
                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_GetInvoiceDetail", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            var _InvoiceDetail = multipleresult.Read<vw_InvoiceDetailViewModel>().ToList();
                            var _InvoiceDetailItems = multipleresult.Read<vw_InvoiceContractDetailList>().ToList();
                            var _InvoicFranchiseeItems = multipleresult.Read<InvoiceFranchiseeBillingDetailViewModel>().ToList();

                            oInvoiceDetailViewModel.InvoiceDetail = _InvoiceDetail.FirstOrDefault();
                            oInvoiceDetailViewModel.InvoiceDetailItems = _InvoiceDetailItems;
                            oInvoiceDetailViewModel.FranchiseeBillingDetails = _InvoicFranchiseeItems;

                            var InvDetail = context.MasterTrxDetails.Where(x => x.InvoiceId == invoiceid && (x.MasterTrxTypeListId == 1 || x.MasterTrxTypeListId == 5)).FirstOrDefault();
                            oInvoiceDetailViewModel.MasterTrxTypeListId = (int)InvDetail.MasterTrxTypeListId;
                        }
                    }
                }

                return oInvoiceDetailViewModel;
            }
        }
        public InvoiceDetailViewModel GetInvoiceDetail(int invoiceid)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                InvoiceDetailViewModel oInvoiceDetailViewModel = new InvoiceDetailViewModel();

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceid);
                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_GetInvoiceDetail", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            var _InvoiceDetail = multipleresult.Read<vw_InvoiceDetailViewModel>().ToList();
                            var _InvoiceDetailItems = multipleresult.Read<vw_InvoiceContractDetailList>().ToList();
                            var _InvoicFranchiseeItems = multipleresult.Read<InvoiceFranchiseeBillingDetailViewModel>().ToList();

                            oInvoiceDetailViewModel.InvoiceDetail = _InvoiceDetail.FirstOrDefault();
                            oInvoiceDetailViewModel.InvoiceDetailItems = _InvoiceDetailItems;
                            oInvoiceDetailViewModel.FranchiseeBillingDetails = _InvoicFranchiseeItems;

                            var InvDetail = context.MasterTrxDetails.Where(x => x.InvoiceId == invoiceid && (x.MasterTrxTypeListId == 1 || x.MasterTrxTypeListId == 5)).FirstOrDefault();
                            oInvoiceDetailViewModel.MasterTrxTypeListId = (int)InvDetail.MasterTrxTypeListId;
                        }
                    }
                }



                ////oInvoiceDetailViewModel.InvoiceDetail = context.vw_InvoiceDetail.SingleOrDefault(o => o.InvoiceId == invoiceid);
                //oInvoiceDetailViewModel.InvoiceDetail = context.vw_InvoiceDetail.FirstOrDefault(o => o.InvoiceId == invoiceid);
                //oInvoiceDetailViewModel.InvoiceDetailItems = context.vw_InvoiceContractDetailList.Where(o => o.InvoiceId == invoiceid).ToList();

                var invBillingPay = context.BillingPays.Where(o => o.InvoiceId == invoiceid);
                oInvoiceDetailViewModel.FranchiseeBill = (invBillingPay != null ? invBillingPay.FirstOrDefault().TransactionNumber : string.Empty);
                oInvoiceDetailViewModel.BillingPayId = (invBillingPay != null ? invBillingPay.FirstOrDefault().BillingPayId : 0);


                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceid);
                    oInvoiceDetailViewModel.lstInvoiceTransactionHistory = conn.Query<InvoiceTransactionHistoryViewModel>("dbo.portal_spGet_C_InvoiceTransactionHistory", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
                }
                if (oInvoiceDetailViewModel.lstInvoiceTransactionHistory == null)
                    oInvoiceDetailViewModel.lstInvoiceTransactionHistory = new List<InvoiceTransactionHistoryViewModel>();

                var RegionsData = (oInvoiceDetailViewModel.InvoiceDetail != null ? context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId) : null);
                if (RegionsData != null)
                {
                    if (Convert.ToBoolean(RegionsData.RemitSameAsMain) == true)
                    {
                        oInvoiceDetailViewModel.InvoiceRegion = context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                    }
                    else
                    {
                        //RemitTo with Region
                        List<Region> Regionlist = new List<Region>();
                        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                        {
                            var parmas = new DynamicParameters();
                            parmas.Add("@RegionId", oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                            Regionlist = conn.Query<Region>("dbo.portal_spGet_A_RemitToWithRegion", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
                        }
                        oInvoiceDetailViewModel.InvoiceRegion = Regionlist.FirstOrDefault();
                    }
                }
                return oInvoiceDetailViewModel;
            }
        }

        public InvoiceDetailViewModel GetInvoiceDetailForPayment(int invoiceid)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                InvoiceDetailViewModel oInvoiceDetailViewModel = new InvoiceDetailViewModel();

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceid);
                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_GetInvoiceDetail", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            var _InvoiceDetail = multipleresult.Read<vw_InvoiceDetailViewModel>().ToList();
                            var _InvoiceDetailItems = multipleresult.Read<vw_InvoiceContractDetailList>().ToList();
                            var _InvoicFranchiseeItems = multipleresult.Read<InvoiceFranchiseeBillingDetailViewModel>().ToList();

                            oInvoiceDetailViewModel.InvoiceDetail = _InvoiceDetail.FirstOrDefault();
                            oInvoiceDetailViewModel.InvoiceDetailItems = _InvoiceDetailItems;
                            oInvoiceDetailViewModel.FranchiseeBillingDetails = _InvoicFranchiseeItems;

                            var InvDetail = context.MasterTrxDetails.Where(x => x.InvoiceId == invoiceid && (x.MasterTrxTypeListId == 1 || x.MasterTrxTypeListId == 5)).FirstOrDefault();
                            oInvoiceDetailViewModel.MasterTrxTypeListId = (int)InvDetail.MasterTrxTypeListId;
                        }
                    }
                }



                ////oInvoiceDetailViewModel.InvoiceDetail = context.vw_InvoiceDetail.SingleOrDefault(o => o.InvoiceId == invoiceid);
                //oInvoiceDetailViewModel.InvoiceDetail = context.vw_InvoiceDetail.FirstOrDefault(o => o.InvoiceId == invoiceid);
                //oInvoiceDetailViewModel.InvoiceDetailItems = context.vw_InvoiceContractDetailList.Where(o => o.InvoiceId == invoiceid).ToList();

                var invBillingPay = context.BillingPays.Where(o => o.InvoiceId == invoiceid);
                oInvoiceDetailViewModel.FranchiseeBill = (invBillingPay != null ? invBillingPay.FirstOrDefault().TransactionNumber : string.Empty);
                oInvoiceDetailViewModel.BillingPayId = (invBillingPay != null ? invBillingPay.FirstOrDefault().BillingPayId : 0);


                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceid);
                    oInvoiceDetailViewModel.lstInvoiceTransactionHistory = conn.Query<InvoiceTransactionHistoryViewModel>("dbo.portal_spGet_C_InvoiceTransactionHistory", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
                }
                if (oInvoiceDetailViewModel.lstInvoiceTransactionHistory == null)
                    oInvoiceDetailViewModel.lstInvoiceTransactionHistory = new List<InvoiceTransactionHistoryViewModel>();

                var RegionsData = (oInvoiceDetailViewModel.InvoiceDetail != null ? context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId) : null);
                if (RegionsData != null)
                {
                    if (Convert.ToBoolean(RegionsData.RemitSameAsMain) == true)
                    {
                        oInvoiceDetailViewModel.InvoiceRegion = context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                    }
                    else
                    {
                        //RemitTo with Region
                        List<Region> Regionlist = new List<Region>();
                        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                        {
                            var parmas = new DynamicParameters();
                            parmas.Add("@RegionId", oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                            Regionlist = conn.Query<Region>("dbo.portal_spGet_A_RemitToWithRegion", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
                        }
                        oInvoiceDetailViewModel.InvoiceRegion = Regionlist.FirstOrDefault();
                    }
                }
                return oInvoiceDetailViewModel;
            }
        }

        public ConsolidatedInvoiceDetailViewModel GetConsolidatedInvoiceDetail(int consolidatedInvoiceid)
        {
            ConsolidatedInvoiceDetailViewModel oInvoiceDetailViewModel = new ConsolidatedInvoiceDetailViewModel();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@consolidatedInvoiceid", consolidatedInvoiceid);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_GetConsolidatedInvoiceDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        var _InvoiceDetail = multipleresult.Read<ConsolidatedInvoiceDetailViewModel>().ToList();
                        var _Region = multipleresult.Read<Region>().ToList();
                        var _InvoicItems = multipleresult.Read<ConsolidatedInvoiceDetailInvoiceViewModel>().ToList();

                        oInvoiceDetailViewModel = _InvoiceDetail.FirstOrDefault();
                        oInvoiceDetailViewModel.InvoiceRegion = _Region.FirstOrDefault();
                        oInvoiceDetailViewModel.Invoices = _InvoicItems;
                    }
                }

                List<ConsolidatedInvoiceTransactionHistoryModel> ListHistModel = new List<ConsolidatedInvoiceTransactionHistoryModel>();
                if (oInvoiceDetailViewModel.Invoices != null && oInvoiceDetailViewModel.Invoices.Count() > 0)
                {
                    foreach (var item in oInvoiceDetailViewModel.Invoices)
                    {
                        ConsolidatedInvoiceTransactionHistoryModel ItemModel = new ConsolidatedInvoiceTransactionHistoryModel();
                        List<InvoiceTransactionHistoryViewModel> histItem = new List<InvoiceTransactionHistoryViewModel>();
                        var parmas1 = new DynamicParameters();
                        parmas1.Add("@InvoiceId", item.InvoiceId);
                        histItem = conn.Query<InvoiceTransactionHistoryViewModel>("dbo.portal_spGet_C_InvoiceTransactionHistory", parmas1, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();

                        ItemModel.InvoiceNo = item.InvoiceNo;
                        ItemModel.lstInvoiceTransactionHistory = histItem;
                        ListHistModel.Add(ItemModel);
                    }
                    oInvoiceDetailViewModel.ConsolidatedInvoiceHistoryListModel = ListHistModel;
                }
                if (oInvoiceDetailViewModel.ConsolidatedInvoiceHistoryListModel == null)
                    oInvoiceDetailViewModel.ConsolidatedInvoiceHistoryListModel = new List<ConsolidatedInvoiceTransactionHistoryModel>();

            }
            return oInvoiceDetailViewModel;
        }
        public bool InvoiceRevert(int invoiceid)
        {
            bool retVal = true;

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                int _otherTRX = context.MasterTrxDetails.Where(o => o.InvoiceId == invoiceid && (o.MasterTrxTypeListId == 2 || o.MasterTrxTypeListId == 3 || o.MasterTrxTypeListId == 17 || o.MasterTrxTypeListId == 24)).Count();
                if (_otherTRX > 0)
                {
                    retVal = false;
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                    {
                        var parmas = new DynamicParameters();
                        parmas.Add("@InvoiceId", invoiceid);
                        conn.Execute("dbo.portal_spGet_AR_RevertInvoice", parmas, commandType: CommandType.StoredProcedure);
                    }
                }
            }

            return retVal;
        }


        public IEnumerable<ARCustomerWithCreditListViewModel> GetCreditListWithSearch(int month, int year, int searchBy, string searchValue)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = "EXEC portal_spGet_AR_CreditSearchListByCustomerandInvoice @RegionId,@BillMonth,@BillYear,@SearchBy,@SearchValue";


                var result = conn.Query<ARCustomerWithCreditListViewModel>(query.ToString(), new
                {
                    RegionId = SelectedRegionId,
                    BillMonth = month,
                    BillYear = year,
                    SearchBy = searchBy,
                    SearchValue = searchValue
                });

                return result;
            }
        }


        public List<EmailViewModel> GetCustomerEbillEmails(int customerid)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<EmailViewModel> lstEmail = context.Emails.Where(o => o.ClassId == customerid && o.TypeListId == 1 && o.ContactTypeListId == 8).MapEnumerable<EmailViewModel, Email>().ToList(); ;

                return lstEmail;
            }
        }


        public ManualInvoiceDetailViewModel GetManualInvoiceDetail(int MasterTmpTrxId)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                ManualInvoiceDetailViewModel oManualInvoiceDetailViewModel = new ManualInvoiceDetailViewModel();

                oManualInvoiceDetailViewModel.CustomerTransactionItems = context.vw_AR_CustomerTransaction.Where(o => o.MasterTmpTrxId == MasterTmpTrxId).ToList();
                oManualInvoiceDetailViewModel.FrenchiseeTransactionItems = context.vw_AR_FrenchiseeTransaction.Where(o => o.MasterTmpTrxId == MasterTmpTrxId).ToList();

                return oManualInvoiceDetailViewModel;
            }
        }




        private InvoiceDetailViewModel GetInvoiceDetailNForCredit(int invoiceid, ref List<CreditFranchiseeDetailViewModel> lstFranchiseItems)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                InvoiceDetailViewModel oInvoiceDetailViewModel = new InvoiceDetailViewModel();


                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceid);
                    parmas.Add("@IsCredit", true);
                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_GetInvoiceDetailForCredit", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            var _InvoiceDetail = multipleresult.Read<vw_InvoiceDetailViewModel>().ToList();
                            var _InvoiceDetailItems = multipleresult.Read<vw_InvoiceContractDetailList>().ToList();
                            var _invBillingPay = multipleresult.Read<BillingPay>().ToList();
                            var _invInvoiceFranchisee = multipleresult.Read<portal_spGet_AR_InvoiceFranchiseeBalance_Result>().ToList();

                            oInvoiceDetailViewModel.InvoiceDetail = _InvoiceDetail.FirstOrDefault();
                            oInvoiceDetailViewModel.InvoiceDetailItems = _InvoiceDetailItems;
                            oInvoiceDetailViewModel.FranchiseeBill = (_invBillingPay.Count() > 0 ? _invBillingPay.FirstOrDefault().TransactionNumber : string.Empty);
                            oInvoiceDetailViewModel.BillingPayId = (_invBillingPay.Count() > 0 ? _invBillingPay.FirstOrDefault().BillingPayId : 0);


                            lstFranchiseItems = new List<CreditFranchiseeDetailViewModel>();
                            foreach (var res in _invInvoiceFranchisee)
                            {
                                CreditFranchiseeDetailViewModel cfdvm = new CreditFranchiseeDetailViewModel();
                                cfdvm.CreditFranchiseeId = -1;
                                cfdvm.CreditAmount = 0;
                                cfdvm.InvoiceFranchiseeDetailItem = res;
                                lstFranchiseItems.Add(cfdvm);
                            }
                        }
                    }
                }


                //oInvoiceDetailViewModel.InvoiceDetail = context.vw_InvoiceDetail.FirstOrDefault(o => o.InvoiceId == invoiceid);
                //oInvoiceDetailViewModel.InvoiceDetailItems = context.vw_InvoiceContractDetailList.Where(o => o.InvoiceId == invoiceid).ToList();

                //var invBillingPay = context.BillingPays.Where(o => o.InvoiceId == invoiceid);
                //oInvoiceDetailViewModel.FranchiseeBill = (invBillingPay != null ? invBillingPay.FirstOrDefault().TransactionNumber : string.Empty);
                //oInvoiceDetailViewModel.BillingPayId = (invBillingPay != null ? invBillingPay.FirstOrDefault().BillingPayId : 0);










                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceid);
                    oInvoiceDetailViewModel.lstInvoiceTransactionHistory = conn.Query<InvoiceTransactionHistoryViewModel>("dbo.portal_spGet_C_InvoiceTransactionHistory", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure)
                        .Where(o => o.MasterTrxTypeListId != 2).ToList();
                }
                if (oInvoiceDetailViewModel.lstInvoiceTransactionHistory == null)
                    oInvoiceDetailViewModel.lstInvoiceTransactionHistory = new List<InvoiceTransactionHistoryViewModel>();

                var RegionsData = (oInvoiceDetailViewModel.InvoiceDetail != null ? context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId) : null);
                if (RegionsData != null)
                {
                    if (Convert.ToBoolean(RegionsData.RemitSameAsMain) == true)
                    {
                        oInvoiceDetailViewModel.InvoiceRegion = context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                    }
                    else
                    {
                        //RemitTo with Region
                        List<Region> Regionlist = new List<Region>();
                        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                        {
                            var parmas = new DynamicParameters();
                            parmas.Add("@RegionId", oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                            Regionlist = conn.Query<Region>("dbo.portal_spGet_A_RemitToWithRegion", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
                        }
                        oInvoiceDetailViewModel.InvoiceRegion = Regionlist.FirstOrDefault();
                    }
                }
                return oInvoiceDetailViewModel;
            }
        }
        private InvoiceDetailViewModel GetInvoiceDetailNForCreditTemp(int invoiceid, ref List<CreditFranchiseeDetailViewModel> lstFranchiseItems)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                InvoiceDetailViewModel oInvoiceDetailViewModel = new InvoiceDetailViewModel();


                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceid);
                    parmas.Add("@IsCredit", true);
                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_GetInvoiceDetailForCreditTemp", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            var _InvoiceDetail = multipleresult.Read<vw_InvoiceDetailViewModel>().ToList();
                            var _InvoiceDetailItems = multipleresult.Read<vw_InvoiceContractDetailList>().ToList();
                            var _invBillingPay = multipleresult.Read<BillingPay>().ToList();
                            var _invInvoiceFranchisee = multipleresult.Read<portal_spGet_AR_InvoiceFranchiseeBalance_Result>().ToList();

                            oInvoiceDetailViewModel.InvoiceDetail = _InvoiceDetail.FirstOrDefault();
                            oInvoiceDetailViewModel.InvoiceDetailItems = _InvoiceDetailItems;
                            oInvoiceDetailViewModel.FranchiseeBill = (_invBillingPay.Count() > 0 ? _invBillingPay.FirstOrDefault().TransactionNumber : string.Empty);
                            oInvoiceDetailViewModel.BillingPayId = (_invBillingPay.Count() > 0 ? _invBillingPay.FirstOrDefault().BillingPayId : 0);


                            lstFranchiseItems = new List<CreditFranchiseeDetailViewModel>();
                            foreach (var res in _invInvoiceFranchisee)
                            {
                                CreditFranchiseeDetailViewModel cfdvm = new CreditFranchiseeDetailViewModel();
                                cfdvm.CreditFranchiseeId = -1;
                                cfdvm.CreditAmount = 0;
                                cfdvm.InvoiceFranchiseeDetailItem = res;
                                lstFranchiseItems.Add(cfdvm);
                            }
                        }
                    }
                }


                oInvoiceDetailViewModel.lstInvoiceTransactionHistory = new List<InvoiceTransactionHistoryViewModel>();

                var RegionsData = (oInvoiceDetailViewModel.InvoiceDetail != null ? context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId) : null);
                if (RegionsData != null)
                {
                    if (Convert.ToBoolean(RegionsData.RemitSameAsMain) == true)
                    {
                        oInvoiceDetailViewModel.InvoiceRegion = context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                    }
                    else
                    {
                        //RemitTo with Region
                        List<Region> Regionlist = new List<Region>();
                        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                        {
                            var parmas = new DynamicParameters();
                            parmas.Add("@RegionId", oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                            Regionlist = conn.Query<Region>("dbo.portal_spGet_A_RemitToWithRegion", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
                        }
                        oInvoiceDetailViewModel.InvoiceRegion = Regionlist.FirstOrDefault();
                    }
                }
                return oInvoiceDetailViewModel;
            }
        }



        public CreditDetailViewModel GetCreditDetailForInvoiceN(int invoiceId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CreditDetailViewModel viewModel = new CreditDetailViewModel();
                List<CreditFranchiseeDetailViewModel> lstFranchiseItems = new List<CreditFranchiseeDetailViewModel>();
                viewModel.CreditId = -1;
                viewModel.Invoice = GetInvoiceDetailNForCredit(invoiceId, ref lstFranchiseItems);

                viewModel.InvoiceAmount = Convert.ToDecimal(viewModel.Invoice.InvoiceDetailItems.Sum(s => s.Total));
                viewModel.InvoiceBalance = Convert.ToDecimal(viewModel.Invoice.InvoiceDetailItems.Sum(s => s.Balance));
                viewModel.InvoiceOpenCRBalance = viewModel.InvoiceBalance;
                viewModel.FranchiseeItems = lstFranchiseItems;
                return viewModel;
            }
        }


        public CreditDetailViewModel GetTaxCreditDetailForInvoice(int invoiceId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CreditDetailViewModel viewModel = new CreditDetailViewModel();

                InvoiceDetailViewModel oInvoiceDetailViewModel = new InvoiceDetailViewModel();

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceId);
                    parmas.Add("@IsCredit", true);
                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_GetInvoiceDetailForTaxCredit", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            var _InvoiceDetail = multipleresult.Read<vw_InvoiceDetailViewModel>().ToList();
                            var _InvoiceDetailItems = multipleresult.Read<vw_InvoiceContractDetailList>().ToList();

                            oInvoiceDetailViewModel.InvoiceDetail = _InvoiceDetail.FirstOrDefault();
                            oInvoiceDetailViewModel.InvoiceDetailItems = _InvoiceDetailItems;

                        }
                    }
                }

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@InvoiceId", invoiceId);
                    oInvoiceDetailViewModel.lstInvoiceTransactionHistory = conn.Query<InvoiceTransactionHistoryViewModel>("dbo.portal_spGet_C_InvoiceTransactionHistory", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure)
                        .Where(o => o.MasterTrxTypeListId != 2).ToList();
                }

                if (oInvoiceDetailViewModel.lstInvoiceTransactionHistory == null)
                    oInvoiceDetailViewModel.lstInvoiceTransactionHistory = new List<InvoiceTransactionHistoryViewModel>();

                var RegionsData = (oInvoiceDetailViewModel.InvoiceDetail != null ? context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId) : null);
                if (RegionsData != null)
                {
                    if (Convert.ToBoolean(RegionsData.RemitSameAsMain) == true)
                    {
                        oInvoiceDetailViewModel.InvoiceRegion = context.Regions.SingleOrDefault(o => o.RegionId == oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                    }
                    else
                    {
                        //RemitTo with Region
                        List<Region> Regionlist = new List<Region>();
                        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                        {
                            var parmas = new DynamicParameters();
                            parmas.Add("@RegionId", oInvoiceDetailViewModel.InvoiceDetail.RegionId);
                            Regionlist = conn.Query<Region>("dbo.portal_spGet_A_RemitToWithRegion", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
                        }
                        oInvoiceDetailViewModel.InvoiceRegion = Regionlist.FirstOrDefault();
                    }
                }

                viewModel.CreditId = -1;
                viewModel.Invoice = oInvoiceDetailViewModel;

                viewModel.InvoiceAmount = Convert.ToDecimal(viewModel.Invoice.InvoiceDetailItems.Sum(s => s.Total));
                viewModel.InvoiceBalance = Convert.ToDecimal(viewModel.Invoice.InvoiceDetailItems.Sum(s => s.Balance));

                viewModel.InvoiceOpenCRBalance = viewModel.InvoiceBalance;

                return viewModel;
            }
        }
        public bool InsertTaxCreditDetailForInvoiceTransaction(int _invoiceId, decimal _ApplyTaxAmount, DateTime _TrxDate, string TrxDesc)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Invoice _invoiceOject = context.Invoices.Single(o => o.InvoiceId == _invoiceId);
                var PR = context.Periods.SingleOrDefault(p => p.BillMonth == _TrxDate.Month && p.BillYear == _TrxDate.Year);


                List<MasterTrxTax> lstMasterTrxTax = context.MasterTrxTaxes.Where(d => d.InvoiceId == _invoiceId).ToList();


                MasterTrxDetail oMasterTrxDetail = new MasterTrxDetail();

                oMasterTrxDetail.AccountRebate = 0;
                oMasterTrxDetail.BPPAdmin = 0;
                oMasterTrxDetail.ClientSupplies = false;
                oMasterTrxDetail.Commission = false;
                oMasterTrxDetail.CommissionTotal = 0;
                oMasterTrxDetail.CPIPercentage = 0;
                oMasterTrxDetail.DistributionAmount = 0;
                oMasterTrxDetail.ExtendedPrice = 0;
                oMasterTrxDetail.ExtraWork = 0;
                oMasterTrxDetail.FeesDetail = false;
                oMasterTrxDetail.IsDelete = false;
                oMasterTrxDetail.FRDeduction = false;
                oMasterTrxDetail.FRRevenues = false;
                oMasterTrxDetail.LineNo = -1;
                oMasterTrxDetail.MasterTrxId = 0;
                oMasterTrxDetail.Quantity = 0;
                oMasterTrxDetail.ReSell = false;
                oMasterTrxDetail.SourceId = 0;
                oMasterTrxDetail.TotalFee = 0;
                oMasterTrxDetail.UnitPrice = 0;
                oMasterTrxDetail.AmountTypeListId = 1;
                oMasterTrxDetail.CreatedBy = LoginUserId;
                oMasterTrxDetail.CreatedDate = DateTime.Now;
                oMasterTrxDetail.DetailDescription = TrxDesc;
                //oMasterTrxDetail.HeaderId;                
                oMasterTrxDetail.InvoiceId = _invoiceId;
                oMasterTrxDetail.MasterTrxTypeListId = 59;
                oMasterTrxDetail.ServiceTypeListId = 57;
                oMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                oMasterTrxDetail.RegionId = _invoiceOject.RegionId;
                oMasterTrxDetail.TaxDetail = true;
                oMasterTrxDetail.Total = _ApplyTaxAmount;
                oMasterTrxDetail.TotalTax = Math.Round(_ApplyTaxAmount,2);
                oMasterTrxDetail.Transactiondate = _TrxDate;
                oMasterTrxDetail.TransactionStatusListId = 3;
                oMasterTrxDetail.ClassId = _invoiceOject.ClassId;
                oMasterTrxDetail.TypelistId = 1;


                context.MasterTrxDetails.Add(oMasterTrxDetail);
                context.SaveChanges();

                decimal _ApplyTaxAmountT = _ApplyTaxAmount;
                decimal _ApplyTaxAmountTAMT = (decimal)lstMasterTrxTax.Sum(k => k.Amount);


                if (lstMasterTrxTax.Count > 0)
                {
                    foreach (var item in lstMasterTrxTax)
                    {
                        var prs = (decimal)(item.Amount / _ApplyTaxAmountTAMT) * 100;

                        MasterTrxTax oMasterTrxTax = new MasterTrxTax();
                        oMasterTrxTax.Amount = Math.Round(_ApplyTaxAmount * prs / 100);
                        oMasterTrxTax.AmountTypeListId = 1;
                        //oMasterTrxTax.County;
                        //oMasterTrxTax.CountyTaxCode;
                        oMasterTrxTax.CreatedBy = LoginUserId;
                        oMasterTrxTax.CreatedDate = DateTime.Now;
                        oMasterTrxTax.CustomerId = _invoiceOject.ClassId;
                        oMasterTrxTax.FranchiseeId = item.FranchiseeId;
                        oMasterTrxTax.FRDeduction = false;
                        oMasterTrxTax.FRRevenues = true;
                        oMasterTrxTax.InvoiceId = _invoiceId;
                        oMasterTrxTax.IsDelete = false;
                        oMasterTrxTax.MasterTrxDetailId = oMasterTrxDetail.MasterTrxDetailId;
                        oMasterTrxTax.PeriodId = (PR != null ? PR.PeriodId : 0);
                        oMasterTrxTax.RegionId = _invoiceOject.RegionId;
                        //oMasterTrxTax.TaxRateId;
                        //oMasterTrxTax.TaxratePercentage;
                        context.MasterTrxTaxes.Add(oMasterTrxTax);
                        context.SaveChanges();
                    }
                }
                else
                {
                    MasterTrxTax oMasterTrxTax = new MasterTrxTax();
                    oMasterTrxTax.Amount = _ApplyTaxAmount;
                    oMasterTrxTax.AmountTypeListId = 1;
                    //oMasterTrxTax.County;
                    //oMasterTrxTax.CountyTaxCode;
                    oMasterTrxTax.CreatedBy = LoginUserId;
                    oMasterTrxTax.CreatedDate = DateTime.Now;
                    oMasterTrxTax.CustomerId = _invoiceOject.ClassId;
                    //oMasterTrxTax.FranchiseeId = item.FranchiseeId;
                    oMasterTrxTax.FRDeduction = false;
                    oMasterTrxTax.FRRevenues = true;
                    oMasterTrxTax.InvoiceId = _invoiceId;
                    oMasterTrxTax.IsDelete = false;
                    oMasterTrxTax.MasterTrxDetailId = oMasterTrxDetail.MasterTrxDetailId;
                    oMasterTrxTax.PeriodId = (PR != null ? PR.PeriodId : 0);
                    oMasterTrxTax.RegionId = _invoiceOject.RegionId;
                    //oMasterTrxTax.TaxRateId;
                    //oMasterTrxTax.TaxratePercentage;
                    context.MasterTrxTaxes.Add(oMasterTrxTax);
                    context.SaveChanges();
                }

                return true;
            }
        }

        public CreditDetailViewModel GetCreditDetailForCreditN(int creditId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Credit credit = context.Credits.Where(o => o.CreditId == creditId).FirstOrDefault();
                if (credit == null)
                    return null;

                MasterTrx mt = context.MasterTrxes.Where(o => o.MasterTrxId == credit.MasterTrxId).FirstOrDefault();
                List<CreditFranchisee> creditFranchisees = context.CreditFranchisees.Where(o => o.CreditId == creditId).ToList();

                CreditDetailViewModel viewModel = new CreditDetailViewModel();
                List<CreditFranchiseeDetailViewModel> lstFranchiseItems = new List<CreditFranchiseeDetailViewModel>();
                viewModel.Credit = credit;
                viewModel.CreditId = credit.CreditId;
                viewModel.Invoice = GetInvoiceDetailNForCredit((int)credit.InvoiceId, ref lstFranchiseItems);

                List<MasterTrxDetail> mtds = context.MasterTrxDetails.Where(o => o.MasterTrxId == credit.MasterTrxId).OrderBy(o => o.LineNo).ToList();

                if (mtds.Count() > 0)
                    viewModel.UnappliedCreditAmount = (decimal)mtds.Sum(o => o.Total);
                //List<MasterTrxDetail> mtds = context.MasterTrxDetails.Where(o => o.MasterTrxId == credit.MasterTrxId && o.InvoiceId != null && o.LineNo != null).OrderBy(o => o.LineNo).ToList();
                viewModel.CreditAmounts = new List<decimal>();
                for (int i = 0; i < viewModel.Invoice.InvoiceDetailItems.Max(o => o.LineNumber); i++)
                    viewModel.CreditAmounts.Add(mtds.Where(o => o.LineNo == i + 1).FirstOrDefault()?.Total ?? 0);

                viewModel.InvoiceAmount = Convert.ToDecimal(viewModel.Invoice.InvoiceDetailItems.Sum(s => s.Total));
                viewModel.InvoiceBalance = Convert.ToDecimal(viewModel.Invoice.InvoiceDetailItems.Sum(s => s.Balance));
                viewModel.InvoiceOpenCRBalance = viewModel.InvoiceBalance;
                viewModel.FranchiseeItems = lstFranchiseItems;
                foreach (var franchiseeItem in viewModel.FranchiseeItems)
                {
                    var cf = creditFranchisees.Where(o => o.FranchiseeId == franchiseeItem.InvoiceFranchiseeDetailItem.FranchiseeId).FirstOrDefault();

                    var cfmtd = context.MasterTrxDetails.Where(o => o.MasterTrxId == cf.MasterTrxId).FirstOrDefault();

                    franchiseeItem.CreditFranchiseeId = cf.CreditFranchiseeId;
                    if (cfmtd != null)
                        franchiseeItem.CreditAmount = (decimal)cfmtd.ExtendedPrice;
                }
                return viewModel;
            }
        }

        public CreditDetailViewModel GetCreditDetailForTaxCreditTempN(int creditId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //CreditDetailViewModel viewModel = new CreditDetailViewModel();
                CreditTemp creditTemp = context.CreditTemps.SingleOrDefault(c => c.CreditTempId == creditId);
                if (creditTemp == null)
                    return null;
                int invoiceId = creditTemp.InvoiceId == null ? 0 : (int)creditTemp.InvoiceId;
                CreditDetailViewModel viewModel = GetTaxCreditDetailForInvoice(invoiceId);
                viewModel.CreditId = creditTemp.CreditTempId;
                decimal? pendingAmount = context.CreditTempDetails.Where(p => p.CreditTempId == creditTemp.CreditTempId).Sum(p => p.TotalTax);
                viewModel.UnappliedCreditAmount = pendingAmount == null ? 0 : (decimal)pendingAmount;
                Credit oCredit = new Credit();
                oCredit.ClassId = creditTemp.CustomerId;
                oCredit.CreatedBy = creditTemp.CreatedBy;
                oCredit.CreatedDate = creditTemp.CreatedDate;
                oCredit.CreditDescription = creditTemp.CreditDescription;
                oCredit.CreditId = creditTemp.CreditTempId;
                oCredit.CreditReasonListId = creditTemp.CreditReasonListId;
                oCredit.InvoiceId = creditTemp.InvoiceId;
                oCredit.IsDelete = false;
                oCredit.MasterTrxId = -1;
                oCredit.PeriodId = creditTemp.PeriodId;
                oCredit.RegionId = creditTemp.RegionId;
                oCredit.TransactionDate = creditTemp.TransactionDate;
                oCredit.TransactionNumber = creditTemp.TransactionNumber;
                oCredit.TransactionStatusListId = creditTemp.TransactionStatusListId;
                oCredit.TypeListId = 1;



                viewModel.Credit = oCredit;
                viewModel.CreditId = oCredit.CreditId;
                return viewModel;
            }
        }

        public CreditDetailViewModel GetCreditDetailForCreditTempN(int creditId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                CreditTemp creditTemp = context.CreditTemps.SingleOrDefault(c => c.CreditTempId == creditId);
              
                if (creditTemp == null)
                    return null;


                MasterTrx oMasterTrx = new MasterTrx();
                oMasterTrx.AccountTypeListId = creditTemp.AccountTypeListId;
                //oMasterTrx.BatchId;
                oMasterTrx.BillMonth = creditTemp.TransactionDate.Value.Month;
                oMasterTrx.BillYear = creditTemp.TransactionDate.Value.Year;
                oMasterTrx.ClassId = creditTemp.CustomerId;
                oMasterTrx.CreatedBy = creditTemp.CreatedBy;
                oMasterTrx.CreatedDate = creditTemp.CreatedDate;
                oMasterTrx.HeaderId = creditTemp.CreditTempId;
                //oMasterTrx.Invoices;
                oMasterTrx.MasterTrxId = -1;
                oMasterTrx.MasterTrxTypeListId = creditTemp.MasterTrxTypeListId;
                oMasterTrx.PeriodId = creditTemp.PeriodId;
                oMasterTrx.RegionId = creditTemp.RegionId;
                oMasterTrx.StatusId = creditTemp.TransactionStatusListId;
                oMasterTrx.TrxDate = creditTemp.TransactionDate;
                oMasterTrx.TypeListId = 1;

                //MasterTrx mt = context.MasterTrxes.Where(o => o.MasterTrxId == credit.MasterTrxId).FirstOrDefault();



                List<CreditTempDetail> lstCreditTempDetailFR = context.CreditTempDetails.Where(r => r.CreditTempId == creditTemp.CreditTempId && r.MasterTrxTypeListId == 8).ToList();

                List<CreditFranchisee> creditFranchisees = new List<CreditFranchisee>();
                CreditFranchisee oCreditFranchisees;
                foreach (var item in lstCreditTempDetailFR)
                {
                    oCreditFranchisees = new CreditFranchisee();
                    oCreditFranchisees.BillingPayId = item.FC_BillingPayId;
                    oCreditFranchisees.CreatedBy = creditTemp.CreatedBy;
                    oCreditFranchisees.CreatedDate = creditTemp.CreatedDate;
                    oCreditFranchisees.CreditFranchiseeDescription = creditTemp.CreditDescription;
                    oCreditFranchisees.CreditFranchiseeId = item.CreditTempDetailId;
                    oCreditFranchisees.CreditId = creditTemp.CreditTempId;
                    oCreditFranchisees.FranchiseeId = item.ClassId;
                    oCreditFranchisees.isActive = true;
                    oCreditFranchisees.MasterTrxId = -1;
                    oCreditFranchisees.PeriodId = creditTemp.PeriodId;
                    oCreditFranchisees.RegionId = creditTemp.RegionId;
                    oCreditFranchisees.TransactionDate = creditTemp.TransactionDate;
                    oCreditFranchisees.TransactionNumber = "";
                    oCreditFranchisees.TransactionStatusListId = creditTemp.TransactionStatusListId;

                    creditFranchisees.Add(oCreditFranchisees);
                }



                CreditDetailViewModel viewModel = new CreditDetailViewModel();
                List<CreditFranchiseeDetailViewModel> lstFranchiseItems = new List<CreditFranchiseeDetailViewModel>();

                Credit oCredit = new Credit();
                oCredit.ClassId = creditTemp.CustomerId;
                oCredit.CreatedBy = creditTemp.CreatedBy;
                oCredit.CreatedDate = creditTemp.CreatedDate;
                oCredit.CreditDescription = creditTemp.CreditDescription;
                oCredit.CreditId = creditTemp.CreditTempId;
                oCredit.CreditReasonListId = creditTemp.CreditReasonListId;
                oCredit.InvoiceId = creditTemp.InvoiceId;
                oCredit.IsDelete = false;
                oCredit.MasterTrxId = -1;
                oCredit.PeriodId = creditTemp.PeriodId;
                oCredit.RegionId = creditTemp.RegionId;
                oCredit.TransactionDate = creditTemp.TransactionDate;
                oCredit.TransactionNumber = creditTemp.TransactionNumber;
                oCredit.TransactionStatusListId = creditTemp.TransactionStatusListId;
                oCredit.TypeListId = 1;



                viewModel.Credit = oCredit;
                viewModel.CreditId = oCredit.CreditId;

                viewModel.Invoice = GetInvoiceDetailNForCreditTemp((int)oCredit.InvoiceId, ref lstFranchiseItems);



                //List<MasterTrxDetail> mtds = context.MasterTrxDetails.Where(o => o.MasterTrxId == credit.MasterTrxId).OrderBy(o => o.LineNo).ToList();


                List<CreditTempDetail> lstCreditTempDetailCUS = context.CreditTempDetails.Where(r => r.CreditTempId == creditTemp.CreditTempId && r.MasterTrxTypeListId == 3).ToList();
                List<MasterTrxDetail> lstMasterTrxDetail = new List<MasterTrxDetail>();
                MasterTrxDetail oMasterTrxDetail;
                foreach (var item in lstCreditTempDetailCUS)
                {
                    oMasterTrxDetail = new MasterTrxDetail();
                    oMasterTrxDetail.MasterTrxDetailId = item.CreditTempDetailId;
                    oMasterTrxDetail.MasterTrxId = -1;
                    oMasterTrxDetail.MasterTrxTypeListId = item.MasterTrxTypeListId;
                    oMasterTrxDetail.ServiceTypeListId = item.ServiceTypeListId;
                    oMasterTrxDetail.DetailDescription = item.DetailDescription;
                    oMasterTrxDetail.LineNo = item.LineNo;
                    //oMasterTrxDetail.UnitPrice;
                    //oMasterTrxDetail.Quantity;
                    oMasterTrxDetail.ExtendedPrice = item.ExtendedPrice;
                    oMasterTrxDetail.TaxDetail = item.TaxDetail;
                    oMasterTrxDetail.TotalTax = Math.Round((decimal)item.TotalTax,2);
                    oMasterTrxDetail.AmountTypeListId = item.AmountTypeListId;
                    oMasterTrxDetail.Total = item.Total;
                    oMasterTrxDetail.InvoiceId = creditTemp.InvoiceId;

                    oMasterTrxDetail.HeaderId = creditTemp.CreditTempId;
                    oMasterTrxDetail.CPIPercentage = 0;
                    oMasterTrxDetail.CreatedBy = creditTemp.CreatedBy;
                    oMasterTrxDetail.CreatedDate = creditTemp.CreatedDate;
                    oMasterTrxDetail.PeriodId = creditTemp.PeriodId;
                    oMasterTrxDetail.RegionId = creditTemp.RegionId;
                    oMasterTrxDetail.FeesDetail = false;
                    oMasterTrxDetail.TotalFee = 0;
                    oMasterTrxDetail.AccountRebate = 0;
                    oMasterTrxDetail.BPPAdmin = 0;
                    oMasterTrxDetail.ClientSupplies = false;
                    oMasterTrxDetail.Commission = false;
                    oMasterTrxDetail.CommissionTotal = 0;
                    oMasterTrxDetail.ExtraWork = 0;
                    oMasterTrxDetail.FRDeduction = false;
                    oMasterTrxDetail.FRRevenues = false;
                    oMasterTrxDetail.ReSell = false;
                    oMasterTrxDetail.Transactiondate = creditTemp.TransactionDate;
                    oMasterTrxDetail.TransactionStatusListId = creditTemp.TransactionStatusListId;
                    oMasterTrxDetail.ClassId = creditTemp.CustomerId;
                    oMasterTrxDetail.TypelistId = 1;
                    oMasterTrxDetail.IsDelete = false;
                    //oMasterTrxDetail.ImpAppFromInvId;
                    //oMasterTrxDetail.DistributionAmount;
                    //oMasterTrxDetail.MCustomerId;
                    //oMasterTrxDetail.MFranchiseeId;
                    //oMasterTrxDetail.ModifiedBy;
                    //oMasterTrxDetail.ModifiedDate;
                    //oMasterTrxDetail.PayFranchisee;
                    //oMasterTrxDetail.PrimaryPaymentId;
                    oMasterTrxDetail.SourceId = item.SourceId;
                    oMasterTrxDetail.SourceTypeListId = item.SourceTypeListId;
                    //oMasterTrxDetail.TurnaroundPaymentStatusListId;
                    lstMasterTrxDetail.Add(oMasterTrxDetail);

                }


                if (lstCreditTempDetailCUS.Count() > 0)
                    viewModel.UnappliedCreditAmount = (decimal)lstCreditTempDetailCUS.Sum(o => o.Total);
                //List<MasterTrxDetail> mtds = context.MasterTrxDetails.Where(o => o.MasterTrxId == credit.MasterTrxId && o.InvoiceId != null && o.LineNo != null).OrderBy(o => o.LineNo).ToList();
                viewModel.CreditAmounts = new List<decimal>();
                for (int i = 0; i < viewModel.Invoice.InvoiceDetailItems.Max(o => o.LineNumber); i++)
                    viewModel.CreditAmounts.Add(lstCreditTempDetailCUS.Where(o => o.LineNo == i + 1).FirstOrDefault()?.Total ?? 0);

                viewModel.InvoiceAmount = Convert.ToDecimal(viewModel.Invoice.InvoiceDetailItems.Sum(s => s.Total));
                viewModel.InvoiceBalance = Convert.ToDecimal(viewModel.Invoice.InvoiceDetailItems.Sum(s => s.Balance));
                viewModel.InvoiceOpenCRBalance = viewModel.InvoiceBalance;
                viewModel.FranchiseeItems = lstFranchiseItems;
                foreach (var franchiseeItem in viewModel.FranchiseeItems)
                {
                    var cf = creditFranchisees.Where(o => o.FranchiseeId == franchiseeItem.InvoiceFranchiseeDetailItem.FranchiseeId).FirstOrDefault();

                    //var cfmtd = lstCreditTempDetailFR.Where(o => o.ClassId == cf.MasterTrxId).ToList();
                    var cfmtd = lstCreditTempDetailFR.Where(o => o.CreditTempDetailId == cf.CreditFranchiseeId).ToList();

                    franchiseeItem.CreditFranchiseeId = cf.CreditFranchiseeId;
                    if (cfmtd.Count > 0)
                        franchiseeItem.CreditAmount = (decimal)cfmtd.Sum(c => c.ExtendedPrice);
                }
                return viewModel;
            }
        }

        public CreditDetailViewModel GetCreditDetailForInvoicePayment(int invoiceId)
        {
            CreditDetailViewModel viewModel = new CreditDetailViewModel();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@InvoiceId", invoiceId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_InvoiceBalanceForPayment", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        viewModel.CreditId = -1;

                        viewModel.Invoice = GetInvoiceDetail(invoiceId);


                        portal_spGet_AR_InvoiceBalance_Result balanceResult = multipleresult.Read<portal_spGet_AR_InvoiceBalance_Result>().ToList().FirstOrDefault();
                        viewModel.InvoiceAmount = Convert.ToDecimal(balanceResult.Amount);
                        viewModel.InvoiceBalance = Convert.ToDecimal(balanceResult.Balance);
                        viewModel.FranchiseeItems = new List<CreditFranchiseeDetailViewModel>();

                        var lst = multipleresult.Read<portal_spGet_AR_InvoiceFranchiseeBalance_Result>().ToList();
                        foreach (var res in lst)
                        {
                            CreditFranchiseeDetailViewModel cfdvm = new CreditFranchiseeDetailViewModel();
                            cfdvm.CreditFranchiseeId = -1;
                            cfdvm.CreditAmount = 0;
                            cfdvm.InvoiceFranchiseeDetailItem = res;

                            viewModel.FranchiseeItems.Add(cfdvm);
                        }
                    }
                }

            }
            return viewModel;


        }

        public CreditDetailViewModel GetCreditDetailForInvoice(int invoiceId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                CreditDetailViewModel viewModel = new CreditDetailViewModel();

                viewModel.CreditId = -1;

                viewModel.Invoice = GetInvoiceDetail(invoiceId);

                portal_spGet_AR_InvoiceBalance_Result balanceResult = context.portal_spGet_AR_InvoiceBalance(invoiceId).FirstOrDefault();
                viewModel.InvoiceAmount = Convert.ToDecimal(balanceResult.Amount);
                viewModel.InvoiceBalance = Convert.ToDecimal(balanceResult.Balance);


                decimal crTotalAMT = (decimal)(from mtd in context.MasterTrxDetails
                                               join mt in context.MasterTrxes on mtd.MasterTrxId equals mt.MasterTrxId
                                               where mtd.InvoiceId == invoiceId && mt.MasterTrxTypeListId == 3 && mt.StatusId != 1 && mt.StatusId != 2 && mt.StatusId != 12 && mt.StatusId != 13
                                               select mtd).ToList().Sum(j => j.Total);


                viewModel.InvoiceOpenCRBalance = Convert.ToDecimal(balanceResult.Amount) - crTotalAMT;

                viewModel.FranchiseeItems = new List<CreditFranchiseeDetailViewModel>();

                var lst = context.portal_spGet_AR_InvoiceFranchiseeBalance(invoiceId).ToList();
                foreach (var res in lst)
                {
                    CreditFranchiseeDetailViewModel cfdvm = new CreditFranchiseeDetailViewModel();
                    cfdvm.CreditFranchiseeId = -1;
                    cfdvm.CreditAmount = 0;
                    cfdvm.InvoiceFranchiseeDetailItem = res;

                    viewModel.FranchiseeItems.Add(cfdvm);
                }

                return viewModel;
            }
        }

        public CreditDetailViewModel GetCreditDetailForCredit(int creditId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                Credit credit = context.Credits.Where(o => o.CreditId == creditId).FirstOrDefault();
                if (credit == null)
                    return null;

                MasterTrx mt = context.MasterTrxes.Where(o => o.MasterTrxId == credit.MasterTrxId).FirstOrDefault();

                List<CreditFranchisee> creditFranchisees = context.CreditFranchisees.Where(o => o.CreditId == creditId).ToList();

                CreditDetailViewModel viewModel = GetCreditDetailForInvoice((int)credit.InvoiceId);
                viewModel.Credit = credit;
                viewModel.CreditId = credit.CreditId;
                if (mt?.BillMonth != null) viewModel.CreditBillMonth = (int)mt?.BillMonth;
                if (mt?.BillYear != null) viewModel.CreditBillYear = (int)mt?.BillYear;




                MasterTrxDetail unappliedMTD = context.MasterTrxDetails.Where(o => o.MasterTrxId == credit.MasterTrxId && o.InvoiceId == null && o.LineNo == null).FirstOrDefault();
                if (unappliedMTD != null)
                    viewModel.UnappliedCreditAmount = (decimal)unappliedMTD.Total;

                List<MasterTrxDetail> mtds = context.MasterTrxDetails.Where(o => o.MasterTrxId == credit.MasterTrxId && o.InvoiceId != null && o.LineNo != null).OrderBy(o => o.LineNo).ToList();
                viewModel.CreditAmounts = new List<decimal>();
                for (int i = 0; i < viewModel.Invoice.InvoiceDetailItems.Max(o => o.LineNumber); i++)
                    viewModel.CreditAmounts.Add(mtds.Where(o => o.LineNo == i + 1).FirstOrDefault()?.Total ?? 0);

                foreach (var franchiseeItem in viewModel.FranchiseeItems)
                {
                    var cf = creditFranchisees.Where(o => o.FranchiseeId == franchiseeItem.InvoiceFranchiseeDetailItem.FranchiseeId).FirstOrDefault();

                    var cfmtd = context.MasterTrxDetails.Where(o => o.MasterTrxId == cf.MasterTrxId).FirstOrDefault();

                    franchiseeItem.CreditFranchiseeId = cf.CreditFranchiseeId;
                    if (cfmtd != null)
                        franchiseeItem.CreditAmount = (decimal)cfmtd.ExtendedPrice;
                }
                return viewModel;
            }
        }

        public List<AgingViewModel> getAgingList(string franchiseid, string searchby, string searchvalue, string agingdate, string paymentdate, string months, string orderby, string include, string balance, string nonchargebackonly)
        {
            string spName = "spSearch_AR_AgingInvoicesTmp";
            List<AgingViewModel> agingList = new List<AgingViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@searchBy", Convert.ToInt32(searchby));
                cmd.Parameters.AddWithValue("@searchvalue", searchvalue);
                cmd.Parameters.AddWithValue("@agingdate", agingdate);
                cmd.Parameters.AddWithValue("@paymentdate", paymentdate);
                cmd.Parameters.AddWithValue("@months", Convert.ToInt32(months));
                cmd.Parameters.AddWithValue("@orderby", Convert.ToInt32(orderby));
                cmd.Parameters.AddWithValue("@include", Convert.ToInt32(include));
                cmd.Parameters.AddWithValue("@balance", Convert.ToDecimal(balance));
                cmd.Parameters.AddWithValue("@nonchargebackonly", Convert.ToInt32(nonchargebackonly));

                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {

                        AgingViewModel ag = new AgingViewModel();
                        ag.id = reader["invoiceid"].ToString();
                        ag.franchiseId = reader["franchiseid"].ToString();
                        ag.customerName = reader["Customer Name"].ToString();
                        ag.customerId = reader["customerid"].ToString();
                        ag.phone = reader["Phone"].ToString();
                        ag.franchiseName = reader["Franchise Name"].ToString();
                        ag.franchiseNo = reader["Franchise No"].ToString();
                        ag.customerNo = reader["Customer No"].ToString();
                        ag.invDate = Convert.ToDateTime(reader["Invoice Date"].ToString()).ToShortDateString();
                        ag.totalAmount = reader["Invoice Total"].ToString();
                        ag.invNumber = reader["Invoice No"].ToString();
                        ag.dueDate = Convert.ToDateTime(reader["Due Date"].ToString()).ToShortDateString();
                        ag.onemo = reader["Current"].ToString() == "" ? "0.00" : Convert.ToDecimal(reader["Current"]).ToString();
                        ag.twomo = reader["1-30"].ToString() == "" ? "0.00" : Convert.ToDecimal(reader["1-30"]).ToString();
                        ag.threemo = reader["31-60"].ToString() == "" ? "0.00" : Convert.ToDecimal(reader["31-60"]).ToString();
                        ag.fourmo = reader["61-90"].ToString() == "" ? "0.00" : Convert.ToDecimal(reader["61-90"]).ToString();
                        ag.fivemo = reader["91+"].ToString() == "" ? "0.00" : Convert.ToDecimal(reader["91+"]).ToString();

                        agingList.Add(ag);


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    reader.Close();
                }


                return agingList;
            }
        }

        public List<ARLogViewModel> getARLogList(string dateType, string startDate, string endDate)
        {
            string spName = "spGet_AR_Log";
            List<ARLogViewModel> arLogList = new List<ARLogViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                string date_format = "yyyy-MM-dd HH:mm:ss.fff";
                DateTime start_date = DateTime.Parse(startDate);// DateTime.ParseExact(startDate, date_format, null);
                DateTime end_date = DateTime.Parse(endDate);// DateTime.ParseExact(endDate, date_format, null);

                cmd.Parameters.AddWithValue("@datetype", Convert.ToInt32(dateType));
                cmd.Parameters.AddWithValue("@startdate", start_date.Date.ToString(date_format));
                cmd.Parameters.AddWithValue("@enddate", end_date.Date.ToString(date_format));

                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {

                        ARLogViewModel ag = new ARLogViewModel();
                        ag.customerNo = reader["number"].ToString();
                        ag.customerName = reader["name"].ToString();
                        ag.invoiceNo = reader["invoiceno"].ToString();
                        ag.referenceNo = reader["referenceno"].ToString();
                        ag.date = Convert.ToDateTime(reader["transdate"].ToString()).ToShortDateString();
                        ag.total = Convert.ToDecimal(reader["total"].ToString());

                        arLogList.Add(ag);


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    reader.Close();
                }


                return arLogList;
            }
        }

        public List<InvoiceSearchViewModel> getSearchInvoiceList(string searchBy, string searchValue, string emailOnly, string printOnly)
        {
            string spName = "spGet_AR_Log";
            List<InvoiceSearchViewModel> arLogList = new List<InvoiceSearchViewModel>();
            using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                //string date_format = "yyyy-MM-dd HH:mm:ss.fff";
                //DateTime start_date = DateTime.Parse(startDate);// DateTime.ParseExact(startDate, date_format, null);
                //DateTime end_date = DateTime.Parse(endDate);// DateTime.ParseExact(endDate, date_format, null);

                //cmd.Parameters.AddWithValue("@datetype", Convert.ToInt32(dateType));
                //cmd.Parameters.AddWithValue("@startdate", start_date.Date.ToString(date_format));
                //cmd.Parameters.AddWithValue("@enddate", end_date.Date.ToString(date_format));

                //SqlDataReader reader = cmd.ExecuteReader();
                //try
                //{
                //    while (reader.Read())
                //    {

                //        InvoiceSearchViewModel ag = new InvoiceSearchViewModel();
                //        ag.customerNo = reader["number"].ToString();
                //        ag.customerName = reader["name"].ToString();
                //        ag.invoiceNo = reader["invoiceno"].ToString();
                //        ag.referenceNo = reader["referenceno"].ToString();
                //        ag.date = Convert.ToDateTime(reader["transdate"].ToString()).ToShortDateString();
                //        ag.total = Convert.ToDecimal(reader["total"].ToString());

                //        arLogList.Add(ag);


                //    }
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.StackTrace);
                //}
                //finally
                //{
                //    reader.Close();
                //}


                return arLogList;
            }
        }
        //public List<InvoiceSearchViewModel> getSearchInvoiceList(string searchBy, string searchValue, string emailOnly, string printOnly)
        //{
        //    string spName = "spGet_AR_Log";
        //    List<InvoiceSearchViewModel> arLogList = new List<InvoiceSearchViewModel>();
        //    using (SqlConnection con = new SqlConnection(DBService.getConnectionStringJkBuf()))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand(spName, con);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        string date_format = "yyyy-MM-dd HH:mm:ss.fff";
        //        //DateTime start_date = DateTime.Parse(startDate);// DateTime.ParseExact(startDate, date_format, null);
        //        //DateTime end_date = DateTime.Parse(endDate);// DateTime.ParseExact(endDate, date_format, null);

        //        cmd.Parameters.AddWithValue("@datetype", Convert.ToInt32(dateType));
        //        cmd.Parameters.AddWithValue("@startdate", start_date.Date.ToString(date_format));
        //        cmd.Parameters.AddWithValue("@enddate", end_date.Date.ToString(date_format));

        //        SqlDataReader reader = cmd.ExecuteReader();
        //        try
        //        {
        //            while (reader.Read())
        //            {

        //                InvoiceSearchViewModel ag = new InvoiceSearchViewModel();
        //                ag.customerNo = reader["number"].ToString();
        //                ag.customerName = reader["name"].ToString();
        //                ag.invoiceNo = reader["invoiceno"].ToString();
        //                ag.referenceNo = reader["referenceno"].ToString();
        //                ag.date = Convert.ToDateTime(reader["transdate"].ToString()).ToShortDateString();
        //                ag.total = Convert.ToDecimal(reader["total"].ToString());

        //                arLogList.Add(ag);


        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.StackTrace);
        //        }
        //        finally
        //        {
        //            reader.Close();
        //        }


        //        return arLogList;
        //    }
        //}

        public List<AgingViewModel> getAgingListByFranchise(string franchiseid, string searchby, string searchvalue, string agingdate, string paymentdate, string months, string orderby, string include, string balance, string nonchargebackonly)
        {
            string spName = "spSearch_AR_AgingInvoices";



            List<string> paramz = new List<string>();
            paramz.Add(franchiseid);
            paramz.Add(searchby);
            paramz.Add(searchvalue);
            paramz.Add(agingdate);
            paramz.Add(paymentdate);
            paramz.Add(months);
            paramz.Add(orderby);
            paramz.Add(include);
            paramz.Add(balance);
            paramz.Add(nonchargebackonly);


            SqlDataReader reader = DBService.executeSP(spName, paramz);
            List<AgingViewModel> agingVMList = new List<AgingViewModel>();
            while (reader.Read())
            {

                //AgingViewModel ag = new AgingViewModel();
                //ag.customer = reader["Customer"].ToString();
                //ag.invDate = Convert.ToDateTime(reader["invDate"]);
                //ag.invNumber = reader["invNumber"].ToString();
                //ag.dueDate = Convert.ToDateTime(reader["dueDate"]);
                //ag.current = float.Parse(reader["current"].ToString());
                //ag._130 = float.Parse(reader["_130"].ToString());
                //ag._3160 = float.Parse(reader["_3160"].ToString());
                //ag._6190 = float.Parse(reader["_6190"].ToString());
                //ag._91plus = float.Parse(reader["_91plus"].ToString());

                //agingVMList.Add(ag);
            }

            return agingVMList;
        }

        public List<GetCustomerViewModwl> GetCustomers(string namePrefix, string InvoicePrefix)
        {
            using (var context = new jkDatabaseEntities())
            {
                var Query = context.Customers.Join(context.Invoices, _customer => _customer.CustomerId, _invoice => _invoice.ClassId, (_customer, _invoice) => new { Customer = _customer, Invoice = _invoice }).Select(c => new GetCustomerViewModwl { Name = c.Customer.Name, InvoiceNo = c.Invoice.InvoiceNo }).ToList();
                return Query;
            }
        }

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

        public List<PaymentMethodList> GetAll_PaymentMethodList()
        {
            List<PaymentMethodList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.PaymentMethodLists.ToList();
                return data;
            }
        }
        public List<AdjustmentReasonList> GetAll_AdjustmentReasonList()
        {
            List<AdjustmentReasonList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.AdjustmentReasonLists.ToList();
                return data;
            }
        }

        public List<CreditReasonList> GetAll_CreditReasonList(bool isTax)
        {
            List<CreditReasonList> data;
            using (var context = new jkDatabaseEntities())
            {
                data = context.CreditReasonLists.
                    Where(x=>x.IsTaxCredit.Equals(isTax)).OrderBy(o => o.Name).ToList();
                //    data = context.SearchDateLists. Select(x => new SearchDateList {Name= x.Name, SearchDateListId= x.SearchDateListId });
                return data;
            }
        }

        public List<LockboxEDIDataViewModel> GetLockboxData(int LockboxId)
        {
            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{
            //    return context.portal_spGet_AR_LockboxEDIData(LockboxId).MapEnumerable<LockboxEDIDataViewModel, portal_spGet_AR_LockboxEDIData_Result>().ToList();
            //}
            List<LockboxEDIDataViewModel> lstresult = new List<LockboxEDIDataViewModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LockboxEDIId", LockboxId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_LockboxEDIData", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstresult = multipleresult.Read<LockboxEDIDataViewModel>().ToList();
                    }
                }
            }
            return lstresult;
        }


        public List<LockboxPendingViewModel> GetLockboxPendingListData(int regionId)
        {
            List<LockboxPendingViewModel> lstresult = new List<LockboxPendingViewModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_LockboxEDIPendingList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstresult = multipleresult.Read<LockboxPendingViewModel>().ToList();
                    }
                }
            }
            return lstresult;
        }


        public LockboxEDIData GetLockboxDataDetail(int LockboxId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                LockboxEDIData oLockboxEDIData = new LockboxEDIData();
                List<LockboxEDIDataViewModel> lstLockboxEDIDataViewModel = context.portal_spGet_AR_LockboxEDIData(LockboxId).MapEnumerable<LockboxEDIDataViewModel, portal_spGet_AR_LockboxEDIData_Result>().ToList();

                bool IsFirst = true;
                List<LockboxEDIDataMatchedViewModel> lstLockboxEDIDataMatchedViewModel = new List<LockboxEDIDataMatchedViewModel>();
                List<LockboxEDIDataUnMatchedViewModel> lstLockboxEDIDataUnMatchedViewModel = new List<LockboxEDIDataUnMatchedViewModel>();

                LockboxEDIDataMatchedViewModel oLockboxEDIDataMatchedViewModel = new LockboxEDIDataMatchedViewModel();
                LockboxEDIDataUnMatchedViewModel oLockboxEDIDataUnMatchedViewModel = new LockboxEDIDataUnMatchedViewModel();

                int MatchedId = 0;
                int UnMatchedId = 0;
                decimal LockboxApplyAmount = 0;
                decimal LockboxExecptionAmount = 0;

                var d = lstLockboxEDIDataViewModel.Where(t => t.InvoiceId == null || t.BalanceAmount != 0).ToList();

                foreach (LockboxEDIDataViewModel o in lstLockboxEDIDataViewModel)
                {
                    if (IsFirst)
                    {
                        oLockboxEDIData.LockboxAmount = o.TotalDollars;
                        oLockboxEDIData.LockboxEDIId = o.LockboxEDIId;
                        oLockboxEDIData.LockboxNumber = o.LockboxNumber;
                        oLockboxEDIData.RegionId = o.RegionId;
                        oLockboxEDIData.RegionName = o.RegionName;

                        IsFirst = false;
                    }
                    if (d.Where(k => k.CheckNumber == o.CheckNumber).ToList().Count > 0)
                    {
                        UnMatchedId++;
                        oLockboxEDIDataUnMatchedViewModel = new LockboxEDIDataUnMatchedViewModel();
                        oLockboxEDIDataUnMatchedViewModel.ApplyAmount = o.ApplyAmount;
                        oLockboxEDIDataUnMatchedViewModel.BalanceAmount = o.BalanceAmount;
                        oLockboxEDIDataUnMatchedViewModel.CheckNumber = o.CheckNumber;
                        oLockboxEDIDataUnMatchedViewModel.CustomerId = o.CustomerId;
                        oLockboxEDIDataUnMatchedViewModel.CustomerName = o.CustomerName;
                        oLockboxEDIDataUnMatchedViewModel.CustomerNo = o.CustomerNo;
                        oLockboxEDIDataUnMatchedViewModel.InvoiceAmount = o.InvoiceAmount;
                        oLockboxEDIDataUnMatchedViewModel.InvoiceDate = o.InvoiceDate;
                        oLockboxEDIDataUnMatchedViewModel.InvoiceId = o.InvoiceId;
                        oLockboxEDIDataUnMatchedViewModel.InvoiceNo = o.InvoiceNo;
                        oLockboxEDIDataUnMatchedViewModel.LockboxEDIDetailId = o.LockboxEDIDetailId;
                        oLockboxEDIDataUnMatchedViewModel.UnMatchedId = UnMatchedId;
                        oLockboxEDIDataUnMatchedViewModel.RowNo = o.RowNo;
                        lstLockboxEDIDataUnMatchedViewModel.Add(oLockboxEDIDataUnMatchedViewModel);
                        LockboxExecptionAmount = LockboxExecptionAmount + (decimal)o.ApplyAmount;
                    }
                    else
                    {
                        MatchedId++;
                        oLockboxEDIDataMatchedViewModel = new LockboxEDIDataMatchedViewModel();
                        oLockboxEDIDataMatchedViewModel.ApplyAmount = o.ApplyAmount;
                        oLockboxEDIDataMatchedViewModel.BalanceAmount = o.BalanceAmount;
                        oLockboxEDIDataMatchedViewModel.CheckNumber = o.CheckNumber;
                        oLockboxEDIDataMatchedViewModel.CustomerId = o.CustomerId;
                        oLockboxEDIDataMatchedViewModel.CustomerName = o.CustomerName;
                        oLockboxEDIDataMatchedViewModel.CustomerNo = o.CustomerNo;
                        oLockboxEDIDataMatchedViewModel.InvoiceAmount = o.InvoiceAmount;
                        oLockboxEDIDataMatchedViewModel.InvoiceDate = o.InvoiceDate;
                        oLockboxEDIDataMatchedViewModel.InvoiceId = o.InvoiceId;
                        oLockboxEDIDataMatchedViewModel.InvoiceNo = o.InvoiceNo;
                        oLockboxEDIDataMatchedViewModel.LockboxEDIDetailId = o.LockboxEDIDetailId;
                        oLockboxEDIDataMatchedViewModel.MatchedId = MatchedId;
                        oLockboxEDIDataMatchedViewModel.RowNo = o.RowNo;
                        lstLockboxEDIDataMatchedViewModel.Add(oLockboxEDIDataMatchedViewModel);
                        LockboxApplyAmount = LockboxApplyAmount + (decimal)o.ApplyAmount;
                    }


                }

                oLockboxEDIData.LockboxApplyAmount = LockboxApplyAmount;
                oLockboxEDIData.LockboxExecptionAmount = LockboxExecptionAmount;
                oLockboxEDIData.LockboxEDIDataMatched = lstLockboxEDIDataMatchedViewModel;
                oLockboxEDIData.LockboxEDIDataUnmatched = lstLockboxEDIDataUnMatchedViewModel;
                return oLockboxEDIData;
            }
        }

        public LockboxEDI AlreadyExistFileUploadLockboxUpdated(string _line)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<LockboxEDI> lstLockbox = context.LockboxEDIs.Where(o => o.LockboxData.StartsWith(_line)).ToList();
                if (lstLockbox.Count > 0)
                    return lstLockbox.LastOrDefault();

            }
            return new LockboxEDI();
        }

        public Region GetRegionByLockboxNumber(string lockboxnumber)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                return context.Regions.SingleOrDefault(o => o.LockboxId == lockboxnumber);
            }
        }

        public void LockboxEDIProcess(int lockboxId)
        {

            List<ARLogListFinalViewModel> lstARLogList = new List<ARLogListFinalViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LockboxEDIId", lockboxId);
                conn.Query("dbo.portal_spGet_AR_LockboxEDIProcess", parmas, commandType: CommandType.StoredProcedure);
            }


        }
        public void UploadLockboxDate(int lockboxId, DateTime chkDate)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                LockboxEDI oLockboxEDI = context.LockboxEDIs.SingleOrDefault(o => o.LockboxEDIId == lockboxId);
                if (oLockboxEDI != null)
                {
                    oLockboxEDI.LockboxDate = chkDate;
                    context.SaveChanges();
                }
            }
        }
        public List<LockboxEDIDataViewModel> UploadLockboxUpdated(List<CommonTransmissionViewModel> inputdata)
        {
            int retVal = 0;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                bool isFirst = true;
                LockboxEDIHistory oLockboxEDIHistory;
                int rowid = 0;
                foreach (CommonTransmissionViewModel o in inputdata)
                {
                    rowid++;
                    if (isFirst)
                    {
                        LockboxEDI oLockboxEDI = new LockboxEDI();
                        oLockboxEDI.IsProcessed = false;
                        oLockboxEDI.LockboxDate = o.LockboxDate;
                        oLockboxEDI.LockboxFileName = o.LockboxFileName;
                        oLockboxEDI.UploadedOn = DateTime.Now;
                        oLockboxEDI.LockboxData = o.LockboxData;
                        oLockboxEDI.RegionId = o.RegionId;// SelectedRegionId;
                        oLockboxEDI.RegionName = o.RegionName;
                        oLockboxEDI.StatusListId = 51;
                        oLockboxEDI.CreatedBy = LoginUserId;
                        context.LockboxEDIs.Add(oLockboxEDI);
                        context.SaveChanges();
                        isFirst = false;
                        retVal = oLockboxEDI.LockboxEDIId;
                    }

                    oLockboxEDIHistory = new LockboxEDIHistory();
                    oLockboxEDIHistory.RowNo = rowid;
                    oLockboxEDIHistory.AccountNumber = o.AccountNumber;
                    oLockboxEDIHistory.ApplyAmount = o.ApplyAmount;
                    oLockboxEDIHistory.BankName = o.BankName;
                    oLockboxEDIHistory.BankState = o.BankState;
                    oLockboxEDIHistory.BatchNumber = o.BatchNumber;
                    oLockboxEDIHistory.BlockSize = o.BlockSize;
                    oLockboxEDIHistory.CheckNumber = o.CheckNumber;
                    oLockboxEDIHistory.CustomerNo = o.CustomerNo;
                    oLockboxEDIHistory.Destination = o.Destination;
                    oLockboxEDIHistory.DollarAmount = o.DollarAmount;
                    oLockboxEDIHistory.FormatCodeUncompressed = o.FormatCodeUncompressed;
                    oLockboxEDIHistory.HHMM = o.HHMM;
                    oLockboxEDIHistory.InvoiceNo = o.InvoiceNo;
                    oLockboxEDIHistory.ItemNumber = o.ItemNumber;
                    oLockboxEDIHistory.LastOverflowIndicator = o.LastOverflowIndicator;
                    oLockboxEDIHistory.LockboxDate = o.LockboxDate;
                    oLockboxEDIHistory.LockboxEDIId = retVal;
                    oLockboxEDIHistory.LockboxFileName = o.LockboxFileName;
                    oLockboxEDIHistory.LockboxNumber = o.LockboxNumber;
                    oLockboxEDIHistory.LockboxRaw = o.LockboxRaw;
                    oLockboxEDIHistory.Origin = o.Origin;
                    oLockboxEDIHistory.PriorityCode = o.PriorityCode;
                    oLockboxEDIHistory.RecordCount = o.RecordCount;
                    oLockboxEDIHistory.RecordSize = o.RecordSize;
                    oLockboxEDIHistory.RecordType = o.RecordType;
                    oLockboxEDIHistory.ReferenceCode = o.ReferenceCode;
                    oLockboxEDIHistory.RegionBankName = o.RegionBankName;
                    oLockboxEDIHistory.SequenceNumber = o.SequenceNumber;
                    oLockboxEDIHistory.ServiceType = o.ServiceType;
                    oLockboxEDIHistory.TotalDollars = o.TotalDollars;
                    oLockboxEDIHistory.TotalItems = o.TotalItems;
                    oLockboxEDIHistory.TransitRoutingNumber = o.TransitRoutingNumber;
                    oLockboxEDIHistory.TypeofOverFlowingRecord = o.TypeofOverFlowingRecord;
                    oLockboxEDIHistory.YYMMDD = o.YYMMDD;
                    oLockboxEDIHistory.OverflowAmount = 0;
                    oLockboxEDIHistory.RegionId = o.RegionId;// SelectedRegionId;
                    oLockboxEDIHistory.RegionName = o.RegionName;

                    oLockboxEDIHistory.IsODeposit = false;
                    oLockboxEDIHistory.DepositReason = "";
                    oLockboxEDIHistory.DepositServiceTypeListId = -1;
                    oLockboxEDIHistory.DepositPayeeType = "";
                    oLockboxEDIHistory.DepositPayeeId = -1;
                    oLockboxEDIHistory.DepositPayeeName = "";
                    oLockboxEDIHistory.DepositPayeeNo = "";



                    oLockboxEDIHistory.IsActive = true;
                    oLockboxEDIHistory.IsProcessed = false;
                    oLockboxEDIHistory.ModifiedBy = LoginUserId;
                    oLockboxEDIHistory.ModifiedOn = DateTime.UtcNow;

                    //oLockboxEDIHistory.s

                    context.LockboxEDIHistories.Add(oLockboxEDIHistory);
                }
                context.SaveChanges();

                LockboxEDIProcess(retVal);

                return GetLockboxData(retVal);

            }
        }


        public bool HaveMultipleInvoiceDistribution(int invoiceId)
        {
            bool retVal = false;
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                int a = context.BillingPays.Where(o => o.InvoiceId == invoiceId).ToList().Count;
                int q = (from pd in context.Invoices
                         join od in context.MasterTrxDetails on pd.MasterTrxId equals od.MasterTrxId
                         where pd.InvoiceId == invoiceId
                         select pd).ToList().Count;
                if ((a + q) > 2)
                    retVal = true;

            }
            return retVal;
        }
        public static string GenerateCommandText(string storedProcedure, SqlParameter[] parameters)
        {
            string CommandText = "EXEC {0} {1}";
            string[] ParameterNames = new string[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterNames[i] = parameters[i].ParameterName;
            }

            return string.Format(CommandText, storedProcedure, string.Join(",", ParameterNames));
        }

        public bool UpdateLockboxDetailCheckInactive(int LockboxId, string CheckNumber)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {


                conn.Query(string.Format("Update LockboxEDIHistory SET IsActive=0 WHERE LockboxEDIId = {0} AND CheckNumber = '{1}'", LockboxId, CheckNumber), commandType: CommandType.Text);

            }
            return true;
        }
        public bool UpdateLockboxDetail(int LockboxId, int LockboxEDIHistoryId, string CheckNumber, int CustomerId, int InvoiceId, decimal InvoiceAmount, decimal BalanceAmount,
            decimal ApplyAmount, decimal OverflowAmount, int StatusListId, bool IsNEW,
            bool IsODeposit = false, string DepositReason = "", string DepositPayeeType = "", int DepositServiceTypeListId = -1, int DepositPayeeId = -1, string DepositPayeeName = "", string DepositPayeeNo = "")
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LockboxEDIId", LockboxId);
                parmas.Add("@LockboxEDIHistoryId", (IsNEW ? 0 : LockboxEDIHistoryId));
                parmas.Add("@IsNew", IsNEW);
                parmas.Add("@CheckNumber", CheckNumber);
                parmas.Add("@CustomerId", CustomerId);
                parmas.Add("@InvoiceId", InvoiceId);
                parmas.Add("@InvoiceAmount", InvoiceAmount);
                parmas.Add("@BalanceAmount", BalanceAmount);
                parmas.Add("@ApplyAmount", ApplyAmount);
                parmas.Add("@OverflowAmount", OverflowAmount);
                parmas.Add("@StatusListId", StatusListId);
                parmas.Add("@UserId", LoginUserId);

                parmas.Add("@IsODeposit", IsODeposit);
                parmas.Add("@DepositReason", DepositReason);
                parmas.Add("@DepositPayeeType", DepositPayeeType);
                parmas.Add("@DepositServiceTypeListId", DepositServiceTypeListId);
                parmas.Add("@DepositPayeeId", DepositPayeeId);
                parmas.Add("@DepositPayeeName", DepositPayeeName);
                parmas.Add("@DepositPayeeNo", DepositPayeeNo);



                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_LockboxDetailUpdate", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        return true;
                    }
                }



                //if (IsProcessed)
                //{
                //    if (OverflowAmount > 0)
                //        conn.Query(string.Format("Update LockboxEDIHistory SET CustomerId={0},InvoiceId={1},StatusListId = 52,ApplyAmount ={2}, OverflowAmount={3},InvoiceAmount =ISNULL((SELECT SUM(OriginalTotal) FROM fn_GetInvoiceLineItemBalance({1})),0) WHERE LockboxEDIHistoryId = {4}", CustomerId, InvoiceId, ApplyAmount, OverflowAmount, LockboxEDIHistoryId), commandType: CommandType.Text);
                //    else
                //        conn.Query(string.Format("Update LockboxEDIHistory SET CustomerId={0},InvoiceId={1},StatusListId = 52,ApplyAmount ={2}, OverflowAmount={3},InvoiceAmount =ISNULL((SELECT SUM(OriginalTotal) FROM fn_GetInvoiceLineItemBalance({1})),0) WHERE LockboxEDIHistoryId = {4}", CustomerId, InvoiceId, ApplyAmount, 0, LockboxEDIHistoryId), commandType: CommandType.Text);

                //    conn.Query(string.Format("Update LockboxEDI SET IsProcessed=1,ProcessedOn=getdate(),StatusListId = 52 WHERE LockboxEDIId = {0}", LockboxId), commandType: CommandType.Text);
                //}
                //else
                //{
                //    if (OverflowAmount > 0)
                //        conn.Query(string.Format("Update LockboxEDIHistory SET CustomerId={0},InvoiceId={1},StatusListId = 49,ApplyAmount ={2}, OverflowAmount={3} ,InvoiceAmount =ISNULL((SELECT SUM(OriginalTotal) FROM fn_GetInvoiceLineItemBalance({1})),0) WHERE LockboxEDIHistoryId = {4}", CustomerId, InvoiceId, ApplyAmount, OverflowAmount, LockboxEDIHistoryId), commandType: CommandType.Text);
                //    else
                //        conn.Query(string.Format("Update LockboxEDIHistory SET CustomerId={0},InvoiceId={1},StatusListId = 49,ApplyAmount ={2}, OverflowAmount={3} ,InvoiceAmount =ISNULL((SELECT SUM(OriginalTotal) FROM fn_GetInvoiceLineItemBalance({1})),0) WHERE LockboxEDIHistoryId = {4}", CustomerId, InvoiceId, ApplyAmount, 0, LockboxEDIHistoryId), commandType: CommandType.Text);
                //}
            }
            return true;
        }

        public bool ProcessLockboxPayment(int LockboxId)
        {

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@LockboxId", LockboxId);
                parmas.Add("@CreatedBy", LoginUserId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_LockboxProcessPayment", parmas, commandType: CommandType.StoredProcedure, commandTimeout: 6000))
                {
                    if (multipleresult != null)
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        public bool ProcessPayment(string ChaqueNumber, int InvoiceId, int CustomerId, decimal InvoiceAmount, decimal ApplyAmount, int PaymentMethodListId, int TransactionStatusListId, List<PartialLockboxPaymentItemViewModel> lstPartialLockboxPaymentItem, DateTime? TRDate = null, decimal? TCheckAmount = 0, decimal OverflowAmount = 0)
        {


            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {

                if (TransactionStatusListId == 6)
                {

                    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                    {
                        var parmas = new DynamicParameters();
                        parmas.Add("@ChaqueNumber", ChaqueNumber);
                        parmas.Add("@InvoiceId", InvoiceId);
                        parmas.Add("@CustomerId", CustomerId);
                        parmas.Add("@InvoiceAmount", InvoiceAmount);
                        parmas.Add("@ApplyAmount", ApplyAmount);
                        parmas.Add("@OverflowAmount", OverflowAmount);
                        parmas.Add("@PaymentMethodListId", PaymentMethodListId);// --1 Check, 2 Credit Card, 3 EFT, 4 Lockbox                        
                                                                                //parmas.Add("@TransactionStatusListId", TransactionStatusListId);  //--6  Paid, 7 Paid Partial
                        parmas.Add("@TransactionStatusListId", 3);
                        if (TRDate != null)
                            parmas.Add("@TransactionDate", TRDate);
                        parmas.Add("@CreatedBy", LoginUserId);
                        parmas.Add("@CheckAmount", TCheckAmount);



                        using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_LockboxPayment", parmas, commandType: CommandType.StoredProcedure))
                        //using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_Payment", parmas, commandType: CommandType.StoredProcedure))
                        {
                            if (multipleresult != null)
                            {
                                return true;
                            }
                        }
                    }

                    //context.portal_spCreate_AR_Payment(ChaqueNumber, InvoiceId, CustomerId, InvoiceAmount, ApplyAmount, PaymentMethodListId, TransactionStatusListId, LoginUserId);
                }
                else
                {


                    var PaymentItemDT = new DataTable();

                    PaymentItemDT.Columns.Add("Id", typeof(int));
                    PaymentItemDT.Columns.Add("InvoiceId", typeof(int));
                    PaymentItemDT.Columns.Add("BillPayId", typeof(int));
                    PaymentItemDT.Columns.Add("LineNumber", typeof(int));
                    PaymentItemDT.Columns.Add("CustomerId", typeof(int));
                    PaymentItemDT.Columns.Add("FranchiseeId", typeof(int));
                    PaymentItemDT.Columns.Add("ApplyAmount", typeof(decimal));
                    PaymentItemDT.Columns.Add("IsCustomerSide", typeof(bool));

                    int fn = 0;
                    foreach (PartialLockboxPaymentItemViewModel o in lstPartialLockboxPaymentItem)
                    {
                        fn++;
                        PaymentItemDT.Rows.Add(fn, o.InvoiceId, o.BillPayId, o.LineNumber, o.CustomerId, o.FranchiseeId, o.ApplyAmount, o.IsCustomerSide);
                    }
                    if (lstPartialLockboxPaymentItem.Count == 0)
                    {
                        PaymentItemDT.Rows.Add(1, InvoiceId, 0, 1, CustomerId, 0, ApplyAmount, true);
                        PaymentItemDT.Rows.Add(1, InvoiceId, 0, 1, CustomerId, 0, ApplyAmount, false);
                    }

                    List<SqlParameter> lstSqlParameter = new List<SqlParameter>();

                    lstSqlParameter.Add(new SqlParameter("@ChaqueNumber", ChaqueNumber));
                    lstSqlParameter.Add(new SqlParameter("@InvoiceId", InvoiceId));
                    lstSqlParameter.Add(new SqlParameter("@CustomerId", CustomerId));
                    lstSqlParameter.Add(new SqlParameter("@InvoiceAmount", InvoiceAmount));
                    lstSqlParameter.Add(new SqlParameter("@ApplyAmount", ApplyAmount));
                    lstSqlParameter.Add(new SqlParameter("@PaymentMethodListId", PaymentMethodListId));
                    //lstSqlParameter.Add(new SqlParameter("@TransactionStatusListId", TransactionStatusListId));
                    lstSqlParameter.Add(new SqlParameter("@TransactionStatusListId", 3));
                    lstSqlParameter.Add(new SqlParameter("@CreatedBy", LoginUserId));
                    if (TRDate != null)
                        lstSqlParameter.Add(new SqlParameter("@TransactionDate", TRDate));
                    lstSqlParameter.Add(new SqlParameter("@CheckAmount", TCheckAmount));
                    var parameter = new SqlParameter("@PaymentItems", SqlDbType.Structured);
                    parameter.Value = PaymentItemDT;
                    parameter.TypeName = "dbo.PartialLockboxPaymentItem";
                    lstSqlParameter.Add(parameter);



                    var reft = context.Database.ExecuteSqlCommand(GenerateCommandText("dbo.portal_spCreate_AR_PaymentPartial", lstSqlParameter.ToArray()), lstSqlParameter.ToArray());
                }


                return true;
            }
        }

        public bool ProcessLockboxOtherDeposit(DateTime TransactionDate, int PayeeId, string PayeeType, string DepositReason, int DepositServiceTypeListId, decimal ApplyAmount, string ChaqueNumber, int LockboxId)
        {


            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {


                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();

                    parmas.Add("@TransactionDate", TransactionDate);
                    parmas.Add("@PayeeId", PayeeId);
                    parmas.Add("@PayeeType", PayeeType);
                    parmas.Add("@DepositReason", DepositReason);
                    parmas.Add("@DepositServiceTypeListId", DepositServiceTypeListId);
                    parmas.Add("@ApplyAmount", ApplyAmount);
                    parmas.Add("@ChaqueNumber", ChaqueNumber);
                    parmas.Add("@CreatedBy", LoginUserId);
                    parmas.Add("@LockboxId", LockboxId);

                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_LockboxOtherDeposit", parmas, commandType: CommandType.StoredProcedure))
                    //using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_Payment", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            return true;
                        }
                    }
                }


                return true;
            }
        }

        public bool InsertOtherDeposit(DateTime TransactionDate, int PayeeId, string PayeeType, string DepositReason, int DepositServiceTypeListId, decimal ApplyAmount, string ChaqueNumber,
            string PayeeName, string PayeeNumber)
        {


            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {


                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();


                    //OD_txtPayeeName
                    //OD_txtPayeeId
                    //OD_txtPayeeType
                    //OD_txtPayeeNumber
                    //OD_txtReferenceNo
                    //OD_txtAmount
                    //OD_txtDate
                    //OD_ServiceTypeListIdTrans
                    //OD_txtReason

                    parmas.Add("@TransactionDate", TransactionDate);
                    parmas.Add("@PayeeId", PayeeId);
                    parmas.Add("@PayeeType", PayeeType);
                    parmas.Add("@DepositReason", DepositReason);
                    parmas.Add("@DepositServiceTypeListId", DepositServiceTypeListId);
                    parmas.Add("@ApplyAmount", ApplyAmount);
                    parmas.Add("@ChaqueNumber", ChaqueNumber);
                    parmas.Add("@CreatedBy", LoginUserId);
                    parmas.Add("@PayeeName", PayeeName);
                    parmas.Add("@PayeeNumber", PayeeNumber);
                    parmas.Add("@LockboxId", 0);
                    parmas.Add("@RegionId", SelectedRegionId);

                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_OtherDeposit", parmas, commandType: CommandType.StoredProcedure))
                    //using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_Payment", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            return true;
                        }
                    }
                }


                return true;
            }
        }


        public bool ProcessCheckbookPaymentLockbox(DateTime TransactionDate, decimal CheckAmount, int LockboxId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var _r = (from LB in context.LockboxEDIs
                          join LBH in context.LockboxEDIHistories on LB.LockboxEDIId equals LBH.LockboxEDIId
                          where LB.LockboxEDIId == LockboxId
                          select new
                          {
                              LB.RegionId,
                              LBH.LockboxNumber
                          }).ToList();

                if (_r.Count > 0)
                {
                    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                    {
                        var parmas = new DynamicParameters();
                        parmas.Add("@TransactionDate", TransactionDate);
                        parmas.Add("@RegionId", _r.FirstOrDefault().RegionId);
                        parmas.Add("@CreatedBy", LoginUserId);
                        parmas.Add("@CustomerId", null);
                        parmas.Add("@ApplyAmount", CheckAmount);
                        //parmas.Add("@ReferenceNo", Convert.ToString(_r.FirstOrDefault().LockboxNumber) + " - " + TransactionDate.ToString("MM/dd/yyyy"));
                        //parmas.Add("@Notes", "Remittance Received - Lockbox:" + Convert.ToString(_r.FirstOrDefault().LockboxNumber));

                        parmas.Add("@ReferenceNo", "LB" + Convert.ToString(_r.FirstOrDefault().LockboxNumber));
                        parmas.Add("@Notes", "Lockbox:" + " - " + TransactionDate.ToString("MM/dd/yyyy"));

                        parmas.Add("@FundTypeListId", 4);
                        using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_CheckBookInsert", parmas, commandType: CommandType.StoredProcedure))
                        {
                            if (multipleresult != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }



            return true;
            //@TransactionDate Date = NULL,
            //@RegionId INT,
            //@CreatedBy int,
            //@CustomerId INT = -1,
            //@ApplyAmount DECIMAL(18,2),
            //@ReferenceNo varchar(500)= '',
            //@Notes varchar(500)= NULL,
            //@FundTypeListId INT = 3


            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{

            //    if (TransactionStatusListId == 6)
            //    {

            //        using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            //        {
            //            var parmas = new DynamicParameters();
            //            parmas.Add("@ChaqueNumber", ChaqueNumber);
            //            parmas.Add("@InvoiceId", InvoiceId);
            //            parmas.Add("@CustomerId", CustomerId);
            //            parmas.Add("@InvoiceAmount", InvoiceAmount);
            //            parmas.Add("@ApplyAmount", ApplyAmount);
            //            parmas.Add("@OverflowAmount", OverflowAmount);
            //            parmas.Add("@PaymentMethodListId", PaymentMethodListId);// --1 Check, 2 Credit Card, 3 EFT, 4 Lockbox                        
            //            //parmas.Add("@TransactionStatusListId", TransactionStatusListId);  //--6  Paid, 7 Paid Partial
            //            parmas.Add("@TransactionStatusListId", 3);
            //            if (TRDate != null)
            //                parmas.Add("@TransactionDate", TRDate);
            //            parmas.Add("@CreatedBy", LoginUserId);
            //            parmas.Add("@CheckAmount", TCheckAmount);



            //            using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_LockboxPayment", parmas, commandType: CommandType.StoredProcedure))
            //            //using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_Payment", parmas, commandType: CommandType.StoredProcedure))
            //            {
            //                if (multipleresult != null)
            //                {
            //                    return true;
            //                }
            //            }
            //        }

            //        //context.portal_spCreate_AR_Payment(ChaqueNumber, InvoiceId, CustomerId, InvoiceAmount, ApplyAmount, PaymentMethodListId, TransactionStatusListId, LoginUserId);
            //    }
            //    else
            //    {


            //        var PaymentItemDT = new DataTable();

            //        PaymentItemDT.Columns.Add("Id", typeof(int));
            //        PaymentItemDT.Columns.Add("InvoiceId", typeof(int));
            //        PaymentItemDT.Columns.Add("BillPayId", typeof(int));
            //        PaymentItemDT.Columns.Add("LineNumber", typeof(int));
            //        PaymentItemDT.Columns.Add("CustomerId", typeof(int));
            //        PaymentItemDT.Columns.Add("FranchiseeId", typeof(int));
            //        PaymentItemDT.Columns.Add("ApplyAmount", typeof(decimal));
            //        PaymentItemDT.Columns.Add("IsCustomerSide", typeof(bool));

            //        int fn = 0;
            //        foreach (PartialLockboxPaymentItemViewModel o in lstPartialLockboxPaymentItem)
            //        {
            //            fn++;
            //            PaymentItemDT.Rows.Add(fn, o.InvoiceId, o.BillPayId, o.LineNumber, o.CustomerId, o.FranchiseeId, o.ApplyAmount, o.IsCustomerSide);
            //        }
            //        if (lstPartialLockboxPaymentItem.Count == 0)
            //        {
            //            PaymentItemDT.Rows.Add(1, InvoiceId, 0, 1, CustomerId, 0, ApplyAmount, true);
            //            PaymentItemDT.Rows.Add(1, InvoiceId, 0, 1, CustomerId, 0, ApplyAmount, false);
            //        }

            //        List<SqlParameter> lstSqlParameter = new List<SqlParameter>();

            //        lstSqlParameter.Add(new SqlParameter("@ChaqueNumber", ChaqueNumber));
            //        lstSqlParameter.Add(new SqlParameter("@InvoiceId", InvoiceId));
            //        lstSqlParameter.Add(new SqlParameter("@CustomerId", CustomerId));
            //        lstSqlParameter.Add(new SqlParameter("@InvoiceAmount", InvoiceAmount));
            //        lstSqlParameter.Add(new SqlParameter("@ApplyAmount", ApplyAmount));
            //        lstSqlParameter.Add(new SqlParameter("@PaymentMethodListId", PaymentMethodListId));
            //        //lstSqlParameter.Add(new SqlParameter("@TransactionStatusListId", TransactionStatusListId));
            //        lstSqlParameter.Add(new SqlParameter("@TransactionStatusListId", 3));
            //        lstSqlParameter.Add(new SqlParameter("@CreatedBy", LoginUserId));
            //        if (TRDate != null)
            //            lstSqlParameter.Add(new SqlParameter("@TransactionDate", TRDate));
            //        lstSqlParameter.Add(new SqlParameter("@CheckAmount", TCheckAmount));
            //        var parameter = new SqlParameter("@PaymentItems", SqlDbType.Structured);
            //        parameter.Value = PaymentItemDT;
            //        parameter.TypeName = "dbo.PartialLockboxPaymentItem";
            //        lstSqlParameter.Add(parameter);



            //        var reft = context.Database.ExecuteSqlCommand(GenerateCommandText("dbo.portal_spCreate_AR_PaymentPartial", lstSqlParameter.ToArray()), lstSqlParameter.ToArray());
            //    }


            //    return true;
            //}
        }




        //        public bool ProcessPayment(List<LockboxPaymentViewModel> lstLockboxPayment)
        //        {




        //            using (jkDatabaseEntities context = new jkDatabaseEntities())
        //            {

        //                //if (TransactionStatusListId == 6)
        //                //{

        //                //    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
        //                //    {
        //                //        var parmas = new DynamicParameters();
        //                //        parmas.Add("@ChaqueNumber", ChaqueNumber);
        //                //        parmas.Add("@InvoiceId", InvoiceId);
        //                //        parmas.Add("@CustomerId", CustomerId);
        //                //        parmas.Add("@InvoiceAmount", InvoiceAmount);
        //                //        parmas.Add("@ApplyAmount", ApplyAmount);
        //                //        parmas.Add("@OverflowAmount", OverflowAmount);
        //                //        parmas.Add("@PaymentMethodListId", PaymentMethodListId);// --1 Check, 2 Credit Card, 3 EFT, 4 Lockbox                        
        //                //        parmas.Add("@TransactionStatusListId", TransactionStatusListId);  //--6  Paid, 7 Paid Partial
        //                //        if (TRDate != null)
        //                //            parmas.Add("@TransactionDate", TRDate);
        //                //        parmas.Add("@CreatedBy", LoginUserId);
        //                //        parmas.Add("@CheckAmount", TCheckAmount);



        //                //        using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_LockboxPayment", parmas, commandType: CommandType.StoredProcedure))
        //                //        //using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_Payment", parmas, commandType: CommandType.StoredProcedure))
        //                //        {
        //                //            if (multipleresult != null)
        //                //            {
        //                //                return true;
        //                //            }
        //                //        }
        //                //    }

        //                //    //context.portal_spCreate_AR_Payment(ChaqueNumber, InvoiceId, CustomerId, InvoiceAmount, ApplyAmount, PaymentMethodListId, TransactionStatusListId, LoginUserId);
        //                //}
        //                //else
        //                //{


        //                //    var PaymentItemDT = new DataTable();

        //                //    PaymentItemDT.Columns.Add("Id", typeof(int));
        //                //    PaymentItemDT.Columns.Add("InvoiceId", typeof(int));
        //                //    PaymentItemDT.Columns.Add("BillPayId", typeof(int));
        //                //    PaymentItemDT.Columns.Add("LineNumber", typeof(int));
        //                //    PaymentItemDT.Columns.Add("CustomerId", typeof(int));
        //                //    PaymentItemDT.Columns.Add("FranchiseeId", typeof(int));
        //                //    PaymentItemDT.Columns.Add("ApplyAmount", typeof(decimal));
        //                //    PaymentItemDT.Columns.Add("IsCustomerSide", typeof(bool));

        //                //    int fn = 0;
        //                //    foreach (PartialLockboxPaymentItemViewModel o in lstPartialLockboxPaymentItem)
        //                //    {
        //                //        fn++;
        //                //        PaymentItemDT.Rows.Add(fn, o.InvoiceId, o.BillPayId, o.LineNumber, o.CustomerId, o.FranchiseeId, o.ApplyAmount, o.IsCustomerSide);
        //                //    }
        //                //    if (lstPartialLockboxPaymentItem.Count == 0)
        //                //    {
        //                //        PaymentItemDT.Rows.Add(1, InvoiceId, 0, 1, CustomerId, 0, ApplyAmount, true);
        //                //        PaymentItemDT.Rows.Add(1, InvoiceId, 0, 1, CustomerId, 0, ApplyAmount, false);
        //                //    }

        //                //    List<SqlParameter> lstSqlParameter = new List<SqlParameter>();

        //                //    lstSqlParameter.Add(new SqlParameter("@ChaqueNumber", ChaqueNumber));
        //                //    lstSqlParameter.Add(new SqlParameter("@InvoiceId", InvoiceId));
        //                //    lstSqlParameter.Add(new SqlParameter("@CustomerId", CustomerId));
        //                //    lstSqlParameter.Add(new SqlParameter("@InvoiceAmount", InvoiceAmount));
        //                //    lstSqlParameter.Add(new SqlParameter("@ApplyAmount", ApplyAmount));
        //                //    lstSqlParameter.Add(new SqlParameter("@PaymentMethodListId", PaymentMethodListId));
        //                //    lstSqlParameter.Add(new SqlParameter("@TransactionStatusListId", TransactionStatusListId));
        //                //    lstSqlParameter.Add(new SqlParameter("@CreatedBy", LoginUserId));
        //                //    if (TRDate != null)
        //                //        lstSqlParameter.Add(new SqlParameter("@TransactionDate", TRDate));
        //                //    lstSqlParameter.Add(new SqlParameter("@CheckAmount", TCheckAmount));
        //                //    var parameter = new SqlParameter("@PaymentItems", SqlDbType.Structured);
        //                //    parameter.Value = PaymentItemDT;
        //                //    parameter.TypeName = "dbo.PartialLockboxPaymentItem";
        //                //    lstSqlParameter.Add(parameter);



        //                //    var reft = context.Database.ExecuteSqlCommand(GenerateCommandText("dbo.portal_spCreate_AR_PaymentPartial", lstSqlParameter.ToArray()), lstSqlParameter.ToArray());
        //                //}


        //                return true;
        //            }







        //            DateTime _TrasactionDate = lstLockboxPayment.FirstOrDefault().LockboxDate;



        //            using (jkDatabaseEntities context = new jkDatabaseEntities())
        //            {


        //                MasterTrxTypeList _MasterTrxTypeList_Customer = context.MasterTrxTypeLists.SingleOrDefault(o => o.MasterTrxTypeListId == 2);//2   Payment
        //                MasterTrxTypeList _MasterTrxTypeList_Franchisee = context.MasterTrxTypeLists.SingleOrDefault(o => o.MasterTrxTypeListId == 7);//7   Franchisee BillPay



        //                var _tempInvoice = context.Invoices.SingleOrDefault(c => c.InvoiceId == lstLockboxPayment.FirstOrDefault().InvoiceId);
        //                int _RegionId = _tempInvoice != null ? (int)_tempInvoice.RegionId : SelectedRegionId;
        //                //MasterTrx
        //                MasterTrx oMasterTrx = new MasterTrx();



        //                oMasterTrx.TrxDate = _TrasactionDate;
        //                oMasterTrx.BillMonth = _TrasactionDate.Month;
        //                oMasterTrx.BillYear = _TrasactionDate.Year;

        //                var _period = context.Periods.Where(p => p.BillMonth == _TrasactionDate.Month && p.BillYear == _TrasactionDate.Year).FirstOrDefault();
        //                int _PeriodId = _period != null ? _period.PeriodId : 0;
        //                oMasterTrx.PeriodId = _PeriodId;
        //                oMasterTrx.CreatedBy = LoginUserId;
        //                oMasterTrx.CreatedDate = DateTime.Now;
        //                oMasterTrx.MasterTrxTypeListId = 2;
        //                oMasterTrx.RegionId = _RegionId;
        //                oMasterTrx.StatusId = 4;

        //                //oMasterTrx.BatchId;
        //                //oMasterTrx.AccountTypeListId;
        //                //oMasterTrx.HeaderId;
        //                //oMasterTrx.ClassId;
        //                //oMasterTrx.TypeListId;


        //                context.MasterTrxes.Add(oMasterTrx);
        //                int MasterTrxId = oMasterTrx.MasterTrxId;






        //                //Loop........Start
        //                foreach (var item in lstLockboxPayment)
        //                {
        //                    //##Customer Side

        //                    Address _Address = context.Addresses.Where(a => a.TypeListId == 1 && a.ContactTypeListId == 1 && a.ClassId == 1).FirstOrDefault();
        //                    TaxRate _TaxRate = context.TaxRates.Where(t => t.AddressId == _Address.AddressId).FirstOrDefault();

        //                    //--GET TRANSACTION NUMBER
        //                    string TransactionNumber = "";
        //                    //DECLARE @TTransactionNumber varchar(11) = ''
        //                    //EXEC[dbo].[portal_spGet_GetTrasactionNumber] @MasterTrxTypeListId = @TMasterTrxTypeListId,@RegionId = @LRegionId,@UserId = @CreatedBy,@TransactionNumber = @TTransactionNumber OUTPUT

        //                    Payment oPayment = new Payment();
        //                    oPayment.CheckAmount = item.CheckAmount;
        //                    oPayment.ClassId = item.CustomerId;
        //                    oPayment.CreatedBy = LoginUserId;
        //                    oPayment.CreatedDate = DateTime.Now;
        //                    oPayment.IsDelete = false;
        //                    oPayment.MasterTrxId = MasterTrxId;
        //                    oPayment.PaymentDescription = "";
        //                    oPayment.PaymentMethodListId = 4;
        //                    oPayment.PaymentNo = item.ChaqueNumber;
        //                    oPayment.PeriodId = _PeriodId;
        //                    oPayment.RegionId = _RegionId;
        //                    oPayment.TransactionDate = _TrasactionDate;
        //                    oPayment.TransactionNumber = TransactionNumber;
        //                    oPayment.TransactionStatusListId = item.TransactionStatusListId;
        //                    oPayment.TypeListId = 1;

        //                    context.Payments.Add(oPayment);
        //                    int PaymentId = oPayment.PaymentId;







        ////SELECT MTD.MasterTrxDetailId,MTD.[MasterTrxId],MTD.[LineNo], CustomerId,OriginalSubtotal,OriginalTax,OriginalTotal,BalanceSubtotal,BalanceTax,BalanceTotal 
        ////FROM  [dbo].[MasterTrxDetail] MTD WITH(NOLOCK)  
        ////INNER JOIN [dbo].[Invoice] INV WITH(NOLOCK) ON MTD.MasterTrxId =INV.MasterTrxId AND INV.InvoiceId = @LInvoiceId AND SourceTypeListId IN (1,2)
        ////INNER JOIN (SELECT InvoiceId,[LineNo],CustomerId,OriginalSubtotal,OriginalTax,OriginalTotal,BalanceSubtotal,BalanceTax,BalanceTotal 
        ////FROM dbo.fn_GetInvoiceLineItemBalanceForRegion(@LRegionId,@LInvoiceId)) AS IB 
        ////ON IB.[LineNo]= MTD.[LineNo] AND IB.InvoiceId=MTD.InvoiceId AND MTD.MasterTrxTypeListId IN (1,5)





        //                    //if(item.Items.Count==0)
        //                    //{


        //                    //}
        //                    //else
        //                    //{

        //                    //}





        //                    //Payment
        //                    //MasterTrxDetail
        //                    //MasterTrxTax

        //                    //Overflow {OverPayment}


        //                    //##Franchisee Side

        //                    //PaymentBillingFranchisee
        //                    //MasterTrxDetail
        //                    //MasterTrxFeeDetail
        //                }
        //                //Loop........End

        //                //Checkbook





        //                //if (TransactionStatusListId == 6)
        //                //{

        //                //    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
        //                //    {
        //                //        var parmas = new DynamicParameters();
        //                //        parmas.Add("@ChaqueNumber", ChaqueNumber);
        //                //        parmas.Add("@InvoiceId", InvoiceId);
        //                //        parmas.Add("@CustomerId", CustomerId);
        //                //        parmas.Add("@InvoiceAmount", InvoiceAmount);
        //                //        parmas.Add("@ApplyAmount", ApplyAmount);
        //                //        parmas.Add("@OverflowAmount", OverflowAmount);
        //                //        parmas.Add("@PaymentMethodListId", PaymentMethodListId);// --1 Check, 2 Credit Card, 3 EFT, 4 Lockbox                        
        //                //        parmas.Add("@TransactionStatusListId", TransactionStatusListId);  //--6  Paid, 7 Paid Partial
        //                //        if (TRDate != null)
        //                //            parmas.Add("@TransactionDate", TRDate);
        //                //        parmas.Add("@CreatedBy", LoginUserId);
        //                //        parmas.Add("@CheckAmount", TCheckAmount);



        //                //        using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_LockboxPayment", parmas, commandType: CommandType.StoredProcedure))
        //                //        //using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_Payment", parmas, commandType: CommandType.StoredProcedure))
        //                //        {
        //                //            if (multipleresult != null)
        //                //            {
        //                //                return true;
        //                //            }
        //                //        }
        //                //    }

        //                //    //context.portal_spCreate_AR_Payment(ChaqueNumber, InvoiceId, CustomerId, InvoiceAmount, ApplyAmount, PaymentMethodListId, TransactionStatusListId, LoginUserId);
        //                //}
        //                //else
        //                //{


        //                //    var PaymentItemDT = new DataTable();

        //                //    PaymentItemDT.Columns.Add("Id", typeof(int));
        //                //    PaymentItemDT.Columns.Add("InvoiceId", typeof(int));
        //                //    PaymentItemDT.Columns.Add("BillPayId", typeof(int));
        //                //    PaymentItemDT.Columns.Add("LineNumber", typeof(int));
        //                //    PaymentItemDT.Columns.Add("CustomerId", typeof(int));
        //                //    PaymentItemDT.Columns.Add("FranchiseeId", typeof(int));
        //                //    PaymentItemDT.Columns.Add("ApplyAmount", typeof(decimal));
        //                //    PaymentItemDT.Columns.Add("IsCustomerSide", typeof(bool));

        //                //    int fn = 0;
        //                //    foreach (PartialLockboxPaymentItemViewModel o in lstPartialLockboxPaymentItem)
        //                //    {
        //                //        fn++;
        //                //        PaymentItemDT.Rows.Add(fn, o.InvoiceId, o.BillPayId, o.LineNumber, o.CustomerId, o.FranchiseeId, o.ApplyAmount, o.IsCustomerSide);
        //                //    }
        //                //    if (lstPartialLockboxPaymentItem.Count == 0)
        //                //    {
        //                //        PaymentItemDT.Rows.Add(1, InvoiceId, 0, 1, CustomerId, 0, ApplyAmount, true);
        //                //        PaymentItemDT.Rows.Add(1, InvoiceId, 0, 1, CustomerId, 0, ApplyAmount, false);
        //                //    }

        //                //    List<SqlParameter> lstSqlParameter = new List<SqlParameter>();

        //                //    lstSqlParameter.Add(new SqlParameter("@ChaqueNumber", ChaqueNumber));
        //                //    lstSqlParameter.Add(new SqlParameter("@InvoiceId", InvoiceId));
        //                //    lstSqlParameter.Add(new SqlParameter("@CustomerId", CustomerId));
        //                //    lstSqlParameter.Add(new SqlParameter("@InvoiceAmount", InvoiceAmount));
        //                //    lstSqlParameter.Add(new SqlParameter("@ApplyAmount", ApplyAmount));
        //                //    lstSqlParameter.Add(new SqlParameter("@PaymentMethodListId", PaymentMethodListId));
        //                //    lstSqlParameter.Add(new SqlParameter("@TransactionStatusListId", TransactionStatusListId));
        //                //    lstSqlParameter.Add(new SqlParameter("@CreatedBy", LoginUserId));
        //                //    if (TRDate != null)
        //                //        lstSqlParameter.Add(new SqlParameter("@TransactionDate", TRDate));
        //                //    lstSqlParameter.Add(new SqlParameter("@CheckAmount", TCheckAmount));
        //                //    var parameter = new SqlParameter("@PaymentItems", SqlDbType.Structured);
        //                //    parameter.Value = PaymentItemDT;
        //                //    parameter.TypeName = "dbo.PartialLockboxPaymentItem";
        //                //    lstSqlParameter.Add(parameter);



        //                //    var reft = context.Database.ExecuteSqlCommand(GenerateCommandText("dbo.portal_spCreate_AR_PaymentPartial", lstSqlParameter.ToArray()), lstSqlParameter.ToArray());
        //                //}


        //                return true;
        //            }
        //        }


        public decimal GetCustomerCreditBalance(int customerId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var result = context.fn_GetCustomerCreditBalance(customerId).FirstOrDefault();
                if (result == null) return 0;

                return (decimal)result.Balance;
            }
        }

        public bool InsertLockboxEDIProcessed(int LockboxId, string ChaqueNumber, int InvoiceId, int CustomerId, decimal ApplyAmount)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@LockboxEDIId", LockboxId);
                    parmas.Add("@CheckNumber", ChaqueNumber);
                    parmas.Add("@CustomerId", CustomerId);
                    parmas.Add("@InvoiceId", InvoiceId);
                    parmas.Add("@ApplyAmount", ApplyAmount);

                    using (var multipleresult = conn.QueryMultiple("dbo.portal_spCreate_AR_InsertLockboxEDIProcessed", parmas, commandType: CommandType.StoredProcedure))
                    {
                        if (multipleresult != null)
                        {
                            return true;
                        }
                    }
                }
                return true;
            }
        }

        public IEnumerable<PaymentListViewModel> GetPaymentList(string regionIds = "", DateTime? from = null, DateTime? to = null, int month = 0, int year = 0)
        {
            List<PaymentListViewModel> lstPaymentList = new List<PaymentListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", regionIds ?? SelectedRegionId.ToString());
                parmas.Add("@FromDate", from);
                parmas.Add("@ToDate", to);
                parmas.Add("@BillMonth", month);
                parmas.Add("@BillYear", year);

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGetPaymentList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstPaymentList = multipleresult.Read<PaymentListViewModel>().ToList();
                    }
                }

                return lstPaymentList;
            }
            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{
            //    var list = context.portal_spGetPaymentList(regionIds ?? SelectedRegionId.ToString(), from, to, month, year).ToList();
            //    return list;
            //}
        }
        public IEnumerable<portal_spGet_AR_PendingPaymentList_Result> GetPendingPaymentList(string regionIds = "", DateTime? from = null, DateTime? to = null)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var list = context.portal_spGet_AR_PendingPaymentList(regionIds ?? SelectedRegionId.ToString(), from, to).ToList();
                return list;
            }
        }
        public IEnumerable<portal_spGet_AR_PendingPaymentList_Result> GetPendingtempPaymentList(string regionIds = "", DateTime? from = null, DateTime? to = null)
        {
            List<portal_spGet_AR_PendingPaymentList_Result> lstResult = new List<portal_spGet_AR_PendingPaymentList_Result>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", regionIds);
                parmas.Add("@FromDate", from);
                parmas.Add("@ToDate", to);

                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_PendingPaymentTempList", parmas, commandType: CommandType.StoredProcedure))
                {
                    lstResult = multipleresult.Read<portal_spGet_AR_PendingPaymentList_Result>().ToList();
                }
            }
            return lstResult;

            //using (jkDatabaseEntities context = new jkDatabaseEntities())
            //{
            //    var list = context.portal_spGet_AR_PendingPaymentList(regionIds ?? SelectedRegionId.ToString(), from, to).ToList();
            //    return list;
            //}
        }

        public List<ARCustomerWithCreditListViewModel> GetCreditList(int? regionId, DateTime fromdate, DateTime todate, string searchtext, bool closed, bool consolidated)
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<ARCustomerWithCreditListViewModel> lstARCustomerListView = new List<ARCustomerWithCreditListViewModel>();

                List<portal_spGet_AR_CustomerWithCreditList_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_CustomerWithCreditList(regionId ?? SelectedRegionId, fromdate, todate, searchtext, closed, consolidated).ToList();

                foreach (portal_spGet_AR_CustomerWithCreditList_Result o in lstARInvoiceListViewModel)
                {
                    lstARCustomerListView.Add(new ARCustomerWithCreditListViewModel()
                    {
                        CreditId = o.CreditId,
                        TransactionNumber = o.TransactionNumber,
                        CreditDate = (o.CreditDate == null ? "" : Convert.ToDateTime(o.CreditDate).ToString("MM/dd/yyyy")),
                        CustomerNo = o.CustomerNo,
                        CustomerName = o.CustomerName,
                        Reason = o.Reason,
                        Description = o.Description,
                        InvoiceNo = o.InvoiceNo,
                        InvAmount = o.InvAmount,
                        CrdAmount = o.CrdAmount,
                    });
                }

                return lstARCustomerListView;
            }
            // new BillRunSummaryDetailViewModel();
        }

        public List<ARCustomerWithCreditListViewModel> GetPendingCreditList(int? regionId, DateTime fromdate, DateTime todate, string searchtext, bool consolidated)
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                List<ARCustomerWithCreditListViewModel> lstARCustomerListView = new List<ARCustomerWithCreditListViewModel>();

                List<portal_spGet_AR_PendingCreditList_Result> lstARInvoiceListViewModel = context.portal_spGet_AR_PendingCreditList(regionId ?? SelectedRegionId, fromdate, todate, searchtext, consolidated).ToList();

                foreach (var o in lstARInvoiceListViewModel)
                {
                    lstARCustomerListView.Add(new ARCustomerWithCreditListViewModel()
                    {
                        CreditId = o.CreditId,
                        TransactionNumber = o.TransactionNumber,
                        CreditDate = (o.CreditDate == null ? "" : Convert.ToDateTime(o.CreditDate).ToString("MM/dd/yyyy")),
                        CustomerName = o.CustomerName,
                        Reason = o.Reason,
                        Description = o.Description,
                        InvoiceNo = o.InvoiceNo,
                        InvAmount = o.InvAmount,
                        CrdAmount = o.CrdAmount,
                        Status = o.Status,
                        CreatedBy = o.CreatedBy
                    });
                }

                return lstARCustomerListView;
            }
            // new BillRunSummaryDetailViewModel();
        }

        public List<ARCustomerWithCreditListViewModel> GetPendingCreditTempList(int? regionId, DateTime fromdate, DateTime todate, string searchtext, bool consolidated)
        {
            //int m = 0, int y = 0, string st = "", string fb = "", bool eo = false, string sv = "", string sb = "", bool cb = false
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                //ARInvoiceListViewModel

                using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@RegionId", regionId ?? SelectedRegionId);
                    parmas.Add("@FromDate", fromdate.ToString("MM/dd/yyyy"));
                    parmas.Add("@ToDate", todate.ToString("MM/dd/yyyy"));
                    parmas.Add("@SearchText", searchtext);
                    parmas.Add("@IncludeConsolidated", consolidated);
                    List<portal_spGet_AR_PendingCreditList_Result> lstARInvoiceListViewModel = conn.Query<portal_spGet_AR_PendingCreditList_Result>("dbo.portal_spGet_AR_PendingCreditTempList", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();

                    List<ARCustomerWithCreditListViewModel> lstARCustomerListView = new List<ARCustomerWithCreditListViewModel>();

                    foreach (var o in lstARInvoiceListViewModel)
                    {
                        CreditTemp creditTemp = context.CreditTemps.Single(p => p.CreditTempId == o.CreditId);
                        lstARCustomerListView.Add(new ARCustomerWithCreditListViewModel()
                        {
                            CreditId = o.CreditId,
                            TransactionNumber = o.TransactionNumber,
                            CreditDate = (o.CreditDate == null ? "" : Convert.ToDateTime(o.CreditDate).ToString("MM/dd/yyyy")),
                            CustomerName = o.CustomerName,
                            Reason = o.Reason,
                            Description = o.Description,
                            InvoiceNo = o.InvoiceNo,
                            InvAmount = o.InvAmount,
                            CrdAmount = o.CrdAmount,
                            Status = o.Status,
                            CreatedBy = o.CreatedBy,
                            IsTaxCredit = creditTemp.MasterTrxTypeListId == 58
                        });
                    }

                    return lstARCustomerListView;
                }



            }
            // new BillRunSummaryDetailViewModel();
        }

        public CustomerCreditDetailsPopupModel CustomerCreditDetailPopup(int CreditId)
        {
            CustomerCreditDetailsPopupModel CustomerCreditDetailsPopupModel = new CustomerCreditDetailsPopupModel();

            List<JKApi.Data.DAL.Address> lstAddress = new List<Address>();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                var Data = context.Credits.Where(f => f.CreditId == CreditId).FirstOrDefault();
                if (Data != null)
                {
                    CustomerCreditDetailsPopupModel.InvoiceDetailItems = context.vw_InvoiceContractDetailList.Where(o => o.InvoiceId == Data.InvoiceId).MapEnumerable<InvoiceContractDetailListViewModel, vw_InvoiceContractDetailList>().ToList();
                    /*Invoice Transaction history*/
                    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                    {
                        var parmas = new DynamicParameters();
                        parmas.Add("@InvoiceId", Data.InvoiceId);
                        CustomerCreditDetailsPopupModel.InvoiceTransactionHistoryList = conn.Query<InvoiceTransactionHistoryViewModel>("dbo.portal_spGet_C_InvoiceTransactionHistory", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();
                    }
                    if (CustomerCreditDetailsPopupModel.InvoiceTransactionHistoryList == null)
                        CustomerCreditDetailsPopupModel.InvoiceTransactionHistoryList = new List<InvoiceTransactionHistoryViewModel>();


                    int TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    int ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                    CustomerCreditDetailsPopupModel.CreditId = Data.CreditId;
                    CustomerCreditDetailsPopupModel.CreditNo = Data.TransactionNumber;
                    CustomerCreditDetailsPopupModel.CreditDate = Data.CreatedDate;
                    CustomerCreditDetailsPopupModel.Description = Data.CreditDescription;
                    var Customer = context.Customers.Where(w => w.CustomerId == Data.ClassId);
                    if (Customer != null && Customer.Count() > 0)
                    {
                        CustomerCreditDetailsPopupModel.CustomerNo = Customer.FirstOrDefault().CustomerNo;
                        CustomerCreditDetailsPopupModel.CustomerName = Customer.FirstOrDefault().Name;
                    }
                    var CusAddress = context.Addresses.Where(w => w.ClassId == Data.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusAddress != null && CusAddress.Count() > 0)
                    {
                        CustomerCreditDetailsPopupModel.Address1 = CusAddress.FirstOrDefault().Address1;
                        CustomerCreditDetailsPopupModel.Address2 = CusAddress.FirstOrDefault().Address2;
                        CustomerCreditDetailsPopupModel.City = CusAddress.FirstOrDefault().City;
                        CustomerCreditDetailsPopupModel.PostalCode = CusAddress.FirstOrDefault().PostalCode;

                    }
                    var CusPhone = context.Phones.Where(w => w.ClassId == Data.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusPhone != null && CusPhone.Count() > 0)
                    {
                        CustomerCreditDetailsPopupModel.Phone = CusPhone.FirstOrDefault().Phone1;
                        CustomerCreditDetailsPopupModel.PhoneExt = CusPhone.FirstOrDefault().PhoneExt;
                    }

                    List<CustomerCreditPaymentType> CustomerCreditPaymentTypeList = new List<CustomerCreditPaymentType>();

                    var MasterTrxDetails = context.MasterTrxDetails.Where(w => w.InvoiceId == Data.InvoiceId && w.MasterTrxTypeListId == (int)JKApi.Business.Enumeration.MasterTrxTypeList.CustomerInvoice);
                    if (MasterTrxDetails != null && MasterTrxDetails.Count() > 0)
                    {
                        CustomerCreditPaymentType CustomerCreditPaymentType1 = new CustomerCreditPaymentType();
                        CustomerCreditPaymentType1.Type = "Invoice Customer";
                        var Invoice = context.Invoices.Where(f => f.InvoiceId == Data.InvoiceId);
                        if (Invoice != null && Invoice.Count() > 0)
                        {
                            CustomerCreditPaymentType1.Date = Invoice.FirstOrDefault().InvoiceDate;
                            CustomerCreditPaymentType1.Number = Invoice.FirstOrDefault().InvoiceNo;
                            CustomerCreditPaymentType1.Description = Invoice.FirstOrDefault().InvoiceDescription;
                        }
                        CustomerCreditPaymentType1.Amount = (MasterTrxDetails.FirstOrDefault().Total.HasValue ? MasterTrxDetails.FirstOrDefault().Total.Value : 0);
                        CustomerCreditPaymentTypeList.Add(CustomerCreditPaymentType1);
                    }
                    var MasterTrxDetails2 = context.MasterTrxDetails.Where(w => w.InvoiceId == Data.InvoiceId && w.MasterTrxTypeListId == (int)JKApi.Business.Enumeration.MasterTrxTypeList.CustomerCredit);
                    if (MasterTrxDetails2 != null && MasterTrxDetails2.Count() > 0)
                    {
                        CustomerCreditDetailsPopupModel.CreditAmount = (MasterTrxDetails2.FirstOrDefault().Total.HasValue ? MasterTrxDetails2.FirstOrDefault().Total.Value : 0);
                        CustomerCreditPaymentType CustomerCreditPaymentType2 = new CustomerCreditPaymentType();
                        CustomerCreditPaymentType2.Type = "Invoice Credit Customer";
                        CustomerCreditPaymentType2.Date = MasterTrxDetails2.FirstOrDefault().CreatedDate;
                        CustomerCreditPaymentType2.Number = Data.TransactionNumber;
                        CustomerCreditPaymentType2.Description = Data.CreditDescription;
                        CustomerCreditPaymentType2.Amount = (MasterTrxDetails2.FirstOrDefault().Total.HasValue ? MasterTrxDetails2.FirstOrDefault().Total.Value : 0);
                        CustomerCreditPaymentType2.ExtendedPrice = (MasterTrxDetails2.FirstOrDefault().ExtendedPrice.HasValue ? MasterTrxDetails2.FirstOrDefault().ExtendedPrice.Value : 0);
                        CustomerCreditPaymentType2.TotalTax = (MasterTrxDetails2.FirstOrDefault().TotalTax.HasValue ? MasterTrxDetails2.FirstOrDefault().TotalTax.Value : 0);
                        CustomerCreditPaymentTypeList.Add(CustomerCreditPaymentType2);
                    }
                    CustomerCreditDetailsPopupModel.CustomerCreditPaymentTypeList = CustomerCreditPaymentTypeList;
                }

                return CustomerCreditDetailsPopupModel;
            }

            //return CustomerCreditDetailsPopupModel;

        }

        public PaymentDetailsPopupModel PaymentDetailPopup(int paymentId, bool IsPending = false)
        {

            PaymentDetailsPopupModel PaymentDetailsPopupModel = new PaymentDetailsPopupModel();

            List<JKApi.Data.DAL.Address> lstAddress = new List<Address>();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                //Header Detail 
                var Data = context.Payments.Where(f => f.PaymentId == paymentId).FirstOrDefault();
                if (Data != null)
                {
                    int TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    int ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                    PaymentDetailsPopupModel.PaymentId = Data.PaymentId;
                    PaymentDetailsPopupModel.PaymentNo = Data.PaymentNo;
                    PaymentDetailsPopupModel.PaymentDate = Data.TransactionDate;
                    PaymentDetailsPopupModel.TransactionNumber = Data.TransactionNumber;

                    var pymt = context.PaymentMethodLists.Where(f => f.PaymentMethodListId == Data.PaymentMethodListId).FirstOrDefault();
                    PaymentDetailsPopupModel.PaymentType = (pymt != null ? pymt.Name : string.Empty);
                    PaymentDetailsPopupModel.PaymentTypeId = pymt != null ? pymt.PaymentMethodListId : 0;
                    PaymentDetailsPopupModel.Note = Data.PaymentDescription;

                    //var ptr = context.TransactionStatusLists.Where(f => f.TransactionStatusListId == Data.TransactionStatusListId);
                    //PaymentDetailsPopupModel.Note = (ptr != null ? ptr.FirstOrDefault().Name : string.Empty);


                    var Customer = context.Customers.Where(w => w.CustomerId == Data.ClassId);
                    if (Customer != null && Customer.Count() > 0)
                    {
                        PaymentDetailsPopupModel.CustomerId = Customer.FirstOrDefault().CustomerId;
                        PaymentDetailsPopupModel.CustomerNo = Customer.FirstOrDefault().CustomerNo;
                        PaymentDetailsPopupModel.CustomerName = Customer.FirstOrDefault().Name;
                    }
                    var CusAddress = context.Addresses.Where(w => w.ClassId == Data.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusAddress != null && CusAddress.Count() > 0)
                    {
                        PaymentDetailsPopupModel.Address1 = CusAddress.FirstOrDefault().Address1;
                        PaymentDetailsPopupModel.Address2 = CusAddress.FirstOrDefault().Address2;
                        PaymentDetailsPopupModel.City = CusAddress.FirstOrDefault().City;
                        PaymentDetailsPopupModel.PostalCode = CusAddress.FirstOrDefault().PostalCode;

                    }
                    var CusPhone = context.Phones.Where(w => w.ClassId == Data.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusPhone != null && CusPhone.Count() > 0)
                    {
                        PaymentDetailsPopupModel.Phone = CusPhone.FirstOrDefault().Phone1;
                        PaymentDetailsPopupModel.PhoneExt = CusPhone.FirstOrDefault().PhoneExt;
                    }



                    List<PaymentDetailType> PaymentDetailTypeList = new List<PaymentDetailType>();




                    //PaymentDetailsPopupModel.PaymentDetailType;                    
                    //PaymentDetailsPopupModel.StateName;
                    //PaymentDetailsPopupModel.PaymentAmount;


                    //List<InvoiceTransactionHistoryViewModel> lstInvoiceTransactionHistoryViewModel =  new List<InvoiceTransactionHistoryViewModel>()
                    int _oINV = 0;
                    var MasterTrxDetails = context.MasterTrxDetails.Where(w => w.MasterTrxId == Data.MasterTrxId);
                    if (MasterTrxDetails != null && MasterTrxDetails.Count() > 0)
                    {
                        PaymentDetailsPopupModel.PaymentAmount = (decimal)(MasterTrxDetails.Where(p => p.AmountTypeListId == 1).ToList().Sum(c => c.Total));
                        PaymentDetailType PaymentDetailType2 = new PaymentDetailType();
                        PaymentDetailType2.Type = "Payment";
                        PaymentDetailType2.Number = Data.PaymentNo;
                        PaymentDetailType2.TransactionNumber = Data.TransactionNumber;
                        PaymentDetailType2.AmountTypeListId = 1;
                        PaymentDetailType2.Date = Data.TransactionDate;
                        PaymentDetailType2.Description = Data.PaymentDescription;
                        PaymentDetailType2.Amount = PaymentDetailsPopupModel.PaymentAmount;
                        PaymentDetailTypeList.Add(PaymentDetailType2);

                        PaymentDetailsPopupModel.PaymentDetailInvoiceTransactionHistoryList = new List<PaymentDetailInvoiceTransactionHistory>();
                        PaymentDetailInvoiceTransactionHistory oPaymentDetailInvoiceTransactionHistory = new PaymentDetailInvoiceTransactionHistory();


                        foreach (var ob in MasterTrxDetails)
                        {
                            var Data1 = context.Invoices.Where(f => f.InvoiceId == ob.InvoiceId).FirstOrDefault();
                            var CUData1 = context.Customers.SingleOrDefault(f => f.CustomerId == Data1.ClassId);

                            PaymentDetailType PaymentDetailType1 = new PaymentDetailType();
                            PaymentDetailType1.Type = ob.MasterTrxTypeListId == 17 ? "OVERPAID" : "Invoice";

                            PaymentDetailType1.CustomerId = CUData1?.CustomerId;
                            PaymentDetailType1.CustomerNo = CUData1?.CustomerNo;
                            PaymentDetailType1.CustomerName = CUData1?.Name;

                            PaymentDetailType1.TransactionNumber = Data1.InvoiceNo;

                            PaymentDetailType1.Number = Data1.InvoiceNo;
                            PaymentDetailType1.TransactionNumber = Data1.InvoiceNo;
                            PaymentDetailType1.AmountTypeListId = ob.AmountTypeListId;
                            PaymentDetailType1.Date = Data1.InvoiceDate;
                            PaymentDetailType1.Description = Data1.InvoiceDescription;
                            PaymentDetailType1.Amount = (ob.Total.HasValue ? ob.Total.Value : 0);
                            PaymentDetailType1.MasterTrxTypeListId = (int)ob.MasterTrxTypeListId;
                            PaymentDetailType1.InvoiceId = (int)Data1.InvoiceId;
                            if (Data.TransactionStatusListId == 3)
                                PaymentDetailType1.BalanceAmount = (decimal)(ob.MasterTrxTypeListId == 17 ? 0 : context.fn_GetInvoiceBalance((int)Data1.InvoiceId).FirstOrDefault()?.BalanceTotal);
                            else
                                PaymentDetailType1.BalanceAmount = (decimal)(ob.MasterTrxTypeListId == 17 ? 0 : context.fn_GetInvoiceBalance((int)Data1.InvoiceId).FirstOrDefault()?.BalanceTotal) - (ob.Total.HasValue ? ob.Total.Value : 0);
                            PaymentDetailTypeList.Add(PaymentDetailType1);

                            if (!IsPending)
                            {
                                oPaymentDetailInvoiceTransactionHistory = new PaymentDetailInvoiceTransactionHistory();
                                if (_oINV != Data1.InvoiceId)
                                {
                                    _oINV = Data1.InvoiceId;
                                    /* Transaction History */

                                    //PaymentDetailsPopupModel.InvoiceDetailItems = context.vw_InvoiceContractDetailList.Where(i => i.InvoiceId == Data1.InvoiceId).MapEnumerable<InvoiceContractDetailListViewModel, vw_InvoiceContractDetailList>().ToList();
                                    /*Invoice Transaction history*/
                                    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                                    {
                                        var parmas = new DynamicParameters();
                                        parmas.Add("@InvoiceId", Data1.InvoiceId);

                                        var resultTRH = conn.Query<InvoiceTransactionHistoryViewModel>("dbo.portal_spGet_C_InvoiceTransactionHistory", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();

                                        if (resultTRH != null)
                                        {
                                            oPaymentDetailInvoiceTransactionHistory.InvoiceTransactionHistoryList = resultTRH;
                                            oPaymentDetailInvoiceTransactionHistory.PaymentId = Data.PaymentId;
                                            oPaymentDetailInvoiceTransactionHistory.InvoiceId = (int)ob.InvoiceId;
                                            oPaymentDetailInvoiceTransactionHistory.InvoiceNo = Data1.InvoiceNo;
                                        }
                                    }
                                    PaymentDetailsPopupModel.PaymentDetailInvoiceTransactionHistoryList.Add(oPaymentDetailInvoiceTransactionHistory);
                                }

                            }


                        }

                        var mOverpaidTrxDetail = context.MasterTrxDetails.Where(w => (w.HeaderId == PaymentDetailsPopupModel.PaymentId && w.MasterTrxTypeListId == 17)).FirstOrDefault();
                        List<PaymentDetailType> FinalPaymentDetailTypeList = new List<PaymentDetailType>();


                        if (mOverpaidTrxDetail != null)
                        {
                            foreach (var row in PaymentDetailTypeList)
                            {

                                if (row.InvoiceId == mOverpaidTrxDetail.InvoiceId && row.MasterTrxTypeListId == 2)
                                {
                                    row.BalanceAmount = (decimal)mOverpaidTrxDetail.Total;
                                }
                                else
                                {
                                    row.BalanceAmount = 0.00m;
                                }

                            }
                        }

                        PaymentDetailTypeList.RemoveAll(x => (x.BalanceAmount == 0.00m && x.MasterTrxTypeListId == 17));


                    }
                    PaymentDetailsPopupModel.PaymentDetailType = PaymentDetailTypeList;

                    //var MasterTrxDetails2 = context.MasterTrxDetails.Where(w => w.HeaderId == paymentId && w.MasterTrxTypeListId == 2);
                    //if (MasterTrxDetails2 != null && MasterTrxDetails2.Count() > 0)
                    //{
                    //    PaymentDetailsPopupModel.PaymentAmount = (MasterTrxDetails2.FirstOrDefault().Total.HasValue ? MasterTrxDetails2.FirstOrDefault().Total.Value : 0);
                    //    PaymentDetailType PaymentDetailType2 = new PaymentDetailType();
                    //    PaymentDetailType2.Type = "Payment";
                    //    PaymentDetailType2.Number = Data.PaymentNo;
                    //    PaymentDetailType2.AmountTypeListId = MasterTrxDetails2.FirstOrDefault().AmountTypeListId;
                    //    PaymentDetailType2.Date = MasterTrxDetails2.FirstOrDefault().Transactiondate;
                    //    PaymentDetailType2.Description = MasterTrxDetails2.FirstOrDefault().DetailDescription;
                    //    PaymentDetailType2.Amount = (MasterTrxDetails2.FirstOrDefault().Total.HasValue ? MasterTrxDetails2.FirstOrDefault().Total.Value : 0);
                    //    PaymentDetailTypeList.Add(PaymentDetailType2);
                    //}
                    //PaymentDetailsPopupModel.PaymentDetailType = PaymentDetailTypeList;
                }

                return PaymentDetailsPopupModel;
            }
        }

        public PaymentDetailsPopupModel PaymentDetailPopupFromTemp(int paymentId, bool IsPending = false)
        {

            PaymentDetailsPopupModel PaymentDetailsPopupModel = new PaymentDetailsPopupModel();

            List<JKApi.Data.DAL.Address> lstAddress = new List<Address>();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                //Header Detail 
                var Data = context.PaymentTemps.Where(f => f.PaymentTempId == paymentId && f.MasterTrxTypeListId != 7).FirstOrDefault();
                if (Data != null)
                {
                    int TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    int ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                    PaymentDetailsPopupModel.PaymentId = Data.PaymentTempId;
                    PaymentDetailsPopupModel.PaymentNo = Data.PaymentNo;
                    PaymentDetailsPopupModel.PaymentDate = Data.TransactionDate;
                    PaymentDetailsPopupModel.TransactionNumber = Data.TransactionNumber;

                    var pymt = context.PaymentMethodLists.Where(f => f.PaymentMethodListId == Data.PaymentMethodListId).FirstOrDefault();
                    PaymentDetailsPopupModel.PaymentType = (pymt != null ? pymt.Name : string.Empty);
                    PaymentDetailsPopupModel.PaymentTypeId = pymt != null ? pymt.PaymentMethodListId : 0;
                    PaymentDetailsPopupModel.Note = Data.PaymentDescription;

                    //var ptr = context.TransactionStatusLists.Where(f => f.TransactionStatusListId == Data.TransactionStatusListId);
                    //PaymentDetailsPopupModel.Note = (ptr != null ? ptr.FirstOrDefault().Name : string.Empty);


                    var Customer = context.Customers.Where(w => w.CustomerId == Data.ClassId);
                    if (Customer != null && Customer.Count() > 0)
                    {
                        PaymentDetailsPopupModel.CustomerId = Customer.FirstOrDefault().CustomerId;
                        PaymentDetailsPopupModel.CustomerNo = Customer.FirstOrDefault().CustomerNo;
                        PaymentDetailsPopupModel.CustomerName = Customer.FirstOrDefault().Name;
                    }
                    var CusAddress = context.Addresses.Where(w => w.ClassId == Data.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusAddress != null && CusAddress.Count() > 0)
                    {
                        PaymentDetailsPopupModel.Address1 = CusAddress.FirstOrDefault().Address1;
                        PaymentDetailsPopupModel.Address2 = CusAddress.FirstOrDefault().Address2;
                        PaymentDetailsPopupModel.City = CusAddress.FirstOrDefault().City;
                        PaymentDetailsPopupModel.PostalCode = CusAddress.FirstOrDefault().PostalCode;

                    }
                    var CusPhone = context.Phones.Where(w => w.ClassId == Data.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusPhone != null && CusPhone.Count() > 0)
                    {
                        PaymentDetailsPopupModel.Phone = CusPhone.FirstOrDefault().Phone1;
                        PaymentDetailsPopupModel.PhoneExt = CusPhone.FirstOrDefault().PhoneExt;
                    }



                    List<PaymentDetailType> PaymentDetailTypeList = new List<PaymentDetailType>();




                    //PaymentDetailsPopupModel.PaymentDetailType;                    
                    //PaymentDetailsPopupModel.StateName;
                    //PaymentDetailsPopupModel.PaymentAmount;


                    //List<InvoiceTransactionHistoryViewModel> lstInvoiceTransactionHistoryViewModel =  new List<InvoiceTransactionHistoryViewModel>()
                    int _oINV = 0;
                    var MasterTrxDetails = context.PaymentTempDetails.Where(w => w.MasterTrxId == Data.MasterTrxId && w.MasterTrxTypeListId != 7);
                    if (MasterTrxDetails != null && MasterTrxDetails.Count() > 0)
                    {
                        PaymentDetailsPopupModel.PaymentAmount = (decimal)(MasterTrxDetails.Where(p => p.AmountTypeListId == 1).ToList().Sum(c => c.Total));
                        PaymentDetailType PaymentDetailType2 = new PaymentDetailType();
                        PaymentDetailType2.Type = "Payment";
                        PaymentDetailType2.Number = Data.PaymentNo;
                        PaymentDetailType2.TransactionNumber = Data.TransactionNumber;
                        PaymentDetailType2.AmountTypeListId = 1;
                        PaymentDetailType2.Date = Data.TransactionDate;
                        PaymentDetailType2.Description = Data.PaymentDescription;
                        PaymentDetailType2.Amount = PaymentDetailsPopupModel.PaymentAmount;
                        PaymentDetailTypeList.Add(PaymentDetailType2);

                        PaymentDetailsPopupModel.PaymentDetailInvoiceTransactionHistoryList = new List<PaymentDetailInvoiceTransactionHistory>();
                        PaymentDetailInvoiceTransactionHistory oPaymentDetailInvoiceTransactionHistory = new PaymentDetailInvoiceTransactionHistory();


                        foreach (var ob in MasterTrxDetails)
                        {
                            var Data1 = context.Invoices.Where(f => f.InvoiceId == ob.InvoiceId).FirstOrDefault();
                            var CUData1 = context.Customers.SingleOrDefault(f => f.CustomerId == Data1.ClassId);
                            List<MasterTrxDetail> _invMDAmount = context.MasterTrxDetails.Where(f => f.InvoiceId == Data1.InvoiceId && (f.MasterTrxTypeListId == 1 || f.MasterTrxTypeListId == 5)).ToList();

                            PaymentDetailType PaymentDetailType1 = new PaymentDetailType();
                            PaymentDetailType1.Type = ob.MasterTrxTypeListId == 17 ? "OVERPAID" : "Invoice";

                            PaymentDetailType1.CustomerId = CUData1?.CustomerId;
                            PaymentDetailType1.CustomerNo = CUData1?.CustomerNo;
                            PaymentDetailType1.CustomerName = CUData1?.Name;

                            PaymentDetailType1.TransactionNumber = Data1.InvoiceNo;

                            PaymentDetailType1.Number = Data1.InvoiceNo;
                            PaymentDetailType1.TransactionNumber = Data1.InvoiceNo;
                            PaymentDetailType1.AmountTypeListId = ob.AmountTypeListId;
                            PaymentDetailType1.Date = Data1.InvoiceDate;
                            PaymentDetailType1.Description = Data1.InvoiceDescription;
                            PaymentDetailType1.Amount = (ob.Total.HasValue ? ob.Total.Value : 0);
                            PaymentDetailType1.MasterTrxTypeListId = (int)ob.MasterTrxTypeListId;
                            PaymentDetailType1.InvoiceId = (int)Data1.InvoiceId;
                            PaymentDetailType1.InvoiceAmount = (decimal)_invMDAmount.Sum(i => i.Total);
                            if (Data.TransactionStatusListId == 3)
                                PaymentDetailType1.BalanceAmount = (decimal)(ob.MasterTrxTypeListId == 17 ? 0 : context.fn_GetInvoiceBalance((int)Data1.InvoiceId).FirstOrDefault()?.BalanceTotal);
                            else
                                PaymentDetailType1.BalanceAmount = (decimal)(ob.MasterTrxTypeListId == 17 ? 0 : context.fn_GetInvoiceBalance((int)Data1.InvoiceId).FirstOrDefault()?.BalanceTotal) - (ob.Total.HasValue ? ob.Total.Value : 0);
                            PaymentDetailTypeList.Add(PaymentDetailType1);

                            if (!IsPending)
                            {
                                oPaymentDetailInvoiceTransactionHistory = new PaymentDetailInvoiceTransactionHistory();
                                if (_oINV != Data1.InvoiceId)
                                {
                                    _oINV = Data1.InvoiceId;
                                    /* Transaction History */

                                    //PaymentDetailsPopupModel.InvoiceDetailItems = context.vw_InvoiceContractDetailList.Where(i => i.InvoiceId == Data1.InvoiceId).MapEnumerable<InvoiceContractDetailListViewModel, vw_InvoiceContractDetailList>().ToList();
                                    /*Invoice Transaction history*/
                                    using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
                                    {
                                        var parmas = new DynamicParameters();
                                        parmas.Add("@InvoiceId", Data1.InvoiceId);

                                        var resultTRH = conn.Query<InvoiceTransactionHistoryViewModel>("dbo.portal_spGet_C_InvoiceTransactionHistory", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();

                                        if (resultTRH != null)
                                        {
                                            oPaymentDetailInvoiceTransactionHistory.InvoiceTransactionHistoryList = resultTRH;
                                            oPaymentDetailInvoiceTransactionHistory.PaymentId = Data.PaymentTempId;
                                            oPaymentDetailInvoiceTransactionHistory.InvoiceId = (int)ob.InvoiceId;
                                            oPaymentDetailInvoiceTransactionHistory.InvoiceNo = Data1.InvoiceNo;
                                        }
                                    }
                                    PaymentDetailsPopupModel.PaymentDetailInvoiceTransactionHistoryList.Add(oPaymentDetailInvoiceTransactionHistory);
                                }

                            }


                        }

                        var mOverpaidTrxDetail = context.PaymentTempDetails.Where(w => (w.HeaderId == PaymentDetailsPopupModel.PaymentId && w.MasterTrxTypeListId == 17)).FirstOrDefault();
                        List<PaymentDetailType> FinalPaymentDetailTypeList = new List<PaymentDetailType>();


                        if (mOverpaidTrxDetail != null)
                        {
                            foreach (var row in PaymentDetailTypeList)
                            {

                                if (row.InvoiceId == mOverpaidTrxDetail.InvoiceId && row.MasterTrxTypeListId == 2)
                                {
                                    row.BalanceAmount = (decimal)mOverpaidTrxDetail.Total;
                                }
                                else
                                {
                                    row.BalanceAmount = 0.00m;
                                }

                            }
                        }

                        PaymentDetailTypeList.RemoveAll(x => (x.BalanceAmount == 0.00m && x.MasterTrxTypeListId == 17));


                    }
                    PaymentDetailsPopupModel.PaymentDetailType = PaymentDetailTypeList;

                    //var MasterTrxDetails2 = context.MasterTrxDetails.Where(w => w.HeaderId == paymentId && w.MasterTrxTypeListId == 2);
                    //if (MasterTrxDetails2 != null && MasterTrxDetails2.Count() > 0)
                    //{
                    //    PaymentDetailsPopupModel.PaymentAmount = (MasterTrxDetails2.FirstOrDefault().Total.HasValue ? MasterTrxDetails2.FirstOrDefault().Total.Value : 0);
                    //    PaymentDetailType PaymentDetailType2 = new PaymentDetailType();
                    //    PaymentDetailType2.Type = "Payment";
                    //    PaymentDetailType2.Number = Data.PaymentNo;
                    //    PaymentDetailType2.AmountTypeListId = MasterTrxDetails2.FirstOrDefault().AmountTypeListId;
                    //    PaymentDetailType2.Date = MasterTrxDetails2.FirstOrDefault().Transactiondate;
                    //    PaymentDetailType2.Description = MasterTrxDetails2.FirstOrDefault().DetailDescription;
                    //    PaymentDetailType2.Amount = (MasterTrxDetails2.FirstOrDefault().Total.HasValue ? MasterTrxDetails2.FirstOrDefault().Total.Value : 0);
                    //    PaymentDetailTypeList.Add(PaymentDetailType2);
                    //}
                    //PaymentDetailsPopupModel.PaymentDetailType = PaymentDetailTypeList;
                }

                return PaymentDetailsPopupModel;
            }
        }

        public bool ReversalPayment(int Id)
        {

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@PaymentId", Id);
                parmas.Add("@CreatedBy", LoginUserId);

                int retVal = conn.Execute("dbo.portal_spCreate_AR_ReversalPayment", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure);


            }

            return true;
        }

        public int UpdatePaymentDetailPopup(int Id, DateTime PaymentDate, int PaymentType, string PaymentNo, decimal Amount, string Note, bool onlyDetail, FullManualPaymentViewModel inputdata)
        {
            int retVal = Id;
            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();

            JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
            JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();

            customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(2, inputdata.RegionId);
            franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(7, inputdata.RegionId);

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var PR = context.Periods.SingleOrDefault(p => p.BillMonth == PaymentDate.Month && p.BillYear == PaymentDate.Year);

                //Header Table MasterTrxTemp                

                //Customer Side
                //Header Table PaymentTemp
                PaymentTemp custPayment = context.PaymentTemps.Where(f => f.PaymentTempId == Id).FirstOrDefault();

                PaymentTempMasterTrx custMasterTrx = context.PaymentTempMasterTrxes.SingleOrDefault(pm => pm.PaymentTempMasterTrxId == custPayment.MasterTrxId);
                custMasterTrx.TrxDate = PaymentDate;
                custMasterTrx.BillMonth = PR.BillMonth;
                custMasterTrx.BillYear = PR.BillYear;
                custMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                custMasterTrx.ModifiedBy = LoginUserId;
                custMasterTrx.ModifiedDate = DateTime.Now;
                context.SaveChanges();



                if (onlyDetail)
                {
                    List<PaymentTemp> lstcustPayment = context.PaymentTemps.Where(f => f.MasterTrxId == custMasterTrx.PaymentTempMasterTrxId).ToList();
                    foreach (var cItem in lstcustPayment)
                    {
                        cItem.TransactionDate = PaymentDate;
                        cItem.PaymentMethodListId = PaymentType;
                        cItem.PaymentNo = PaymentNo;
                        cItem.PaymentDescription = Note;
                        context.SaveChanges();

                        List<PaymentTemp> lstfrPaymentTemp = context.PaymentTemps.Where(f => f.PaymentId == cItem.PaymentTempId).ToList();
                        foreach (var iTemF in lstfrPaymentTemp)
                        {
                            iTemF.TransactionDate = PaymentDate;
                            iTemF.PaymentMethodListId = PaymentType;
                            iTemF.PaymentNo = PaymentNo;
                            iTemF.PaymentDescription = Note;
                            context.SaveChanges();
                        }
                    }

                }
                else
                {
                    List<int> lstCust = inputdata.Invoices.Select(g => g.InvoiceCustomerId).Distinct().ToList<int>();
                    List<PaymentTemp> lstcustPayment = context.PaymentTemps.Where(f => f.MasterTrxId == custMasterTrx.PaymentTempMasterTrxId).ToList();
                    foreach (var cItem in lstcustPayment)
                    {
                        List<PaymentTempDetail> lstcustPaymentDetail = context.PaymentTempDetails.Where(f => f.HeaderId == cItem.PaymentTempId).ToList();
                        foreach (var iTem in lstcustPaymentDetail)
                        {
                            List<PaymentTempDetailTax> lstFPaymentDetailFee = context.PaymentTempDetailTaxes.Where(f => f.MasterTrxDetailId == iTem.PaymentTempDetailId).ToList();
                            context.PaymentTempDetailTaxes.RemoveRange(lstFPaymentDetailFee);
                            context.SaveChanges();

                        }
                        context.PaymentTempDetails.RemoveRange(lstcustPaymentDetail);
                        context.SaveChanges();


                        List<PaymentTemp> lstfrPaymentTemp = context.PaymentTemps.Where(f => f.PaymentId == cItem.PaymentTempId).ToList();
                        foreach (var iTemF in lstfrPaymentTemp)
                        {
                            List<PaymentTempMasterTrx> lstFPaymentTempMasterTrx = context.PaymentTempMasterTrxes.Where(f => f.PaymentTempMasterTrxId == iTemF.MasterTrxId).ToList();
                            context.PaymentTempMasterTrxes.RemoveRange(lstFPaymentTempMasterTrx);
                            context.SaveChanges();

                            List<PaymentTempDetail> lstFPaymentDetail = context.PaymentTempDetails.Where(f => f.HeaderId == cItem.PaymentTempId).ToList();
                            foreach (var iTemFD in lstcustPaymentDetail)
                            {
                                List<PaymentTempDetailFee> lstFPaymentDetailFee = context.PaymentTempDetailFees.Where(f => f.MasterTrxDetailId == iTemFD.PaymentTempDetailId).ToList();
                                context.PaymentTempDetailFees.RemoveRange(lstFPaymentDetailFee);
                                context.SaveChanges();
                            }
                            context.PaymentTempDetails.RemoveRange(lstFPaymentDetail);
                            context.SaveChanges();
                        }
                        context.PaymentTemps.RemoveRange(lstfrPaymentTemp);
                        context.SaveChanges();



                    }
                    context.PaymentTemps.RemoveRange(lstcustPayment);
                    context.SaveChanges();


                    //PaymentTempMasterTrx customerMasterTrx = new PaymentTempMasterTrx();
                    //customerMasterTrx.TypeListId = 1; // customer
                    //customerMasterTrx.ClassId = inputdata.CustomerId;
                    //customerMasterTrx.MasterTrxTypeListId = 2; // customer payment
                    //customerMasterTrx.TrxDate = inputdata.TransactionDate;
                    //customerMasterTrx.RegionId = inputdata.RegionId;
                    //customerMasterTrx.StatusId = 1; // open
                    //customerMasterTrx.BillMonth = PR.BillMonth;
                    //customerMasterTrx.BillYear = PR.BillYear;
                    //customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                    //customerMasterTrx.CreatedBy = LoginUserId;
                    //customerMasterTrx.CreatedDate = DateTime.Now;

                    ////customerMasterTrx.AccountTypeListId;              
                    ////customerMasterTrx.HeaderId;

                    //context.PaymentTempMasterTrxes.Add(customerMasterTrx);
                    //context.SaveChanges();

                    int InvIdT = 0;
                    foreach (var _oCustId in lstCust)
                    {

                        PaymentTemp customerPayment = new PaymentTemp();
                        customerPayment.MasterTrxId = custMasterTrx.PaymentTempMasterTrxId;
                        customerPayment.TypeListId = 1; // customer
                        customerPayment.ClassId = _oCustId;
                        customerPayment.RegionId = inputdata.RegionId;
                        customerPayment.PaymentMethodListId = inputdata.PaymentMethodListId;
                        customerPayment.PaymentNo = inputdata.ReferenceNo;
                        customerPayment.TransactionStatusListId = 1;// allPaidInFull ? 6 : 7; // paid : paid partial
                        customerPayment.TempTransactionStatusListId = 3;// allPaidInFull ? 6 : 7; // paid : paid partial
                        customerPayment.CheckAmount = inputdata.CreditAmount > 0 ? 0 : inputdata.PaymentAmount;

                        //GET INVOICE NUMBER FOR Payment Number 
                        InvIdT = inputdata.Invoices.Where(p => p.InvoiceCustomerId == _oCustId).FirstOrDefault().InvoiceId;
                        Invoice _invoiceOject = context.Invoices.Where(o => o.InvoiceId == InvIdT).FirstOrDefault();

                        customerPayment.TransactionNumber = "PMT" + customerTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", custMasterTrx.BillMonth) + (customerTransactionNumberConfigViewModel.LastNumber + 1).ToString();
                        // customerPayment.TransactionNumber = "PMT" + _invoiceOject.InvoiceNo.Trim(); // customerNextTrxNumber;

                        customerPayment.TransactionDate = inputdata.TransactionDate;
                        customerPayment.IsDelete = false;
                        customerPayment.CreatedBy = inputdata.CreatedBy;
                        customerPayment.CreatedBy = inputdata.CreatedBy;
                        customerPayment.ModifiedBy = inputdata.CreatedBy;
                        customerPayment.ModifiedDate = inputdata.CreatedDate;
                        customerPayment.PaymentDescription = inputdata.Notes;
                        customerPayment.PeriodId = (PR != null ? PR.PeriodId : 0);
                        customerPayment.CheckAmount = inputdata.PaymentAmount;
                        context.PaymentTemps.Add(customerPayment);
                        context.SaveChanges();

                        custMasterTrx.HeaderId = customerPayment.PaymentTempId;
                        context.SaveChanges();

                        retVal = customerPayment.PaymentTempId;

                        if (customerTransactionNumberConfigViewModel != null)
                        {
                            customerTransactionNumberConfigViewModel.LastNumber = customerTransactionNumberConfigViewModel.LastNumber + 1;
                            CompanySvc.SaveTransactionNumberConfig(customerTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                        }


                        decimal totalCustomerPayment = 0;
                        decimal totalCustomerTaxes = 0;

                        int custMasterTrxDetailId = 0;
                        foreach (MPInvoiceViewModel ivm in inputdata.Invoices.Where(p => p.InvoiceCustomerId == _oCustId))
                        {
                            if (ivm.InvoicePayment > 0)
                            {
                                foreach (ManualPaymentViewModel cvm in ivm.CustomerPayment.Payments)
                                {
                                    if (cvm.ExtendedPrice > 0)
                                    {
                                        PaymentTempDetail masterTrxDetail = new PaymentTempDetail();
                                        masterTrxDetail.MasterTrxId = custMasterTrx.PaymentTempMasterTrxId;
                                        masterTrxDetail.InvoiceId = ivm.InvoiceId;
                                        masterTrxDetail.LineNo = cvm.LineNo;
                                        masterTrxDetail.MasterTrxTypeListId = 2; // customer payment
                                        masterTrxDetail.HeaderId = customerPayment.PaymentTempId;
                                        masterTrxDetail.RegionId = inputdata.RegionId;
                                        masterTrxDetail.AmountTypeListId = 1; // credit
                                        masterTrxDetail.FeesDetail = false;
                                        masterTrxDetail.TaxDetail = true;
                                        masterTrxDetail.TotalTax = cvm.Tax;
                                        masterTrxDetail.Total = cvm.PaymentAmount;
                                        masterTrxDetail.ExtendedPrice = cvm.ExtendedPrice;
                                        masterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                        masterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                        masterTrxDetail.IsDelete = false;
                                        //masterTrxDetail.ServiceTypeListId =
                                        masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                                        masterTrxDetail.TypelistId = 1; // customer
                                        masterTrxDetail.ClassId = _oCustId;
                                        masterTrxDetail.Transactiondate = inputdata.TransactionDate;
                                        masterTrxDetail.BPPAdmin = 1;
                                        masterTrxDetail.AccountRebate = 1;
                                        masterTrxDetail.AccountRebate = 1;
                                        masterTrxDetail.Commission = false;
                                        masterTrxDetail.CommissionTotal = 0;
                                        masterTrxDetail.FRRevenues = false;
                                        masterTrxDetail.FRDeduction = false;
                                        masterTrxDetail.DetailDescription = inputdata.Notes;

                                        masterTrxDetail.UnitPrice = 0;
                                        masterTrxDetail.CPIPercentage = 0;
                                        masterTrxDetail.TotalFee = 0;

                                        masterTrxDetail.SourceId = 0;
                                        masterTrxDetail.SourceTypeListId = 0;
                                        masterTrxDetail.ServiceTypeListId = 0;
                                        //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                        masterTrxDetail.Quantity = 0;
                                        masterTrxDetail.CPIPercentage = 0;
                                        masterTrxDetail.TotalFee = 0;
                                        masterTrxDetail.Commission = false;
                                        masterTrxDetail.CommissionTotal = 0;
                                        masterTrxDetail.ExtraWork = 0;
                                        masterTrxDetail.ReSell = false;
                                        masterTrxDetail.ClientSupplies = false;

                                        List<PaymentTempDetail> _masterINVDetail = context.PaymentTempDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == cvm.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

                                        if (_masterINVDetail.Count > 0)
                                        {
                                            masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                            masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                            masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                            //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                            masterTrxDetail.Quantity = 0;
                                            masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                            masterTrxDetail.TotalFee = 0;
                                            masterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                            masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                            masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                            masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                            masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                        }



                                        context.PaymentTempDetails.Add(masterTrxDetail);
                                        context.SaveChanges();


                                        custMasterTrxDetailId = masterTrxDetail.PaymentTempDetailId;
                                        // insert customer taxes

                                        PaymentTempDetailTax customerTax = new PaymentTempDetailTax();
                                        customerTax.MasterTrxDetailId = masterTrxDetail.PaymentTempDetailId;
                                        customerTax.Amount = cvm.Tax;
                                        customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / cvm.PaymentAmount, 2);
                                        customerTax.AmountTypeListId = 1; // credit
                                        customerTax.CreatedBy = inputdata.CreatedBy;
                                        customerTax.CreatedDate = inputdata.CreatedDate;

                                        customerTax.InvoiceId = ivm.InvoiceId;
                                        customerTax.RegionId = inputdata.RegionId;
                                        customerTax.PeriodId = (PR != null ? PR.PeriodId : 0);
                                        customerTax.CustomerId = _oCustId;
                                        customerTax.FRRevenues = false;
                                        customerTax.FRDeduction = false;
                                        //customerTax.FranchiseeId = null;
                                        context.PaymentTempDetailTaxes.Add(customerTax);
                                        context.SaveChanges();

                                        totalCustomerPayment += cvm.Total;
                                        totalCustomerTaxes += cvm.Tax;

                                        var invoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
                                        var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
                                        var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

                                        List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
                                        decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
                                        decimal totalTransactions = 0.00m;
                                        decimal grandTotalTransactions = 0.00m;

                                        foreach (var trx in invoiceTransactions)
                                        {
                                            totalTransactions = totalTransactions + (decimal)trx.Total;
                                        }
                                        grandTotalTransactions = totalTransactions + cvm.PaymentAmount;


                                        if (grandTotalTransactions >= invoiceTotal)
                                        {
                                            invoice.TransactionStatusListId = 6; /*6 = Paid*/
                                            invoiceMasterTrx.StatusId = 6;
                                        }
                                        else
                                        {
                                            invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
                                            invoiceMasterTrx.StatusId = 7;
                                        }
                                        context.SaveChanges();
                                    }

                                }
                                foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
                                {
                                    // compute franchisee payment fees

                                    decimal totalFees = 0;

                                    List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
                                    List<PaymentTempDetailFee> franchiseeFees = new List<PaymentTempDetailFee>();

                                    foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
                                    {
                                        PaymentTempDetailFee feeDetail = new PaymentTempDetailFee();
                                        if (feeDef.FeePercentage != null) // percentage
                                        {
                                            feeDetail.FeePercentage = feeDef.FeePercentage;
                                            feeDetail.Amount = (decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M); // Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
                                        }
                                        else // flat amount
                                        {
                                            feeDetail.Amount = feeDef.Amount;
                                            feeDetail.FeePercentage = null;
                                        }
                                        feeDetail.FeeId = feeDef.FeeId;
                                        feeDetail.AmountTypeListId = 1; // credit
                                        feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
                                        feeDetail.CreatedBy = inputdata.CreatedBy;
                                        feeDetail.CreatedDate = inputdata.CreatedDate;
                                        feeDetail.FranchiseeId = fcvm.FranchiseeId;
                                        feeDetail.BillingPayId = fcvm.BillingPayId;
                                        totalFees += feeDetail.Amount ?? 0;

                                        franchiseeFees.Add(feeDetail);
                                    }

                                    // franchisee payment mastertrx

                                    PaymentTempMasterTrx franchiseeMasterTrx = new PaymentTempMasterTrx();
                                    franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
                                    franchiseeMasterTrx.TypeListId = 2; // franchisee
                                    franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
                                    franchiseeMasterTrx.TrxDate = inputdata.TransactionDate;
                                    franchiseeMasterTrx.RegionId = inputdata.RegionId;
                                    franchiseeMasterTrx.StatusId = 1; // open

                                    franchiseeMasterTrx.BillMonth = inputdata.TransactionDate.Month;
                                    franchiseeMasterTrx.BillYear = inputdata.TransactionDate.Year;
                                    franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    franchiseeMasterTrx.CreatedBy = LoginUserId;
                                    franchiseeMasterTrx.CreatedDate = DateTime.Now;
                                    franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    context.PaymentTempMasterTrxes.Add(franchiseeMasterTrx);
                                    context.SaveChanges();


                                    // franchisee payment

                                    string franchiseeNextTrxNumber = franchiseeTransactionNumberConfigViewModel.Prefix.Trim() + franchiseeTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", inputdata.TransactionDate.Month) + (franchiseeTransactionNumberConfigViewModel.LastNumber + 1).ToString();
                                    //string franchiseeNextTrxNumber = "BILPMT" + _invoiceOject.InvoiceNo.Trim(); ;



                                    PaymentTemp franchiseePayment = new PaymentTemp();
                                    franchiseePayment.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
                                    franchiseePayment.PaymentId = customerPayment.PaymentTempId;
                                    franchiseePayment.ClassId = fcvm.FranchiseeId;
                                    franchiseePayment.TypeListId = 2;
                                    franchiseePayment.BillingPayId = fcvm.BillingPayId;
                                    franchiseePayment.RegionId = inputdata.RegionId;
                                    franchiseePayment.LineNo = Convert.ToString(fcvm.Payment.LineNo);
                                    franchiseePayment.Amount = fcvm.Payment.PaymentAmount;
                                    franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
                                    franchiseePayment.TransactionDate = inputdata.TransactionDate;
                                    franchiseePayment.CreatedBy = inputdata.CreatedBy;
                                    franchiseePayment.CreatedDate = inputdata.CreatedDate;
                                    franchiseePayment.IsTARPaid = fcvm.IsTARPaid;
                                    franchiseePayment.IsTurnaroundPayment = fcvm.IsTurnAroundPayment;
                                    franchiseePayment.TransactionStatusListId = 1;
                                    franchiseePayment.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    franchiseePayment.IsBillingFranchisee = true;
                                    context.PaymentTemps.Add(franchiseePayment);
                                    context.SaveChanges();

                                    franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
                                    context.SaveChanges();

                                    if (franchiseeTransactionNumberConfigViewModel != null)
                                    {
                                        franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
                                        CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
                                    }


                                    decimal paymentMinusFees = (decimal)(fcvm.Payment.PaymentAmount - totalFees);// Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

                                    PaymentTempDetail franchiseeMasterTrxDetail = new PaymentTempDetail();
                                    franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
                                    franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
                                    franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
                                    franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
                                    franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentTempId; //customerPayment.PaymentId;
                                    franchiseeMasterTrxDetail.RegionId = inputdata.RegionId;

                                    franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
                                    franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
                                    franchiseeMasterTrxDetail.FeesDetail = true;
                                    franchiseeMasterTrxDetail.TaxDetail = false;
                                    franchiseeMasterTrxDetail.TotalFee = totalFees;
                                    franchiseeMasterTrxDetail.Total = paymentMinusFees;

                                    franchiseeMasterTrxDetail.CreatedBy = inputdata.CreatedBy;
                                    franchiseeMasterTrxDetail.CreatedDate = inputdata.CreatedDate;
                                    franchiseeMasterTrxDetail.IsDelete = false;
                                    franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
                                    franchiseeMasterTrxDetail.TypelistId = 2; // customer
                                    franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
                                    franchiseeMasterTrxDetail.Transactiondate = inputdata.TransactionDate;



                                    franchiseeMasterTrxDetail.BPPAdmin = 1;
                                    franchiseeMasterTrxDetail.AccountRebate = 1;
                                    franchiseeMasterTrxDetail.AccountRebate = 1;
                                    franchiseeMasterTrxDetail.Commission = false;
                                    franchiseeMasterTrxDetail.CommissionTotal = 0;
                                    franchiseeMasterTrxDetail.FRRevenues = false;
                                    franchiseeMasterTrxDetail.FRDeduction = false;
                                    franchiseeMasterTrxDetail.DetailDescription = inputdata.Notes;
                                    List<MasterTrxDetail> _masterINVDetail = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
                                    if (_masterINVDetail.Count > 0)
                                    {
                                        franchiseeMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
                                        franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
                                        franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
                                        //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
                                        franchiseeMasterTrxDetail.Quantity = 0;
                                        franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
                                        franchiseeMasterTrxDetail.TotalTax = 0;
                                        franchiseeMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
                                        franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
                                        franchiseeMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
                                        franchiseeMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
                                        franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

                                    }

                                    context.PaymentTempDetails.Add(franchiseeMasterTrxDetail);
                                    context.SaveChanges();


                                    // insert franchisee fees

                                    foreach (PaymentTempDetailFee feeDetail in franchiseeFees)
                                    {
                                        feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.PaymentTempDetailId; // set the id after insertion

                                        context.PaymentTempDetailFees.Add(feeDetail);
                                        context.SaveChanges();
                                    }


                                }
                            }
                            else
                            {
                                ivm.OverflowAmount = ivm.OverflowAmount - Math.Abs(ivm.InvoicePayment);
                            }

                            if (ivm.OverflowAmount > 0)
                            {
                                PaymentTempDetail masterTrxDetailOP = new PaymentTempDetail();
                                masterTrxDetailOP.MasterTrxId = custMasterTrx.PaymentTempMasterTrxId;
                                masterTrxDetailOP.InvoiceId = ivm.InvoiceId;
                                masterTrxDetailOP.MasterTrxTypeListId = 17; // customer payment
                                masterTrxDetailOP.HeaderId = customerPayment.PaymentTempId;
                                masterTrxDetailOP.RegionId = inputdata.RegionId;
                                masterTrxDetailOP.AmountTypeListId = 1; // credit
                                masterTrxDetailOP.FeesDetail = false;
                                masterTrxDetailOP.TaxDetail = true;
                                masterTrxDetailOP.TotalTax = 0;
                                masterTrxDetailOP.TotalFee = 0;
                                masterTrxDetailOP.Quantity = 1;
                                masterTrxDetailOP.UnitPrice = ivm.OverflowAmount;
                                masterTrxDetailOP.Total = ivm.OverflowAmount;
                                masterTrxDetailOP.ExtendedPrice = ivm.OverflowAmount;
                                masterTrxDetailOP.CreatedBy = inputdata.CreatedBy;
                                masterTrxDetailOP.CreatedDate = inputdata.CreatedDate;
                                masterTrxDetailOP.IsDelete = false;
                                masterTrxDetailOP.PeriodId = custMasterTrx.PeriodId;
                                masterTrxDetailOP.TypelistId = 1; // customer
                                masterTrxDetailOP.ClassId = ivm.InvoiceCustomerId;
                                masterTrxDetailOP.Transactiondate = inputdata.TransactionDate;
                                context.PaymentTempDetails.Add(masterTrxDetailOP);
                                context.SaveChanges();
                            }
                        }
                    }
                }






                return retVal;
            }
        }
        //    public int UpdatePaymentDetailPopup(int Id, DateTime PaymentDate, int PaymentType, string PaymentNo, decimal Amount, string Note, FullManualPaymentViewModel inputdata)
        //    {
        //        //Need to verify fields (column name with values) by Peter sir 
        //        using (jkDatabaseEntities context = new jkDatabaseEntities())
        //        {
        //            context.Configuration.ProxyCreationEnabled = false;

        //            JKApi.Service.Service.Company.CompanyService CompanySvc = new JKApi.Service.Service.Company.CompanyService();
        //            JKViewModels.Administration.Company.TransactionNumberConfigViewModel customerTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
        //            JKViewModels.Administration.Company.TransactionNumberConfigViewModel franchiseeTransactionNumberConfigViewModel = new JKViewModels.Administration.Company.TransactionNumberConfigViewModel();
        //            var PR = context.Periods.SingleOrDefault(p => p.BillMonth == PaymentDate.Month && p.BillYear == PaymentDate.Year);


        //            //Header Table MasterTrxTemp                

        //            //Customer Side
        //            //Header Table PaymentTemp
        //            PaymentTemp custPayment = context.PaymentTemps.Where(f => f.PaymentTempId == Id).FirstOrDefault();
        //            PaymentTempMasterTrx custMasterTrx = context.PaymentTempMasterTrxes.SingleOrDefault(pm => pm.PaymentTempMasterTrxId == custPayment.MasterTrxId);
        //            custMasterTrx.TrxDate = PaymentDate;
        //            custMasterTrx.BillMonth = PR.BillMonth;
        //            custMasterTrx.BillYear = PR.BillYear;
        //            custMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //            custMasterTrx.ModifiedBy = LoginUserId;
        //            custMasterTrx.ModifiedDate = DateTime.Now;
        //            context.SaveChanges();

        //            List<PaymentTemp> lstcustPayment = context.PaymentTemps.Where(f => f.MasterTrxId == custMasterTrx.PaymentTempMasterTrxId).ToList();
        //            foreach (var cItem in lstcustPayment)
        //            {
        //                cItem.TransactionDate = PaymentDate;
        //                cItem.PaymentMethodListId = PaymentType;
        //                cItem.PaymentNo = PaymentNo;
        //                cItem.PaymentDescription = Note;
        //                context.SaveChanges();
        //            }

        //            List<int> lstRemoveInvoices = new List<int>();
        //            foreach (var invItem in lstManualInvoices)
        //            {
        //                if (context.PaymentTempDetails.Where(o => o.InvoiceId == invItem.InvoiceId && o.MasterTrxTypeListId == 2 && o.HeaderId == Id).Count() == 0)
        //                {
        //                    lstRemoveInvoices.Add(invItem.InvoiceId);
        //                }
        //            }

        //            if (lstManualInvoices.Count > 0)
        //            {
        //                foreach (var invItem in lstManualInvoices)
        //                {
        //                    CreditDetailViewModel invoiceDetail = GetCreditDetailForInvoicePayment(invItem.InvoiceId);

        //                    int invId = invItem.InvoiceId;

        //                    var invPayment = invItem.InvoicePayment + invItem.OverflowAmount;
        //                    var invBalance = invItem.InvoiceBalance;
        //                    var invCustomerId = invoiceDetail.Invoice.InvoiceDetail.CustomerId;

        //                    //invoiceDetail.InvoiceAmount;
        //                    invBalance = invoiceDetail.InvoiceBalance - invPayment;

        //                    //oManualInvoice = new MPInvoiceViewModel();
        //                    //oManualInvoice.InvoiceId = invId;
        //                    //oManualInvoice.InvoiceCustomerId = invCustomerId;
        //                    //oManualInvoice.InvoicePayment = invPayment;
        //                    decimal applyAmountforPartialPay = invPayment;

        //                    decimal applybalanceAmountINV = 0;
        //                    decimal applyAmountINV = 0;
        //                    decimal applyOverAmountINV = 0;
        //                    bool invPaidInFull = false;
        //                    int? _RegionId = invoiceDetail.Invoice.InvoiceDetail.RegionId;

        //                    if (invBalance < 0) //OverPayment
        //                    {
        //                        applybalanceAmountINV = 0;
        //                        applyOverAmountINV = Math.Abs(invBalance);
        //                        applyAmountINV = invPayment - Math.Abs(invBalance);
        //                        applyAmountforPartialPay = invPayment - Math.Abs(invBalance);

        //                        if (applyOverAmountINV > 0)
        //                        {
        //                            PaymentTempDetail masterTrxDetailOP = new PaymentTempDetail();
        //                            masterTrxDetailOP.MasterTrxId = custMasterTrx.PaymentTempMasterTrxId;
        //                            masterTrxDetailOP.InvoiceId = invId;
        //                            masterTrxDetailOP.MasterTrxTypeListId = 17; // customer payment
        //                            masterTrxDetailOP.HeaderId = custPayment.PaymentTempId;
        //                            masterTrxDetailOP.RegionId = custPayment.RegionId;
        //                            masterTrxDetailOP.AmountTypeListId = 1; // credit
        //                            masterTrxDetailOP.FeesDetail = false;
        //                            masterTrxDetailOP.TaxDetail = true;
        //                            masterTrxDetailOP.TotalTax = 0;
        //                            masterTrxDetailOP.TotalFee = 0;
        //                            masterTrxDetailOP.Quantity = 1;
        //                            masterTrxDetailOP.UnitPrice = applyOverAmountINV;
        //                            masterTrxDetailOP.Total = applyOverAmountINV;
        //                            masterTrxDetailOP.ExtendedPrice = applyOverAmountINV;
        //                            masterTrxDetailOP.CreatedBy = LoginUserId;
        //                            masterTrxDetailOP.CreatedDate = DateTime.Now;
        //                            masterTrxDetailOP.IsDelete = false;
        //                            masterTrxDetailOP.PeriodId = custMasterTrx.PeriodId;
        //                            masterTrxDetailOP.TypelistId = 1; // customer
        //                            masterTrxDetailOP.ClassId = invCustomerId;
        //                            masterTrxDetailOP.Transactiondate = PaymentDate;
        //                            context.PaymentTempDetails.Add(masterTrxDetailOP);
        //                            context.SaveChanges();
        //                        }

        //                    }
        //                    else
        //                    {
        //                        applybalanceAmountINV = invBalance;
        //                        applyOverAmountINV = 0;
        //                        applyAmountINV = invPayment;
        //                        applyAmountforPartialPay = 0;
        //                    }
        //                    invPaidInFull = applybalanceAmountINV == 0 ? true : false;


        //                    if (invoiceDetail.Invoice.InvoiceDetailItems.Count() > 0)
        //                    {

        //                        decimal _itemBalance = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.Balance);
        //                        decimal _itemTAXAmount = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.TAXAmount);
        //                        decimal _itemTotal = (decimal)invoiceDetail.Invoice.InvoiceDetailItems.Sum(g => g.Total);

        //                        //var item = invoiceDetail.Invoice.InvoiceDetailItems[0];

        //                        decimal itemPaymentAmt = 0;
        //                        decimal itemTotal = 0;
        //                        decimal taxRate = Math.Round(_itemTAXAmount / _itemTotal, 2);


        //                        itemPaymentAmt = Math.Round((applyAmountINV / (1 + taxRate)), 2);
        //                        var taxAmount = Math.Round(itemPaymentAmt * taxRate, 2);// (decimal)(invPayment * taxRate);
        //                        itemTotal = Math.Round(applyAmountINV, 2);



        //                        PaymentTempDetail custMasterTrxDetail = new PaymentTempDetail();
        //                        custMasterTrxDetail.MasterTrxId = custMasterTrx.PaymentTempMasterTrxId;
        //                        custMasterTrxDetail.InvoiceId = invId;
        //                        custMasterTrxDetail.LineNo = -1;
        //                        custMasterTrxDetail.MasterTrxTypeListId = 2; // customer payment
        //                        custMasterTrxDetail.HeaderId = custPayment.PaymentTempId;
        //                        custMasterTrxDetail.RegionId = _RegionId;
        //                        custMasterTrxDetail.AmountTypeListId = 1; // credit
        //                        custMasterTrxDetail.FeesDetail = false;
        //                        custMasterTrxDetail.TaxDetail = true;
        //                        custMasterTrxDetail.TotalTax = 0;
        //                        custMasterTrxDetail.Total = itemTotal;
        //                        custMasterTrxDetail.ExtendedPrice = itemPaymentAmt;
        //                        custMasterTrxDetail.CreatedBy = custPayment.CreatedBy;
        //                        custMasterTrxDetail.CreatedDate = custPayment.CreatedDate;
        //                        custMasterTrxDetail.IsDelete = false;
        //                        //masterTrxDetail.ServiceTypeListId =
        //                        custMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                        custMasterTrxDetail.TypelistId = 1; // customer
        //                        custMasterTrxDetail.ClassId = invCustomerId;
        //                        custMasterTrxDetail.Transactiondate = PaymentDate;
        //                        custMasterTrxDetail.BPPAdmin = 1;
        //                        custMasterTrxDetail.AccountRebate = 1;
        //                        custMasterTrxDetail.AccountRebate = 1;
        //                        custMasterTrxDetail.Commission = false;
        //                        custMasterTrxDetail.CommissionTotal = 0;
        //                        custMasterTrxDetail.FRRevenues = false;
        //                        custMasterTrxDetail.FRDeduction = false;
        //                        custMasterTrxDetail.DetailDescription = Note;

        //                        custMasterTrxDetail.UnitPrice = 0;
        //                        custMasterTrxDetail.CPIPercentage = 0;
        //                        custMasterTrxDetail.TotalFee = 0;

        //                        custMasterTrxDetail.SourceId = 0;
        //                        custMasterTrxDetail.SourceTypeListId = 0;
        //                        custMasterTrxDetail.ServiceTypeListId = 0;
        //                        //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //                        custMasterTrxDetail.Quantity = 0;
        //                        custMasterTrxDetail.CPIPercentage = 0;
        //                        custMasterTrxDetail.TotalFee = 0;
        //                        custMasterTrxDetail.Commission = false;
        //                        custMasterTrxDetail.CommissionTotal = 0;
        //                        custMasterTrxDetail.ExtraWork = 0;
        //                        custMasterTrxDetail.ReSell = false;
        //                        custMasterTrxDetail.ClientSupplies = false;

        //                        List<PaymentTempDetail> _masterINVDetail = context.PaymentTempDetails.Where(m => m.IsDelete != true && m.InvoiceId == invId && m.LineNo == 1 && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

        //                        if (_masterINVDetail.Count > 0)
        //                        {
        //                            custMasterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
        //                            custMasterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
        //                            custMasterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
        //                            //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //                            custMasterTrxDetail.Quantity = 0;
        //                            custMasterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
        //                            custMasterTrxDetail.TotalFee = 0;
        //                            custMasterTrxDetail.Commission = _masterINVDetail[0].Commission;
        //                            custMasterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
        //                            custMasterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
        //                            custMasterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
        //                            custMasterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

        //                        }



        //                        context.PaymentTempDetails.Add(custMasterTrxDetail);
        //                        context.SaveChanges();


        //                        //custMasterTrxDetailId = custMasterTrxDetail.PaymentTempDetailId;
        //                        // insert customer taxes

        //                        PaymentTempDetailTax customerTax = new PaymentTempDetailTax();
        //                        customerTax.MasterTrxDetailId = custMasterTrxDetail.PaymentTempDetailId;
        //                        customerTax.Amount = taxAmount;
        //                        customerTax.TaxratePercentage = Decimal.Round(taxRate * 100.00M, 2);
        //                        customerTax.AmountTypeListId = 1; // credit
        //                        customerTax.CreatedBy = LoginUserId;
        //                        customerTax.CreatedDate = DateTime.Now;

        //                        customerTax.InvoiceId = invId;
        //                        customerTax.RegionId = custPayment.RegionId;
        //                        customerTax.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                        customerTax.CustomerId = custPayment.ClassId;
        //                        customerTax.FRRevenues = false;
        //                        customerTax.FRDeduction = false;
        //                        //customerTax.FranchiseeId = null;
        //                        context.PaymentTempDetailTaxes.Add(customerTax);
        //                        context.SaveChanges();



        //                        //Franchisee
        //                        List<ManualPaymentFranchiseeViewModel> mpfvms = new List<ManualPaymentFranchiseeViewModel>();

        //                        foreach (var item in invoiceDetail.FranchiseeItems)
        //                        {
        //                            var r = item.InvoiceFranchiseeDetailItem;
        //                            int bpId = r.BillingPayId;
        //                            decimal bpPaymentAmt = 0;
        //                            // sanity check to see if payment details were set or if whole invoice was paid
        //                            if (invPaidInFull)
        //                            {
        //                                bpPaymentAmt = (decimal)r.Balance + (decimal)r.BalanceFees; // pay whole balance because whole invoice was paid
        //                            }
        //                            else
        //                            {
        //                                var totalFranchiseeBalance =
        //                               invoiceDetail.FranchiseeItems.Sum(o => o.InvoiceFranchiseeDetailItem.Balance);
        //                                var percentage = r.Balance / totalFranchiseeBalance;
        //                                bpPaymentAmt = itemPaymentAmt * (decimal)percentage;
        //                            }



        //                            ManualPaymentFranchiseeViewModel mpfvm = new ManualPaymentFranchiseeViewModel();
        //                            mpfvm.FranchiseeId = r.FranchiseeId;
        //                            mpfvm.BillingPayId = r.BillingPayId;


        //                            var billingPay = context.BillingPays.Where(o => o.BillingPayId == r.BillingPayId).FirstOrDefault();
        //                            mpfvm.IsTurnAroundPayment = billingPay.HasBeenChargedBack ? true : false;
        //                            mpfvm.IsTARPaid = false;

        //                            ManualPaymentViewModel mpvm = new ManualPaymentViewModel();
        //                            mpvm.MasterTrxDetailId = r.MasterTrxDetailId;
        //                            mpvm.LineNo = (int)r.LineNo;
        //                            mpvm.PaymentAmount = bpPaymentAmt;
        //                            mpfvm.Payment = mpvm;

        //                            mpfvms.Add(mpfvm);





        //                            // compute franchisee payment fees
        //                            decimal totalExamt = bpPaymentAmt;

        //                            decimal totalApplyExamt = 0;

        //                            //if (_paymentAmount <= totalExamt)
        //                            //{
        //                            //    totalApplyExamt = _paymentAmount;
        //                            //    _paymentAmount = 0;
        //                            //}
        //                            //else
        //                            //{
        //                            //    totalApplyExamt = totalExamt;
        //                            //    _paymentAmount = _paymentAmount - totalExamt;
        //                            //}



        //                            decimal totalFees = 0;

        //                            List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == item.InvoiceFranchiseeDetailItem.MasterTrxDetailId).ToList();
        //                            List<PaymentTempDetailFee> franchiseeFees = new List<PaymentTempDetailFee>();

        //                            foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
        //                            {
        //                                PaymentTempDetailFee feeDetail = new PaymentTempDetailFee();
        //                                if (feeDef.FeePercentage != null) // percentage
        //                                {
        //                                    feeDetail.FeePercentage = feeDef.FeePercentage;
        //                                    feeDetail.Amount = Math.Round((decimal)(totalApplyExamt * feeDetail.FeePercentage / 100.0M), 2);
        //                                }
        //                                else // flat amount
        //                                {
        //                                    feeDetail.Amount = feeDef.Amount;
        //                                    feeDetail.FeePercentage = null;
        //                                }
        //                                feeDetail.FeeId = feeDef.FeeId;
        //                                feeDetail.AmountTypeListId = 1; // credit
        //                                feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
        //                                feeDetail.CreatedBy = LoginUserId;
        //                                feeDetail.CreatedDate = DateTime.Now;
        //                                feeDetail.FranchiseeId = item.CreditFranchiseeId;
        //                                feeDetail.BillingPayId = bpId;
        //                                totalFees += feeDetail.Amount ?? 0;

        //                                franchiseeFees.Add(feeDetail);
        //                            }

        //                            // franchisee payment mastertrx

        //                            PaymentTempMasterTrx franchiseeMasterTrx = new PaymentTempMasterTrx();
        //                            franchiseeMasterTrx.ClassId = item.CreditFranchiseeId;
        //                            franchiseeMasterTrx.TypeListId = 2; // franchisee
        //                            franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
        //                            franchiseeMasterTrx.TrxDate = PaymentDate;
        //                            franchiseeMasterTrx.RegionId = custMasterTrx.RegionId;
        //                            franchiseeMasterTrx.StatusId = 1; // open

        //                            franchiseeMasterTrx.BillMonth = PaymentDate.Month;
        //                            franchiseeMasterTrx.BillYear = PaymentDate.Year;
        //                            franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                            franchiseeMasterTrx.CreatedBy = LoginUserId;
        //                            franchiseeMasterTrx.CreatedDate = DateTime.Now;
        //                            franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                            context.PaymentTempMasterTrxes.Add(franchiseeMasterTrx);
        //                            context.SaveChanges();


        //                            // franchisee payment
        //                            franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(7, (int)custMasterTrx.RegionId);
        //                            string franchiseeNextTrxNumber = franchiseeTransactionNumberConfigViewModel.Prefix.Trim() + franchiseeTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", PaymentDate.Month) + (franchiseeTransactionNumberConfigViewModel.LastNumber + 1).ToString();
        //                            //string franchiseeNextTrxNumber = "BILPMT" + _invoiceOject.InvoiceNo.Trim(); ;



        //                            PaymentTemp franchiseePayment = new PaymentTemp();
        //                            franchiseePayment.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
        //                            franchiseePayment.PaymentId = custPayment.PaymentTempId;
        //                            franchiseePayment.ClassId = item.CreditFranchiseeId;
        //                            franchiseePayment.TypeListId = 2;
        //                            franchiseePayment.BillingPayId = bpId;
        //                            franchiseePayment.RegionId = custPayment.RegionId;
        //                            franchiseePayment.LineNo = item.InvoiceFranchiseeDetailItem.LineNo.ToString();
        //                            franchiseePayment.Amount = totalApplyExamt;
        //                            franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
        //                            franchiseePayment.TransactionDate = PaymentDate;
        //                            franchiseePayment.CreatedBy = LoginUserId;
        //                            franchiseePayment.CreatedDate = DateTime.Now;
        //                            franchiseePayment.IsTARPaid = false;
        //                            franchiseePayment.IsTurnaroundPayment = billingPay.HasBeenChargedBack ? true : false;
        //                            franchiseePayment.TransactionStatusListId = 1;
        //                            franchiseePayment.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                            franchiseePayment.IsBillingFranchisee = true;
        //                            context.PaymentTemps.Add(franchiseePayment);
        //                            context.SaveChanges();

        //                            franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
        //                            context.SaveChanges();

        //                            if (franchiseeTransactionNumberConfigViewModel != null)
        //                            {
        //                                franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
        //                                CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
        //                            }


        //                            decimal paymentMinusFees = Math.Round((decimal)(totalApplyExamt - totalFees), 2); // payment amount after taking out fees

        //                            PaymentTempDetail franchiseeMasterTrxDetail = new PaymentTempDetail();
        //                            franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
        //                            franchiseeMasterTrxDetail.InvoiceId = invId;
        //                            franchiseeMasterTrxDetail.LineNo = item.InvoiceFranchiseeDetailItem.LineNo;
        //                            franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
        //                            franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentTempId; //customerPayment.PaymentId;
        //                            franchiseeMasterTrxDetail.RegionId = custPayment.RegionId;

        //                            franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
        //                            franchiseeMasterTrxDetail.ExtendedPrice = totalApplyExamt;
        //                            franchiseeMasterTrxDetail.FeesDetail = true;
        //                            franchiseeMasterTrxDetail.TaxDetail = false;
        //                            franchiseeMasterTrxDetail.TotalFee = totalFees;
        //                            franchiseeMasterTrxDetail.Total = paymentMinusFees;

        //                            franchiseeMasterTrxDetail.CreatedBy = LoginUserId;
        //                            franchiseeMasterTrxDetail.CreatedDate = DateTime.Now;
        //                            franchiseeMasterTrxDetail.IsDelete = false;
        //                            franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
        //                            franchiseeMasterTrxDetail.TypelistId = 2; // customer
        //                            franchiseeMasterTrxDetail.ClassId = item.InvoiceFranchiseeDetailItem.FranchiseeId;
        //                            franchiseeMasterTrxDetail.Transactiondate = PaymentDate;



        //                            franchiseeMasterTrxDetail.BPPAdmin = 1;
        //                            franchiseeMasterTrxDetail.AccountRebate = 1;
        //                            franchiseeMasterTrxDetail.AccountRebate = 1;
        //                            franchiseeMasterTrxDetail.Commission = false;
        //                            franchiseeMasterTrxDetail.CommissionTotal = 0;
        //                            franchiseeMasterTrxDetail.FRRevenues = false;
        //                            franchiseeMasterTrxDetail.FRDeduction = false;
        //                            franchiseeMasterTrxDetail.DetailDescription = Note;
        //                            List<MasterTrxDetail> _masterINVDetailF = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == invId && m.LineNo == item.InvoiceFranchiseeDetailItem.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
        //                            if (_masterINVDetailF.Count > 0)
        //                            {
        //                                franchiseeMasterTrxDetail.SourceId = _masterINVDetailF[0].SourceId;
        //                                franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetailF[0].SourceTypeListId;
        //                                franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetailF[0].ServiceTypeListId;
        //                                //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //                                franchiseeMasterTrxDetail.Quantity = 0;
        //                                franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetailF[0].CPIPercentage;
        //                                franchiseeMasterTrxDetail.TotalTax = 0;
        //                                franchiseeMasterTrxDetail.Commission = _masterINVDetailF[0].Commission;
        //                                franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetailF[0].CommissionTotal;
        //                                franchiseeMasterTrxDetail.ExtraWork = _masterINVDetailF[0].ExtraWork;
        //                                franchiseeMasterTrxDetail.ReSell = _masterINVDetailF[0].ReSell;
        //                                franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetailF[0].ClientSupplies;

        //                            }

        //                            context.PaymentTempDetails.Add(franchiseeMasterTrxDetail);
        //                            context.SaveChanges();


        //                            // insert franchisee fees

        //                            foreach (PaymentTempDetailFee feeDetail in franchiseeFees)
        //                            {
        //                                feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.PaymentTempDetailId; // set the id after insertion

        //                                context.PaymentTempDetailFees.Add(feeDetail);
        //                                context.SaveChanges();
        //                            }




        //                        }




        //                    }


        //                }
        //            }

        //            return 1;
        //        }


        //        ////Header Detail 
        //        //PaymentTemp customerPayment = context.PaymentTemps.Where(f => f.PaymentTempId == Id).FirstOrDefault();
        //        //if (customerPayment != null)
        //        //{
        //        //    customerPayment.TransactionDate = PaymentDate;
        //        //    customerPayment.PaymentMethodListId = PaymentType;
        //        //    customerPayment.PaymentNo = PaymentNo;
        //        //    customerPayment.PaymentDescription = Note;
        //        //    //Need to verify fields (column name with values) by Peter sir 
        //        //    context.SaveChanges();

        //        //    customerTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(2, (int)customerPayment.RegionId);
        //        //    franchiseeTransactionNumberConfigViewModel = CompanySvc.GetTransactionNumberConfig(7, (int)customerPayment.RegionId);

        //        //    //update records
        //        //    //List<int> lstCust = inputdata.Invoices.Select(g => g.InvoiceCustomerId).Distinct().ToList<int>();



        //        //    PaymentTempMasterTrx customerMasterTrx = context.PaymentTempMasterTrxes.SingleOrDefault(pm => pm.PaymentTempMasterTrxId == customerPayment.MasterTrxId);

        //        //    customerMasterTrx.TrxDate = PaymentDate;
        //        //    customerMasterTrx.BillMonth = PR.BillMonth;
        //        //    customerMasterTrx.BillYear = PR.BillYear;
        //        //    customerMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //    customerMasterTrx.ModifiedBy = LoginUserId;
        //        //    customerMasterTrx.ModifiedDate = DateTime.Now;
        //        //    context.SaveChanges();

        //        //    int InvIdT = 0;






        //        //    decimal totalCustomerPayment = 0;
        //        //    decimal totalCustomerTaxes = 0;

        //        //    foreach (var ivm in lstManualInvoices)
        //        //    {
        //        //        int custMasterTrxDetailId = 0;

        //        //        if (ivm.InvoicePayment > 0)
        //        //        {


        //        //            PaymentTempDetail masterTrxDetail = new PaymentTempDetail();
        //        //            masterTrxDetail.MasterTrxId = customerMasterTrx.PaymentTempMasterTrxId;
        //        //            masterTrxDetail.InvoiceId = ivm.InvoiceId;
        //        //            masterTrxDetail.LineNo = -1;
        //        //            masterTrxDetail.MasterTrxTypeListId = 2; // customer payment
        //        //            masterTrxDetail.HeaderId = customerPayment.PaymentTempId;
        //        //            masterTrxDetail.RegionId = customerPayment.RegionId;
        //        //            masterTrxDetail.AmountTypeListId = 1; // credit
        //        //            masterTrxDetail.FeesDetail = false;
        //        //            masterTrxDetail.TaxDetail = true;
        //        //            masterTrxDetail.TotalTax = 0;
        //        //            masterTrxDetail.Total = ivm.InvoicePayment;
        //        //            masterTrxDetail.ExtendedPrice = ivm.InvoicePayment;
        //        //            masterTrxDetail.CreatedBy = customerPayment.CreatedBy;
        //        //            masterTrxDetail.CreatedDate = customerPayment.CreatedDate;
        //        //            masterTrxDetail.IsDelete = false;
        //        //            //masterTrxDetail.ServiceTypeListId =
        //        //            masterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //            masterTrxDetail.TypelistId = 1; // customer
        //        //            masterTrxDetail.ClassId = customerPayment.ClassId;
        //        //            masterTrxDetail.Transactiondate = PaymentDate;
        //        //            masterTrxDetail.BPPAdmin = 1;
        //        //            masterTrxDetail.AccountRebate = 1;
        //        //            masterTrxDetail.AccountRebate = 1;
        //        //            masterTrxDetail.Commission = false;
        //        //            masterTrxDetail.CommissionTotal = 0;
        //        //            masterTrxDetail.FRRevenues = false;
        //        //            masterTrxDetail.FRDeduction = false;
        //        //            masterTrxDetail.DetailDescription = Note;

        //        //            masterTrxDetail.UnitPrice = 0;
        //        //            masterTrxDetail.CPIPercentage = 0;
        //        //            masterTrxDetail.TotalFee = 0;

        //        //            masterTrxDetail.SourceId = 0;
        //        //            masterTrxDetail.SourceTypeListId = 0;
        //        //            masterTrxDetail.ServiceTypeListId = 0;
        //        //            //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //        //            masterTrxDetail.Quantity = 0;
        //        //            masterTrxDetail.CPIPercentage = 0;
        //        //            masterTrxDetail.TotalFee = 0;
        //        //            masterTrxDetail.Commission = false;
        //        //            masterTrxDetail.CommissionTotal = 0;
        //        //            masterTrxDetail.ExtraWork = 0;
        //        //            masterTrxDetail.ReSell = false;
        //        //            masterTrxDetail.ClientSupplies = false;

        //        //            List<PaymentTempDetail> _masterINVDetail = context.PaymentTempDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == 1 && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();

        //        //            if (_masterINVDetail.Count > 0)
        //        //            {
        //        //                masterTrxDetail.SourceId = _masterINVDetail[0].SourceId;
        //        //                masterTrxDetail.SourceTypeListId = _masterINVDetail[0].SourceTypeListId;
        //        //                masterTrxDetail.ServiceTypeListId = _masterINVDetail[0].ServiceTypeListId;
        //        //                //masterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //        //                masterTrxDetail.Quantity = 0;
        //        //                masterTrxDetail.CPIPercentage = _masterINVDetail[0].CPIPercentage;
        //        //                masterTrxDetail.TotalFee = 0;
        //        //                masterTrxDetail.Commission = _masterINVDetail[0].Commission;
        //        //                masterTrxDetail.CommissionTotal = _masterINVDetail[0].CommissionTotal;
        //        //                masterTrxDetail.ExtraWork = _masterINVDetail[0].ExtraWork;
        //        //                masterTrxDetail.ReSell = _masterINVDetail[0].ReSell;
        //        //                masterTrxDetail.ClientSupplies = _masterINVDetail[0].ClientSupplies;

        //        //            }



        //        //            context.PaymentTempDetails.Add(masterTrxDetail);
        //        //            context.SaveChanges();


        //        //            custMasterTrxDetailId = masterTrxDetail.PaymentTempDetailId;
        //        //            // insert customer taxes

        //        //            PaymentTempDetailTax customerTax = new PaymentTempDetailTax();
        //        //            customerTax.MasterTrxDetailId = masterTrxDetail.PaymentTempDetailId;
        //        //            customerTax.Amount = 0;// cvm.Tax;
        //        //            customerTax.TaxratePercentage = Decimal.Round(0 * 100.00M / ivm.InvoicePayment, 2);
        //        //            //customerTax.TaxratePercentage = Decimal.Round(cvm.Tax * 100.00M / ivm.InvoicePayment, 2);
        //        //            customerTax.AmountTypeListId = 1; // credit
        //        //            customerTax.CreatedBy = customerPayment.CreatedBy;
        //        //            customerTax.CreatedDate = customerPayment.CreatedDate;

        //        //            customerTax.InvoiceId = ivm.InvoiceId;
        //        //            customerTax.RegionId = customerPayment.RegionId;
        //        //            customerTax.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //            customerTax.CustomerId = customerPayment.ClassId;
        //        //            customerTax.FRRevenues = false;
        //        //            customerTax.FRDeduction = false;
        //        //            //customerTax.FranchiseeId = null;
        //        //            context.PaymentTempDetailTaxes.Add(customerTax);
        //        //            context.SaveChanges();


        //        //            var invoice = context.Invoices.Where(o => o.InvoiceId == ivm.InvoiceId).FirstOrDefault();
        //        //            var invoiceMasterTrx = context.MasterTrxes.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();
        //        //            var invoiceMasterTrxDetail = context.MasterTrxDetails.Where(m => m.MasterTrxId == invoice.MasterTrxId).FirstOrDefault();

        //        //            List<MasterTrxDetail> invoiceTransactions = context.MasterTrxDetails.Where(o => o.InvoiceId == invoice.InvoiceId && o.MasterTrxTypeListId == 2 && o.MasterTrxTypeListId == 3).ToList();
        //        //            decimal invoiceTotal = (decimal)invoiceMasterTrxDetail.Total;
        //        //            decimal totalTransactions = 0.00m;
        //        //            decimal grandTotalTransactions = 0.00m;

        //        //            foreach (var trx in invoiceTransactions)
        //        //            {
        //        //                totalTransactions = totalTransactions + (decimal)trx.Total;
        //        //            }
        //        //            grandTotalTransactions = totalTransactions + ivm.InvoicePayment;


        //        //            if (grandTotalTransactions >= invoiceTotal)
        //        //            {
        //        //                invoice.TransactionStatusListId = 6; /*6 = Paid*/
        //        //                invoiceMasterTrx.StatusId = 6;
        //        //            }
        //        //            else
        //        //            {
        //        //                invoice.TransactionStatusListId = 7; /*7 = Paid Partial*/
        //        //                invoiceMasterTrx.StatusId = 7;
        //        //            }
        //        //            context.SaveChanges();

        //        //            List<MasterTrxDetail> lstmasterTrxDetailFR = context.MasterTrxDetails.Where(inv => inv.InvoiceId == ivm.InvoiceId && (inv.MasterTrxTypeListId == 4 || inv.MasterTrxTypeListId == 4)).ToList();


        //        //            decimal _paymentAmount = ivm.InvoicePayment;
        //        //            foreach (var frItem in lstmasterTrxDetailFR)
        //        //            {
        //        //                // compute franchisee payment fees
        //        //                decimal totalExamt = (decimal)frItem.ExtendedPrice;

        //        //                decimal totalApplyExamt = 0;

        //        //                if (_paymentAmount <= totalExamt)
        //        //                {
        //        //                    totalApplyExamt = _paymentAmount;
        //        //                    _paymentAmount = 0;
        //        //                }
        //        //                else
        //        //                {
        //        //                    totalApplyExamt = totalExamt;
        //        //                    _paymentAmount = _paymentAmount - totalExamt;
        //        //                }



        //        //                decimal totalFees = 0;

        //        //                List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == frItem.MasterTrxDetailId).ToList();
        //        //                List<PaymentTempDetailFee> franchiseeFees = new List<PaymentTempDetailFee>();

        //        //                foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
        //        //                {
        //        //                    PaymentTempDetailFee feeDetail = new PaymentTempDetailFee();
        //        //                    if (feeDef.FeePercentage != null) // percentage
        //        //                    {
        //        //                        feeDetail.FeePercentage = feeDef.FeePercentage;
        //        //                        feeDetail.Amount = Math.Round((decimal)(totalApplyExamt * feeDetail.FeePercentage / 100.0M), 2);
        //        //                    }
        //        //                    else // flat amount
        //        //                    {
        //        //                        feeDetail.Amount = feeDef.Amount;
        //        //                        feeDetail.FeePercentage = null;
        //        //                    }
        //        //                    feeDetail.FeeId = feeDef.FeeId;
        //        //                    feeDetail.AmountTypeListId = 1; // credit
        //        //                    feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
        //        //                    feeDetail.CreatedBy = customerPayment.CreatedBy;
        //        //                    feeDetail.CreatedDate = customerPayment.CreatedDate;
        //        //                    feeDetail.FranchiseeId = frItem.ClassId;
        //        //                    feeDetail.BillingPayId = frItem.BillingPayId;
        //        //                    totalFees += feeDetail.Amount ?? 0;

        //        //                    franchiseeFees.Add(feeDetail);
        //        //                }

        //        //                // franchisee payment mastertrx

        //        //                PaymentTempMasterTrx franchiseeMasterTrx = new PaymentTempMasterTrx();
        //        //                franchiseeMasterTrx.ClassId = frItem.ClassId;
        //        //                franchiseeMasterTrx.TypeListId = 2; // franchisee
        //        //                franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
        //        //                franchiseeMasterTrx.TrxDate = PaymentDate;
        //        //                franchiseeMasterTrx.RegionId = customerPayment.RegionId;
        //        //                franchiseeMasterTrx.StatusId = 1; // open

        //        //                franchiseeMasterTrx.BillMonth = PaymentDate.Month;
        //        //                franchiseeMasterTrx.BillYear = PaymentDate.Year;
        //        //                franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //                franchiseeMasterTrx.CreatedBy = LoginUserId;
        //        //                franchiseeMasterTrx.CreatedDate = DateTime.Now;
        //        //                franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //                context.PaymentTempMasterTrxes.Add(franchiseeMasterTrx);
        //        //                context.SaveChanges();


        //        //                // franchisee payment

        //        //                string franchiseeNextTrxNumber = franchiseeTransactionNumberConfigViewModel.Prefix.Trim() + franchiseeTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", PaymentDate.Month) + (franchiseeTransactionNumberConfigViewModel.LastNumber + 1).ToString();
        //        //                //string franchiseeNextTrxNumber = "BILPMT" + _invoiceOject.InvoiceNo.Trim(); ;



        //        //                PaymentTemp franchiseePayment = new PaymentTemp();
        //        //                franchiseePayment.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
        //        //                franchiseePayment.PaymentId = customerPayment.PaymentTempId;
        //        //                franchiseePayment.ClassId = frItem.ClassId;
        //        //                franchiseePayment.TypeListId = 2;
        //        //                franchiseePayment.BillingPayId = frItem.BillingPayId;
        //        //                franchiseePayment.RegionId = customerPayment.RegionId;
        //        //                franchiseePayment.LineNo = Convert.ToString(frItem.LineNo);
        //        //                franchiseePayment.Amount = totalApplyExamt;
        //        //                franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
        //        //                franchiseePayment.TransactionDate = PaymentDate;
        //        //                franchiseePayment.CreatedBy = customerPayment.CreatedBy;
        //        //                franchiseePayment.CreatedDate = customerPayment.CreatedDate;
        //        //                franchiseePayment.IsTARPaid = frItem.IsTARPaid;
        //        //                //franchiseePayment.IsTurnaroundPayment = frItem.IsTurnAroundPayment;
        //        //                franchiseePayment.TransactionStatusListId = 1;
        //        //                franchiseePayment.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //                franchiseePayment.IsBillingFranchisee = true;
        //        //                context.PaymentTemps.Add(franchiseePayment);
        //        //                context.SaveChanges();

        //        //                franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
        //        //                context.SaveChanges();

        //        //                if (franchiseeTransactionNumberConfigViewModel != null)
        //        //                {
        //        //                    franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
        //        //                    CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
        //        //                }


        //        //                decimal paymentMinusFees = Math.Round((decimal)(totalApplyExamt - totalFees), 2); // payment amount after taking out fees

        //        //                PaymentTempDetail franchiseeMasterTrxDetail = new PaymentTempDetail();
        //        //                franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
        //        //                franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
        //        //                franchiseeMasterTrxDetail.LineNo = frItem.LineNo;
        //        //                franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
        //        //                franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentTempId; //customerPayment.PaymentId;
        //        //                franchiseeMasterTrxDetail.RegionId = customerPayment.RegionId;

        //        //                franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
        //        //                franchiseeMasterTrxDetail.ExtendedPrice = totalApplyExamt;
        //        //                franchiseeMasterTrxDetail.FeesDetail = true;
        //        //                franchiseeMasterTrxDetail.TaxDetail = false;
        //        //                franchiseeMasterTrxDetail.TotalFee = totalFees;
        //        //                franchiseeMasterTrxDetail.Total = paymentMinusFees;

        //        //                franchiseeMasterTrxDetail.CreatedBy = customerPayment.CreatedBy;
        //        //                franchiseeMasterTrxDetail.CreatedDate = customerPayment.CreatedDate;
        //        //                franchiseeMasterTrxDetail.IsDelete = false;
        //        //                franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //                franchiseeMasterTrxDetail.TypelistId = 2; // customer
        //        //                franchiseeMasterTrxDetail.ClassId = frItem.ClassId;
        //        //                franchiseeMasterTrxDetail.Transactiondate = PaymentDate;



        //        //                franchiseeMasterTrxDetail.BPPAdmin = 1;
        //        //                franchiseeMasterTrxDetail.AccountRebate = 1;
        //        //                franchiseeMasterTrxDetail.AccountRebate = 1;
        //        //                franchiseeMasterTrxDetail.Commission = false;
        //        //                franchiseeMasterTrxDetail.CommissionTotal = 0;
        //        //                franchiseeMasterTrxDetail.FRRevenues = false;
        //        //                franchiseeMasterTrxDetail.FRDeduction = false;
        //        //                franchiseeMasterTrxDetail.DetailDescription = Note;
        //        //                List<MasterTrxDetail> _masterINVDetailF = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == frItem.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
        //        //                if (_masterINVDetailF.Count > 0)
        //        //                {
        //        //                    franchiseeMasterTrxDetail.SourceId = _masterINVDetailF[0].SourceId;
        //        //                    franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetailF[0].SourceTypeListId;
        //        //                    franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetailF[0].ServiceTypeListId;
        //        //                    //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //        //                    franchiseeMasterTrxDetail.Quantity = 0;
        //        //                    franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetailF[0].CPIPercentage;
        //        //                    franchiseeMasterTrxDetail.TotalTax = 0;
        //        //                    franchiseeMasterTrxDetail.Commission = _masterINVDetailF[0].Commission;
        //        //                    franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetailF[0].CommissionTotal;
        //        //                    franchiseeMasterTrxDetail.ExtraWork = _masterINVDetailF[0].ExtraWork;
        //        //                    franchiseeMasterTrxDetail.ReSell = _masterINVDetailF[0].ReSell;
        //        //                    franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetailF[0].ClientSupplies;

        //        //                }

        //        //                context.PaymentTempDetails.Add(franchiseeMasterTrxDetail);
        //        //                context.SaveChanges();


        //        //                // insert franchisee fees

        //        //                foreach (PaymentTempDetailFee feeDetail in franchiseeFees)
        //        //                {
        //        //                    feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.PaymentTempDetailId; // set the id after insertion

        //        //                    context.PaymentTempDetailFees.Add(feeDetail);
        //        //                    context.SaveChanges();
        //        //                }




        //        //            }


        //        //            //if (ivm.FranchiseePayments != null)
        //        //            //{
        //        //            //    foreach (ManualPaymentFranchiseeViewModel fcvm in ivm.FranchiseePayments)
        //        //            //    {
        //        //            //        // compute franchisee payment fees

        //        //            //        decimal totalFees = 0;

        //        //            //        List<MasterTrxFeeDetail> franchiseeFeeDefs = context.MasterTrxFeeDetails.Where(o => o.MasterTrxDetailId == fcvm.Payment.MasterTrxDetailId).ToList();
        //        //            //        List<PaymentTempDetailFee> franchiseeFees = new List<PaymentTempDetailFee>();

        //        //            //        foreach (MasterTrxFeeDetail feeDef in franchiseeFeeDefs)
        //        //            //        {
        //        //            //            PaymentTempDetailFee feeDetail = new PaymentTempDetailFee();
        //        //            //            if (feeDef.FeePercentage != null) // percentage
        //        //            //            {
        //        //            //                feeDetail.FeePercentage = feeDef.FeePercentage;
        //        //            //                feeDetail.Amount = Math.Round((decimal)(fcvm.Payment.PaymentAmount * feeDetail.FeePercentage / 100.0M), 2);
        //        //            //            }
        //        //            //            else // flat amount
        //        //            //            {
        //        //            //                feeDetail.Amount = feeDef.Amount;
        //        //            //                feeDetail.FeePercentage = null;
        //        //            //            }
        //        //            //            feeDetail.FeeId = feeDef.FeeId;
        //        //            //            feeDetail.AmountTypeListId = 1; // credit
        //        //            //            feeDetail.SourceTypeListId = feeDef.SourceTypeListId;
        //        //            //            feeDetail.CreatedBy = customerPayment.CreatedBy;
        //        //            //            feeDetail.CreatedDate = customerPayment.CreatedDate;
        //        //            //            feeDetail.FranchiseeId = fcvm.FranchiseeId;
        //        //            //            feeDetail.BillingPayId = fcvm.BillingPayId;
        //        //            //            totalFees += feeDetail.Amount ?? 0;

        //        //            //            franchiseeFees.Add(feeDetail);
        //        //            //        }

        //        //            //        // franchisee payment mastertrx

        //        //            //        PaymentTempMasterTrx franchiseeMasterTrx = new PaymentTempMasterTrx();
        //        //            //        franchiseeMasterTrx.ClassId = fcvm.FranchiseeId;
        //        //            //        franchiseeMasterTrx.TypeListId = 2; // franchisee
        //        //            //        franchiseeMasterTrx.MasterTrxTypeListId = 7; // franchisee payment
        //        //            //        franchiseeMasterTrx.TrxDate = PaymentDate;
        //        //            //        franchiseeMasterTrx.RegionId = customerPayment.RegionId;
        //        //            //        franchiseeMasterTrx.StatusId = 1; // open

        //        //            //        franchiseeMasterTrx.BillMonth = PaymentDate.Month;
        //        //            //        franchiseeMasterTrx.BillYear = PaymentDate.Year;
        //        //            //        franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //            //        franchiseeMasterTrx.CreatedBy = LoginUserId;
        //        //            //        franchiseeMasterTrx.CreatedDate = DateTime.Now;
        //        //            //        franchiseeMasterTrx.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //            //        context.PaymentTempMasterTrxes.Add(franchiseeMasterTrx);
        //        //            //        context.SaveChanges();


        //        //            //        // franchisee payment

        //        //            //        string franchiseeNextTrxNumber = franchiseeTransactionNumberConfigViewModel.Prefix.Trim() + franchiseeTransactionNumberConfigViewModel.RegionNumber + string.Format("{0:00}", PaymentDate.Month) + (franchiseeTransactionNumberConfigViewModel.LastNumber + 1).ToString();
        //        //            //        //string franchiseeNextTrxNumber = "BILPMT" + _invoiceOject.InvoiceNo.Trim(); ;



        //        //            //        PaymentTemp franchiseePayment = new PaymentTemp();
        //        //            //        franchiseePayment.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
        //        //            //        franchiseePayment.PaymentId = customerPayment.PaymentTempId;
        //        //            //        franchiseePayment.ClassId = fcvm.FranchiseeId;
        //        //            //        franchiseePayment.TypeListId = 2;
        //        //            //        franchiseePayment.BillingPayId = fcvm.BillingPayId;
        //        //            //        franchiseePayment.RegionId = customerPayment.RegionId;
        //        //            //        franchiseePayment.LineNo = Convert.ToString(fcvm.Payment.LineNo);
        //        //            //        franchiseePayment.Amount = fcvm.Payment.PaymentAmount;
        //        //            //        franchiseePayment.TransactionNumber = franchiseeNextTrxNumber;
        //        //            //        franchiseePayment.TransactionDate = PaymentDate;
        //        //            //        franchiseePayment.CreatedBy = customerPayment.CreatedBy;
        //        //            //        franchiseePayment.CreatedDate = customerPayment.CreatedDate;
        //        //            //        franchiseePayment.IsTARPaid = fcvm.IsTARPaid;
        //        //            //        franchiseePayment.IsTurnaroundPayment = fcvm.IsTurnAroundPayment;
        //        //            //        franchiseePayment.TransactionStatusListId = 1;
        //        //            //        franchiseePayment.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //            //        franchiseePayment.IsBillingFranchisee = true;
        //        //            //        context.PaymentTemps.Add(franchiseePayment);
        //        //            //        context.SaveChanges();

        //        //            //        franchiseeMasterTrx.HeaderId = franchiseePayment.BillingPayId;
        //        //            //        context.SaveChanges();

        //        //            //        if (franchiseeTransactionNumberConfigViewModel != null)
        //        //            //        {
        //        //            //            franchiseeTransactionNumberConfigViewModel.LastNumber = franchiseeTransactionNumberConfigViewModel.LastNumber + 1;
        //        //            //            CompanySvc.SaveTransactionNumberConfig(franchiseeTransactionNumberConfigViewModel.ToModel<TransactionNumberConfig, JKViewModels.Administration.Company.TransactionNumberConfigViewModel>()).ToModel<JKViewModels.Administration.Company.TransactionNumberConfigViewModel, TransactionNumberConfig>();
        //        //            //        }


        //        //            //        decimal paymentMinusFees = Math.Round((decimal)(fcvm.Payment.PaymentAmount - totalFees), 2); // payment amount after taking out fees

        //        //            //        PaymentTempDetail franchiseeMasterTrxDetail = new PaymentTempDetail();
        //        //            //        franchiseeMasterTrxDetail.MasterTrxId = franchiseeMasterTrx.PaymentTempMasterTrxId;
        //        //            //        franchiseeMasterTrxDetail.InvoiceId = ivm.InvoiceId;
        //        //            //        franchiseeMasterTrxDetail.LineNo = fcvm.Payment.LineNo;
        //        //            //        franchiseeMasterTrxDetail.MasterTrxTypeListId = 7; // franchisee payment
        //        //            //        franchiseeMasterTrxDetail.HeaderId = franchiseePayment.PaymentTempId; //customerPayment.PaymentId;
        //        //            //        franchiseeMasterTrxDetail.RegionId = customerPayment.RegionId;

        //        //            //        franchiseeMasterTrxDetail.AmountTypeListId = 2; // debit
        //        //            //        franchiseeMasterTrxDetail.ExtendedPrice = fcvm.Payment.PaymentAmount;
        //        //            //        franchiseeMasterTrxDetail.FeesDetail = true;
        //        //            //        franchiseeMasterTrxDetail.TaxDetail = false;
        //        //            //        franchiseeMasterTrxDetail.TotalFee = totalFees;
        //        //            //        franchiseeMasterTrxDetail.Total = paymentMinusFees;

        //        //            //        franchiseeMasterTrxDetail.CreatedBy = customerPayment.CreatedBy;
        //        //            //        franchiseeMasterTrxDetail.CreatedDate = customerPayment.CreatedDate;
        //        //            //        franchiseeMasterTrxDetail.IsDelete = false;
        //        //            //        franchiseeMasterTrxDetail.PeriodId = (PR != null ? PR.PeriodId : 0);
        //        //            //        franchiseeMasterTrxDetail.TypelistId = 2; // customer
        //        //            //        franchiseeMasterTrxDetail.ClassId = fcvm.FranchiseeId;
        //        //            //        franchiseeMasterTrxDetail.Transactiondate = PaymentDate;



        //        //            //        franchiseeMasterTrxDetail.BPPAdmin = 1;
        //        //            //        franchiseeMasterTrxDetail.AccountRebate = 1;
        //        //            //        franchiseeMasterTrxDetail.AccountRebate = 1;
        //        //            //        franchiseeMasterTrxDetail.Commission = false;
        //        //            //        franchiseeMasterTrxDetail.CommissionTotal = 0;
        //        //            //        franchiseeMasterTrxDetail.FRRevenues = false;
        //        //            //        franchiseeMasterTrxDetail.FRDeduction = false;
        //        //            //        franchiseeMasterTrxDetail.DetailDescription = Note;
        //        //            //        List<MasterTrxDetail> _masterINVDetailF = context.MasterTrxDetails.Where(m => m.IsDelete != true && m.InvoiceId == ivm.InvoiceId && m.LineNo == fcvm.Payment.LineNo && (m.MasterTrxTypeListId == 1 || m.MasterTrxTypeListId == 5)).ToList();
        //        //            //        if (_masterINVDetailF.Count > 0)
        //        //            //        {
        //        //            //            franchiseeMasterTrxDetail.SourceId = _masterINVDetailF[0].SourceId;
        //        //            //            franchiseeMasterTrxDetail.SourceTypeListId = _masterINVDetailF[0].SourceTypeListId;
        //        //            //            franchiseeMasterTrxDetail.ServiceTypeListId = _masterINVDetailF[0].ServiceTypeListId;
        //        //            //            //franchiseeMasterTrxDetail.DetailDescription = _masterINVDetail[0].DetailDescription;
        //        //            //            franchiseeMasterTrxDetail.Quantity = 0;
        //        //            //            franchiseeMasterTrxDetail.CPIPercentage = _masterINVDetailF[0].CPIPercentage;
        //        //            //            franchiseeMasterTrxDetail.TotalTax = 0;
        //        //            //            franchiseeMasterTrxDetail.Commission = _masterINVDetailF[0].Commission;
        //        //            //            franchiseeMasterTrxDetail.CommissionTotal = _masterINVDetailF[0].CommissionTotal;
        //        //            //            franchiseeMasterTrxDetail.ExtraWork = _masterINVDetailF[0].ExtraWork;
        //        //            //            franchiseeMasterTrxDetail.ReSell = _masterINVDetailF[0].ReSell;
        //        //            //            franchiseeMasterTrxDetail.ClientSupplies = _masterINVDetailF[0].ClientSupplies;

        //        //            //        }

        //        //            //        context.PaymentTempDetails.Add(franchiseeMasterTrxDetail);
        //        //            //        context.SaveChanges();


        //        //            //        // insert franchisee fees

        //        //            //        foreach (PaymentTempDetailFee feeDetail in franchiseeFees)
        //        //            //        {
        //        //            //            feeDetail.MasterTrxDetailId = franchiseeMasterTrxDetail.PaymentTempDetailId; // set the id after insertion

        //        //            //            context.PaymentTempDetailFees.Add(feeDetail);
        //        //            //            context.SaveChanges();
        //        //            //        }


        //        //            //    }
        //        //            //}
        //        //        }
        //        //        else
        //        //        {
        //        //            ivm.OverflowAmount = ivm.OverflowAmount - Math.Abs(ivm.InvoicePayment);
        //        //        }

        //        //        if (ivm.OverflowAmount > 0)
        //        //        {
        //        //            PaymentTempDetail masterTrxDetailOP = new PaymentTempDetail();
        //        //            masterTrxDetailOP.MasterTrxId = customerMasterTrx.PaymentTempMasterTrxId;
        //        //            masterTrxDetailOP.InvoiceId = ivm.InvoiceId;
        //        //            masterTrxDetailOP.MasterTrxTypeListId = 17; // customer payment
        //        //            masterTrxDetailOP.HeaderId = customerPayment.PaymentTempId;
        //        //            masterTrxDetailOP.RegionId = customerPayment.RegionId;
        //        //            masterTrxDetailOP.AmountTypeListId = 1; // credit
        //        //            masterTrxDetailOP.FeesDetail = false;
        //        //            masterTrxDetailOP.TaxDetail = true;
        //        //            masterTrxDetailOP.TotalTax = 0;
        //        //            masterTrxDetailOP.TotalFee = 0;
        //        //            masterTrxDetailOP.Quantity = 1;
        //        //            masterTrxDetailOP.UnitPrice = ivm.OverflowAmount;
        //        //            masterTrxDetailOP.Total = ivm.OverflowAmount;
        //        //            masterTrxDetailOP.ExtendedPrice = ivm.OverflowAmount;
        //        //            masterTrxDetailOP.CreatedBy = customerPayment.CreatedBy;
        //        //            masterTrxDetailOP.CreatedDate = customerPayment.CreatedDate;
        //        //            masterTrxDetailOP.IsDelete = false;
        //        //            masterTrxDetailOP.PeriodId = customerMasterTrx.PeriodId;
        //        //            masterTrxDetailOP.TypelistId = 1; // customer
        //        //            masterTrxDetailOP.ClassId = ivm.InvoiceCustomerId;
        //        //            masterTrxDetailOP.Transactiondate = PaymentDate;
        //        //            context.PaymentTempDetails.Add(masterTrxDetailOP);
        //        //            context.SaveChanges();
        //        //        }

        //        //    }








        //        //    return 1;
        //        //}
        //        //else
        //        //{
        //        //    return 0;
        //        //}
        //    //}
        //}

        public PaymentDetailPrintViewModel GetPaymentDetailPrint(int paymentId)
        {
            /*Initialize paymentDetail print vm*/
            var paymentDetailPrint = new PaymentDetailPrintViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;

                /*Get Payment First Data thought ID*/
                var pData = context.Payments.Where(f => f.PaymentId == paymentId).FirstOrDefault();

                /*If payment is not empty*/
                if (pData != null)
                {
                    int TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    int ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;
                    paymentDetailPrint.PaymentId = pData.PaymentId;
                    paymentDetailPrint.PaymentNo = pData.PaymentNo;
                    paymentDetailPrint.PaymentDate = pData.TransactionDate;
                    paymentDetailPrint.TransactionNumber = pData.TransactionNumber;
                    var pymt = context.PaymentMethodLists.Where(f => f.PaymentMethodListId == pData.PaymentMethodListId).FirstOrDefault();
                    paymentDetailPrint.PaymentType = (pymt != null ? pymt.Name : string.Empty);

                    /*Get the Transaction Status through respective Id */
                    var ptr = context.TransactionStatusLists.Where(f => f.TransactionStatusListId == pData.TransactionStatusListId);
                    paymentDetailPrint.Note = (ptr != null ? ptr.FirstOrDefault().Name : string.Empty);

                    /*Get Customer through payment ClassId*/
                    var Customer = context.Customers.Where(w => w.CustomerId == pData.ClassId);
                    if (Customer != null && Customer.Count() > 0)
                    {
                        paymentDetailPrint.CustomerId = Customer.FirstOrDefault().CustomerId;
                        paymentDetailPrint.CustomerNo = Customer.FirstOrDefault().CustomerNo;
                        paymentDetailPrint.CustomerName = Customer.FirstOrDefault().Name;
                    }

                    /*Get Customer Address through payment ClassId , Address Type, Contact Type, And Active */
                    var CusAddress = context.Addresses.Where(w => w.ClassId == pData.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusAddress != null && CusAddress.Count() > 0)
                    {
                        paymentDetailPrint.Address1 = CusAddress.FirstOrDefault().Address1;
                        paymentDetailPrint.Address2 = CusAddress.FirstOrDefault().Address2;
                        paymentDetailPrint.City = CusAddress.FirstOrDefault().City;
                        paymentDetailPrint.PostalCode = CusAddress.FirstOrDefault().PostalCode;

                    }

                    /*Get Customer Phone Number through classId , Phone Type , Contact Type  */
                    var CusPhone = context.Phones.Where(w => w.ClassId == pData.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusPhone != null && CusPhone.Count() > 0)
                    {
                        paymentDetailPrint.Phone = CusPhone.FirstOrDefault().Phone1;
                        paymentDetailPrint.PhoneExt = CusPhone.FirstOrDefault().PhoneExt;
                    }


                    var paymentDetailType = new PaymentDetailType();

                    /*Get Payment Detail Payment (2) type*/
                    var MasterTrxDetails = context.MasterTrxDetails.Where(w => w.HeaderId == paymentId && w.MasterTrxTypeListId == 2);
                    if (MasterTrxDetails != null && MasterTrxDetails.Count() > 0)
                    {
                        paymentDetailPrint.PaymentAmount = (MasterTrxDetails.FirstOrDefault().Total.HasValue ? MasterTrxDetails.FirstOrDefault().Total.Value : 0);

                        paymentDetailType.Type = "Payment";
                        paymentDetailType.Number = pData.PaymentNo;
                        paymentDetailType.AmountTypeListId = MasterTrxDetails.FirstOrDefault().AmountTypeListId;
                        paymentDetailType.Date = MasterTrxDetails.FirstOrDefault().Transactiondate;
                        paymentDetailType.Description = MasterTrxDetails.FirstOrDefault().DetailDescription;
                        paymentDetailType.Amount = (MasterTrxDetails.FirstOrDefault().Total.HasValue ? MasterTrxDetails.FirstOrDefault().Total.Value : 0);

                    }
                    paymentDetailPrint.PaymentDetailType = paymentDetailType;

                }
            }

            return paymentDetailPrint;
        }

        public CreditDetailPrintViewModel GetCreditDetailPrint(int credidId)
        {
            /*Initialize CreditDetail print vm*/
            var crediDetailPrintvm = new CreditDetailPrintViewModel();

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                /*Get Payment First Data thought ID*/
                var cData = context.Credits.Where(f => f.CreditId == credidId).FirstOrDefault();

                /*If payment is not empty*/
                if (cData != null)
                {
                    int TypeListId = (int)JKApi.Business.Enumeration.TypeList.Customer;
                    int ContactTypeListId = (int)JKApi.Business.Enumeration.ContactTypeList.Main;

                    crediDetailPrintvm.CreditNo = cData.TransactionNumber;
                    crediDetailPrintvm.CreditDate = cData.TransactionDate;
                    crediDetailPrintvm.Description = cData.CreditDescription;

                    /*Get Customer through payment ClassId*/
                    var Customer = context.Customers.Where(w => w.CustomerId == cData.ClassId);
                    if (Customer != null && Customer.Count() > 0)
                    {
                        crediDetailPrintvm.CustomerNo = Customer.FirstOrDefault().CustomerNo;
                        crediDetailPrintvm.CustomerName = Customer.FirstOrDefault().Name;
                    }

                    /*Get Customer Address through payment ClassId , Address Type, Contact Type, And Active */
                    var CusAddress = context.Addresses.Where(w => w.ClassId == cData.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusAddress != null && CusAddress.Count() > 0)
                    {
                        crediDetailPrintvm.Address1 = CusAddress.FirstOrDefault().Address1;
                        crediDetailPrintvm.Address2 = CusAddress.FirstOrDefault().Address2;
                        crediDetailPrintvm.City = CusAddress.FirstOrDefault().City;
                        crediDetailPrintvm.PostalCode = CusAddress.FirstOrDefault().PostalCode;

                    }

                    /*Get Customer Phone Number through classId , Phone Type , Contact Type  */
                    var CusPhone = context.Phones.Where(w => w.ClassId == cData.ClassId && w.TypeListId == TypeListId && w.ContactTypeListId == ContactTypeListId && w.IsActive == true).OrderByDescending(o => o.CreatedDate);
                    if (CusPhone != null && CusPhone.Count() > 0)
                    {
                        crediDetailPrintvm.Phone = CusPhone.FirstOrDefault().Phone1;
                        crediDetailPrintvm.PhoneExt = CusPhone.FirstOrDefault().PhoneExt;
                    }

                    CustomerCreditPaymentType CustomerCreditPaymentType = new CustomerCreditPaymentType();

                    var MasterTrxDetails2 = context.MasterTrxDetails.Where(w => w.InvoiceId == cData.InvoiceId && w.MasterTrxTypeListId == (int)JKApi.Business.Enumeration.MasterTrxTypeList.CustomerCredit);
                    if (MasterTrxDetails2 != null && MasterTrxDetails2.Count() > 0)
                    {
                        crediDetailPrintvm.CreditAmount = (MasterTrxDetails2.FirstOrDefault().Total.HasValue ? MasterTrxDetails2.FirstOrDefault().Total.Value : 0);
                        CustomerCreditPaymentType.Type = "Invoice Credit Customer";
                        CustomerCreditPaymentType.Date = MasterTrxDetails2.FirstOrDefault().Transactiondate;
                        CustomerCreditPaymentType.Number = cData.TransactionNumber;
                        CustomerCreditPaymentType.Description = cData.CreditDescription;
                        CustomerCreditPaymentType.Amount = (MasterTrxDetails2.FirstOrDefault().Total.HasValue ? MasterTrxDetails2.FirstOrDefault().Total.Value : 0);

                    }
                    crediDetailPrintvm.CustomerCreditPaymentType = CustomerCreditPaymentType;
                }
            }
            return crediDetailPrintvm;
        }



        public List<MonthlyBillRunResultViewModel> GenerateMonthlyBillRun(int month, int year, string selectedRegionIds = "")
        {
            List<MonthlyBillRunResultViewModel> lstMonthlyBill = new List<MonthlyBillRunResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {


                foreach (string rID in selectedRegionIds.Split(','))
                {
                    var parmas = new DynamicParameters();
                    parmas.Add("@RegionId", rID);
                    parmas.Add("@BillMonth", month);
                    parmas.Add("@BillYear", year);
                    parmas.Add("@CreatedBy", LoginUserId);
                    parmas.Add("@BatchId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    int retVal = conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_Invoice", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure);
                    retVal = conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_BillingPay", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure);
                    retVal = conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_Lease", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure);
                    retVal = conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_FinderFee", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure);
                    retVal = conn.Execute("dbo.portal_spCreate_AR_GenerateBillRun_FranchiseeManualTransaction", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure);


                }




                var parmas1 = new DynamicParameters();
                parmas1.Add("@RegionIds", (selectedRegionIds != "" ? selectedRegionIds : SelectedRegionId.ToString()));
                parmas1.Add("@BillMonth", month);
                parmas1.Add("@BillYear", year);

                lstMonthlyBill = conn.Query<MonthlyBillRunResultViewModel>("dbo.portal_spGet_AR_MonthlyBillRunGenerateList", parmas1, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();

            }

            return lstMonthlyBill;


        }

        public List<portal_spCreate_AR_MonthlyBillRunGenerateList_Result> GetMonthlyBillRunData(int month, int year)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<portal_spCreate_AR_MonthlyBillRunGenerateList_Result> lstMonthlyBillRunGenerate = context.portal_spCreate_AR_MonthlyBillRunGenerateList(SelectedRegionId.ToString(), month, year, 0, 0, 0, 0).ToList();
                return lstMonthlyBillRunGenerate;
            }
        }

        public List<MonthlyBillRunResultViewModel> GetMonthlyBillRunResultData(int month, int year, string selectedRegionIds = "")
        {

            List<MonthlyBillRunResultViewModel> lstMonthlyBill = new List<MonthlyBillRunResultViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", selectedRegionIds);
                parmas.Add("@BillMonth", month);
                parmas.Add("@BillYear", year);

                lstMonthlyBill = conn.Query<MonthlyBillRunResultViewModel>("dbo.portal_spGet_AR_MonthlyBillRunGenerateList", parmas, commandTimeout: 1000, commandType: CommandType.StoredProcedure).ToList();

            }

            return lstMonthlyBill;
        }



        public List<AgingReportViewModel> AgingReport(AgingReportViewModel agingReportViewModel)
        {
            List<AgingReportViewModel> agingList = new List<AgingReportViewModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                // code by ajay prakash
                var monthToInclude = agingReportViewModel.monthsToInclude > 0 ? -agingReportViewModel.monthsToInclude : 0;
                var dateTo = agingReportViewModel.ReportDate.Value;
                var dateFrom = dateTo.AddMonths((int)monthToInclude);
                if (monthToInclude == 0)
                {
                    dateFrom = new DateTime(dateTo.Year, dateTo.Month, 1);
                }

                try
                {
                    var query = @"exec dbo.portal_spGet_AR_AgingReport @isNonChargebackOnly,@isSummaryView, @invDateFrom,@invDateTo,@agingDate,@paymentDate,@monthsToInclude,@regionIds, @IsMonthView";
                    //var query = @"exec dbo.portal_spGet_AR_AgingReport_test @isNonChargebackOnly,@isSummaryView, @invDateFrom,@invDateTo,@agingDate,@paymentDate,@monthsToInclude,@regionIds";
                    agingList = conn.Query<AgingReportViewModel>(query, new
                    {
                        isNonChargebackOnly = agingReportViewModel.isNonChargebackOnly,
                        isSummaryView = agingReportViewModel.isSummaryView,
                        invDateFrom = dateFrom,
                        invDateTo = dateTo,
                        agingDate = agingReportViewModel.agingDate,
                        paymentDate = agingReportViewModel.PaymentDate,
                        monthsToInclude = monthToInclude,
                        regionIds = agingReportViewModel.regionIds,
                        IsMonthView = agingReportViewModel.IsMonthView
                    }).ToList();

                }
                catch (Exception ex)
                {

                }



                // old code
                //var query = @"exec dbo.portal_spGet_AR_AgingReport @isNonChargebackOnly,@isSummaryView, @agingDate,@monthsToInclude,@regionIds";


                //agingList = conn.Query<AgingReportViewModel>(query, new
                //{
                //    agingDate = agingReportViewModel.agingDate,
                //    monthsToInclude = agingReportViewModel.monthsToInclude,
                //    regionIds = agingReportViewModel.regionIds,
                //    isNonChargebackOnly = agingReportViewModel.isNonChargebackOnly,
                //    isSummaryView = agingReportViewModel.isSummaryView
                //}).ToList();


                //var query = @"exec dbo.spGet_AgingReport @isNonChargebackOnly,@isSummaryView, @agingDate,@paymentDateFrom,@paymentDateTo,@monthsToInclude,@orderByList,@includeList,@balanceList,@regionIds";


                //agingList = conn.Query<AgingReportViewModel>(query, new
                //{
                //    agingDate = agingReportViewModel.agingDate,
                //    paymentDateFrom = agingReportViewModel.paymentDateFrom,
                //    paymentDateTo = agingReportViewModel.paymentDateTo,
                //    monthsToInclude = agingReportViewModel.monthsToInclude,
                //    orderByList = agingReportViewModel.orderByList,
                //    regionIds = agingReportViewModel.regionIds,
                //    includeList = agingReportViewModel.includeList,
                //    balanceList = agingReportViewModel.balanceList,
                //    isNonChargebackOnly = agingReportViewModel.isNonChargebackOnly,
                //    isSummaryView = agingReportViewModel.isSummaryView
                //}).ToList();

            }
            return agingList;


        }
        public List<AgingReportViewModel> AgingDataForCollectionCall(int CustomerId)
        {
            List<AgingReportViewModel> agingList = new List<AgingReportViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"exec dbo.portal_spGet_AR_AgingDataForCollectionCall @CustomerId";
                agingList = conn.Query<AgingReportViewModel>(query, new
                {
                    CustomerId = CustomerId
                }).ToList();
            }
            return agingList;
        }

        public IEnumerable<ARCustomerWithCreditListViewModel> GetCustomerWiseCreditList(string regionIds, DateTime? spnStartDate = null, DateTime? spnEndDate = null, int month = 0, int year = 0)
        {
            if (string.IsNullOrEmpty(regionIds) || regionIds == "null" || regionIds == null)
            {
                regionIds = "0";
            }

            List<ARCustomerWithCreditListViewModel> result = new List<ARCustomerWithCreditListViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionIds);
                parmas.Add("@StartDate", spnStartDate);
                parmas.Add("@EndDate", spnEndDate);
                parmas.Add("@BillMonth", month);
                parmas.Add("@BillYear", year);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_CustomerWiseCreditList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        result = multipleresult.Read<ARCustomerWithCreditListViewModel>().ToList();
                    }
                }
                return result;
            }

        }


        public List<ARLogListFinalViewModel> GetARLogListData(string regionIds, DateTime? CreatedOn)
        {
            List<ARLogListFinalViewModel> lstARLogList = new List<ARLogListFinalViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionIds", regionIds);
                parmas.Add("@CreatedDate", CreatedOn);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_ARLogList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        List<ARLogListViewModel> lstResult = multipleresult.Read<ARLogListViewModel>().ToList();
                        List<ARLogListViewModel> lstResult1 = multipleresult.Read<ARLogListViewModel>().ToList();

                        var query = (from r in lstResult orderby r.RegionId select r.RegionId).Distinct();
                        ARLogListFinalViewModel oARLog;
                        foreach (string rId in regionIds.Split(','))
                        {
                            if (rId != "")
                            {
                                int _rId = int.Parse(rId);
                                oARLog = new ARLogListFinalViewModel();
                                oARLog.RegionId = _rId;
                                oARLog.TotalAmount = lstResult.Where(o => o.RegionId == _rId).Sum(p => p.InvoiceAmount);
                                //oARLog.TotalDeposit = lstResult.Where(o => o.RegionId == rId).Sum(p => p.CheckAmount);
                                oARLog.TotalDeposit = lstResult.Where(o => o.RegionId == _rId).Select(s => new { s.CheckNumber, s.CheckAmount }).Distinct().Sum(j => j.CheckAmount);
                                oARLog.TotalBalance = lstResult.Where(o => o.RegionId == _rId).Sum(p => p.InvoiceBalance);
                                oARLog.ARLogs = lstResult.Where(o => o.RegionId == _rId).ToList();
                                oARLog.AROtherLogs = lstResult1.Where(o => o.RegionId == _rId).ToList();
                                oARLog.TotalDeposit = oARLog.TotalDeposit + lstResult1.Where(o => o.RegionId == _rId).Select(s => new { s.CheckNumber, s.CheckAmount }).Distinct().Sum(j => j.CheckAmount);
                                lstARLogList.Add(oARLog);
                            }
                        }

                    }
                }
            }
            return lstARLogList;
        }



        public ClosedPeriodViewModel GetClosedPeriodForClose(int regionId)
        {
            List<ClosedPeriodViewModel> lstARInvoiceListView = new List<ClosedPeriodViewModel>();
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {

                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_COM_ValidatedClosePeriod", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        lstARInvoiceListView = multipleresult.Read<ClosedPeriodViewModel>().ToList();
                    }
                }
                return lstARInvoiceListView.FirstOrDefault();
            }

        }

        public bool UpdateStatusClosedPeriod(int PeriodClosedId)
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


        public bool HaveChargebackforInvoiceId(int InvoiceId)
        {
            bool _retVal = false;
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@InvoiceId", InvoiceId);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AP_HaveChargebackforInvoiceId", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        if (multipleresult.Read().ToList().Count > 0)
                            _retVal = true;
                        else
                            _retVal = false;
                    }
                }

            }
            return _retVal;
        }

        public List<OverPaymentListViewModel> OverPaymentReportData(string regionIds, DateTime? FromDate, DateTime? ToDate, string SearchText)
        {
            List<OverPaymentListViewModel> OverPaymentList = new List<OverPaymentListViewModel>();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@RegionId", regionIds);
                parmas.Add("@FromDate", FromDate);
                parmas.Add("@ToDate", ToDate);
                parmas.Add("@SearchText", SearchText);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGet_AR_OverflowPaymentList", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        OverPaymentList = multipleresult.Read<OverPaymentListViewModel>().ToList();
                    }
                }

            }
            return OverPaymentList;


        }

        public string GetCustomerBillingEmail(int Id)
        {
            string EmailAddress = "";
            var context = new jkDatabaseEntities();
            var Data = context.Emails.Where(w => w.TypeListId == (int)(JKApi.Business.Enumeration.TypeList.Customer) && w.ContactTypeListId == (int)(JKApi.Business.Enumeration.ContactTypeList.BillingContact) && w.ClassId == Id && w.IsActive == true).ToList();
            if (Data != null && Data.Count > 0)
            {
                EmailAddress = Data.FirstOrDefault().EmailAddress;
            }
            return EmailAddress;
        }

        public IEnumerable<InvoiceDetailsForNumber> GetInvoiceStatusWiseInvoiceNumber(int month, int year, string searchtext, int v, string r)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC portal_spGet_AR_InvoiceNumber @RegionId,@BillMonth,@BillYear,@SearchText,@InvoiceTypeListId";
                return conn.Query<InvoiceDetailsForNumber>(query, new
                {
                    RegionId = r,
                    BillMonth = month,
                    BillYear = year,
                    SearchText = searchtext,
                    InvoiceTypeListId = v

                });
            }
        }

        public IEnumerable<PastDueViewModel> GetAllPastDueStatement(DateTime? reportDate, int? monthsToInclude, string regionIds)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC portal_spGet_AR_PastDueStatement @reportDate,@monthsToInclude,@regionIds";
                return conn.Query<PastDueViewModel>(query, new
                {
                    reportDate = reportDate,
                    monthsToInclude = monthsToInclude,
                    regionIds = regionIds
                });
            }
        }
        public PastDueStatementDetailModel GetPastDueStatementDetailsPopup(int Id, DateTime? reportDate)
        {
            PastDueStatementDetailModel modelView = new PastDueStatementDetailModel();
            var context = new jkDatabaseEntities();
            var Customer = context.Customers.Where(w => w.CustomerId == Id).FirstOrDefault();
            if (Customer != null && Customer.CustomerId > 0)
            {
                modelView.CustomerId = Customer.CustomerId;
                modelView.CustomerName = Customer.Name;
                modelView.CustomerNo = Customer.CustomerNo;
                modelView.RegionId = (Customer.RegionId.HasValue ? Customer.RegionId.Value : 0);
            }
            //Address  1
            var Address = context.Addresses.OrderByDescending(s => s.AddressId).Where(w => w.TypeListId == (int)(JKApi.Business.Enumeration.TypeList.Customer) && w.ContactTypeListId == (int)(JKApi.Business.Enumeration.ContactTypeList.Main) && w.ClassId == Id && w.IsActive == true).FirstOrDefault();
            if (Address != null && Address.AddressId > 0)
            {
                modelView.Address1 = Address.Address1;
                modelView.Address2 = Address.Address2;
                modelView.City = Address.City;
                modelView.StateName = Address.StateName;
                modelView.PostalCode = Address.PostalCode;
            }

            var Phone = context.Phones.OrderByDescending(s => s.PhoneId).Where(w => w.TypeListId == (int)(JKApi.Business.Enumeration.TypeList.Customer) && w.ContactTypeListId == (int)(JKApi.Business.Enumeration.ContactTypeList.Main) && w.ClassId == Id && w.IsActive == true).FirstOrDefault();
            if (Phone != null && Phone.PhoneId > 0)
            {
                modelView.Phone = Phone.Phone1;
                modelView.PhoneExt = Phone.PhoneExt;

            }
            //Address 2
            var Address2 = context.Addresses.OrderByDescending(s => s.AddressId).Where(w => w.TypeListId == (int)(JKApi.Business.Enumeration.TypeList.Customer) && w.ContactTypeListId == (int)(JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation) && w.ClassId == Id && w.IsActive == true).FirstOrDefault();
            if (Address2 != null && Address.AddressId > 0)
            {
                modelView.SoldAddress1 = Address2.Address1;
                modelView.SoldAddress2 = Address2.Address2;
                modelView.SoldCity = Address2.City;
                modelView.SoldStateName = Address2.StateName;
                modelView.SoldPostalCode = Address2.PostalCode;
            }

            var Phone2 = context.Phones.OrderByDescending(s => s.PhoneId).Where(w => w.TypeListId == (int)(JKApi.Business.Enumeration.TypeList.Customer) && w.ContactTypeListId == (int)(JKApi.Business.Enumeration.ContactTypeList.PhysicalLocation) && w.ClassId == Id && w.IsActive == true).FirstOrDefault();
            if (Phone2 != null && Phone.PhoneId > 0)
            {
                modelView.SoldPhone = Phone2.Phone1;
                modelView.SoldPhoneExt = Phone2.PhoneExt;

            }

            List<PastDueStatementFranchiseeModel> PastDueStatementFranchiseeModel = new List<PastDueStatementFranchiseeModel>();
            var lst = (from f in context.Franchisees
                       join d in context.Distributions
                       on f.FranchiseeId equals d.FranchiseeId
                       where d.CustomerId == Id
                       select new { f.FranchiseeId, f.FranchiseeNo, f.Name }).ToList();
            if (lst != null && lst.Count() > 0)
            {
                foreach (var item in lst)
                {
                    PastDueStatementFranchiseeModel objModel = new JKViewModels.AccountReceivable.PastDueStatementFranchiseeModel();
                    objModel.FranchiseeId = item.FranchiseeId;
                    objModel.FranchiseeNo = item.FranchiseeNo;
                    objModel.FranchiseeName = item.Name;
                    PastDueStatementFranchiseeModel.Add(objModel);
                }
                modelView.PastDueStatementFranchisee = PastDueStatementFranchiseeModel;
            }
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var query = @"EXEC portal_spGet_AR_PastDueStatementInvoiceDetails @CustomerId,@reportDate";
                var DataResult = conn.Query<PastDueStatementInvoiceModel>(query, new
                {
                    CustomerId = Id,
                    reportDate = reportDate
                });
                if (DataResult != null && DataResult.Count() > 0)
                {
                    modelView.PastDueStatementInvoices = DataResult.ToList();
                }
            }
            return modelView;
        }

        public RegionSetting GetAdditionalBilling(int RegId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var Data = context.RegionSettings.Where(x => x.RegionId == RegId && x.RegionConfigurationId == 25).FirstOrDefault();
                return Data;
            }
        }



        public OverPaymentCustomerInvoiceViewModel GetOverPaymentCustomerInvoiceDetail(int invoiceId, int OverflowId = 0, decimal OverflowAmount = 0)
        {
            OverPaymentCustomerInvoiceViewModel OverPaymentCustomerInvoiceDetail = new OverPaymentCustomerInvoiceViewModel();

            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@InvoiceId", invoiceId);
                parmas.Add("@OverflowId", OverflowId);
                parmas.Add("@OverflowAmount", OverflowAmount);
                using (var multipleresult = conn.QueryMultiple("dbo.portal_spGET_AR_OverPaymentCustomerInvoiceDetail", parmas, commandType: CommandType.StoredProcedure))
                {
                    if (multipleresult != null)
                    {
                        var lst = multipleresult.Read<OverPaymentCustomerInvoiceViewModel>().ToList();
                        if (lst.Count > 0)
                            OverPaymentCustomerInvoiceDetail = lst.FirstOrDefault();
                    }
                }

            }
            return OverPaymentCustomerInvoiceDetail;
        }


        public bool InsertGeneralLedgerTrx(int MasterTrxTypeListId, decimal Amount, decimal TaxAmount, DateTime TrxDate, int RegionId, int? MasterTrxId, int? AccountTypeListId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@MasterTrxTypeListId", MasterTrxTypeListId);
                parmas.Add("@Amount", Amount);
                parmas.Add("@TaxAmount", TaxAmount);
                parmas.Add("@MasterTrxId", MasterTrxId);
                parmas.Add("@AccountTypeListId", AccountTypeListId);
                parmas.Add("@TrxDate", TrxDate);
                parmas.Add("@RegionId", RegionId);
                parmas.Add("@CreatedBy", LoginUserId);

                var result = conn.Execute("dbo.portal_spCreate_GeneralLedgerTrx", parmas, commandType: CommandType.StoredProcedure);

            }
            return true;
        }

        public bool PostPayment2GeneralLedgerTrx_Checkbook(int _regionId)
        {
            using (SqlConnection conn = new SqlConnection(SQLHelper.ConnectionStringTransaction))
            {
                var parmas = new DynamicParameters();
                parmas.Add("@PostDate", DateTime.Now.ToString("MM/dd/yyy"));
                parmas.Add("@RegionId", _regionId);

                var result = conn.Execute("dbo.spCreate_AR_PaymentPostInCheckbookAndGL", parmas, commandType: CommandType.StoredProcedure);

            }
            return true;
        }



    }

    #region ViewModels
    public class BankDetailByRouting
    {
        public string telephone { get; set; }
        public int code { get; set; }
        public string institution_status_code { get; set; }
        public string address { get; set; }
        public string data_view_code { get; set; }
        public string change_date { get; set; }
        public string state { get; set; }
        public string office_code { get; set; }
        public string city { get; set; }
        public string rn { get; set; }
        public string new_routing_number { get; set; }
        public string record_type_code { get; set; }
        public string zip { get; set; }
        public string routing_number { get; set; }
        public string message { get; set; }
        public string customer_name { get; set; }
    }
    public class InvoiceDetailViewModel
    {
        public vw_InvoiceDetailViewModel InvoiceDetail { get; set; }
        public List<vw_InvoiceContractDetailList> InvoiceDetailItems { get; set; }
        public List<InvoiceFranchiseeBillingDetailViewModel> FranchiseeBillingDetails { get; set; }
        public List<InvoiceTransactionHistoryViewModel> lstInvoiceTransactionHistory { get; set; }
        public Region InvoiceRegion { get; set; }
        public string FranchiseeBill { get; set; }
        public int BillingPayId { get; set; }

        public int MasterTrxTypeListId { get; set; }


    }
    public class vw_InvoiceDetailViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string Message { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string CustomerNo { get; set; }
        public string Customer { get; set; }
        public Nullable<int> AddressId_SoldTo { get; set; }
        public Nullable<int> AddressId_For { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string B_Name { get; set; }
        public string B_Attention { get; set; }
        public string B_Address1 { get; set; }
        public string B_Address2 { get; set; }
        public string B_City { get; set; }
        public string B_State { get; set; }
        public string B_PostalCode { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Phone { get; set; }
        public string B_Phone { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public string EmailAddress { get; set; }
        public string InvoiceMessage { get; set; }

        public int? ConsolidatedInvoiceId { get; set; }

        public int? MasterTrxTypeListId { get; set; }
    }

    public class ConsolidatedInvoiceDetailViewModel
    {

        public int ConsolidatedInvoiceId { get; set; }
        public string ConsolidatedInvoiceNo { get; set; }
        public Nullable<System.DateTime> ConsolidatedInvoiceDate { get; set; }
        public Nullable<System.DateTime> ConsolidatedInvoiceDueDate { get; set; }
        public Nullable<int> RegionId { get; set; }

        public Nullable<int> CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> AddressId_SoldTo { get; set; }
        public Nullable<int> AddressId_For { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string B_Name { get; set; }
        public string B_Attention { get; set; }
        public string B_Address1 { get; set; }
        public string B_Address2 { get; set; }
        public string B_City { get; set; }
        public string B_State { get; set; }
        public string B_PostalCode { get; set; }
        public string B_Phone { get; set; }

        public Region InvoiceRegion { get; set; }
        public List<ConsolidatedInvoiceDetailInvoiceViewModel> Invoices { get; set; }
        public List<ConsolidatedInvoiceTransactionHistoryModel> ConsolidatedInvoiceHistoryListModel { get; set; }

    }
    public class ConsolidatedInvoiceTransactionHistoryModel
    {
        public string InvoiceNo { get; set; }
        public List<InvoiceTransactionHistoryViewModel> lstInvoiceTransactionHistory { get; set; }
    }
    public class ConsolidatedInvoiceDetailInvoiceViewModel
    {
        public Nullable<int> CustomerId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string InvoiceClass { get; set; }
        public string InvoiceDescription { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> ExtendedPrice { get; set; }
        public decimal TAXAmount { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> ServiceTypeListId { get; set; }
        public int MasterTrxTypeListId { get; set; }
    }

    public class PaymentDetailPrintViewModel
    {
        public int PaymentId { get; set; }
        public string PaymentNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public decimal PaymentAmount { get; set; }

        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }

        public PaymentDetailType PaymentDetailType { get; set; }

        //public List<InvoiceTransactionHistoryViewModel> InvoiceTransactionHistoryList { get; set; }
        //public List<InvoiceContractDetailListViewModel> InvoiceDetailItems { get; set; }

        public string Note { get; set; }
        public string TransactionNumber { get; set; }

    }
    public class CreditDetailPrintViewModel
    {
        public int CreditId { get; set; }
        public string CreditNo { get; set; }
        public DateTime? CreditDate { get; set; }
        public decimal CreditAmount { get; set; }

        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }

        public CustomerCreditPaymentType CustomerCreditPaymentType { get; set; }

        //public List<InvoiceTransactionHistoryViewModel> InvoiceTransactionHistoryList { get; set; }
        //public List<InvoiceContractDetailListViewModel> InvoiceDetailItems { get; set; }

        public string Description { get; set; }
    }
    public class ManualInvoiceDetailViewModel
    {
        public List<vw_AR_CustomerTransaction> CustomerTransactionItems { get; set; }
        public List<vw_AR_FrenchiseeTransaction> FrenchiseeTransactionItems { get; set; }
        public List<PendingDashboardDataModel> PendingDashboardDataModel { get; set; }
        public int USERID { get; set; }
    }
    public class GetCustomerViewModwl
    {
        public string Name { get; set; }
        public string InvoiceNo { get; set; }
    }
    public class CreditDetailViewModel
    {
        public int CreditId { get; set; }
        public Credit Credit { get; set; }
        public int CreditBillMonth { get; set; }
        public int CreditBillYear { get; set; }
        public decimal UnappliedCreditAmount { get; set; }
        public List<decimal> CreditAmounts { get; set; }
        public InvoiceDetailViewModel Invoice { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal InvoiceBalance { get; set; }
        public decimal InvoiceOpenCRBalance { get; set; }
        public List<CreditFranchiseeDetailViewModel> FranchiseeItems { get; set; }
    }
    public class CreditFranchiseeDetailViewModel
    {
        public int CreditFranchiseeId { get; set; }
        public decimal CreditAmount { get; set; }

        public portal_spGet_AR_InvoiceFranchiseeBalance_Result InvoiceFranchiseeDetailItem { get; set; }
    }
    #endregion
}
