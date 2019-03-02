using Application.Web.Core;
using JKApi.Data.DAL;
using JKApi.Service.ServiceContract.CRM;
using JKViewModels.CRM;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    public class CRMContactsController : ViewControllerBase
    {
        public CRMContactsController(ICRM_Service crmService)
        {
            _crmService = crmService;
        }

        // GET: CRM/CRMContact
        public ActionResult Index()
        {

            return View();
        }
        

        // GET: CRM/CRMContact/GetContacts
        public JsonResult GetContacts(int ContactTypeId)
        {
        
            CRMContact cnt = new CRMContact();
            CRMNewContactViewModel cRMNewContactViewModel = new CRMNewContactViewModel();
            try
            {
                jkDatabaseEntities jk = new jkDatabaseEntities();
                cnt = jk.CRMContacts.Find(ContactTypeId);
                
                cRMNewContactViewModel.BusinessAddress = cnt.BusinessAddress;
                cRMNewContactViewModel.BusinessFaxPhone = cnt.BusinessFaxPhone;
                cRMNewContactViewModel.BusinessPhone = cnt.BusinessPhone;
                cRMNewContactViewModel.Company = cnt.Company;
                cRMNewContactViewModel.ContactTypeId = cnt.CRMContactTypeId;
                cRMNewContactViewModel.DisplayAs = cnt.DisplayAs;
                cRMNewContactViewModel.Email = cnt.Email;
                cRMNewContactViewModel.FileAs = cnt.FileAs;
                cRMNewContactViewModel.FullName = cnt.FullName;
                cRMNewContactViewModel.HomePhone = cnt.HomePhone;
                cRMNewContactViewModel.IMAddress = cnt.IMAddress;
                cRMNewContactViewModel.IsMailingAddress = cnt.IsMailingAddress;
                cRMNewContactViewModel.Jobtitle = cnt.Jobtitle;
                cRMNewContactViewModel.MobilePhone = cnt.MobilePhone;
                cRMNewContactViewModel.WebPageAddress = cnt.WebPageAddress;
                //var contactTypes = _crmService.
            }
            catch (Exception ex)
            {
                string fault = ex.Message;
              
            }

            return Json(cRMNewContactViewModel, JsonRequestBehavior.AllowGet);
        }

        // POST: CRM/CRMContact/Create
        [HttpPost]
        public JsonResult Create(CRMNewContactViewModel model)
        {
            var success = false;
            var message = "Please Try Again!!!";

            try
            {
                jkDatabaseEntities jk = new jkDatabaseEntities();
                CRMContact cRMContact=null;
                if (model.ContactID == null)
                {
                  cRMContact = new CRMContact();
                }
                else
                {
                    cRMContact = jk.CRMContacts.Find(model.ContactID);
                }
                 
                cRMContact.CRMContactTypeId = model.ContactTypeId;
                cRMContact.FullName = model.FullName;
                cRMContact.Company = model.Company;
                cRMContact.Jobtitle = model.Jobtitle;
                cRMContact.FileAs = model.FileAs;
                cRMContact.Email = model.Email;
                cRMContact.DisplayAs = model.DisplayAs;
                cRMContact.WebPageAddress = model.WebPageAddress;
                cRMContact.IMAddress = model.IMAddress;
                cRMContact.BusinessPhone = model.BusinessPhone;
                cRMContact.HomePhone = model.HomePhone;
                cRMContact.BusinessFaxPhone = model.BusinessFaxPhone;
                cRMContact.MobilePhone = model.MobilePhone;
                cRMContact.IsMailingAddress = model.IsMailingAddress;
                cRMContact.BusinessAddress = model.BusinessAddress;
                cRMContact.IsActive = true;
                cRMContact.CreatedBy = int.Parse(_claimView.GetCLAIM_USERID());
                cRMContact.CreatedOn = DateTime.Now;
                cRMContact.UserId= int.Parse(_claimView.GetCLAIM_USERID());
                cRMContact.RegionId = SelectedRegionId;
                if (model.ContactID == null)
                    jk.CRMContacts.Add(cRMContact);
                jk.SaveChangesAsync();
                success = true;
                message = "Success";

            }
            catch (Exception ex)
            {
                string fault = ex.Message;
                success = false;
                message = "Please Try Again!!!";
            }

            return Json(new { success, message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ContactNewForm(int? id)
        {
            if(id == null)
            {
                return PartialView("_ContactForm", new CRMNewContactViewModel());
            }
            else
            {
                CRMContact cnt = new CRMContact();
                CRMNewContactViewModel cRMNewContactViewModel = new CRMNewContactViewModel();
                try
                {
                    jkDatabaseEntities jk = new jkDatabaseEntities();
                    cnt = jk.CRMContacts.Find(id);

                    cRMNewContactViewModel.BusinessAddress = cnt.BusinessAddress;
                    cRMNewContactViewModel.BusinessFaxPhone = cnt.BusinessFaxPhone;
                    cRMNewContactViewModel.BusinessPhone = cnt.BusinessPhone;
                    cRMNewContactViewModel.Company = cnt.Company;
                    cRMNewContactViewModel.ContactTypeId = cnt.CRMContactTypeId;
                    cRMNewContactViewModel.DisplayAs = cnt.DisplayAs;
                    cRMNewContactViewModel.Email = cnt.Email;
                    cRMNewContactViewModel.FileAs = cnt.FileAs;
                    cRMNewContactViewModel.FullName = cnt.FullName;
                    cRMNewContactViewModel.HomePhone = cnt.HomePhone;
                    cRMNewContactViewModel.IMAddress = cnt.IMAddress;
                    cRMNewContactViewModel.IsMailingAddress = cnt.IsMailingAddress;
                    cRMNewContactViewModel.Jobtitle = cnt.Jobtitle;
                    cRMNewContactViewModel.MobilePhone = cnt.MobilePhone;
                    cRMNewContactViewModel.WebPageAddress = cnt.WebPageAddress;
                    cRMNewContactViewModel.ContactID = cnt.CRMContactId;
                    //var contactTypes = _crmService.
                }
                catch (Exception ex)
                {
                    string fault = ex.Message;

                }
                return PartialView("_ContactForm", cRMNewContactViewModel);

            }
        }
    }


}