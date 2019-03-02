using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using JKViewModels.Common;

namespace JKApi.Service.Service.File
{
    public interface IFileService
    {
        FileModel AddOrUpdateFile(FileModel model);
        ViewFileListModel GetFileListByClassAndType(ViewFileListModel listModel);
        FileModel GetFile(int fileId);

        string GetUploadDocumentFile(string fileName);
    }

    public class FileService : BaseService, IFileService
    {
        public FileModel AddOrUpdateFile(FileModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FileId", model.FileId);
            parameters.Add("@RegionId", model.RegionId);
            parameters.Add("@ClassId", model.ClassId);
            parameters.Add("@FileType", model.FileType);
            parameters.Add("@FileName", model.FileName);
            parameters.Add("@ContentType", model.ContentType);
            parameters.Add("@FileUrl", model.FileUrl);
            parameters.Add("@IsEnable", true);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@CreatedDate", model.FileId == 0 ? DateTime.Now : model.CreatedDate);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@ModifiedDate", model.ModifiedDate ?? DateTime.Now);

            if (model.FileId == 0)
            {
                parameters.Add("@CreatedDate", DateTime.Now);
                parameters.Add("@CreatedBy", model.CreatedBy);
            }

            using (var result = FmsDbConn.QuerySingleOrDefault<FileModel>(DBConstants.common_AddOrUpdateFile, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public ViewFileListModel GetFileListByClassAndType(ViewFileListModel listModel)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ClassId", listModel.ClassId);
            parameters.Add("@FileType", listModel.FileType);
            parameters.Add("@IsEnable", listModel.IsEnable);
            parameters.Add("@SortColumn", listModel.FileType);
            parameters.Add("@SortOrder", listModel.SortBy);

            using (var multipleResult = FmsDbConn.QueryMultiple(DBConstants.common_GetFileListByClassAndType, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                var fileList = multipleResult?.Read<FileModel>().ToList();
                if (fileList != null) listModel.FileList = fileList;
                return listModel;
            }
        }

        public FileModel GetFile(int fileId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FileId", fileId);

            using (var result = FmsDbConn.QuerySingleOrDefault<FileModel>(DBConstants.common_GetFile, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result;
            }
        }

        public string GetUploadDocumentFile(string fileName)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@fileName", fileName);

            using (var result = FmsDbConn.QuerySingleOrDefault<FileModel>(DBConstants.common_GetFile, parameters,
                    commandType: CommandType.StoredProcedure))
            {
                return result?.ToString() ?? string.Empty;
            }
        }
    }
}
