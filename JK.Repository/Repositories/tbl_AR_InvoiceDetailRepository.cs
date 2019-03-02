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
    public class tbl_AR_InvoiceDetailRepository : BaseRepository<tbl_AR_InvoiceDetail> , Itbl_AR_InvoiceDetailRepository
    {
        public tbl_AR_InvoiceDetailRepository(DbContext context) : base(context){}
    }
}
