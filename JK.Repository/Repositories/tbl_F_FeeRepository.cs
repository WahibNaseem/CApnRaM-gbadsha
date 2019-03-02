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
    public class tbl_F_FeeRepository : BaseRepository<tbl_F_Fee> , Itbl_F_FeeRepository
    {
        public tbl_F_FeeRepository(DbContext context) : base(context){}
    }
}
