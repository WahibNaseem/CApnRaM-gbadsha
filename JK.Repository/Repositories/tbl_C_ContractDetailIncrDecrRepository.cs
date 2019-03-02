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
    public class tbl_C_ContractDetailIncrDecrRepository : BaseRepository<tbl_C_ContractDetailIncrDecr> , Itbl_C_ContractDetailIncrDecrRepository
    {
        public tbl_C_ContractDetailIncrDecrRepository(DbContext context) : base(context){}
    }
}