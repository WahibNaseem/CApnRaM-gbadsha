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
    public class tbl_F_FindersFeeRepository : BaseRepository<tbl_F_FindersFee> , Itbl_F_FindersFeeRepository
    {
        public tbl_F_FindersFeeRepository(DbContext context) : base(context){}
    }
}
