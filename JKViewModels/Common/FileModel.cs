using System.Collections.Generic;
using JKViewModels.Inspection;

namespace JKViewModels.Common
{
    public class FileModel : BaseEntityModel
    {
        public int FileId { get; set; }
        public int RegionId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FileUrl { get; set; }
        public int ClassId { get; set; }
        public int FileType { get; set; }

        public string FileUniqueId()
        {
            return $"{FileId}_{FileName}";
        }
        
    }

    public class ViewFileListModel : ListModel
    {
        public string SearchText { get; set; }
        public int ActiveStatus { get; set; }
        public int ClassId { get; set; }
        public int FileType { get; set; }
        public IList<FileModel> FileList { get; set; }

        public ViewFileListModel()
        {
            FileList = new List<FileModel>();
        }
    }

    public class CrmFileModel
    {
        public string Path { get; set; }
        public string FileName { get; set; }
    }
}
