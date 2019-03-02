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
    public class tbl_AR_InvoiceDetailDescriptionRepository : BaseRepository<tbl_AR_InvoiceDetailDescription> , Itbl_AR_InvoiceDetailDescriptionRepository
    {
        public tbl_AR_InvoiceDetailDescriptionRepository(DbContext context) : base(context){}
    }
}
