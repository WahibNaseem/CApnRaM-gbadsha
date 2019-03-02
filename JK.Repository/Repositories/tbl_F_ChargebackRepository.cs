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
    public class tbl_F_ChargebackRepository : BaseRepository<tbl_F_Chargeback> , Itbl_F_ChargebackRepository
    {
        public tbl_F_ChargebackRepository(DbContext context) : base(context){}
    }
}
