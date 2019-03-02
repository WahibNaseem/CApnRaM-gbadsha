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
    public class tbl_AR_PaymentMoveReasonRepository : BaseRepository<tbl_AR_PaymentMoveReason> , Itbl_AR_PaymentMoveReasonRepository
    {
        public tbl_AR_PaymentMoveReasonRepository(DbContext context) : base(context){}
    }
}
