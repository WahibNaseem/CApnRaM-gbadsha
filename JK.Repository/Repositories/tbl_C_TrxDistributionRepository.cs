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
    public class tbl_C_TrxDistributionRepository : BaseRepository<tbl_C_TrxDistribution> , Itbl_C_TrxDistributionRepository
    {
        public tbl_C_TrxDistributionRepository(DbContext context) : base(context){}
    }
}
