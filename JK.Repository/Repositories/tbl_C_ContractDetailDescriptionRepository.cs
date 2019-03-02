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
    public class tbl_C_ContractDetailDescriptionRepository : BaseRepository<tbl_C_ContractDetailDescription> , Itbl_C_ContractDetailDescriptionRepository
    {
        public tbl_C_ContractDetailDescriptionRepository(DbContext context) : base(context){}
    }
}
