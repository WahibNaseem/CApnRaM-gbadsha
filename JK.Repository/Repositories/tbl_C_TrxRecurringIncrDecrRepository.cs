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
    public class tbl_C_TrxRecurringIncrDecrRepository : BaseRepository<tbl_C_TrxRecurringIncrDecr> , Itbl_C_TrxRecurringIncrDecrRepository
    {
        public tbl_C_TrxRecurringIncrDecrRepository(DbContext context) : base(context){}
    }
}
