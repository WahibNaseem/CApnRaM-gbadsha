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
    public class tbl_C_ConsolidatedInvoiceRepository : BaseRepository<tbl_C_ConsolidatedInvoice> , Itbl_C_ConsolidatedInvoiceRepository
    {
        public tbl_C_ConsolidatedInvoiceRepository(DbContext context) : base(context){}
    }
}
