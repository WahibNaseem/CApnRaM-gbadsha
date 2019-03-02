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
    public class CRM_InitialCommunicationRepository : BaseRepository<CRM_InitialCommunication>, ICRM_InitialCommunicationRepository
    {
        public CRM_InitialCommunicationRepository(DbContext dbContext) : base(dbContext) { }
        
    }
  
}
