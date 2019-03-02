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
    public class CustomerLogRepository : BaseRepository<CustomerLog> , ICustomerLogRepository
    {
        public CustomerLogRepository(DbContext context) : base(context){}
    }
}
