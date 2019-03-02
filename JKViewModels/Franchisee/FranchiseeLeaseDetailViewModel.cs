using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchisee
{
    public class FranchiseeLeaseDetailViewModel
    {
        public int LeaseId { get; set; } //(int, not null)
        public int FranchiseeId { get; set; } //(int, not null)
        public string FranchiseeNo { get; set; } //(varchar(10), null)
        public string Name { get; set; } //(varchar(150), null)
        public string Address { get; set; } //(varchar(252), null)
        public string City { get; set; } //(varchar(50), not null)
        public string StateName { get; set; } //(varchar(20), not null)
        public string PostalCode { get; set; } //(varchar(20), not null)
        public string LeaseNumber { get; set; } //(varchar(100), null)
        public string LeaseDescription { get; set; } //(varchar(max), null)
        public int StatusListId { get; set; } //(int, not null)
        public int TypeListId { get; set; } //(int, null)
        public string Make { get; set; } //(varchar(100), null)
        public string Model { get; set; } //(varchar(100), null)
        public string SerialNo { get; set; } //(varchar(50), null)
        public DateTime SignDate { get; set; } //(datetime, null)
        public DateTime StartDate { get; set; } //(datetime, null)
        public bool ChargeTaxUpFront { get; set; } //(bit, null)
        public bool IncludeFirstPaymentInDownPayment { get; set; }
        public decimal LeaseAmount { get; set; } //(decimal(12,2), null)
        public decimal LeaseTotalTax { get; set; } //(decimal(12,2), null)
        public decimal LeaseTotal { get; set; } //(decimal(12,2), null)
        public decimal MonthlyPaymentAmount { get; set; } //(decimal(12,2), null)
        public decimal MonthlyTaxRate { get; set; } //(decimal(12,2), null)
        public decimal MonthlyPaymentTotal { get; set; } //(decimal(12,2), null)
        public int InstallmentDownPaymentNum { get; set; } //(int, null)
        public int InstallmentMonthlyPaymentNum { get; set; } //(int, null)
        public int InstallmentLastPaymentNum { get; set; } //(int, null)
        public int InstallmentTotalPaymentNum { get; set; } //(int, null)
        public bool DownPaymentPaid { get; set; } //(bit, null)
        public int NumOfPaymentsPaid { get; set; } //(int, null)
        public decimal TotalAmountPaid { get; set; } //(decimal(12,2), null)
        public int RegionId { get; set; } //(int, null)
        public int CreatedBy { get; set; } //(int, null)
        public DateTime? CreatedDate { get; set; } //(datetime, null)
        public int ModifiedBy { get; set; } //(int, null)
        public DateTime? ModifiedDate { get; set; } //(datetime, null)
        public string StatusListName { get; set; } //(varchar(250), null)
        public decimal Balance { get; set; }
    }

}
