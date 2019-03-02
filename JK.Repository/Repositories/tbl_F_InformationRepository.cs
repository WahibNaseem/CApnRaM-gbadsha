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
    public class tbl_F_InformationRepository : BaseRepository<tbl_F_Information> , Itbl_F_InformationRepository
    {
        public tbl_F_InformationRepository(DbContext context) : base(context){}
    }
}
