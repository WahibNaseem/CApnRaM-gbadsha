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
    public class tbl_C_TrxRecurringRepository : BaseRepository<tbl_C_TrxRecurring> , Itbl_C_TrxRecurringRepository
    {
        public tbl_C_TrxRecurringRepository(DbContext context) : base(context){}
    }
}
