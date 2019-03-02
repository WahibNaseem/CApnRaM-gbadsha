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
    public class tbl_AR_PaymentDetailRepository : BaseRepository<tbl_AR_PaymentDetail> , Itbl_AR_PaymentDetailRepository
    {
        public tbl_AR_PaymentDetailRepository(DbContext context) : base(context){}
    }
}
