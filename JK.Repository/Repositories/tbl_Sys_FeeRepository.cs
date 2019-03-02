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
    public class tbl_Sys_FeeRepository : BaseRepository<tbl_Sys_Fee> , Itbl_Sys_FeeRepository
    {
        public tbl_Sys_FeeRepository(DbContext context) : base(context){}
    }
}
