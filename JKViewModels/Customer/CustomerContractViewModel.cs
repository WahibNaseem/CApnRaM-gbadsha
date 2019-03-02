using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class CustomerContractViewModel
    {
        public CustomerContractViewModel()
        {
            this.lstContractDetail = new List<CustomerContractDetailViewModel>();
        }
        public int CustomerId { get; set; } //(int, not null)
        public string CustomerNo { get; set; } //(varchar(25), null)
        public string CustomerName { get; set; } //(varchar(125), null)
        public string Address1 { get; set; } //(varchar(125), null)
        public string Address2 { get; set; } //(varchar(125), null)
        public string City { get; set; } //(varchar(50), null)
        public int StateListId { get; set; } //(int, null)
        public string StateName { get; set; } //(varchar(50), null)
        public string PostalCode { get; set; } //(varchar(20), null)
        public int ContactTypeListId { get; set; } //(int, null)
        public int ContractId { get; set; } //(int, not null)
        public int AccountTypeListId { get; set; } //(int, null)
        public string AccountTypeListName { get; set; } //(varchar(75), null)
        public int ContractTypeListId { get; set; } //(int, null)
        public string ContractTypeListName { get; set; } //(varchar(125), null)
        public int AgreementTypeListId { get; set; } //(int, null)
        public string AgreementTypeListName { get; set; } //(varchar(75), null)
        public int SoldById { get; set; } //(int, null)
        public string SoldByName { get; set; } //(varchar(201), null)
        public string PurchaseOrderNo { get; set; } //(varchar(20), null)
        public DateTime SignDate { get; set; } //(date, null)
        public DateTime StartDate { get; set; } //(date, null)
        public int ContractTermMonth { get; set; } //(int, null)
        public DateTime ExpirationDate { get; set; } //(date, null)
        public decimal Amount { get; set; } //(decimal(18,2), null)
        public string ContractDescription { get; set; } //(varchar(max), null)
        public bool IsActive { get; set; } //(bit, null)
        public int? StatusId { get; set; } //(int, null)
        public int? StatusListId { get; set; } //(int, null)
        public int? CreatedBy { get; set; } //(int, null)
        public DateTime? CreatedDate { get; set; } //(datetime, null)
        public bool Qualified { get; set; } //(bit, null)
        public decimal? InitialCleanAmount { get; set; } //(decimal(18,2), null)
        public int? SoldById_Secondary { get; set; } //(int, null)
        public int? CommissionTransactionStatusListId { get; set; } //(int, null)
        public int RegionId { get; set; }
        public virtual List<CustomerContractDetailViewModel> lstContractDetail { get; set; }
    }

    public class CustomerContractDetailViewModel
    {
        public int ContractDetailId { get; set; } //(int, not null)
        public int ContractId { get; set; } //(int, null)
        public int ServiceTypeListId { get; set; } //(int, null)
        public string ServiceTypeListName { get; set; } //(varchar(50), not null)
        public int BillingFrequencyListId { get; set; } //(int, null)
        public string BillingFrequencyListName { get; set; } //(varchar(75), null)
        public int LineNumber { get; set; } //(int, null)
        public string SquareFootage { get; set; } //(varchar(12), null)
        public int CleanTimes { get; set; } //(int, null)
        public bool Mon { get; set; } //(bit, null)
        public bool Tues { get; set; } //(bit, null)
        public bool Wed { get; set; } //(bit, null)
        public bool Thur { get; set; } //(bit, null)
        public bool Fri { get; set; } //(bit, null)
        public bool Sat { get; set; } //(bit, null)
        public bool Sun { get; set; } //(bit, null)
        public int CleanFrequencyListId { get; set; } //(int, null)
        public string CleanFrequencyListName { get; set; } //(varchar(125), null)
        public decimal Amount { get; set; } //(decimal(18,2), null)
        public bool BPPAdmin { get; set; } //(bit, null)
        public bool CPIIncrease { get; set; } //(bit, null)
        public bool AccountRebate { get; set; } //(bit, null)
        public int CreatedBy { get; set; } //(int, null)
        public DateTime CreatedDate { get; set; } //(datetime, null)
        public string Description { get; set; } //(varchar(max), null)
        public DateTime StartTime { get; set; } //(datetime, null)
        public DateTime EndTime { get; set; } //(datetime, null)
        public bool IsActive { get; set; } //(bit, null)
        public int separateInvoice { get; set; } //(int, null)
        public int CPIMonthApplied { get; set; } //(int, null)
        public decimal CPIPercent { get; set; } //(decimal(18,4), null)
        public bool TaxExcempt { get; set; } //(bit, null)
        public bool WeekEnd { get; set; } //(bit, null)

        public bool SeparateInvoice { get
            {
                return (separateInvoice == 1 ? true : false);
            }
            set
            {                
                separateInvoice = value?1:0;
            }
        } //(int, null)
    }

}
