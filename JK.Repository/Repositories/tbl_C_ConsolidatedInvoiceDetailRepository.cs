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
    public class tbl_C_ConsolidatedInvoiceDetailRepository : BaseRepository<tbl_C_ConsolidatedInvoiceDetail> , Itbl_C_ConsolidatedInvoiceDetailRepository
    {
        public tbl_C_ConsolidatedInvoiceDetailRepository(DbContext context) : base(context){}
    }
}
