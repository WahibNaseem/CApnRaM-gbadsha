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
    public class tbl_sys_TransactionTypeGroupRepository : BaseRepository<tbl_sys_TransactionTypeGroup> , Itbl_sys_TransactionTypeGroupRepository
    {
        public tbl_sys_TransactionTypeGroupRepository(DbContext context) : base(context){}
    }
}
