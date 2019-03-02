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
    public class tbl_C_StatusRepository : BaseRepository<tbl_C_Status> , Itbl_C_StatusRepository
    {
        public tbl_C_StatusRepository(DbContext context) : base(context){}
    }
}
