namespace JKApi.WebAPI.Dtos
{
    public interface IQueryDto
    {
    }

    public class FileUploadRequestDto : IRequestDto
    {
        public int FileId { get; set; }
        public int RegionId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
        public int ClassId { get; set; }
        public int FileType { get; set; }
        public bool IsEnable { get; set; }
    }

    public class FileQueryDto : IQueryDto
    {
        public int FranchiseeId { get; set; }
        public int FileType { get; set; }
        public bool IsEnable { get; set; }
    }

    public class FileListByClassIdRequestDto : IRequestDto
    {
        public int ClassId { get; set; }
        public bool IsEnable { get; set; }
    }
}