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
    public class tbl_F_MonthlyCustomerAccountTotalsRepository : BaseRepository<tbl_F_MonthlyCustomerAccountTotals> , Itbl_F_MonthlyCustomerAccountTotalsRepository
    {
        public tbl_F_MonthlyCustomerAccountTotalsRepository(DbContext context) : base(context){}
    }
}
