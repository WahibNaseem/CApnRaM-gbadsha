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
  public  class CRM_FvPresentationRepository :BaseRepository<CRM_FvPresentation> ,ICRM_FvPresentationRepository
    {
        public CRM_FvPresentationRepository(DbContext dbContext) : base(dbContext) { }
    }                 
}
