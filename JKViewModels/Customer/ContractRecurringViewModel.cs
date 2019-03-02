using JKViewModels.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Customer
{
    public class ContractRecurringViewModel
    {
        public int Id { get; set; }
        public int Customerid { get; set; }
        public int Servicelocationid { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }  //MG -- 5-12-2015 Defaulting to 1
        public double Itemamt { get; set; }
        public double Trxamt { get; set; }
        public int TrxTypeId { get; set; }
        public string Invoiceflag { get; set; }
        public string Invoicemsg { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public int Trxsubjecttofees { get; set; }
        public int Trxtax { get; set; }
        public int Trxbppadmin { get; set; }
        public int Createdby { get; set; }
        public DateTime Createdate { get; set; }
        public int Modifiedby { get; set; }
        public DateTime Modifieddate { get; set; }
        public int Detailserviceid { get; set; }
        public String RtrxFrequencyTypeId { get; set; }
        public int ContractId { get; set; }
        public string TypeId { get; set; }
        public string TypeDescription { get; set; }
        public int SeparateInvoice { get; set; }
        public int AccountRebate { get; set; }

        public double Amount { get; set; }
        public int SubjectToFees { get; set; }
        public int Taxable { get; set; }
        public int SubjectToFeesChk { get; set; }
        public int CPIIncrease { get; set; }
        public int BPPAdminCalc { get; set; }
        public string FrequencyTypeId { get; set; }

        public List<FeeViewModel> _fees { get; set; }
    }
}
