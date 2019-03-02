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
    public class tbl_Register_2Repository : BaseRepository<tbl_Register_2> , Itbl_Register_2Repository
    {
        public tbl_Register_2Repository(DbContext context) : base(context){}
    }
}
