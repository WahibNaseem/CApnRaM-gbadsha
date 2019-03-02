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
    public class tbl_F_MonthlyTotalRepository : BaseRepository<tbl_F_MonthlyTotal> , Itbl_F_MonthlyTotalRepository
    {
        public tbl_F_MonthlyTotalRepository(DbContext context) : base(context){}
    }
}
