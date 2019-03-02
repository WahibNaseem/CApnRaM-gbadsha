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
    public class tbl_Sys_StatusReasonRepository : BaseRepository<tbl_Sys_StatusReason> , Itbl_Sys_StatusReasonRepository
    {
        public tbl_Sys_StatusReasonRepository(DbContext context) : base(context){}
    }
}
