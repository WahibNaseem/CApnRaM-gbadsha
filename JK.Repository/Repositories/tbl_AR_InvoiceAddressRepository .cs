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
    public class tbl_AR_InvoiceAddressRepository : BaseRepository<tbl_AR_InvoiceAddress> , Itbl_AR_InvoiceAddressRepository
    {
        public tbl_AR_InvoiceAddressRepository(DbContext context) : base(context){}
    }
}
