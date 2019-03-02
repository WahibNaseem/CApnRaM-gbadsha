namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    using MvcBreadCrumbs;
    using System.Web.Mvc;
    using Application.Web.Core;
    using JKViewModels;

    [BreadCrumb(Clear = true)]
    public class CRMDashboardController : ViewControllerBase
    {
        public ActionResult Index()
        {
            ViewBag.HMenu = "CRM";

            ViewBag.CurrentMenu = "DashBoard";
            BreadCrumb.Clear();
            BreadCrumb.Add(Url.Action("Index", "DashBoard", new { area = "CRM" }), "DashBoard");

            DashboardViewModel DashboardViewModel = new DashboardViewModel();

            return View(DashboardViewModel);
        }
    }
}