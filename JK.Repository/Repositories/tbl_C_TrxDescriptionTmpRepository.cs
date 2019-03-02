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
    public class tbl_C_TrxDescriptionTmpRepository : BaseRepository<tbl_C_TrxDescriptionTmp> , Itbl_C_TrxDescriptionTmpRepository
    {
        public tbl_C_TrxDescriptionTmpRepository(DbContext context) : base(context){}
    }
}
