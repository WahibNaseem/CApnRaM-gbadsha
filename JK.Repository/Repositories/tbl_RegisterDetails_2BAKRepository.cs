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
    public class tbl_RegisterDetails_2BAKRepository : BaseRepository<tbl_RegisterDetails_2BAK> , Itbl_RegisterDetails_2BAKRepository
    {
        public tbl_RegisterDetails_2BAKRepository(DbContext context) : base(context){}
    }
}
