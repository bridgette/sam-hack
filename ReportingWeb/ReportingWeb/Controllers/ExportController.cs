using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReportingWeb.ExportFiles;
using TableStorageClient;
using System.IO;

namespace ReportingWeb.Controllers
{
    public class ExportController : Controller
    {
        private static string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=cohenhackathon;AccountKey=2EIamWgLyH38CBBqJRAULJTvNOHlqRZiaeb/x16c7q23YJJtPqVI+vTmsyCj6+eUfeOajX7c6cjUkGHB0JrlMg==";

        private TableStorageClient.TableStorageClient tableClient = new TableStorageClient.TableStorageClient(StorageConnectionString);

        // GET api/<controller>
        public ActionResult GetAction(string eventId, string timeStamp)
        {
            tableClient.GetActionRecordsByPartitionAndSubkey("Action", eventId, timeStamp);

            return Content("".ToString(), "application/json"); ;
        }

        // GET api/<controller>/5
        public ActionResult GetEvent(string id, string name)
        {
            tableClient.GetEventRecordsByPartitionAndSubkey("Event", name, id);
            return Content("".ToString(), "application/json"); ;
        }

        public ActionResult ExportAction(List<Action> actions)
        {
            //get path
            var filePath = ExportAsCSV.Export<Action>(actions);
            //convert to bytes
            var fileData = System.IO.File.ReadAllBytes(filePath);
            string contentType = MimeMapping.GetMimeMapping(filePath);
            string[] directories = filePath.Split(Path.DirectorySeparatorChar);
            string filename = directories[directories.Length - 1];
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(fileData, contentType);
        }

        public ActionResult ExportEvent(List<EventTable> events)
        {
            //get path
            var filePath = ExportAsCSV.Export<EventTable>(events);
            //convert to bytes
            var fileData = System.IO.File.ReadAllBytes(filePath);
            string contentType = MimeMapping.GetMimeMapping(filePath);
            string filename = "";
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(fileData, contentType);
        }
    }
}
