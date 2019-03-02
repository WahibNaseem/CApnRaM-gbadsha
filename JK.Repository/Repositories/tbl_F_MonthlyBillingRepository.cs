using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JK.Repository.Repositories
{
    public class tbl_F_MonthlyBillingRepository : BaseRepository<tbl_F_MonthlyBilling> , Itbl_F_MonthlyBillingRepository
    {
        public tbl_F_MonthlyBillingRepository(DbContext context) : base(context){}
    }
}
