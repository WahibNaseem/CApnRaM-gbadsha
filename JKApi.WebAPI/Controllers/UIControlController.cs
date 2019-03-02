using JKApi.Service.Service;
using JKViewModels;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace JKApi.WebAPI.Controllers
{
    public class UIControlController : Controller
    {
        private UIService _service;
        public UIControlController()
        {
            _service = new UIService();
        }
        /// <summary>
        /// Horizontal Menu
        /// </summary>
        /// <param name="GetMenusByMenuTypeName">Get Menus By Menu Type Name</param>
        /// <returns>Get Horizontal Menu</returns> 
        [HttpGet]
        [Route("GetMenusByMenuTypeName")] 
        public ActionResult GetMenusByMenuTypeName(string MenuTypeName)
        {
          //  IEnumerable<MenuViewModel> Menu = _service.GetMenusByMenuTypeName(MenuTypeName).ToList();
            return Json(new { aaData = "" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Vertical Menu
        /// </summary>
        /// <param name="GetMenusByMenuTypeName">Get Menus By Menu Type Name</param>
        /// <returns>Get Horizontal Menu</returns> 
        [HttpGet]
        [Route("GetMenusByMenuTypeNameAndParentId")]
        public ActionResult GetMenusByMenuTypeNameAndParentId(string MenuTypeName, string ParentId)
        {
            int _parentId = 0;
            if(!string.IsNullOrEmpty(ParentId))
            {
                _parentId = Convert.ToInt32(ParentId);
            }
           // IEnumerable<MenuViewModel> Menu = _service.GetMenusByMenuTypeNameAndParentId(MenuTypeName, _parentId).ToList();
            return Json(new { aaData = "" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Vertical Menu
        /// </summary>
        /// <param name="GetMenusByMenuTypeName">Get Menus By Menu Type Name</param>
        /// <returns>Get Horizontal Menu</returns> 
        [HttpGet]
        [Route("GetMenusByMenuParentId")]
        public ActionResult GetMenusByMenuParentId(string ParentId)
        {
            int _parentId = 0;
            if (!string.IsNullOrEmpty(ParentId))
            {
                _parentId = Convert.ToInt32(ParentId);
            }
           // IEnumerable<MenuViewModel> Menu = _service.GetMenusByParentId(_parentId).ToList();
            return Json(new { aaData = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}
