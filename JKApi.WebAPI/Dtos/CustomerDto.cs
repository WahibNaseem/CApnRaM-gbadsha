using System;

namespace JKApi.WebAPI.Dtos
{
    public class CustomerByFranchiseeRequestDto : IRequestDto
    {
        public int FranchiseeId { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
    }

    public class CustomerByRegionRequestDto : IRequestDto
    {
        public int RegionId { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
    }

    public class NearbyByFranchiseeRequestDto : CustomerByFranchiseeRequestDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
    }

    public class NearbyByRegionRequestDto : IRequestDto
    {
        public int RegionId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
    }
}