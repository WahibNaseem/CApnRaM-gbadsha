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
    public class tbl_sys_C_ServiceLogAreaRepository : BaseRepository<tbl_sys_C_ServiceLogArea> , Itbl_sys_C_ServiceLogAreaRepository
    {
        public tbl_sys_C_ServiceLogAreaRepository(DbContext context) : base(context){}
    }
}