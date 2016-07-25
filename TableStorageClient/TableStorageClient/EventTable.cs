using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableStorageClient
{
    public class EventTable : TableEntity
    {
        public EventTable(Guid eventId, string eventName)
        {
            this.PartitionKey = eventId.ToString();
            this.RowKey = eventName;
        }

        public EventTable() { }

        public string EventStartDate { get; set; }

        public string EventEndDate { get; set; }

        public int TotalIn { get; set; }

        public int TotalOut { get; set; }
    }
}
