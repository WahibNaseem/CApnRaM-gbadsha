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
  public class CRMCloseTempDocumentRepository : BaseRepository<CRM_CloseTempDocument>, ICRMCloseTempDocumentRepository
    {
        public CRMCloseTempDocumentRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
