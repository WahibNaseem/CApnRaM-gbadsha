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
    public class tbl_R_DeductionPayeeRepository : BaseRepository<tbl_R_DeductionPayee> , Itbl_R_DeductionPayeeRepository
    {
        public tbl_R_DeductionPayeeRepository(DbContext context) : base(context){}
    }
}
