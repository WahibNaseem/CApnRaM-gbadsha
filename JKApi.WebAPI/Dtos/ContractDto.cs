using System;

namespace JKApi.WebAPI.Dtos
{
    // ======================================================================================
    #region Request
    // ======================================================================================

    /// <summary>
    /// ContractByFranchiseeRequestDto
    /// </summary>
    public class ContractByFranchiseeRequestDto : IRequestDto
    {
        /// <summary>
        /// FranchiseeId
        /// </summary>
        public int FranchiseeId { get; set; }
    }

    #endregion

    // ======================================================================================
    #region Response
    // ======================================================================================

    /// <summary>
    /// ContractResponseDto
    /// </summary>
    public class ContractResponseDto : IResponseDto
    {
        /// <summary>
        /// DistributionId
        /// </summary>
        public int DistributionId { get; set; }

        /// <summary>
        /// FranchiseeId
        /// </summary>
        public int FranchiseeId { get; set; }

        /// <summary>
        /// ContractId
        /// </summary>
        public int ContractId { get; set; }

        /// <summary>
        /// ContractDetailid
        /// </summary>
        public int ContractDetailId { get; set; }

        /// <summary>
        /// CustomerId
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// SoldById
        /// </summary>
        public int SoldById { get; set; }

        /// <summary>
        /// PurchaseOrderNumber
        /// </summary>
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// CustomerName
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// PrimaryContact
        /// </summary>
        public string PrimaryContact { get; set; }

        /// <summary>
        /// PrimaryContactPhone
        /// </summary>
        public string PrimaryContactPhone { get; set; }

        /// <summary>
        /// PrimaryContactPhoneExt
        /// </summary>
        public string PrimaryContactPhoneExt { get; set; }

        /// <summary>
        /// ContractType
        /// </summary>
        public string ContractType { get; set; }

        /// <summary>
        /// ServiceType
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// AccountType
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// ExpirationDate
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// ContractDescription
        /// </summary>
        public string ContractDescription { get; set; }

        /// <summary>
        /// ContractTermMonth
        /// </summary>
        public int ContractTermMonth { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// SquareFootage
        /// </summary>
        public string SquareFootage { get; set; }

        /// <summary>
        /// CleanTimes
        /// </summary>
        public int CleanTimes { get; set; }

        /// <summary>
        /// Mon
        /// </summary>
        public bool Mon { get; set; }

        /// <summary>
        /// Tue
        /// </summary>
        public bool Tue { get; set; }

        /// <summary>
        /// Wed
        /// </summary>
        public bool Wed { get; set; }

        /// <summary>
        /// Thu
        /// </summary>
        public bool Thu { get; set; }

        /// <summary>
        /// Fri
        /// </summary>
        public bool Fri { get; set; }

        /// <summary>
        /// Sat
        /// </summary>
        public bool Sat { get; set; }

        /// <summary>
        /// Sun
        /// </summary>
        public bool Sun { get; set; }

        /// <summary>
        /// StartTime
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// EndTime
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public AddressResponseDto Address { get; set; }
    }

    #endregion
}