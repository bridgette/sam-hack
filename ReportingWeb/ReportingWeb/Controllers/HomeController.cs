using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StorageClient;
using System.Web.Script.Serialization;
using ReportingWeb.ExportFiles;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace ReportingWeb.Controllers
{
    public class HomeController : Controller
    {
        private string EventTableName = "Event";
        private string ActionTableName = "Action";

        /// <summary>
        /// Create Table Storage Client
        /// </summary>
        private TableStorageClient tableClient;
        private BlobStorageClient blobClient;


        /// <summary>
        /// Instantiate controller
        /// </summary>
        public HomeController()
        {
            tableClient = new TableStorageClient(Constants.StorageConnectionString);
            blobClient = new BlobStorageClient(Constants.StorageConnectionString);
        }

        /// <summary>
        /// Main controller action to render page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Query data and return as JSON
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <param name="startDate">Nullable Start Date</param>
        /// <param name="endDate">Nullable End Date</param>
        /// <returns>List of results as JSON ActionResult</returns>
        [HttpPost]
        public ActionResult QueryData(string eventId, string startDate, string endDate)
        {
            DateTime startDateTime = SafeParseDate(startDate);
            DateTime endDateTime = SafeParseDate(endDate);
            if(endDateTime == new DateTime())
            {
                endDateTime = DateTime.Now;
            }

            List<ActionTable> records = GetDataAndCache(eventId, startDate, endDate);

            return ReturnJson(records);
        }

        /// <summary>
        /// Get and cache data
        /// </summary>
        private List<ActionTable> GetDataAndCache(string eventId, string startDate, string endDate)
        {
            DateTime startDateTime = SafeParseDate(startDate);
            DateTime endDateTime = SafeParseDate(endDate);
            if (endDateTime == new DateTime())
            {
                endDateTime = DateTime.Now;
            }

            string cacheKey = eventId + startDate + endDate;

            if (Session[cacheKey] != null)
            {
                //TODO uncomment after development
                //return Session[cacheKey] as List<ActionTable>;
            }

            List<ActionTable> records = tableClient.GetActionRecordsInPartition(this.ActionTableName, eventId);
            records = records.Where(x => DateTime.Parse(x.RowKey) >= startDateTime && DateTime.Parse(x.RowKey) <= endDateTime).OrderBy(x => DateTime.Parse(x.RowKey)).ToList();

            Session[cacheKey] = records;

            return records;

        }
        
        /// <summary>
        /// Get all events in the system
        /// </summary>
        /// <returns>List of events as JSON ActionResult</returns>
        [HttpPost]
        public ActionResult GetEvents()
        {
            List<EventTable> events = GetEventsInner();
            return ReturnJson(events);
        }

        /// <summary>
        /// Get list of events data
        /// </summary>
        private List<EventTable> GetEventsInner()
        {
            return tableClient.GetAllPartitions(this.EventTableName).OrderBy(x => x.RowKey).ToList();
        }

        [HttpGet]
        public ActionResult ExportActionTable(string eventId, string startDate, string endDate)
        {
            List<ActionTable> records = this.GetDataAndCache(eventId, startDate, endDate);
            string csv = new CsvExport<ActionTable>(records).Export();
            string blobPath = blobClient.UploadBlob(csv);
            return Redirect(blobPath);
        }

        [HttpGet]
        public ActionResult ExportEventTable()
        {
            List<EventTable> events = GetEventsInner();
            string csv = new CsvExport<EventTable>(events).Export();
            string blobPath = blobClient.UploadBlob(csv);
            return Redirect(blobPath);
        }

        /// <summary>
        /// Parse date with additional defaults and checks
        /// </summary>
        /// <param name="dateString">Date string</param>
        /// <returns>Parsed string as DateTime</returns>
        private DateTime SafeParseDate(string dateString)
        {
            DateTime parsedDateTime = new DateTime();

            if (!String.IsNullOrWhiteSpace(dateString))
            {
                DateTime.TryParse(dateString, out parsedDateTime);
            }

            return parsedDateTime;
        }

        /// <summary>
        /// Return an object as a JSON ActionResult
        /// </summary>
        /// <param name="data">Generic Object to serialize</param>
        /// <returns>JSON as ActionResult</returns>
        private ActionResult ReturnJson(Object data)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return Content(jss.Serialize(data), "application/json");
        }

        public ActionResult GenerateData()
        {
            EventTable eventTable = new EventTable(Guid.NewGuid(), "Demo Event");
            tableClient.InsertRecord("Event", eventTable);

            var action = new ActionTable(eventTable.PartitionKey, DateTime.Now.AddDays(-500));
            action.CountIn = 9;
            action.CountOut = 1;
            var action1 = new ActionTable(eventTable.PartitionKey, DateTime.Now.AddDays(-1000));
            action1.CountIn = 9;
            action.CountOut = 2;
            var action2 = new ActionTable(eventTable.PartitionKey, DateTime.Now.AddDays(-1500));
            action2.CountIn = 1;
            action2.CountOut = 3;
            var action3 = new ActionTable(eventTable.PartitionKey, DateTime.Now.AddDays(-2000));
            action3.CountIn = 3;
            action3.CountOut = 4;
            var action4 = new ActionTable(eventTable.PartitionKey, DateTime.Now.AddDays(-9000));
            action4.CountIn = 4;
            action4.CountOut = 2;

            tableClient.InsertRecord("Action", action);
            tableClient.InsertRecord("Action", action1);
            tableClient.InsertRecord("Action", action2);
            tableClient.InsertRecord("Action", action3);
            tableClient.InsertRecord("Action", action4);

            return RedirectToAction("Index");
        }

        public ActionResult CreateTables()
        {
            tableClient.CreateTable(this.EventTableName);
            tableClient.CreateTable(this.ActionTableName);

            return RedirectToAction("Index");
        }

        public ActionResult DeleteTables()
        {
            tableClient.DeleteTable(this.EventTableName);
            tableClient.DeleteTable(this.ActionTableName);

            return RedirectToAction("Index");
        }
    }
}