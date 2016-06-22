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
        private string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=cohenhackathon;AccountKey=2EIamWgLyH38CBBqJRAULJTvNOHlqRZiaeb/x16c7q23YJJtPqVI+vTmsyCj6+eUfeOajX7c6cjUkGHB0JrlMg==";

        public ActionResult Index()
        {
            Guid partitionId = Guid.NewGuid();

            TableStorageClient.TableStorageClient tableClient = new TableStorageClient.TableStorageClient(this.StorageConnectionString);
            tableClient.CreateTable("Action");

            ActionTableEntity e = new ActionTableEntity(partitionId, Guid.NewGuid());
            e.ActionType = "asd";
            e.EventName = "asdasd";

            tableClient.InsertRecord("Action", e);

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