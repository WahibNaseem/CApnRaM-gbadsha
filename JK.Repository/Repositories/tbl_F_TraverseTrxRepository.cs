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
    public class tbl_F_TraverseTrxRepository : BaseRepository<tbl_F_TraverseTrx> , Itbl_F_TraverseTrxRepository
    {
        public tbl_F_TraverseTrxRepository(DbContext context) : base(context){}
    }
}
