using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels.Franchise
{
    public class ManageFranchiseViewModel
    {
        public int id { get; set; }
        public string txteinssn { get; set; }
        public bool chkincorporated { get; set; }
        public bool chkprint1099 { get; set; }
        public string txtname { get; set; }
        public string txtaddress { get; set; }
        public string txtaddress2 { get; set; }
        public string txtcity { get; set; }
        public string lststate { get; set; }
        public string txtpostalcode { get; set; }
        public string lsttaxauthority { get; set; }
        public string txtphone { get; set; }
        public string txtext { get; set; }
        public string txtcell { get; set; }
        public string txtfax { get; set; }
        public string txtemail { get; set; }
        public string txtName1099 { get; set; }
        public bool chkpayeesameasmain { get; set; }
        public string txtpayname { get; set; }
        public string txtpayaddress { get; set; }
        public string txtpayaddress2 { get; set; }
        public string txtpaycity { get; set; }
        public string lstpaystate { get; set; }
        public string txtpaypostalcode { get; set; }
        public string txtdatesign { get; set; }
        public string lstfterm { get; set; }
        public string txtexpirationdate { get; set; }
        public string lstplantype { get; set; }
        public string txtplanamount { get; set; }
        public string txtibamount { get; set; }
        public string txtdownpayment { get; set; }
        public string txtinterest { get; set; }
        public string txtpaymentamount { get; set; }
        public string txtnoofpayments { get; set; }
        public string txtpaymentno { get; set; }
        public string txtpaymentstartdate { get; set; }
        public string txtpaymenttriggeramt { get; set; }
        public string txtlegaloblstart { get; set; }
        public string txtlegaloblfulfilled { get; set; }
        public string txtlegalobldue { get; set; }
        public bool chkbbpadminfee { get; set; }
        public bool chkchargeback { get; set; }
        public bool chkgeneratereport { get; set; }
        public bool chkaccountrebate { get; set; }
    }
}
