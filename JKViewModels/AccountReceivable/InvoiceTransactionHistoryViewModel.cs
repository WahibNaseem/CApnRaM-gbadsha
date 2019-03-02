using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.AccountReceivable
{
    public class InvoiceTransactionHistoryViewModel
    {
        public int MasterTrxId { get; set; } //(int, not null)
        public int MasterTrxTypeListId { get; set; } //(int, null)
        public string MasterTrxTypeListName { get; set; } //(varchar(75), null)
        public DateTime TransactionDate { get; set; } //(datetime, null)
        public string TransactionNumber { get; set; } //(nvarchar(50), null)
        public string Description { get; set; } //(nvarchar(max), null)
        public int RefId { get; set; } //(int, null)
        public decimal Amount { get; set; } //(decimal(38,2), null)
        public int AmountTypeListId { get; set; } //(int, null)
        public int ClassId { get; set; } //(int, null)
        public int TypeListId { get; set; } //(int, null)
        //public string TransactionNumber { get; set; } //(nvarchar(max), null)
        public int InvoiceId { get; set; } //(int, null)
        public decimal ExtendedPrice { get; set; } //(decimal(38,2), null)
        public decimal TotalTax { get; set; } //(decimal(38,2), null)

        //public string Number { get; set; } //(nvarchar(50), null)
        //public string TransactionTypeListId { get; set; }
        //public string TransactionTypeName { get; set; }


        public int? CustomerId { get; set; } //(nvarchar(50), null)
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
    }
}
