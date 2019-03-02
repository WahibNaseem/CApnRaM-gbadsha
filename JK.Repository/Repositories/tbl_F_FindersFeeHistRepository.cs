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
    public class tbl_F_FindersFeeHistRepository : BaseRepository<tbl_F_FindersFeeHist> , Itbl_F_FindersFeeHistRepository
    {
        public tbl_F_FindersFeeHistRepository(DbContext context) : base(context){}
    }
}
