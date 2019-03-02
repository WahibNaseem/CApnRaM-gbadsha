namespace JKApi.WebAPI.Dtos.NegativeDue
{
    public class GetListRequestDto : IRequestDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int FranchiseeStatus { get; set; }
        public string RegionIds { get; set; }
        public int SelectedPeriodId { get; set; }
    }
}