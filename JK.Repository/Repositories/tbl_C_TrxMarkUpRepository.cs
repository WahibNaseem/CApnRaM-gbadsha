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
    public class tbl_C_TrxMarkUpRepository : BaseRepository<tbl_C_TrxMarkUp> , Itbl_C_TrxMarkUpRepository
    {
        public tbl_C_TrxMarkUpRepository(DbContext context) : base(context){}
    }
}
