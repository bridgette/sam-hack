using Microsoft.WindowsAzure.Storage.Table;
using System;

public class ActionTable : TableEntity
{
    public ActionTable(Guid eventId, DateTime timeStamp)
    {
        this.PartitionKey = eventId.ToString();
        this.RowKey = timeStamp.ToLongDateString();
    }

    public ActionTable() { }

    public int CountIn { get; set; }

    public int CountOut { get; set; }
}

public enum ActionType
{
    PersonEnters,
    PersonExists
}