using JKApi.Service.ServiceContract.CustomerInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKApi.Data.DAL;
using JK.Repository.Uow;
using JKViewModels.Administration.Company;

namespace JKApi.Service.Service.CustomerInvoice
{
    public class CustomerInvoiceService : BaseService, ICustomerInvoiceService
    {

        #region ConstructorCalls

        public CustomerInvoiceService(IJKEfUow uow)
        {
            Uow = uow;
        }

        #endregion




        public List<CommonLadgerAccountViewModel> GetLedgerAccountList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<CommonLadgerAccountViewModel> lstoCommonLadgerAccountViewModel = new List<CommonLadgerAccountViewModel>();
                lstoCommonLadgerAccountViewModel.AddRange(
                (from LAcct in context.LedgerAccts
                 join GLType in context.GLAccountTypeLists on LAcct.GLAccountTypeListId equals GLType.GLAccountTypeListId
                 select new { LAcct, GLType }).Select(i => new CommonLadgerAccountViewModel
                 {
                     AccountId = i.LAcct.LedgerAcctId,
                     PerentAccountId = 0,
                     AccountNo = i.LAcct.GL_Number,
                     Name = i.LAcct.GL_Name,
                     Description = i.LAcct.LedgerAcctDescription,// i.LAcct.Description,
                     IsSubAccount = false,
                     Balance = 0,
                     BalanceTotal = 0,
                     Attach = "",
                     Type = i.GLType.Name,
                     TypeId = i.GLType.GLAccountTypeListId
                 }).ToList());

