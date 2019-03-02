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
    public class tbl_F_TrxRecurringStatusHistRepository : BaseRepository<tbl_F_TrxRecurringStatusHist> , Itbl_F_TrxRecurringStatusHistRepository
    {
        public tbl_F_TrxRecurringStatusHistRepository(DbContext context) : base(context){}
    }
}
