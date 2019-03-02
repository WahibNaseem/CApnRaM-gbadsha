using JKApi.Business.Domain;
using JKApi.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JKApi.WebAPI.Mapper
{
    public interface IMapper
    {
        IDomainBase ModelToDomain(IModelBase modelObj);

        IModelBase DomainToModel(IDomainBase domainObj);
    }
}