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
    public class tbl_C_InformationStatusHistRepository : BaseRepository<tbl_C_InformationStatusHist> , Itbl_C_InformationStatusHistRepository
    {
        public tbl_C_InformationStatusHistRepository(DbContext context) : base(context){}
    }
}
