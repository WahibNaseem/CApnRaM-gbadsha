using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using JKViewModels.Job;
using JKViewModels.Common;

namespace JKApi.Service.Service.Job
{
    public interface IJobService
    {
        List<JobModel> GetJobList();
        List<JobModel> GetJobListByRegionId(int regionId);
        List<JobModel> GetJobListByFranchiseeId(int franchiseeId);
        List<JobModel> GetJobListByCustomerId(int customerId);
        JobModel GetJob(int jobId);
        JobModel AddOrUpdateJob(JobModel jobModel);
        JobModel UpdateJob(JobModel jobModel);
    }

    public class JobService : BaseService, IJobService
    {
        public List<JobModel> GetJobList()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetJobList, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<JobModel, AddressModel, JobModel>((job, address) =>
                {
                    job.Address = address;
                    return job;
                }, "AddressId").ToList();
            }
        }

        public List<JobModel> GetJobListByRegionId(int regionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RegionId", regionId);
            parameters.Add("@IsEnable", true);
            parameters.Add("@SortColumn", "CustomerName");

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetJobListByRegion, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<JobModel, AddressModel, JobModel>((job, address) =>
                {
                    job.Address = address;
                    return job;
                }, "AddressId").ToList();
            }
        }

        public List<JobModel> GetJobListByFranchiseeId(int franchiseeId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FranchiseeId", franchiseeId);
            parameters.Add("@IsEnable", true);
            parameters.Add("@SortColumn", "CustomerName");

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetJobListByFranchisee, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<JobModel, AddressModel, JobModel>((job, address) =>
                {
                    job.Address = address;
                    return job;
                }, "AddressId").ToList();
            }
        }

        public List<JobModel> GetJobListByCustomerId(int customerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerId", customerId);
            parameters.Add("@IsEnable", true);
            parameters.Add("@SortColumn", "CustomerName");

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetJobListByCustomer, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<JobModel, AddressModel, JobModel>((job, address) =>
                {
                    job.Address = address;
                    return job;
                }, "AddressId").ToList();
            }
        }

        public JobModel GetJob(int jobId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@JobId", jobId);
            parameters.Add("@IsEnable", true);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_GetJob, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var jobs = multipleResult?.Read<JobModel, AddressModel, JobModel>((job, address) =>
                {
                    job.Address = address;
                    return job;
                }, "AddressId");
                return jobs.Any() ? jobs.First() : null;
            }
        }

        public JobModel AddOrUpdateJob(JobModel jobModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DistributionId", jobModel.DistributionId);
            parameters.Add("@JobId", jobModel.JobId);
            parameters.Add("@RegionId", jobModel.RegionId);
            parameters.Add("@FranchiseeId", jobModel.FranchiseeId);
            parameters.Add("@ContractId", jobModel.ContractId);
            parameters.Add("@ContractDetailId", jobModel.ContractDetailId);
            parameters.Add("@AddressId", jobModel.AddressId);
            parameters.Add("@CustomerId", jobModel.CustomerId);
            parameters.Add("@SoldById", jobModel.SoldById);
            parameters.Add("@ContractTypeListId", jobModel.ContractTypeListId);
            parameters.Add("@AccountTypeListId", jobModel.AccountTypeListId);
            parameters.Add("@ServiceTypeListId", jobModel.ServiceTypeListId);
            parameters.Add("@PurchaseOrderNumber", jobModel.PurchaseOrderNumber);
            parameters.Add("@CustomerName", jobModel.CustomerName);
            parameters.Add("@PrimaryContact", jobModel.PrimaryContact);
            parameters.Add("@PrimaryContactPhone", jobModel.PrimaryContactPhone);
            parameters.Add("@PrimaryContactPhoneExt", jobModel.PrimaryContactPhoneExt);
            parameters.Add("@StartDate", jobModel.StartDate);
            parameters.Add("@ExpirationDate", jobModel.ExpirationDate);
            parameters.Add("@ContractDescription", jobModel.ContractDescription);
            parameters.Add("@Amount", jobModel.Amount);
            parameters.Add("@SquareFootage", jobModel.SquareFootage);
            parameters.Add("@CleanTimes", jobModel.CleanTimes);
            parameters.Add("@Mon", jobModel.Mon);
            parameters.Add("@Tue", jobModel.Tue);
            parameters.Add("@Wed", jobModel.Wed);
            parameters.Add("@Thu", jobModel.Thu);
            parameters.Add("@Fri", jobModel.Fri);
            parameters.Add("@Sat", jobModel.Sat);
            parameters.Add("@Sun", jobModel.Sun);
            parameters.Add("@StartTime", jobModel.StartTime);
            parameters.Add("@EndTime", jobModel.EndTime);
            parameters.Add("@AssignedTeamId", jobModel.AssignedTeamId);
            parameters.Add("@AssignedEmployeeId", jobModel.AssignedEmployeeId);
            parameters.Add("@AssignedTemplate", jobModel.AssignedTemplate);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_AddOrUpdateJob, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<JobModel, AddressModel, JobModel>((job, address) =>
                {
                    job.Address = address;
                    return job;
                }, "AddressId").First();
            }
        }

        public JobModel UpdateJob(JobModel jobModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DistributionId", jobModel.DistributionId);
            parameters.Add("@JobId", jobModel.JobId);
            parameters.Add("@RegionId", jobModel.RegionId);
            parameters.Add("@FranchiseeId", jobModel.FranchiseeId);
            parameters.Add("@ContractId", jobModel.ContractId);
            parameters.Add("@ContractDetailId", jobModel.ContractDetailId);
            parameters.Add("@AddressId", jobModel.AddressId);
            parameters.Add("@CustomerId", jobModel.CustomerId);
            parameters.Add("@SoldById", jobModel.SoldById);
            parameters.Add("@ContractTypeListId", jobModel.ContractTypeListId);
            parameters.Add("@AccountTypeListId", jobModel.AccountTypeListId);
            parameters.Add("@ServiceTypeListId", jobModel.ServiceTypeListId);
            parameters.Add("@PurchaseOrderNumber", jobModel.PurchaseOrderNumber);
            parameters.Add("@CustomerName", jobModel.CustomerName);
            parameters.Add("@PrimaryContact", jobModel.PrimaryContact);
            parameters.Add("@PrimaryContactPhone", jobModel.PrimaryContactPhone);
            parameters.Add("@PrimaryContactPhoneExt", jobModel.PrimaryContactPhoneExt);
            parameters.Add("@StartDate", jobModel.StartDate);
            parameters.Add("@ExpirationDate", jobModel.ExpirationDate);
            parameters.Add("@ContractDescription", jobModel.ContractDescription);
            parameters.Add("@Amount", jobModel.Amount);
            parameters.Add("@SquareFootage", jobModel.SquareFootage);
            parameters.Add("@CleanTimes", jobModel.CleanTimes);
            parameters.Add("@Mon", jobModel.Mon);
            parameters.Add("@Tue", jobModel.Tue);
            parameters.Add("@Wed", jobModel.Wed);
            parameters.Add("@Thu", jobModel.Thu);
            parameters.Add("@Fri", jobModel.Fri);
            parameters.Add("@Sat", jobModel.Sat);
            parameters.Add("@Sun", jobModel.Sun);
            parameters.Add("@StartTime", jobModel.StartTime);
            parameters.Add("@EndTime", jobModel.EndTime);
            parameters.Add("@AssignedTeamId", jobModel.AssignedTeamId);
            parameters.Add("@AssignedEmployeeId", jobModel.AssignedEmployeeId);
            parameters.Add("@AssignedTemplate", jobModel.AssignedTemplate);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.sp_UpdateJob, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return multipleResult?.Read<JobModel, AddressModel, JobModel>((job, address) =>
                {
                    job.Address = address;
                    return job;
                }, "AddressId").First();
            }
        }
    }
}
