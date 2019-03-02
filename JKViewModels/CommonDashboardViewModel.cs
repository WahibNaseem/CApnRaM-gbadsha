using JKViewModels.CRM;
using JKViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKViewModels
{

    public class CommonDashboardViewModel
    {
        public CommonDashboardViewModel()
        {
            DashboardModel = new DashboardModel();
        }
        public DashboardModel DashboardModel { get; set; }
        public List<DashboardModel> BillingBreakdownBySizeChartData { get; set; }
        public List<DashboardModel> DashboardAccountTypeWiseChartData { get; set; }
        public List<DashboardModel> RevenueWiseTopCustomerChartData { get; set; }
        
    }

    public class CommonManagementDashboardViewModel
    {
        public CommonManagementDashboardViewModel()
        {
            DashboardModel = new DashboardModel();
        }
        public DashboardModel DashboardModel { get; set; }
        public List<DashboardModel> BillingAccountBreakdownBySize { get; set; }
        public List<DashboardModel> TopRevenuedAccountType { get; set; }
        public List<DashboardModel> TopRevenuedCustomers { get; set; }
        public List<DashboardModel> MonthlyRevenues { get; set; }
        public List<DashboardModel> BillingAccountBreakdownByContract { get; set; }

    }

    public class DynamicReportViewModel
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ProcedureName { get; set; }
        public bool IsActive { get; set; }

    }

    public class DynamicReportColumnListViewModel
    {
        public int ReportId { get; set; }
        public string ColumName { get; set; }
        public string ColumNameDB { get; set; }
        public bool IsActive { get; set; }

    }


}
