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
    public class tbl_F_FulfillmentRepository : BaseRepository<tbl_F_Fulfillment> , Itbl_F_FulfillmentRepository
    {
        public tbl_F_FulfillmentRepository(DbContext context) : base(context){}
    }
}
