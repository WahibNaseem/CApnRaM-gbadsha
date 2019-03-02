namespace JKApi.WebAPI.Dtos
{
    /// <summary>
    /// AddressResponseDto
    /// </summary>
    public class AddressResponseDto : IResponseDto
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}