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
    public class tbl_F_FindersFeeStatusHistRepository : BaseRepository<tbl_F_FindersFeeStatusHist> , Itbl_F_FindersFeeStatusHistRepository
    {
        public tbl_F_FindersFeeStatusHistRepository(DbContext context) : base(context){}
    }
}
