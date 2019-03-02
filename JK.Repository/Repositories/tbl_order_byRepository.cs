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
    public class tbl_order_byRepository : BaseRepository<tbl_order_by> , Itbl_order_byRepository
    {
        public tbl_order_byRepository(DbContext context) : base(context){}
    }
}
