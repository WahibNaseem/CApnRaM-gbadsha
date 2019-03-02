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
    public class tbl_F_DistributionRepository : BaseRepository<tbl_F_Distribution> , Itbl_F_DistributionRepository
    {
        public tbl_F_DistributionRepository(DbContext context) : base(context){}
    }
}
