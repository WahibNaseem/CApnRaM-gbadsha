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
    public class tbl_C_TrxBatchRepository : BaseRepository<tbl_C_TrxBatch> , Itbl_C_TrxBatchRepository
    {
        public tbl_C_TrxBatchRepository(DbContext context) : base(context){}
    }
}
