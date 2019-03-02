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
    public class tbl_AR_InvoiceHeaderRepository : BaseRepository<tbl_AR_InvoiceHeader> , Itbl_AR_InvoiceHeaderRepository
    {
        public tbl_AR_InvoiceHeaderRepository(DbContext context) : base(context){}
    }
}
