namespace JKApi.WebAPI.Dtos.Common
{
    /// <summary>
    /// AddressResponseDto
    /// </summary>
    public class AddressResponseDto : IResponseDto
    {
        /// <summary>
        /// AddressId
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// AddressId
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Address2
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// ZipCode
        /// </summary>
        public string ZipCode { get; set; }
    }
}