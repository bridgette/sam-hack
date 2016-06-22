using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableStorageClient
{
    public class EventTableEntity : TableEntity
    {
        public EventTableEntity(Guid eventId, string eventName)
        {
            this.PartitionKey = eventId.ToString();
            this.RowKey = eventName;
        }

        public EventTableEntity() { }

        public DateTime EventDate { get; set; }
    }
}
