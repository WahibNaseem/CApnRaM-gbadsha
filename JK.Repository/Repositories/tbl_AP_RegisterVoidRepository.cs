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
    public class tbl_AP_RegisterVoidRepository : BaseRepository<tbl_AP_RegisterVoid> , Itbl_AP_RegisterVoidRepository
    {
        public tbl_AP_RegisterVoidRepository(DbContext context) : base(context){}
    }
}
