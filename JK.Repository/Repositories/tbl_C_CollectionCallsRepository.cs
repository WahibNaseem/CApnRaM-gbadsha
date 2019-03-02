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
    public class tbl_C_CollectionCallsRepository : BaseRepository<tbl_C_CollectionCalls> , Itbl_C_CollectionCallsRepository
    {
        public tbl_C_CollectionCallsRepository(DbContext context) : base(context){}
    }
}
