using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKApi.Service.Helper.Extension
{

    public enum CAgreement
    {
        [Description("Customer")]
        Customer = 2,
        [Description("JaniKing")]
        JaniKing = 1,
    }

    public enum CTerms
    {
        [Description("0")]
        _0 = 0,
        [Description("1")]
        _1 = 1,
        [Description("2")]
        _2 = 2,
        [Description("3")]
        _3 = 3,
    }


    public enum CustomerStatus
    {
        [Description("Active")]
        Active = 1,

        [Description("Cancelled")]
        Cancelled,

        [Description("Suspended")]
        Suspended,

        [Description("Pending")]
        Pending,
        
    }


    public enum BillMonths
    {
        [Description("January")]
        January = 1,

        [Description("February")]
        February,

        [Description("March")]
        March,

        [Description("April")]
        April,

        [Description("May")]
        May,

        [Description("June")]
        June,

        [Description("July")]
        July,

        [Description("August")]
        August,

        [Description("September")]
        September,

        [Description("October")]
        October,

        [Description("November")]
        November,

        [Description("December")]
        December
    }

    

    public enum SearchByAging
    {
        [Description("Customer No.")]
        CustomerNo = 1,

        [Description("Customer Name")]
        CustomerName = 2,

        [Description("Franchise No.")]
        FranchiseNo = 4,

        [Description("Franchise Name")]
        FranchiseName = 5,
    }

    public enum OrderByAging
    {
        [Description("Customer Name")]
        CustomerName = 1,

        [Description("Invoice Balance")]
        InvoiceBalance = 2,

    }

    public enum IncludeListAging
    {
        [Description("All Invoices")]
        AllInvoices = 1,

        [Description("Open Invoices")]
        OpenInvoices = 3,
    }

    public enum balanceListAging
    {
        [Description("No Restriction")]
        NoRestriction = -1,

        [Description("less than $0")]
        LessThan0 = 0,

        [Description("greater than $0")]
        GreaterThan0 = 1,

        [Description("$1,000 or more")]
        _1000orMore = 1000,

        [Description("$5,000 or more")]
        _5000orMore = 5000,

        [Description("10,000 or more")]
        _10000orMore = 10000,

        [Description("$15,000 or more")]
        _15000orMore = 15000,

        [Description("$20,000 or more")]
        _20000orMore = 20000,

        [Description("$25,000 or more")]
        _25000orMore = 25000,
    }

    public enum dateTypeARLog
    {
        [Description("Entered Date")]
        EnteredDate = 1,

        [Description("Transaction Date")]
        TransactionDate = 2,
    }
}
