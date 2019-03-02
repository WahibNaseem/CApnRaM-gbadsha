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
    public class tbl_F_MonthlyTotalBillingDetailRepository : BaseRepository<tbl_F_MonthlyTotalBillingDetail> , Itbl_F_MonthlyTotalBillingDetailRepository
    {
        public tbl_F_MonthlyTotalBillingDetailRepository(DbContext context) : base(context){}
    }
}
