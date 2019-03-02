using System.ComponentModel.DataAnnotations;
using JKApi.WebAPI.Common;

namespace JKApi.WebAPI.Dtos
{
    #region Interfaces

    public interface IContainerResponseDto
    {
        bool IsSuccess { get; set; }
        ApiException.ErrorCode MessageCode { get; set; }
        string Message { get; set; }
        object Data { set; get; }
    }

    public interface IUrlParamRequestDto
    {
    }

    public interface IRequestDto
    {
    }

    public interface IResponseDto
    {
    }

    public interface INotificationDto
    {
    }

    #endregion

    #region Classes

    public class ContainerResponseDto : IContainerResponseDto
    {
        [Required]
        public bool IsSuccess { get; set; }

        [Required]
        public ApiException.ErrorCode MessageCode { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public object Data { get; set; }
    }

    public class ErrorDto
    {
        [Required]
        public int MessageCode { get; set; }
        [Required]
        public string Message { get; set; }
    }

    public class PageRequestDto : IRequestDto
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
    }

    #endregion
}