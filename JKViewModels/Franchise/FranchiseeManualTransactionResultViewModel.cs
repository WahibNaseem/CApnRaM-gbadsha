using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class FranchiseeManualTransactionResultViewModel
    {
        public int FranchiseeManualTransactionTempId { get; set; } //(int, not null)
        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string Name { get; set; } //(varchar(150), null)
        public int ServiceTypeListId { get; set; } //(int, not null)
        public string FranchiseeTransactionTypeListName { get; set; } //(varchar(max), null)
        public string SR { get; set; } //(varchar(max), null)
        public string Description { get; set; } //(varchar(max), null)
        public decimal Subtotal { get; set; } //(decimal(12,2), null)
        public decimal Tax { get; set; } //(decimal(12,2), null)
        public decimal Total { get; set; } //(decimal(12,2), null)
        public int MasterTrxTypeListId { get; set; } //(int, not null)
        public string MasterTrxTypeListName { get; set; } //(varchar(75), null)
        public DateTime TransactionDate { get; set; } //(date, null)
        public int RegionId { get; set; } //(int, null)
        public int StatusListId { get; set; } //(int, null)
        public string Status { get; set; } //(int, null)
        public int StatusId { get; set; } //(int, null)
        public string CreatedBy { get; set; }
    }
}
