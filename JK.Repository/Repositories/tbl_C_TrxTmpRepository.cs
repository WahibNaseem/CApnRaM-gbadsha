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
    public class tbl_C_TrxTmpRepository : BaseRepository<tbl_C_TrxTmp> , Itbl_C_TrxTmpRepository
    {
        public tbl_C_TrxTmpRepository(DbContext context) : base(context){}
    }
}
