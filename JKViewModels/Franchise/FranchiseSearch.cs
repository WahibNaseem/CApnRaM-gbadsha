using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class FranchiseSearch
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string Statename { get; set; }
        public string Postalcode { get; set; }
        public string Phone { get; set; }
        public string Distribution { get; set; }
    }
    public class FranchiseListItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }


    


    public class vFranchiseViewModel
    {
        public int id { get; set; }
        public int regionId { get; set; }
        public string number { get; set; }
        public Nullable<System.DateTime> signdate { get; set; }
        public int term { get; set; }
        public Nullable<System.DateTime> expirationdate { get; set; }
        public int status { get; set; }
        public int chargeback { get; set; }
        public int chargebackbppadmin { get; set; }
        public int generatereport { get; set; }
        public string plantype { get; set; }
        public Nullable<decimal> planamount { get; set; }
        public Nullable<decimal> ibamount { get; set; }
        public Nullable<System.DateTime> equipmentfulfill { get; set; }
        public Nullable<System.DateTime> supplyfulfill { get; set; }
        public Nullable<System.DateTime> trainingfulfill { get; set; }
        public Nullable<System.DateTime> legaloblstart { get; set; }
        public Nullable<System.DateTime> legaloblfulfilled { get; set; }
        public int createdby { get; set; }
        public System.DateTime createdate { get; set; }
        public int modifiedby { get; set; }
        public System.DateTime modifieddate { get; set; }
        public Nullable<int> incorporated { get; set; }
        public Nullable<int> print1099 { get; set; }
        public Nullable<int> bbpadminfee { get; set; }
        public Nullable<decimal> downpayment { get; set; }
        public Nullable<int> noofpayments { get; set; }
        public Nullable<decimal> paymentamount { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalcode { get; set; }
        public string phone { get; set; }
        public string ext { get; set; }
        public string cell { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string name1099 { get; set; }
        public Nullable<int> taxAuthorityId { get; set; }
        public string ein { get; set; }
        public Nullable<int> payeesameas { get; set; }
        public Nullable<System.DateTime> statusdate { get; set; }
        public string statusnotes { get; set; }
        public Nullable<System.DateTime> resumedate { get; set; }
        public Nullable<int> statusreasonid { get; set; }
        public Nullable<int> finderfeetypeid { get; set; }
        public Nullable<decimal> legalobldue { get; set; }
        public Nullable<decimal> planinterest { get; set; }
        public string businesslicense { get; set; }
        public int inhouse { get; set; }
    }
    public class FranchiseTransaction
    {
        public int id { get; set; }
        public int franid { get; set; }
        public int trxtypeid { get; set; }
        public Nullable<System.DateTime> trxdate { get; set; }
        public string trxname { get; set; }
        public Nullable<int> credit { get; set; }
        public Nullable<int> resell { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public Nullable<decimal> unitprice { get; set; }
        public Nullable<decimal> extendedprice { get; set; }
        public Nullable<decimal> subtotal { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<decimal> total { get; set; }
        public string description { get; set; }
        public int noofpayments { get; set; }
        public int paymentsbilled { get; set; }
        public System.DateTime startdate { get; set; }
        public int status { get; set; }
        public Nullable<System.DateTime> statusdate { get; set; }
        public Nullable<System.DateTime> resumedate { get; set; }
        public int createdby { get; set; }
        public System.DateTime createddate { get; set; }
        public int modifiedby { get; set; }
        public System.DateTime modifieddate { get; set; }
        public Nullable<decimal> grosstotal { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string franname { get; set; }
        public string statusname
        {
            get; set;
        }
    }
}
