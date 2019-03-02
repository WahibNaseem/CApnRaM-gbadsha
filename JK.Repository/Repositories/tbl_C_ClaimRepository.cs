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
    public class tbl_C_ClaimRepository : BaseRepository<tbl_C_Claim> , Itbl_C_ClaimRepository
    {
        public tbl_C_ClaimRepository(DbContext context) : base(context){}
    }
}
