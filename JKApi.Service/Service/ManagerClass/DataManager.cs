using System;
using System.Collections;
using System.IO;
using System.Net;
using FluentScheduler;
using JK.Repository.Helpers;
using JK.Repository.RepositoryFactory;
using JK.Repository.Uow;
using JKApi.Core;
using JKApi.Data.DAL;
using JKApi.Service.Service.CRM;
using JKApi.Service.Service.Customer;
using JKApi.Service.Service.FOM;
using JKApi.Service.Service.Inspection;
using JKApi.Service.Service.Job;
using JKApi.Service.ServiceContract.CRM;
using JKApi.Service.ServiceContract.Customer;
using JKApi.Service.ServiceContract.Inspection;
using JKViewModels.Common;
using JKViewModels.Inspection;
using JKViewModels.Job;
using Newtonsoft.Json;

namespace JKApi.Service.Service.ManagerClass
{
    public class DataManager
    {
        private static readonly Lazy<DataManager> Lazy = new Lazy<DataManager>(() => new DataManager());
        public static DataManager Instance => Lazy.Value;

        private readonly IJKEfUow _uow;
        private readonly ICacheProvider _cacheProvider;
        private readonly ICommonService _commonService;
        private readonly IContractService _contractService;
        private readonly IJobService _jobService;
        private readonly IInspectionService _inspectionService;
        private readonly ITemplateService _templateService;
        private readonly ICustomerService _customerService;
        private readonly ICRM_Service _crmService;
        protected NLogger NLogger = NLogger.Instance;

        private DataManager()
        {
            _uow = new JKEfUow(new RepositoryProvider(new RepositoryFactories())
            {
                DbContext = new jkDatabaseEntities()
            });
            _cacheProvider = new CacheProvider();
            _commonService = new CommonService();
            _contractService = new ContractService(_uow, _cacheProvider);
            _jobService = new JobService();
            _inspectionService = new InspectionService(_uow, _cacheProvider);
            _templateService = new TemplateService(_inspectionService);
            _customerService = new CustomerService();
            _crmService = new CRM_Service(_uow, _cacheProvider);
        }

        #region Private

        private RootObjectlatlngViewModel _getCooridatesFromAddress(string address)
        {
            var root = new RootObjectlatlngViewModel();
            try
            {
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key=AIzaSyCrw533WZZijl0sItzXs-DjfyHMN5g4xD8";
                var req = (HttpWebRequest)WebRequest.Create(url);
                var res = (HttpWebResponse)req.GetResponse();

                using (var streamreader = new StreamReader(res.GetResponseStream()))
                {
                    var result = streamreader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        root = JsonConvert.DeserializeObject<RootObjectlatlngViewModel>(result);
                    }
                }
            }
            catch (Exception exception)
            {
                NLogger.Error($"Exception: {exception.Message}");
            }
            return root;
        }
        #endregion

        #region Public

        public void StartRepeatingProcess()
        {
            JobManager.AddJob(GenerateJobAndInspection, (job) => job.ToRunNow());
            JobManager.AddJob(UpdateCustomerAddressCoordinate, (job) => job.ToRunNow());
            JobManager.AddJob(UpdateLeadAddressCoordinate, (job) => job.ToRunNow());
        }

        public void StopRepeatingProcess()
        {
            JobManager.Stop();
        }

