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
    public class tbl_Sys_ConfigurationRepository : BaseRepository<tbl_Sys_Configuration> , Itbl_Sys_ConfigurationRepository
    {
        public tbl_Sys_ConfigurationRepository(DbContext context) : base(context){}
    }
}
