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
    public class tbl_AR_CreditRepository : BaseRepository<tbl_AR_Credit> , Itbl_AR_CreditRepository
    {
        public tbl_AR_CreditRepository(DbContext context) : base(context){}
    }
}
