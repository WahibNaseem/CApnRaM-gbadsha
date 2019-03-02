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
    public class tbl_F_InformationStatusHistRepository : BaseRepository<tbl_F_InformationStatusHist> , Itbl_F_InformationStatusHistRepository
    {
        public tbl_F_InformationStatusHistRepository(DbContext context) : base(context){}
    }
}
