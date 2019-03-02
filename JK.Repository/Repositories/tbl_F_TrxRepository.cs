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
    public class tbl_F_TrxRepository : BaseRepository<tbl_F_Trx> , Itbl_F_TrxRepository
    {
        public tbl_F_TrxRepository(DbContext context) : base(context){}
    }
}
