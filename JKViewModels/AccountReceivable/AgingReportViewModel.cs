using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace JKViewModels.AccountReceivable
{
    public class AgingReportViewModel
    {
        public AgingReportViewModel()
        {
            this.RegionIdsViewModel = new HashSet<RegionIdsViewModel>();
        }
        #region Search Filter....
        public DateTime? agingDate { get; set; }
        public DateTime? paymentDateFrom { get; set; }
        public DateTime? paymentDateTo { get; set; }
        public int? monthsToInclude { get; set; }
        public int? orderByList { get; set; }
        public int? includeList { get; set; }
        public int? balanceList { get; set; }
        public bool? isNonChargebackOnly { get; set; }
        public int? regionId { get; set; }
        public bool? isSummaryView { get; set; }

        public string IsMonthView { get; set; }
        #endregion Search Filter....

        #region Data ....
        [DisplayName("Id")]
        public string id { get; set; }
        [DisplayName("Franchise Id")]
        public string franchiseId { get; set; }
        [DisplayName("Franchise No")]
        public string franchiseNo { get; set; }
        [DisplayName("Franchise Name")]
        public string franchiseName { get; set; }
        [DisplayName("Customer Id")]
        public string customerId { get; set; }
        [DisplayName("Customer No")]
        public string customerNo { get; set; }
        [DisplayName("Customer Name")]
        public string customerName { get; set; }
        [DisplayName("Phone")]
        [DisplayFormat(DataFormatString = "{0:(###) ###-####}", ApplyFormatInEditMode = true)]
        public string phone { get; set; }
        [DisplayName("Inv Date")]
        //[DataType(DataType.Date)]
        public string invDate { get; set; }
        [DisplayName("Inv Number")]
        public string invNumber { get; set; }
        [DisplayName("Due Date")]
        public string dueDate { get; set; }
        [DisplayName("Invoice Total")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string totalAmount { get; set; }
        [DisplayName("Original Total")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string originalTotal { get; set; }
        [DisplayName("Current")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string onemo { get; set; }
        [DisplayName("1-30")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string twomo { get; set; }
        [DisplayName("31-60")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string threemo { get; set; }
        [DisplayName("61-90")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string fourmo { get; set; }
        [DisplayName("91+")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string fivemo { get; set; }
        public string regionIds { get; set; }

        public string allTotal { get; set; }

        public virtual IEnumerable<RegionIdsViewModel> RegionIdsViewModel { get; set; }

        [DisplayName("Total")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public string sixmo { get; set; }

        // Property added by Ajay Prakash
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string RegionAcronym { get; set; }

        public DateTime? ReportDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        #endregion

        public static AgingReportViewModel ConvertToModel(DataRow row)
        {
            var model = new AgingReportViewModel();
            model.id = Convert.ToString(row["id"]);
            //model.franchiseId = Convert.ToString(row["franchiseId"]);
            //model.franchiseNo = Convert.ToString(row["franchiseNo"]);
            //model.franchiseName = Convert.ToString(row["franchiseName"]);
            model.customerId = Convert.ToString(row["customerId"]);
            model.customerNo = Convert.ToString(row["customerNo"]);
            model.customerName = Convert.ToString(row["customerName"]);
            model.phone = Convert.ToString(row["phone"]);
            model.invDate = Convert.ToString(row["invDate"]);
            model.invNumber = Convert.ToString(row["invNumber"]);
            model.dueDate = Convert.ToString(row["dueDate"]);
            model.totalAmount = Convert.ToString(row["totalAmount"]);
            model.onemo = Convert.ToString(row["onemo"]);
            model.twomo = Convert.ToString(row["twomo"]);
            model.threemo = Convert.ToString(row["threemo"]);
            model.fourmo = Convert.ToString(row["fourmo"]);
            model.fivemo = Convert.ToString(row["fivemo"]);
            model.originalTotal = Convert.ToString(row["originalTotal"]);
            return model;
        }

    }
    public class RegionIdsViewModel
    {
        public int regionId { get; set; }
        public string regionName { get; set; }
    }

    public class PastDueViewModel
    {
        public int Flag { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int DayDifference { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public int PeriodId { get; set; }
        public decimal Total { get; set; }
        public int RegionId { get; set; }
        public int MasterTrxId { get; set; }
        public string RegionName { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public decimal FirstSegment { get; set; }
        public decimal SecondSegment { get; set; }
        public decimal ThirdSegment { get; set; }
        public decimal FourthSegment { get; set; }
        public decimal NoneSegment { get; set; }

    }

    public class PastDueStatementDetailModel
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }

        public string SoldAddress1 { get; set; }
        public string SoldAddress2 { get; set; }
        public string SoldCity { get; set; }
        public string SoldStateName { get; set; }
        public string SoldPostalCode { get; set; }
        public string SoldPhone { get; set; }
        public string SoldPhoneExt { get; set; }

        public int RegionId { get; set; }
        public List<PastDueStatementFranchiseeModel> PastDueStatementFranchisee { get; set; }
        public List<PastDueStatementInvoiceModel> PastDueStatementInvoices { get; set; }

        public JKViewModels.Customer.RemitToViewModel RemitTo { get; set; }

    }
    public class PastDueStatementFranchiseeModel
    {
        public int FranchiseeId { get; set; }
        public string FranchiseeNo { get; set; }
        public string FranchiseeName { get; set; }
    }

    public class PastDueStatementInvoiceModel
    {
        public DateTime? InvoiceDate { get; set; }
        public int DayDifference { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public int PeriodId { get; set; }
        public decimal Total { get; set; }
        public int RegionId { get; set; }
        public int MasterTrxId { get; set; }
        public string RegionName { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public DateTime? DueDate { get; set; }
        public string InvoiceDescription { get; set; }
    }

    public class RegionGroupBy
    {
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public decimal FirstTotal { get; set; }
        public decimal SecondTotal { get; set; }
        public decimal ThirdTotal { get; set; }
        public decimal FourthTotal { get; set; }
        public decimal FifthTotal { get; set; }
        public decimal FinalTotal { get; set; }
    }
}

