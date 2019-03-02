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
    public class tbl_C_PaymentsRepository : BaseRepository<tbl_C_Payments> , Itbl_C_PaymentsRepository
    {
        public tbl_C_PaymentsRepository(DbContext context) : base(context){}
    }
}
