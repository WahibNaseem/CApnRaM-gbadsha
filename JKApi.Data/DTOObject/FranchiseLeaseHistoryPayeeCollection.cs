using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data.DTOObject
{
   public class FranchiseLeaseHistoryPayeeCollection
    {
        public List<portal_spget_F_LeaseHistory_all_Result> portal_spget_F_LeaseHistory_all_Result { get; set; }
        public List<portal_spget_F_LeaseHistory_Specific_Result> portal_spget_F_LeaseHistory_Specific_Result { get; set; }

        public string PaymentAmount { get; set; }

        public string PaymentTax { get; set; } 
    }
}
