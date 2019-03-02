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
    public class tbl_RegisterDetails_2Repository : BaseRepository<tbl_RegisterDetails_2> , Itbl_RegisterDetails_2Repository
    {
        public tbl_RegisterDetails_2Repository(DbContext context) : base(context){}
    }
}
