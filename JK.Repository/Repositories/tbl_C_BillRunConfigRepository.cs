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
    public class tbl_C_BillRunConfigRepository : BaseRepository<tbl_C_BillRunConfig> , Itbl_C_BillRunConfigRepository
    {
        public tbl_C_BillRunConfigRepository(DbContext context) : base(context){}
    }
}
