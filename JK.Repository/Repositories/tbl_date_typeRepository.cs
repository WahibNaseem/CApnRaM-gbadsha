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
    public class tbl_date_typeRepository : BaseRepository<tbl_date_type> , Itbl_date_typeRepository
    {
        public tbl_date_typeRepository(DbContext context) : base(context){}
    }
}
