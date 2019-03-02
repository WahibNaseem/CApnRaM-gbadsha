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
    public class tbl_sys_PaymentTypeGroupRepository : BaseRepository<tbl_sys_PaymentTypeGroup> , Itbl_sys_PaymentTypeGroupRepository
    {
        public tbl_sys_PaymentTypeGroupRepository(DbContext context) : base(context){}
    }
}
