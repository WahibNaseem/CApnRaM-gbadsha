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
    public class tbl_C_BillingAddressRepository : BaseRepository<tbl_C_BillingAddress> , Itbl_C_BillingAddressRepository
    {
        public tbl_C_BillingAddressRepository(DbContext context) : base(context){}
    }
}
