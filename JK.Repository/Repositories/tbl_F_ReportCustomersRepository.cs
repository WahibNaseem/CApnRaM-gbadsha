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
    public class tbl_F_ReportCustomersRepository : BaseRepository<tbl_F_ReportCustomers> , Itbl_F_ReportCustomersRepository
    {
        public tbl_F_ReportCustomersRepository(DbContext context) : base(context){}
    }
}
