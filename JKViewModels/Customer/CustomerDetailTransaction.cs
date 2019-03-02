using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
            public class CustomerDetailTransactionViewModel
            {
                public int MasterTrxId { get; set; }
                public string Number { get; set; }
                public Nullable<decimal> Amount { get; set; }
                public Nullable<int> ClassId { get; set; }
                public Nullable<int> TypeListId { get; set; }
                public Nullable<int> TransactionTypeId { get; set; }
                public string TransactionTypeName { get; set; }
                public Nullable<System.DateTime> TransactionDate { get; set; }
                public string Description { get; set; }
                public Nullable<int> RefId { get; set; }
                public Nullable<int> AmountTypeListId { get; set; }
                public string TransactionNumber { get; set; }
            }

            public class CustomerDetailTransactionWithStatusViewModel
            {
                public int MasterTrxId { get; set; }
                public string Number { get; set; }
                public Nullable<decimal> Amount { get; set; }
                public Nullable<int> ClassId { get; set; }
                public Nullable<int> TypeListId { get; set; }
                public Nullable<int> TransactionTypeId { get; set; }
                public string TransactionTypeName { get; set; }
                public Nullable<System.DateTime> TransactionDate { get; set; }
                public string Description { get; set; }
                public Nullable<int> RefId { get; set; }
                public Nullable<int> AmountTypeListId { get; set; }
                public string TransactionNumber { get; set; }
                public string StatusName { get; set; }

                public Nullable<int> Quantity { get; set; }
                public Nullable<decimal> UnitPrice { get; set; }
                public Nullable<decimal> CPIPercentage { get; set; }
                public Nullable<decimal> ExtendedPrice { get; set; }
                public Nullable<decimal> DistributionAmount { get; set; }
                public Nullable<decimal> TotalTax { get; set; }
                public Nullable<decimal> TotalFee { get; set; }
                public Nullable<decimal> Total { get; set; }
                public Nullable<decimal> DebitAmount { get; set; }
                public Nullable<decimal> CreditAmount { get; set; }
            }
            public class CustomerTransactionJson
            {
                public decimal StartingBalance { get; set; }
                public IEnumerable<CustomerDetailTransactionWithStatusViewModel> CustomerViewModel { get; set; }
            }

            public class GetPercentPaidByDateReport
            {
                public DateTime TrxDate { get; set; }
                public int InvoiceCount { get; set; }
                public decimal PaidAmount { get; set; }
                public decimal InvoiceAmount { get; set; }
                public int RegionID { get; set; }
                public decimal DailyPercent { get; set; }
                public decimal PaidUptoDate { get; set; }
                public decimal PercentUptoDate { get; set; }

    }

}
