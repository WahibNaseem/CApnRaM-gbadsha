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
    public class tbl_Sys_AccountRebateRepository : BaseRepository<tbl_Sys_AccountRebate> , Itbl_Sys_AccountRebateRepository
    {
        public tbl_Sys_AccountRebateRepository(DbContext context) : base(context){}
    }
}
