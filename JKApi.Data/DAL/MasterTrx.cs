//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JKApi.Data.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MasterTrx
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MasterTrx()
        {
            this.Adjustments = new HashSet<Adjustment>();
            this.APBills = new HashSet<APBill>();
            this.BillingPays = new HashSet<BillingPay>();
            this.Chargebacks = new HashSet<Chargeback>();
            this.CheckBooks = new HashSet<CheckBook>();
            this.Credits = new HashSet<Credit>();
            this.CreditFranchisees = new HashSet<CreditFranchisee>();
            this.Deposits = new HashSet<Deposit>();
            this.FindersFeeBills = new HashSet<FindersFeeBill>();
            this.FranchiseeManualTransactions = new HashSet<FranchiseeManualTransaction>();
            this.GeneralLedgers = new HashSet<GeneralLedger>();
            this.GeneralLedgerTrxes = new HashSet<GeneralLedgerTrx>();
            this.Invoices = new HashSet<Invoice>();
            this.LeaseBills = new HashSet<LeaseBill>();
            this.MasterTrxDetails = new HashSet<MasterTrxDetail>();
            this.NotificationMessageForDashboards = new HashSet<NotificationMessageForDashboard>();
            this.Payments = new HashSet<Payment>();
            this.PaymentBillingFranchisees = new HashSet<PaymentBillingFranchisee>();
            this.TurnArounds = new HashSet<TurnAround>();
            this.VendorInvoices = new HashSet<VendorInvoice>();
        }
    
        public int MasterTrxId { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> BillMonth { get; set; }
        public Nullable<int> BillYear { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> BatchId { get; set; }
        public Nullable<int> AccountTypeListId { get; set; }
        public Nullable<System.DateTime> TrxDate { get; set; }
        public Nullable<int> MasterTrxImpId { get; set; }
        public Nullable<int> HeaderId { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Adjustment> Adjustments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APBill> APBills { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillingPay> BillingPays { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chargeback> Chargebacks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CheckBook> CheckBooks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Credit> Credits { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditFranchisee> CreditFranchisees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Deposit> Deposits { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FindersFeeBill> FindersFeeBills { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FranchiseeManualTransaction> FranchiseeManualTransactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GeneralLedger> GeneralLedgers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GeneralLedgerTrx> GeneralLedgerTrxes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LeaseBill> LeaseBills { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasterTrxDetail> MasterTrxDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationMessageForDashboard> NotificationMessageForDashboards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentBillingFranchisee> PaymentBillingFranchisees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TurnAround> TurnArounds { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VendorInvoice> VendorInvoices { get; set; }
    }
}
