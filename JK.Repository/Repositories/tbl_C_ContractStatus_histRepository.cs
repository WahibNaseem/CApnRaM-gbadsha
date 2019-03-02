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
    public class tbl_C_ContractStatus_histRepository : BaseRepository<tbl_C_ContractStatus_hist> , Itbl_C_ContractStatus_histRepository
    {
        public tbl_C_ContractStatus_histRepository(DbContext context) : base(context){}
    }
}
