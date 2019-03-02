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
    public class tbl_GL_PartyTypeRepository : BaseRepository<tbl_GL_PartyType> , Itbl_GL_PartyTypeRepository
    {
        public tbl_GL_PartyTypeRepository(DbContext context) : base(context){}
    }
}
