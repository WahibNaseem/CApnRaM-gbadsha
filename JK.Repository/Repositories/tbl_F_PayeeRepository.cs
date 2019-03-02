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
    public class tbl_F_PayeeRepository : BaseRepository<tbl_F_Payee> , Itbl_F_PayeeRepository
    {
        public tbl_F_PayeeRepository(DbContext context) : base(context){}
    }
}
