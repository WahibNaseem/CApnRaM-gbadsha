using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class TransactionFranchiseSearch
    {
        public int id { get; set; }
        public int franid { get; set; }
        public string trxdate { get; set; }
        public string unitprice { get; set; }
        public string tax { get; set; }
        public string trxtotal { get; set; }
        public string description { get; set; }
        public string billmonth { get; set; }
        public string billyear { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string franname { get; set; }
        public string customername { get; set; }
        public string customerno { get; set; }
        public int customerid { get; set; }
        public string invoiceno { get; set; }


    }


    public class TransactionFranchisePending
    {
        public int id { get; set; }
        public Nullable<int> franid { get; set; }
        public Nullable<int> customertrxid { get; set; }
        public Nullable<int> frecurringid { get; set; }
        public Nullable<int> trxtypeid { get; set; }
        public string trxname { get; set; }
        public Nullable<System.DateTime> trxdate { get; set; }
        public Nullable<int> credit { get; set; }
        public Nullable<int> resell { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public Nullable<decimal> unitprice { get; set; }
        public Nullable<decimal> extendedprice { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<decimal> trxtotal { get; set; }
        public Nullable<decimal> feeamount { get; set; }
        public string description { get; set; }
        public Nullable<int> paymentno { get; set; }
        public Nullable<int> billmonth { get; set; }
        public Nullable<int> billyear { get; set; }
        public Nullable<decimal> balance { get; set; }
        public Nullable<decimal> balancetax { get; set; }
        public Nullable<decimal> totaldue { get; set; }
        public Nullable<int> appliedto { get; set; }
        public Nullable<int> chargedback { get; set; }
        public Nullable<int> createdby { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string franname { get; set; }
        public string customername { get; set; }
        public Nullable<int> customerid { get; set; }
        public int franchiseid { get; set; }
        public string resultmsg { get; set; }
        public int status { get; set; }
        public int temporary { get; set; }
        public string importid { get; set; }
        public string vendorcode { get; set; }
        public string vendorinvoiceno { get; set; }
        public Nullable<int> trxtypeid1 { get; set; }
        public Nullable<int> credit1 { get; set; }
        public Nullable<long> rowver { get; set; }
    }


}
