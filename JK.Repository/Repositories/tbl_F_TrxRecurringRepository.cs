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
    public class tbl_F_TrxRecurringRepository : BaseRepository<tbl_F_TrxRecurring> , Itbl_F_TrxRecurringRepository
    {
        public tbl_F_TrxRecurringRepository(DbContext context) : base(context){}
    }
}
