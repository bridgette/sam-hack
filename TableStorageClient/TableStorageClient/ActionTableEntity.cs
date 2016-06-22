using Microsoft.WindowsAzure.Storage.Table;
using System;

public class ActionTableEntity : TableEntity
{
    public ActionTableEntity(Guid eventId, Guid imageId)
    {
        this.PartitionKey = eventId.ToString();
        this.RowKey = imageId.ToString();
    }

    public ActionTableEntity() { }

    public string TimeStamp { get; set; }

    public string ActionType { get; set; }

    public string EventName { get; set; }
}

public enum ActionType
{
    PersonEnters,
    PersonExists
}