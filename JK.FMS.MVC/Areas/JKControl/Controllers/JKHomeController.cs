using Application.Web.Core;
using JKViewModels.Common;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.JKControl.Controllers
{
    public class JKHomeController : ViewControllerBase
    { 
        // GET: JKControl/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeCompany(int prm)
        {

            var userViewModel = _claimView.GetCLAIM_PERSON_INFORMATION();
            if (userViewModel != null)
            {
                var selectedCompanyDetail = userViewModel.Regions.Where(x => x.RegionId == prm).FirstOrDefault();

                var newIdentity = new ClaimsIdentity(User.Identity); 
                newIdentity.RemoveClaim(newIdentity.FindFirst(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID));
                newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_ID, Convert.ToString(prm)));

                newIdentity.RemoveClaim(newIdentity.FindFirst(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS));
                newIdentity.AddClaim(new Claim(ClaimTypeName.CLAIM_SELECTED_COMPANY_DETAILS, JsonConvert.SerializeObject(selectedCompanyDetail)));

                //Regenerate user identity
                HttpContext.GetOwinContext().Authentication.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(newIdentity), new AuthenticationProperties() { IsPersistent = false });
                
                return RedirectToAction("Index", "JKHome", new { area = "JKControl" });
            }
            return RedirectToAction("LogOff", "User", new { area = "JKControl" });
        }
    }
}