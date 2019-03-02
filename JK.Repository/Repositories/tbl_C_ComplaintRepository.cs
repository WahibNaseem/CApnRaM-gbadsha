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
    public class tbl_C_ComplaintRepository : BaseRepository<tbl_C_Complaint> , Itbl_C_ComplaintRepository
    {
        public tbl_C_ComplaintRepository(DbContext context) : base(context){}
    }
}