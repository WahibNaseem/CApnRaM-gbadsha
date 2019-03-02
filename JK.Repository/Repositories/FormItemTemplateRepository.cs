﻿using System.Data.Entity;
using JK.Repository.Contracts;
using JKApi.Data.DAL;

namespace JK.Repository.Repositories
{
    public class FormItemTemplateRepository : BaseRepository<FormItemTemplate>, IFormItemTemplateRepository
    {
        public FormItemTemplateRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}