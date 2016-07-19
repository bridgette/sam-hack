using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageClient
{
    public class EventTable : TableEntity
    {
        public EventTable(Guid eventId, string eventName)
        {
            this.PartitionKey = eventId.ToString();
            this.RowKey = eventName;
        }

        public EventTable() { }

        /// <summary>
        /// Start Date Of Event
        /// </summary>
        public string EventStartDate { get; set; }

        /// <summary>
        /// End Date Of Event
        /// </summary>
        public string EventEndDate { get; set; }

        /// <summary>
        /// Total people who have entered frame
        /// </summary>
        public int TotalIn { get; set; }

        /// <summary>
        /// Total people who have exited frame
        /// </summary>
        public int TotalOut { get; set; }
    }
}
