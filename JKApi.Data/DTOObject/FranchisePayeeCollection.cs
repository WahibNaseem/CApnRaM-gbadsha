using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Data.DTOObject
{
   public class FranchisePayeeCollection
    {
        public Franchisee tbl_F_Information { get; set; }
        public FranchiseeFee tbl_F_Payee { get; set; }
    }
}
