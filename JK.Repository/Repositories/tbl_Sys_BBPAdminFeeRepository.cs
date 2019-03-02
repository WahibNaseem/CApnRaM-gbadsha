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
    public class tbl_Sys_BBPAdminFeeRepository : BaseRepository<tbl_Sys_BBPAdminFee> , Itbl_Sys_BBPAdminFeeRepository
    {
        public tbl_Sys_BBPAdminFeeRepository(DbContext context) : base(context){}
    }
}
