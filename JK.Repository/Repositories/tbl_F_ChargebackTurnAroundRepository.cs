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
    public class tbl_F_ChargebackTurnAroundRepository : BaseRepository<tbl_F_ChargebackTurnAround> , Itbl_F_ChargebackTurnAroundRepository
    {
        public tbl_F_ChargebackTurnAroundRepository(DbContext context) : base(context){}
    }
}
