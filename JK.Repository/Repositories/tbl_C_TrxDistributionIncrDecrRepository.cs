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
    public class tbl_C_TrxDistributionIncrDecrRepository : BaseRepository<tbl_C_TrxDistributionIncrDecr> , Itbl_C_TrxDistributionIncrDecrRepository
    {
        public tbl_C_TrxDistributionIncrDecrRepository(DbContext context) : base(context){}
    }
}
