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
    public class tbl_Register_2BAKRepository : BaseRepository<tbl_Register_2BAK> , Itbl_Register_2BAKRepository
    {
        public tbl_Register_2BAKRepository(DbContext context) : base(context){}
    }
}