        public void GenerateJobAndInspection()
        {
            try
            {
                var jobList = _jobService.GetJobList();
                var contracts = _contractService.GetAllContracts();
                if (jobList.Count == 0)
                {
                    NLogger.Debug(">>>>> START JOB IMPORTING TASK");
                    
                    foreach (var contract in contracts)
                    {
                        if (contract.RegionId == 2 || contract.RegionId == 14 || contract.RegionId == 24)
                        {
                            var job = ConvertModelToDto<JobModel>(contract);
                            if (job != null)
                            {
                                job.AddressId = contract.Address?.AddressId ?? 0;
                                job.Address = contract.Address;
                                _jobService.AddOrUpdateJob(job);
                            }
                        }  
                    }
                    jobList = _jobService.GetJobList();
                }

                var inspectionForms = _inspectionService.GetInspectionFormList();
                if (inspectionForms.Count == 0)
                {
                    NLogger.Debug(">>>>> START INSPECTION IMPORTING TASK");
                    foreach (var job in jobList)
                    {
                        NLogger.Debug($"Create inspection for jobId={job.JobId}");
                        var accountType = job.AccountTypeListId;
                        var template = _templateService.GetTemplateByAccountType(accountType);
                        if (template == null) template = _templateService.GetTemplateByAccountType(144); // Generic template
                        if (template != null)
                        {
                            _templateService.AssignInspectionFromTemplate(template, job);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                NLogger.Error($"Exception: {exception.Message}");
            }
        }

        public void UpdateCustomerAddressCoordinate()
        {
            try
            {
                var customers = _customerService.GetCustomerList();
                foreach (var customer in customers)
                {
                    var address = customer.Address;
                    if (address == null || (address.Latitude != 0 && address.Longitude != 0)) continue;
                    var coordinate = _getCooridatesFromAddress(address.FormattedFullAddress);
                    if (coordinate.results.Count > 0)
                    {
                        var latitude = Convert.ToDecimal(coordinate.results[0].geometry.location.lat);
                        var longitude = Convert.ToDecimal(coordinate.results[0].geometry.location.lng);
                        _commonService.UpdateGMapAddress(address.AddressId, latitude, longitude);
                    }
                }
            }
            catch (Exception exception)
            {
                NLogger.Error($"Exception: {exception.Message}");
            }
        }

        public void UpdateLeadAddressCoordinate()
        {
            try
            {
                var leads = _crmService.GetAll_CRM_AccountCustomerDetail();
                foreach (var lead in leads)
                {
                    var address1 = lead.CompanyAddressLine1;
                    var address2 = lead.CompanyAddressLine2;
                    var city = lead.CompanyCity;
                    var state = lead.CompanyState;
                    var zipCode = lead.CompanyZipCode;

                    if (address1 != null && city != null && state != null && zipCode != null)
                    {
                        var addressLine = string.IsNullOrEmpty(address2) ? "{0}" : "{0} {1}";
                        var fullAddress = string.Format(addressLine + ", {2}, {3} {4}", address1, address2, city, state, zipCode);
                        var coordinate = _getCooridatesFromAddress(fullAddress);
                        if (coordinate.results.Count > 0)
                        {
                            lead.CompanyLatitude = Convert.ToDecimal(coordinate.results[0].geometry.location.lat);
                            lead.CompanyLongitude = Convert.ToDecimal(coordinate.results[0].geometry.location.lng);
                            _crmService.UpdateCRM_AccountCustomerDetail_Coordinate(lead);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                NLogger.Error($"Exception: {exception.Message}");
            }
        }

        protected T ConvertModelToDto<T>(object model) where T : new()
        {
            if (model == null) return default(T);

            var propertyName = string.Empty;
            var dto = new T();
            var dtoProperties = dto.GetType().GetProperties();
            foreach (var dtoProperty in dtoProperties)
            {
                var modelProperty = model.GetType().GetProperty(dtoProperty.Name);
                if (modelProperty != null)
                {
                    // We do not process nesting
                    if (modelProperty.PropertyType.FullName != null &&
                        (modelProperty.PropertyType.IsClass && !modelProperty.PropertyType.FullName.StartsWith("System.")))
                    {
                        continue;
                    }

                    var value = modelProperty.GetValue(model, null);
                    if (!(value is IList) || !value.GetType().IsGenericType)
                    {
                        dtoProperty.SetValue(dto, value, null);
                    }
                }
            }
            return dto;
        }

        #endregion
    }
}
