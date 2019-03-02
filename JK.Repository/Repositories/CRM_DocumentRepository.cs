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
   public class CRM_DocumentRepository : BaseRepository<CRM_Document>, ICRM_DocumentRepository
    {
       public CRM_DocumentRepository(DbContext context) : base(context) { }
    }
}
