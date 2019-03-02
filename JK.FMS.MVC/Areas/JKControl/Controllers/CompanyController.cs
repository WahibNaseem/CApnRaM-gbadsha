using Application.Web.Core;
using JK.Resources;
using JKApi.Service.ServiceContract.Company;
using JKApi.Service.ServiceContract.JKControl;
using JKViewModels;
using JKViewModels.JKControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.JKControl.Controllers
{
    public class CompanyController : ViewControllerBase
    {

        public CompanyController(ICompanyService companyService)
        {
            this._companyService = companyService;
        }

        // GET: JKControl/Company
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save(string prm = "")
        {
            return View();
        }

        #region Company Listing
        public ActionResult List()
        {
            RegionViewListModel companyViewListModel = new RegionViewListModel();
            companyViewListModel.CurrentPage = 1;
            companyViewListModel.CurrentPage = Convert.ToInt32(WebConfigResource.ListPageSize);
            return View(companyViewListModel);
        }

        public ActionResult SearchContent(RegionViewListModel companyViewListModel)
        {
            return PartialView("_CompanyList", companyViewListModel);
        }

        public ActionResult DeactivateCompany(RegionViewListModel companyViewListModel)
        {
            return Json(new { Message = companyViewListModel.Message, MessageType = companyViewListModel.MessageType }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ActivateCompany(RegionViewListModel companyViewListModel)
        {
            return Json(new { Message = companyViewListModel.Message, MessageType = companyViewListModel.MessageType }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteContent(RegionViewListModel companyViewListModel)
        {
            return Json(new { Message = companyViewListModel.Message, MessageType = companyViewListModel.MessageType }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}