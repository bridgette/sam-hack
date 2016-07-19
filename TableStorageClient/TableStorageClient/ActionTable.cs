using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace StorageClient
{
    public class ActionTable : TableEntity
    {
        public ActionTable(string eventId, DateTime timeStamp)
        {
            this.PartitionKey = eventId.ToString();
            this.RowKey = timeStamp.ToLongDateString();
        }

        public ActionTable() { }

        /// <summary>
        /// People who have entered frame
        /// </summary>
        public int CountIn { get; set; }

        /// <summary>
        /// People who have left frame
        /// </summary>
        public int CountOut { get; set; }
    }
}