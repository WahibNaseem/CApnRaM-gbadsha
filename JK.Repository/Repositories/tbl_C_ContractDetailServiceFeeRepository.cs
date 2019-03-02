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
    public class tbl_C_ContractDetailServiceFeeRepository : BaseRepository<tbl_C_ContractDetailServiceFee> , Itbl_C_ContractDetailServiceFeeRepository
    {
        public tbl_C_ContractDetailServiceFeeRepository(DbContext context) : base(context){}
    }
}
