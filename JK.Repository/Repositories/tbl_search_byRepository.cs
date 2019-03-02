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
    public class tbl_search_byRepository : BaseRepository<tbl_search_by> , Itbl_search_byRepository
    {
        public tbl_search_byRepository(DbContext context) : base(context){}
    }
}
