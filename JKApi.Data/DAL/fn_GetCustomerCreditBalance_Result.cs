//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JKApi.Data.DAL
{
    using System;
    
    public partial class fn_GetCustomerCreditBalance_Result
    {
        public int CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public Nullable<decimal> TotalCredit { get; set; }
        public Nullable<decimal> TotalCreditUsed { get; set; }
        public Nullable<decimal> Balance { get; set; }
    }
}