                lstoCommonLadgerAccountViewModel.AddRange(
                (from LAcct in context.LedgerAccts
                 join GLType in context.GLAccountTypeLists on LAcct.GLAccountTypeListId equals GLType.GLAccountTypeListId
                 join LSAcct in context.LedgerSubAccts on LAcct.LedgerAcctId equals LSAcct.LedgerAcctId
                 select new { LSAcct, GLType }).Select(i => new CommonLadgerAccountViewModel
                 {
                     AccountId = i.LSAcct.LedgerSubAcctId,
                     PerentAccountId = i.LSAcct.LedgerAcctId,
                     AccountNo = i.LSAcct.GLSubAcct_Number.ToString(),
                     Name = i.LSAcct.GLSubAcct_Name,
                     Description = i.LSAcct.LedgerSubAcctDescription,
                     IsSubAccount = true,
                     Balance =0,
                     BalanceTotal = 0,
                     Attach = "",
                     Type = i.GLType.Name,
                     TypeId = i.GLType.GLAccountTypeListId
                 }).ToList());
                return lstoCommonLadgerAccountViewModel;
            }
        }

        public List<CommonLadgerAccountViewModel> GetChartofAccountList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<CommonLadgerAccountViewModel> lstoCommonLadgerAccountViewModel = new List<CommonLadgerAccountViewModel>();
                lstoCommonLadgerAccountViewModel.AddRange(
                (from LAcct in context.vw_ChartofAccounts
                 select new { LAcct }).Select(i => new CommonLadgerAccountViewModel
                 {
                     AccountId = i.LAcct.AccountId,
                     PerentAccountId = i.LAcct.PerentAccountId,
                     AccountNo = i.LAcct.AccountNo.ToString(),
                     Name = i.LAcct.Name,
                     Description = i.LAcct.Description,
                     IsSubAccount = false,
                     Balance = (decimal)i.LAcct.BalanceTotal,// i.LAcct.IsSubAccount==true?(decimal)i.LAcct.BalanceTotal:0,
                     BalanceTotal = (decimal)i.LAcct.BalanceTotal,
                     Attach = "",
                     Type = i.LAcct.Name,
                     TypeId = i.LAcct.TypeId
                 }).ToList().OrderBy(c=>c.AccountNo));

                return lstoCommonLadgerAccountViewModel;
            }
        }

        public CommonLadgerAccountViewModel GetLedgerAccountDetailforEdit(int accid, bool issubaccount)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                List<CommonLadgerAccountViewModel> lstoCommonLadgerAccountViewModel = new List<CommonLadgerAccountViewModel>();


                lstoCommonLadgerAccountViewModel.AddRange(
                       (from LAcct in context.LedgerAccounts 
                        join GLType in context.GLAccountTypeLists on LAcct.GLAccountTypeListId equals GLType.GLAccountTypeListId
                        where LAcct.LedgerAccountId == accid
                        select new { LAcct, GLType }).Select(i => new CommonLadgerAccountViewModel
                        {
                            AccountId = i.LAcct.LedgerAccountId,
                            PerentAccountId = i.LAcct.ParentLedgerAccountId,
                            AccountNo = i.LAcct.LedgerNumber,
                            Name = i.LAcct.LedgerName,
                            Description = i.LAcct.LedgerDescription,
                            IsSubAccount = i.LAcct.ParentLedgerAccountId>0?true:false,
                            Balance = 0,
                            BalanceTotal = 0,
                            Attach = "",
                            Type = i.GLType.Name,
                            TypeId = i.GLType.GLAccountTypeListId
                        }).ToList());

                //if (issubaccount)
                //{

                //    lstoCommonLadgerAccountViewModel.AddRange(
                //        (from LAcct in context.LedgerAccts
                //         join GLType in context.GLAccountTypeLists on LAcct.GLAccountTypeListId equals GLType.GLAccountTypeListId
                //         join LSAcct in context.LedgerSubAccts on LAcct.LedgerAcctId equals LSAcct.LedgerAcctId
                //         where LSAcct.LedgerSubAcctId == accid
                //         select new { LSAcct, GLType }).Select(i => new CommonLadgerAccountViewModel
                //         {
                //             AccountId = i.LSAcct.LedgerSubAcctId,
                //             PerentAccountId = i.LSAcct.LedgerAcctId,
                //             AccountNo = i.LSAcct.GLSubAcct_Number.ToString(),
                //             Name = i.LSAcct.GLSubAcct_Name,
                //             Description = i.LSAcct.LedgerSubAcctDescription,
                //             IsSubAccount = true,
                //             Balance = 0,
                //             BalanceTotal = 0,
                //             Attach = "",
                //             Type = i.GLType.Name,
                //             TypeId = i.GLType.GLAccountTypeListId
                //         }).ToList());

                //}
                //else
                //{

                //    lstoCommonLadgerAccountViewModel.AddRange(
                //    (from LAcct in context.LedgerAccts
                //     join GLType in context.GLAccountTypeLists on LAcct.GLAccountTypeListId equals GLType.GLAccountTypeListId
                //     where LAcct.LedgerAcctId == accid
                //     select new { LAcct, GLType }).Select(i => new CommonLadgerAccountViewModel
                //     {
                //         AccountId = i.LAcct.LedgerAcctId,
                //         PerentAccountId = 0,
                //         AccountNo = i.LAcct.GL_Number,
                //         Name = i.LAcct.GL_Name,
                //         Description = i.LAcct.LedgerAcctDescription,// i.LAcct.Description,
                //         IsSubAccount = false,
                //         Balance = 0,
                //         BalanceTotal = 0,
                //         Attach = "",
                //         Type = i.GLType.Name,
                //         TypeId = i.GLType.GLAccountTypeListId
                //     }).ToList());
                //}

                return lstoCommonLadgerAccountViewModel.FirstOrDefault();
            }
        }

        public List<GeneralLadgerAccountListViewModel> GetLedgerMasterAccountList()
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var query = (from LAcct in context.LedgerAccounts
                             join GLType in context.GLAccountTypeLists on LAcct.GLAccountTypeListId equals GLType.GLAccountTypeListId
                             where LAcct.ParentLedgerAccountId == 0
                             select new
                             {
                                 LedgerAcctId = LAcct.LedgerAccountId,
                                 GLSubAcct_Number = LAcct.LedgerNumber,
                                 GLSubAcct_Name = LAcct.LedgerName,
                                 GLTypeName = GLType.Name
                             }).Select(i => new GeneralLadgerAccountListViewModel
                             {
                                 LedgerAcctId = i.LedgerAcctId,
                                 AccountNo = i.GLSubAcct_Number.ToString(),
                                 Name = i.GLSubAcct_Name,
                                 Type = i.GLTypeName
                             }).ToList();



                return query.ToList<GeneralLadgerAccountListViewModel>();
            }
        }

        public GeneralLedger DeleteGeneralLedger(int id)
        {
            var entity = Uow.GeneralLedger.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.GeneralLedger.Update(entity);
            Uow.Commit();
            return entity;
        }

        public GLAccountTypeList DeleteGLAccountTypeList(int id)
        {
            var entity = Uow.GLAccountTypeList.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.GLAccountTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public Invoice DeleteInvoice(int id)
        {
            var entity = Uow.Invoice.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.Invoice.Update(entity);
            Uow.Commit();
            return entity;
        }

        public InvoiceMessage DeleteInvoiceMessage(int id)
        {
            var entity = Uow.InvoiceMessage.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.InvoiceMessage.Update(entity);
            Uow.Commit();
            return entity;
        }

        public InvoiceTypeList DeleteInvoiceTypeList(int id)
        {

            var entity = Uow.InvoiceTypeList.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.InvoiceTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public LedgerAcct DeleteLedgerAcct(int id)
        {
            var entity = Uow.LedgerAcct.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.LedgerAcct.Update(entity);
            Uow.Commit();
            return entity;
        }

        public LedgerSubAcct DeleteLedgerSubAcct(int id)
        {
            var entity = Uow.LedgerSubAcct.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.LedgerSubAcct.Update(entity);
            Uow.Commit();
            return entity;
        }

        public MasterTrx DeleteMasterTrx(int id)
        {
            var entity = Uow.MasterTrx.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.MasterTrx.Update(entity);
            Uow.Commit();
            return entity;
        }

        public MasterTrxDetail DeleteMasterTrxDetail(int id)
        {
            var entity = Uow.MasterTrxDetail.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.MasterTrxDetail.Update(entity);
            Uow.Commit();
            return entity;
        }

        //public MasterTrxDetailDescription DeleteMasterTrxDetailDescription(int id)
        //{
        //    var entity = _uow.MasterTrxDetailDescription.GetById(id);


        //    // Need a Column for soft Delete
        //    entity.IsDelete = true;
        //    _uow.MasterTrxDetailDescription.Update(entity);
        //    _uow.Commit();
        //    return entity;
        //}

        public MasterTrxStatusList DeleteMasterTrxStatusList(int id)
        {
            var entity = Uow.MasterTrxStatusList.GetById(id);


            // Need a Column for soft Delete
            //entity.IsDelete = true;
            Uow.MasterTrxStatusList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public MasterTrxTax DeleteMasterTrxTax(int id)
        {
            var entity = Uow.MasterTrxTax.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.MasterTrxTax.Update(entity);
            Uow.Commit();
            return entity;
        }

        public MasterTrxTypeList DeleteMasterTrxTypeList(int id)
        {
            var entity = Uow.MasterTrxTypeList.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.MasterTrxTypeList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<GeneralLedger> GetGeneralLedger()
        {
            var qry = Uow.GeneralLedger.GetAll();
            return qry;
        }

        public GeneralLedger GetGeneralLedgerById(int id)
        {
            return Uow.GeneralLedger.GetById(id);
        }

        public IQueryable<GLAccountTypeList> GetGLAccountTypeList()
        {
            var qry = Uow.GLAccountTypeList.GetAll();
            return qry;
        }

        public GLAccountTypeList GetGLAccountTypeListById(int id)
        {
            return Uow.GLAccountTypeList.GetById(id);
        }

        public IQueryable<Invoice> GetInvoice()
        {
            var qry = Uow.Invoice.GetAll();
            return qry;
        }

        public Invoice GetInvoiceById(int id)
        {
            return Uow.Invoice.GetById(id);
        }

        public IQueryable<InvoiceMessage> GetInvoiceMessage()
        {
            var qry = Uow.InvoiceMessage.GetAll();
            return qry;
        }

        public InvoiceMessage GetInvoiceMessageById(int id)
        {
            return Uow.InvoiceMessage.GetById(id);
        }

        public IQueryable<InvoiceTypeList> GetInvoiceTypeList()
        {
            var qry = Uow.InvoiceTypeList.GetAll();
            return qry;
        }

        public InvoiceTypeList GetInvoiceTypeListById(int id)
        {
            return Uow.InvoiceTypeList.GetById(id);
        }

        public IQueryable<LedgerAcct> GetLedgerAcct()
        {
            var qry = Uow.LedgerAcct.GetAll();
            return qry;
        }

        public LedgerAcct GetLedgerAcctById(int id)
        {
            return Uow.LedgerAcct.GetById(id);
        }

        public IQueryable<LedgerSubAcct> GetLedgerSubAcct()
        {
            var qry = Uow.LedgerSubAcct.GetAll();
            return qry;
        }

        public LedgerSubAcct GetLedgerSubAcctById(int id)
        {
            return Uow.LedgerSubAcct.GetById(id);
        }

        public IQueryable<MasterTrx> GetMasterTrx()
        {
            var qry = Uow.MasterTrx.GetAll();
            return qry;
        }

        public MasterTrx GetMasterTrxById(int id)
        {
            return Uow.MasterTrx.GetById(id);
        }

        public IQueryable<MasterTrxDetail> GetMasterTrxDetail()
        {
            var qry = Uow.MasterTrxDetail.GetAll();
            return qry;
        }

        public MasterTrxDetail GetMasterTrxDetailById(int id)
        {
            return Uow.MasterTrxDetail.GetById(id);
        }

        //public IQueryable<MasterTrxDetailDescription> GetMasterTrxDetailDescription()
        //{
        //    var qry = _uow.MasterTrxDetailDescription.GetAll();
        //    return qry;
        //}

        //public MasterTrxDetailDescription GetMasterTrxDetailDescriptionById(int id)
        //{
        //    return _uow.MasterTrxDetailDescription.GetById(id);
        //}

        public IQueryable<MasterTrxStatusList> GetMasterTrxStatusList()
        {
            var qry = Uow.MasterTrxStatusList.GetAll();
            return qry;
        }

        public MasterTrxStatusList GetMasterTrxStatusListById(int id)
        {
            return Uow.MasterTrxStatusList.GetById(id);
        }

        public IQueryable<MasterTrxTax> GetMasterTrxTax()
        {
            var qry = Uow.MasterTrxTax.GetAll();
            return qry;
        }

        public MasterTrxTax GetMasterTrxTaxById(int id)
        {
            return Uow.MasterTrxTax.GetById(id);
        }

        public IQueryable<MasterTrxTypeList> GetMasterTrxTypeList()
        {
            var qry = Uow.MasterTrxTypeList.GetAll();
            return qry;
        }

        public MasterTrxTypeList GetMasterTrxTypeListById(int id)
        {
            return Uow.MasterTrxTypeList.GetById(id);
        }

        public GeneralLedger SaveGeneralLedger(GeneralLedger GeneralLedger)
        {
            var ID = GeneralLedger.GeneralLedgerId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.GeneralLedger.Add(GeneralLedger);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.GeneralLedger.Update(GeneralLedger);
                Uow.Commit();
            }

            return GeneralLedger;
        }

        public GLAccountTypeList SaveGLAccountTypeList(GLAccountTypeList GLAccountTypeList)
        {

            var ID = GLAccountTypeList.GLAccountTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.GLAccountTypeList.Add(GLAccountTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.GLAccountTypeList.Update(GLAccountTypeList);
                Uow.Commit();
            }

            return GLAccountTypeList;
        }

        public Invoice SaveInvoice(Invoice Invoice)
        {
            var ID = Invoice.InvoiceId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Invoice.Add(Invoice);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Invoice.Update(Invoice);
                Uow.Commit();
            }

            return Invoice;
        }

        public InvoiceMessage SaveInvoiceMessage(InvoiceMessage InvoiceMessage)
        {
            var ID = InvoiceMessage.InvoiceMessageId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.InvoiceMessage.Add(InvoiceMessage);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.InvoiceMessage.Update(InvoiceMessage);
                Uow.Commit();
            }

            return InvoiceMessage;
        }

        public InvoiceTypeList SaveInvoiceTypeList(InvoiceTypeList InvoiceTypeList)
        {
            var ID = InvoiceTypeList.InvoiceTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.InvoiceTypeList.Add(InvoiceTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.InvoiceTypeList.Update(InvoiceTypeList);
                Uow.Commit();
            }

            return InvoiceTypeList;
        }

        public LedgerAcct SaveLedgerAcct(LedgerAcct LedgerAcct)
        {
            var ID = LedgerAcct.LedgerAcctId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.LedgerAcct.Add(LedgerAcct);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.LedgerAcct.Update(LedgerAcct);
                Uow.Commit();
            }

            return LedgerAcct;
        }

        public LedgerAccount SaveLedgerAccount(LedgerAccount _ledgeraccount)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var ID = _ledgeraccount.LedgerAccountId;
                var isNew = ID == 0;
                //add new entry
                if (isNew)
                {
                    context.LedgerAccounts.Add(_ledgeraccount);
                    context.SaveChanges();
                }
                else //update existing entry
                {
                    LedgerAccount oLedgerAccount = context.LedgerAccounts.SingleOrDefault(o => o.LedgerAccountId == ID);
                    oLedgerAccount.ModifiedBy = LoginUserId;
                    oLedgerAccount.ModifiedDate = DateTime.Now;
                    oLedgerAccount.GLAccountTypeListId = _ledgeraccount.GLAccountTypeListId;
                    oLedgerAccount.IsActive = _ledgeraccount.IsActive;
                    oLedgerAccount.IsDelete = _ledgeraccount.IsDelete;
                    oLedgerAccount.LedgerAccountId = _ledgeraccount.LedgerAccountId;
                    oLedgerAccount.LedgerDescription = _ledgeraccount.LedgerDescription;
                    oLedgerAccount.LedgerName = _ledgeraccount.LedgerName;
                    oLedgerAccount.LedgerNumber = _ledgeraccount.LedgerNumber;
                    oLedgerAccount.ParentLedgerAccountId = _ledgeraccount.ParentLedgerAccountId;
                    context.SaveChanges();
                }

                return _ledgeraccount;
            }
        }


        public LedgerSubAcct SaveLedgerSubAcct(LedgerSubAcct LedgerSubAcct)
        {
            var ID = LedgerSubAcct.LedgerSubAcctId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.LedgerSubAcct.Add(LedgerSubAcct);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.LedgerSubAcct.Update(LedgerSubAcct);
                Uow.Commit();
            }

            return LedgerSubAcct;
        }

        public MasterTrx SaveMasterTrx(MasterTrx MasterTrx)
        {
            var ID = MasterTrx.MasterTrxId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.MasterTrx.Add(MasterTrx);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.MasterTrx.Update(MasterTrx);
                Uow.Commit();
            }

            return MasterTrx;
        }

        public MasterTrxDetail SaveMasterTrxDetail(MasterTrxDetail MasterTrxDetail)
        {
            var ID = MasterTrxDetail.MasterTrxDetailId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.MasterTrxDetail.Add(MasterTrxDetail);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.MasterTrxDetail.Update(MasterTrxDetail);
                Uow.Commit();
            }

            return MasterTrxDetail;
        }

        //public MasterTrxDetailDescription SaveMasterTrxDetailDescription(MasterTrxDetailDescription MasterTrxDetailDescription)
        //{
        //    var ID = MasterTrxDetailDescription.MasterTrxDetailDescriptionId;
        //    var isNew = ID == 0;
        //    //add new entry
        //    if (isNew)
        //    {
        //        _uow.MasterTrxDetailDescription.Add(MasterTrxDetailDescription);
        //        _uow.Commit();
        //    }
        //    else //update existing entry
        //    {

        //        _uow.MasterTrxDetailDescription.Update(MasterTrxDetailDescription);
        //        _uow.Commit();
        //    }

        //    return MasterTrxDetailDescription;
        //}

        public MasterTrxStatusList SaveMasterTrxStatusList(MasterTrxStatusList MasterTrxStatusList)
        {
            var ID = MasterTrxStatusList.MasterTrxStatusListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.MasterTrxStatusList.Add(MasterTrxStatusList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.MasterTrxStatusList.Update(MasterTrxStatusList);
                Uow.Commit();
            }

            return MasterTrxStatusList;
        }

        public MasterTrxTax SaveMasterTrxTax(MasterTrxTax MasterTrxTax)
        {
            var ID = MasterTrxTax.MasterTrxTaxId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.MasterTrxTax.Add(MasterTrxTax);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.MasterTrxTax.Update(MasterTrxTax);
                Uow.Commit();
            }

            return MasterTrxTax;
        }

        public MasterTrxTypeList SaveMasterTrxTypeList(MasterTrxTypeList MasterTrxTypeList)
        {
            var ID = MasterTrxTypeList.MasterTrxTypeListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.MasterTrxTypeList.Add(MasterTrxTypeList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.MasterTrxTypeList.Update(MasterTrxTypeList);
                Uow.Commit();
            }

            return MasterTrxTypeList;
        }

        public IQueryable<Payment> GetPayment()
        {
            var qry = Uow.Payment.GetAll();
            return qry;
        }

        public Payment GetPaymentById(int id)
        {
            return Uow.Payment.GetById(id);
        }

        public Payment SavePayment(Payment Payment)
        {
            var ID = Payment.PaymentId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Payment.Add(Payment);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Payment.Update(Payment);
                Uow.Commit();
            }

            return Payment;
        }

        public Payment DeletePayment(int id)
        {
            var entity = Uow.Payment.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.Payment.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<PaymentMethodList> GetPaymentMethodList()
        {
            var qry = Uow.PaymentMethodList.GetAll();
            return qry;
        }

        public PaymentMethodList GetPaymentMethodListById(int id)
        {
            return Uow.PaymentMethodList.GetById(id);
        }

        public PaymentMethodList SavePaymentMethodList(PaymentMethodList PaymentMethodList)
        {
            var ID = PaymentMethodList.PaymentMethodListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.PaymentMethodList.Add(PaymentMethodList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.PaymentMethodList.Update(PaymentMethodList);
                Uow.Commit();
            }

            return PaymentMethodList;
        }

        public PaymentMethodList DeletePaymentMethodList(int id)
        {
            var entity = Uow.PaymentMethodList.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.PaymentMethodList.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<Credit> GetCredit()
        {
            var qry = Uow.Credit.GetAll();
            return qry;
        }

        public Credit GetCreditById(int id)
        {
            return Uow.Credit.GetById(id);
        }

        public Credit SaveCredit(Credit Credit)
        {
            var ID = Credit.CreditId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.Credit.Add(Credit);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.Credit.Update(Credit);
                Uow.Commit();
            }

            return Credit;
        }

        public Credit DeleteCredit(int id)
        {
            var entity = Uow.Credit.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.Credit.Update(entity);
            Uow.Commit();
            return entity;
        }

        public IQueryable<CreditReasonList> GetCreditReasonList()
        {
            var qry = Uow.CreditReasonList.GetAll();
            return qry;
        }

        public CreditReasonList GetCreditReasonListById(int id)
        {
            return Uow.CreditReasonList.GetById(id);
        }

        public CreditReasonList SaveCreditReasonList(CreditReasonList CreditReasonList)
        {
            var ID = CreditReasonList.CreditReasonListId;
            var isNew = ID == 0;
            //add new entry
            if (isNew)
            {
                Uow.CreditReasonList.Add(CreditReasonList);
                Uow.Commit();
            }
            else //update existing entry
            {

                Uow.CreditReasonList.Update(CreditReasonList);
                Uow.Commit();
            }

            return CreditReasonList;
        }

        public CreditReasonList DeleteCreditReasonList(int id)
        {
            var entity = Uow.CreditReasonList.GetById(id);


            // Need a Column for soft Delete
            entity.IsDelete = true;
            Uow.CreditReasonList.Update(entity);
            Uow.Commit();
            return entity;
        }
    }
}
