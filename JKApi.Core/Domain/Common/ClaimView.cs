using JKApi.Data.DAL;
using JKViewModels;
using JKViewModels.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace JKApi.Core.Common
{
    public class ClaimView
    {
        private static readonly Lazy<ClaimView> lazy = new Lazy<ClaimView>(() => new ClaimView());
        public static ClaimView Instance { get { return lazy.Value; } }

        public string GetCLAIM_USERNAME()
        {
            ////Get the user's claims
            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_USERNAME.ToLower()).Select(clm => clm.Value).FirstOrDefault();
            //}
            //else
            //{
            //    return string.Empty;
            //}

            if (HttpContext.Current.Session["UserData"] != null)
                return ((UserViewModel)HttpContext.Current.Session["UserData"]).UserName;
            else
                return null;
        }

        public string GetCLAIM_USERID()
        {
            ////Get the user's claims
            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_USERID.ToLower()).Select(clm => clm.Value).FirstOrDefault();
            //}
            //else
            //{
            //    return string.Empty;
            //}

            if (HttpContext.Current.Session["UserData"] != null)
                return ((UserViewModel)HttpContext.Current.Session["UserData"]).UserId.ToString();
            else
                return null;
        }

        public string GetCLAIM_ROLE_TYPE()
        {
            //Get the user's claims
            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_ROLE_TYPE.ToLower()).Select(clm => clm.Value).FirstOrDefault();
            //}
            //else
            //{
            //    return string.Empty;
            //}
            return string.Empty;
        }

        public string GetCLAIM_Role()
        {
            //Get the user's claims
            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypes.Role.ToLower()).Select(clm => clm.Value).FirstOrDefault();
            //}
            //else
            //{
            //    return string.Empty;
            //}

            return string.Empty;
        }

        public UserViewModel GetCLAIM_PERSON_INFORMATION()
        {
            if (HttpContext.Current.Session["UserData"] != null)
                return (UserViewModel)HttpContext.Current.Session["UserData"];
            else
                return null;
            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0 && currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_PERSON_INFORMATION.ToLower()).Select(clm => clm.Value).Any())
            //{
            //    return JsonConvert.DeserializeObject<UserViewModel>(currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_PERSON_INFORMATION.ToLower()).Select(clm => clm.Value).FirstOrDefault());
            //}
            //else
            //{
            //    return null;
            //}
        }


        public int GetCLAIM_SELECTED_PERIOD_ID()
        {
            //Get the user's claims
            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return Convert.ToInt32(currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_SELECTED_PERIOD_ID.ToLower()).Select(clm => clm.Value).FirstOrDefault());
            //}
            //else
            //{
            //    return 0;
            //}

            if (HttpContext.Current.Session["SelectedPeriodId"] != null)
                return (int)HttpContext.Current.Session["SelectedPeriodId"];
            else
                return 0;
        }

        public int GetCLAIM_PERIOD_ID()
        {
            int PeriodId = 0;
            int claimId = GetCLAIM_SELECTED_PERIOD_ID();
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data = context.PeriodCloseds.Where(x => x.PeriodClosedId == claimId).FirstOrDefault();
                if (data != null)
                {
                    PeriodId = (int)data.PeriodId;
                }
            }
            return PeriodId;
        }

        public int UpdateLoginTracking(int UserId, int TrackingId, int MenuId,string refUrl)
        {

            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data = context.Auth_UpdateLoginTracking(UserId, TrackingId, MenuId,refUrl);
            }
            
            return 0;
        }

        public int IsLoggedinTracking(int UserId, int TrackingId)
        {
            using (jkDatabaseEntities context = new jkDatabaseEntities())
            {
                var data = context.AuthLoginTrackings.Where(x => x.UserId == UserId && x.LoginTrackingId == TrackingId && x.LogoutDateTime == null).ToList();
                if (data != null && data.Count > 0)
                {
                    return 1;
                }
            }

            return 0;
        }

        public string GetCLAIM_SELECTED_COMPANY_ID()
        {
            //Get the user's claims
            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_SELECTED_COMPANY_ID.ToLower()).Select(clm => clm.Value).FirstOrDefault();
            //}
            //else
            //{
            //    return null;
            //}

            if (HttpContext.Current.Session["SelectedRegionId"] != null)
                return (string)HttpContext.Current.Session["SelectedRegionId"].ToString();
            else
                return "0";

        }

        public int GetCLAIM_LoginTrackingId()
        {
            if (HttpContext.Current.Session["LoginTrackingId"] != null)
                return (int)HttpContext.Current.Session["LoginTrackingId"];
            else
                return 0;

        }

        public List<RoleModel> GetCLAIM_ROLELIST()
        {
            if (HttpContext.Current.Session["UserData"] != null)
                return ((UserViewModel)HttpContext.Current.Session["UserData"]).Roles;
            else
                return null;

            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return JsonConvert.DeserializeObject<UserViewModel>(currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_PERSON_INFORMATION.ToLower()).Select(clm => clm.Value).FirstOrDefault()).Roles;
            //}
            //else
            //{
            //    return null;
            //}
        }

        public List<RolebasedARAccessDetailModel> GETCLAIM_AR_PERMISSION()
        {

            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return JsonConvert.DeserializeObject<UserViewModel>(currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_PERSON_INFORMATION.ToLower()).Select(clm => clm.Value).FirstOrDefault()).lstRolebasedARAccessDetailModel;
            //}
            //else
            //{
            //    return null;
            //}

            if (HttpContext.Current.Session["UserData"] != null)
                return ((UserViewModel)HttpContext.Current.Session["UserData"]).lstRolebasedARAccessDetailModel;
            else
                return null;
        }

        public List<PeriodAccessModel> GetCLAIM_PeriodAccess()
        {

            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return JsonConvert.DeserializeObject<UserViewModel>(currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_PERSON_INFORMATION.ToLower()).Select(clm => clm.Value).FirstOrDefault()).lstPeriodAccess;
            //}
            //else
            //{
            //    return null;
            //}

            if (HttpContext.Current.Session["UserData"] != null)
                return ((UserViewModel)HttpContext.Current.Session["UserData"]).lstPeriodAccess;
            else
                return null;
        }

        public List<MenuModel> GetCLAIM_MENULIST()
        {
            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return JsonConvert.DeserializeObject<UserViewModel>(currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_PERSON_INFORMATION.ToLower()).Select(clm => clm.Value).FirstOrDefault()).lstMenu;
            //}
            //else
            //{
            //    return null;
            //}


            if (HttpContext.Current.Session["UserData"] != null)
                return ((UserViewModel)HttpContext.Current.Session["UserData"]).lstMenu;
            else
                return null;
        }

        public List<RegionInfoViewModel> GETCLAIM_REGIONLIST()
        {

            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return JsonConvert.DeserializeObject<UserViewModel>(currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_PERSON_INFORMATION.ToLower()).Select(clm => clm.Value).FirstOrDefault()).Regions;
            //}
            //else
            //{
            //    return null;
            //}


            if (HttpContext.Current.Session["UserData"] != null)
                return ((UserViewModel)HttpContext.Current.Session["UserData"]).Regions;
            else
                return null;
        }

        public List<RolebasedEDAccessDetailModel> GETCLAIM_ED_PERMISSION()
        {

            //var currentPrincipalClaims = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).Claims;

            ////If there are claims present, then get the type of Company
            //if (currentPrincipalClaims != null && currentPrincipalClaims.Count() > 0)
            //{
            //    return JsonConvert.DeserializeObject<UserViewModel>(currentPrincipalClaims.Where(clm => clm.Type.ToLower() == ClaimTypeName.CLAIM_PERSON_INFORMATION.ToLower()).Select(clm => clm.Value).FirstOrDefault()).lstRolebasedARAccessDetailModel;
            //}
            //else
            //{
            //    return null;
            //}

            if (HttpContext.Current.Session["UserData"] != null)
                return ((UserViewModel)HttpContext.Current.Session["UserData"]).lstRolebasedEDAccessDetailModel;
            else
                return null;
        }

    }
}
