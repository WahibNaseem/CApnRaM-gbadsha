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
    public class tbl_C_TrxRepository : BaseRepository<tbl_C_Trx> , Itbl_C_TrxRepository
    {
        public tbl_C_TrxRepository(DbContext context) : base(context){}
    }
}
