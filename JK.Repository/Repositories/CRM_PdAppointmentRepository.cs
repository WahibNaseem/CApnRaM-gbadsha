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
    public class CRM_PdAppointmentRepository : BaseRepository<CRM_PdAppointment>, ICRM_PdAppointmentRepository
    {
        public CRM_PdAppointmentRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
