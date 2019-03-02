using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JK.FMS.MVC.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult Index()
        {
            return View("Error");
        }
        public ViewResult NotFound()
        {
            Response.StatusCode = 200;  //you may want to set this to 200
            return View("NotFound");
        }
    }
}