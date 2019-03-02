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
    public class tbl_C_ContractFeeRepository : BaseRepository<tbl_C_ContractFee> , Itbl_C_ContractFeeRepository
    {
        public tbl_C_ContractFeeRepository(DbContext context) : base(context){}
    }
}
