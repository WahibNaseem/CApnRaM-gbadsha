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
    public class tbl_F_TrxFeeTmpRepository : BaseRepository<tbl_F_TrxFeeTmp> , Itbl_F_TrxFeeTmpRepository
    {
        public tbl_F_TrxFeeTmpRepository(DbContext context) : base(context){}
    }
}
