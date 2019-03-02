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
    public class tbl_C_TrxDistributionHistoryRepository : BaseRepository<tbl_C_TrxDistributionHistory> , Itbl_C_TrxDistributionHistoryRepository
    {
        public tbl_C_TrxDistributionHistoryRepository(DbContext context) : base(context){}
    }
}
