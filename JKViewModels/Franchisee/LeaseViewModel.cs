﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKViewModels.JKControl;

namespace JKViewModels.Franchisee
{
    public class LeaseViewModel
    {        
        public int LeaseId { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> TypeListId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> StatusListId { get; set; }
        public string LeaseNumber { get; set; }
        public string LeaseDescription { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public Nullable<System.DateTime> SignDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public bool ChargeTaxUpFront { get; set; }
        public Nullable<decimal> LeaseAmount { get; set; }
        public Nullable<decimal> LeaseTotalTax { get; set; }
        public Nullable<decimal> LeaseTotal { get; set; }
        public Nullable<decimal> MonthlyPaymentAmount { get; set; }
        public Nullable<decimal> MonthlyTaxRate { get; set; }
        public Nullable<decimal> MonthlyPaymentTotal { get; set; }
        public Nullable<int> InstallmentDownPaymentNum { get; set; }
        public Nullable<int> InstallmentMonthlyPaymentNum { get; set; }
        public Nullable<int> InstallmentLastPaymentNum { get; set; }
        public Nullable<bool> DownPaymentPaid { get; set; }
        public Nullable<int> NumOfPaymentsPaid { get; set; }
        public Nullable<decimal> TotalAmountPaid { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ImpId { get; set; }
        public bool IncludeFirstPaymentInDownPayment { get; set; }
        public Nullable<decimal> Balance { get; set; }        
    }
}
