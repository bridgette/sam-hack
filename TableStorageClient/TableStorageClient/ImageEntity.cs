using Microsoft.WindowsAzure.Storage.Table;
using System;

public class ImageEntity : TableEntity
{
    public ImageEntity(Guid eventId, Guid imageId)
    {
        this.PartitionKey = eventId.ToString();
        this.RowKey = imageId.ToString();
    }

    public ImageEntity() { }

    public int PersonCount { get; set; }

    public DateTime TimeStamp { get; set; }

    public string EventName { get; set; }
}