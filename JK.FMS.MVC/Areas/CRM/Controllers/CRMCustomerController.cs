using JKViewModels.Common;

namespace JK.FMS.MVC.Areas.CRM.Controllers
{
    using System.Web.Mvc;
    using Application.Web.Core;
    using System.Linq;
    using System;
    using JKApi.Service;
    using JKApi.Service.ServiceContract.Customer;
    using JKApi.Service.ServiceContract.Franchisee;
    using JKViewModels.Customer;
    using Common;
    using System.Collections.Generic;
    using JKViewModels;
    using JKApi.Data.DAL;
    using System.IO;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using iTextSharp.tool.xml;
    using iTextSharp.tool.xml.pipeline.html;
    using iTextSharp.tool.xml.parser;
    using iTextSharp.tool.xml.pipeline.css;
    using iTextSharp.tool.xml.html;
    using iTextSharp.tool.xml.pipeline.end;
    using JKApi.Service.Service.TaxAPI;

    [OutputCache(Duration = JKApi.Service.Helper.Constants.OutputCacheExpireInSecond)]
    public class CRMCustomerController : ViewControllerBase
    {
        ImportTax _ImpTax = new ImportTax();
        public CRMCustomerController(ICustomerService customerService, ICommonService commonService, IFranchiseeService franchiseeService)
        {
            CustomerService = customerService;
            FranchiseeService = franchiseeService;
            _commonService = commonService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Customer";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMCustomer", new { area = "CRM" }), "Customer");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);
            int TypeList = Convert.ToInt32(JKApi.Business.Enumeration.TypeList.Customer);
            var statuslist = CustomerService.GetStatusList().Where(one => one.TypeListId == TypeList).ToList();
            ViewBag.statusList = new SelectList(statuslist, "StatusListId", "Name", 1);

            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }

        [HttpGet]
        public ActionResult PendingList()
        {
            CustomerSearchResultViewModelListModel model = new CustomerSearchResultViewModelListModel();
            ViewBag.HMenu = "CustomerSales";

            ViewBag.CurrentMenu = "Customer";
            MvcBreadCrumbs.BreadCrumb.Clear();
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMDashboard", new { area = "CRM" }), "CRM");
            MvcBreadCrumbs.BreadCrumb.Add(Url.Action("Index", "CRMCustomer", new { area = "CRM" }), "Customer");

            var regionlist = _commonService.GetRegionList().Where(x => x.RegionId != 0).ToList();
            ViewBag.regionlist = new SelectList(regionlist, "RegionId", "Name", SelectedRegionId);

            ViewBag.selectedRegionId = SelectedRegionId;

            return View();
        }

        [HttpGet]
        public ActionResult CustomerPendingList(string rgId)
        {
            try
            {
                var contactTypeList = Convert.ToInt32(JKApi.Business.Enumeration.ContactTypeList.Main);
                var customers = CustomerService.GetCustomerSearchList(contactTypeList, "38", rgId);
                var result = (from f in customers
                              select new
                              {
                                  f.CustomerId,
                                  f.CustomerNo,
                                  f.CustomerName,
                                  f.Address,
                                  f.StateName,
                                  f.City,
                                  f.PostalCode,
                                  //CustomerName = "CustomerName",
                                  //Address= "Address",
                                  //StateName= "StateName",
                                  //City= "City",
                                  //PostalCode= "PostalCode",
                                  Amount = string.Format("{0:c}", f.Amount),
                                  Phone = f.Phone != null ? CRMUtils.FormatUsPhoneNumber(f.Phone) : string.Empty,
                                  f.RegionName,
                                  StatusName = (f.StatusName ?? "").Trim(),
                                  AcTypeListName = (f.AccountTypeListName ?? "").Trim(),
                                  CreatedBy = f.CreatedBy
                              }).ToList();

                return Json(new
                {
                    aadata = result,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var host = System.Web.HttpContext.Current.Request.Url.Host.ToLower();
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckContractAndContractDetailforDistribution(int id)
        {
            var CustomerContractDetail = CustomerService.HasContractAndContractDetailByCustomerId(id);
            ViewBag.CustomerContractDetail = CustomerContractDetail;

            return Json(CustomerContractDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckUploadDocumentforCustomer(int id, int statusId)
        {
            var retVal = CustomerService.HasDocumentByCustomerId(id, statusId);
            return Json(retVal, JsonRequestBehavior.AllowGet);
        }

        #region 
        [HttpGet]
        public ActionResult CustomerDetailPopup(int id = -1, int status = 0, string callfrom = "", int TotalContractDetailDistributionLines = 0, int DetailLineNeedsFinderFee = 0)
        {
            int? CustomerID = id;
            ViewBag.CustomerID = id;

            List<portal_spGet_C_DistributionWithNoFinderFee_Result> custDistWithNoFF = new List<portal_spGet_C_DistributionWithNoFinderFee_Result>();
            custDistWithNoFF = FranchiseeService.GetCustomerDistributionWithNoFinderFee(id);

            FullCustomerViewModel1 FullCustomerViewModel = new FullCustomerViewModel1();
            CustomerDistributionDetailsModel custDistribution = new CustomerDistributionDetailsModel();
            custDistribution = CustomerService.GetCustomerDistributionDetails(id);
            var customersummaryInfo = CustomerService.GetServiceCallListForSearch(id);
            if (id > 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(CustomerID);
                List<PendingDashboardDataModel> MessageData = new List<PendingDashboardDataModel>();
                MessageData = _commonService.GetDashboardPendingData(null).Where(r => r.CustomerID == id).OrderBy(r => r.MessageDate).ToList<PendingDashboardDataModel>();
                MaintenanceTemp oMaintenanceTemp = jkEntityModel.MaintenanceTemps.Where(o => o.ClassId == CustomerID && o.TypeListId == 1 && o.MaintenanceTypeListId == 7).ToList().FirstOrDefault();

                CustomerDetailViewModel customerDetailViewModel = null;
                foreach (var a in response.ToList())
                {
                    customerDetailViewModel = new CustomerDetailViewModel();

                    customerDetailViewModel.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    customerDetailViewModel.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    customerDetailViewModel.CustomerId = String.IsNullOrEmpty(a.CustomerId.ToString()) ? String.Empty : a.CustomerId.ToString();
                    customerDetailViewModel.Account_Type = String.IsNullOrEmpty(a.AccountType.ToString()) ? String.Empty : a.AccountType.ToString();
                    customerDetailViewModel.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                    customerDetailViewModel.Address2 = String.IsNullOrEmpty(a.Address2) ? String.Empty : a.Address2.ToString();

                    if (a.Phone != null)
                    {
                        customerDetailViewModel.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Phone))
                    //{
                    //    customerDetailViewModel.Phone += customerDetailViewModel.Phone;
                    //}

                    if (a.Fax != null)
                    {
                        customerDetailViewModel.Fax = String.IsNullOrEmpty(a.Fax.ToString()) ? String.Empty : a.Fax.ToString();
                    }
                    //if (!string.IsNullOrEmpty(a.Ext) && !string.IsNullOrEmpty(customerDetailViewModel.Fax))
                    //{
                    //    customerDetailViewModel.Fax += customerDetailViewModel.Phone;
                    //}
                    if (a.EmailAddress != null)
                    {
                        customerDetailViewModel.Email = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.ContactName != null)
                    {
                        customerDetailViewModel.ContactName = String.IsNullOrEmpty(a.ContactName.ToString()) ? String.Empty : a.ContactName.ToString();
                    }
                    if (a.ContactTitle != null)
                    {
                        customerDetailViewModel.Title = String.IsNullOrEmpty(a.ContactTitle.ToString()) ? String.Empty : a.ContactTitle.ToString();
                    }
                    if (a.Cell != null)
                    {
                        customerDetailViewModel.CustomerCell = String.IsNullOrEmpty(a.Cell.ToString()) ? String.Empty : a.Cell.ToString();
                    }
                    if (a.EmailAddress != null)
                    {
                        customerDetailViewModel.CustomerEmail = String.IsNullOrEmpty(a.EmailAddress.ToString()) ? String.Empty : a.EmailAddress.ToString();
                    }
                    if (a.Ext != null)
                    {
                        customerDetailViewModel.PhoneExtension = String.IsNullOrEmpty(a.Ext.ToString()) ? String.Empty : a.Ext.ToString();
                    }
                    // customerDetailViewModel.Amount = Convert.ToDecimal(_Amount.Value.ToString());
                    if (a.Amount != null)
                    {
                        if (a.Amount.ToString().Trim().Length > 0)
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal(a.Amount.ToString());
                        }
                        else
                        {
                            customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                        }
                    }
                    else
                    {
                        customerDetailViewModel.Amount = Convert.ToDecimal("0.00");
                    }
                }
                if (oMaintenanceTemp != null)
                {
                    FullCustomerViewModel.MaintenanceTempId = oMaintenanceTemp.MaintenanceTempId;
                }
                FullCustomerViewModel.CustomerDetail = customerDetailViewModel;
                FullCustomerViewModel.MessagesData = MessageData;
                FullCustomerViewModel.USERID = int.Parse(_claimView.GetCLAIM_USERID());

                var ValidationItem = CustomerService.ValidationItemListStatus(status, (int)JKApi.Business.Enumeration.TypeList.Customer);
                ViewBag.ValidationItem = ValidationItem;

                #region For Region Accounting
                var ValidationItemOld = CustomerService.ValidationItemListStatus(38, (int)JKApi.Business.Enumeration.TypeList.Customer);
                ViewBag.ValidationItemOld = ValidationItemOld;
                var ValidationItemResult = CustomerService.GetValidationByClassId((int)CustomerID, 39);
                var statusNote = CustomerService.GetStatusByClassId((int)CustomerID, 39);
                ViewBag.ValidationItemResult = ValidationItemResult;
                ViewBag.statusNote = statusNote?.StatusNotes;
                #endregion

            }

            ViewBag.HasActiveDistribution = customersummaryInfo.DistributionCount>0?true:false;
            ViewBag.HasActiveContract = (customersummaryInfo.ContractTypeListId != 3 ? (customersummaryInfo.ContractId > 0 ? true : false) : (customersummaryInfo.ContractId > 0 && customersummaryInfo.ContractDetailCount > 0 ? true : false));
            ViewBag.HasActiveCustomerInfo = (customersummaryInfo.CustomerId>0 && customersummaryInfo.BillingAddressCount>0 && customersummaryInfo.MainAddressCount>0 && customersummaryInfo.BillSettingsCount>0)?true:false;
            ViewBag.HasActiveContractType = ViewBag.HasActiveContract==true?customersummaryInfo.ContractTypeListId:0;


            ViewBag.TotalDistribution = customersummaryInfo.DistributionCount;
            ViewBag.Status = status;
            ViewBag.StatusName = CustomerService.GetAll_StatusList().Where(o => o.StatusListId == status).FirstOrDefault()?.Name ?? "";
            ViewBag.CallFrom = callfrom;
            ViewBag.DistributionDetailLineNeedsFF = (custDistWithNoFF.Count > 0 ? custDistWithNoFF[0].DetailLineNumber : 0);

            int CustomerContractId = 0;
            var CustomerContract = CustomerService.GetContractByCustomerId(id);
            if (CustomerContract != null)
            {
                CustomerContractId = CustomerContract.ContractId;
            }
            ViewBag.CustomerContract = CustomerContractId;

            var CustomerContractDetail = CustomerService.HasContractAndContractDetailByCustomerId(id);
            ViewBag.CustomerContractDetail = CustomerContractDetail;

            var CustomerDistributions = CustomerService.GetCustomerDistributionList(id);
            ViewBag.Distributions = CustomerDistributions;
            //var CustomerDistribution = CustomerService.GetCustomerDistribution(id);
            if (CustomerDistributions.Count > 0)
            {
                ViewBag.DistributionFranchiseeId = CustomerDistributions[0].FranchiseeId;
                ViewBag.DistributionId = CustomerDistributions[0].DistributionId;
            }
            else
            {
                ViewBag.DistributionFranchiseeId = 0;
                ViewBag.DistributionId = 0;
            }
            ViewBag.HasDocumentUploaded = CustomerService.HasDocumentByCustomerId(id, status); ;


            var FindersFeeCustomer = CustomerService.GetFindersFeewithCustomerId(id, (CustomerDistributions.Count > 0 ? CustomerDistributions[0].DistributionId : 0));
            if (FindersFeeCustomer != null)
            {
                ViewBag.FindersFeeCustomer = FindersFeeCustomer.FindersFeeId;
            }
            else
            {
                ViewBag.FindersFeeCustomer = 0;
            }



            return PartialView("_CustomerDetailPopup", FullCustomerViewModel);
        }

        public ActionResult CustomerUpdateStatus(int CustomerId, string Note, int Status, string valIds)
        {
            if (CustomerId > 0)
            {
                CustomerService.UpdateStatusToNextStep(CustomerId, Note, Status, valIds);






                if (Status == (int)JKApi.Business.Enumeration.CustomerStatusList.RegionOperation)
                {
                    _commonService.CommonInsertNotification(8, "", false, CustomerId, 1, null, null, null,LoginUserId);
                }
                else if (Status == (int)JKApi.Business.Enumeration.CustomerStatusList.RegionAccounting)
                {
                    _commonService.CommonInsertNotification(9, "", false, CustomerId, 1, null, null, null, LoginUserId);

                    //var GetCustomerInfo = CustomerService.GetCustomerDetailsById(CustomerId);
                    var GetCustomerInfo = jkEntityModel.Customers.Where(x => x.CustomerId == CustomerId).FirstOrDefault();

                    if (GetCustomerInfo != null)
                    {
                        var RegionName = jkEntityModel.Regions.Where(x => x.RegionId == GetCustomerInfo.RegionId).FirstOrDefault();

                        #region Email send function According to Admin configuration

                        //Get Feature Type Id by Feature Name
                        var getNew_Customer_Submit = jkEntityModel.FeatureTypes.Where(x => x.FeatureName == FeatureNameModel.New_Customer_Submit.ToString().Replace("_", " ")).FirstOrDefault();

                        if (getNew_Customer_Submit != null && getNew_Customer_Submit.FeatureTypeId > 0)
                        {
                            //Get Feature Type Email Id by Feature Type Id
                            var messageDetails = jkEntityModel.FeatureTypeEmails.Where(x => x.FeatureTypeId == getNew_Customer_Submit.FeatureTypeId && x.IsEnable == true).FirstOrDefault();
                            if (messageDetails != null && messageDetails.FeatureTypeEmailId > 0)
                            {
                                //Get Feature Email Body By Message Name Or Tempalte Name 
                                var messageBody = jkEntityModel.MailMessageTemplates.Where(x => x.MailMessageTemplateId == messageDetails.MailMessageTemplateId).FirstOrDefault();
                                //MailMessageTemplateModel objItem = _commonService.GetEmailTemplate(MessageNameModel.FranchiseSecion1.ToString());
                                if (messageBody.MailMessageTemplateId > 0)
                                {
                                    string MessageBody = messageBody.MessageBody;
                                    string Subject = messageBody.Subject;

                                    MessageBody = MessageBody.Replace("<<custname>>", !string.IsNullOrWhiteSpace(GetCustomerInfo.Name) ? GetCustomerInfo.Name : GetCustomerInfo.Name2);
                                    MessageBody = MessageBody.Replace("<<regionname>>", RegionName.Name);

                                    _mailService.SendEmailAsyncWithFrom(messageDetails.FromEmail, messageDetails.ToEmailId, MessageBody, Subject);

                                    var CustEmailId = jkEntityModel.Emails.Where(x => x.TypeListId == 1 && x.ClassId == CustomerId && x.ContactTypeListId == 6).FirstOrDefault();

                                    if (messageDetails.EmailToCustomer == true && GetCustomerInfo != null && CustEmailId != null && !string.IsNullOrWhiteSpace(CustEmailId.EmailAddress))
                                    {
                                        _mailService.SendEmailAsync(CustEmailId.EmailAddress, MessageBody, Subject);
                                    }
                                }


                            }
                        }
                        #endregion
                    }
                }
                if (Status == (int)JKApi.Business.Enumeration.CustomerStatusList.RegionAccounting)
                {
                    List<portal_spGet_AP_TaxRateAPI_Result> lstTaxRate = CustomerService.GetTaxrateList(CustomerId, 1);
                    List<Address> lstAddress = CustomerService.GetAddressList(CustomerId, 1);
                    _ImpTax.CallAPIAndImportData(lstAddress, lstTaxRate);
                }

                //CustomerService.savePendingMessage(Note, CustomerId, Status);
            }
            return Json(new { Data = CustomerId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerPushBackToRO(int CustomerId, string Note)
        {
            if (CustomerId > 0)
            {
                CustomerService.UpdateStatus(CustomerId, Note, 39, 38, null);

                _commonService.CommonInsertNotification(10, Note, false, CustomerId, 1, null, null, null, LoginUserId);
            }
            return Json(new { Data = CustomerId, success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerAccountOffringPopup(int? id)
        {
            if (id > 0)
            {
                var response = jkEntityModel.portal_spGet_CustomerDetail(Convert.ToInt32(id));
                foreach (var a in response.ToList())
                {
                    ViewBag.CustomerId = a.CustomerId;
                    ViewBag.CustomerName = String.IsNullOrEmpty(a.CustomerName.ToString()) ? String.Empty : a.CustomerName.ToString();
                    ViewBag.CustomerNo = String.IsNullOrEmpty(a.CustomerNo.ToString()) ? String.Empty : a.CustomerNo.ToString();
                    ViewBag.Address = String.IsNullOrEmpty(a.MainAddress) ? null : a.MainAddress.ToString();
                    ViewBag.Address2 = String.IsNullOrEmpty(Convert.ToString(a.Address2)) ? String.Empty : a.Address2.ToString();

                    if (a.Phone != null)
                    {
                        ViewBag.Phone = String.IsNullOrEmpty(a.Phone.ToString()) ? String.Empty : a.Phone.ToString();
                    }
                }


            }
            ViewBag.FranchiseeTypeList = new SelectList(CustomerService.GetAll_FranchiseeTypeList(), "FranchiseeTypeListId", "Name");
            return PartialView("_CustomerAccountOffringPopup");
        }
        #endregion

        #region :: New Account Form ::

        public ActionResult NewAccountForm(int CustomerId)
        {
            List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();

            var dataModel = CustomerService.GetCustomerFranchiseeForNewAccountForm(CustomerId);
            if (dataModel != null)
            {
                lstCustomerFranchiseeDistribution = dataModel;
            }
            ViewBag.CustomerId = CustomerId;
             
            var _formdata = CustomerService.GetCSAccountWalkThursFormFieldDetailWithCustomer(CustomerId);
            ViewBag.FormFieldDetail = _formdata;

            return PartialView("_NewAccount", lstCustomerFranchiseeDistribution);
        }
        public FileResult NewAccountFormExport(int CustomerId)
        {
            if (CustomerId > 0)
            {
                List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();
                string HTMLContent = string.Empty;

                var dataModel = CustomerService.GetCustomerFranchiseeForNewAccountForm(CustomerId);
                if (dataModel != null && dataModel.Count > 0)
                {
                    var _formdata = CustomerService.GetCSAccountWalkThursFormFieldDetailWithCustomer(CustomerId);
                    ViewBag.FormFieldDetail = _formdata;

                    foreach (var item in dataModel)
                    {
                        HTMLContent += RenderPartialViewToString("_NewAccountFormExportPDF", item);
                    }
                }
                ViewBag.CustomerId = CustomerId;
                return File(GetPDF(HTMLContent), "application/pdf", "_NewAccountFormExportToPDF.pdf");
            }
            return null;
        }
        public JsonResult NewAccountFormPrint(int CustomerId)
        {
            if (CustomerId > 0)
            {
                List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();
                string HTMLContent = string.Empty;
                var dataModel = CustomerService.GetCustomerFranchiseeForNewAccountForm(CustomerId);
                if (dataModel != null && dataModel.Count > 0)
                {
                    lstCustomerFranchiseeDistribution = dataModel;

                    var _formdata = CustomerService.GetCSAccountWalkThursFormFieldDetailWithCustomer(CustomerId);
                    ViewBag.FormFieldDetail = _formdata;

                    foreach (var item in dataModel)
                    {
                        HTMLContent += RenderPartialViewToString("_NewAccountFormExportPDF", item);
                    }
                }
                //HTMLContent += RenderPartialViewToString("_NewAccountFormExportPDF", lstCustomerFranchiseeDistribution);
                var retPath = "/Upload/InvoiceFiles/" + "tmp_" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), "tmp_" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");
                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO
                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult SaveNewAccountForm(int CustomerId, int FranchiseeId=0,string arrdata="")
        {
            if (CustomerId > 0)
            {
                List<NewAccountFormFieldModel> _list = new List<NewAccountFormFieldModel>();
                if (arrdata != "")
                {
                    _list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NewAccountFormFieldModel>>(arrdata);
                    if (_list.Count() > 0)
                    {
                        CustomerService.SaveCSAccountWalkThursFormFieldDetail(CustomerId, FranchiseeId, _list);
                    }
                }

                List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();
                string HTMLContent = string.Empty;
                var dataModel = CustomerService.GetCustomerFranchiseeForNewAccountForm(CustomerId);
                if (dataModel != null && dataModel.Count > 0)
                {
                    lstCustomerFranchiseeDistribution = dataModel;


                    var _formdata = CustomerService.GetCSAccountWalkThursFormFieldDetailWithCustomer(CustomerId);
                    ViewBag.FormFieldDetail = _formdata;

                    foreach (var item in dataModel)
                    {
                        HTMLContent += RenderPartialViewToString("_NewAccountFormExportPDF", item);
                    }
                }
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument")))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument"));
                }
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString()));
                }
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), CustomerId.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), CustomerId.ToString()));
                }
                var retPath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + CustomerId.ToString() + "/" + CustomerId + "_NewAccountForm.pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + CustomerId.ToString() + "/"), CustomerId + "_NewAccountForm.pdf");
                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO

                //save upload document
                CustomerService.SaveUploadDocument(int.Parse(CustomerId.ToString()), 1, (int)JKApi.Business.Enumeration.FileTypeList.AccountWalkThurs, retPath, "Account Walk Thurs", "pdf", 0, false);
                

                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            ViewBag.DomainUrl = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Authority + "/";

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public byte[] GetPDF(string pHTML)
        {


            byte[] bytesArray = null;
            using (var ms = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4, 25, 25, 25, 25))
                {
                    document.SetPageSize(iTextSharp.text.PageSize.A4);
                    using (PdfWriter writer = PdfWriter.GetInstance(document, ms))
                    {
                        document.Open();
                        using (var strReader = new StringReader(pHTML))
                        {
                            //Set factories
                            HtmlPipelineContext htmlContext = new HtmlPipelineContext(null);
                            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                            //Set css
                            ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
                            //cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath("~/Content/bootstrap.min.css"), true);
                            cssResolver.AddCssFile(Server.MapPath("~/Content/newaccountform_pdf.css"), true);
                            //Export
                            IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
                            var worker = new XMLWorker(pipeline, true);
                            var xmlParse = new XMLParser(true, worker);
                            xmlParse.Parse(strReader);
                            xmlParse.Flush();
                        }
                        document.Close();
                    }
                }
                bytesArray = ms.ToArray();
            }
            return bytesArray;



        }

        #endregion

        #region :: Account Acceptance :: 
        public ActionResult AccountAcceptanceForm(int CustomerId)
        {
            //if (CustomerId > 0)
            //{
            //    List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();

            //    var dataModel = CustomerService.GetCustomerFranchiseeForNewAccountForm(CustomerId);
            //    if (dataModel != null)
            //    {
            //        lstCustomerFranchiseeDistribution = dataModel;
            //    }
            //    ViewBag.CustomerId = CustomerId;
            //    return PartialView("_AccountAcceptanceForm", lstCustomerFranchiseeDistribution);
            //}
            //return null;  

            //ViewBag.CustomerId = CustomerId;
            //return PartialView("_AccountAcceptanceForm");

            ViewBag.CustomerId = CustomerId;
            return PartialView("_AccountAcceptanceForm", CustomerService.GetAccountAcceptanceInfoOffer(CustomerId));
        }
        public FileResult AccountAcceptanceExport(int CustomerId)
        {
            if (CustomerId > 0)
            {
                List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();
                string HTMLContent = string.Empty;

                var dataModel = CustomerService.GetCustomerFranchiseeForNewAccountForm(CustomerId);
                if (dataModel != null && dataModel.Count > 0)
                {
                    foreach (var item in dataModel)
                    {
                        HTMLContent += RenderPartialViewToString("_AccountAcceptanceExportPDF", item);
                    }
                }
                ViewBag.CustomerId = CustomerId;
                return File(GetPDF(HTMLContent), "application/pdf", "_AccountAcceptanceExportToPDF.pdf");
            }
            return null;

            //if (CustomerId > 0)
            //{
            //    string HTMLContent = string.Empty;
            //    HTMLContent += RenderPartialViewToString("_AccountAcceptanceExportPDF", null);
            //    return File(GetPDF(HTMLContent), "application/pdf", "_NewAccountFormExportToPDF.pdf");
            //}
            //return null;
        }
        public JsonResult AccountAcceptancePrint(int CustomerId)
        {
            if (CustomerId > 0)
            {
                List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();
                string HTMLContent = string.Empty;
                var dataModel = CustomerService.GetCustomerFranchiseeForNewAccountForm(CustomerId);
                if (dataModel != null && dataModel.Count > 0)
                {
                    lstCustomerFranchiseeDistribution = dataModel;
                    foreach (var item in dataModel)
                    {
                        HTMLContent += RenderPartialViewToString("_AccountAcceptanceExportPDF", item);
                    }
                }
                //HTMLContent += RenderPartialViewToString("_NewAccountFormExportPDF", lstCustomerFranchiseeDistribution);
                var retPath = "/Upload/InvoiceFiles/" + "tmp_aac" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), "tmp_aac" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");
                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO
                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
            //if (CustomerId > 0)
            //{
            //    string HTMLContent = string.Empty;
            //    HTMLContent += RenderPartialViewToString("_AccountAcceptanceExportPDF", null);
            //    var retPath = "/Upload/InvoiceFiles/" + "tmp_" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf";
            //    var path = Path.Combine(Server.MapPath("~/Upload/InvoiceFiles/"), "tmp_" + DateTime.Now.ToString("MMddyyyyHHmmsstt") + ".pdf");
            //    System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO
            //    return Json(retPath, JsonRequestBehavior.AllowGet);
            //}
            //return null;
        }
        public JsonResult SaveAccountAcceptanceForm(int CustomerId)
        {
            if (CustomerId > 0)
            {
                List<CustomerFranchiseeDistribution> lstCustomerFranchiseeDistribution = new List<CustomerFranchiseeDistribution>();
                string HTMLContent = string.Empty;
                var dataModel = CustomerService.GetCustomerFranchiseeForNewAccountForm(CustomerId);
                if (dataModel != null && dataModel.Count > 0)
                {
                    lstCustomerFranchiseeDistribution = dataModel;
                    foreach (var item in dataModel)
                    {
                        HTMLContent += RenderPartialViewToString("_NewAccountFormExportPDF", item);
                    }
                }
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument")))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument"));
                }
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString()));
                }
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), CustomerId.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Upload"), "CustomerDocument", SelectedRegionId.ToString(), CustomerId.ToString()));
                }
                var retPath = "~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + CustomerId.ToString() + "/" + CustomerId + "_AccountAcceptanceForm.pdf";
                var path = Path.Combine(Server.MapPath("~/Upload/CustomerDocument/" + SelectedRegionId.ToString() + "/" + CustomerId.ToString() + "/"), CustomerId + "_AccountAcceptanceForm.pdf");
                System.IO.File.WriteAllBytes(path, GetPDF(HTMLContent)); // Requires System.IO
                return Json(retPath, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        #endregion 

    }
}