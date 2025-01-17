﻿using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace JK.FMS.MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LoggingFilterAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CompressLiveAttribute());
        }
    }

    public class CompressLiveAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var _encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(_encodingsAccepted)) return;

            _encodingsAccepted = _encodingsAccepted.ToLowerInvariant();
            var _response = filterContext.HttpContext.Response;

            if (_encodingsAccepted.Contains("deflate"))
            {
                _response.AppendHeader("Content-encoding", "deflate");
                _response.Filter = new DeflateStream(_response.Filter, CompressionMode.Compress);
            }
            else if (_encodingsAccepted.Contains("gzip"))
            {
                _response.AppendHeader("Content-encoding", "gzip");
                _response.Filter = new GZipStream(_response.Filter, CompressionMode.Compress);
            }
        }
    }
}
