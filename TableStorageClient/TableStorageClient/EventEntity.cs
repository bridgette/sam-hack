using Microsoft.WindowsAzure.Storage.Table;
using System;

public class EventEntity : TableEntity
{
    public EventEntity(Guid eventId, string dateShortString)
    {
        this.PartitionKey = eventId.ToString();
        this.RowKey = dateShortString;
    }

    public EventEntity() { }

    public string EventName { get; set; }

    public DateTime TimeStamp { get; set; }
}