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
    public class tbl_F_TrxTmpRepository : BaseRepository<tbl_F_TrxTmp> , Itbl_F_TrxTmpRepository
    {
        public tbl_F_TrxTmpRepository(DbContext context) : base(context){}
    }
}
