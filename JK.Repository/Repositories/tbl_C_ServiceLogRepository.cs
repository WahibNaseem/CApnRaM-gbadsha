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
    public class tbl_C_ServiceLogRepository : BaseRepository<tbl_C_ServiceLog> , Itbl_C_ServiceLogRepository
    {
        public tbl_C_ServiceLogRepository(DbContext context) : base(context){}
    }
}
