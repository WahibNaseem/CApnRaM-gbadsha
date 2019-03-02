using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountsPayable
{
    public class FranchiseBillingDetailViewModel
    {
        public string BillingNo { get; set; }

        public DateTime? BillingDate { get; set; }

        public int FranchiseeId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerNo { get; set; }

        public string FranchiseeNo { get; set; }

        public string FranchiseeName { get; set; }

        public string CustomerName { get; set; }

        public FranchiseBillingDetailAddress FromAddress { get; set; }

        public FranchiseBillingDetailAddress ToAddress { get; set; }

        public FranchiseBillingDetailTrx Transaction { get; set; }

        public List<FranchiseBillingDetailTrxDetail> TransactionDetails { get; set; }

        public List<FranchiseBillingDetailFeeDetail> FeeDetails { get; set; }
    }

    public class FranchiseBillingDetailAddress
    {
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Phone { get; set; }
    }

    public class FranchiseBillingDetailTrx
    {
        public string BillingMonthYear { get; set; }    

        public string ServiceType { get; set; }

        public string Description { get; set; }

        public string InvoiceNo { get; set; }   

        public int InvoiceID { get; set; }
    }

    public class FranchiseBillingDetailTrxDetail
    {
        public int Id { get; set; }

        public int? ServiceTypeListid { get; set; }

        public int? LineNo { get; set; }

        public string ServiceDetail { get; set; }

        public int? Quantity { get; set; }

        public decimal? ContractPrice { get; set; }

        public decimal? Fee { get; set; }

        public decimal? PayFranchisee { get; set; }

        public bool? FeeDetail { get; set; }
    }

    public class FranchiseBillingDetailFeeDetail
    {
        public string FeeType { get; set; }

        public decimal? FeeAmount { get; set; }   

        public decimal? feePercentage { get; set; }
    }
}
