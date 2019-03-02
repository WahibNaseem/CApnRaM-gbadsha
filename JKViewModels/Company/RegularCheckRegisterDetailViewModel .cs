using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Company
{
    public class RegularCheckRegisterDetailViewModel
    {
        public string CheckNum { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public string InvoiceNum { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public string Reference { get; set; } 
        public double Additon { get; set; }
        public double Deduction { get; set; }
        public Nullable<System.DateTime> VoidDate { get; set; }
        public string Name { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string AcctId { get; set; }
        public string Desc { get; set; }
        public double DebitAmt { get; set; }
        public double CreditAmt { get; set; }
    }

}


