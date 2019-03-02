using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Web.Http;
using JKApi.Service;
using JKApi.WebAPI.Dtos;
using JKApi.WebAPI.Common;
using JKApi.Core;
using JKApi.Service.Service.File;
using JKApi.WebAPI.Filters;
using JKViewModels.Common;

namespace JKApi.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v{version:apiVersion}/file")]
    [Authorized]
    public class FileController : BaseApiController
    {
        private readonly string _fileDirectory;
        private readonly ICommonService _commonService;
        private readonly IFileService _fileService;

        // ======================================================================================
        #region FileController > Constructor
        // ======================================================================================

        public FileController(ICommonService commonService, IFileService fileService)
        {
            _fileDirectory = ConfigurationManager.AppSettings[AppConstants.APPSETTINGS_FILESDIRECTORY];
            _commonService = commonService;
            _fileService = fileService;
        }

        #endregion
        // ======================================================================================
        #region FileController > Private
        // ======================================================================================

        public string _getFilePath(string directory, int regionId, string folderName)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0}\\", _fileDirectory);
            stringBuilder.AppendFormat("region_{0}\\", regionId);
            stringBuilder.AppendFormat("{0}\\", folderName ?? "unknown");
            var path = stringBuilder.ToString();
            return path;
        }

        public string _getFile(string directory, int regionId, string folderName, string fileName)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0}\\", _fileDirectory);
            stringBuilder.AppendFormat("region_{0}\\", regionId);
            stringBuilder.AppendFormat("{0}\\", folderName ?? "unknown");
            stringBuilder.AppendFormat("{0}", fileName ?? "unknown");
            var path = stringBuilder.ToString();
            return path;
        }

        #endregion
        // ======================================================================================
        #region FileController > API Calls
        // ======================================================================================

        [Route("upload")]
        [HttpPut]
        [ResponseType(typeof(FileModel))]
        public IHttpActionResult Upload()
        {
            var file = HttpContext.Current.Request.Files["File"];
            if (file == null)
            {
                return ResponseErrorResult(ApiException.ErrorCode.NoUploadFile);
            }

            try
            {
                var fileId = Convert.ToInt32(HttpContext.Current.Request.Form["FileId"]);
                var classId = Convert.ToInt32(HttpContext.Current.Request.Form["ClassId"]);
                var regionId = Convert.ToInt32(HttpContext.Current.Request.Form["RegionId"]);
                var fileType = Convert.ToInt32(HttpContext.Current.Request.Form["FileType"]);
                var isEnable = Convert.ToBoolean(HttpContext.Current.Request.Form["IsEnable"]);
                var createdBy = Convert.ToInt32(HttpContext.Current.Request.Form["CreatedBy"]);
                var modifiedBy = Convert.ToInt32(HttpContext.Current.Request.Form["ModifiedBy"]);

                // Check to see if the file exists, then delete the current one from the directory
                var fileModel = _fileService.GetFile(fileId);
                var fileTypeModel = _commonService.GetFileType(fileType);
                if (fileModel != null)
                {
                    var currentFile = _getFile(_fileDirectory, fileModel.RegionId, fileTypeModel.Name, fileModel.FileUniqueId());
                    AppUtils.DeleteFile(currentFile);
                }

                // Add or update the file to the database
                fileModel = _fileService.AddOrUpdateFile(new FileModel
                {
                    FileId = fileModel?.FileId ?? 0,
                    FileName = file.FileName,
                    ClassId = classId,
                    RegionId = regionId,
                    ContentType = file.ContentType,
                    FileType = fileTypeModel?.FileType ?? 0,
                    IsEnable = isEnable,
                    CreatedBy = createdBy,
                    ModifiedBy = modifiedBy
                });

                if (fileModel == null || fileModel.FileId <= 0)
                {
                    return ResponseErrorResult(ApiException.ErrorCode.FailExecute);
                }

                // Get the new path
                var fileUniqueId = string.Format($"{fileModel.FileId}_{fileModel.FileName}");
                var newPath = _getFilePath(_fileDirectory, fileModel.RegionId, fileTypeModel.Name);
                AppUtils.SaveFile(newPath, file, fileUniqueId);

                // Build the url the update with the new path
                var url = Request.RequestUri.AbsoluteUri.Remove(Request.RequestUri.AbsoluteUri.Length - Request.RequestUri.Segments.Last().Length);
                url = url.EndsWith("/") ? url.Substring(0, url.Length - 1) : url;
                if (fileTypeModel != null) url = url + "/" + fileTypeModel.Name;
                url = url + "/" + fileUniqueId;
                fileModel.FileUrl = url;
                fileModel = _fileService.AddOrUpdateFile(fileModel);

                return ResponseSuccessResult(fileModel);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("{foldername}/{filename}")]
        [OverrideAuthorization]
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetFileContent(string foldername, string filename)
        {
            if (foldername == null || filename == null)
            {
                return NotFound();
            }

            try
            {
                var substrings = filename.Split('_');

                if (substrings.Length > 0)
                {
                    var fileId = substrings[0];
                    var fileModel = _fileService.GetFile(Convert.ToInt32(fileId));
                    if (fileModel == null || fileModel.FileId <= 0)
                    {
                        return NotFound();
                    }
                    var filePath = _getFile(_fileDirectory, fileModel.RegionId, foldername, fileModel.FileUniqueId());
                    var fileInfo = new FileInfo(filePath);
                    return !fileInfo.Exists ? (IHttpActionResult)NotFound() : new FileResult(fileInfo.FullName);
                }
                return NotFound();
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("listbyclassid")]
        [HttpPost]
        [ResponseType(typeof(IList<FileModel>))]
        public IHttpActionResult ListByClassId(FileListByClassIdRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var listModel = new ViewFileListModel
                {
                    ClassId = requestDto.ClassId,
                    IsEnable = requestDto.IsEnable
                };
                listModel = _fileService.GetFileListByClassAndType(listModel);
                return ResponseSuccessResult(listModel.FileList);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("{fileid}")]
        [HttpGet]
        [ResponseType(typeof(FileModel))]
        public IHttpActionResult ById(int fileid)
        {
            if (!ModelState.IsValid)
            {
                ResponseForInvalidModelState();
            }

            try
            {
                var file = _fileService.GetFile(fileid);
                return ResponseSuccessResult(file);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("FileName/{filename}")]
        [HttpGet]
        public IHttpActionResult GetUploadDocumentFileContent(string filename)
        {
            if (filename == null)
            {
                return NotFound();
            }

            try
            {
                var path = _fileService.GetUploadDocumentFile(filename);
                var fileInfo = new FileInfo(path);
                return !fileInfo.Exists ? (IHttpActionResult)NotFound() : new FileResult(fileInfo.FullName);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        [Route("FileView")]
        [HttpGet]
        public IHttpActionResult FileView(int CustomerId)
        {
            try
            {
                if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"], "CustomerDocument")))
                {
                    Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"], "CustomerDocument"));
                }
                if (!Directory.Exists(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"], "CustomerDocument", CustomerId.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"], "CustomerDocument", CustomerId.ToString()));
                }

                var objFileViewModel = new List<CrmFileModel>();
                var dir = new DirectoryInfo(Path.Combine(ConfigurationManager.AppSettings["FilesDirectory"], "CustomerDocument", CustomerId.ToString()));

                foreach (var flInfo in dir.GetFiles())
                {
                    objFileViewModel.Add(new CrmFileModel() { Path = flInfo.FullName.Replace(ConfigurationManager.AppSettings["FilesDirectory"], @"\docs"), FileName = flInfo.Name });
                }
                return ResponseSuccessResult(objFileViewModel);
            }
            catch (ApiException exception)
            {
                return ResponseErrorResult(exception.Code);
            }
            catch (Exception exception)
            {
                return ResponseErrorResult(ApiException.ErrorCode.FailExecute, exception.Message);
            }
        }

        #endregion
        // ======================================================================================
    }
}
