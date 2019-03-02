using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JKViewModels.AccountReceivable
{
    public class ARLogListFinalViewModel
    {
        public int RegionId { get; set; } //(int, null)
        public decimal TotalAmount { get; set; } //(decimal(38,2), not null)       
        public decimal TotalDeposit { get; set; } //(decimal(16,2), null)  
        public decimal TotalBalance { get; set; } //(decimal(38,2), not null)
        public List<ARLogListViewModel> ARLogs { get; set; }
        public List<ARLogListViewModel> AROtherLogs { get; set; }
    }
    public class ARLogListViewModel
    {
        public int MasterTrxDetailId { get; set; } //(int, null)
        public int RegionId { get; set; } //(int, null)
        public string RegionName { get; set; } //(varchar(8), null)
        public int CustomerId { get; set; } //(int, null)
        public string CustomerNo { get; set; } //(varchar(25), null)
        public string CustomerName { get; set; } //(varchar(125), null)
        public int InvoiceId { get; set; } //(int, null)
        public string InvoiceNo { get; set; } //(char(25), null)
        public decimal InvoiceAmount { get; set; } //(decimal(38,2), not null)
        public decimal InvoiceOBalance { get; set; } //(decimal(38,2), not null)
        public decimal PaymentAmount { get; set; } //(decimal(16,2), null)
        public decimal InvoiceBalance { get; set; } //(decimal(38,2), not null)
        public string CheckNumber { get; set; } //(varchar(50), null)
        public decimal CheckAmount { get; set; } //(decimal(38,2), not null)    
        public int PaymentMethodListId { get; set; }
        public int PaymentId { get; set; }
        public string Description { get; set; }
    }
}