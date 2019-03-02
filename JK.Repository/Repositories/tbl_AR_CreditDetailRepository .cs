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
    public class tbl_AR_CreditDetailRepository : BaseRepository<tbl_AR_CreditDetail> , Itbl_AR_CreditDetailRepository
    {
        public tbl_AR_CreditDetailRepository(DbContext context) : base(context){}
    }
}
