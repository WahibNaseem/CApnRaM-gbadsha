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
    public class tbl_C_Contract_AdjHistRepository : BaseRepository<tbl_C_Contract_AdjHist> , Itbl_C_Contract_AdjHistRepository
    {
        public tbl_C_Contract_AdjHistRepository(DbContext context) : base(context){}
    }
}
