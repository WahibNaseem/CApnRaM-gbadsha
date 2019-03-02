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
    public class tbl_includeRepository : BaseRepository<tbl_include> , Itbl_includeRepository
    {
        public tbl_includeRepository(DbContext context) : base(context){}
    }
}
