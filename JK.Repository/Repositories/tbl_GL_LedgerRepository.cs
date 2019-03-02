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
    public class tbl_GL_LedgerRepository : BaseRepository<tbl_GL_Ledger> , Itbl_GL_LedgerRepository
    {
        public tbl_GL_LedgerRepository(DbContext context) : base(context){}
    }
}
