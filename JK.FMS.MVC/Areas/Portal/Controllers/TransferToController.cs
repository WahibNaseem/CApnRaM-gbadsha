using MvcBreadCrumbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JKApi.Data.DAL;
using JKViewModels.Management;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using JKApi.Service.Helper.Extension;
using Application.Web.Core;
using JKApi.Service.ServiceContract.Management;
using JKApi.Data.DTOObject;
using JKViewModels;
using JKViewModels.Customer;
using MoreLinq;

namespace JK.FMS.MVC.Areas.Portal.Controllers
{
    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    [Filter.RoleBasedAuthorize]
    [BreadCrumb(Clear = true, Label = "Company", Order = -1)]
    public class TransferToController : ViewControllerBase
    {
        #region Constructor
        public TransferToController()
        {
            //ManagementService = managementService;
            //this._commonService = commonService;
            //ViewBag.HMenu = "Management";
        }

        #endregion
        
    }
}