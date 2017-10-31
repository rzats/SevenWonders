using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SevenWonders.WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Customers()
        {
            string apiUri = Url.HttpRouteUrl("API Default", new { controller = "CustomersManagement" });
            ViewBag.ApiUrl = new Uri(Request.Url, apiUri).AbsoluteUri.ToString();

            return View();
        }

        public ActionResult Managers()
        {
            string apiUri = Url.HttpRouteUrl("API Default", new { controller = "ManagersManagement" });
            ViewBag.ApiUrl = new Uri(Request.Url, apiUri).AbsoluteUri.ToString();

            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }public ActionResult Tours()
        {
            return View();
        }    }
}
