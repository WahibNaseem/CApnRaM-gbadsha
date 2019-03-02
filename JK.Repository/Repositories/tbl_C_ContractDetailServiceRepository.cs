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
    public class tbl_C_ContractDetailServiceRepository : BaseRepository<tbl_C_ContractDetailService> , Itbl_C_ContractDetailServiceRepository
    {
        public tbl_C_ContractDetailServiceRepository(DbContext context) : base(context){}
    }
}
