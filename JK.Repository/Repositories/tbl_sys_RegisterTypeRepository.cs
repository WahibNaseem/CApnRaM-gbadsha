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
    public class tbl_sys_RegisterTypeRepository : BaseRepository<tbl_sys_RegisterType> , Itbl_sys_RegisterTypeRepository
    {
        public tbl_sys_RegisterTypeRepository(DbContext context) : base(context){}
    }
}
