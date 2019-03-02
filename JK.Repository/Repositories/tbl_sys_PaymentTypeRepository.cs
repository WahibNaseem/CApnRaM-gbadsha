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
    public class tbl_sys_PaymentTypeRepository : BaseRepository<tbl_sys_PaymentType> , Itbl_sys_PaymentTypeRepository
    {
        public tbl_sys_PaymentTypeRepository(DbContext context) : base(context){}
    }
}
