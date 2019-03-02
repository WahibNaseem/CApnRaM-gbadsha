using JKApi.Data.DAL;
using JKViewModels.Administration.Company;
using System.Collections.Generic;
using System.Linq;

namespace JKApi.Service.ServiceContract.CustomerInvoice
{
    public interface ICustomerInvoiceService
    {
        #region LedgerAcct Calls
        IQueryable<LedgerAcct> GetLedgerAcct();

        LedgerAcct GetLedgerAcctById(int id);

        LedgerAcct SaveLedgerAcct(LedgerAcct LedgerAcct);

        LedgerAcct DeleteLedgerAcct(int id);

        #endregion

        #region LedgerSubAcct Calls

        IQueryable<LedgerSubAcct> GetLedgerSubAcct();

        LedgerSubAcct GetLedgerSubAcctById(int id);

        LedgerSubAcct SaveLedgerSubAcct(LedgerSubAcct LedgerSubAcct);
        LedgerAccount SaveLedgerAccount(LedgerAccount _ledgeraccount);
        LedgerSubAcct DeleteLedgerSubAcct(int id);

        #endregion
        List<CommonLadgerAccountViewModel> GetLedgerAccountList();
        List<CommonLadgerAccountViewModel> GetChartofAccountList();
        
        CommonLadgerAccountViewModel GetLedgerAccountDetailforEdit(int accid, bool issubaccount);
        List<GeneralLadgerAccountListViewModel> GetLedgerMasterAccountList();

        #region GeneralLedger Calls

        IQueryable<GeneralLedger> GetGeneralLedger();

        GeneralLedger GetGeneralLedgerById(int id);

        GeneralLedger SaveGeneralLedger(GeneralLedger GeneralLedger);

        GeneralLedger DeleteGeneralLedger(int id);



        #endregion

        #region GLAccountTypeList Calls

        IQueryable<GLAccountTypeList> GetGLAccountTypeList();

        GLAccountTypeList GetGLAccountTypeListById(int id);

        GLAccountTypeList SaveGLAccountTypeList(GLAccountTypeList GLAccountTypeList);

        GLAccountTypeList DeleteGLAccountTypeList(int id);



        #endregion

        #region MasterTrx Calls

        IQueryable<MasterTrx> GetMasterTrx();

        MasterTrx GetMasterTrxById(int id);

        MasterTrx SaveMasterTrx(MasterTrx MasterTrx);

        MasterTrx DeleteMasterTrx(int id);

        #endregion

        #region MasterTrxTypeList Calls

        IQueryable<MasterTrxTypeList> GetMasterTrxTypeList();

        MasterTrxTypeList GetMasterTrxTypeListById(int id);

        MasterTrxTypeList SaveMasterTrxTypeList(MasterTrxTypeList MasterTrxTypeList);

        MasterTrxTypeList DeleteMasterTrxTypeList(int id);



        #endregion

        #region MasterTrxStatusList Calls

        IQueryable<MasterTrxStatusList> GetMasterTrxStatusList();

        MasterTrxStatusList GetMasterTrxStatusListById(int id);

        MasterTrxStatusList SaveMasterTrxStatusList(MasterTrxStatusList MasterTrxStatusList);

        MasterTrxStatusList DeleteMasterTrxStatusList(int id);



        #endregion

        #region MasterTrxDetail Calls

        IQueryable<MasterTrxDetail> GetMasterTrxDetail();

        MasterTrxDetail GetMasterTrxDetailById(int id);

        MasterTrxDetail SaveMasterTrxDetail(MasterTrxDetail MasterTrxDetail);

        MasterTrxDetail DeleteMasterTrxDetail(int id);



        #endregion
        //#region MasterTrxDetailDescription Calls

        //IQueryable<MasterTrxDetailDescription> GetMasterTrxDetailDescription();

        //MasterTrxDetailDescription GetMasterTrxDetailDescriptionById(int id);

        //MasterTrxDetailDescription SaveMasterTrxDetailDescription(MasterTrxDetailDescription MasterTrxDetailDescription);

        //MasterTrxDetailDescription DeleteMasterTrxDetailDescription(int id);



        //#endregion
        #region MasterTrxTax Calls

        IQueryable<MasterTrxTax> GetMasterTrxTax();

        MasterTrxTax GetMasterTrxTaxById(int id);

        MasterTrxTax SaveMasterTrxTax(MasterTrxTax MasterTrxTax);

        MasterTrxTax DeleteMasterTrxTax(int id);



        #endregion

        #region Invoice Calls

        IQueryable<Invoice> GetInvoice();

        Invoice GetInvoiceById(int id);

        Invoice SaveInvoice(Invoice Invoice);

        Invoice DeleteInvoice(int id);



        #endregion

        #region InvoiceTypeList Calls

        IQueryable<InvoiceTypeList> GetInvoiceTypeList();

        InvoiceTypeList GetInvoiceTypeListById(int id);

        InvoiceTypeList SaveInvoiceTypeList(InvoiceTypeList InvoiceTypeList);

        InvoiceTypeList DeleteInvoiceTypeList(int id);



        #endregion

        #region InvoiceMessage Calls

        IQueryable<InvoiceMessage> GetInvoiceMessage();

        InvoiceMessage GetInvoiceMessageById(int id);

        InvoiceMessage SaveInvoiceMessage(InvoiceMessage InvoiceMessage);

        InvoiceMessage DeleteInvoiceMessage(int id);



        #endregion

        #region Payment Calls

        IQueryable<Payment> GetPayment();

        Payment GetPaymentById(int id);

        Payment SavePayment(Payment Payment);

        Payment DeletePayment(int id);

        #endregion

        #region PaymentMethodList Calls

        IQueryable<PaymentMethodList> GetPaymentMethodList();

        PaymentMethodList GetPaymentMethodListById(int id);

        PaymentMethodList SavePaymentMethodList(PaymentMethodList PaymentMethodList);

        PaymentMethodList DeletePaymentMethodList(int id);

        #endregion

        #region Credit Calls

        IQueryable<Credit> GetCredit();

        Credit GetCreditById(int id);

        Credit SaveCredit(Credit Credit);

        Credit DeleteCredit(int id);

        #endregion

        #region CreditReasonList Calls

        IQueryable<CreditReasonList> GetCreditReasonList();

        CreditReasonList GetCreditReasonListById(int id);

        CreditReasonList SaveCreditReasonList(CreditReasonList CreditReasonList);

        CreditReasonList DeleteCreditReasonList(int id);

        #endregion



        

    }
}
