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
    public class tbl_F_LogRepository : BaseRepository<tbl_F_Log> , Itbl_F_LogRepository
    {
        public tbl_F_LogRepository(DbContext context) : base(context){}
    }
}
