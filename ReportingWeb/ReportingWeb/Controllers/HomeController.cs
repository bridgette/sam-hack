using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using TableStorageClient;

namespace ReportingWeb.Controllers
{
    public class HomeController : Controller
    {
        private string StorageConnectionString = "";

        public ActionResult Index()
        {
            Guid partitionId = Guid.NewGuid();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}