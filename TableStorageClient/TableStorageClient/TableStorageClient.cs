using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using System;
using System.Collections.Generic;
using System.Net;

namespace TableStorageClient
{
    public class TableStorageClient
    {
        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;

        public TableStorageClient(string connectionString)
        {
            storageAccount = CloudStorageAccount.Parse(connectionString);
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public void CreateTable(string tableReferenceName)
        {
            CloudTable table = tableClient.GetTableReference(tableReferenceName);
            table.CreateIfNotExists();
        }

        public TableResult InsertRecord(string tableReferenceName, TableEntity record)
        {
            CloudTable table = tableClient.GetTableReference(tableReferenceName);
            TableOperation insertOperation = TableOperation.Insert(record);
            TableResult result = table.Execute(insertOperation);

            return result;
        }

        public List<EventTable> GetEventRecordsInPartition(string tableReferenceName, string partitionName)
        {
            CloudTable table = tableClient.GetTableReference(tableReferenceName);
            TableQuery<EventTable> query = new TableQuery<EventTable>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));
            List<EventTable> entities = new List<EventTable>();

            foreach (EventTable entity in table.ExecuteQuery<EventTable>(query))
            {
                entities.Add(entity);
            }

            return entities;
        }

        public List<EventTable> GetEventRecordsByPartitionAndSubkey(string tableReferenceName, string partitionName, string subKeyName)
        {
            CloudTable table = tableClient.GetTableReference(tableReferenceName);

            TableQuery<EventTable> rangeQuery = new TableQuery<EventTable>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, subKeyName)));

            List<EventTable> entities = new List<EventTable>();

            foreach (EventTable entity in table.ExecuteQuery(rangeQuery))
            {
                entities.Add(entity);
            }

            return entities;
        }

        public List<ActionTable> GetAcionRecordsInPartition(string tableReferenceName, string partitionName)
        {
            CloudTable table = tableClient.GetTableReference(tableReferenceName);
            TableQuery<ActionTable> query = new TableQuery<ActionTable>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));
            List<ActionTable> entities = new List<ActionTable>();
            foreach (ActionTable entity in table.ExecuteQuery<ActionTable>(query))
            {
                entities.Add(entity);
            }

            return entities;
        }

        public List<ActionTable> GetActionRecordsByPartitionAndSubkey(string tableReferenceName, string partitionName, string subKeyName)
        {
            CloudTable table = tableClient.GetTableReference(tableReferenceName);
            TableQuery<ActionTable> rangeQuery = new TableQuery<ActionTable>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, subKeyName)));

            List<ActionTable> entities = new List<ActionTable>();
            foreach (ActionTable entity in table.ExecuteQuery(rangeQuery))
            {
                entities.Add(entity);
            }

            return entities;
        }
    }
}
