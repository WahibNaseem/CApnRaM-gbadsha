using System;
using System.Collections.Generic;
using JKApi.WebAPI.Common;
using JKApi.WebAPI.Dtos.Common;
using Newtonsoft.Json;

namespace JKApi.WebAPI.Dtos.Customer
{
    /// <summary>
    /// ContractDetailByFranchiseeRequestDto
    /// </summary>
    public class CustomerDetailByFranchiseeRequestDto : IRequestDto
    {
        /// <summary>
        /// FranchiseeId
        /// </summary>
        public int FranchiseeId { get; set; }
    }

    /// <summary>
    /// ContractDetailResponseDto
    /// </summary>
    public class CustomerResponseDto : IResponseDto
    {
        /// <summary>
        /// CustomerId
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// CustomerId
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// CustomerName
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// AccountType
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// ServiceType
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }


        /// <summary>
        /// Address
        /// </summary>
        public AddressResponseDto Address { get; set; }
        
    }
}