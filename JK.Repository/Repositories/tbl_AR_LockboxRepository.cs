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
    public class tbl_AR_LockboxRepository : BaseRepository<tbl_AR_Lockbox> , Itbl_AR_LockboxRepository
    {
        public tbl_AR_LockboxRepository(DbContext context) : base(context){}
    }
}
