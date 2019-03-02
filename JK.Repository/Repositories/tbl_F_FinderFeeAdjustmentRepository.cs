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
    public class tbl_F_FinderFeeAdjustmentRepository : BaseRepository<tbl_F_FinderFeeAdjustment> , Itbl_F_FinderFeeAdjustmentRepository
    {
        public tbl_F_FinderFeeAdjustmentRepository(DbContext context) : base(context){}
    }
}
