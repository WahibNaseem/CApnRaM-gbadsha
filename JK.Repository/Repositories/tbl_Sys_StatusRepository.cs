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
    public class tbl_Sys_StatusRepository : BaseRepository<tbl_Sys_Status> , Itbl_Sys_StatusRepository
    {
        public tbl_Sys_StatusRepository(DbContext context) : base(context){}
    }
}
