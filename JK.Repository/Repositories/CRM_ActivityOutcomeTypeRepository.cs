﻿using JK.Repository.Contracts;
using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JK.Repository.Repositories
{
    public class CRM_ActivityOutcomeTypeRepository : BaseRepository<CRM_ActivityOutcomeType>, ICRM_ActivityOutcomeTypeRepository
    {
        public CRM_ActivityOutcomeTypeRepository(DbContext context) : base(context) { }
    }
}
