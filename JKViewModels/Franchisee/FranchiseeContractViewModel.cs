using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeContractViewModel
    {
        public int FranchiseeContractId { get; set; }
        public Nullable<int> FranchiseeId { get; set; }
        public Nullable<int> FranchiseeContractTypeListId { get; set; }
        public Nullable<int> Term { get; set; }
        public Nullable<System.DateTime> DateSign { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> IBAmount { get; set; }
        public Nullable<decimal> DownPaymentAmount { get; set; }
        public Nullable<decimal> MonthlyPaymentAmount { get; set; }
        public Nullable<decimal> Interest { get; set; }
        public Nullable<int> TotalPayments { get; set; }
        public Nullable<int> CurrentPayment { get; set; }
        public Nullable<decimal> TriggerAmount { get; set; }
        public Nullable<decimal> LegalObligationAmount { get; set; }
        public Nullable<decimal> LegalObligaitonDueAmount { get; set; }
        public Nullable<System.DateTime> LegalObligationStartDate { get; set; }
        public Nullable<System.DateTime> LegalObligationEndDate { get; set; }
        public Nullable<int> DaysToFullfill { get; set; }
        public Nullable<System.DateTime> LastRenewedContractDate { get; set; }
    }
}
