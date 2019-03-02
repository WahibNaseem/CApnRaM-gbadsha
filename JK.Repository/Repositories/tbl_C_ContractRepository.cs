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
    public class tbl_C_ContractRepository : BaseRepository<tbl_C_Contract> , Itbl_C_ContractRepository
    {
        public tbl_C_ContractRepository(DbContext context) : base(context){}
    }
}
