using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace JK.Repository.Repositories
{
    public class CRM_NoteTypeRespository : BaseRepository<CRM_NoteType>, ICRM_NoteTypeRepository
    {
        public CRM_NoteTypeRespository(DbContext dbContext) : base(dbContext) { }   
    }
}
