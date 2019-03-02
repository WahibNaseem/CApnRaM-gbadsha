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
    public class tbl_F_NegativeDuePaymentRepository : BaseRepository<tbl_F_NegativeDuePayment> , Itbl_F_NegativeDuePaymentRepository
    {
        public tbl_F_NegativeDuePaymentRepository(DbContext context) : base(context){}
    }
}
