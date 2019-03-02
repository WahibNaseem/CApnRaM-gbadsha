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
    public class tbl_AR_OtherDepositsRepository : BaseRepository<tbl_AR_OtherDeposits> , Itbl_AR_OtherDepositsRepository
    {
        public tbl_AR_OtherDepositsRepository(DbContext context) : base(context){}
    }
}
