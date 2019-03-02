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
    public class tbl_F_FulfillmentHistRepository : BaseRepository<tbl_F_FulfillmentHist> , Itbl_F_FulfillmentHistRepository
    {
        public tbl_F_FulfillmentHistRepository(DbContext context) : base(context){}
    }
}
