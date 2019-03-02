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
    public class tbl_C_ContractIncrDecrRepository : BaseRepository<tbl_C_ContractIncrDecr> , Itbl_C_ContractIncrDecrRepository
    {
        public tbl_C_ContractIncrDecrRepository(DbContext context) : base(context){}
    }
}
