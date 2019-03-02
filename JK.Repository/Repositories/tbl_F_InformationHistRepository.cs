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
    public class tbl_F_InformationHistRepository : BaseRepository<tbl_F_InformationHist> , Itbl_F_InformationHistRepository
    {
        public tbl_F_InformationHistRepository(DbContext context) : base(context){}
    }
}
