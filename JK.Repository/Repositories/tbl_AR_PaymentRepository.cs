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
    public class tbl_AR_PaymentRepository : BaseRepository<tbl_AR_Payment> , Itbl_AR_PaymentRepository
    {
        public tbl_AR_PaymentRepository(DbContext context) : base(context){}
    }
}
