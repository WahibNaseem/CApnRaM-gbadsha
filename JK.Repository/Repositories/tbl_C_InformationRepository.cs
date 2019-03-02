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
    public class tbl_C_InformationRepository : BaseRepository<tbl_C_Information> , Itbl_C_InformationRepository
    {
        public tbl_C_InformationRepository(DbContext context) : base(context){}
    }
}
