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
    
    public partial class Invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoice()
        {
            this.Adjustments = new HashSet<Adjustment>();
            this.BillingPays = new HashSet<BillingPay>();
            this.CCTransactions = new HashSet<CCTransaction>();
            this.Credits = new HashSet<Credit>();
            this.FranchiseeReportDetails = new HashSet<FranchiseeReportDetail>();
            this.FranchiseeReportFinalizedDetails = new HashSet<FranchiseeReportFinalizedDetail>();
            this.FranchiseeReportFinalizedDetails1 = new HashSet<FranchiseeReportFinalizedDetail>();
            this.InvoiceMessages = new HashSet<InvoiceMessage>();
            this.MasterTrxDetails = new HashSet<MasterTrxDetail>();
            this.Overflows = new HashSet<Overflow>();
            this.Payments = new HashSet<Payment>();
            this.TurnArounds = new HashSet<TurnAround>();
        }
    
        public int InvoiceId { get; set; }
        public Nullable<int> MasterTrxId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> InvoiceTypeListId { get; set; }
        public Nullable<int> AddressId_SoldTo { get; set; }
        public Nullable<int> AddressId_For { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> ContractId { get; set; }
        public Nullable<int> TransactionStatusListId { get; set; }
        public string InvoiceDescription { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string TransactionNumber { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> InvoiceMessageDetail { get; set; }
        public string InvoiceReferenceLetter { get; set; }
        public Nullable<int> sys_cust { get; set; }
        public Nullable<int> PeriodId { get; set; }
        public Nullable<int> MasterTrxTypeListId { get; set; }
        public Nullable<bool> ExtraWork { get; set; }
        public Nullable<int> PayGroup { get; set; }
        public Nullable<bool> ConsolidatedInvoice { get; set; }
        public Nullable<int> ConsolidatedInvoiceId { get; set; }
    
        public virtual MasterTrx MasterTrx { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Adjustment> Adjustments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillingPay> BillingPays { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCTransaction> CCTransactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Credit> Credits { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FranchiseeReportDetail> FranchiseeReportDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FranchiseeReportFinalizedDetail> FranchiseeReportFinalizedDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FranchiseeReportFinalizedDetail> FranchiseeReportFinalizedDetails1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceMessage> InvoiceMessages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MasterTrxDetail> MasterTrxDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Overflow> Overflows { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TurnAround> TurnArounds { get; set; }
    }
}