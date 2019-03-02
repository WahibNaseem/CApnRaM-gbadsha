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
    public class CRMTerritoryAssignmentNewRepository : BaseRepository<CRM_Territory_Assignment_New>, ICRMTerritoryAssignmentNewRepository
    {
        public CRMTerritoryAssignmentNewRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
